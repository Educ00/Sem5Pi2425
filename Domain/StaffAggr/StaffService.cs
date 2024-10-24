using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sem5Pi2425.Domain.LogAggr;
using Sem5Pi2425.Domain.PatientAggr;
using Sem5Pi2425.Domain.Shared;
using Sem5Pi2425.Domain.SystemUserAggr;
using Type = System.Type;

namespace Sem5Pi2425.Domain.StaffAggr
{
    public class StaffService : StaffService.IStaffService
    {
        private readonly IStaffRepository _staffRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogRepository _logger;
        

        public StaffService(IStaffRepository staffRepository, IUserRepository userRepository, IUnitOfWork unitOfWork, ILogRepository logger)
        {
            _staffRepository = staffRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
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
            Boolean changedcontact=false;
       var staff = await _staffRepository.GetByIdAsync(new UserId(user.Id.Value));
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
            changedcontact=true;
            staff.User.UpdateEmail(newEmail);
        }

        if (newFullName != null)
        {
            staff.User.UpdateFullName(newFullName);
        }

        if (newPhoneNumber != null)
        {
            changedcontact=true;
            staff.User.UpdatePhoneNumber(newPhoneNumber);
        }

        if (newSpecialization != null)
        {
            staff.UpdateSpecialization((Specialization)newSpecialization);
        }
        
        if (!string.IsNullOrEmpty(dto.availableSlotsList))
        {
            var availableSlots = dto.availableSlotsList
                .Split(',')
                .Select(slot => new AvailableSlots(slot.Trim()))
                .ToList();
            staff.UpdateAvailableSlots(availableSlots);
        }
        
        if (!string.IsNullOrEmpty(dto.specialization))
        {
            if (Enum.TryParse<Specialization>(dto.specialization, true, out var specialization))
            {
                staff.UpdateSpecialization(specialization);
            }
            else
            {
                throw new BusinessRuleValidationException("Invalid gender. Use 'Male' or 'Female'");
            }
        }

        if (changedcontact == true)
        {
            
        }
       


        await _unitOfWork.CommitAsync();

        return new StaffDTO(new UserDto(staff.User),  staff.UniqueIdentifier,staff.AvailableSlots, staff.Specialization);
        }

        public async Task<ActionResult<StaffDTO>> InactivateStaff(UserDto user)
        {
            var staff = await _staffRepository.GetByIdAsync(new UserId(user.Id.ToString()));
            if (staff == null)
            {
                throw new BusinessRuleValidationException($"Staff with ID {user.Id} not found");
            }

            
            await _unitOfWork.CommitAsync();

            return new StaffDTO(new UserDto(staff.User), staff.UniqueIdentifier, staff.AvailableSlots, staff.Specialization);
        }
        
        public async Task<List<StaffDTO>> ListStaff()
        {
            List<StaffDTO> list=new List<StaffDTO>();
            var staff = await _staffRepository.GetAllAsync();
            if (staff == null)
            {
                throw new BusinessRuleValidationException($"Staff not found");
            }

            foreach (var s in staff)
            {
                list.Add(new StaffDTO(new UserDto(s.User), s.UniqueIdentifier, s.AvailableSlots, s.Specialization));
            }
            return list;
        }
        
        public async Task<StaffEditDto> GetStaffByEmail(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                throw new BusinessRuleValidationException($"User with email {email} not found");
            }

            var staff = await _staffRepository.GetByIdAsync(user.Id);
            if (staff == null)
            {
                throw new BusinessRuleValidationException($"Staff with email {email} not found");
            }

            return new StaffEditDto(
                user.Username.ToString(),
                user.Email.ToString(),
                user.FullName.ToString(),
                user.PhoneNumber.ToString(),
                staff.UniqueIdentifier.ToString(),
                string.Join(",", staff.AvailableSlots.Select(slot => slot.ToString())),
                staff.Specialization.ToString()
            );
        }

        public interface IStaffService
        {
            Task<ActionResult<StaffDTO>> CreateStaff(StaffCreateDTO dto);
            Task<ActionResult<StaffDTO>> EditStaff(UserDto id,StaffEditDto dto);
            
            Task<ActionResult<StaffDTO>> InactivateStaff(UserDto id);
            
            Task<List<StaffDTO>> ListStaff();
            
            Task<StaffEditDto> GetStaffByEmail(string email);
        }
    }
}
