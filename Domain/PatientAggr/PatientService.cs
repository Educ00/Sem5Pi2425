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

    public async Task<ActionResult<PatientDto>> AddPatientAsync(RegisterPatientDto registerPatientDto) {
        var temp = _userRepository.GetByEmailAsync(registerPatientDto.Email);
        if (temp.Result != null) {
            throw new BusinessRuleValidationException("Email already registed!");
        }

        temp = _userRepository.GetByUsername(registerPatientDto.Username);
        if (temp.Result != null) {
            throw new BusinessRuleValidationException("Username already taken!");
        }
        
        var user = new User(UserId.NewUserId(), new Username(registerPatientDto.Username), new Email(registerPatientDto.Email),
            new FullName(registerPatientDto.FullName), new PhoneNumber(registerPatientDto.PhoneNumber), Role.patient);
        
        var birthDate = DateOnly.Parse(registerPatientDto.BirthDate);
        var emergencyContact = new EmergencyContact(new PhoneNumber(registerPatientDto.EmergencyContactPhoneNumber), new FullName(registerPatientDto.EmergencyContactFullName), new Email(registerPatientDto.EmergencyContactEmail));
        List<MedicalCondition> medicalConditionsList = [
            new MedicalCondition(registerPatientDto.MedicalConditions)
        ];
        Enum.TryParse(registerPatientDto.Gender, true, out Gender gender);
        var patient = new Patient(user, emergencyContact, medicalConditionsList, birthDate, gender, null);
        
        await this._userRepository.AddAsync(user);
        await this._patientRepository.AddAsync(patient);
        await this._unitOfWork.CommitAsync();
            
        return new PatientDto(new UserDto(user), emergencyContact, medicalConditionsList, patient.BirthDate, patient.Gender, patient.AppointmentsHistory);        
          
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
        var birthDate = DateOnly.Parse(dto.BirthDate);
        var emergencyContact = new EmergencyContact(new PhoneNumber(dto.EmergencyContactPhoneNumber), new FullName(dto.EmergencyContactFullName), new Email(dto.EmergencyContactEmail));
        List<MedicalCondition> medicalConditionsList = [
            new MedicalCondition(dto.MedicalConditions)
        ];
        Enum.TryParse(dto.Gender, true, out Gender gender);
        var patient = new Patient(user, emergencyContact, medicalConditionsList, birthDate, gender, null);
        await _emailService.SendActivationEmailAsync(patient.User.Email.Value, patient.User.ActivationToken);
        await _userRepository.AddAsync(user);
        await _patientRepository.AddAsync(patient);
        await _unitOfWork.CommitAsync();
        return new PatientDto(new UserDto(patient.User), patient.EmergencyContact, patient.MedicalConditions,
            patient.BirthDate, patient.Gender, patient.AppointmentsHistory);
    }

    public async Task<bool> RequestAccountDeletion(string email) {
        var user = await _userRepository.GetByEmailAsync(email);

        if (user == null) {
            throw new BusinessRuleValidationException("This email does not exist in the database");
        }
        
        var patient = await _patientRepository.GetByIdAsync(user.Id);

        if (patient == null) {
            throw new BusinessRuleValidationException("Patient not found");
        }

        var deletionToken = Guid.NewGuid().ToString();
        patient.User.SetDeletionToken(deletionToken);

        await _emailService.SendAccountDeletionConfirmationEmailAsync(patient.User.Email.Value, deletionToken);

        await _unitOfWork.CommitAsync();
        return true;
    }

    public async Task<bool> ConfirmAccountDeletion(string deletionToken) {
        var user = await _userRepository.GetByDeletionToken(deletionToken);

        if (user == null) {
            throw new BusinessRuleValidationException("Invalid deletion token");
        }

        var patient = await _patientRepository.GetByIdAsync(user.Id);

        if (patient == null) {
            throw new BusinessRuleValidationException("Patient not found");
        }

        patient.MarkForDeletion();
        user.MarkForDeletion();

        await _unitOfWork.CommitAsync();
        
        await DeletePatientData(patient);
        return true;
    }

    private async Task DeletePatientData(Patient patient) {
        // TODO: escolher qual informação é apagada, neste momento vamos apagar toda!
        var email = patient.User.Email.Value;
        _userRepository.Remove(patient.User);
        _patientRepository.Remove(patient);
        
        await _emailService.SendAccountDeletionCompletedEmailAsync(email);
        
        await _unitOfWork.CommitAsync();
    }
}