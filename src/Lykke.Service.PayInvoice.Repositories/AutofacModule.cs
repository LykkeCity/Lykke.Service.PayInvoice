﻿using Autofac;
using AzureStorage;
using AzureStorage.Blob;
using AzureStorage.Tables;
using Common.Log;
using Lykke.Service.PayInvoice.Core.Repositories;
using Lykke.SettingsReader;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.PayInvoice.Repositories
{
    public class AutofacModule : Module
    {
        private readonly IReloadingManager<string> _connectionString;
        private readonly ILog _log;

        public AutofacModule(IReloadingManager<string> connectionString, ILog log)
        {
            _connectionString = connectionString;
            _log = log;
        }

        protected override void Load(ContainerBuilder builder)
        {
            const string invoicesTableName = "Invoices";
            const string invoiceFilesTableName = "InvoiceFiles";
            
            builder.RegisterInstance<IFileRepository>(
                new FileRepository(AzureBlobStorage.Create(_connectionString)));
            builder.RegisterInstance<IFileInfoRepository>(
                new FileInfoRepository(CreateTable<FileInfoEntity>(invoiceFilesTableName)));
            builder.RegisterInstance<IInvoiceRepository>(
                new InvoiceRepository(CreateTable<InvoiceEntity>(invoicesTableName)));
            builder.RegisterInstance<IInvoiceMerchantLinkRepository>(
                new InvoiceMerchantLinkRepository(CreateTable<InvoiceMerchantLinkEntity>(invoicesTableName)));
        }

        private INoSQLTableStorage<T> CreateTable<T>(string name) where T : TableEntity, new()
        {
            return AzureTableStorage<T>.Create(_connectionString, name, _log);
        }
    }
}
