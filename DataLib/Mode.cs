using Swashbuckle.AspNetCore.Annotations;

namespace DataLib
{
    [SwaggerSchema(Description = "Opérateur de comparaison appliqué à la mesure (ex : =, <, <=, >, >=, !=, entre).")]
    public static class Mode
    {
        public const string Unknown = "Unknown";
        public const string Equal = "Equal"; // =
        public const string NotEqual = "NotEqual"; // !=
        public const string Less = "Less"; // <
        public const string LessOrEqual = "LessOrEqual"; // <=
        public const string Greater = "Greater"; // >
        public const string GreaterOrEqual = "GreaterOrEqual"; // >=
        public const string BetweenInclusive = "BetweenInclusive"; // Val1 <= x <= Val2
        public const string BetweenExclusive = "BetweenExclusive"; // Val1 < x < Val2
    }
}
