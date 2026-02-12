using Swashbuckle.AspNetCore.Annotations;

namespace DataLib
{
    [SwaggerSchema(Description = "Unité de mesure (ex : °C, V, Ohm).")]
    public static class Unit
    {
        public const string Unknown = "Unknown";
        public const string Volt = "Volt";
        public const string Ampere = "Ampere";
        public const string Ohm = "Ohm";
        public const string Celsius = "Celsius";
        public const string Percent = "Percent";
        public const string Second = "Second";
        public const string Hertz = "Hertz";
    }
}
