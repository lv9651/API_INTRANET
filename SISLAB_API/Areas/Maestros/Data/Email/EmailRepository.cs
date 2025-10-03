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
    private readonly string _fromEmail = "lvelasquez@qf.com.pe"; // Tu correo
    private readonly string _fromPassword = "Luis@2023$";





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
}