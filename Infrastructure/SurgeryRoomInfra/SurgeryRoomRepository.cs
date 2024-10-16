using Microsoft.EntityFrameworkCore;
using Sem5Pi2425.Domain.SurgeryRoomAggr;
using Sem5Pi2425.Infrastructure.Shared;

namespace Sem5Pi2425.Infrastructure.SurgeryRoomInfra;

public class SurgeryRoomRepository : BaseRepository<SurgeryRoom, RoomNumber>, ISurgeryRoomRepository{
    public SurgeryRoomRepository(Sem5Pi2425DbContext context) : base(context.SurgeryRooms) {
        
    }
}