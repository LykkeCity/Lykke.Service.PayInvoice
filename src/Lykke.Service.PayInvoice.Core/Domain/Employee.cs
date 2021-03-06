﻿namespace Lykke.Service.PayInvoice.Core.Domain
{
    /// <summary>
    /// Represents an employee profile.
    /// </summary>
    public class Employee
    {
        /// <summary>
        /// The employee id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The employee email.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// The employee first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// The employee last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// The employee merchant id.
        /// </summary>
        public string MerchantId { get; set; }
        /// <summary>
        /// The employee block status
        /// </summary>
        public bool IsBlocked { get; set; }
        /// <summary>
        /// The employee internal supervisor attribute
        /// </summary>
        public bool IsInternalSupervisor { get; set; }

        /// <summary>
        /// The employee deleted attribute
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Makes a shallow copy of the employee
        /// </summary>
        /// <returns></returns>
        public Employee ShallowCopy()
        {
            return (Employee) MemberwiseClone();
        }
    }
}
