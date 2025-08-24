
using SISLAB_API.Areas.Maestros.Models;

using System.Collections.Generic;
using System.Threading.Tasks;
using SISLAB_API.Areas.Maestros.Models;


namespace SISLAB_API.Areas.Maestros.Services
{
    public class InductionService

    {
        private readonly InductionRepository _InductionRepository;
        private readonly VideoService _videoservice;

        public InductionService(InductionRepository InductionRepository)
        {
            _InductionRepository = InductionRepository;
        }

        public async Task<IEnumerable<Induction>> GetAllInductionAsync()
        {
            return await _InductionRepository.GetAllInductionAsync();
        }

        public async Task SaveInductionAsync(Induction Induction)
        {
            await _InductionRepository.SaveInductionAsync(Induction);
        }

        public async Task<string> GetVideoPathByIdAsync(string id)
        {
            return await _videoservice.GetVideoPathByIdAsync(id);
        }


        public async Task<bool> DeleteNewsByIdAsync(int id)
        {
            return await _InductionRepository.DeleteNewsByIdAsync(id);
        }

        public async Task<InductAct> ActualizarInduccionAsync(InductAct noticia)
        {
            // Llamamos directamente al repositorio para realizar la actualización
            var updated = await _InductionRepository.UpdateInductionAsync(noticia);

            return updated ? noticia : null; // Si la actualización tuvo éxito, retornamos la noticia
        }




        public async Task AddCommentAsync(int videoId, CommentR comment)
        {
            await _InductionRepository.AddCommentAsync(videoId, comment);
        }


        public async Task AddCommentAsyncN(int videoId, CommentN comment)
        {
            await _InductionRepository.AddCommentAsyncN(videoId, comment);
        }



        public async Task<IEnumerable<Comment>> GetCommentsForVideoAsync(int videoId)
        {
            return await _InductionRepository.GetCommentsByVideoIdAsync(videoId);
        }


        public async Task<IEnumerable<Comment>> GetCommentsForVideoAsyncN(int videoId)
        {
            return await _InductionRepository.GetCommentsByVideoIdAsyncN(videoId);
        }




        public async Task<IEnumerable<VideoProgress>> GetVideoProgressAsync(string userId, string module)
        {
            return await _InductionRepository.GetVideoProgressAsync(userId, module);
        }


        public async Task AddVideoProgressAsync(string userId, VideoProgressRequest request)
        {
            // Get the video title
            var videoTitle = await _InductionRepository.GetVideoTitleByIdAsync(request.VideoId);

            if (string.IsNullOrEmpty(videoTitle))
            {
                throw new Exception("Video no encontrado");
            }

            // Add the video progress
            await _InductionRepository.AddVideoProgressAsync(userId, request.VideoId);

            // Add a notification
           
        }






    }

}


