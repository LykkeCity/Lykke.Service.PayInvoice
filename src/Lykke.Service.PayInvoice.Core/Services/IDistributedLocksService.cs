﻿using System;
using System.Threading.Tasks;

namespace Lykke.Service.PayInvoice.Core.Services
{
    public interface IDistributedLocksService
    {
        Task<bool> TryAcquireLockAsync(string key, string token, DateTime expiration);
        Task<bool> ReleaseLockAsync(string key, string token);
    }
}
