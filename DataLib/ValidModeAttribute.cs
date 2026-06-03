using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace DataLib
{
    /// <summary>
    /// Attribut de validation pour s'assurer que la valeur du Mode est l'une des constantes définies dans la classe Mode.
    /// </summary>
    public class ValidModeAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("Le mode ne peut pas être null.");
            }

            string modeValue = value.ToString() ?? string.Empty;

            // Récupérer toutes les constantes de la classe Mode
            var validModes = typeof(Mode)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string))
            .Select(fi => fi.GetValue(null)?.ToString())
            .Where(v => v != null)
            .ToList();

            if (!validModes.Contains(modeValue))
            {
                var validModesString = string.Join(", ", validModes);
                return new ValidationResult($"Le mode '{modeValue}' n'est pas valide. Les modes valides sont : {validModesString}");
            }

            return ValidationResult.Success;
        }
    }
}
