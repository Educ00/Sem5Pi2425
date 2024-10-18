using System.Threading.Tasks;
using Sem5Pi2425.Domain.Shared;
using Sem5Pi2425.Domain.SystemUserAggr;

namespace Sem5Pi2425.Domain.PatientAggr;

public interface IPatientRepository : IRepository<Patient, UserId> { }
 
