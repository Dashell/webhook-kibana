namespace Webhook.Infrastructure
{
    public class ErrorResult
    {
        /// <summary>
        /// Contenu de l'erreur
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// Code de l'erreur
        /// </summary>
        public double? ErrorCode { get; set; }
        public string? StackTrace { get; set; }

        public ErrorResult(string error, double? errorCode, string? stackTrace)
        {
            Error = error;
            ErrorCode = errorCode;
            StackTrace = stackTrace;
        }
    }
}
