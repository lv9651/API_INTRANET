// Repositories/EmailRepository.cs
using MySql.Data.MySqlClient;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using MySql.Data.MySqlClient; // Use MySqlConnection for MySQL
using System.Data;
using Dapper;
public class EmailRepository
{
    private readonly string _smtpServer = "mail.qf.com.pe"; // Cambia según tu proveedor
    private readonly int _smtpPort = 587; // Cambia según tu proveedor
    private readonly string _fromEmail = "notificacionquantia@qf.com.pe"; // Tu correo
    private readonly string _fromPassword = "QFsistemas26%";





    private readonly string _connectionString;

    // Constructor to inject IConfiguration and get the connection string
    public EmailRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("SislabConnection");
    }




    public async Task<string> GetEmailByDniAsync(string dni)
    {
        using var conn = new MySqlConnection(_connectionString);
        string sql = "SELECT correo FROM users WHERE dni = @dni LIMIT 1;";
        return await conn.ExecuteScalarAsync<string>(sql, new { dni });
    }

    // 📧 Enviar email sin adjunto
    public async Task SendEmailSugAsync(string toEmail, string subject, string body)
    {
        using (var client = new SmtpClient(_smtpServer, _smtpPort))
        {
            client.Credentials = new NetworkCredential(_fromEmail, _fromPassword);
            client.EnableSsl = true;

            using (var mailMessage = new MailMessage())
            {
                mailMessage.From = new MailAddress(_fromEmail);
                mailMessage.Subject = subject;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = false;
                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
            }
        }
    }
    /* public async Task SendEmailAsync(string toEmail, string subject, string body, IFormFile file)*/
    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        using (var client = new SmtpClient(_smtpServer, _smtpPort))
        {
            client.Credentials = new NetworkCredential(_fromEmail, _fromPassword);
            client.EnableSsl = true;

            using (var mailMessage = new MailMessage())
            {
                mailMessage.From = new MailAddress(_fromEmail);
                mailMessage.Subject = subject;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = false;
                mailMessage.To.Add(toEmail);

                // Si hay un archivo, se agrega como adjunto
             /*   if (file != null && file.Length > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    using (var stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);
                        mailMessage.Attachments.Add(new Attachment(new MemoryStream(stream.ToArray()), fileName));
                    }
                }*/

                await client.SendMailAsync(mailMessage);
            }
        }
    }
    public async Task SendEmailHtmlAsync(string toEmail, string subject, string bodyHtml)
    {
        using (var client = new SmtpClient(_smtpServer, _smtpPort))
        {
            client.Credentials = new NetworkCredential(_fromEmail, _fromPassword);
            client.EnableSsl = true;

            using (var mailMessage = new MailMessage())
            {
                mailMessage.From = new MailAddress(_fromEmail);
                mailMessage.Subject = subject;
                mailMessage.Body = bodyHtml;
                mailMessage.IsBodyHtml = true;  // HTML activado
                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
            }
        }
    }

    // 📧 Enviar correo de confirmación de reserva
    // 📧 Enviar correo de confirmación de reserva con el formato solicitado
    public async Task SendReservaConfirmacionAsync(
    string toEmail,
    string usuarioNombre,
    string salaNombre,
    DateTime fecha,
    string horaInicio,
    string horaFin,
    string areaNombre,
    string motivo,
    string usuarioDni)
    {
        var subject = $"📅 Reserva de Sala - {salaNombre} - {fecha:dd/MM/yyyy} - {horaInicio} a {horaFin}";

        var bodyHtml = $@"
    <html>
    <head>
        <style>
            body {{ font-family: Arial, sans-serif; }}
            .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
            .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 20px; text-align: center; border-radius: 10px 10px 0 0; }}
            .content {{ background: #f9f9f9; padding: 20px; border-radius: 0 0 10px 10px; }}
            .detalle {{ background: white; padding: 15px; border-radius: 8px; margin: 15px 0; }}
            .detalle-item {{ padding: 8px; border-bottom: 1px solid #eee; }}
            .detalle-item strong {{ display: inline-block; width: 130px; }}
            .footer {{ text-align: center; margin-top: 20px; font-size: 12px; color: #888; }}
        </style>
    </head>
    <body>
        <div class='container'>
            <div class='header'>
                <h2>📅 Programación de Sala de Reuniones</h2>
            </div>
            <div class='content'>
                <p>Estimados,</p>
                
                <p><strong>La presente es para informar la programación de la sala {salaNombre}</strong></p>
                
                <div class='detalle'>
                    <h3>📋 Detalles de la reserva:</h3>
                    <div class='detalle-item'><strong>🔹 Reservado por:</strong> {usuarioNombre} (DNI: {usuarioDni})</div>
                    <div class='detalle-item'><strong>🔹 Ubicacion:</strong> {salaNombre}</div>
                    <div class='detalle-item'><strong>🔹 Fecha:</strong> {fecha:dddd, dd/MM/yyyy}</div>
                    <div class='detalle-item'><strong>🔹 Horario:</strong> {horaInicio} - {horaFin}</div>
                    <div class='detalle-item'><strong>🔹 Área:</strong> {areaNombre}</div>
                    <div class='detalle-item'><strong>🔹 Motivo:</strong> {motivo}</div>
                </div>
                
                <p>📌 <strong>Recomendaciones:</strong></p>
                <ul>
                    <li>Llegar 5 minutos antes</li>
                    <li>Contar con los materiales necesarios</li>
                    <li>Al finalizar, dejar la sala en orden</li>
                </ul>
            </div>
            <div class='footer'>
                <p>Este es un correo automático, por favor no responder.</p>
                <p><strong>Sistema de Reservas - Intranet</strong></p>
            </div>
        </div>
    </body>
    </html>";

        var listaCC = new List<string>
    {
       "pvaldivia@qf.com.pe",
       "contabilidad@qf.com.pe",
       "comercial@qf.com.pe",
       "jennyizarra@qf.com.pe",
       "srivera@qf.com.pe",
       "esoto@qf.com.pe",
       "floayza@qf.com.pe",
       "robregon@vinali.pe",
       "msilva@qf.com.pe",
       "abernilla@qf.com.pe",
       "fgonzales@orvit.pe",
       "mnina@qf.com.pe",
       "aparedes@qf.com.pe",
       "rbalcazar@qf.com.pe",
       "mcadenillas@qf.com.pe",
       "cbravo@qf.com.pe",
       "mquinones@qf.com.pe",
       "kvalverde@qf.com.pe",
       "rruiz@qf.com.pe",
       "jleon@qf.com.pe",
        "lvelasquez@qf.com.pe",     // Administrador
        "desarrollo@qf.com.pe",        // Gerencia
        "soporte@qf.com.pe"  
        // Agrega más aquí
    };

        // Agregar el correo del usuario
        if (!string.IsNullOrEmpty(toEmail))
        {
            listaCC.Add(toEmail);
        }

        // Eliminar duplicados
        listaCC = listaCC.Distinct().ToList();


        // Enviar con el usuario en copia (CC) y sin destinatario principal
        await SendEmailWithCcOnlyAsync(listaCC, null, subject, bodyHtml);
    }

    // 📧 Enviar email con formato HTML y copia (CC)
    public async Task SendEmailWithCcOnlyAsync(List<string> ccEmails, string toEmail, string subject, string bodyHtml)
    {
        using (var client = new SmtpClient(_smtpServer, _smtpPort))
        {
            client.Credentials = new NetworkCredential(_fromEmail, _fromPassword);
            client.EnableSsl = true;

            using (var mailMessage = new MailMessage())
            {
                mailMessage.From = new MailAddress(_fromEmail);
                mailMessage.Subject = subject;
                mailMessage.Body = bodyHtml;
                mailMessage.IsBodyHtml = true;

                // Agregar todos los CC
                if (ccEmails != null && ccEmails.Any())
                {
                    foreach (var cc in ccEmails)
                    {
                        if (!string.IsNullOrWhiteSpace(cc))
                        {
                            mailMessage.CC.Add(cc.Trim());
                        }
                    }
                }

                // Agregar destinatario principal si existe
                if (!string.IsNullOrEmpty(toEmail))
                {
                    mailMessage.To.Add(toEmail);
                }

                await client.SendMailAsync(mailMessage);
            }
        }
    }
    // 📧 Enviar correo de cancelación de reserva
    public async Task SendReservaCancelacionAsync(
     string toEmail,
     string usuarioNombre,
     string salaNombre,
     DateTime fecha,
     string horaInicio,
     string horaFin,
     string usuarioDni)
    {
        var subject = $"❌ Cancelación de Reserva - {salaNombre} - {fecha:dd/MM/yyyy} - {horaInicio} a {horaFin}";

        var bodyHtml = $@"
    <html>
    <head>
        <style>
            body {{ font-family: Arial, sans-serif; }}
            .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
            .header {{ background: #dc3545; color: white; padding: 20px; text-align: center; border-radius: 10px 10px 0 0; }}
            .content {{ background: #f9f9f9; padding: 20px; border-radius: 0 0 10px 10px; }}
            .detalle {{ background: white; padding: 15px; border-radius: 8px; margin: 15px 0; }}
            .detalle-item {{ padding: 8px; border-bottom: 1px solid #eee; }}
            .detalle-item strong {{ display: inline-block; width: 130px; }}
            .footer {{ text-align: center; margin-top: 20px; font-size: 12px; color: #888; }}
        </style>
    </head>
    <body>
        <div class='container'>
            <div class='header'>
                <h2>❌ Cancelación de Sala de Reuniones</h2>
            </div>
            <div class='content'>
                <p>Estimados,</p>
                
                <p><strong>La presente es para informar la cancelación de la reserva de la sala {salaNombre}</strong></p>
                
                <div class='detalle'>
                    <h3>📋 Detalles de la reserva cancelada:</h3>
                    <div class='detalle-item'><strong>🔹 Reservado por:</strong> {usuarioNombre} (DNI: {usuarioDni})</div>
                    <div class='detalle-item'><strong>🔹 Ubicacion:</strong> {salaNombre}</div>
                    <div class='detalle-item'><strong>🔹 Fecha:</strong> {fecha:dddd, dd/MM/yyyy}</div>
                    <div class='detalle-item'><strong>🔹 Horario:</strong> {horaInicio} - {horaFin}</div>
                </div>
                
                <p>✅ El horario queda disponible para nuevas reservas.</p>
                
                <p>📌 <strong>Nota:</strong> Esta cancelación ha sido registrada en el sistema.</p>
            </div>
            <div class='footer'>
                <p>Este es un correo automático, por favor no responder.</p>
                <p><strong>Sistema de Reservas - Intranet</strong></p>
            </div>
        </div>
    </body>
    </html>";

        // Enviar con copia (CC) al usuario y al administrador
        var listaCC = new List<string>
    {
       "pvaldivia@qf.com.pe",
       "contabilidad@qf.com.pe",
       "comercial@qf.com.pe",
       "jennyizarra@qf.com.pe",
       "srivera@qf.com.pe",
       "esoto@qf.com.pe",
       "floayza@qf.com.pe",
       "robregon@vinali.pe",
       "msilva@qf.com.pe",
       "abernilla@qf.com.pe",
       "fgonzales@orvit.pe",
       "mnina@qf.com.pe",
       "aparedes@qf.com.pe",
       "rbalcazar@qf.com.pe",
       "mcadenillas@qf.com.pe",
       "cbravo@qf.com.pe",
       "mquinones@qf.com.pe",
       "kvalverde@qf.com.pe",
       "rruiz@qf.com.pe",
       "jleon@qf.com.pe",
        "lvelasquez@qf.com.pe",     // Administrador
        "desarrollo@qf.com.pe",        // Gerencia
        "soporte@qf.com.pe"          // Soporte
        // Agrega más aquí
    };

        // Agregar el correo del usuario
        if (!string.IsNullOrEmpty(toEmail))
        {
            listaCC.Add(toEmail);
        }

        // Eliminar duplicados
        listaCC = listaCC.Distinct().ToList();


        // Enviar con el usuario en copia (CC) y sin destinatario principal
        await SendEmailWithCcOnlyAsync(listaCC, null, subject, bodyHtml);
    }
}