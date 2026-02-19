using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Watchly.Application.UsuarioTitulo;
using Watchly.Domain.Enum;

namespace Watchly.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("minha-lista")]
    public sealed class UsuarioTituloController : ControllerBase
    {
        private readonly IUsuarioTitulo _service;

        public UsuarioTituloController(IUsuarioTitulo service)
        {
            _service = service;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> AddToList(
            [FromBody] AddToListRequest request,
            CancellationToken ct)
        {
            await _service.AddToListAsync(request, ct);
            return Created(string.Empty, null);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<UserTituloResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyList(
            [FromQuery] StatusTitulo? status,
            CancellationToken ct)
        {
            var result = await _service.GetMyListAsync(status, ct);
            return Ok(result);
        }

        [HttpPut("{tituloId:guid}")]
        [ProducesResponseType(typeof(UserTituloResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Update(
            [FromRoute] Guid tituloId,
            [FromBody] UpdateTrackingRequest request,
            CancellationToken ct)
        {
            var result = await _service.UpdateAsync(tituloId, request, ct);
            return Ok(result);
        }

        [HttpDelete("{tituloId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Remove(
            [FromRoute] Guid tituloId,
            CancellationToken ct)
        {
            await _service.RemoveAsync(tituloId, ct);
            return NoContent();
        }
    }
}