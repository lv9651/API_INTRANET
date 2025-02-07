using Microsoft.AspNetCore.Mvc;
using SISLAB_API.Areas.Maestros.Models;
using SISLAB_API.Areas.Maestros.Services;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace SISLAB_API.Areas.Maestros.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioService _clinicaUserService;
        private readonly EmailService _emailService;

        // Constructor para inyectar los servicios
        public UsuarioController(UsuarioService clinicaUserService, EmailService emailService)
        {
            _clinicaUserService = clinicaUserService;
            _emailService = emailService;
        }

        [HttpPost]
        public IActionResult RegisterUser([FromBody] Usuario user)
        {
            if (user == null)
                return BadRequest("Invalid user data.");

            try
            {
                // Llamar al servicio para insertar el usuario y obtener el mensaje de respuesta
                string responseMessage = _clinicaUserService.InsertarClinicaUser(user);

                // Si el mensaje contiene información de error, devolver un error
                if (responseMessage.Contains("Error"))
                {
                    return BadRequest(responseMessage);
                }

                // Enviar el correo con la imagen fija como adjunto
                string subject = "Bienvenido a Clinica Vinali,registro exitoso";
                string body = $"<h1>Hola {user.Nombres},</h1><p>Gracias por registrarte. Estamos a un paso de cuidar tu salud.</p>";

                // Llamar al servicio de correo para enviar el mensaje
                _emailService.SendEmailWithImageAsync(user.Correo, subject, body);

                // Devolver el mensaje de éxito
                return Ok(new { message = responseMessage });
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("recuperar-correo")]
        public async Task<IActionResult> RecuperarCorreo([FromBody] string dni)
        {
            var correo = await _clinicaUserService.RecuperarCorreoPorDniAsync(dni);
            if (string.IsNullOrEmpty(correo))
            {
                return BadRequest("No se encontró un usuario con ese DNI.");
            }

            return Ok("El código de verificación ha sido enviado a tu correo.");
        }

        [HttpPost("validar-codigo")]
        public async Task<IActionResult> ValidarCodigo([FromBody] CodigoVerificacionRequest request)
        {
            // Llamar al servicio para validar el código, el cual retorna un bool
            var esValido = await _clinicaUserService.ValidarCodigoAsync(request.Dni, request.Codigo);

            if (!esValido)
            {
                return BadRequest("Código inválido o expirado.");
            }

            return Ok("Código válido. Ahora puedes cambiar tu contraseña.");
        }

        // Endpoint para cambiar la contraseña
        [HttpPost("cambiar-clave")]
        public async Task<IActionResult> CambiarClave([FromBody] CambiarContraseñaRequest request)
        {
            await _clinicaUserService.CambiarContraseñaAsync(request.Dni, request.NuevaContraseña);
            return Ok("Contraseña cambiada exitosamente.");
        }

        [HttpPost("autenticar")]
        public async Task<IActionResult> Autenticar([FromBody] AuthRequest request)
        {
            if (string.IsNullOrEmpty(request.documento) || string.IsNullOrEmpty(request.Clave))
            {
                return BadRequest("El DNI y la clave son requeridos.");
            }

            try
            {
                // Llamar al servicio para autenticar al usuario
                var usuario = await _clinicaUserService.AutenticarUsuarioAsync(request.documento, request.Clave);

                if (usuario == null)
                {
                    return Unauthorized("Usuario o clave incorrectos.");
                }

                // Devolver la información del usuario (o token si es necesario)
                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }










}


public class AuthRequest
{
    public string documento { get; set; }
    public string Clave { get; set; }
}


public class CodigoVerificacionRequest
    {
        public string Dni { get; set; }
        public string Codigo { get; set; }
    }

    public class CambiarContraseñaRequest
    {
        public string Dni { get; set; }
        public string NuevaContraseña { get; set; }
    }
