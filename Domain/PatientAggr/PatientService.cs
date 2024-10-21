using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sem5Pi2425.Domain.EmailAggr;
using Sem5Pi2425.Domain.LogAggr;
using Sem5Pi2425.Domain.Shared;
using Sem5Pi2425.Domain.SystemUserAggr;
using Type = Sem5Pi2425.Domain.LogAggr.Type;

namespace Sem5Pi2425.Domain.PatientAggr;

public class PatientService {
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IEmailService _emailService;
    private readonly LogService _logService;

    public PatientService(IUnitOfWork unitOfWork, IUserRepository userRepository, IPatientRepository patientRepository,
        IEmailService emailService, LogService logService) {
        this._unitOfWork = unitOfWork;
        this._userRepository = userRepository;
        this._patientRepository = patientRepository;
        this._emailService = emailService;
        this._logService = logService;
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

        var user = new User(UserId.NewUserId(), new Username(registerPatientDto.Username),
            new Email(registerPatientDto.Email),
            new FullName(registerPatientDto.FullName), new PhoneNumber(registerPatientDto.PhoneNumber), Role.patient);

        var birthDate = DateOnly.Parse(registerPatientDto.BirthDate);
        var emergencyContact = new EmergencyContact(new PhoneNumber(registerPatientDto.EmergencyContactPhoneNumber),
            new FullName(registerPatientDto.EmergencyContactFullName),
            new Email(registerPatientDto.EmergencyContactEmail));
        List<MedicalCondition> medicalConditionsList = [
            new MedicalCondition(registerPatientDto.MedicalConditions)
        ];
        Enum.TryParse(registerPatientDto.Gender, true, out Gender gender);
        var patient = new Patient(user, emergencyContact, medicalConditionsList, birthDate, gender, null);

        await this._userRepository.AddAsync(user);
        await this._patientRepository.AddAsync(patient);
        await this._unitOfWork.CommitAsync();

        return new PatientDto(new UserDto(user), emergencyContact, medicalConditionsList, patient.BirthDate,
            patient.Gender, patient.AppointmentsHistory);
    }

    public async Task<PatientDto> EditPatientAsync(string id, EditPatientDto updateDto) {
        var patient = await _patientRepository.GetByIdAsync(new UserId(id));

        patient.User.UpdateFullName(new FullName(updateDto.FullName));
        patient.User.UpdatePhoneNumber(new PhoneNumber(updateDto.PhoneNumber));
        patient.User.UpdateEmail(new Email(updateDto.Email));
        patient.UpdateEmergencyContact(new EmergencyContact(
            new PhoneNumber(updateDto.EmergencyContactPhoneNumber),
            new FullName(updateDto.EmergencyContactFullName),
            new Email(updateDto.EmergencyContactPhoneNumber)
        ));

        patient.UpdateMedicalConditions(new List<MedicalCondition>());

        await _unitOfWork.CommitAsync();
        return new PatientDto(new UserDto(patient.User), patient.EmergencyContact, patient.MedicalConditions,
            patient.BirthDate, patient.Gender, patient.AppointmentsHistory);
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
        var emergencyContact = new EmergencyContact(new PhoneNumber(dto.EmergencyContactPhoneNumber),
            new FullName(dto.EmergencyContactFullName), new Email(dto.EmergencyContactEmail));
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

        var patient = await _patientRepository.GetByIdAsync(new UserId(user.Id.Value));

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

        var patient = await _patientRepository.GetByIdAsync(new UserId(user.Id.Value));

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


    public async Task<PatientDto> EditPatientProfileAsync(string id, EditPatientDto dto) {
        var sendConfirmationEmail = false;
        var patient = await _patientRepository.GetByIdAsync(new UserId(id));

        if (patient == null) {
            throw new BusinessRuleValidationException("Patient not found");
        }

        if (patient.User == null) {
            throw new BusinessRuleValidationException("Patient does not have a user associated");
        }

        ArgumentNullException.ThrowIfNull(dto);

        var changes = "";

        Email email = patient.User.Email;
        if (dto.Email != null) {
            sendConfirmationEmail = true;
            email = new Email(dto.Email);
            changes += "\n" + $"change: email: {patient.User.Email.Value} -> {email.Value}";
        }

        FullName fullName = patient.User.FullName;
        if (dto.FullName != null) {
            sendConfirmationEmail = true;
            fullName = new FullName(dto.FullName);
            changes += "\n" + $"change: fullName: {patient.User.FullName.Value} -> {fullName.Value}";
        }

        PhoneNumber phoneNumber = patient.User.PhoneNumber;
        if (dto.PhoneNumber != null) {
            sendConfirmationEmail = true;
            phoneNumber = new PhoneNumber(dto.PhoneNumber);
            changes += "\n" + $"change: phoneNumber: {patient.User.PhoneNumber.Value} -> {phoneNumber.Value}";
        }

        DateOnly birthDate = patient.BirthDate;
        if (DateOnly.TryParse(dto.BirthDate, out var newBirthDate)) {
            sendConfirmationEmail = true;
            birthDate = newBirthDate;
            changes += "\n" + $"change: birthDate: {patient.BirthDate} -> {birthDate}";
        }

        Gender gender = patient.Gender;
        if (Enum.TryParse<Gender>(dto.Gender, out var newGender)) {
            sendConfirmationEmail = true;
            gender = newGender;
            changes += "\n" + $"\nchange: gender: {patient.Gender} -> {gender}";
        }

        PhoneNumber emergencyContactPhoneNumber = patient.EmergencyContact.PhoneNumber;
        if (dto.EmergencyContactPhoneNumber != null) {
            sendConfirmationEmail = true;
            emergencyContactPhoneNumber = new PhoneNumber(dto.EmergencyContactPhoneNumber);
            changes +=
                "\n" + $"change: emergencyContactPhoneNumber: {patient.EmergencyContact.PhoneNumber.Value} -> {emergencyContactPhoneNumber.Value}";
        }

        FullName emergencyContactFullName = patient.EmergencyContact.FullName;
        if (dto.EmergencyContactFullName != null) {
            sendConfirmationEmail = true;
            emergencyContactFullName = new FullName(dto.EmergencyContactFullName);
            changes +=
                "\n" + $"change: emergencyContactFullName: {patient.EmergencyContact.FullName.Value} -> {emergencyContactFullName.Value}";
        }

        Email emergencyContactEmail = patient.EmergencyContact.Email;
        if (dto.EmergencyContactEmail != null) {
            sendConfirmationEmail = true;
            emergencyContactEmail = new Email(dto.EmergencyContactEmail);
            changes +=
                "\n" + $"change: emergencyContactEmail: {patient.EmergencyContact.Email.Value} -> {emergencyContactEmail.Value}";
        }

        var emergencyContact =
            new EmergencyContact(emergencyContactPhoneNumber, emergencyContactFullName, emergencyContactEmail);

        List<MedicalCondition> medicalConditions = patient.MedicalConditions;
        if (dto.MedicalConditions != null) {
            sendConfirmationEmail = true;
            medicalConditions = new List<MedicalCondition> { new MedicalCondition(dto.MedicalConditions) };
            changes +=
                "\n" + $"change: medicalConditions: {string.Join(", ", patient.MedicalConditions.Select(mc => mc.Value))} -> {string.Join(", ", medicalConditions.Select(mc => mc.Value))}";
        }

        patient.User.UpdateEmail(email);
        patient.User.UpdateFullName(fullName);
        patient.User.UpdatePhoneNumber(phoneNumber);
        patient.UpdateBirthDate(birthDate);
        patient.UpdateGender(gender);
        patient.UpdateEmergencyContact(emergencyContact);
        patient.UpdateMedicalConditions(medicalConditions);


        if (string.IsNullOrEmpty(changes)) {
            throw new BusinessRuleValidationException("No changes were made to the patient profile.");
        }

        if (sendConfirmationEmail) {
            await _emailService.SendProfileChangedConfirmationEmailAsync(patient.User.Email.Value);
        }

        _logService.AddLogAsync(new LogDto(null, Type.ProfileChange.ToString(), changes, patient.Id.Value, null));

        await _unitOfWork.CommitAsync();
        return new PatientDto(new UserDto(patient.User), emergencyContact, medicalConditions, birthDate, gender,
            patient.AppointmentsHistory);
    }
}