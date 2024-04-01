using MapOfActivitiesAPI.Models;
using Microsoft.AspNetCore.SignalR;
using System.Timers;

namespace MapOfActivitiesAPI.Services
{
    public class NotificationHub : Hub, INotificationHub
    {
        private MapOfActivitiesAPIContext _context;
        private System.Timers.Timer notificationTimer;
        public NotificationHub(MapOfActivitiesAPIContext context) 
        { 
            _context = context;
            notificationTimer = new System.Timers.Timer(5000);
            notificationTimer.Elapsed += OnTimerElapsed;
            notificationTimer.AutoReset = true;
            notificationTimer.Start();
        }
        private async void OnTimerElapsed(object source, ElapsedEventArgs e)
        {
            var e5 = _context.Events.Where(x => x.StartTime <= DateTime.Now.AddMinutes(30) && x.StartTime >= DateTime.Now.AddMinutes(30).AddSeconds(5)).Select(x => x).FirstOrDefault();
            var e30 = _context.Events.Where(x => x.StartTime <= DateTime.Now.AddMinutes(5) && x.StartTime >= DateTime.Now.AddMinutes(5).AddSeconds(5)).Select(x => x).FirstOrDefault();
            if ( e5 != null)
            {
                await SendGroupNotification(e5.Id, "5 хв до початку", e5.Name);
            }
            else if (e30 != null)
            {
                await SendGroupNotification(e30.Id, "5 хв до початку", e30.Name);
            }
        }
        public override async Task OnConnectedAsync()
        {
            await SendMessageToEveryone("F", Context.ConnectionId);
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
        public async Task SendMessageToEveryone(string name, string message)
        {
            if (Context.ConnectionId != null)
            {
                await Clients.All.SendAsync("SendMessageToEveryone", name, message);
            }
        }
        public async Task SendMessageToUser(string userId, string name, string message)
        {
            await Clients.User(userId).SendAsync("SendMessageToUser", name, message);
        }
        public async Task AddToGroup(Visitings visiting, string JoinedUserId, string сonId)
        {
            Connection conection = new Connection();
            conection.ConnectionId = сonId;
            if (conection.VisitingsList == null){conection.VisitingsList = new List<Visitings>();}
            else{conection.VisitingsList.Add(visiting);}
            _context.Connections.Add(conection);
            await _context.SaveChangesAsync();
            var eventsUserIdAndName = _context.Events.Where(x => x.Id == visiting.EventId).Select(x => new{ x.UserId, x.Name}).FirstOrDefault();
            await Groups.AddToGroupAsync(сonId, visiting.EventId.ToString());
            var authorVisit = _context.Visitings.Where(x => x.UserId == eventsUserIdAndName.UserId).Select(x => x).FirstOrDefault();
            var connectionId = _context.Connections.Where(x => x.VisitingsList.Contains(authorVisit)).Select(x => x.ConnectionId).FirstOrDefault();
            await Clients.User(connectionId).SendAsync("AddToGroup", eventsUserIdAndName.Name, JoinedUserId);
        }
        public async Task RemoveFromGroup(Visitings visiting, string JoinedUserId)
        {
            var userConnectionId = _context.Connections.Where(x => x.VisitingsList.Contains(visiting)).Select(x => x.ConnectionId).FirstOrDefault();
            var eventsUserIdAndName = _context.Events.Where(x => x.Id == visiting.EventId).Select(x => new { x.UserId, x.Name }).FirstOrDefault();
            await Groups.RemoveFromGroupAsync(userConnectionId, visiting.EventId.ToString());
            var authorVisit = _context.Visitings.Where(x => x.UserId == eventsUserIdAndName.UserId).Select(x => x).FirstOrDefault();
            var connectionId = _context.Connections.Where(x => x.VisitingsList.Contains(authorVisit)).Select(x => x.ConnectionId).FirstOrDefault();
            await Clients.User(connectionId).SendAsync("RemoveFromGroup", eventsUserIdAndName.Name, JoinedUserId);
        }
        public async Task SendGroupNotification(int eventId, string notificationType, string eventName)
        {
            string message = ""; 
            switch (notificationType)
            {
                case "5 хв до початку":
                    message = "До початку заходу залишилось 5 хвилин.";
                    break;
                case "30 хв до початку":
                    message = "До початку заходу залишилось 30 хвилин.";
                    break;
                case "подію було змінено":
                    message = "Подію було успішно змінено.";
                    break;
                case "подію було видалено":
                    message = "Подію було видалено з системи.";
                    break;
                default:
                    message = "Невідоме повідомлення.";
                    break;
            }
            await Clients.Group(eventId.ToString()).SendAsync("SendGroupNotification", message, eventName);
        }
    }
}
