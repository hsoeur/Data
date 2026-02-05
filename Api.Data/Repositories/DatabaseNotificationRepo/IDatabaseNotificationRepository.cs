using DataLib;

namespace Api.Data.Repositories.DatabaseNotificationRepo
{
    public interface IDatabaseNotificationRepository
    {
        Task<IEnumerable<DatabaseNotification>> GetAllAsync();                // 1. Lister toutes les notifications
        Task<DatabaseNotification> CreateAsync(DatabaseNotification notif);   // 2. Créer une notification
        Task<IEnumerable<DatabaseNotification>> SearchAsync(string keyword);  // 3. Rechercher une notification
        Task<DatabaseNotification?> ProcessAsync(string notificationUid, string processedBy);

    }
}
