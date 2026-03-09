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
        /// Endpoint conservé pour compatibilité.
        /// </summary>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { api = true });
        }
    }
}
