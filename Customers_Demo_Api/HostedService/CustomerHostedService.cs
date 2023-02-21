using Customers_Demo_Service.Service;

namespace Customers_Demo_Api.HostedService
{
    public class CustomerHostedService : IHostedService, IDisposable
    {
        private Timer? timer;

        private readonly ICustomerService _customerService;
        public CustomerHostedService(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(Do, null, 0, Timeout.Infinite);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private void Do(object? state)
        {
            while (true)
            {
                Thread.Sleep(10);
                _customerService.AddLeaderboards();
            }
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}
