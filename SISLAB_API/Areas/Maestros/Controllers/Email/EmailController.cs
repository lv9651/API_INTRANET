using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;

[ApiController]
[Route("api/[controller]")]
public class EmailController : ControllerBase
{
    private readonly EmailService _emailService;

    public EmailController(EmailService emailService)
    {
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
    }

    [HttpPost("avisorespuesta")]
    public async Task<IActionResult> AvisoRespuesta([FromBody] EmailSug emailModel)
    {
        if (emailModel == null || string.IsNullOrWhiteSpace(emailModel.Dni))
            return BadRequest("El DNI es obligatorio.");

        try
        {
            await _emailService.SendAvisoRespuestaSugerenciaAsync(emailModel.Dni, emailModel.Nombre);
            return Ok(new { message = "Correo de aviso enviado exitosamente" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al enviar el correo.", error = ex.Message });
        }
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendEmail([FromForm] EmailModel emailModel)
    {
        if (emailModel == null || string.IsNullOrWhiteSpace(emailModel.Email))
        {
            return BadRequest("Datos del correo inválidos.");
        }

        try
        {
            await _emailService.SendDocumentEmailAsync(emailModel);
            return Ok(new { message = "Email enviado exitosamente" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al enviar el correo.", error = ex.Message });
        }
    }
}