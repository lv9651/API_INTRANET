using Dapper;
using MySql.Data.MySqlClient;
using SISLAB_API.Areas.Maestros.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

public class AgendaRepository 
    {
        private readonly string? _connectionString;

        public AgendaRepository(IConfiguration  configuration)
        {
        _connectionString = configuration.GetConnectionString("SislabConnection");

    }

        public void AddAgendaItem(AgendaItem item)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var query = "INSERT INTO Agenda (meeting_name, dni, date, time,time_2,nombre) VALUES (@MeetingName, @Dni, @Date, @Time,@Time_2,@nombre)";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MeetingName", item.MeetingName);
                    command.Parameters.AddWithValue("@Dni", item.Dni);
                    command.Parameters.AddWithValue("@Date", item.Date);
                    command.Parameters.AddWithValue("@Time", item.Time);
                command.Parameters.AddWithValue("@Time_2", item.Time_2);
                command.Parameters.AddWithValue("@nombre", item.nombre);
                command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteAgendaItem(int id)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var query = "DELETE FROM Agenda WHERE id = @Id";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

    public async Task<int> CrearAsync(CrearSugerenciaRequest sugerencia)
    {
        using var conn = new MySqlConnection(_connectionString);
        var sql = "CALL CrearSugerencia(@EmpleadoDni, @Tipo, @Mensaje);";
        var parameters = new
        {
            EmpleadoDni = sugerencia.EmpleadoDni,
            Tipo = sugerencia.Tipo,
            Mensaje = sugerencia.Mensaje
        };
        await conn.ExecuteAsync(sql, parameters);

        var id = await conn.ExecuteScalarAsync<int>("SELECT LAST_INSERT_ID();");
        return id;
    }

    public async Task<IEnumerable<Sugerencia>> ObtenerPorEmpleadoAsync(string dni)
    {
        using var conn = new MySqlConnection(_connectionString);
        var sql = @"SELECT s.*, e.Nombre AS EmpleadoNombre
                    FROM sugerencias s
                    LEFT JOIN users e ON e.dni = s.empleado_dni
                    WHERE s.empleado_dni = @dni
                    ORDER BY s.fecha_envio DESC";
        return await conn.QueryAsync<Sugerencia>(sql, new { dni });
    }
    public async Task<IEnumerable<Sugerencia>> ObtenerTodasAsync()
    {
        using var conn = new MySqlConnection(_connectionString);
        var sql = "CALL ObtenerTodasLasSugerencias();";
        return await conn.QueryAsync<Sugerencia>(sql);
    }

    public async Task<bool> MarcarComoRevisadaAsync(int id, string revisadoPorDni, string respuestaRh = null)
    {
        using var conn = new MySqlConnection(_connectionString);

        if (respuestaRh == null)
        {
            var sql = "CALL MarcarSugerenciaComoRevisada(@Id, @RevisadoPorDni);";
            var result = await conn.ExecuteAsync(sql, new { Id = id, RevisadoPorDni = revisadoPorDni });
            return result > 0;
        }
        else
        {
            var sql = "CALL ResponderSugerenciaConComentario(@Id, @RevisadoPorDni, @Respuesta);";
            var result = await conn.ExecuteAsync(sql, new { Id = id, RevisadoPorDni = revisadoPorDni, Respuesta = respuestaRh });
            return result > 0;
        }

    }
    public List<AgendaItem> GetAgendaItems(DateTime date)
        {
            var items = new List<AgendaItem>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT * FROM Agenda WHERE date = @Date";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Date", date);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items.Add(new AgendaItem
                            {
                                Id = reader.GetInt32("id"),  
                                MeetingName = reader.GetString("meeting_name"),
                                Dni = reader.GetString("dni"),
                                Date = reader.GetDateTime("date").ToString("yyyy-MM-dd"),
                                Time = reader.GetTimeSpan("time"),
                                Time_2 = reader.GetTimeSpan("time_2"),
                                nombre = reader.GetString("nombre"),
                            });
                        }
                    }
                }
            }
            return items;
        }
    }
