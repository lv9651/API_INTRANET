// Services/EmailService.cs
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

public class EmailService
{
    private readonly EmailRepository _emailRepository;

    public EmailService(EmailRepository emailRepository)
    {
        _emailRepository =  emailRepository;
    }

    public async Task SendAvisoRespuestaSugerenciaAsync(string dni, string nombre)
    {
        // 1. Buscar correo del empleado
        var correo = await _emailRepository.GetEmailByDniAsync(dni);
        if (string.IsNullOrEmpty(correo))
            throw new Exception($"No se encontró correo para el DNI {dni}");

        // 2. Construir mensaje
        string subject = "Respuesta a tu sugerencia - Intranet QF";
        string body = $"Hola {nombre},\n\n" +
                      $"Se respondió tu sugerencia, favor de revisarlo en la Intranet QF.\n\n" +
                      $"Saludos cordiales,\nRecursos Humanos";

        // 3. Enviar email
        await _emailRepository.SendEmailSugAsync(correo, subject, body);
    }
    public async Task SendDocumentEmailAsync(EmailModel emailModel)
    {
        string subject = $" {emailModel.Beneficio}";
        /*string body = $"Se ha enviado un nuevo documento para el empleado con DNI: {emailModel.Dni}. Descripción: {emailModel.Descripcion}";*/
        string body = $"Estimado Colaborador:Mediante la presente, le informamos que se acaba de cargar su boleta de pago correspondiente al presente mes al INTRANET QF. Favor de visualizar y firmar la boleta de pago.Saludos Cordiales.";





        // Envía el correo con el documento adjunto
        /*  await _emailRepository.SendEmailAsync(emailModel.Email, subject, body, emailModel.Document);*/

        await _emailRepository.SendEmailAsync(emailModel.Email, subject, body);
    }
}