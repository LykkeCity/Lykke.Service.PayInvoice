﻿namespace Lykke.Service.PayInvoice.Models.Employee
{
    public class EmployeeModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MerchantId { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsInternalSupervisor { get; set; }
        public bool IsDeleted { get; set; }
    }
}
