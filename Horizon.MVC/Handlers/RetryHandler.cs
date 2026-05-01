using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Horizon.MVC.Handlers
{
    public class RetryHandler : DelegatingHandler
    {
        private readonly int _maxRetries;

        public RetryHandler(int maxRetries = 3)
        {
            _maxRetries = maxRetries;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = null;
            for (int i = 0; i < _maxRetries; i++)
            {
                response = await base.SendAsync(request, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    return response;
                }
                
                // Wait before retrying (exponential backoff)
                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, i)), cancellationToken);
            }
            return response;
        }
    }
}
