﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AzureStorage;
using AzureStorage.Tables.Templates.Index;
using Lykke.Service.PayInvoice.Core.Domain;
using Lykke.Service.PayInvoice.Core.Repositories;
using Lykke.Service.PayInvoice.Repositories.Extensions;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.PayInvoice.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private ConcurrentDictionary<string, string> partitionKeyByRowKey = new ConcurrentDictionary<string, string>();
        private readonly INoSQLTableStorage<InvoiceEntity> _storage;
        private readonly INoSQLTableStorage<AzureIndex> _invoiceIdIndexStorage;
        private readonly INoSQLTableStorage<AzureIndex> _paymentRequestIdIndexStorage;
        private readonly InvoiceEntity Entity = new InvoiceEntity();

        public InvoiceRepository(
            INoSQLTableStorage<InvoiceEntity> storage,
            INoSQLTableStorage<AzureIndex> invoiceIdIndexStorage,
            INoSQLTableStorage<AzureIndex> paymentRequestIdIndexStorage)
        {
            _storage = storage;
            _invoiceIdIndexStorage = invoiceIdIndexStorage;
            _paymentRequestIdIndexStorage = paymentRequestIdIndexStorage;
        }

        public async Task<IReadOnlyList<Invoice>> GetAsync()
        {
            IList<InvoiceEntity> entities = await _storage.GetDataAsync();

            return Mapper.Map<List<Invoice>>(entities);
        }

        public async Task<IReadOnlyList<Invoice>> GetAllPaidAsync()
        {
            var filter = StatusEqual(InvoiceStatus.Paid)
                .Or(StatusEqual(InvoiceStatus.Overpaid))
                .Or(StatusEqual(InvoiceStatus.Underpaid))
                .Or(StatusEqual(InvoiceStatus.LatePaid));

            var tableQuery = new TableQuery<InvoiceEntity>().Where(filter);

            var result = await _storage.WhereAsync(tableQuery);

            return Mapper.Map<List<Invoice>>(result);

            string StatusEqual(InvoiceStatus status)
            {
                return nameof(Entity.Status).PropertyEqual(status.ToString());
            }
        }

        public async Task<IReadOnlyList<Invoice>> GetAsync(string merchantId)
        {
            IEnumerable<InvoiceEntity> entities = await _storage.GetDataAsync(GetPartitionKey(merchantId));

            return Mapper.Map<List<Invoice>>(entities);
        }
        
        public async Task<Invoice> GetAsync(string merchantId, string invoiceId)
        {
            InvoiceEntity entity = await _storage.GetDataAsync(GetPartitionKey(merchantId), GetRowKey(invoiceId));

            return Mapper.Map<Invoice>(entity);
        }

        public async Task<IReadOnlyList<Invoice>> GetByIdsAsync(string merchantId, IEnumerable<string> invoiceIds)
        {
            var filter = nameof(Entity.PartitionKey).PropertyEqual(GetPartitionKey(merchantId));

            var localFilter = string.Empty;
            foreach (var invoiceId in invoiceIds)
            {
                localFilter = localFilter.OrIfNotEmpty(nameof(Entity.RowKey).PropertyEqual(invoiceId));
            }
            filter = filter.AndIfNotEmpty(localFilter);

            var tableQuery = new TableQuery<InvoiceEntity>().Where(filter);

            var result = await _storage.WhereAsync(tableQuery);

            return Mapper.Map<List<Invoice>>(result);
        }

        public async Task<IReadOnlyList<Invoice>> GetByFilterAsync(InvoiceFilter invoiceFilter)
        {
            var filter = string.Empty;

            if (invoiceFilter.MerchantIds.Any())
            {
                var localFilter = string.Empty;
                foreach (var merchantId in invoiceFilter.MerchantIds)
                {
                    localFilter = localFilter.OrIfNotEmpty(nameof(Entity.PartitionKey).PropertyEqual(GetPartitionKey(merchantId)));
                }
                filter = localFilter;
            }

            if (invoiceFilter.ClientMerchantIds.Any())
            {
                var localFilter = string.Empty;
                foreach (var clientMerchantId in invoiceFilter.ClientMerchantIds)
                {
                    localFilter = localFilter.OrIfNotEmpty(nameof(Entity.ClientName).PropertyEqual(clientMerchantId));
                }
                filter = filter.AndIfNotEmpty(localFilter);
            }

            if(invoiceFilter.Statuses.Any())
            {
                var localFilter = string.Empty;
                foreach (var status in invoiceFilter.Statuses)
                {
                    localFilter = localFilter.OrIfNotEmpty(nameof(Entity.Status).PropertyEqual(status.ToString()));
                }
                filter = filter.AndIfNotEmpty(localFilter);
            }

            if(invoiceFilter.Dispute)
            {
                filter = filter.AndIfNotEmpty(nameof(Entity.Dispute).PropertyEqual(true));
            }

            if (invoiceFilter.BillingCategories.Any())
            {
                var localFilter = string.Empty;
                foreach (var cat in invoiceFilter.BillingCategories)
                {
                    if (cat.IsEmpty()) continue;
                    localFilter = localFilter.OrIfNotEmpty(nameof(Entity.BillingCategory).PropertyEqual(cat));
                }
                filter = filter.AndIfNotEmpty(localFilter);
            }

            if (filter.IsEmpty())
            {
                filter = nameof(Entity.RowKey).PropertyNotEqual(GetInvoiceIdIndexRowKey())
                    .And(nameof(Entity.RowKey).PropertyNotEqual(GetPaymentRequestIndexRowKey()));
            }

            var tableQuery = new TableQuery<InvoiceEntity>().Where(filter);

            var result = await _storage.WhereAsync(tableQuery);

            return Mapper.Map<List<Invoice>>(result);
        }

        public async Task<Invoice> FindByIdAsync(string invoiceId)
        {
            string partitionKey;
            string rowKey = GetRowKey(invoiceId);

            if (!partitionKeyByRowKey.TryGetValue(rowKey, out partitionKey))
            {
                AzureIndex index =
                await _invoiceIdIndexStorage.GetDataAsync(GetInvoiceIdIndexPartitionKey(invoiceId), GetInvoiceIdIndexRowKey());

                if (index == null)
                    return null;

                partitionKey = index.PrimaryPartitionKey;
                partitionKeyByRowKey.TryAdd(rowKey, partitionKey);
            }

            InvoiceEntity entity = await _storage.GetDataAsync(partitionKey, rowKey);

            var invoice = Mapper.Map<Invoice>(entity);

            return invoice;
        }

        public async Task<Invoice> FindByPaymentRequestIdAsync(string paymentRequestId)
        {
            AzureIndex index =
                await _paymentRequestIdIndexStorage.GetDataAsync(GetPaymentRequestIndexPartitionKey(paymentRequestId), GetPaymentRequestIndexRowKey());

            if (index == null)
                return null;

            InvoiceEntity entity = await _storage.GetDataAsync(index);
            
            var invoice = Mapper.Map<Invoice>(entity);

            return invoice;
        }
        
        public async Task<Invoice> InsertAsync(Invoice invoice)
        {
            var entity = new InvoiceEntity(GetPartitionKey(invoice.MerchantId), CreateRowKey());

            Mapper.Map(invoice, entity);

            await _storage.InsertAsync(entity);

            await InsertInvoiceIdIndexAsync(entity);
            await InsertPaymentRequestIdIndexAsync(entity);

            return Mapper.Map<Invoice>(entity);
        }

        public async Task UpdateAsync(Invoice invoice)
        {
            var entity = new InvoiceEntity(GetPartitionKey(invoice.MerchantId), GetRowKey(invoice.Id));

            Mapper.Map(invoice, entity);

            entity.ETag = "*";

            await _storage.ReplaceAsync(entity);
            
            await InsertPaymentRequestIdIndexAsync(entity);
        }

        public async Task SetStatusAsync(string merchantId, string invoiceId, InvoiceStatus status)
        {
            await _storage.MergeAsync(GetPartitionKey(merchantId), GetRowKey(invoiceId), entity =>
            {
                entity.Status = status.ToString();
                return entity;
            });
        }

        public async Task SetPaidAmountAsync(string merchantId, string invoiceId, decimal paidAmount)
        {
            await _storage.MergeAsync(GetPartitionKey(merchantId), GetRowKey(invoiceId), entity =>
            {
                entity.PaidAmount = paidAmount;
                return entity;
            });
        }

        public async Task DeleteAsync(string merchantId, string invoiceId)
        {
            InvoiceEntity entity = await _storage.GetDataAsync(GetPartitionKey(merchantId), GetRowKey(invoiceId));
            
            if(entity == null)
                return;

            await _storage.DeleteAsync(entity);

            await DeleteInvoiceIdIndexAsync(entity);
            await DeletePaymentRequestIdIndexAsync(entity);
        }

        private async Task InsertInvoiceIdIndexAsync(InvoiceEntity entity)
        {
            var index = AzureIndex.Create(GetInvoiceIdIndexPartitionKey(entity.RowKey), GetInvoiceIdIndexRowKey(), entity);
            
            await _invoiceIdIndexStorage.InsertOrReplaceAsync(index);
        }
        
        private async Task InsertPaymentRequestIdIndexAsync(InvoiceEntity entity)
        {
            if (string.IsNullOrEmpty(entity.PaymentRequestId))
                return;
            
            var index = AzureIndex.Create(GetPaymentRequestIndexPartitionKey(entity.PaymentRequestId), GetPaymentRequestIndexRowKey(), entity);
            
            await _paymentRequestIdIndexStorage.InsertOrReplaceAsync(index);
        }

        private async Task DeleteInvoiceIdIndexAsync(InvoiceEntity entity)
        {
            await _invoiceIdIndexStorage.DeleteAsync(GetInvoiceIdIndexPartitionKey(entity.RowKey), GetInvoiceIdIndexRowKey());
        }

        private async Task DeletePaymentRequestIdIndexAsync(InvoiceEntity entity)
        {
            if (string.IsNullOrEmpty(entity.PaymentRequestId))
                return;
            
            await _paymentRequestIdIndexStorage.DeleteAsync(GetPaymentRequestIndexPartitionKey(entity.PaymentRequestId), GetPaymentRequestIndexRowKey());
        }
        
        private static string GetPartitionKey(string merchantId)
            => merchantId;

        private static string GetRowKey(string invoiceId)
            => invoiceId;
        
        private static string CreateRowKey()
            => Guid.NewGuid().ToString("D");
        
        private static string GetInvoiceIdIndexPartitionKey(string invoiceId)
            => invoiceId;

        private static string GetInvoiceIdIndexRowKey()
            => "InvoiceIdIndex";
        
        private static string GetPaymentRequestIndexPartitionKey(string paymentRequestId)
            => paymentRequestId;

        private static string GetPaymentRequestIndexRowKey()
            => "PaymentRequestIdIndex";
    }
}
