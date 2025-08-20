using Assignment6.Models.Domain;

namespace Assignment6.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(int userId);
        Task<int> CreateAsync(User user);
        Task<bool> UpdatePasswordAsync(int userId, string passwordHash);
        Task<bool> EmailExistsAsync(string email);
    }

    public interface IPatientRepository
    {
        Task<Patient?> GetByIdAsync(int patientId);
        Task<IEnumerable<Patient>> GetAllAsync();
        Task<IEnumerable<Patient>> SearchAsync(string? searchTerm, int pageNumber, int pageSize);
        Task<int> GetSearchCountAsync(string? searchTerm);
        Task<int> CreateAsync(Patient patient);
        Task<bool> UpdateAsync(Patient patient);
        Task<bool> DeleteAsync(int patientId);
    }

    public interface IDoctorRepository
    {
        Task<Doctor?> GetByIdAsync(int doctorId);
        Task<IEnumerable<Doctor>> GetAllAsync();
        Task<int> CreateAsync(Doctor doctor);
        Task<bool> UpdateAsync(Doctor doctor);
        Task<bool> DeleteAsync(int doctorId);
        Task<IEnumerable<Visit>> GetScheduleAsync(int doctorId, DateTime startDate, DateTime endDate);
        Task<(int TotalPatients, int TotalVisits, decimal TotalBilled)> GetStatisticsAsync(int doctorId, DateTime startDate, DateTime endDate);
    }

    public interface IVisitRepository
    {
        Task<Visit?> GetByIdAsync(int visitId);
        Task<IEnumerable<Visit>> GetAllAsync();
        Task<IEnumerable<Visit>> GetByPatientIdAsync(int patientId);
        Task<IEnumerable<Visit>> GetByDoctorIdAsync(int doctorId);
        Task<int> CreateAsync(Visit visit);
        Task<bool> UpdateStatusAsync(int visitId, string status, string? notes);
        Task<bool> DeleteAsync(int visitId);
        Task<bool> CheckDoctorAvailabilityAsync(int doctorId, DateTime visitDate, int duration);
    }

    public interface IFeeScheduleRepository
    {
        Task<FeeSchedule?> GetByIdAsync(int feeScheduleId);
        Task<IEnumerable<FeeSchedule>> GetAllAsync();
        Task<int> CreateAsync(FeeSchedule feeSchedule);
        Task<bool> UpdateAsync(FeeSchedule feeSchedule);
        Task<bool> DeleteAsync(int feeScheduleId);
    }

    public interface IActivityLogRepository
    {
        Task<IEnumerable<ActivityLog>> GetAsync(string? entityType, DateTime? startDate, DateTime? endDate, int pageNumber, int pageSize);
        Task<int> GetCountAsync(string? entityType, DateTime? startDate, DateTime? endDate);
        Task CreateAsync(ActivityLog activityLog);
    }
}
