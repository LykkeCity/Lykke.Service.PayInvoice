﻿using System;
using System.Threading.Tasks;
using Refit;

namespace Lykke.Service.PayInvoice.Client
{
    internal class ApiRunner
    {
        public async Task RunAsync(Func<Task> method)
        {
            try
            {
                await method();
            }
            catch (ApiException exception)
            {
                throw new ErrorResponseException("An error occurred  during calling api", exception);
            }
        }

        public async Task<T> RunAsync<T>(Func<Task<T>> method)
        {
            try
            {
                return await method();
            }
            catch (ApiException exception)
            {
                throw new ErrorResponseException("An error occurred  during calling api", exception);
            }
        }
    }
}
