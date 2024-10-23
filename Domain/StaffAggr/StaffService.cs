using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            
            User temp = await _userRepository.GetByEmailAsync(dto.Email);
            // Create Staff object
            var uniqueIdentifier = new UniqueIdentifier(dto.uniqueIdentifier);
            List<AvailableSlots> availableSlotsList = [new AvailableSlots(dto.availableSlotsList)];
            var specialization = Enum.Parse<Specialization>(dto.specialization);
            var staff = new Staff(temp, availableSlotsList, uniqueIdentifier, specialization); 


            // Add staff to the repository
            await _staffRepository.AddAsync(staff);
            await _unitOfWork.CommitAsync();

            // Return DTO for the created staff
            return new StaffDTO(new UserDto(staff.User), staff.UniqueIdentifier, staff.AvailableSlots, staff.Specialization);
        }

        public async Task<ActionResult<StaffDTO>> EditStaff(string id, StaffEditDto dto)
        {
       var staff = await _staffRepository.GetByIdAsync(new UserId(id));
        if (staff == null)
        {
            throw new BusinessRuleValidationException($"Staff with ID {id} not found");
        }

        if (staff.User == null)
        {
            throw new BusinessRuleValidationException($"User data for staff with ID {id} is missing");
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



        public interface IStaffService
        {
            Task<ActionResult<StaffDTO>> CreateStaff(StaffCreateDTO dto);
            Task<ActionResult<StaffDTO>> EditStaff(string id,StaffEditDto dto);
        }
    }
}
