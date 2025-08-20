# PowerShell script to generate BCrypt hashes
# First, let's use C# inline to generate the hashes

Add-Type -Path "bin\Debug\net8.0\*.dll"

$passwords = @(
    @{ Role = "Admin"; Email = "admin@hospital.com"; Password = "admin" },
    @{ Role = "Receptionist"; Email = "receptionist@hospital.com"; Password = "receptionist" },
    @{ Role = "Doctor"; Email = "dr.smith@hospital.com"; Password = "dr.smith" },
    @{ Role = "Doctor"; Email = "dr.johnson@hospital.com"; Password = "dr.johnson" }
)

Write-Host "-- Generated Password Hashes for Database"
Write-Host "-- Use these hashes in your DML INSERT statements"
Write-Host ""

foreach ($user in $passwords) {
    try {
        $hashedPassword = [BCrypt.Net.BCrypt]::HashPassword($user.Password)
        Write-Host "-- $($user.Role): $($user.Email)"
        Write-Host "-- Password: $($user.Password)"
        Write-Host "-- Hash: '$hashedPassword'"
        Write-Host ""
    }
    catch {
        Write-Host "Error hashing password for $($user.Email): $_"
    }
}

Write-Host "-- Complete INSERT statements:"
Write-Host ""

try {
    $adminHash = [BCrypt.Net.BCrypt]::HashPassword("admin")
    $receptionistHash = [BCrypt.Net.BCrypt]::HashPassword("receptionist")
    $drSmithHash = [BCrypt.Net.BCrypt]::HashPassword("dr.smith")
    $drJohnsonHash = [BCrypt.Net.BCrypt]::HashPassword("dr.johnson")

    Write-Host "INSERT INTO Users (Email, PasswordHash, PasswordSalt, Role, FirstName, LastName) VALUES"
    Write-Host "('admin@hospital.com', '$adminHash', '', 'Admin', 'System', 'Administrator'),"
    Write-Host "('dr.smith@hospital.com', '$drSmithHash', '', 'Doctor', 'John', 'Smith'),"
    Write-Host "('dr.johnson@hospital.com', '$drJohnsonHash', '', 'Doctor', 'Sarah', 'Johnson'),"
    Write-Host "('receptionist@hospital.com', '$receptionistHash', '', 'Receptionist', 'Mary', 'Williams');"
}
catch {
    Write-Host "Error generating hashes: $_"
}
