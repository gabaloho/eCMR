using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace HerbavitaCMR.Services
{
    public class CustomHttpMessageHandler : DelegatingHandler
    {

        // Centralized base address (update this if the API address changes)
        public static readonly Uri BaseAddress = new Uri("https://192.168.0.137:7236/");

        public CustomHttpMessageHandler()
            : base(new HttpClientHandler
            {
#if DEBUG
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
#endif
            })
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Ensure the Accept header is set for JSON
            if (!request.Headers.Accept.Any(h => h.MediaType == "application/json"))
            {
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }

            // Send the request
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
