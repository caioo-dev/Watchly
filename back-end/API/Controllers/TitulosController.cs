
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Watchly.Application.Titulos;
using Watchly.Domain.Enum;

namespace Watchly.API.Controllers
{
    [ApiController]
    [Route("titulos")]
    public sealed class TitulosController : ControllerBase
    {
        private readonly ITitulosService _service;

        public TitulosController(ITitulosService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Buscar(
            [FromQuery] string query,
            [FromQuery] TipoTitulo? tipo,
            CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("O parâmetro 'query' é obrigatório.");

            try
            {
                var result = await _service.BuscarAsync(query, tipo, ct);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ex.Message, ex.StackTrace });
            }
        }

        [HttpGet("{fonte}/{externalId}")]
        [ProducesResponseType(typeof(TituloDetalheResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> BuscarDetalhe(
            [FromRoute] FonteTitulo fonte,
            [FromRoute] string externalId,
            CancellationToken ct)
        {
            var result = await _service.BuscarDetalheAsync(externalId, fonte, ct);
            return Ok(result);
        }
    }
}