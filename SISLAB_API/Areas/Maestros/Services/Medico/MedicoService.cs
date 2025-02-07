using SISLAB_API.Areas.Maestros.Models;

using System.Collections.Generic;

namespace SISLAB_API.Areas.Maestros.Services
{
    public class MedicoService
    {
        private readonly MedicoRepository _medicoRepository;

        public MedicoService(MedicoRepository AgendaRepository)
        {
            _medicoRepository = AgendaRepository;
        }

   

        public async Task<IEnumerable<Medico>> ObtenerEspecialidadesAsync()
        {
            return await _medicoRepository.ObtenerEspecialidadesAsync();
        }

        public async Task<IEnumerable<Sucursal>> ObtenerSucursalesAsync()
        {
            return await _medicoRepository.ObtenerSucursalesAsync();
        }

        public async Task<IEnumerable<Med_bus>> BuscarMedicoPorEspecialidadAsync(string descripcion)
        {
            return await _medicoRepository.BuscarMedicoPorEspecialidadAsync(descripcion);
        }

        public async Task<IEnumerable<Dependiente>> BuscarDependiente(string dni)
        {
            return await _medicoRepository.BuscarDependienteAsync(dni);
        }

        public async Task<IEnumerable<Paciente>> BuscarIdpaciente(string dni)
        {
            return await _medicoRepository.BuscaridPacienteAsync(dni);
        }


        public async Task<IEnumerable<Bus_cita_cliente>> Busq_cita_clienteAsync(string documento)
        {
            return await _medicoRepository.Busq_cita_clienteAsync(documento);
        }


        public async Task<IEnumerable<PacienteDependiente>> Busq_dependientePaciente(string documento)
        {
            return await _medicoRepository.Busq_PacienteDependienteAsync(documento);
        }
        public async Task<IEnumerable<Busq_dia_med>> BuscardiaMedicoAsync(string colegio)
        {
            return await _medicoRepository.BuscardiaMedicoAsync(colegio);
        }


        public async Task<IEnumerable<Historial_clinico>> Busq_historial_clinico(string documento)
        {
            return await _medicoRepository.Busq_HistorialClinicoAsync(documento);
        }
        public async Task<IEnumerable<Hor_dia_med>> BuscardiaHoraMedicoAsync(string fecha, string colegio, string idmodalidad)
        {
            return await _medicoRepository.BuscarDiaHoraMedicoAsync(fecha, colegio,idmodalidad);
        }

        public async Task<int> InsertarPacienteFamiliarAsync(Reg_pariente request)
        {
            try
            {
                // Llamamos al repositorio para insertar los datos y obtener el cli_codigo
                int cliCodigo = await _medicoRepository.InsertarFammedAsync(request);

                // Retornamos el cli_codigo insertado
                return cliCodigo;
            }
            catch (Exception ex)
            {
                // Manejo de excepciones (si algo sale mal)
                throw new Exception("Error al insertar el paciente familiar: " + ex.Message);
            }
        }
    

    public async Task<IEnumerable<Tipo_pago>> ObtenerTipopago()
        {
            return await _medicoRepository.ObtenerTipoPago();
        }

    }
}