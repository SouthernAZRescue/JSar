using System;

namespace JSar.Web.UI.Features.Shared
{
    public class ErrorViewModel
    {
        public string CorrelationId { get; set; }

        public bool ShowCorrelationId => !string.IsNullOrEmpty(CorrelationId);

        public string Message { get; set; }

        public bool ShowMessage => !string.IsNullOrEmpty(Message);
    }
}