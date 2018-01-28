﻿using System;

namespace Lykke.Service.PayInvoice.Core.Domain
{
    public interface IInvoiceDetails : IInvoice
    {
        string WalletAddress { get; }
        double PaymentAmount { get; }
        DateTime OrderDueDate { get; }
        DateTime OrderCreatedDate { get; }
    }
}