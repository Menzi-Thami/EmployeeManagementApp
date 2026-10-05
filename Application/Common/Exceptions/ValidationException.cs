using System;
using System.Collections.Generic;

namespace EmployeeManagementApp.Application.Common.Exceptions
{
    /// <summary>
    /// Thrown when input is well-formed but breaks a business rule (for example an
    /// unknown job title). Translated to HTTP 400 ValidationProblemDetails by the
    /// web layer's GlobalExceptionHandler; MVC forms add it to ModelState instead.
    /// </summary>
    public class ValidationException : Exception
    {
        public ValidationException(string propertyName, string message)
            : base(message)
        {
            Errors = new Dictionary<string, string[]> { [propertyName] = [message] };
        }

        public IReadOnlyDictionary<string, string[]> Errors { get; }
    }
}
