using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using SISLAB_API.Areas.Maestros.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;


    public class ReservaRepository
    {
        private readonly string _connectionString;

        public ReservaRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SislabConnection");
        }

        // Crear conexión con la base de datos
        private MySqlConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    public async Task<Reserva> GetReservaByIdAsync(int reservaId)
    {
        using (var connection = CreateConnection())
        {
            var parameters = new DynamicParameters();
            parameters.Add("@p_reserva_id", reservaId);

            var reserva = await connection.QueryFirstOrDefaultAsync<Reserva>(
                "sp_GetReservaById",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return reserva;
        }
    }
    // Obtener todas las salas activas
    public async Task<IEnumerable<Sala>> GetSalasAsync()
        {
            using (var connection = CreateConnection())
            {
                var salas = await connection.QueryAsync<Sala>(
                    "sp_GetSalas",
                    commandType: CommandType.StoredProcedure
                );
                return salas;
            }
        }

        // Obtener reservas por fecha
        public async Task<IEnumerable<Reserva>> GetReservasByFechaAsync(DateTime fecha)
        {
            using (var connection = CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@p_fecha", fecha.ToString("yyyy-MM-dd"));

                var reservas = await connection.QueryAsync<Reserva>(
                    "sp_GetReservasByFecha",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                return reservas;
            }
        }

        // Obtener reservas por DNI de usuario
        public async Task<IEnumerable<Reserva>> GetReservasByUsuarioDniAsync(string usuarioDni)
        {
            using (var connection = CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@p_usuario_dni", usuarioDni);

                var reservas = await connection.QueryAsync<Reserva>(
                    "sp_GetReservasByUsuarioDni",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                return reservas;
            }
        }

        // Verificar disponibilidad
        public async Task<bool> CheckDisponibilidadAsync(int salaId, DateTime fecha, TimeSpan horaInicio, TimeSpan horaFin)
        {
            using (var connection = CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@p_sala_id", salaId);
                parameters.Add("@p_fecha", fecha.ToString("yyyy-MM-dd"));
                parameters.Add("@p_hora_inicio", horaInicio.ToString());
                parameters.Add("@p_hora_fin", horaFin.ToString());
                parameters.Add("@p_existe_conflicto", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await connection.ExecuteAsync(
                    "sp_CheckDisponibilidad",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                var conflicto = parameters.Get<int>("@p_existe_conflicto");
                return conflicto == 0;
            }
        }

    // Crear reserva
    public async Task<(bool success, int? reservaId, string errorMessage)> CreateReservaAsync(ReservaRequest request)
    {
        using (var connection = CreateConnection())
        {
            // Convertir string a TimeSpan
            if (!TimeSpan.TryParse(request.HoraInicio, out TimeSpan horaInicio))
            {
                return (false, null, "Formato de hora inicio inválido. Use HH:MM");
            }

            if (!TimeSpan.TryParse(request.HoraFin, out TimeSpan horaFin))
            {
                return (false, null, "Formato de hora fin inválido. Use HH:MM");
            }

            var parameters = new DynamicParameters();
            parameters.Add("@p_usuario_dni", request.UsuarioDni);
            parameters.Add("@p_usuario_nombre", request.UsuarioNombre);
            parameters.Add("@p_usuario_email", request.UsuarioEmail);
            parameters.Add("@p_sala_id", request.SalaId);
            parameters.Add("@p_fecha", request.Fecha.ToString("yyyy-MM-dd"));
            parameters.Add("@p_hora_inicio", horaInicio.ToString());
            parameters.Add("@p_hora_fin", horaFin.ToString());
            parameters.Add("@p_area", request.Area);
            parameters.Add("@p_area_nombre", request.AreaNombre);
            parameters.Add("@p_motivo", request.Motivo ?? "");
            parameters.Add("@p_reserva_id", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@p_error_message", dbType: DbType.String, direction: ParameterDirection.Output, size: 255);

            await connection.ExecuteAsync(
                "sp_CreateReserva",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var reservaId = parameters.Get<int?>("@p_reserva_id");
            var errorMessage = parameters.Get<string>("@p_error_message");

            if (reservaId.HasValue && reservaId.Value > 0)
            {
                return (true, reservaId.Value, null);
            }
            else
            {
                return (false, null, errorMessage ?? "Error al crear la reserva");
            }
        }
    }

    // Cancelar reserva
    public async Task<bool> CancelarReservaAsync(int reservaId)
        {
            using (var connection = CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@p_reserva_id", reservaId);
                parameters.Add("@p_actualizado", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await connection.ExecuteAsync(
                    "sp_CancelarReserva",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                var actualizado = parameters.Get<int>("@p_actualizado");
                return actualizado > 0;
            }
        }
    }
