using System;

namespace EmployeeManagementApp.Application.Common.Exceptions
{
    /// <summary>
    /// Thrown when a requested resource does not exist. Translated to an
    /// HTTP 404 ProblemDetails response by the web layer's GlobalExceptionHandler.
    /// </summary>
    public class NotFoundException : Exception
    {
        public NotFoundException()
        {
        }

        public NotFoundException(string message)
            : base(message)
        {
        }

        public NotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
