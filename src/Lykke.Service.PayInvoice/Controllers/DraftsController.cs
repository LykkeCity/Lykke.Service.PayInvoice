﻿using System;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Common.Log;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Common.Log;
using Lykke.Service.PayInvoice.Core.Domain;
using Lykke.Service.PayInvoice.Core.Exceptions;
using Lykke.Service.PayInvoice.Core.Extensions;
using Lykke.Service.PayInvoice.Core.Services;
using Lykke.Service.PayInvoice.Extensions;
using Lykke.Service.PayInvoice.Models.Invoice;
using Lykke.Service.PayInvoice.Validation;
using LykkePay.Common.Validation;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.PayInvoice.Controllers
{
    [Route("api/[controller]")]
    public class DraftsController : Controller
    {
        private readonly IInvoiceService _invoiceService;
        private readonly ILog _log;

        public DraftsController(
            IInvoiceService invoiceService, 
            ILogFactory logFactory)
        {
            _invoiceService = invoiceService;
            _log = logFactory.CreateLog(this);
        }

        /// <summary>
        /// Creates draft invoice.
        /// </summary>
        /// <param name="model">The model that describe an invoice.</param>
        /// <response code="200">Created draft invoice.</response>
        /// <response code="400">Invalid model.</response>
        [HttpPost]
        [SwaggerOperation("DraftsAdd")]
        [ProducesResponseType(typeof(InvoiceModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ValidateModel]
        public async Task<IActionResult> CreateAsync([FromBody] CreateInvoiceModel model)
        {
            try
            {
                model.ValidateDueDate();

                Invoice invoice = await _invoiceService.CreateDraftAsync(Mapper.Map<Invoice>(model));

                return Ok(Mapper.Map<InvoiceModel>(invoice));
            }
            catch (InvoiceDueDateException ex)
            {
                _log.WarningWithDetails(ex.Message, model.Sanitize());

                return BadRequest(ErrorResponse.Create(ex.Message));
            }
        }

        /// <summary>
        /// Updates draft invoice.
        /// </summary>
        /// <param name="model">The model that describe an invoice.</param>
        /// <response code="204">Invoice successfully updated.</response>
        /// <response code="400">Invalid model.</response>
        /// <response code="404">Invoice not found.</response>
        [HttpPut]
        [SwaggerOperation("InvoicesUpdateDraft")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ValidateModel]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateInvoiceModel model)
        {
            try
            {
                model.ValidateDueDate();

                var invoice = Mapper.Map<Invoice>(model);

                await _invoiceService.UpdateDraftAsync(invoice);

                return NoContent();
            }
            catch (InvoiceDueDateException ex)
            {
                _log.WarningWithDetails(ex.Message, model.Sanitize());

                return BadRequest(ErrorResponse.Create(ex.Message));
            }
            catch (InvoiceNotFoundException ex)
            {
                _log.WarningWithDetails(ex.Message, model.Sanitize());

                return NotFound(ErrorResponse.Create("Invoice with such Id and MerchantId not found"));
            }
            catch (InvalidOperationException ex)
            {
                _log.WarningWithDetails(ex.Message, model.Sanitize());

                return BadRequest(ErrorResponse.Create(ex.Message));
            }
        }
    }
}
