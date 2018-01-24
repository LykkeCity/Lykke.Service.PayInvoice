﻿using System;
using System.Runtime.Serialization;

namespace Lykke.Service.PayInvoice.Core.Exceptions
{
    /// <summary>
    /// The exception that is thrown when attempting to add an employee with same email.
    /// </summary>
    [Serializable]
    public class EmployeeExistException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeExistException"/> class.
        /// </summary>
        public EmployeeExistException()
            : base("Employee with same email already exists.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeExistException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        protected EmployeeExistException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeExistException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (<c>Nothing</c> in Visual Basic) if no inner exception is specified.</param>
        public EmployeeExistException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}