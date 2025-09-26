using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using SISLAB_API.Areas.Maestros.Models;

public class DataTransferService
{
    private readonly string _sqlServerConnectionString;
    private readonly string _mysqlConnectionString;

    public DataTransferService(IConfiguration configuration)
    {
        _sqlServerConnectionString = configuration.GetConnectionString("SqlServerConnection");
        _mysqlConnectionString = configuration.GetConnectionString("SislabConnection");
    }

    public async Task TransferDataAsync()
    {
        try
        {
            using (var sqlConnection = new SqlConnection(_sqlServerConnectionString))
            {
                await sqlConnection.OpenAsync();

                var sqlQuery = "SELECT DOCIDEN as dni, DOCIDEN as username , DOCIDEN as password,\r\nCONCAT(NOMBRE,' ',APEPAT,' ',APEMAT)NOMBRE,1 as role_id,EMAIL\r\n from TrabajadorIntranet \r\n where SITUACIÓN=1\r\n union all\r\nSELECT DOCIDEN as dni, DOCIDEN as username , DOCIDEN as password,\r\nCONCAT(NOMBRE,' ',APEPAT,' ',APEMAT)NOMBRE,1 as role_id,EMAIL\r\nfrom PLMEDSOL.DBO.TrabajadorIntranet  where SITUACIÓN=1  union all\r\nSELECT DOCIDEN as dni, DOCIDEN as username , DOCIDEN as password,\r\nCONCAT(NOMBRE,' ',APEPAT,' ',APEMAT)NOMBRE ,1 as role_id,EMAIL from PLQUIMSA.DBO.TrabajadorIntranet \r\n where SITUACIÓN=1 UNION ALL SELECT DOCIDEN AS dni, DOCIDEN AS username, DOCIDEN AS password,\r\n       CONCAT(NOMBRE, ' ', APEPAT, ' ', APEMAT) AS NOMBRE,\r\n       1 AS role_id,\r\n       EMAIL\r\nFROM PLDIST.DBO.TrabajadorIntranet\r\nWHERE SITUACIÓN = 1;";

                using (var command = new SqlCommand(sqlQuery, sqlConnection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    using (var mysqlConnection = new MySqlConnection(_mysqlConnectionString))
                    {
                        await mysqlConnection.OpenAsync();

                        while (await reader.ReadAsync())
                        {
                            var usuario = new Usuario
                            {
                                dni = reader.GetString(0),
                                Username = reader.GetString(1),
                                clave = reader.GetString(2),
                                nombres = reader.GetString(3),
                                correo = reader.GetString(5),
                            };

                            var insertQuery = "INSERT INTO users (correo,dni, username, password,role_id,NOMBRE) VALUES (@correo,@dni, @Username, @clave,1,@nombres)";
                            using (var insertCommand = new MySqlCommand(insertQuery, mysqlConnection))
                            {
                                insertCommand.Parameters.AddWithValue("@dni", usuario.dni);
                                insertCommand.Parameters.AddWithValue("@Username", usuario.Username);
                                insertCommand.Parameters.AddWithValue("@clave", usuario.clave);
                                insertCommand.Parameters.AddWithValue("@nombres", usuario.nombres);
                                insertCommand.Parameters.AddWithValue("@correo", usuario.correo);

                                await insertCommand.ExecuteNonQueryAsync();
                            }
                        }
                    }
                }
            }
        }
        catch (SqlException sqlEx)
        {
            // Manejo específico de errores de SQL Server
            throw new InvalidOperationException($"Error al acceder a SQL Server: {sqlEx.Message}", sqlEx);
        }
        catch (MySqlException mySqlEx)
        {
            // Manejo específico de errores de MySQL
            throw new InvalidOperationException($"Error al acceder a MySQL: {mySqlEx.Message}", mySqlEx);
        }
        catch (Exception ex)
        {
            // Manejo general de otros errores
            throw new InvalidOperationException($"Error inesperado: {ex.Message}", ex);
        }
    }
}