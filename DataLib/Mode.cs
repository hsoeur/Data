using Swashbuckle.AspNetCore.Annotations;

namespace DataLib
{
    [SwaggerSchema(Description = "Opérateur de comparaison appliqué à la mesure (ex : =, <, <=, >, >=, !=, entre).")]
    public enum Mode
    {
        Unknown =0,
        Equal =1, // =
        NotEqual =2, // !=
        Less =3, // <
        LessOrEqual =4, // <=
        Greater =5, // >
        GreaterOrEqual =6, // >=
        BetweenInclusive =7,// Val1 <= x <= Val2
        BetweenExclusive =8 // Val1 < x < Val2
    }
}
