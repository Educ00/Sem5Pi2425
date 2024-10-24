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

    public async Task<ActionResult<UserDto>> GetUserByIdAsyncPdto(PatientDto dto) {
        var patient = await this._patientRepository.GetByIdAsync(dto.User.Id);

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

    public async Task<PatientDto> EditPatientAsync(string id, EditPatientDto updateDto)
{
    try 
    {
        var patient = await _patientRepository.GetByIdAsync(new UserId(id));
        if (patient == null)
        {
            throw new BusinessRuleValidationException($"Patient with ID {id} not found");
        }

        if (patient.User == null)
        {
            throw new BusinessRuleValidationException($"User data for patient with ID {id} is missing");
        }

        Email newEmail = null;
        Email newEmergencyEmail = null;
        FullName newFullName = null;
        PhoneNumber newPhoneNumber = null;
        EmergencyContact newEmergencyContact = null;

        if (!string.IsNullOrEmpty(updateDto.Email))
        {
            try 
            {
                newEmail = new Email(updateDto.Email.Trim());
            }
            catch (ArgumentException)
            {
                throw new BusinessRuleValidationException($"Invalid email format: {updateDto.Email}");
            }
        }

        if (!string.IsNullOrEmpty(updateDto.FullName))
        {
            try 
            {
                newFullName = new FullName(updateDto.FullName.Trim());
            }
            catch (ArgumentException ex)
            {
                throw new BusinessRuleValidationException($"Invalid full name: {ex.Message}");
            }
        }

        if (!string.IsNullOrEmpty(updateDto.PhoneNumber))
        {
            try 
            {
                newPhoneNumber = new PhoneNumber(updateDto.PhoneNumber.Trim());
            }
            catch (ArgumentException ex)
            {
                throw new BusinessRuleValidationException($"Invalid phone number: {ex.Message}");
            }
        }

        if (!string.IsNullOrEmpty(updateDto.EmergencyContactEmail) &&
            !string.IsNullOrEmpty(updateDto.EmergencyContactFullName) &&
            !string.IsNullOrEmpty(updateDto.EmergencyContactPhoneNumber))
        {
            try 
            {
                newEmergencyEmail = new Email(updateDto.EmergencyContactEmail.Trim());
                var emergencyFullName = new FullName(updateDto.EmergencyContactFullName.Trim());
                var emergencyPhone = new PhoneNumber(updateDto.EmergencyContactPhoneNumber.Trim());
                
                newEmergencyContact = new EmergencyContact(
                    emergencyPhone,
                    emergencyFullName,
                    newEmergencyEmail
                );
            }
            catch (ArgumentException ex)
            {
                throw new BusinessRuleValidationException($"Invalid emergency contact information: {ex.Message}");
            }
        }

        if (newEmail != null)
        {
            patient.User.UpdateEmail(newEmail);
        }

        if (newFullName != null)
        {
            patient.User.UpdateFullName(newFullName);
        }

        if (newPhoneNumber != null)
        {
            patient.User.UpdatePhoneNumber(newPhoneNumber);
        }

        if (newEmergencyContact != null)
        {
            patient.UpdateEmergencyContact(newEmergencyContact);
        }

        if (!string.IsNullOrEmpty(updateDto.BirthDate))
        {
            if (DateOnly.TryParse(updateDto.BirthDate, out var birthDate))
            {
                patient.UpdateBirthDate(birthDate);
            }
            else
            {
                throw new BusinessRuleValidationException("Invalid birth date format. Use format: YYYY-MM-DD");
            }
        }

        if (!string.IsNullOrEmpty(updateDto.Gender))
        {
            if (Enum.TryParse<Gender>(updateDto.Gender, true, out var gender))
            {
                patient.UpdateGender(gender);
            }
            else
            {
                throw new BusinessRuleValidationException("Invalid gender. Use 'Male' or 'Female'");
            }
        }

        if (!string.IsNullOrEmpty(updateDto.MedicalConditions))
        {
            var medicalConditions = updateDto.MedicalConditions
                .Split(',')
                .Select(condition => new MedicalCondition(condition.Trim()))
                .ToList();
            patient.UpdateMedicalConditions(medicalConditions);
        }

        await _unitOfWork.CommitAsync();

        return new PatientDto(new UserDto(patient.User), patient.EmergencyContact, patient.MedicalConditions,
            patient.BirthDate, patient.Gender, patient.AppointmentsHistory);
    }
    catch (BusinessRuleValidationException)
    {
        throw;
    }
    catch (Exception ex)
    {
        throw new BusinessRuleValidationException($"Error updating patient: {ex.Message}");
    }
}
    
    
    public async Task<bool> RequestAccountDeletionWithoutEmail(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);

        if (user == null)
        {
            throw new BusinessRuleValidationException("This email does not exist in the database");
        }

        var patient = await _patientRepository.GetByIdAsync(new UserId(user.Id.Value));

        if (patient == null)
        {
            throw new BusinessRuleValidationException("Patient not found");
        }

        // Generate and return the deletion token
        var deletionToken = Guid.NewGuid().ToString();
        user.SetDeletionToken(deletionToken);
        await _unitOfWork.CommitAsync();
    
        return true;
    }
  
    public async Task<bool> ConfirmAccountDeletionByAdmin(string userId)
    {
        var user = await _userRepository.GetByIdAsync(new UserId(userId));

        if (user == null)
        {
            throw new BusinessRuleValidationException("User not found");
        }

        var patient = await _patientRepository.GetByIdAsync(new UserId(userId));

        if (patient == null)
        {
            throw new BusinessRuleValidationException("Patient not found");
        }

        patient.MarkForDeletion();
        user.MarkForDeletion();

        await _unitOfWork.CommitAsync();
        await DeletePatientDataByAdmin(patient);
    
        return true;
    }
    
    
    private async Task DeletePatientDataByAdmin(Patient patient)
    {
        _userRepository.Remove(patient.User);
        _patientRepository.Remove(patient);
    
        await _unitOfWork.CommitAsync();
    }
    
    public async Task<(List<UserDto> Users, int TotalCount)> GetPatientProfilesAsync(
    string searchTerm = null, 
    string searchBy = null,
    int pageNumber = 1, 
    int pageSize = 10)
{
    var users = await this._userRepository.GetAllAsync();
    Console.WriteLine($"Total users found: {users.Count}"); 

    foreach (var user in users)
    {
        Console.WriteLine($"User: {user.FullName}, Role: {user.Role}, Email: {user.Email}");
    }

    var patientUsers = users.Where(u => u.Role == Role.patient).ToList();
    Console.WriteLine($"Patients found: {patientUsers.Count}"); 

    if (!string.IsNullOrWhiteSpace(searchTerm))
    {
        Console.WriteLine($"Searching for: {searchTerm} in {searchBy}"); 
        patientUsers = searchBy?.ToLower() switch
        {
            "email" => patientUsers.Where(u => 
                u.Email.Value.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList(),
            "name" => patientUsers.Where(u => 
                u.FullName.Value.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList(),
            "phone" => patientUsers.Where(u => 
                u.PhoneNumber.Value.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList(),
            _ => patientUsers.Where(u => 
                u.Email.Value.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                u.FullName.Value.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                u.PhoneNumber.Value.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList()
        };
      
    }

    var totalCount = patientUsers.Count;

    var paginatedUsers = patientUsers
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToList();


    var userDtos = paginatedUsers.Select(user => new UserDto(
        user.Id,
        user.Active,
        user.Username.ToString(),
        user.Email,
        user.FullName.ToString(),
        user.PhoneNumber.ToString(),
        user.Role.ToString()
    )).ToList();

    return (userDtos, totalCount);
}
    
    public async Task<PatientDto> SignIn(RegisterPatientDto dto) {
        var temp = await _userRepository.GetByEmailAsync(dto.Email);
        if (temp != null) {
            throw new BusinessRuleValidationException("Email already registed!");
        }

        temp = await _userRepository.GetByUsername(dto.Username);
        if (temp != null) {
            throw new BusinessRuleValidationException("Username already taken!");
        }

        temp = await _userRepository.GetByPhoneNumber(dto.PhoneNumber);
        if (temp != null) {
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