using MimeKit;
using MailKit.Net.Smtp;
using System.IO;
using System.Threading.Tasks;

namespace SISLAB_API.Areas.Maestros.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Método para enviar un correo con el código de recuperación
        public async Task EnviarCodigoRecuperacionAsync(string toEmail, string codigo)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Clinica Vinali", _configuration["SmtpSettings:Username"]));
            email.To.Add(new MailboxAddress("", toEmail));
            email.Subject = "Código de Recuperación de Contraseña";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $"<html><body><h1>Tu código de recuperación es:</h1><h2>{codigo}</h2></body></html>"
            };

            email.Body = bodyBuilder.ToMessageBody();

            // Usar MailKit SmtpClient para enviar el correo
            using (var smtpClient = new SmtpClient())
            {
                try
                {
                    await smtpClient.ConnectAsync(_configuration["SmtpSettings:Host"], int.Parse(_configuration["SmtpSettings:Port"]), false);
                    await smtpClient.AuthenticateAsync(_configuration["SmtpSettings:Username"], _configuration["SmtpSettings:Password"]);
                    await smtpClient.SendAsync(email);
                    await smtpClient.DisconnectAsync(true);
                    Console.WriteLine("Correo enviado exitosamente.");
                }
                catch (Exception ex)
                {
                    // Manejar errores de conexión o autenticación
                    Console.WriteLine($"Error al enviar el correo: {ex.Message}");
                }
            }
        }

        // Método para enviar un correo con imagen embebida y adjunta (opcional)
        public async Task SendEmailWithImageAsync(string toEmail, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Clinica Vinali", _configuration["SmtpSettings:Username"]));
            email.To.Add(new MailboxAddress("", toEmail));
            email.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = body
            };

            // Ruta fija de la imagen, la puedes configurar dinámicamente desde appsettings.json
            string imagePath = @"\\PANDAFILE\Intranet\clinica\doc.jpg";  // Usar configuración dinámica

            // Verificar si la imagen existe en la ruta especificada
            if (File.Exists(imagePath))
            {
                // Adjuntar la imagen como archivo
                bodyBuilder.Attachments.Add(imagePath);

                // O bien, para incrustar la imagen directamente en el cuerpo del correo:
                var image = bodyBuilder.LinkedResources.Add(imagePath);
                image.ContentId = "unique-image-id";  // Este ID se usará en el cuerpo HTML

                // Modificar el cuerpo HTML para incluir la imagen embebida
                bodyBuilder.HtmlBody = $"<html><body><h1>{body}</h1><img src='cid:{image.ContentId}' /></body></html>";
            }
            else
            {
                // Manejar el caso si la imagen no existe
                bodyBuilder.HtmlBody = $"<html><body><h1>{body}</h1><p>La imagen no está disponible en la ruta especificada.</p></body></html>";
            }

            email.Body = bodyBuilder.ToMessageBody();

            // Usar MailKit SmtpClient para enviar el correo
            using (var smtpClient = new SmtpClient())
            {
                try
                {
                    await smtpClient.ConnectAsync(_configuration["SmtpSettings:Host"], int.Parse(_configuration["SmtpSettings:Port"]), false);
                    await smtpClient.AuthenticateAsync(_configuration["SmtpSettings:Username"], _configuration["SmtpSettings:Password"]);
                    await smtpClient.SendAsync(email);
                    await smtpClient.DisconnectAsync(true);
                    Console.WriteLine("Correo enviado exitosamente.");
                }
                catch (Exception ex)
                {
                    // Manejar errores de conexión o autenticación
                    Console.WriteLine($"Error al enviar el correo: {ex.Message}");
                }
            }
        }
    }
}