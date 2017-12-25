﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using JSar.Web.UI.Models.AccountViewModels;
using JSar.Membership.Messages;
using JSar.Membership.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using JSar.Membership.Messages.Commands;
using JSar.Membership.Messages.Queries;
using JSar.Web.UI.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace JSar.Web.Mvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly Serilog.ILogger _logger;
        private readonly IMediator _mediator;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(ILogger logger, IMediator mediator, SignInManager<AppUser> signInManager)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        }
        
        // 
        // HTTP-GET: /Account/Register

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            _logger.Verbose("MVC request: HTTP-GET:/Account/Register");

            // TODO: Handle return URLs Register(string returnUrl = null)

            return View();
        }

        //
        // HTTP-POST: /Account/Register

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            _logger.Verbose("MVC request: HTTP-POST:/Account/Register");

            // TODO: When adding Azure AD authentication capability, detect if authentication 
            // type is OIDC and modify registration accordingly. Cache and preserve claims for  
            // login. See: AzureTest1 project for sample implementation.

            if (!ModelState.IsValid) return View(model);
            
            CommonResult userResult = await _mediator.Send(
                new RegisterLocalUser(
                    model.FirstName, 
                    model.LastName, 
                    model.PrimaryPhone, 
                    model.Email, 
                    model.Password));

            if (! userResult)
            {
                ModelState.AddErrorsFromCommonResult(userResult);
                return View(model);
            } 

            // Log the user in.

            return await SignIn(  new SignInViewModel()
            {
                UserName = model.Email,
                Password = model.Password,
                RememberMe = model.RememberMe
            } );
        }

        // 
        // HTTP-GET: /Account/SignIn(?ReturnUrl=...)

        [HttpGet]
        public async Task<IActionResult> SignIn(string ReturnUrl = null)
        {
            _logger.Verbose("MVC request: HTTP-GET:/Account/SignIn(?ReturnUrl...)");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Use this one after setting up Azure AD signin.
            // await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            
            // TODO: Resume work here, pass along ReturnUrl through the chain.

            return View();
        }

        //
        // HTTP-POST: /Account/SignIn

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(SignInViewModel model)
        {
            _logger.Verbose("MVC request: HTTP-POST:/Account/SignIn");

            // Get user.

            var getUserResult = await _mediator.Send(
                new GetUserByEmail(model.UserName));
            
            if (! getUserResult.Success)
            {
                ModelState.AddErrorsFromCommonResult(getUserResult);
                return View();
            }

            // Login user to create app cookie, which is used for authentication on next request.

            var signInResult = await _mediator.Send(
                new SignInByPassword(getUserResult.Data, model.Password, model.RememberMe, false));
            
            if (! signInResult.Success)
            {
                ModelState.AddErrorsFromCommonResult(signInResult);
                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }

        //
        // HTTP-POST: /Account/SignOut

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignOut()
        {
            _logger.Verbose("MVC request: HTTP-POST:/Account/SignOut");

            string name = User.Identity.Name;

            await _signInManager.SignOutAsync();

            _logger.Information(string.Format("User {0} logged out.", name));

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

    }
}
