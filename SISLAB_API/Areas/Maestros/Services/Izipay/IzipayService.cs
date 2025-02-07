
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using YourNamespace.Models;

namespace YourNamespace.Services
{
    public class IzipayService
    {
        private readonly HttpClient _httpClient;

        public IzipayService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

      
        public async Task<string> ProcessPaymentAsync(PaymentRequest request)
        {
            // Verificación si el tipo de pago es 'CREDITO'
            if (request.PaymentType == "CREDITO")
            {
                if (request.CreditAvailable.HasValue && decimal.Parse(request.Amount) > request.CreditAvailable.Value)
                {
                    return "El total supera al crédito disponible.";
                }

                // Procesar la venta directamente (CREDITO)
                return "Venta procesada con crédito disponible.";
            }

            // Verificación si el tipo de pago es 'CONTADO'
            if (request.PaymentType == "CONTADO")
            {
                // Generar transactionId
                var transactionId = request.transactionId;

                // Crear el objeto de configuración similar al `iziConfig` en JavaScript
                var iziConfig = new
                {
                    config = new
                    {
                        transactionId = request.transactionId, // Usar el transactionId generado
                        action = "pay", // Acción de pago
                        merchantCode = "4007701",
                        order = new
                        {
                            orderNumber = request.OrderNumber, // Número de pedido
                            currency = "PEN", // Moneda
                            amount = request.Amount, // Monto total
                            processType = "AT", // Tipo de procesamiento
                            merchantBuyerId = "4007701",
                            payMethod = "CARD,QR", // Métodos de pago
                            dateTimeTransaction = DateTime.Now.ToString("yyyyMMddHHmmssfff") // Fecha y hora de la transacción
                        },
                        billing = new
                        {
                            firstName = request.FirstName,
                            lastName = request.LastName,
                            email = request.Email,
                            phoneNumber = request.PhoneNumber,
                            street = request.Street,
                            city = "Lima",
                            state = "Lima",
                            country = "PE",
                            postalCode = "15038",
                            documentType = "DNI", // Tipo de documento
                            document = request.DocumentNumber // Número de documento
                        }
                    },
                    appearence = new
                    {
                        customize = new
                        {
                            visibility = new
                            {
                                hideResultScreen = true // Esconder pantalla de resultado
                            }
                        }
                    }
                };

                // Crear el cuerpo de la solicitud para la API de Izipay
                var bodyObject = new
                {
                    requestSource = "ECOMMERCE", // Valor fijo
                    merchantCode = "4007701",
                    //4079862

                    orderNumber = request.OrderNumber, // El número de pedido
                    publicKey = "VErethUtraQuxas57wuMuquprADrAHAb",
                    


                    //ZWqVgTV2MZ1HsssPwlEnBbapIuT9B1Q8

                    amount = request.Amount
                };

                // Convertir el objeto a JSON para enviarlo en la solicitud
                var content = new StringContent(JsonSerializer.Serialize(bodyObject), Encoding.UTF8, "application/json");

                // Configurar la solicitud HTTP
                var httpRequest = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("https://sandbox-api-pw.izipay.pe/security/v1/Token/Generate"),
                    Headers =
                    {
                        { "transactionId", transactionId },  // Incluye el transactionId en los headers
                        { "Accept", "application/json" }      // Define el tipo de contenido esperado
                    },
                    Content = content
                };

                try
                {
                    // Enviar la solicitud HTTP
                    using (var response = await _httpClient.SendAsync(httpRequest))
                    {
                        response.EnsureSuccessStatusCode();
                        var responseBody = await response.Content.ReadAsStringAsync();
                        // Procesar la respuesta para obtener el token de autorización
                        var authorizationToken = ExtractTokenFromResponse(responseBody);

                        // A continuación, se podría cargar el formulario de pago utilizando el token obtenido
                        // Aquí, se simula el proceso de checkout, para adaptarlo debes integrarlo a tu interfaz

                        // Simulando el callback del checkout
                        if (!string.IsNullOrEmpty(authorizationToken))
                        {
                            // Llamar al callback de respuesta de pago
                            var responsePayment = new
                            {
                                code = "00", // Suponiendo que el código de respuesta es "00" (éxito)
                                message = "Pago procesado con éxito"
                            };

                            return responseBody;
                        }

                        return "Error al procesar el pago: No se obtuvo el token de autorización.";
                    }
                }
                catch (Exception ex)
                {
                    return $"Error al procesar el pago: {ex.Message}";
                }
            }

            return "Tipo de pago no válido.";
        }

        private string ExtractTokenFromResponse(string responseBody)
        {
            // Implementar el parseo del token desde la respuesta de la API de Izipay
            try
            {
                var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseBody);
                if (jsonResponse.TryGetProperty("response", out JsonElement responseElement) &&
                    responseElement.TryGetProperty("token", out JsonElement tokenElement))
                {
                    return tokenElement.GetString();
                }
            }
            catch (Exception)
            {
                return null;
            }

            return null;
        }
    }
}