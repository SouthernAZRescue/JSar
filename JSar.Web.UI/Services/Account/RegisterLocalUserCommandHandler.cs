﻿using JSar.Web.UI.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JSar.Web.UI.Domain.Abstractions;
using JSar.Web.UI.Domain.Aggregates.Person;
using JSar.Web.UI.Infrastructure.Data;
using JSar.Web.UI.Infrastructure.Logging;
using JSar.Web.UI.Services.CQRS;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace JSar.Web.UI.Services.Account
{
    public class RegisterLocalUserCommandHandler : CommandHandler<RegisterLocalUser, CommonResult>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly MembershipDbContext _dbContext;
        private readonly IRepository<Person> _personRepository;

        public RegisterLocalUserCommandHandler(UserManager<AppUser> userManager, MembershipDbContext dbContext, IRepository<Person> personRepository, ILogger logger) : base (logger)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager), "Constructor parameter 'userManager' cannot be null. EID: 532F339A");
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(userManager), "Constructor parameter 'unitOfWork' cannot be null. EID: D481A958");
            _personRepository = personRepository ?? throw new ArgumentNullException(nameof(userManager), "Constructor parameter 'personRepository' cannot be null. EID: 741B7D6D");
        }
        protected override async Task<CommonResult> HandleCore(RegisterLocalUser command, CancellationToken cancellationToken)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    // Register user...
                    var addUserResult = await CreateUser(command);

                    if (!addUserResult.Succeeded)
                        return AddUserErrorResult(addUserResult, command.MessageId);

                    // ...and add associated Person aggregate
                    await CreatePerson(command);

                    transaction.Commit();

                    return new CommonResult(
                        messageId: command.MessageId,
                        outcome: Outcome.Succeeded);
                }
                catch (Exception ex)
                {
                    var errorResult = ex.CommonResultFromRequestException(command, _logger);
                    errorResult.LogCommonResultError("Error saving person during user registration", command.GetType(), _logger);
                    return errorResult;
                }
            }
        }

        private async Task<IdentityResult> CreateUser(RegisterLocalUser command)
        {
            IdentityResult addUserResult;

            if (command.Password == null)
            {
                addUserResult = await _userManager.CreateAsync(command.User);
            }
            else
            {
                 addUserResult = await _userManager.CreateAsync(command.User, command.Password);
            }

            return addUserResult;
        }

        private async Task CreatePerson(RegisterLocalUser command)
        {
            Person person = new Person(
                command.User.FirstName,
                command.User.LastName,
                command.User.Email,
                command.User.PhoneNumber,
                Guid.NewGuid());

            _personRepository.AddOrUpdate(person);

            await _dbContext.SaveChangesAsync();
        }

        private CommonResult AddUserErrorResult(IdentityResult addUserResult, Guid messageId)
        {
            var errors = new ResultErrorCollection();

            foreach (IdentityError error in addUserResult.Errors)
            {
                errors.Add(error.Code, error.Description);
            }

            CommonResult result = new CommonResult(
                messageId: messageId,
                outcome: Outcome.ExecutionFailure,
                flashMessage: "RegisterLocalUser command execution failed.",
                errors: errors
                );

            result.LogCommonResultError("User registration error", this.GetType(), _logger);

            return result;
        }
    }
}
