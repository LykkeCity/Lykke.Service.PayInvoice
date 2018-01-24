﻿namespace Lykke.Service.PayInvoice.Client.Models.Employee
{
    /// <summary>
    /// Represent a merchant employee information.
    /// </summary>
    public class EmployeeModel
    {
        /// <summary>
        /// Gets or sets an id.
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// Gets or sets an email.
        /// </summary>
        public string Email { get; set; }
        
        /// <summary>
        /// Gets or sets a first name.
        /// </summary>
        public string FirstName { get; set; }
        
        /// <summary>
        /// Gets or sets a last name.
        /// </summary>
        public string LastName { get; set; }
        
        /// <summary>
        /// Gets or sets a merchant id.
        /// </summary>
        public string MerchantId { get; set; }
    }
}