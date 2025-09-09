using Dapper;
using MySql.Data.MySqlClient;
using SISLAB_API.Areas.Maestros.Models;
using System.Data;
using System.Threading.Tasks;

public class UserRepository
{
    private readonly string? _connectionString;

    // Constructor que recibe la cadena de conexión desde la configuración
    public UserRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("SislabConnection");
    }

    // Método para crear la conexión con la base de datos
    private MySqlConnection CreateConnection()
    {
        return new MySqlConnection(_connectionString);
    }

    // Método para obtener todos los usuarios
    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        var users = new List<User>();

        using (var connection = CreateConnection())
        {
            await connection.OpenAsync();

            string query = @"
                SELECT u.id, u.dni, u.username, u.password, r.name AS role, u.NOMBRE AS NOMBRE
                FROM users u
                LEFT JOIN roles r ON u.role_id = r.id";

            using (var command = new MySqlCommand(query, connection))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        users.Add(new User
                        {
                            id = reader.GetInt32("id"),
                            dni = reader.GetString("dni"),
                            username = reader.GetString("username"),
                            password = reader.GetString("password"),
                            role = reader.GetString("role"),
                            NOMBRE = reader.GetString("NOMBRE")
                        });
                    }
                }
            }
        }

        return users;
    }



    public async Task<int> UpdateUserPassword(string dni, string newPassword)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_dni", dni);
            parameters.Add("p_new_password", newPassword);

            // Usar QueryFirstOrDefaultAsync para obtener el resultado del SELECT
            var result = await connection.QueryFirstOrDefaultAsync<int>(
                "UpdateUserPassword",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result;
        }
    }
    // Método para obtener todos los roles
    public async Task<IEnumerable<Role>> GetAllRolAsync()
    {
        var roles = new List<Role>();

        using (var connection = CreateConnection())
        {
            await connection.OpenAsync();

            string query = @"SELECT * FROM roles";

            using (var command = new MySqlCommand(query, connection))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        roles.Add(new Role
                        {
                            id = reader.GetInt32("id"),
                            name = reader.GetString("name")
                        });
                    }
                }
            }
        }

        return roles;
    }








    public async Task<string> GetEmailByDniAsync(string dni)
    {
        using (var connection = CreateConnection())
        {
            await connection.OpenAsync();

            string query = "SELECT correo FROM users WHERE dni = @dni LIMIT 1";

            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@dni", dni);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return reader.GetString("correo");
                    }
                    else
                    {
                        return null; // No se encontró un correo para el DNI
                    }
                }
            }
        }
    }

    // Método para actualizar un usuario en la base de datos
    public async Task<bool> UpdateUserAsync(User user)
    {
        using (var connection = CreateConnection())
        {
            await connection.OpenAsync();

            string query = @"
                UPDATE users 
                SET username = @username, password = @password, role_id = @role_id, NOMBRE = @NOMBRE,
     updated_at = NOW()
                WHERE id = @id";

            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@username", user.username);
                command.Parameters.AddWithValue("@password", user.password); // Asegúrate de encriptar la contraseña
                command.Parameters.AddWithValue("@role_id", user.role);
                command.Parameters.AddWithValue("@NOMBRE", user.NOMBRE);
                command.Parameters.AddWithValue("@id", user.id);

                int affectedRows = await command.ExecuteNonQueryAsync();
                return affectedRows > 0; // Si se afectaron filas, significa que se actualizó correctamente
            }
        }
    }
}