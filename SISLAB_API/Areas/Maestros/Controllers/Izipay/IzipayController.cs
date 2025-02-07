// Controllers/PaymentController.cs
using Microsoft.AspNetCore.Mvc;
using YourNamespace.Models;
using YourNamespace.Services;
using System.Threading.Tasks;

namespace YourNamespace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IzipayService _izipayService;

        // Constructor que inyecta el servicio
        public PaymentController(IzipayService izipayService)
        {
            _izipayService = izipayService;
        }

        [HttpPost("process-payment")]
        public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequest paymentRequest)
        {
            if (paymentRequest == null)
            {
                return BadRequest("Los datos del pago no son válidos.");
            }

            // Llamar al servicio para procesar el pago
            var result = await _izipayService.ProcessPaymentAsync(paymentRequest);

            // Devolver la respuesta al cliente
            if (result.StartsWith("Pago procesado con éxito"))
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
    }
}