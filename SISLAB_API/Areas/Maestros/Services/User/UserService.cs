using SISLAB_API.Areas.Maestros.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public class UserService
{
    private readonly UserRepository _userRepository;

    // Inyección de dependencias para el repositorio
    public UserService(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    // Obtener todos los usuarios
    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _userRepository.GetAllUsersAsync();
    }

    // Obtener todos los roles
    public async Task<IEnumerable<Role>> GetAllRolsAsync()
    {
        return await _userRepository.GetAllRolAsync();
    }

    public async Task<string> GetEmailByDni(string dni)
    {
        // Aquí llamamos a GetEmailByDniAsync que devuelve un string (el correo)
        var email = await _userRepository.GetEmailByDniAsync(dni);

        if (string.IsNullOrEmpty(email))
        {
            throw new InvalidOperationException($"No se encontró un correo para el DNI {dni}.");
        }

        return email;
    }


    public async Task<bool> UpdatePassword(UserPasswordUpdate model)
    {
        var affectedRows = await _userRepository.UpdateUserPassword(model.Dni, model.NewPassword);
        return affectedRows > 0;
    }
    // Método para actualizar un usuario


    // Aquí podrías agregar más métodos según lo necesites (ej. agregar, eliminar usuarios)
}