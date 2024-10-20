using Sem5Pi2425.Domain.Shared;

namespace Sem5Pi2425.Domain.AppointmentAggr
{

    public class AppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppointmentRepository _repo;

        public AppointmentService(IUnitOfWork unitOfWork, IAppointmentRepository repo)
        {
            _unitOfWork = unitOfWork;
            _repo = repo;
        }

    }
}