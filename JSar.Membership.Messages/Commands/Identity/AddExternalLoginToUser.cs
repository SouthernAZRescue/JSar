﻿using JSar.Membership.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using JSar.Membership.Messages.Results;

namespace JSar.Membership.Messages.Commands.Identity
{
    public class AddExternalLoginToUser : Command<CommonResult>
    {
        public AddExternalLoginToUser(AppUser user, ExternalLoginInfo info, Guid messageId = default(Guid)) : base(messageId)
        {
        }

        public AppUser User { get; }
        
        public ExternalLoginInfo LoginInfo { get; }
    }
}
