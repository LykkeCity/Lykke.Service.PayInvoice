﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Common.Log;
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
        private readonly ILog _log;

        public EmployeeService(
            IEmployeeRepository employeeRepository,
            ILogFactory logFactory)
        {
            _employeeRepository = employeeRepository;
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

        public Task<Employee> GetByEmailAsync(string email)
        {
            return _employeeRepository.FindAsync(email);
        }

        public Task<IReadOnlyList<Employee>> GetByMerchantIdAsync(string merchantId)
        {
            return _employeeRepository.GetByMerchantIdAsync(merchantId);
        }

        public async Task<Employee> AddAsync(Employee employee)
        {
            Employee existingEmployee = await _employeeRepository.FindAsync(employee.Email);
            
            if(existingEmployee != null)
                throw new EmployeeExistException();
                
            Employee createdEmployee = await _employeeRepository.InsertAsync(employee);

            _log.Info("Employee added.", employee.Sanitize());
            
            return createdEmployee;
        }

        public async Task UpdateAsync(Employee employee)
        {
            Employee existingEmployee = await _employeeRepository.GetByIdAsync(employee.Id);
            
            if(existingEmployee == null)
                throw new EmployeeNotFoundException();
                
            await _employeeRepository.UpdateAsync(employee);
            
            _log.Info("Employee updated.", employee.Sanitize());
        }

        public async Task DeleteAsync(string employeeId)
        {
            await _employeeRepository.DeleteAsync(employeeId);
            
            _log.Info("Employee deleted.", new { employeeId });
        }
    }
}
