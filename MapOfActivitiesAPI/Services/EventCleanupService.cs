using MapOfActivitiesAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MapOfActivitiesAPI.Services
{
    public class EventCleanupService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IServiceScopeFactory _scopeFactory;

        public EventCleanupService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(RemoveExpiredEvents, null, TimeSpan.Zero, TimeSpan.FromHours(24));
            return Task.CompletedTask;
        }

        private void RemoveExpiredEvents(object state)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<MapOfActivitiesAPIContext>();

                //var eventsToDelete = context.Events.Where(e => e.EndTime < DateTime.UtcNow).ToList();

                //foreach (var eventToDelete in eventsToDelete)
                //{
                //    var complaintsToDelete = context.Complaints.Where(c => c.EventId == eventToDelete.Id);
                //    context.Complaints.RemoveRange(complaintsToDelete);
                //}

                //context.Events.RemoveRange(eventsToDelete);
                context.SaveChanges();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
