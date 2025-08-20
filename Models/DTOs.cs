using System.ComponentModel.DataAnnotations;

namespace Assignment6.Models.DTOs
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string Password { get; set; }
    }

    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string Password { get; set; }

        [Required]
        public required string Role { get; set; }
        
        [Required]
        public required string FirstName { get; set; }
        
        [Required]
        public required string LastName { get; set; }
    }

    public class ChangePasswordDto
    {
        [Required]
        public required string CurrentPassword { get; set; }
        [Required]
        public required string NewPassword { get; set; }
    }

    public class AuthResponseDto
    {
        public required string Token { get; set; }
        public required string Role { get; set; }
        public required string Email { get; set; }
        public required string FullName { get; set; }
        public DateTime Expiration { get; set; }
    }

    public class DoctorDto
    {
        public int DoctorID { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Specialization { get; set; }
        public required string Email { get; set; }
        public required string Phone { get; set; }
        public required string LicenseNumber { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public DateTime CreatedDate { get; set; }
    }

    public class CreateDoctorDto
    {
        [Required]
        public required string FirstName { get; set; }
        [Required]
        public required string LastName { get; set; }
        [Required]
        public required string Specialization { get; set; }
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        [Required]
        public required string Phone { get; set; }
        [Required]
        public required string LicenseNumber { get; set; }
    }

    public class UpdateDoctorDto
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Specialization { get; set; }
        [EmailAddress]
        public required string Email { get; set; }
        public string? Phone { get; set; }
        public required string LicenseNumber { get; set; }
    }

    public class PatientDto
    {
        public int PatientID { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public required string Gender { get; set; }
        public required string Email { get; set; }
        public required string Phone { get; set; }
        public required string Address { get; set; }
        public required string EmergencyContact { get; set; }
        public required string EmergencyPhone { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public int Age => DateTime.Now.Year - DateOfBirth.Year;
        public DateTime CreatedDate { get; set; }
    }

    public class CreatePatientDto
    {
        [Required]
        public required string FirstName { get; set; }
        [Required]
        public required string LastName { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public required string Gender { get; set; }
        [EmailAddress]
        public required string Email { get; set; }
        public required string Phone { get; set; }
        public required string Address { get; set; }
        public required string EmergencyContact { get; set; }
        public required string EmergencyPhone { get; set; }
    }

    public class UpdatePatientDto
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public required string Gender { get; set; }
        [EmailAddress]
        public required string Email { get; set; }
        public required string Phone { get; set; }
        public required string Address { get; set; }
        public required string EmergencyContact { get; set; }
        public required string EmergencyPhone { get; set; }
    }

    public class VisitDto
    {
        public int VisitID { get; set; }
        public int PatientID { get; set; }
        public int DoctorID { get; set; }
        public DateTime VisitDate { get; set; }
        public required string Notes { get; set; }
        public required string Status { get; set; }
        public required string VisitType { get; set; }
        public required string Reason { get; set; }
        public int Duration { get; set; }
        public decimal TotalAmount { get; set; }
        public required string PatientName { get; set; }
        public required string DoctorName { get; set; }
        public required string DoctorSpecialization { get; set; }
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
        public required string Notes { get; set; }
        public required string VisitType { get; set; }
        public required string Reason { get; set; }
        public int Duration { get; set; }
    }

    public class UpdateVisitStatusDto
    {
        [Required]
        public required string Status { get; set; }
        public required string Notes { get; set; }
    }

    public class FeeScheduleDto
    {
        public int FeeScheduleID { get; set; }
        public required string CPTCode { get; set; }
        public required string Description { get; set; }
        public decimal Amount { get; set; }
    }

    public class CreateFeeScheduleDto
    {
        [Required]
        public required string CPTCode { get; set; }
        public required string Description { get; set; }
        [Required]
        public decimal Amount { get; set; }
        public required string ServiceName { get; set; }
        public required string ServiceCode { get; set; }
    }
    
    public class UpdateFeeScheduleDto
    {
        public required string CPTCode { get; set; }
        public required string Description { get; set; }
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
        public required string UserName { get; set; }
        public required string Action { get; set; }
        public required string Details { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class PagedResultDto<T>
    {
        public required IEnumerable<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
