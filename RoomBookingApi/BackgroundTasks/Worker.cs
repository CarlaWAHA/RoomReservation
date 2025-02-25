using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RoomBookingApi.BackgroundTasks
{
    public class Worker : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine("Worker en cours d'exécution...");
                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}
