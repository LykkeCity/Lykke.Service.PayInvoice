﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.PayCallback.Client.InvoiceConfirmation;
using Lykke.Service.PayInvoice.Core.Domain.InvoiceConfirmation;
using Lykke.Service.PayInvoice.Core.Domain.PushNotification;
using Lykke.Service.PayInvoice.Core.Services;
using Lykke.Service.PayInvoice.Core.Settings;
using Lykke.Service.PayPushNotifications.Client.Publisher;
using Polly;
using Polly.Retry;

namespace Lykke.Service.PayInvoice.Services
{
    public class PushNotificationService : IPushNotificationService
    {
        private readonly NotificationPublisher _pushNotificationPublisher;
        private readonly RetryPolicy _retryPolicy;
        private readonly ILog _log;

        public PushNotificationService(
            NotificationPublisher pushNotificationPublisher,
            RetryPolicySettings retryPolicySettings,
            ILogFactory logFactory)
        {
            _pushNotificationPublisher = pushNotificationPublisher;
            _log = logFactory.CreateLog(this);
            _retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    retryPolicySettings.DefaultAttempts,
                    attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                    (ex, timespan) => _log.Error(ex, "Publish confirmations to callback with retry"));
        }

        public async Task PublishDisputeCancelled(DisputeCancelledPushNotificationCommand command)
        {
            var notificationMessage = new NotificationMessage
            {
                MerchantIds = new string[] { command.NotifiedMerchantId },
                Message = "Dispute cancelled."
            };

            await _retryPolicy.ExecuteAsync(() => _pushNotificationPublisher.PublishAsync(notificationMessage));

            _log.Info("Information sent to push notifications service", new { notificationMessage });
        }

        public async Task PublishDisputeRaised(DisputeRaisedPushNotificationCommand command)
        {
            var notificationMessage = new NotificationMessage
            {
                MerchantIds = new string[] { command.NotifiedMerchantId },
                Message = "Dispute raised."
            };

            await _retryPolicy.ExecuteAsync(() => _pushNotificationPublisher.PublishAsync(notificationMessage));

            _log.Info("Information sent to push notifications service", new { notificationMessage });
        }

        public async Task PublishInvoicePayment(InvoicePaidPushNotificationCommand command)
        {
            var notificationMessage = new NotificationMessage
            {
                MerchantIds = new string[] { command.NotifiedMerchantId },
                Message = $"Invoice has been paid by {command.PayerMerchantName}: {command.PaidAmount} USD received."
            };

            await _retryPolicy.ExecuteAsync(() => _pushNotificationPublisher.PublishAsync(notificationMessage));

            _log.Info("Information sent to push notifications service", new { notificationMessage });
        }
    }
}
