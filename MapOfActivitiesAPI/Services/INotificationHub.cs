using MapOfActivitiesAPI.Models;

namespace MapOfActivitiesAPI.Services
{
    public interface INotificationHub
    {
        Task SendMessageToEveryone(string name, string message);
        Task SendMessageToUser(string userId, string name, string message);
        Task AddToGroup(Visitings visiting, string JoinedUserId, string сonId);
        Task RemoveFromGroup(Visitings visiting, string JoinedUserId);
        Task SendGroupNotification(int eventId, string message, string eventName);

    }
}
