public class VentaService
{
    private readonly VentaRepository _ventaRepository;

    // Constructor que inyecta el repositorio
    public VentaService(VentaRepository ventaRepository)
    {
        _ventaRepository = ventaRepository;
    }

    // Método que maneja la lógica de negocio
    public async Task<VentaResponse> CrearVentaAsync(VentaRequest request)
    {
        try
        {
            // Insertar la venta usando el repositorio
            int idVenta = await _ventaRepository.InsertarVentaAsync(request);

            // Retornar respuesta exitosa
            return new VentaResponse
            {
                IdVenta = idVenta,
                IdCita = request.IdCita,
                Message = "Venta creada exitosamente."
            };
        }
        catch (Exception ex)
        {
            // En caso de error, retornar mensaje de error
            return new VentaResponse
            {
                IdVenta = 0,
                Message = $"Error al crear la venta: {ex.Message}"
            };
        }
    }
}