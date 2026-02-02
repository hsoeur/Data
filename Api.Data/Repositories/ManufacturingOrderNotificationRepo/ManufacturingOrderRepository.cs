using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLib;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Api.Data.Repositories.ManufacturingOrderNotificationRepo
{
    public class ManufacturingOrderNotificationRepository : IManufacturingOrderNotificationRepository
    {
        private readonly AppDbContext _context;

        public ManufacturingOrderNotificationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(ManufacturingOrderNotification notification)
        {
            try
            {
                var existingNotification = await _context.ManufacturingOrderNotifications
                    .FirstOrDefaultAsync(n =>
                        n.OfNumber == notification.OfNumber &&
                        n.ChangeType == notification.ChangeType);

                if (existingNotification != null)
                {
                    if (existingNotification.NewValue == notification.NewValue)
                    {
                        Log.Warning("Notification déjà existante pour l'ordre {OrderNumber} avec la même valeur.", notification.NotificationUid);
                        return;
                    }
                    else
                    {
                        if (existingNotification.Processed == "NOT_PROCESSED")
                        {
                            existingNotification.NewValue = notification.NewValue;
                            existingNotification.CreatedDate = notification.CreatedDate;

                            Log.Information("Notification existante mise à jour pour OF {OrderNumber}.", notification.OfNumber);

                            await _context.SaveChangesAsync();
                            return;
                        }
                        else
                        {
                            _context.ManufacturingOrderNotifications.Add(notification);
                            await _context.SaveChangesAsync();
                            return;
                        }
                    }
                }

                // Pas de notification existante → on ajoute normalement
                _context.ManufacturingOrderNotifications.Add(notification);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erreur lors de la création de la notification pour l'ordre {OrderNumber}", notification.NotificationUid);
                throw;
            }
        }

        public async Task<IEnumerable<ManufacturingOrderNotification>> GetAllAsync()
        {
            try
            {
                return await _context.ManufacturingOrderNotifications.Where(not => not.Processed == "NOT_PROCESSED").ToListAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erreur lors de la récupération des notifications");
                throw;
            }
        }

        public async Task UpdateAsync(ManufacturingOrderNotification notification)
        {
            try
            {
                var existing = await _context.ManufacturingOrderNotifications
                    .FirstOrDefaultAsync(n => n.NotificationUid == notification.NotificationUid);

                if (existing == null)
                {
                    Log.Warning("Notification non trouvée avec l'UID {NotificationUid}", notification.NotificationUid);
                    return; // ou throw new KeyNotFoundException(...)
                }

                existing.ProcessedBy = notification.ProcessedBy;
                existing.ProcessedDate = notification.ProcessedDate;
                existing.Processed = "PROCESSED";

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erreur lors de la mise à jour de la notification {NotificationUid}", notification.NotificationUid);
                throw;
            }
        }

        public async Task UpdateQuantityAsync(string notificationUid, string newQuantity, string operatorUid)
        {
            try
            {
                var notification = await _context.ManufacturingOrderNotifications
                    .FirstOrDefaultAsync(n => n.NotificationUid == notificationUid);

                if (notification == null)
                {
                    Log.Warning("Notification not found for UID: {NotificationUid}", notificationUid);
                    return;
                }

                //notification.NewValue = newQuantity;
                notification.Processed = "UPDATED";
                notification.ProcessedDate = DateTimeOffset.UtcNow;
                notification.ProcessedBy = operatorUid;

                await _context.SaveChangesAsync();

                Log.Information("Quantity notification updated for UID: {NotificationUid}", notificationUid);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error updating quantity notification for UID: {NotificationUid}", notificationUid);
                throw;
            }
        }

        public async Task UpdateSerialNumberAsync(string notificationUid, string newSerialNumber, string operatorUid)
        {
            try
            {
                var notification = await _context.ManufacturingOrderNotifications
                    .FirstOrDefaultAsync(n => n.NotificationUid == notificationUid);

                if (notification == null)
                {
                    Log.Warning("Notification not found for UID: {NotificationUid}", notificationUid);
                    return;
                }

                //notification.NewValue = newSerialNumber;
                notification.Processed = "UPDATED";
                notification.ProcessedDate = DateTimeOffset.UtcNow;
                notification.ProcessedBy = operatorUid;

                await _context.SaveChangesAsync();

                Log.Information("Serial number notification updated for UID: {NotificationUid}", notificationUid);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error updating serial number notification for UID: {NotificationUid}", notificationUid);
                throw;
            }
        }
    }
}
