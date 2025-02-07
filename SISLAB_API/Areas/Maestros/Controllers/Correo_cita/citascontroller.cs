using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class CitasController : ControllerBase
{
    private readonly CorreoService _correoService;

    public CitasController(CorreoService correoService)
    {
        _correoService = correoService;
    }

    [HttpPost("enviar-detalles-cita")]
    public async Task<IActionResult> EnviarDetallesCita([FromBody] CitaMedica citaMedica)
    {
        if (citaMedica == null || string.IsNullOrEmpty(citaMedica.Email))
        {
            return BadRequest("El email es obligatorio.");
        }

        try
        {
            await _correoService.EnviarDetallesCitaAsync(citaMedica);
            return Ok("Detalles de la cita enviados con éxito.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al enviar el correo: {ex.Message}");
        }
    }
}