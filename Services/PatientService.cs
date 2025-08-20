using Assignment6.Models.Domain;
using Assignment6.Models.DTOs;
using Assignment6.Repository.Interfaces;

namespace Assignment6.Services
{
    public interface IPatientService
    {
        Task<PatientDto?> GetByIdAsync(int patientId);
        Task<PagedResultDto<PatientDto>> GetAllAsync(string? searchTerm, int pageNumber, int pageSize);
        Task<int> CreateAsync(CreatePatientDto createPatientDto);
        Task<bool> UpdateAsync(int patientId, UpdatePatientDto updatePatientDto);
        Task<bool> DeleteAsync(int patientId);
    }

    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IActivityLogRepository _activityLogRepository;

        public PatientService(IPatientRepository patientRepository, IActivityLogRepository activityLogRepository)
        {
            _patientRepository = patientRepository;
            _activityLogRepository = activityLogRepository;
        }

        public async Task<PatientDto?> GetByIdAsync(int patientId)
        {
            var patient = await _patientRepository.GetByIdAsync(patientId);
            return patient != null ? MapToDto(patient) : null;
        }

        public async Task<PagedResultDto<PatientDto>> GetAllAsync(string? searchTerm, int pageNumber, int pageSize)
        {
            var patients = await _patientRepository.SearchAsync(searchTerm, pageNumber, pageSize);
            var totalCount = await _patientRepository.GetSearchCountAsync(searchTerm);

            return new PagedResultDto<PatientDto>
            {
                Items = patients.Select(MapToDto).ToList(),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<int> CreateAsync(CreatePatientDto createPatientDto)
        {
            var patient = MapFromCreateDto(createPatientDto);
            var patientId = await _patientRepository.CreateAsync(patient);

            await _activityLogRepository.CreateAsync(new ActivityLog
            {
                EntityType = "Patient",
                EntityID = patientId,
                Action = "Created",
                Details = $"New patient {patient.FullName} created"
            });

            return patientId;
        }

        public async Task<bool> UpdateAsync(int patientId, UpdatePatientDto updatePatientDto)
        {
            var patient = MapFromUpdateDto(patientId, updatePatientDto);
            var result = await _patientRepository.UpdateAsync(patient);

            if (result)
            {
                await _activityLogRepository.CreateAsync(new ActivityLog
                {
                    EntityType = "Patient",
                    EntityID = patientId,
                    Action = "Updated",
                    Details = $"Patient {patient.FullName} updated"
                });
            }

            return result;
        }

        public async Task<bool> DeleteAsync(int patientId)
        {
            var patient = await _patientRepository.GetByIdAsync(patientId);
            if (patient == null) return false;

            var result = await _patientRepository.DeleteAsync(patientId);

            if (result)
            {
                await _activityLogRepository.CreateAsync(new ActivityLog
                {
                    EntityType = "Patient",
                    EntityID = patientId,
                    Action = "Deleted",
                    Details = $"Patient {patient.FullName} deleted"
                });
            }

            return result;
        }

        private static PatientDto MapToDto(Patient patient)
        {
            return new PatientDto
            {
                PatientID = patient.PatientID,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                DateOfBirth = patient.DateOfBirth,
                Gender = patient.Gender,
                Email = patient.Email ?? string.Empty,
                Phone = patient.Phone ?? string.Empty,
                Address = patient.Address ?? string.Empty,
                EmergencyContact = patient.EmergencyContact ?? string.Empty,
                EmergencyPhone = patient.EmergencyPhone ?? string.Empty,
                CreatedDate = patient.CreatedDate
            };
        }

        private static Patient MapFromCreateDto(CreatePatientDto dto)
        {
            return new Patient
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                DateOfBirth = dto.DateOfBirth,
                Gender = dto.Gender,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address,
                EmergencyContact = dto.EmergencyContact,
                EmergencyPhone = dto.EmergencyPhone
            };
        }

        private static Patient MapFromUpdateDto(int patientId, UpdatePatientDto dto)
        {
            return new Patient
            {
                PatientID = patientId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                DateOfBirth = dto.DateOfBirth ?? DateTime.Now,
                Gender = dto.Gender,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address,
                EmergencyContact = dto.EmergencyContact,
                EmergencyPhone = dto.EmergencyPhone
            };
        }
    }
}
