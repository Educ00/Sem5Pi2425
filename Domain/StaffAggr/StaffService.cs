using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Sem5Pi2425.Domain.Shared;
using Sem5Pi2425.Domain.SystemUserAggr;

namespace Sem5Pi2425.Domain.StaffAggr
{
    public class StaffService : StaffService.IStaffService
    {
        private readonly IStaffRepository _staffRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        

        public StaffService(IStaffRepository staffRepository, IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _staffRepository = staffRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ActionResult<StaffDTO>> CreateStaff(StaffCreateDTO dto)
        {
            // First check if user exists
            User user = await _userRepository.GetByEmailAsync(dto.Email);
    
            // If user doesn't exist, create one
            if (user == null)
            {
                user = new User(UserId.NewUserId(),
                    new Username(dto.Username),
                    new Email(dto.Email),
                    new FullName(dto.FullName),
                    new PhoneNumber(dto.PhoneNumber),
                    Role.doctor  // Or get this from the DTO/determine based on business rules
                );
                await _userRepository.AddAsync(user);
                await _unitOfWork.CommitAsync();
            }
    
            // Now create the staff member
            var uniqueIdentifier = new UniqueIdentifier(dto.uniqueIdentifier);
            List<AvailableSlots> availableSlotsList = [new AvailableSlots(dto.availableSlotsList)];
            var specialization = Enum.Parse<Specialization>(dto.specialization);
    
            var staff = new Staff(user, availableSlotsList, uniqueIdentifier, specialization);
    
            await _staffRepository.AddAsync(staff);
            await _unitOfWork.CommitAsync();
    
            return new StaffDTO(new UserDto(staff.User), staff.UniqueIdentifier, staff.AvailableSlots, staff.Specialization);
        }

        public async Task<ActionResult<StaffDTO>> EditStaff(UserDto user, StaffEditDto dto)
        {
       var staff = await _staffRepository.GetByIdAsync(new UserId(user.Id.ToString()));
        if (staff == null)
        {
            throw new BusinessRuleValidationException($"Staff with ID {user.Id} not found");
        }
        
        Email newEmail = null;
        FullName newFullName = null;
        PhoneNumber newPhoneNumber = null;
        Nullable< Specialization> newSpecialization=null;

        if (!string.IsNullOrEmpty(dto.Email))
        {
            try 
            {
                newEmail = new Email(dto.Email.Trim());
            }
            catch (ArgumentException)
            {
                throw new BusinessRuleValidationException($"Invalid email format: {dto.Email}");
            }
        }

        if (!string.IsNullOrEmpty(dto.FullName))
        {
            try 
            {
                newFullName = new FullName(dto.FullName.Trim());
            }
            catch (ArgumentException ex)
            {
                throw new BusinessRuleValidationException($"Invalid full name: {ex.Message}");
            }
        }

        if (!string.IsNullOrEmpty(dto.PhoneNumber))
        {
            try 
            {
                newPhoneNumber = new PhoneNumber(dto.PhoneNumber.Trim());
            }
            catch (ArgumentException ex)
            {
                throw new BusinessRuleValidationException($"Invalid phone number: {ex.Message}");
            }
        }

        if (!string.IsNullOrEmpty(dto.specialization))
        {
            try 
            {
                newSpecialization = Enum.Parse<Specialization>(dto.specialization);
            }
            catch (ArgumentException ex)
            {
                throw new BusinessRuleValidationException($"Invalid specialization: {ex.Message}");
            }
        }

        if (newEmail != null)
        {
            staff.User.UpdateEmail(newEmail);
        }

        if (newFullName != null)
        {
            staff.User.UpdateFullName(newFullName);
        }

        if (newPhoneNumber != null)
        {
            staff.User.UpdatePhoneNumber(newPhoneNumber);
        }

        if (newSpecialization != null)
        {
            staff.UpdateSpecialization((Specialization)newSpecialization);
        }
        



        await _unitOfWork.CommitAsync();

        return new StaffDTO(new UserDto(staff.User),  staff.UniqueIdentifier,staff.AvailableSlots, staff.Specialization);
        }

        public Task<ActionResult<StaffDTO>> InactivateStaff(UserDto id)
        {
            throw new NotImplementedException();
        }


        public interface IStaffService
        {
            Task<ActionResult<StaffDTO>> CreateStaff(StaffCreateDTO dto);
            Task<ActionResult<StaffDTO>> EditStaff(UserDto id,StaffEditDto dto);
            
            Task<ActionResult<StaffDTO>> InactivateStaff(UserDto id);
        }
    }
}
