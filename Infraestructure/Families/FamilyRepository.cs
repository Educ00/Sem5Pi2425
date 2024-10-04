using Sem5Pi2425.Domain.Families;
using Sem5Pi2425.Infrastructure.Shared;

namespace Sem5Pi2425.Infrastructure.Families
{
    public class FamilyRepository : BaseRepository<Family, FamilyId>, IFamilyRepository
    {
      
        public FamilyRepository(Sem5Pi2425DbContext context):base(context.Families)
        {
            
        }

    }
}