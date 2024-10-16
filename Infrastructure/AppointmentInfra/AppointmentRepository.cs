using Microsoft.EntityFrameworkCore;
using Sem5Pi2425.Domain.AppointmentAggr;
using Sem5Pi2425.Infrastructure.Shared;

namespace Sem5Pi2425.Infrastructure.AppointmentInfra;

public class AppointmentRepository : BaseRepository<Appointment, AppointmentId>, IAppointmentRepository{
    public AppointmentRepository(Sem5Pi2425DbContext context) : base(context.Appointments) { }
}