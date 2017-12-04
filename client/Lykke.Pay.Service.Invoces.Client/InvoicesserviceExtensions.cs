// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Lykke.Pay.Service.Invoces.Client
{
    using Models;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Extension methods for Invoicesservice.
    /// </summary>
    public static partial class InvoicesserviceExtensions
    {
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            public static IList<IInvoiceEntity> ApiInvoicesGet(this IInvoicesservice operations)
            {
                return operations.ApiInvoicesGetAsync().GetAwaiter().GetResult();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<IList<IInvoiceEntity>> ApiInvoicesGetAsync(this IInvoicesservice operations, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.ApiInvoicesGetWithHttpMessagesAsync(null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='invoice'>
            /// </param>
            public static bool? ApiInvoicesPost(this IInvoicesservice operations, InvoiceEntity invoice = default(InvoiceEntity))
            {
                return operations.ApiInvoicesPostAsync(invoice).GetAwaiter().GetResult();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='invoice'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<bool?> ApiInvoicesPostAsync(this IInvoicesservice operations, InvoiceEntity invoice = default(InvoiceEntity), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.ApiInvoicesPostWithHttpMessagesAsync(invoice, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='invoiceId'>
            /// </param>
            public static IInvoiceEntity ApiInvoicesByInvoiceIdGet(this IInvoicesservice operations, string invoiceId)
            {
                return operations.ApiInvoicesByInvoiceIdGetAsync(invoiceId).GetAwaiter().GetResult();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='invoiceId'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<IInvoiceEntity> ApiInvoicesByInvoiceIdGetAsync(this IInvoicesservice operations, string invoiceId, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.ApiInvoicesByInvoiceIdGetWithHttpMessagesAsync(invoiceId, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            public static IsAliveResponse IsAlive(this IInvoicesservice operations)
            {
                return operations.IsAliveAsync().GetAwaiter().GetResult();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<IsAliveResponse> IsAliveAsync(this IInvoicesservice operations, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.IsAliveWithHttpMessagesAsync(null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

    }
}
