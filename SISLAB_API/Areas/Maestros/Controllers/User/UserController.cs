using Microsoft.AspNetCore.Mvc;
using SISLAB_API.Areas.Maestros.Models;
using SISLAB_API.Areas.Maestros.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SISLAB_API.Areas.Maestros.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        // Inyección de dependencias del servicio UserService
        public UserController(UserService userService)
        {
            _userService = userService;
        }

        // Obtener todos los usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsersAsync()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        // Obtener todos los roles
        [HttpGet("role")]
        public async Task<ActionResult<IEnumerable<Role>>> GetAllRolAsync()
        {
            var roles = await _userService.GetAllRolsAsync();
            return Ok(roles);
        }


        [HttpPost("update-password")]
        public async Task<IActionResult> UpdatePassword([FromBody] UserPasswordUpdate model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Dni) || string.IsNullOrEmpty(model.NewPassword))
                {
                    return BadRequest(new { success = false, message = "DNI y nueva contraseña son requeridos" });
                }

                var result = await _userService.UpdatePassword(model);

                if (result)
                {
                    return Ok(new { success = true, message = "Contraseña actualizada exitosamente" });
                }
                else
                {
                    return BadRequest(new { success = false, message = "No se pudo actualizar la contraseña" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    error = ex.Message
                });
            }
        }
       
    }
}