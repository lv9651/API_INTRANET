using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

public class VentaRepository
{
    private readonly string _connectionString;

    // Constructor que obtiene la cadena de conexión desde la configuración
    public VentaRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("SqlServersige");
    }

    // Método para insertar una venta
    public async Task<int> InsertarVentaAsync(VentaRequest request)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            var parameters = new DynamicParameters();
            parameters.Add("@idsucursal", request.IdSucursal);
            parameters.Add("@iddocumentotributario", request.IdDocumentoTributario);
            parameters.Add("@jsonventa", request.JsonVenta);
            parameters.Add("@json", request.Json);
            parameters.Add("@jsonventapago", request.JsonVentaPago);
            parameters.Add("@@jsonventaDetalle", request.jsonventaDetalle);
            parameters.Add("@usuariomanipula", request.UsuarioManipula);
            parameters.Add("@idcita", request.IdCita);
            parameters.Add("@idpaciente", request.idpaciente);
            parameters.Add("@idhorariomedicodividido", request.IdHorarioMedicoDividido);
            parameters.Add("@idventa", dbType: DbType.Int32, direction: ParameterDirection.Output);

            // Ejecutar el procedimiento almacenado
            await connection.ExecuteAsync("InsertarVenta_Cita_web", parameters, commandType: CommandType.StoredProcedure);

            // Retornar el ID de la venta generado
            return parameters.Get<int>("@idventa");
        }
    }
}