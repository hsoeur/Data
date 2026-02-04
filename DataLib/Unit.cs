using Swashbuckle.AspNetCore.Annotations;

namespace DataLib
{
 [SwaggerSchema(Description = "Unité de mesure (ex : °C, V, Ohm).")]
 public enum Unit
 {
 Unknown =0,
 Volt =1,
 Ampere =2,
 Ohm =3,
 Celsius =4,
 Percent =5,
 Second =6
 }
}
