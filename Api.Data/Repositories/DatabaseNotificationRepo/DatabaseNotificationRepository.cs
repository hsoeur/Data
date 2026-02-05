using DataLib;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Api.Data.Repositories.DatabaseNotificationRepo
{
    public class DatabaseNotificationRepository : IDatabaseNotificationRepository
    {
        private readonly AppDbContext _context;

        public DatabaseNotificationRepository(AppDbContext context, ILogger<DatabaseNotificationRepository> logger)
        {
            _context = context;
        }

        // 1. Lister toutes les notifications
        public async Task<IEnumerable<DatabaseNotification>> GetAllAsync()
        {
            try
            {
                return await _context.DatabaseNotifications
                                     .OrderByDescending(n => n.ModifiedAt)
                                     .ToListAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[DatabaseNotificationRepository] Erreur lors de la récupération de toutes les notifications");
                return Enumerable.Empty<DatabaseNotification>();
            }
        }

        // 2. Créer une notification
        public async Task<DatabaseNotification> CreateAsync(DatabaseNotification notif)
        {
            try
            {
                // Vérifier si la notification existe déjà
                var exists = await _context.DatabaseNotifications.AnyAsync(n =>
                    n.EntityType == notif.EntityType &&
                    n.EntityUId == notif.EntityUId &&
                    n.PropertyName == notif.PropertyName &&
                    n.OldValue == notif.OldValue &&
                    n.NewValue == notif.NewValue
                );

                if (exists)
                {
                    Log.Warning("[DatabaseNotificationRepository] Notification déjà existante - pas d'insertion (EntityType={EntityType}, EntityUid={EntityUid}, Property={Property}, Old={Old}, New={New})",
                        notif.EntityType, notif.EntityUId, notif.PropertyName, notif.OldValue, notif.NewValue);

                    return notif; // Retourne l’objet sans insertion
                }

                _context.DatabaseNotifications.Add(notif);
                await _context.SaveChangesAsync();

                Log.Information("[DatabaseNotificationRepository] Notification créée avec succès (Id={DatabaseNotificationUid})", notif.DatabaseNotificationUid);

                return notif;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[DatabaseNotificationRepository] Erreur lors de la création d'une notification (EntityType={EntityType}, EntityUid={EntityUid})", notif.EntityType, notif.EntityUId);
                throw;
            }
        }


        // 3. Rechercher une notification
        public async Task<IEnumerable<DatabaseNotification>> SearchAsync(string keyword)
        {
            try
            {
                var results = await _context.DatabaseNotifications
                                            .Where(n =>
                                                n.EntityType.Contains(keyword) ||
                                                n.EntityUId.Contains(keyword) ||
                                                n.PropertyName.Contains(keyword) ||
                                                n.ModifiedBy.Contains(keyword)

                                            )
                                            .OrderByDescending(n => n.ModifiedAt)
                                            .ToListAsync();

                Log.Information("[DatabaseNotificationRepository] Recherche effectuée avec le mot clé '{Keyword}' - {Count} résultats", keyword, results.Count);

                return results;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[DatabaseNotificationRepository] Erreur lors de la recherche de notifications avec le mot-clé '{Keyword}'", keyword);
                return Enumerable.Empty<DatabaseNotification>();
            }
        }

        public async Task<DatabaseNotification?> ProcessAsync(string notificationUid, string processedBy)
        {
            try
            {
                var notif = await _context.DatabaseNotifications
                                          .FirstOrDefaultAsync(n => n.DatabaseNotificationUid == notificationUid);

                if (notif == null)
                {
                    Log.Warning("[DatabaseNotificationRepository] Impossible de traiter la notification {Uid} : non trouvée", notificationUid);
                    return null;
                }

                // ⚠️ Exemple : mise à jour dans une table "ManufacturingOrders"
                if (notif.EntityType == "ManufacturingOrder")
                {
                    // Convertir EntityUId (string) en int pour comparer avec ManufacturingOrderUid (int)
                    int moUid;
                    if (!int.TryParse(notif.EntityUId, out moUid))
                    {
                        Log.Warning("[DatabaseNotificationRepository] EntityUId invalide pour ManufacturingOrder: {EntityUId}", notif.EntityUId);
                        return null;
                    }

                    var order = await _context.ManufacturingOrders
                                              .FirstOrDefaultAsync(o => o.ManufacturingOrderUid == moUid);

                    if (order != null)
                    {
                        if (notif.PropertyName == "Quantity")
                        {
                            // Quantity est un init-only, utiliser EF pour affecter la valeur
                            if (int.TryParse(notif.NewValue, out var qty))
                            {
                                var entry = _context.Entry(order);
                                entry.Property("Quantity").CurrentValue = qty;
                                entry.Property("Quantity").IsModified = true;
                            }
                            else
                            {
                                Log.Warning("[DatabaseNotificationRepository] Nouvelle valeur de quantité invalide: {NewValue}", notif.NewValue);
                            }
                        }
                        else if (notif.PropertyName == "SerialNumber")
                        {
                            // FirstSerialNumber est init-only; utiliser EF pour affecter la valeur
                            if (int.TryParse(notif.NewValue, out var sn))
                            {
                                var entry = _context.Entry(order);
                                entry.Property("FirstSerialNumber").CurrentValue = sn;
                                entry.Property("FirstSerialNumber").IsModified = true;
                            }
                            else
                            {
                                Log.Warning("[DatabaseNotificationRepository] Nouvelle valeur de numéro de série invalide: {NewValue}", notif.NewValue);
                            }
                        }

                        Log.Information("[DatabaseNotificationRepository] Mise à jour de {EntityType} {EntityUid} : {PropertyName} = {NewValue}",
                            notif.EntityType, notif.EntityUId, notif.PropertyName, notif.NewValue);
                    }
                    else
                    {
                        Log.Warning("[DatabaseNotificationRepository] Entité {EntityType} {EntityUid} non trouvée, mise à jour ignorée.",
                            notif.EntityType, notif.EntityUId);
                    }
                }

                // Mettre à jour la notification comme traitée
                notif.Processed = "PROCESSED";
                notif.ProcessedBy = processedBy;
                notif.ProcessedDate = DateTimeOffset.Now;

                _context.DatabaseNotifications.Update(notif);

                await _context.SaveChangesAsync();

                Log.Information("[DatabaseNotificationRepository] Notification {Uid} traitée par {User}", notificationUid, processedBy);

                return notif;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[DatabaseNotificationRepository] Erreur lors du traitement de la notification {Uid}", notificationUid);
                throw;
            }
        }

    }
}
