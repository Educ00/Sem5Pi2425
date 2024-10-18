using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Sem5Pi2425.Domain.EmailAggr;
using Sem5Pi2425.Domain.Shared;
using Sem5Pi2425.Domain.SystemUserAggr;

namespace Sem5Pi2425.Domain.PatientAggr;

public class PatientService {
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IEmailService _emailService;
    
    public PatientService(IUnitOfWork unitOfWork, IUserRepository userRepository, IPatientRepository patientRepository,IEmailService emailService) {
        this._unitOfWork = unitOfWork;
        this._userRepository = userRepository;
        this._patientRepository = patientRepository;
        this._emailService = emailService;
    }
    
    
    // TODO: remake this method to work with PatientDto
    public async Task<ActionResult<UserDto>> GetUserByIdAsync(UserId id) {
        var patient = await this._patientRepository.GetByIdAsync(id);

        return patient == null ? null : new UserDto(patient.User);
    }

    public async Task<PatientDto> SignIn(RegisterPatientDto dto) {
        var temp = _userRepository.GetByEmailAsync(dto.Email);
        if (temp.Result != null) {
            throw new BusinessRuleValidationException("Email already registed!");
        }

        temp = _userRepository.GetByUsername(dto.Username);
        if (temp.Result != null) {
            throw new BusinessRuleValidationException("Username already taken!");
        }

        temp = _userRepository.GetByPhoneNumber(dto.PhoneNumber);
        if (temp.Result != null) {
            throw new BusinessRuleValidationException("PhoneNumber already taken!");
        }
        var user = new User(UserId.NewUserId(), new Username(dto.Username), new Email(dto.Email),
            new FullName(dto.FullName), new PhoneNumber(dto.PhoneNumber), Role.patient);
        user.SetPassword(dto.Password);
        var birthDate = DateOnly.Parse(dto.BirthDate);
        var emergencyContact = new EmergencyContact(new PhoneNumber(dto.EmergencyContactPhoneNumber), new FullName(dto.EmergencyContactFullName), new Email(dto.EmergencyContactEmail));
        List<MedicalCondition> medicalConditionsList = [
            new MedicalCondition(dto.MedicalConditions)
        ];
        if (Enum.TryParse(dto.Gender, true, out Gender gender))
        {
        }
        else
        {
            Console.WriteLine("Invalid gender string.");
        }

        var patient = new Patient(user, emergencyContact, medicalConditionsList, birthDate, gender, null);
        await _emailService.SendConfirmationEmailAsync(patient.User.Email.Value);
        await _userRepository.AddAsync(user);
        await _patientRepository.AddAsync(patient);
        await _unitOfWork.CommitAsync();
        return new PatientDto(new UserDto(patient.User), patient.EmergencyContact, patient.MedicalConditions,
            patient.BirthDate, patient.Gender, patient.AppointmentsHistory);
    }
}