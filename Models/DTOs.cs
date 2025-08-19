using System.ComponentModel.DataAnnotations;

namespace Assignment6.Models.DTOs
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; }
        
        [Required]
        public string FirstName { get; set; }
        
        [Required]
        public string LastName { get; set; }
    }

    public class ChangePasswordDto
    {
        [Required]
        public string CurrentPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }

    public class AuthResponseDto
    {
        public string Token { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public DateTime Expiration { get; set; }
    }

    public class DoctorDto
    {
        public int DoctorID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Specialization { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string LicenseNumber { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public DateTime CreatedDate { get; set; }
    }

    public class CreateDoctorDto
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Specialization { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string LicenseNumber { get; set; }
    }

    public class UpdateDoctorDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Specialization { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string? Phone { get; set; }
        public string LicenseNumber { get; set; }
    }

    public class PatientDto
    {
        public int PatientID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string EmergencyContact { get; set; }
        public string EmergencyPhone { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public int Age => DateTime.Now.Year - DateOfBirth.Year;
        public DateTime CreatedDate { get; set; }
    }

    public class CreatePatientDto
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public string Gender { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string EmergencyContact { get; set; }
        public string EmergencyPhone { get; set; }
    }

    public class UpdatePatientDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string EmergencyContact { get; set; }
        public string EmergencyPhone { get; set; }
    }

    public class VisitDto
    {
        public int VisitID { get; set; }
        public int PatientID { get; set; }
        public int DoctorID { get; set; }
        public DateTime VisitDate { get; set; }
        public string Notes { get; set; }
        public string Status { get; set; }
        public string VisitType { get; set; }
        public string Reason { get; set; }
        public int Duration { get; set; }
        public decimal TotalAmount { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public string DoctorSpecialization { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class CreateVisitDto
    {
        [Required]
        public int PatientID { get; set; }
        [Required]
        public int DoctorID { get; set; }
        [Required]
        public DateTime VisitDate { get; set; }
        public string Notes { get; set; }
        public string VisitType { get; set; }
        public string Reason { get; set; }
        public int Duration { get; set; }
    }

    public class UpdateVisitStatusDto
    {
        [Required]
        public string Status { get; set; }
        public string Notes { get; set; }
    }

    public class FeeScheduleDto
    {
        public int FeeScheduleID { get; set; }
        public string CPTCode { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
    }

    public class CreateFeeScheduleDto
    {
        [Required]
        public string CPTCode { get; set; }
        public string Description { get; set; }
        [Required]
        public decimal Amount { get; set; }
        public string ServiceName { get; set; }
        public string ServiceCode { get; set; }
    }
    
    public class UpdateFeeScheduleDto
    {
        public string CPTCode { get; set; }
        public string Description { get; set; }
        public decimal? Amount { get; set; }
    }

    public class DoctorStatisticsDto
    {
        public int TotalPatients { get; set; }
        public int TotalVisits { get; set; }
        public decimal TotalBilled { get; set; }
    }

    public class ActivityLogDto
    {
        public int LogID { get; set; }
        public string UserName { get; set; }
        public string Action { get; set; }
        public string Details { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class PagedResultDto<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
