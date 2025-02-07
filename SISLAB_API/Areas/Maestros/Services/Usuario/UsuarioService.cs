using SISLAB_API.Areas.Maestros.Models;
using SISLAB_API.Areas.Maestros.Repositories;

namespace SISLAB_API.Areas.Maestros.Services
{
    public class UsuarioService
    {
        private readonly UsuarioRepository _clinicaUserRepository;
        private readonly EmailService _emailService;

        // Constructor que inyecta el repositorio
        public UsuarioService(UsuarioRepository clinicaUserRepository, EmailService emailService)
        {
            _clinicaUserRepository = clinicaUserRepository;
            _emailService = emailService;
        }

        // Método para insertar un usuario
        public string InsertarClinicaUser(Usuario user)
        {
            // Llamar al repositorio para insertar el usuario
            return _clinicaUserRepository.InsertarClinicaUser(user);
        }



        public async Task<string> RecuperarCorreoPorDniAsync(string dni)
        {
            var correo = await _clinicaUserRepository.ObtenerCorreoPorDniAsync(dni);
            if (string.IsNullOrEmpty(correo))
            {
                return null; // Retorna null si no se encontró un correo para el DNI
            }

            // Generar un código aleatorio de 6 dígitos
            var codigo = GenerarCodigoAleatorio();

            // Almacenar o actualizar el código en la base de datos
            await _clinicaUserRepository.ActualizarCodigoRecuperacionAsync(dni, codigo);

            // Enviar el código por correo
            await _emailService.EnviarCodigoRecuperacionAsync(correo, codigo);

            return correo;
        }

        // Método para generar un código aleatorio de 6 dígitos
        private string GenerarCodigoAleatorio()
        {
            var random = new Random();
            var codigo = random.Next(100000, 999999).ToString(); // Genera un número aleatorio de 6 dígitos
            return codigo;
        }

        public async Task<bool> ValidarCodigoAsync(string dni, string codigoRecuperacion)
        {
            // Obtener el código de recuperación almacenado en la base de datos para el DNI
            string codigoAlmacenado = await _clinicaUserRepository.ObtenerCodigoPorDniAsync(dni);

            // Si el código almacenado es igual al código recibido, entonces es válido
            return codigoRecuperacion == codigoAlmacenado;
        }
        public async Task<bool> CambiarContraseñaAsync(string dni, string nuevaContraseña)
        {
            // Validar que la nueva contraseña no esté vacía o nula
            if (string.IsNullOrEmpty(nuevaContraseña))
            {
                return false;
            }

            // Llamar al repositorio para actualizar la contraseña
            bool resultado = await _clinicaUserRepository.ActualizarClaveAsync(dni, nuevaContraseña);

            return resultado; // Retorna true si la contraseña fue actualizada exitosamente, false en caso contrario
        }


        public async Task<dynamic> AutenticarUsuarioAsync(string dni, string clave)
        {
            // Llamar al repositorio para obtener el resultado del procedimiento almacenado
            return await _clinicaUserRepository.GetUsuarioByDocumentoYClaveAsync(dni, clave);
        }


    }

    // Método para autenticar al usuario

 
}