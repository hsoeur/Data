using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Api.Data.Repositories.ManufacturingOrderNotificationRepo;
using DataLib;
using Serilog;
using Swashbuckle.AspNetCore.Annotations;
using Api.Data.Dtos;

namespace Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ManufacturingOrderNotificationController : ControllerBase
    {
        private readonly IManufacturingOrderNotificationRepository _manufacturingOrderNotificationRepository;

        public ManufacturingOrderNotificationController(IManufacturingOrderNotificationRepository manufacturingOrderNotificationRepository)
        {
            _manufacturingOrderNotificationRepository = manufacturingOrderNotificationRepository;
        }

        [SwaggerOperation(
            Summary = "Crée une notification d'ordre de fabrication",
            Description = @"But: enregistrer une divergence détectée sur un OF (quantité ou numéro de série).

Ce que fait concrètement le endpoint:
- Valide les données d'entrée: `ofNumber` >0 et `changeType` non vide, sinon400.
- Construit une notification avec:
 - `notificationUid` généré si absent,
 - `processed` = `NOT_PROCESSED` et `processedDate` = null,
 - `createdDate` = valeur fournie ou l'instant UTC.
- Enregistre la notification via le dépôt (base de données).
- Journalise l'opération (Serilog).
- Retourne200 avec un message de confirmation en cas de succès.
",
            OperationId = "CreateManufacturingOrderNotification"
        )]
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerResponse(StatusCodes.Status200OK, "Notification enregistrée.", typeof(string))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Requête invalide: corps manquant ou paramètres invalides (ex: ofNumber <=0, changeType vide).", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Erreur interne lors de la création de la notification.", typeof(string))]
        public async Task<IActionResult> Create([FromBody] ManufacturingOrderNotificationDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Paramètre invalide",
                    Detail = "Le corps de la requête est requis."
                });
            }
            if (dto.OfNumber <=0)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Paramètre invalide",
                    Detail = "Le paramètre 'OfNumber' est requis et doit être positif."
                });
            }
            if (string.IsNullOrWhiteSpace(dto.ChangeType))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Paramètre invalide",
                    Detail = "Le paramètre 'ChangeType' est requis."
                });
            }

            try
            {
                Log.Information("CreateManufacturingOrderNotification called with OfNumber: {OfNumber}, ChangeType: {ChangeType}", dto.OfNumber, dto.ChangeType);
                var notification = new ManufacturingOrderNotification
                {
                    NotificationUid = dto.NotificationUid?.ToString() ?? Guid.NewGuid().ToString(),
                    OfNumber = dto.OfNumber,
                    ChangeType = dto.ChangeType,
                    OldValue = dto.OldValue,
                    NewValue = dto.NewValue,
                    ProcessedBy = string.Empty,
                    CreatedDate = dto.CreatedDate ?? DateTimeOffset.UtcNow,
                    Processed = "NOT_PROCESSED",
                    ProcessedDate = null
                };
                await _manufacturingOrderNotificationRepository.CreateAsync(notification);
                Log.Information("CreateManufacturingOrderNotification successfully created notification with uid: {NotificationUid}", notification.NotificationUid);
                return Ok("Notification enregistrée.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing CreateManufacturingOrderNotification for OfNumber: {OfNumber}", dto.OfNumber);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Récupère toutes les notifications d'ordre de fabrication",
            Description = @"But: lister toutes les notifications enregistrées.

Ce que fait concrètement le endpoint:
- Récupère la liste complète des notifications via le dépôt.
- Si la liste est vide: retourne204 (aucun contenu).
- Sinon: retourne200 avec la collection de notifications.
- Journalise la requête et le résultat.
",
            OperationId = "GetAllManufacturingOrderNotifications"
        )]
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<ManufacturingOrderNotification>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerResponse(StatusCodes.Status200OK, "Liste des notifications d'ordre de fabrication.", typeof(ICollection<ManufacturingOrderNotification>))]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Aucune notification trouvée." )]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Erreur interne lors de la récupération des notifications.", typeof(string))]
        public async Task<ActionResult<ICollection<ManufacturingOrderNotification>>> GetAll()
        {
            try
            {
                Log.Information("GetAllManufacturingOrderNotifications called");
                var notifications = await _manufacturingOrderNotificationRepository.GetAllAsync();
                if (notifications == null || !notifications.Any())
                {
                    Log.Information("GetAllManufacturingOrderNotifications returned no content");
                    return NoContent();
                }
                Log.Information("GetAllManufacturingOrderNotifications successfully returned {Count} notifications", notifications.Count());
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing GetAllManufacturingOrderNotifications");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [SwaggerOperation(
            Summary = "Met à jour une notification d'ordre de fabrication",
            Description = @"But: modifier une notification existante identifiée par `notificationUid`.

Ce que fait concrètement le endpoint:
- Valide la présence d'un corps et de `notificationUid`, sinon400.
- Envoie l'objet reçu au dépôt pour mise à jour (remplace les champs existants par ceux fournis).
- Journalise l'opération (avant/après appel dépôt).
- Retourne200 avec un message de confirmation en cas de succès.
",
            OperationId = "UpdateManufacturingOrderNotification"
        )]
        [HttpPut("update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerResponse(StatusCodes.Status200OK, "Notification mise à jour.", typeof(string))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Requête invalide: corps manquant ou `notificationUid` absent.", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Erreur interne lors de la mise à jour de la notification.", typeof(string))]
        public async Task<IActionResult> Update([FromBody] ManufacturingOrderNotification updatedNotification)
        {
            if (updatedNotification == null)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Paramètre invalide",
                    Detail = "Le corps de la requête est requis."
                });
            }
            if (string.IsNullOrWhiteSpace(updatedNotification.NotificationUid))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Paramètre invalide",
                    Detail = "Le paramètre 'NotificationUid' est requis."
                });
            }

            try
            {
                Log.Information("UpdateManufacturingOrderNotification called for uid: {NotificationUid}", updatedNotification.NotificationUid);
                await _manufacturingOrderNotificationRepository.UpdateAsync(updatedNotification);
                Log.Information("UpdateManufacturingOrderNotification successfully updated notification with uid: {NotificationUid}", updatedNotification.NotificationUid);
                return Ok("Notification mise à jour.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while processing UpdateManufacturingOrderNotification for uid: {NotificationUid}", updatedNotification.NotificationUid);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
