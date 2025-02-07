public class User
{
    public int UsuarioID { get; set; }
    public string TipoDocumento { get; set; }
    public string NumeroDocumento { get; set; }
    public string Nombres { get; set; }
    public string ApellidoPaterno { get; set; }
    public string ApellidoMaterno { get; set; }
    public string Telefono { get; set; }
    public string Correo { get; set; }
    public bool AceptoTerminos { get; set; }
    public bool AutorizoDatos { get; set; }
    public DateTime FechaRegistro { get; set; }
    public string Codigorecuperacion { get; set; }
}