using System.Data;
using System.Data.SqlClient;
using Assignment6.Infrastructure.Data;
using Assignment6.Models.Domain;
using Assignment6.Repository.Interfaces;

namespace Assignment6.Repository.Implementations
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public DoctorRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Doctor?> GetByIdAsync(int doctorId)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                @"SELECT d.DoctorID, d.UserID, d.FirstName, d.LastName, d.Specialization, 
                         d.LicenseNumber, d.Email, d.Phone, d.IsActive, d.CreatedDate, d.ModifiedDate,
                         u.Email as UserEmail, u.Role as UserRole
                  FROM Doctors d
                  LEFT JOIN Users u ON d.UserID = u.UserID
                  WHERE d.DoctorID = @DoctorID AND d.IsActive = 1",
                (SqlConnection)connection);
            
            command.Parameters.AddWithValue("@DoctorID", doctorId);
            
            connection.Open();
            using var reader = await command.ExecuteReaderAsync();
            
            if (await reader.ReadAsync())
            {
                return MapToDoctor(reader);
            }
            
            return null;
        }

        public async Task<IEnumerable<Doctor>> GetAllAsync()
        {
            var doctors = new List<Doctor>();
            
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                @"SELECT d.DoctorID, d.UserID, d.FirstName, d.LastName, d.Specialization, 
                         d.LicenseNumber, d.Email, d.Phone, d.IsActive, d.CreatedDate, d.ModifiedDate,
                         u.Email as UserEmail, u.Role as UserRole
                  FROM Doctors d
                  LEFT JOIN Users u ON d.UserID = u.UserID
                  WHERE d.IsActive = 1
                  ORDER BY d.FirstName, d.LastName",
                (SqlConnection)connection);
            
            connection.Open();
            using var reader = await command.ExecuteReaderAsync();
            
            while (await reader.ReadAsync())
            {
                doctors.Add(MapToDoctor(reader));
            }
            
            return doctors;
        }

        public async Task<int> CreateAsync(Doctor doctor)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                @"INSERT INTO Doctors (UserID, FirstName, LastName, Specialization, LicenseNumber, Email, Phone)
                  OUTPUT INSERTED.DoctorID
                  VALUES (@UserID, @FirstName, @LastName, @Specialization, @LicenseNumber, @Email, @Phone)",
                (SqlConnection)connection);
            
            command.Parameters.AddWithValue("@UserID", (object?)doctor.UserID ?? DBNull.Value);
            command.Parameters.AddWithValue("@FirstName", doctor.FirstName);
            command.Parameters.AddWithValue("@LastName", doctor.LastName);
            command.Parameters.AddWithValue("@Specialization", (object?)doctor.Specialization ?? DBNull.Value);
            command.Parameters.AddWithValue("@LicenseNumber", (object?)doctor.LicenseNumber ?? DBNull.Value);
            command.Parameters.AddWithValue("@Email", (object?)doctor.Email ?? DBNull.Value);
            command.Parameters.AddWithValue("@Phone", (object?)doctor.Phone ?? DBNull.Value);
            
            connection.Open();
            var result = await command.ExecuteScalarAsync();
            return result != null ? (int)result : 0;
        }

        public async Task<bool> UpdateAsync(Doctor doctor)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                @"UPDATE Doctors SET 
                         UserID = @UserID, FirstName = @FirstName, LastName = @LastName, 
                         Specialization = @Specialization, LicenseNumber = @LicenseNumber, 
                         Email = @Email, Phone = @Phone, ModifiedDate = GETDATE()
                  WHERE DoctorID = @DoctorID",
                (SqlConnection)connection);
            
            command.Parameters.AddWithValue("@DoctorID", doctor.DoctorID);
            command.Parameters.AddWithValue("@UserID", (object?)doctor.UserID ?? DBNull.Value);
            command.Parameters.AddWithValue("@FirstName", doctor.FirstName);
            command.Parameters.AddWithValue("@LastName", doctor.LastName);
            command.Parameters.AddWithValue("@Specialization", (object?)doctor.Specialization ?? DBNull.Value);
            command.Parameters.AddWithValue("@LicenseNumber", (object?)doctor.LicenseNumber ?? DBNull.Value);
            command.Parameters.AddWithValue("@Email", (object?)doctor.Email ?? DBNull.Value);
            command.Parameters.AddWithValue("@Phone", (object?)doctor.Phone ?? DBNull.Value);
            
            connection.Open();
            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int doctorId)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "UPDATE Doctors SET IsActive = 0, ModifiedDate = GETDATE() WHERE DoctorID = @DoctorID",
                (SqlConnection)connection);
            
            command.Parameters.AddWithValue("@DoctorID", doctorId);
            
            connection.Open();
            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<IEnumerable<Visit>> GetScheduleAsync(int doctorId, DateTime startDate, DateTime endDate)
        {
            var visits = new List<Visit>();
            
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                @"SELECT v.VisitID, v.PatientID, v.DoctorID, v.VisitDate, v.VisitType, 
                         v.Status, v.Reason, v.Notes, v.Duration, v.TotalAmount, 
                         v.CreatedDate, v.ModifiedDate,
                         p.FirstName as PatientFirstName, p.LastName as PatientLastName
                  FROM Visits v
                  JOIN Patients p ON v.PatientID = p.PatientID
                  WHERE v.DoctorID = @DoctorID 
                        AND v.VisitDate >= @StartDate 
                        AND v.VisitDate <= @EndDate
                  ORDER BY v.VisitDate",
                (SqlConnection)connection);
            
            command.Parameters.AddWithValue("@DoctorID", doctorId);
            command.Parameters.AddWithValue("@StartDate", startDate);
            command.Parameters.AddWithValue("@EndDate", endDate);
            
            connection.Open();
            using var reader = await command.ExecuteReaderAsync();
            
            while (await reader.ReadAsync())
            {
                visits.Add(MapToVisit(reader));
            }
            
            return visits;
        }

        public async Task<(int TotalPatients, int TotalVisits, decimal TotalBilled)> GetStatisticsAsync(int doctorId, DateTime startDate, DateTime endDate)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                @"SELECT 
                    COUNT(DISTINCT v.PatientID) as TotalPatients,
                    COUNT(v.VisitID) as TotalVisits,
                    ISNULL(SUM(v.TotalAmount), 0) as TotalBilled
                  FROM Visits v
                  WHERE v.DoctorID = @DoctorID 
                        AND v.VisitDate >= @StartDate 
                        AND v.VisitDate <= @EndDate
                        AND v.Status != 'Cancelled'",
                (SqlConnection)connection);
            
            command.Parameters.AddWithValue("@DoctorID", doctorId);
            command.Parameters.AddWithValue("@StartDate", startDate);
            command.Parameters.AddWithValue("@EndDate", endDate);
            
            connection.Open();
            using var reader = await command.ExecuteReaderAsync();
            
            if (await reader.ReadAsync())
            {
                return (
                    reader.GetInt32("TotalPatients"),
                    reader.GetInt32("TotalVisits"),
                    reader.GetDecimal("TotalBilled")
                );
            }
            
            return (0, 0, 0);
        }

        private static Doctor MapToDoctor(IDataReader reader)
        {
            var doctor = new Doctor
            {
                DoctorID = reader.GetInt32(reader.GetOrdinal("DoctorID")),
                UserID = reader.IsDBNull(reader.GetOrdinal("UserID")) ? null : reader.GetInt32(reader.GetOrdinal("UserID")),
                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                Specialization = reader.IsDBNull(reader.GetOrdinal("Specialization")) ? null : reader.GetString(reader.GetOrdinal("Specialization")),
                LicenseNumber = reader.IsDBNull(reader.GetOrdinal("LicenseNumber")) ? null : reader.GetString(reader.GetOrdinal("LicenseNumber")),
                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email")),
                Phone = reader.IsDBNull(reader.GetOrdinal("Phone")) ? null : reader.GetString(reader.GetOrdinal("Phone")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                ModifiedDate = reader.GetDateTime(reader.GetOrdinal("ModifiedDate"))
            };

            // Add User information if available
            if (!reader.IsDBNull(reader.GetOrdinal("UserEmail")))
            {
                doctor.User = new User
                {
                    UserID = doctor.UserID ?? 0,
                    Email = reader.GetString(reader.GetOrdinal("UserEmail")),
                    Role = reader.GetString(reader.GetOrdinal("UserRole"))
                };
            }

            return doctor;
        }

        private static Visit MapToVisit(IDataReader reader)
        {
            return new Visit
            {
                VisitID = reader.GetInt32(reader.GetOrdinal("VisitID")),
                PatientID = reader.GetInt32(reader.GetOrdinal("PatientID")),
                DoctorID = reader.GetInt32(reader.GetOrdinal("DoctorID")),
                VisitDate = reader.GetDateTime(reader.GetOrdinal("VisitDate")),
                VisitType = reader.GetString(reader.GetOrdinal("VisitType")),
                Status = reader.GetString(reader.GetOrdinal("Status")),
                Reason = reader.IsDBNull(reader.GetOrdinal("Reason")) ? null : reader.GetString(reader.GetOrdinal("Reason")),
                Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                Duration = reader.GetInt32(reader.GetOrdinal("Duration")),
                TotalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount")),
                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                ModifiedDate = reader.GetDateTime(reader.GetOrdinal("ModifiedDate")),
                Patient = new Patient
                {
                    PatientID = reader.GetInt32(reader.GetOrdinal("PatientID")),
                    FirstName = reader.GetString(reader.GetOrdinal("PatientFirstName")),
                    LastName = reader.GetString(reader.GetOrdinal("PatientLastName"))
                }
            };
        }
    }
}
