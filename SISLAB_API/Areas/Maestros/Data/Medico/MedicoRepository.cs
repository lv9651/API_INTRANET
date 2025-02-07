using Dapper;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using SISLAB_API.Areas.Maestros.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

public class MedicoRepository
{
    private readonly string? _connectionString;

    // Constructor que obtiene la cadena de conexión desde la configuración
    public MedicoRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("SqlServersige");
    }

    // Método para obtener especialidades
    public async Task<IEnumerable<Medico>> ObtenerEspecialidadesAsync()
    {
        var especialidades = new List<Medico>();

        // Consulta SQL adaptada para SQL Server
        string query = @"
            SELECT med_esp.descripcion
            FROM medico.Medico Med
            INNER JOIN Medico.Especialidad med_esp 
                ON Med.idespecialidad = med_esp.idespecialidad
            INNER JOIN General.Usuario Us 
                ON Med.idusuario = Us.idusuario
            GROUP BY med_esp.descripcion";

        // Usando Dapper para ejecutar la consulta y mapear los resultados
        using (var connection = new SqlConnection(_connectionString))  // Usando SqlConnection para SQL Server
        {
            await connection.OpenAsync();
            var result = await connection.QueryAsync<Medico>(query);  // Dapper ejecutará la consulta

            especialidades = result.AsList();  // Convertir el resultado a una lista
        }

        return especialidades;  // Retornar la lista de especialidades
    }




    public async Task<IEnumerable<Sucursal>> ObtenerSucursalesAsync()
    {
        var especialidades = new List<Sucursal>();

        // Consulta SQL adaptada para SQL Server
        string query = @"select idsucursal,descripcion from general.sucursal where descripcion like '%vinali%'";

        // Usando Dapper para ejecutar la consulta y mapear los resultados
        using (var connection = new SqlConnection(_connectionString))  // Usando SqlConnection para SQL Server
        {
            await connection.OpenAsync();
            var result = await connection.QueryAsync<Sucursal>(query);  // Dapper ejecutará la consulta

            especialidades = result.AsList();  // Convertir el resultado a una lista
        }

        return especialidades;  // Retornar la lista de especialidades
    }



    public async Task<IEnumerable<Med_bus>> BuscarMedicoPorEspecialidadAsync(string descripcionEspecialidad)
    {
        var médicos = new List<Med_bus>();

        // Llamamos al procedimiento almacenado
        string query = "buscar_medico_x_especialidad";  // Nombre del procedimiento almacenado

        // Usamos Dapper para ejecutar el procedimiento almacenado
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var result = await connection.QueryAsync<Med_bus>(
                query,
                new { descripcion = descripcionEspecialidad }, // Pasamos el parámetro @descripcion
                commandType: System.Data.CommandType.StoredProcedure  // Especificamos que es un procedimiento almacenado
            );

            médicos = result.AsList();
        }

        return médicos;  // Retorna la lista de médicos
    }


    public async Task<IEnumerable<Dependiente>> BuscarDependienteAsync(string dni)
    {
        var médicos = new List<Dependiente>();

        // Llamamos al procedimiento almacenado
        string query = "buscar_dependiente";  // Nombre del procedimiento almacenado

        // Usamos Dapper para ejecutar el procedimiento almacenado
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var result = await connection.QueryAsync<Dependiente>(
                query,
                new { dni = dni }, // Pasamos el parámetro @descripcion
                commandType: System.Data.CommandType.StoredProcedure  // Especificamos que es un procedimiento almacenado
            );

            médicos = result.AsList();
        }

        return médicos;  // Retorna la lista de médicos
    }


    public async Task<IEnumerable<Paciente>> BuscaridPacienteAsync(string dni)
    {
        var médicos = new List<Paciente>();

        // Llamamos al procedimiento almacenado
        string query = "bus_pac";  // Nombre del procedimiento almacenado

        // Usamos Dapper para ejecutar el procedimiento almacenado
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var result = await connection.QueryAsync<Paciente>(
                query,
                new { dni = dni }, // Pasamos el parámetro @descripcion
                commandType: System.Data.CommandType.StoredProcedure  // Especificamos que es un procedimiento almacenado
            );

            médicos = result.AsList();
        }

        return médicos;  // Retorna la lista de médicos
    }

    public async Task<IEnumerable<Bus_cita_cliente>> Busq_cita_clienteAsync(string documento)
    {
        var BusqCita = new List<Bus_cita_cliente>();

        // Llamamos al procedimiento almacenado
        string query = "mostrar_cita_guardadas_cliente";  // Nombre del procedimiento almacenado

        // Usamos Dapper para ejecutar el procedimiento almacenado
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var result = await connection.QueryAsync<Bus_cita_cliente>(
                query,
                new { idcliente = documento }, // Pasamos el parámetro @descripcion
                commandType: System.Data.CommandType.StoredProcedure  // Especificamos que es un procedimiento almacenado
            );

            BusqCita = result.AsList();
        }

        return BusqCita;  // Retorna la lista de médicos
    }

    public async Task<IEnumerable<PacienteDependiente>> Busq_PacienteDependienteAsync(string documento)
    {
        var BusqCita = new List<PacienteDependiente>();

        // Llamamos al procedimiento almacenado
        string query = "bus_depend_paciente";  // Nombre del procedimiento almacenado

        // Usamos Dapper para ejecutar el procedimiento almacenado
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var result = await connection.QueryAsync<PacienteDependiente>(
                query,
                new { dni = documento }, // Pasamos el parámetro @descripcion
                commandType: System.Data.CommandType.StoredProcedure  // Especificamos que es un procedimiento almacenado
            );

            BusqCita = result.AsList();
        }

        return BusqCita;  // Retorna la lista de médicos
    }


    public async Task<IEnumerable<Historial_clinico>> Busq_HistorialClinicoAsync(string documento)
    {
        var BusqCita = new List<Historial_clinico>();

        // Llamamos al procedimiento almacenado
        string query = "[Citas].[sp_obtener_historialclinicoxidpaciente]";  // Nombre del procedimiento almacenado

        // Usamos Dapper para ejecutar el procedimiento almacenado
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var result = await connection.QueryAsync<Historial_clinico>(
                query,
                new { idpaciente = documento }, // Pasamos el parámetro @descripcion
                commandType: System.Data.CommandType.StoredProcedure  // Especificamos que es un procedimiento almacenado
            );

            BusqCita = result.AsList();
        }

        return BusqCita;  // Retorna la lista de médicos
    }



    public async Task<int> InsertarFammedAsync(Reg_pariente request)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            var parameters = new DynamicParameters();
            parameters.Add("@json", request.Json);

            // Ejecutar el procedimiento almacenado e insertar los datos
            var cliCodigo = await connection.QuerySingleOrDefaultAsync<int>(
                "insertar_paciente_familiar",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return cliCodigo;  // Retornar el cli_codigo insertado
        }
    }











public async Task<IEnumerable<Tipo_pago>> ObtenerTipoPago()
    {
        var especialidades = new List<Tipo_pago>();

        // Consulta SQL adaptada para SQL Server
        string query = "General.sp_listar_tipopago_combo";

        // Usando Dapper para ejecutar la consulta y mapear los resultados
        using (var connection = new SqlConnection(_connectionString))  // Usando SqlConnection para SQL Server
        {
            await connection.OpenAsync();
            var result = await connection.QueryAsync<Tipo_pago>(query);  // Dapper ejecutará la consulta

            especialidades = result.AsList();  // Convertir el resultado a una lista
        }

        return especialidades;  // Retornar la lista de especialidades
    }




    public async Task<IEnumerable<Busq_dia_med>> BuscardiaMedicoAsync(string colegio)
    {
        var médicos = new List<Busq_dia_med>();

        // Llamamos al procedimiento almacenado
        string query = "buscar_dia_medico";  // Nombre del procedimiento almacenado

        // Usamos Dapper para ejecutar el procedimiento almacenado
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var result = await connection.QueryAsync<Busq_dia_med>(
                query,
                new { colegio = colegio }, // Pasamos el parámetro @descripcion
                commandType: System.Data.CommandType.StoredProcedure  // Especificamos que es un procedimiento almacenado
            );

            médicos = result.AsList();
        }

        return médicos;  // Retorna la lista de médicos
    }



    // Método para ejecutar el procedimiento almacenado
    public async Task<IEnumerable<Hor_dia_med>> BuscarDiaHoraMedicoAsync(string fecha, string colegio,string idmodalidad)
    {
        var horarioDiaMedicoList = new List<Hor_dia_med>();

        using (var connection = new SqlConnection(_connectionString)) // Usamos SqlConnection para SQL Server
        {
            await connection.OpenAsync();

            using (var command = new SqlCommand("horario_dia_medico", connection)) // Usamos SqlCommand para SQL Server
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@fecha", fecha);
                command.Parameters.AddWithValue("@colegio", colegio);
                command.Parameters.AddWithValue("@idmodalidad", idmodalidad);


                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var horarioDiaMedico = new Hor_dia_med
                        {
                            idhorariomedicodividido  = reader["idhorariomedicodividido"].ToString(),
                            horainicio = reader["horainicio"].ToString(),
                            fecha = Convert.ToDateTime(reader["fecha"]),
                            idestado = reader["idestado"].ToString()
                        };

                        horarioDiaMedicoList.Add(horarioDiaMedico);
                    }
                }
            }
        }

        return horarioDiaMedicoList;
    }

}