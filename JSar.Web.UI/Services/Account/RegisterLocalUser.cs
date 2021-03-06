﻿using JSar.Web.UI.Domain.Identity;
using System;
using JSar.Web.UI.Services.CQRS;

namespace JSar.Web.UI.Services.Account
{
    /// <summary>
    /// Register a local user with the Identity system. If successful the returned CommonResult.Data 
    /// contains a single AppUser object. On failure CommonResult.Data contains a List<string> 
    /// of error messages and FlashMessage contains a general error notice string.
    /// </summary>
    public class RegisterLocalUser : CommandBase<CommonResult>
    {
        public RegisterLocalUser(AppUser user, Guid messageId = default(Guid)) 
            : base(messageId)
        {
            User = user;
        }

        public RegisterLocalUser( AppUser user, string password, Guid messageId = default(Guid))
            : this(user, messageId)
        {
            Password = password;
        }

        public AppUser User { get; }
        public string Password { get; }
    }
}
