using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.Data.Swagger
{
    /// <summary>
    /// Ajoute une note générale à la description de chaque endpoint dans Swagger UI.
    /// </summary>
    public sealed class CommonOperationDescriptionFilter : IOperationFilter
    {
        private const string NoteHeader = "\n\nNote générale:";
        private static readonly string NoteBody =
        "\n- Format: application/json" +
        "\n- Dates/Heures: UTC" +
        "\n- Codes de réponse:200 succès,204 aucune donnée,400 requête invalide,404 introuvable,500 erreur interne" +
        "\n- Journalisation: chaque appel est tracé via Serilog";

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Description ??= string.Empty;

            // Évite les doublons si déjà appliqué
            if (operation.Description.Contains("Note générale:"))
            {
            return;
            }

            operation.Description += NoteHeader + NoteBody;
        }
    }
}
