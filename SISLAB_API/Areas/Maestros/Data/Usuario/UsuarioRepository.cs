using Dapper;
using MySql.Data.MySqlClient;
using SISLAB_API.Areas.Maestros.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
namespace SISLAB_API.Areas.Maestros.Repositories
{
    public class UsuarioRepository
    {
        private readonly string _connectionString;

        // Constructor para recibir la cadena de conexión
        public UsuarioRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SqlServersige");

        }

        // Método para insertar un usuario en la base de datos
        public string InsertarClinicaUser(Usuario user)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand("InsertarClinicaUser", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Agregar los parámetros del procedimiento almacenado
                    command.Parameters.AddWithValue("@TipoDocumento", user.TipoDocumento);
                    command.Parameters.AddWithValue("@NumeroDocumento", user.NumeroDocumento);
                    command.Parameters.AddWithValue("@Nombres", user.Nombres);
                    command.Parameters.AddWithValue("@ApellidoPaterno", user.ApellidoPaterno);
                    command.Parameters.AddWithValue("@ApellidoMaterno", user.ApellidoMaterno);
                    command.Parameters.AddWithValue("@Telefono", user.Telefono);
                    command.Parameters.AddWithValue("@Correo", user.Correo);
                    command.Parameters.AddWithValue("@Contrasena", user.Contrasena); // Asumimos que la contraseña ya está cifrada
                    command.Parameters.AddWithValue("@AceptoTerminos", user.AceptoTerminos);
                    command.Parameters.AddWithValue("@AutorizoDatos", user.AutorizoDatos);

                    // Ejecutar el procedimiento almacenado y obtener el ID del usuario insertado
                    return Convert.ToString(command.ExecuteScalar());
                }
            }
        }

        public async Task<string> ObtenerCorreoPorDniAsync(string dni)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                // Usamos Dapper para consultar la base de datos
                var query = "SELECT email FROM [BACKUPQF1908].[dbo].[CLIENTE_TERCERO]  WHERE nrodocumento = @Dni";
                var result = await connection.QueryFirstOrDefaultAsync<string>(query, new { Dni = dni });

                return result; // Retorna el correo asociado al DNI, o null si no se encuentra
            }
        }


        public async Task<string> ObtenerCodigoPorDniAsync(string dni)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                // Usamos Dapper para consultar la base de datos
                var query = "SELECT Codigorecuperacion FROM [BACKUPQF1908].[dbo].[CLIENTE_TERCERO]  WHERE nrodocumento = @Dni";
                var result = await connection.QueryFirstOrDefaultAsync<string>(query, new { Dni = dni });

                return result; // Retorna el correo asociado al DNI, o null si no se encuentra
            }
        }


        // Método para insertar o actualizar el código de recuperación
        public async Task<bool> ActualizarCodigoRecuperacionAsync(string dni, string nuevaContraseña)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "UPDATE [BACKUPQF1908].[dbo].[CLIENTE_TERCERO]  SET Codigorecuperacion = @NuevaContraseña WHERE nrodocumento = @Dni";

                var rowsAffected = await connection.ExecuteAsync(query, new { Dni = dni, NuevaContraseña = nuevaContraseña });

                return rowsAffected > 0; // Retorna true si se actualizó al menos una fila
            }
        }






        public async Task<bool> ActualizarClaveAsync(string dni, string nuevaContraseña)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "UPDATE [BACKUPQF1908].[dbo].[CLIENTE_TERCERO]  SET clave = @NuevaContraseña WHERE nrodocumento = @Dni";

                var rowsAffected = await connection.ExecuteAsync(query, new { Dni = dni, NuevaContraseña = nuevaContraseña });

                return rowsAffected > 0; // Retorna true si se actualizó al menos una fila
            }
        }

        // Método para obtener un usuario por documento y clave
        public async Task<dynamic> GetUsuarioByDocumentoYClaveAsync(string dni, string clave)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                // Ejecuta el procedimiento almacenado y devuelve el resultado como un dynamic
                var result = await connection.QueryFirstOrDefaultAsync<dynamic>(
                    "sp_ValidarUsuarioYClave",
                    new { nrodocumento = dni, clave = clave },
                    commandType: CommandType.StoredProcedure
                );

                return result;
            }




    }



    }















}