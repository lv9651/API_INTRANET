public class VentaRequest
{
    public int IdSucursal { get; set; }
    public int IdDocumentoTributario { get; set; }
    public string JsonVenta { get; set; }
    public string Json { get; set; }
    public string JsonVentaPago { get; set; }
    public string jsonventaDetalle { get; set; }
    public int UsuarioManipula { get; set; }
    public int IdCita { get; set; }

    public int idpaciente { get; set; }
    public int IdHorarioMedicoDividido { get; set; }
}

public class VentaResponse
{
    public int IdVenta { get; set; }
    public int IdCita { get; set; }
    public string Message { get; set; }
}