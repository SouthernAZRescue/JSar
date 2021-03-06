﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using JSar.Web.UI.Services.CQRS;
using MediatR;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Libuv.Internal.Networking;
using Serilog;

namespace JSar.Web.UI.Infrastructure.Logging
{
    /// <summary>
    /// Does logging for the "Handle message" side of the mediator.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class RequestHandlerPipelineLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : CommonResult
    {
        private readonly ILogger _logger;

        public RequestHandlerPipelineLoggingBehavior(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            string requestKind = "REQUEST (Unk type)";

            if (request.GetType().GetInterfaces().Contains(typeof(ICommand<CommonResult>)))
                requestKind = "COMMAND";

            if (request.GetType().GetInterfaces().Contains(typeof(IQuery<CommonResult>)))
                requestKind = "QUERY";

            _logger.Debug(
                "{0:l}: {1:l}, handling, MID: {2:l}, Type: {3:l} ",
                requestKind,
                request.GetType().Name, 
                ((IMessage)request).MessageId.ToString(), 
                request.GetType().FullName);

            return next();
        }
    }
}
