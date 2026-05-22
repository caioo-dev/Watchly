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
        [ResponseCache(Duration = 60)]
        public async Task<ActionResult<TituloExternoResponse>> Buscar(
            [FromQuery] string busca,
            [FromQuery] TipoTitulo? tipo,
            [FromQuery] int page = 1,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(busca))
                return BadRequest("O parâmetro 'busca' é obrigatório.");

            try
            {
                IReadOnlyList<TituloExternoResponse> result = await _service.BuscarAsync(busca, tipo, page, ct);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ex.Message, ex.StackTrace });
            }
        }

        [HttpGet("{fonte}/{tipo}/{externalId}")]
        [ResponseCache(Duration = 60)]
        [ProducesResponseType(typeof(TituloDetalheResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> BuscarDetalhe(
            [FromRoute] FonteTitulo fonte,
            [FromRoute] string externalId,
            [FromRoute] TipoTitulo tipo,
            CancellationToken ct)
        {
            TituloDetalheResponse result = await _service.BuscarDetalheAsync(fonte, tipo, externalId, ct);
            return Ok(result);
        }
    }
}