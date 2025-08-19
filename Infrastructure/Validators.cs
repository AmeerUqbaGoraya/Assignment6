// Author: Your Name
// Date: August 19, 2025

using FluentValidation;
using Assignment6.Models.DTOs;

namespace Assignment6.Infrastructure.Validators
{
    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(255).WithMessage("Email cannot exceed 255 characters");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long");
        }
    }

    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(255).WithMessage("Email cannot exceed 255 characters");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]")
                .WithMessage("Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(100).WithMessage("First name cannot exceed 100 characters")
                .Matches(@"^[a-zA-Z\s]+$").WithMessage("First name can only contain letters and spaces");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters")
                .Matches(@"^[a-zA-Z\s]+$").WithMessage("Last name can only contain letters and spaces");

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Role is required")
                .Must(role => new[] { "Admin", "Doctor", "Receptionist" }.Contains(role))
                .WithMessage("Role must be Admin, Doctor, or Receptionist");
        }
    }

    public class CreatePatientDtoValidator : AbstractValidator<CreatePatientDto>
    {
        public CreatePatientDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(100).WithMessage("First name cannot exceed 100 characters")
                .Matches(@"^[a-zA-Z\s]+$").WithMessage("First name can only contain letters and spaces");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters")
                .Matches(@"^[a-zA-Z\s]+$").WithMessage("Last name can only contain letters and spaces");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Date of birth is required")
                .Must(BeAValidAge).WithMessage("Patient must be between 0 and 150 years old");

            RuleFor(x => x.Gender)
                .NotEmpty().WithMessage("Gender is required")
                .Must(gender => new[] { "Male", "Female", "Other" }.Contains(gender))
                .WithMessage("Gender must be Male, Female, or Other");

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(255).WithMessage("Email cannot exceed 255 characters")
                .When(x => !string.IsNullOrEmpty(x.Email));

            RuleFor(x => x.Phone)
                .Matches(@"^\+?[\d\s\-\(\)]+$").WithMessage("Invalid phone number format")
                .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters")
                .When(x => !string.IsNullOrEmpty(x.Phone));

            RuleFor(x => x.Address)
                .MaximumLength(500).WithMessage("Address cannot exceed 500 characters");

            RuleFor(x => x.EmergencyContact)
                .MaximumLength(255).WithMessage("Emergency contact cannot exceed 255 characters");

            RuleFor(x => x.EmergencyPhone)
                .Matches(@"^\+?[\d\s\-\(\)]+$").WithMessage("Invalid emergency phone number format")
                .MaximumLength(20).WithMessage("Emergency phone number cannot exceed 20 characters")
                .When(x => !string.IsNullOrEmpty(x.EmergencyPhone));
        }

        private bool BeAValidAge(DateTime dateOfBirth)
        {
            var age = DateTime.Today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > DateTime.Today.AddYears(-age)) age--;
            return age >= 0 && age <= 150;
        }
    }

    public class CreateDoctorDtoValidator : AbstractValidator<CreateDoctorDto>
    {
        public CreateDoctorDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(100).WithMessage("First name cannot exceed 100 characters")
                .Matches(@"^[a-zA-Z\s]+$").WithMessage("First name can only contain letters and spaces");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters")
                .Matches(@"^[a-zA-Z\s]+$").WithMessage("Last name can only contain letters and spaces");

            RuleFor(x => x.Specialization)
                .MaximumLength(100).WithMessage("Specialization cannot exceed 100 characters");

            RuleFor(x => x.LicenseNumber)
                .MaximumLength(50).WithMessage("License number cannot exceed 50 characters")
                .Matches(@"^[A-Z]{2}\d{6,}$").WithMessage("License number must be in format: 2 letters followed by at least 6 digits")
                .When(x => !string.IsNullOrEmpty(x.LicenseNumber));

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(255).WithMessage("Email cannot exceed 255 characters")
                .When(x => !string.IsNullOrEmpty(x.Email));

            RuleFor(x => x.Phone)
                .Matches(@"^\+?[\d\s\-\(\)]+$").WithMessage("Invalid phone number format")
                .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters")
                .When(x => !string.IsNullOrEmpty(x.Phone));
        }
    }

    public class CreateVisitDtoValidator : AbstractValidator<CreateVisitDto>
    {
        public CreateVisitDtoValidator()
        {
            RuleFor(x => x.PatientID)
                .GreaterThan(0).WithMessage("Patient ID must be greater than 0");

            RuleFor(x => x.DoctorID)
                .GreaterThan(0).WithMessage("Doctor ID must be greater than 0");

            RuleFor(x => x.VisitDate)
                .NotEmpty().WithMessage("Visit date is required")
                .Must(BeAFutureOrTodayDate).WithMessage("Visit date cannot be in the past");

            RuleFor(x => x.VisitType)
                .NotEmpty().WithMessage("Visit type is required")
                .Must(type => new[] { "Regular", "Emergency", "Follow-up", "Consultation" }.Contains(type))
                .WithMessage("Invalid visit type");

            RuleFor(x => x.Reason)
                .MaximumLength(500).WithMessage("Reason cannot exceed 500 characters");

            RuleFor(x => x.Duration)
                .InclusiveBetween(15, 240).WithMessage("Duration must be between 15 and 240 minutes");
        }

        private bool BeAFutureOrTodayDate(DateTime visitDate)
        {
            return visitDate.Date >= DateTime.Today;
        }
    }

    public class CreateFeeScheduleDtoValidator : AbstractValidator<CreateFeeScheduleDto>
    {
        public CreateFeeScheduleDtoValidator()
        {
            RuleFor(x => x.ServiceName)
                .NotEmpty().WithMessage("Service name is required")
                .MaximumLength(200).WithMessage("Service name cannot exceed 200 characters");

            RuleFor(x => x.ServiceCode)
                .MaximumLength(50).WithMessage("Service code cannot exceed 50 characters")
                .Matches(@"^[A-Z]{2,3}\d{3}$").WithMessage("Service code must be 2-3 letters followed by 3 digits")
                .When(x => !string.IsNullOrEmpty(x.ServiceCode));

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than 0")
                .LessThanOrEqualTo(10000).WithMessage("Amount cannot exceed $10,000");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");
        }
    }

    public class UpdateVisitStatusDtoValidator : AbstractValidator<UpdateVisitStatusDto>
    {
        public UpdateVisitStatusDtoValidator()
        {
            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required")
                .Must(status => new[] { "Scheduled", "InProgress", "Completed", "Cancelled" }.Contains(status))
                .WithMessage("Invalid status. Must be Scheduled, InProgress, Completed, or Cancelled");

            RuleFor(x => x.Notes)
                .MaximumLength(1000).WithMessage("Notes cannot exceed 1000 characters");
        }
    }
}
