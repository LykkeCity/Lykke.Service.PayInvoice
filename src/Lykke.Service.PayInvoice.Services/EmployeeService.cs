﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.PayInternal.Client;
using Lykke.Service.PayInternal.Client.Exceptions;
using Lykke.Service.PayInvoice.Core.Domain;
using Lykke.Service.PayInvoice.Core.Exceptions;
using Lykke.Service.PayInvoice.Core.Repositories;
using Lykke.Service.PayInvoice.Core.Services;
using Lykke.Service.PayInvoice.Core.Utils;
using Lykke.Service.PayInvoice.Services.Extensions;

namespace Lykke.Service.PayInvoice.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IPayInternalClient _payInternalClient;
        private readonly ILog _log;

        public EmployeeService(
            IEmployeeRepository employeeRepository,
            IPayInternalClient payInternalClient,
            ILogFactory logFactory)
        {
            _employeeRepository = employeeRepository;
            _payInternalClient = payInternalClient;
            _log = logFactory.CreateLog(this);
        }

        public Task<IReadOnlyList<Employee>> GetAllAsync()
        {
            return _employeeRepository.GetAsync();
        }

        public async Task<Employee> GetByIdAsync(string employeeId)
        {
            var employee = await _employeeRepository.GetByIdAsync(employeeId);

            if (employee == null)
                throw new EmployeeNotFoundException(employeeId);

            return employee;
        }

        public async Task<Employee> GetByEmailAsync(string email)
        {
            var employee = await _employeeRepository.FindAsync(email);

            if (employee == null)
                throw new EmployeeNotFoundException();

            return employee;
        }

        public Task<IReadOnlyList<Employee>> GetByMerchantIdAsync(string merchantId)
        {
            return _employeeRepository.GetByMerchantIdAsync(merchantId);
        }

        public async Task<Employee> AddAsync(Employee employee)
        {
            try
            {
                var merchant = await _payInternalClient.GetMerchantByIdAsync(employee.MerchantId);
            }
            catch (DefaultErrorResponseException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                throw new MerchantNotFoundException(employee.MerchantId);
            }

            Employee existingEmployee = await _employeeRepository.FindAsync(employee.Email);
            
            if(existingEmployee != null)
                throw new EmployeeExistException();
                
            Employee createdEmployee = await _employeeRepository.InsertAsync(employee);

            _log.Info("Employee added.", employee.Sanitize());
            
            return createdEmployee;
        }

        public async Task UpdateAsync(Employee employee)
        {
            Employee existingEmployee = await _employeeRepository.GetAsync(employee.Id, employee.MerchantId);

            if (existingEmployee == null)
                throw new EmployeeNotFoundException();

            // check for the same email
            Employee sameEmailEmployee = await _employeeRepository.FindAsync(employee.Email);

            if (sameEmailEmployee != null)
                throw new EmployeeExistException();
                
            await _employeeRepository.UpdateAsync(employee, existingEmployee.Email);
            
            _log.Info("Employee updated.", employee.Sanitize());
        }

        public async Task DeleteAsync(string employeeId)
        {
            Employee existingEmployee = await _employeeRepository.GetByIdAsync(employeeId);

            if (existingEmployee == null)
                throw new EmployeeNotFoundException(employeeId);

            await _employeeRepository.DeleteAsync(employeeId);
            
            _log.Info("Employee deleted.", new { employeeId });
        }
    }
}
