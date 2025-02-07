namespace SISLAB_API.Areas.Maestros.Models
{
    public class Usuario
    {
         // ID único para cada usuario
        public string TipoDocumento { get; set; }  // Tipo de documento (DNI, Pasaporte, etc.)
        public string NumeroDocumento { get; set; } // Número de documento
        public string Nombres { get; set; }       // Nombres del usuario
        public string ApellidoPaterno { get; set; } // Apellido paterno
        public string ApellidoMaterno { get; set; } // Apellido materno
        public string Telefono { get; set; }      // Número de teléfono
        public string Correo { get; set; }        // Correo electrónico
        public string Contrasena { get; set; }    // Contraseña del usuario
        public bool AceptoTerminos { get; set; }  // Aceptación de términos
        public bool AutorizoDatos { get; set; }   // Autorización de uso de datos
      
    }
}
