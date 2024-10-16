using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sem5Pi2425.Domain.PatientAggr;
using Sem5Pi2425.Domain.SystemUserAggr;
using Sem5Pi2425.Infrastructure.Shared;

namespace Sem5Pi2425.Infrastructure.PatientInfra;

public class PatientRepository : BaseRepository<Patient, UserId>, IPatientRepository {
    public PatientRepository(Sem5Pi2425DbContext context) : base(context.Patients) { }
}