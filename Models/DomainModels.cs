namespace Assignment6.Models.Domain
{
    public class User
    {
        public int UserID { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        
        public string FullName => $"{FirstName} {LastName}";
    }

    public class Patient
    {
        public int PatientID { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? EmergencyContact { get; set; }
        public string? EmergencyPhone { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        
        public string FullName => $"{FirstName} {LastName}";
        public int Age => DateTime.Now.Year - DateOfBirth.Year - (DateTime.Now.DayOfYear < DateOfBirth.DayOfYear ? 1 : 0);
    }

    public class Doctor
    {
        public int DoctorID { get; set; }
        public int? UserID { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Specialization { get; set; }
        public string? LicenseNumber { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        
        public string FullName => $"{FirstName} {LastName}";
        public User? User { get; set; }
    }

    public class Visit
    {
        public int VisitID { get; set; }
        public int PatientID { get; set; }
        public int DoctorID { get; set; }
        public DateTime VisitDate { get; set; }
        public string VisitType { get; set; } = "Regular";
        public string Status { get; set; } = "Scheduled";
        public string? Reason { get; set; }
        public string? Notes { get; set; }
        public int Duration { get; set; } = 30;
        public decimal TotalAmount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        
        public Patient? Patient { get; set; }
        public Doctor? Doctor { get; set; }
        public List<VisitService> VisitServices { get; set; } = new();
    }

    public class FeeSchedule
    {
        public int FeeScheduleID { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string? ServiceCode { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }

    public class VisitService
    {
        public int VisitServiceID { get; set; }
        public int VisitID { get; set; }
        public int FeeScheduleID { get; set; }
        public int Quantity { get; set; } = 1;
        public decimal Amount { get; set; }
        public DateTime CreatedDate { get; set; }
        
        public FeeSchedule? FeeSchedule { get; set; }
    }

    public class ActivityLog
    {
        public int ActivityLogID { get; set; }
        public int? UserID { get; set; }
        public string EntityType { get; set; } = string.Empty;
        public int EntityID { get; set; }
        public string Action { get; set; } = string.Empty;
        public string? Details { get; set; }
        public string? IPAddress { get; set; }
        public DateTime CreatedDate { get; set; }
        
        public User? User { get; set; }
    }
}
