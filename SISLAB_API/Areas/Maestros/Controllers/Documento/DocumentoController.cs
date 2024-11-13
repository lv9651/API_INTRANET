﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SISLAB_API.Areas.Maestros.Models;
using SISLAB_API.Areas.Maestros.Services;
using System.Web;
using MySql.Data.MySqlClient;


namespace SISLAB_API.Areas.Maestros.Controllers // Cambia esto al espacio de nombres real
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentoController : ControllerBase
    {
        private readonly DocumentoService _documentService;

        public DocumentoController(DocumentoService documentService)
        {
            _documentService = documentService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadDocuments(
            [FromForm] string descripcion,
            [FromForm] string mes,
            [FromForm] string anio,
       
            ICollection<IFormFile> files)
        {
            if (files == null || files.Count == 0)
            {
                return BadRequest("No se han proporcionado archivos.");
            }

            // Crear un diccionario para almacenar los archivos y sus nombres
            var fileStreams = new Dictionary<string, Stream>();

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    // Guardar el archivo en un MemoryStream para procesarlo
                    var stream = new MemoryStream();
                    await file.CopyToAsync(stream);
                    stream.Position = 0; // Reiniciar el stream para lectura posterior
                    fileStreams.Add(file.FileName, stream);
                }
            }

            try
            {
                // Llamar al servicio para guardar los archivos y registrar en la base de datos
                await _documentService.SavePdfsAsync(fileStreams, descripcion, mes, anio);

                return Ok("Archivos cargados y registrados exitosamente.");
            }
            catch (Exception ex)
            {
                // Manejar errores y devolver una respuesta adecuada
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al procesar los archivos: {ex.Message}");
            }
            finally
            {
                // Limpiar recursos (cierre de streams)
                foreach (var stream in fileStreams.Values)
                {
                    stream.Dispose();
                }
            }
        }







        [HttpPost("upload-Benef")]
        public async Task<IActionResult> UploadBenefec(
           [FromForm] string dni,
           [FromForm] string descripcion,
           [FromForm] string beneficio,

           ICollection<IFormFile> files)
        {
            if (files == null || files.Count == 0)
            {
                return BadRequest("No se han proporcionado archivos.");
            }

            // Crear un diccionario para almacenar los archivos y sus nombres
            var fileStreams = new Dictionary<string, Stream>();

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    // Guardar el archivo en un MemoryStream para procesarlo
                    var stream = new MemoryStream();
                    await file.CopyToAsync(stream);
                    stream.Position = 0; // Reiniciar el stream para lectura posterior
                    fileStreams.Add(file.FileName, stream);
                }
            }

            try
            {
                // Llamar al servicio para guardar los archivos y registrar en la base de datos
                await _documentService.SaveBenPdfsAsync(fileStreams, dni, descripcion, beneficio);

                return Ok("Archivos cargados y registrados exitosamente.");
            }
            catch (Exception ex)
            {
                // Manejar errores y devolver una respuesta adecuada
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al procesar los archivos: {ex.Message}");
            }
            finally
            {
                // Limpiar recursos (cierre de streams)
                foreach (var stream in fileStreams.Values)
                {
                    stream.Dispose();
                }
            }
        }



        [HttpDelete("{ide}")]
        public async Task<IActionResult> DeleteBeneficio(int ide)
        {
            var result = await _documentService.DeleteNewsByIdAsync(ide);
            if (!result)
            {
                return NotFound(new { message = "Beneficio no encontrado." });
            }

            return NoContent(); // Devuelve 204 No Content si la eliminación fue exitosa
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<BeneficioEmp>>> GetBenefemple()
        {
            var noticias = await _documentService.GetBenefemple();
            return Ok(noticias);
        }


        [HttpGet("Benef")]
        public IActionResult GetBenefByPath([FromQuery] string path)
        {
            // Sanitiza el path si es necesario
            string sanitizedPath = HttpUtility.UrlDecode(path);

            string filePath = Path.Combine(@"\\PANDAFILE\Intranet\empleado", sanitizedPath);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Archivo no encontrado.");
            }

            try
            {
                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

                // Determinar el tipo de contenido
                string extension = Path.GetExtension(filePath).ToLowerInvariant();
                string contentType = extension switch
                {
                    ".pdf" => "application/pdf",
                    ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    ".jpg" => "image/jpeg",
                    ".png" => "image/png",
                    _ => "application/octet-stream",
                };

                // Asegúrate de que el nombre del archivo sea seguro
                string fileName = Path.GetFileName(filePath);
                Response.Headers.Add("Content-Disposition", $"inline; filename=\"{fileName}\"");

                return File(fileStream, contentType);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al abrir el archivo: {ex.Message}");
            }
        }


        [HttpPost("upload-firma")]
        public async Task<IActionResult> UploadDocuments(
             [FromForm] string dni,
             ICollection<IFormFile> files)
        {
            if (files == null || files.Count == 0)
            {
                return BadRequest("No se han proporcionado la firma.");
            }

            var fileStreams = new Dictionary<string, Stream>();

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var stream = new MemoryStream();
                    await file.CopyToAsync(stream);
                    stream.Position = 0; // Reiniciar el stream para lectura posterior
                    fileStreams.Add(file.FileName, stream);
                }
            }

            try
            {
                // Llamar al servicio para guardar los archivos y registrar en la base de datos
                await _documentService.SavefirmasAsync(fileStreams, dni);
                return Ok("Archivos cargados y registrados exitosamente.");
            }
            catch (MySqlException mySqlEx)
            {
                // Captura el mensaje específico del procedimiento almacenado
                var errorMessage = mySqlEx.Message;

                // Verificar si el mensaje indica que la firma ya existe
                if (errorMessage.Contains("La firma ya existe para este documento."))
                {
                    // Devuelve un conflicto con un mensaje claro
                    return Conflict(new { Message = "La firma ya está registrada para este documento." });
                }

                // En caso de otros errores de MySQL
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { Message = $"Error al procesar los archivos en la base de datos: {errorMessage}" });
            }
            catch (Exception ex)
            {
                // Manejar otros errores
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { Message = $"Error al procesar los archivos: {ex.Message}" });
            }
            finally
            {
                // Limpiar recursos (cierre de streams)
                foreach (var stream in fileStreams.Values)
                {
                    stream.Dispose();
                }
            }
        }





        [HttpGet("GetFIRM")]
        public IActionResult GetFirmByPath([FromQuery] string path)
        {
            // Sanitiza el path si es necesario
            string sanitizedPath = HttpUtility.UrlDecode(path);

            // Construir la ruta completa del archivo
            string filePath = Path.Combine(@"\\PANDAFILE\Intranet\firmas", sanitizedPath, sanitizedPath + "-firma.jpg");

            // Verificar si el archivo existe
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Archivo no encontrado.");
            }

            FileStream fileStream = null;

            try
            {
                // Abrir el archivo como FileStream
                fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

                // Determinar el tipo de contenido
                string extension = Path.GetExtension(filePath).ToLowerInvariant();
                string contentType = extension switch
                {
                    ".pdf" => "application/pdf",
                    ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    ".jpg" => "image/jpeg",
                    ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    _ => "application/octet-stream", // Tipo de contenido por defecto
                };

                string fileName = Path.GetFileName(filePath);
                Response.Headers.Add("Content-Disposition", $"inline; filename=\"{fileName}\"");

                return File(fileStream, contentType);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al abrir el archivo: {ex.Message}");
            }
        }



    }
}