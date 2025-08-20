using Assignment6.Models.Domain;
using Assignment6.Models.DTOs;
using Assignment6.Repository.Interfaces;

namespace Assignment6.Services
{
    public interface IDoctorService
    {
        Task<DoctorDto?> GetByIdAsync(int doctorId);
        Task<IEnumerable<DoctorDto>> GetAllAsync();
        Task<int> CreateAsync(CreateDoctorDto createDoctorDto);
        Task<bool> UpdateAsync(int doctorId, UpdateDoctorDto updateDoctorDto);
        Task<bool> DeleteAsync(int doctorId);
        Task<IEnumerable<VisitDto>> GetScheduleAsync(int doctorId, DateTime startDate, DateTime endDate);
        Task<DoctorStatisticsDto> GetStatisticsAsync(int doctorId, DateTime startDate, DateTime endDate);
    }

    public interface IVisitService
    {
        Task<VisitDto?> GetByIdAsync(int visitId);
        Task<IEnumerable<VisitDto>> GetAllAsync();
        Task<IEnumerable<VisitDto>> GetByPatientIdAsync(int patientId);
        Task<IEnumerable<VisitDto>> GetByDoctorIdAsync(int doctorId);
        Task<int> CreateAsync(CreateVisitDto createVisitDto);
        Task<bool> UpdateStatusAsync(int visitId, UpdateVisitStatusDto updateStatusDto);
        Task<bool> DeleteAsync(int visitId);
    }

    public interface IFeeScheduleService
    {
        Task<FeeScheduleDto?> GetByIdAsync(int feeScheduleId);
        Task<IEnumerable<FeeScheduleDto>> GetAllAsync();
        Task<int> CreateAsync(CreateFeeScheduleDto createFeeScheduleDto);
        Task<bool> UpdateAsync(int feeScheduleId, UpdateFeeScheduleDto updateFeeScheduleDto);
        Task<bool> DeleteAsync(int feeScheduleId);
    }

    public interface IActivityLogService
    {
        Task<PagedResultDto<ActivityLogDto>> GetAsync(string? entityType, DateTime? startDate, DateTime? endDate, int pageNumber, int pageSize);
    }

    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IActivityLogRepository _activityLogRepository;

        public DoctorService(IDoctorRepository doctorRepository, IActivityLogRepository activityLogRepository)
        {
            _doctorRepository = doctorRepository;
            _activityLogRepository = activityLogRepository;
        }

        public async Task<DoctorDto?> GetByIdAsync(int doctorId)
        {
            var doctor = await _doctorRepository.GetByIdAsync(doctorId);
            return doctor != null ? MapToDto(doctor) : null;
        }

        public async Task<IEnumerable<DoctorDto>> GetAllAsync()
        {
            var doctors = await _doctorRepository.GetAllAsync();
            return doctors.Select(MapToDto);
        }

        public async Task<int> CreateAsync(CreateDoctorDto createDoctorDto)
        {
            var doctor = MapFromCreateDto(createDoctorDto);
            var doctorId = await _doctorRepository.CreateAsync(doctor);

            await _activityLogRepository.CreateAsync(new ActivityLog
            {
                EntityType = "Doctor",
                EntityID = doctorId,
                Action = "Created",
                Details = $"New doctor {doctor.FullName} created"
            });

            return doctorId;
        }

        public async Task<bool> UpdateAsync(int doctorId, UpdateDoctorDto updateDoctorDto)
        {
            var doctor = MapFromUpdateDto(doctorId, updateDoctorDto);
            var result = await _doctorRepository.UpdateAsync(doctor);

            if (result)
            {
                await _activityLogRepository.CreateAsync(new ActivityLog
                {
                    EntityType = "Doctor",
                    EntityID = doctorId,
                    Action = "Updated",
                    Details = $"Doctor {doctor.FullName} updated"
                });
            }

            return result;
        }

        public async Task<bool> DeleteAsync(int doctorId)
        {
            var doctor = await _doctorRepository.GetByIdAsync(doctorId);
            if (doctor == null) return false;

            var result = await _doctorRepository.DeleteAsync(doctorId);

            if (result)
            {
                await _activityLogRepository.CreateAsync(new ActivityLog
                {
                    EntityType = "Doctor",
                    EntityID = doctorId,
                    Action = "Deleted",
                    Details = $"Doctor {doctor.FullName} deleted"
                });
            }

            return result;
        }

        public async Task<IEnumerable<VisitDto>> GetScheduleAsync(int doctorId, DateTime startDate, DateTime endDate)
        {
            var visits = await _doctorRepository.GetScheduleAsync(doctorId, startDate, endDate);
            return visits.Select(MapVisitToDto);
        }

        public async Task<DoctorStatisticsDto> GetStatisticsAsync(int doctorId, DateTime startDate, DateTime endDate)
        {
            var stats = await _doctorRepository.GetStatisticsAsync(doctorId, startDate, endDate);
            return new DoctorStatisticsDto
            {
                TotalPatients = stats.TotalPatients,
                TotalVisits = stats.TotalVisits,
                TotalBilled = stats.TotalBilled
            };
        }

        private static DoctorDto MapToDto(Doctor doctor)
        {
            return new DoctorDto
            {
                DoctorID = doctor.DoctorID,
                FirstName = doctor.FirstName,
                LastName = doctor.LastName,
                Specialization = doctor.Specialization ?? string.Empty,
                LicenseNumber = doctor.LicenseNumber ?? string.Empty,
                Email = doctor.Email ?? string.Empty,
                Phone = doctor.Phone ?? string.Empty,
                CreatedDate = doctor.CreatedDate
            };
        }

        private static Doctor MapFromCreateDto(CreateDoctorDto dto)
        {
            return new Doctor
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Specialization = dto.Specialization,
                LicenseNumber = dto.LicenseNumber,
                Email = dto.Email,
                Phone = dto.Phone
            };
        }

        private static Doctor MapFromUpdateDto(int doctorId, UpdateDoctorDto dto)
        {
            return new Doctor
            {
                DoctorID = doctorId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Specialization = dto.Specialization,
                LicenseNumber = dto.LicenseNumber,
                Email = dto.Email,
                Phone = dto.Phone
            };
        }

        private static VisitDto MapVisitToDto(Visit visit)
        {
            return new VisitDto
            {
                VisitID = visit.VisitID,
                PatientID = visit.PatientID,
                DoctorID = visit.DoctorID,
                VisitDate = visit.VisitDate,
                Notes = visit.Notes ?? string.Empty,
                VisitType = visit.VisitType,
                Status = visit.Status,
                Reason = visit.Reason ?? string.Empty,
                Duration = visit.Duration,
                TotalAmount = visit.TotalAmount,
                PatientName = visit.Patient?.FirstName ?? "Unknown",
                DoctorName = visit.Doctor?.FirstName ?? "Unknown",
                DoctorSpecialization = visit.Doctor?.Specialization ?? string.Empty,
                CreatedDate = visit.CreatedDate
            };
        }
    }
}
