namespace SISLAB_API.Areas.Maestros.Models
{
    public class Sugerencia
    {
        public int id { get; set; }
        public string empleado_dni { get; set; }
        public string empleado_nombre { get; set; }  // Lo traes con JOIN
        public string tipo { get; set; }
        public string mensaje { get; set; }
        public DateTime fecha_envio { get; set; }
        public bool revisado { get; set; }
        public DateTime? fecha_revision { get; set; }
        public string revisado_por_dni { get; set; }
        public string respuesta_rh { get; set; }  // Si agregaste ese campo
    }

    public class CrearSugerenciaRequest
    {
        public string EmpleadoDni { get; set; }
        public string Tipo { get; set; }
        public string Mensaje { get; set; }
    }
}