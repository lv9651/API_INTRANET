using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

public class CorreoService
{
    private readonly IConfiguration _configuration;

    public CorreoService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public async Task EnviarDetallesCitaAsync(CitaMedica citaMedica)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress("Clinica Vinali", _configuration["SmtpSettings:Username"]));
        email.To.Add(new MailboxAddress("", citaMedica.Email));
        email.Subject = $"Cita Médica Programada para {citaMedica.Nombre}";

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = $@"
            <html>
                <body>
                    <h1>Detalles de tu Cita Médica:</h1>
                    <p><strong>Especialidad:</strong> {citaMedica.Especialidad}</p>
                    <p><strong>Médico:</strong> {citaMedica.Medico}</p>
                    <p><strong>Sucursal:</strong> {citaMedica.Sucursal}</p>
                    <p><strong>Día:</strong> {citaMedica.Dia}</p>
                    <p><strong>Hora:</strong> {citaMedica.Hora}</p>
                </body>
            </html>"
        };

        email.Body = bodyBuilder.ToMessageBody();

        using (var smtpClient = new SmtpClient())
        {
            try
            {
                // Conectar usando el puerto 587 y STARTTLS
                await smtpClient.ConnectAsync(_configuration["SmtpSettings:Host"], int.Parse(_configuration["SmtpSettings:Port"]), SecureSocketOptions.StartTls);
                await smtpClient.AuthenticateAsync(_configuration["SmtpSettings:Username"], _configuration["SmtpSettings:Password"]);
                await smtpClient.SendAsync(email);
                Console.WriteLine("Correo enviado exitosamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al enviar el correo: {ex.Message}");
            }
            finally
            {
                if (smtpClient.IsConnected)
                {
                    await smtpClient.DisconnectAsync(true);
                }
                smtpClient.Dispose();
            }
        }
    }
}