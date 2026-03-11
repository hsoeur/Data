using DataLib;
using Microsoft.AspNetCore.Mvc;

namespace Api.Data.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public HealthController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Vérifie que l'API est joignable (sans vérifier la base de données).
        /// </summary>
        [HttpGet]
        public IActionResult Live()
        {
            return Ok(new { api = true });
        }

        /// <summary>
        /// Vérifie que la base de données est accessible.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Database(CancellationToken cancellationToken)
        {
            try
            {
                var canConnect = await _dbContext.Database.CanConnectAsync(cancellationToken);
                if (canConnect)
                {
                    return Ok(new { database = true });
                }

                return StatusCode(503, new { database = false });
            }
            catch
            {
                return StatusCode(503, new { database = false });
            }
        }

        /// <summary>
        /// Affiche la configuration de connexion MySQL (sans mot de passe).
        /// </summary>
        [HttpGet]
        public IActionResult Config()
        {
            var server = Environment.GetEnvironmentVariable("MYSQL_HOST") ?? "localhost (default)";
            var port = Environment.GetEnvironmentVariable("MYSQL_PORT") ?? "3306 (default)";
            var database = Environment.GetEnvironmentVariable("MYSQL_DATABASE") ?? "db_production (default)";
            var user = Environment.GetEnvironmentVariable("MYSQL_USER") ?? "root (default)";
            var hasPassword = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("MYSQL_PASSWORD")) || 
            !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("MYSQL_ROOT_PASSWORD"));

            return Ok(new 
            { 
                mysql_host = server,
                mysql_port = port,
                mysql_database = database,
                mysql_user = user,
                mysql_password_set = hasPassword,
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown"
            });
        }

        /// <summary>
        /// Endpoint conservé pour compatibilité.
        /// </summary>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { api = true });
        }
    }
}
