﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Pay.Service.Invoces.Core.Domain;
using Lykke.Pay.Service.Invoces.Core.Services;
using Lykke.Pay.Service.Invoces.Repositories;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Pay.Service.Invoces.Controllers
{
    /// <summary>
    /// Controller for assets
    /// </summary>
    [Route("api/[controller]")]
    public class InvoicesController : Controller
    {
        private readonly IInvoiceService<IInvoiceEntity> _service;

        public InvoicesController(IInvoiceService<IInvoiceEntity> service)
        {
            _service = service;
        }

        [HttpPost]
        [SwaggerOperation("SaveInvoice")]
        public async Task<bool> SaveInvoice([FromBody]InvoiceEntity invoice)
        {
            return await _service.SaveInvoice(invoice);
        }

        [HttpGet]
        [SwaggerOperation("GetInvoices")]
        public async Task<List<IInvoiceEntity>> GetInvoices(string merchantId)
        {
            return await _service.GetInvoices(merchantId);
        }

        [HttpGet("{invoiceId}")]
        [SwaggerOperation("GetInvoiceById")]
        public async Task<IInvoiceEntity> GetInvoice(string merchantId, string invoiceId)
        {
            return await _service.GetInvoice(merchantId, invoiceId);
        }

        [HttpGet("{invoiceId}/delete")]
        [SwaggerOperation("DeleteInvoice")]
        public async Task DeleteInvoice(string merchantId, string invoiceId)
        {
            await _service.DeleteInvoice(merchantId, invoiceId);
        }

        [HttpGet("address/{address}")]
        [SwaggerOperation("GetInvoiceByAddress")]
        public async Task<IInvoiceEntity> GetInvoiceByAddress(string address)
        {
            return await _service.GetInvoiceByAddress(address);
        }

        [HttpGet("files/{invoiceId}")]
        public async Task<List<IFileMetaEntity>> GetFileMetaList(string invoiceId)
        {
            return await _service.GetFileMetaList(invoiceId);
        }

        [HttpGet("file/{fileId}/ofInvoice/{invoiceId}")]
        public async Task<IFileEntity> GetFileEntity(string invoiceId, string fileId)
        {
            return await _service.GetFileEntity(invoiceId, fileId);
        }

        [HttpPost("uploadFile")]
        public async Task UploadFile([FromBody]FileEntity entity)
        {
            await _service.UploadFile(entity);
        }
    }

}