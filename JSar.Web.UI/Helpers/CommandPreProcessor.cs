﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JSar.Membership.Messages;
using JSar.Membership.Messages.Commands;
using JSar.Membership.Messages.Commands.Identity;
using JSar.Membership.Messages.Queries.Identity;
using JSar.Membership.Services.CommandHandlers;
using MediatR;
using MediatR.Pipeline;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace JSar.Web.UI.Helpers
{
    public class CommandPreProcessor<TRequest> : IRequestPreProcessor<TRequest> // where TRequest : ICommand<ICommonResult>
    {
        public Task Process(TRequest request, CancellationToken cancellationToken)
        {
            Debug.WriteLine("***** COMMAND PREPROCESSOR CALLED *****");
            Debug.WriteLine("     * Type of request: " + typeof(TRequest));

            return Task.CompletedTask;
        }
    }
}