using Microsoft.AspNetCore.Mvc;
using SISLAB_API.Areas.Maestros.Models;

using System;
using System.Linq;
using System.Threading.Tasks;

namespace SISLAB_API.Areas.Maestros.Controllers
{
    [Route("api/maestros/[controller]")]
    [ApiController]
    public class ReservarController : ControllerBase
    {
        private readonly ReservaRepository _reservaRepository;
        private readonly EmailRepository _emailRepository;  // ← Usar el existente

        public ReservarController(ReservaRepository reservaRepository, EmailRepository emailRepository)
        {
            _reservaRepository = reservaRepository;
            _emailRepository = emailRepository;
        }

        // GET: api/maestros/reservar/salas
        [HttpGet("salas")]
        public async Task<IActionResult> GetSalas()
        {
            try
            {
                var salas = await _reservaRepository.GetSalasAsync();
                return Ok(salas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // GET: api/maestros/reservar?fecha=2024-01-01
        [HttpGet]
        public async Task<IActionResult> GetReservas([FromQuery] DateTime fecha)
        {
            try
            {
                var reservas = await _reservaRepository.GetReservasByFechaAsync(fecha);
                return Ok(reservas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // GET: api/maestros/reservar/mis-reservas?usuarioDni=002958968
        [HttpGet("mis-reservas")]
        public async Task<IActionResult> GetMisReservas([FromQuery] string usuarioDni)
        {
            try
            {
                var reservas = await _reservaRepository.GetReservasByUsuarioDniAsync(usuarioDni);
                return Ok(reservas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // GET: api/maestros/reservar/disponibilidad
        [HttpGet("disponibilidad")]
        public async Task<IActionResult> CheckDisponibilidad(
        [FromQuery] int salaId,
        [FromQuery] DateTime fecha,
        [FromQuery] string horaInicio,   // Cambiado a string
        [FromQuery] string horaFin)      // Cambiado a string
        {
            try
            {
                // Convertir string a TimeSpan
                TimeSpan horaInicioTs = TimeSpan.Parse(horaInicio);
                TimeSpan horaFinTs = TimeSpan.Parse(horaFin);

                var disponible = await _reservaRepository.CheckDisponibilidadAsync(salaId, fecha, horaInicioTs, horaFinTs);
                return Ok(new { disponible = disponible });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // POST: api/maestros/reservar
        // En ReservarController.cs - método CreateReserva
        [HttpPost]
        public async Task<IActionResult> CreateReserva([FromBody] ReservaRequest request)
        {
            try
            {
                var (success, reservaId, errorMessage) = await _reservaRepository.CreateReservaAsync(request);

                if (!success)
                {
                    return Conflict(new { error = errorMessage });
                }

                // Obtener nombre de la sala para el correo
                var salas = await _reservaRepository.GetSalasAsync();
                var sala = salas.FirstOrDefault(s => s.Id == request.SalaId);

                // Enviar correo de confirmación - PASAR LAS HORAS COMO STRING
                await _emailRepository.SendReservaConfirmacionAsync(
                    request.UsuarioEmail,
                    request.UsuarioNombre,
                    sala?.Nombre ?? "Sala",
                    request.Fecha,
                    request.HoraInicio,  // ← Enviar string directamente
                    request.HoraFin,     // ← Enviar string directamente
                    request.AreaNombre,
                       request.Motivo ?? "Sin motivo especificado",
                     request.UsuarioDni
                 
                );

                return Ok(new { id = reservaId, message = "Reserva creada exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // PUT: api/maestros/reservar/{id}/cancelar
        [HttpPut("{id}/cancelar")]
        public async Task<IActionResult> CancelarReserva(int id)
        {
            try
            {
                // Primero obtener los datos de la reserva antes de cancelar
                var reserva = await _reservaRepository.GetReservaByIdAsync(id);
                if (reserva == null)
                {
                    return NotFound(new { error = "Reserva no encontrada" });
                }

                // Verificar datos para depuración
                Console.WriteLine($"Reserva encontrada - ID: {reserva.Id}");
                Console.WriteLine($"Usuario: {reserva.UsuarioNombre} - DNI: {reserva.UsuarioDni}");
                Console.WriteLine($"Email: {reserva.UsuarioEmail}");
                Console.WriteLine($"Sala: {reserva.SalaNombre}");
                Console.WriteLine($"HoraInicio: {reserva.HoraInicio}");
                Console.WriteLine($"HoraFin: {reserva.HoraFin}");

                var result = await _reservaRepository.CancelarReservaAsync(id);
                if (!result)
                {
                    return NotFound(new { error = "Reserva no encontrada o ya cancelada" });
                }

                // Formatear las horas correctamente
                string horaInicioStr = reserva.HoraInicio.ToString();
                string horaFinStr = reserva.HoraFin.ToString();

                // Si tiene formato largo (HH:MM:SS), tomar solo HH:MM
                if (horaInicioStr.Length > 5)
                {
                    horaInicioStr = horaInicioStr.Substring(0, 5);
                }
                if (horaFinStr.Length > 5)
                {
                    horaFinStr = horaFinStr.Substring(0, 5);
                }

                // Enviar correo de cancelación
                try
                {
                    await _emailRepository.SendReservaCancelacionAsync(
                        reserva.UsuarioEmail,      // toEmail (irá en CC)
                        reserva.UsuarioNombre,     // usuarioNombre
                        reserva.SalaNombre,        // salaNombre
                        reserva.Fecha,             // fecha
                        horaInicioStr,             // horaInicio (formato HH:MM)
                        horaFinStr,                // horaFin (formato HH:MM)
                        reserva.UsuarioDni         // usuarioDni
                    );
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error enviando correo de cancelación: {ex.Message}");
                }

                return Ok(new { message = "Reserva cancelada exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}