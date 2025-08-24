using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SISLAB_API.Areas.Maestros.Models;
using SISLAB_API.Areas.Maestros.Services;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SISLAB_API.Areas.Maestros.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InductionController : ControllerBase
    {
        private readonly InductionService _inductionservice;


        public InductionController(InductionService InductionService)
        {
            _inductionservice = InductionService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Induction>>> GetAllInductionAsync()
        {
            var noticias = await _inductionservice.GetAllInductionAsync();
            return Ok(noticias);
        }

        [HttpPost("upload-videos")]
        [RequestSizeLimit(3221225472)] // 3 GB
        public async Task<IActionResult> UploadVideos(
      [FromForm] int id,
      [FromForm] string title,
      [FromForm] string content,
      [FromForm] string module,
      [FromForm] ICollection<IFormFile> videos)
        {
            if (videos == null || videos.Count == 0)
                return BadRequest("Se deben proporcionar videos.");

            string carpetaDestino = @"\\PANDAFILE\Intranet\Videos";

            // Verifica que exista la carpeta
            if (!Directory.Exists(carpetaDestino))
                return StatusCode(500, "Ruta de destino no existe o no está disponible.");

            try
            {
                foreach (var video in videos)
                {
                    if (video.Length == 0)
                        continue;

                    // Generar nombre único
                    string uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(video.FileName)}";
                    string rutaArchivo = Path.Combine(carpetaDestino, uniqueFileName);

                    // Guardar directamente en disco sin usar MemoryStream
                    using (var fileStream = new FileStream(rutaArchivo, FileMode.Create, FileAccess.Write))
                    {
                        await video.CopyToAsync(fileStream);  // // 80 KB buffer
                    }

                    // Guardar info en base de datos
                    await _inductionservice.SaveInductionAsync(new Induction
                    {
                        id = id,
                        title = title,
                        content = content,
                        module = module,
                        video_url = uniqueFileName
                    });
                }

                return Ok("Videos cargados exitosamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR UploadVideos]: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return StatusCode(500, $"Error al procesar los videos: {ex.Message}");
            }
        }




        [HttpGet("video/{id}")]
        public IActionResult GetVideoById(string id)
        {
            string videoFileName = id; // ya tiene el nombre completo con extensión
            string videoPath = Path.Combine(@"\\PANDAFILE\Intranet\Videos", videoFileName);

            if (!System.IO.File.Exists(videoPath))
            {
                return NotFound("Video no encontrado.");
            }

            var fileStream = new FileStream(videoPath, FileMode.Open, FileAccess.Read, FileShare.Read);

            return File(fileStream, "video/mp4", enableRangeProcessing: true); // 👈 HABILITA STREAMING
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNewsByIdAsync(int id)
        {
            try
            {
                var deleted = await _inductionservice.DeleteNewsByIdAsync(id);
                if (!deleted)
                {
                    return NotFound("Video no encontrada.");
                }
                return NoContent(); // Devuelve un 204 No Content
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al eliminar la noticia: {ex.Message}");
            }
        }




        [HttpPost("{id}/comments")]
        public async Task<IActionResult> AddComment(int id, [FromBody] CommentR comment)
        {
            if (comment == null || string.IsNullOrEmpty(comment.comment))
            {
                return BadRequest("Invalid comment data.");
            }

            try
            {
                await _inductionservice.AddCommentAsync(id, comment);
                return StatusCode(201, new { message = "Comment added successfully" });
            }
            catch (Exception ex)
            {
                // Manejo de errores
                return StatusCode(500, new { message = $"Error adding comment: {ex.Message}" });
            }
        }

        [HttpPost("{id}/commentsN")]
        public async Task<IActionResult> AddCommentN(int id, [FromBody] CommentN comment)
        {
            if (comment == null || string.IsNullOrEmpty(comment.comment))
            {
                return BadRequest("Invalid comment data.");
            }

            try
            {
                await _inductionservice.AddCommentAsyncN(id, comment);
                return StatusCode(201, new { message = "Comment added successfully" });
            }
            catch (Exception ex)
            {
                // Manejo de errores
                return StatusCode(500, new { message = $"Error adding comment: {ex.Message}" });
            }
        }


        [HttpGet("{id}/comments")]
        public async Task<IActionResult> GetComments(int id)
        {
            try
            {
                var comments = await _inductionservice.GetCommentsForVideoAsync(id);
                return Ok(comments);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error fetching comments: {ex.Message}");
                return StatusCode(500, "Error fetching comments");
            }
        }


        [HttpGet("{id}/commentsN")]
        public async Task<IActionResult> GetCommentsN(int id)
        {
            try
            {
                var comments = await _inductionservice.GetCommentsForVideoAsyncN(id);
                return Ok(comments);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error fetching comments: {ex.Message}");
                return StatusCode(500, "Error fetching comments");
            }
        }


        [HttpGet("{userId}/video-progress")]
        public async Task<ActionResult<IEnumerable<VideoProgress>>> GetVideoProgress(string userId, [FromQuery] string module)
        {
            try
            {
                var results = await _inductionservice.GetVideoProgressAsync(userId, module);
                return Ok(results);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error al obtener el progreso de los videos: {ex.Message}");
                return StatusCode(500, new { error = "Error al obtener el progreso de los videos" });
            }
        }



        [HttpPost("{id}/video-progress")]
        public async Task<IActionResult> AddVideoProgress(string userId, [FromBody] VideoProgressRequest request)
        {
            try
            {
                await _inductionservice.AddVideoProgressAsync(userId, request);
                return Ok(new { message = "Progreso del video y notificación añadidos exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error: {ex.Message}" });
            }
        }





        /*
          [HttpPost("{id}/video-progress")]
            public async Task<IActionResult> AddVideoProgress(string userId, [FromBody] VideoProgressRequest request)
            {
                string videoTitle = string.Empty;

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // Primero, obtén el título del video
                    using (var command = new SqlCommand("SELECT title FROM induction_videos WHERE id = @videoId", connection))
                    {
                        command.Parameters.AddWithValue("@videoId", request.VideoId);
                        var result = await command.ExecuteScalarAsync();

                        if (result == null)
                        {
                            return NotFound(new { error = "Video no encontrado" });
                        }

                        videoTitle = result.ToString();
                    }

                    // Agrega el progreso del video
                    using (var insertProgressCommand = new SqlCommand("INSERT INTO user_video_progress (user_id, video_id, watched) VALUES (@userId, @videoId, @watched)", connection))
                    {
                        insertProgressCommand.Parameters.AddWithValue("@userId", userId);
                        insertProgressCommand.Parameters.AddWithValue("@videoId", request.VideoId);
                        insertProgressCommand.Parameters.AddWithValue("@watched", true);

                        await insertProgressCommand.ExecuteNonQueryAsync();
                    }

                    // Agrega la notificación
                    var notificationMessage = $"Has visto el video: {videoTitle}";
                    using (var insertNotificationCommand = new SqlCommand("INSERT INTO notifications (user_id, message) VALUES (@userId, @message)", connection))
                    {
                        insertNotificationCommand.Parameters.AddWithValue("@userId", userId);
                        insertNotificationCommand.Parameters.AddWithValue("@message", notificationMessage);

                        await insertNotificationCommand.ExecuteNonQueryAsync();
                    }
                }

                return Ok(new { message = "Progreso del video y notificación añadidos exitosamente" });
            }
        }


            */

        [HttpPut("act")]
        public async Task<IActionResult> UpdateInduccion([FromBody] InductAct noticia)
        {


            var updatedNoticia = await _inductionservice.ActualizarInduccionAsync(noticia);


            return Ok(new { message = "Inducion actualizada correctamente", data = updatedNoticia });
        }



    }



}




























