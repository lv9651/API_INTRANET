using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class VentaController : ControllerBase
{
    private readonly VentaService _ventaService;

    // Constructor que inyecta el servicio de venta
    public VentaController(VentaService ventaService)
    {
        _ventaService = ventaService;
    }

    // Endpoint para crear una venta
    [HttpPost]
    public async Task<IActionResult> CrearVenta([FromBody] VentaRequest request)
    {
        if (request == null)
        {
            return BadRequest("Los datos de la venta son necesarios.");
        }

        // Llamar al servicio para crear la venta
        var response = await _ventaService.CrearVentaAsync(request);

        // Si hubo un error, retornar el mensaje
        if (response.IdVenta == 0)
        {
            return BadRequest(response.Message);
        }

        // Si la venta fue creada exitosamente
        return Ok(response);
    }
}