﻿using System.ComponentModel.DataAnnotations;

namespace Lykke.Service.PayInvoice.Models.Employee
{
    public class UpdateEmployeeModel
    {
        [Required]
        public string EmployeeId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
        
        [Required]
        public string MerchantId { get; set; }
    }
}