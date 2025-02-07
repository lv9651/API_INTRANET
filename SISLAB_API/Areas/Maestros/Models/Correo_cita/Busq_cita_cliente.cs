

using Org.BouncyCastle.Asn1.X509;

public class Bus_cita_cliente
{
    public string idcliente { get; set; }
    public string serie { get; set; }
    public string numdocumento { get; set; }
    public string total { get; set; }
    public string subtotal { get; set; }
    public string descripcion { get; set; }  // Se agrega la propiedad Email

    public string nombres { get; set; }

    public string apellidopaterno { get; set; }

    public string apellidomaterno { get; set; }

    public DateTime fechacreacion { get; set; }

    public TimeSpan horainicio { get; set; }
    public string especialidad { get; set; }
    public string parentesco { get; set; }
}