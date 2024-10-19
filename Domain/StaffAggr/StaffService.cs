using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sem5Pi2425.Domain.Shared;
using Sem5Pi2425.Domain.SystemUserAggr;

namespace Sem5Pi2425.Domain.StaffAggr
{
    public class StaffService : StaffService.IStaffService
    {
        private readonly IStaffRepository _staffRepository;
        private readonly UserService _userService;
        private readonly IUnitOfWork _unitOfWork; // Assuming you'll need this

        public StaffService(IStaffRepository staffRepository, UserService userService, IUnitOfWork unitOfWork)
        {
            _staffRepository = staffRepository;
            _userService = userService; // Initialize userService here
            _unitOfWork = unitOfWork; // Initialize unitOfWork here
        }

        public async Task<ActionResult<StaffDTO>> CreateStaff(StaffDTO dto, UserDto user)
        {

            var userconverted = new User(
                user.Id,
                new Username(user.Username),
                new Email(user.Email),
                new FullName(user.FullName),
                new PhoneNumber(user.PhoneNumber),
                Enum.Parse<Role>(user.Role)
            );
            
            // Create Staff object
            var staff = new Staff(
                userconverted,
                dto.AvailableSlotsList,
                dto.UniqueIdentifier,
                dto.Specialization
            );

            // Add staff to the repository
            await _staffRepository.AddAsync(staff);
            await _unitOfWork.CommitAsync();

            // Return DTO for the created staff
            return new StaffDTO(staff);
        }


        public async Task<ActionResult<StaffDTO>> EditStaff(StaffDTO dto, UserDto user, Specialization specialization)
        {
            // Retrieve the current user
            var userconverted = new User(
                user.Id,
                new Username(user.Username),
                new Email(user.Email),
                new FullName(user.FullName),
                new PhoneNumber(user.PhoneNumber),
                Enum.Parse<Role>(user.Role)
            );
            

            // Create Staff object
            var staff = new Staff(
                userconverted,
                dto.AvailableSlotsList,
                dto.UniqueIdentifier,
                dto.Specialization
            );

            // Edit staff in the repository
             _staffRepository.Edit(staff, specialization);
            await _unitOfWork.CommitAsync();

            // Return DTO for the edited staff
            return new StaffDTO(staff);
        }

        public interface IStaffService
        {
            Task<ActionResult<StaffDTO>> CreateStaff(StaffDTO dto, UserDto user);
            Task<ActionResult<StaffDTO>> EditStaff(StaffDTO dto, UserDto user, Specialization specialization);
        }
    }
}
