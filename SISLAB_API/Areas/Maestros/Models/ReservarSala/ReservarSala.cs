namespace SISLAB_API.Areas.Maestros.Models
{
    public class Sala
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Capacidad { get; set; }
        public string Ubicacion { get; set; }
        public bool Activa { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class Reserva
    {
        public int Id { get; set; }
        public string UsuarioDni { get; set; }
        public string UsuarioNombre { get; set; }
        public string UsuarioEmail { get; set; }
        public int SalaId { get; set; }
        public string SalaNombre { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public string Area { get; set; }
        public string AreaNombre { get; set; }
        public string Motivo { get; set; }
        public string Estado { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ReservaRequest
    {
        public string UsuarioDni { get; set; }
        public string UsuarioNombre { get; set; }
        public string UsuarioEmail { get; set; }
        public int SalaId { get; set; }
        public DateTime Fecha { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFin { get; set; }
        public string Area { get; set; }
        public string AreaNombre { get; set; }
        public string Motivo { get; set; }
    }
}
