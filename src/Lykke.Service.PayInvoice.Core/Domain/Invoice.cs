﻿using System;

namespace Lykke.Service.PayInvoice.Core.Domain
{
    /// <summary>
    /// Represents an invoice.
    /// </summary>
    public class Invoice
    {
        /// <summary>
        /// The unique identified of the invoice.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The invoice number which provided when it created.
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// The client name.
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// The client email address.
        /// </summary>
        public string ClientEmail { get; set; }

        /// <summary>
        /// The invoice amount.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// The invoice due date in UTC.
        /// </summary>
        public DateTime DueDate { get; set; }

        /// <summary>
        /// The invoice status.
        /// </summary>
        public InvoiceStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the settlement asset id.
        /// </summary>
        public string SettlementAssetId { get; set; }

        /// <summary>
        /// Gets or sets the payment asset id.
        /// </summary>
        public string PaymentAssetId { get; set; }

        /// <summary>
        /// Gets or sets the payment request id.
        /// </summary>
        public string PaymentRequestId { get; set; }

        /// <summary>
        /// Gets or sets the wallet address.
        /// </summary>
        public string WalletAddress { get; set; }

        /// <summary>
        /// The identifier of merchant.
        /// </summary>
        public string MerchantId { get; set; }

        /// <summary>
        /// The identifier of the merchant employee which created invoice.
        /// </summary>
        public string EmployeeId { get; set; }

        /// <summary>
        /// The additional information.
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// The date of creation in UTC.
        /// </summary>
        public DateTime CreatedDate { get; set; }
    }
}