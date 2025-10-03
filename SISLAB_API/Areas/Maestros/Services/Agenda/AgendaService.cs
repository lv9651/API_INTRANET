using SISLAB_API.Areas.Maestros.Models;

using System.Collections.Generic;

namespace SISLAB_API.Areas.Maestros.Services
{
    public class AgendaService
    {
        private readonly AgendaRepository _agendaRepository;

        public AgendaService(AgendaRepository AgendaRepository)
        {
            _agendaRepository = AgendaRepository;
        }

        public void AddAgendaItem(AgendaItem item)
        {
            _agendaRepository.AddAgendaItem(item);
        }

        public void DeleteAgendaItem(int id)
        {
            _agendaRepository.DeleteAgendaItem(id);
        }

        public List<AgendaItem> GetAgendaItems(DateTime date)
        {
            return _agendaRepository.GetAgendaItems(date);
        }

        public Task<int> CrearSugerenciaAsync(CrearSugerenciaRequest sugerencia)
        {
            // Aquí podrías validar datos antes de crear
            return _agendaRepository.CrearAsync(sugerencia);
        }
        public Task<IEnumerable<Sugerencia>> ObtenerPorEmpleadoAsync(string dni) => _agendaRepository.ObtenerPorEmpleadoAsync(dni);
        public Task<IEnumerable<Sugerencia>> ObtenerTodasSugerenciasAsync()
        {
            return _agendaRepository.ObtenerTodasAsync();
        }

        public Task<bool> RevisarSugerenciaAsync(int id, string revisadoPorDni, string respuestaRh = null)
        {
            return _agendaRepository.MarcarComoRevisadaAsync(id, revisadoPorDni, respuestaRh);
        }
    }
}