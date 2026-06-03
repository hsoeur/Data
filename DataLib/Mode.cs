using Swashbuckle.AspNetCore.Annotations;

namespace DataLib
{
    [SwaggerSchema(Description = "Opérateur de comparaison appliqué à la mesure (ex : =, <, <=, >, >=, !=, entre).")]
    public static class Mode
    {
        public const string Between = "Between";
        public const string Equal = "Equal";
        public const string Min = "Min";
        public const string Max = "Max";
        public const string Record = "Record";
        public const string Entry = "Entry";
    }
}
