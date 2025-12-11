using SISLAB_API.Areas.Maestros.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SISLAB_API.Areas.Maestros.Services
{
    public class DocumentoService
    {
        private readonly DocumentRepository _documentRepository;
        private readonly UserService _userRepository;
        private readonly EmailService _userEmailrepos;
        private readonly string _sharedPath;

        public DocumentoService(DocumentRepository documentRepository, UserService userRepository, EmailService userEmailrepos, string sharedPath)
        {
            _documentRepository = documentRepository;
            _userRepository = userRepository; // Inyectamos UserService
            _userEmailrepos = userEmailrepos; // Inyectamos EmailService
            _sharedPath = sharedPath;
        }

        public async Task SavePdfsAsync(Dictionary<string, Stream> files, string descripcion, string mes, string anio)
        {
            if (files == null || files.Count == 0)
            {
                throw new ArgumentException("El diccionario de archivos no puede ser nulo o vacío.");
            }

            foreach (var file in files)
            {
                string fileName = file.Key;
                Stream fileStream = file.Value;

                // Crear la ruta del archivo usando año y mes
                string directoryPath = Path.Combine(_sharedPath, anio, mes);
                string filePath = Path.Combine(directoryPath, fileName);

                try
                {
                    // Crear el directorio si no existe
                    Directory.CreateDirectory(directoryPath);

                    // Guardar el archivo en la ruta compartida
                    using (var fileStreamToSave = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        await fileStream.CopyToAsync(fileStreamToSave);
                    }

                    // Guardar el registro en la base de datos
                    await _documentRepository.InsertDocumentAsync(descripcion, mes, anio, fileName);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"No se pudo guardar el archivo PDF {fileName}.", ex);
                }
            }
        }


        public async Task SaveAndReplaceFileAsync(string dni, string beneficio, string rutaDoc, IFormFile newFile, int documentId)
        {
            // Primero, eliminar el archivo existente (como se hizo previamente)
            var filePath = Path.Combine(@"\\PANDAFILE\Intranet\empleado", dni, beneficio, rutaDoc);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            // Ahora guardar el nuevo archivo
            var newFilePath = Path.Combine(@"\\PANDAFILE\Intranet\empleado", dni, beneficio, newFile.FileName);
            var directory = Path.GetDirectoryName(newFilePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (var fileStream = new FileStream(newFilePath, FileMode.Create))
            {
                await newFile.CopyToAsync(fileStream);
            }

            // Actualizar la ruta del documento en la base de datos
            var updated = await _documentRepository.UpdateDocumentRouteAsync(documentId, newFile.FileName);
            if (!updated)
            {
                throw new Exception("No se pudo actualizar la base de datos con la nueva ruta del archivo.");
            }
        }
 



    public async Task SaveBenPdfsAsync(Dictionary<string, Stream> files, string dni, string descripcion, string beneficio)
        {
            if (files == null || files.Count == 0)
            {
                throw new ArgumentException("El diccionario de archivos no puede ser nulo o vacío.");
            }

            foreach (var file in files)
            {
                string fileName = file.Key;
                Stream fileStream = file.Value;

                string directoryPath = string.Empty;

                try
                {
                    if (beneficio == "BOLETA DE PAGO" || beneficio == "CTS" || beneficio == "GRATIFICACION")
                    {
                        // Si el beneficio es "BOLETA DE PAGO", extraemos el DNI y la descripción del nombre del archivo
                        string fileDni = ExtractDniFromFileName(fileName);
                        string fileDescripcion = ExtractDescripcionFromFileName(fileName);

                        directoryPath = Path.Combine(_sharedPath, "empleado", fileDni, beneficio);
                        descripcion = fileDescripcion; // Establecemos la descripción extraída

                        // Obtener el correo electrónico del usuario SOLO para "BOLETA DE PAGO"
                        string email = await _userRepository.GetEmailByDni(fileDni);

                        // Si no se encuentra el correo, registramos el caso y saltamos al siguiente archivo
                        if (string.IsNullOrEmpty(email))
                        {
                            Console.WriteLine($"No se encontró un correo para el DNI {fileDni}, saltando archivo.");
                            continue; // Salta al siguiente archivo
                        }

                        // Crear el modelo de correo electrónico
                        var emailModel = new EmailModel
                        {
                            Email = email,
                            Dni = fileDni,
                            Descripcion = fileDescripcion,
                            Beneficio = beneficio,
                            Document = ConvertStreamToIFormFile(fileName, fileStream)  // Asumimos que fileStream es el archivo a adjuntar
                        };

                        // Enviar el correo electrónico con el archivo adjunto
                        await _userEmailrepos.SendDocumentEmailAsync(emailModel);
                    }
                    else
                    {
                        // Si no es "BOLETA DE PAGO", usamos el DNI proporcionado
                        directoryPath = Path.Combine(_sharedPath, "empleado", dni, beneficio);
                    }

                    string filePath = Path.Combine(directoryPath, fileName);

                    // Asegurarse de que el stream está en la posición correcta antes de copiarlo
                    if (fileStream.CanSeek)
                    {
                        fileStream.Seek(0, SeekOrigin.Begin); // Coloca el stream en el inicio
                    }

                    // Crear el directorio si no existe
                    Directory.CreateDirectory(directoryPath);

                    // Guardar el archivo en la ruta especificada
                    using (var fileStreamToSave = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        // Copiar el contenido del archivo a la nueva ubicación
                        await fileStream.CopyToAsync(fileStreamToSave);
                    }

                    // Guardar el registro en la base de datos
                    await _documentRepository.InsertBenefAsync(dni, descripcion, beneficio, fileName);
                }
                catch (Exception ex)
                {
                    // Manejo de errores generalizado: si ocurre cualquier error, registramos el error y saltamos al siguiente archivo
                    Console.WriteLine($"Error al procesar el archivo {fileName}: {ex.Message}");
                    continue; // Saltamos al siguiente archivo sin interrumpir el flujo
                }
            }
        }
        // Método para obtener el correo por DNI
        public IFormFile ConvertStreamToIFormFile(string fileName, Stream stream)
        {
            var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            var fileBytes = memoryStream.ToArray();
            var fileContent = new FormFile(new MemoryStream(fileBytes), 0, fileBytes.Length, "file", fileName);

            return fileContent;
        }









        // Método para extraer el DNI del nombre del archivo
        private string ExtractDniFromFileName(string fileName)
        {
            // Suponiendo que el formato es BOL_DNI_Descripcion.pdf
            var parts = fileName.Split('_');
            return parts.Length > 1 ? parts[1] : string.Empty; // Devuelve el DNI (segunda parte)
        }

        // Método para extraer la descripción del nombre del archivo
        private string ExtractDescripcionFromFileName(string fileName)
        {
            // Suponiendo que el formato es BOL_DNI_Descripcion.pdf
            var parts = fileName.Split('_');
            return parts.Length > 2 ? parts[2].Replace(".pdf", "") : string.Empty; // Devuelve la descripción (tercera parte sin la extensión .pdf)
        }




        public async Task SavefirmasAsync(Dictionary<string, Stream> files, string dni)
        {
            if (files == null || files.Count == 0)
            {
                throw new ArgumentException("El diccionario de archivos no puede ser nulo o vacío.");
            }

            foreach (var file in files)
            {
                string fileName = file.Key;
                Stream fileStream = file.Value;

                // Crear la ruta del archivo usando año y mes
                string directoryPath = Path.Combine(_sharedPath,"firmas", dni);
                string filePath = Path.Combine(directoryPath, fileName);

                try
                {
                    // Crear el directorio si no existe
                    Directory.CreateDirectory(directoryPath);

                    // Guardar el archivo en la ruta compartida
                    using (var fileStreamToSave = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        await fileStream.CopyToAsync(fileStreamToSave);
                    }

                    // Guardar el registro en la base de datos
                    await _documentRepository.InsertFirmaAsync(dni, fileName);
                }
                catch (IOException ioEx)
                {
                    // Manejar errores de I/O específicos
                    throw new InvalidOperationException($"Error al intentar guardar el archivo {fileName}: {ioEx.Message}", ioEx);
                }
                catch (Exception ex)
                {
                    // Manejar otros errores
                    throw new InvalidOperationException($"No se pudo guardar el archivo {fileName}. Detalles: {ex.Message}", ex);
                }
            }
        }




        public async Task<bool> DeleteNewsByIdAsync(int ide)
        {
            return await _documentRepository.DeleteNewsByIdAsync(ide);
        }

        public async Task<bool> Vista_documentoByIdAsync(int ide)
        {
            return await _documentRepository.Vista_documentoByIdAsync(ide);
        }


        public async Task<IEnumerable<BeneficioEmp>> GetBenefemple()
        {
            return await _documentRepository.GetBenefemple();
        }






    }



   
}



