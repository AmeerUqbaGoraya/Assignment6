using System.Data;
using System.Data.SqlClient;
using Assignment6.Infrastructure.Data;
using Assignment6.Models.Domain;
using Assignment6.Repository.Interfaces;

namespace Assignment6.Repository.Implementations
{
    public class VisitRepository : IVisitRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public VisitRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Visit?> GetByIdAsync(int visitId)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "SELECT v.VisitID, v.PatientID, v.DoctorID, v.VisitDate, v.VisitType, " +
                "v.Status, v.Reason, v.Notes, v.Duration, v.TotalAmount, v.CreatedDate, v.ModifiedDate, " +
                "p.FirstName as PatientFirstName, p.LastName as PatientLastName, " +
                "d.FirstName as DoctorFirstName, d.LastName as DoctorLastName, d.Specialization " +
                "FROM Visits v " +
                "LEFT JOIN Patients p ON v.PatientID = p.PatientID " +
                "LEFT JOIN Doctors d ON v.DoctorID = d.DoctorID " +
                "WHERE v.VisitID = @VisitID", 
                (SqlConnection)connection);
            
            command.Parameters.AddWithValue("@VisitID", visitId);
            
            connection.Open();
            using var reader = await command.ExecuteReaderAsync();
            
            if (await reader.ReadAsync())
            {
                return MapToVisitWithRelations(reader);
            }
            
            return null;
        }

        public async Task<IEnumerable<Visit>> GetAllAsync()
        {
            var visits = new List<Visit>();
            
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "SELECT v.VisitID, v.PatientID, v.DoctorID, v.VisitDate, v.VisitType, " +
                "v.Status, v.Reason, v.Notes, v.Duration, v.TotalAmount, v.CreatedDate, v.ModifiedDate, " +
                "p.FirstName as PatientFirstName, p.LastName as PatientLastName, " +
                "d.FirstName as DoctorFirstName, d.LastName as DoctorLastName, d.Specialization " +
                "FROM Visits v " +
                "LEFT JOIN Patients p ON v.PatientID = p.PatientID " +
                "LEFT JOIN Doctors d ON v.DoctorID = d.DoctorID " +
                "ORDER BY v.VisitDate DESC", 
                (SqlConnection)connection);
            
            connection.Open();
            using var reader = await command.ExecuteReaderAsync();
            
            while (await reader.ReadAsync())
            {
                visits.Add(MapToVisitWithRelations(reader));
            }
            
            return visits;
        }

        public async Task<IEnumerable<Visit>> GetByPatientIdAsync(int patientId)
        {
            var visits = new List<Visit>();
            
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "SELECT v.VisitID, v.PatientID, v.DoctorID, v.VisitDate, v.VisitType, " +
                "v.Status, v.Reason, v.Notes, v.Duration, v.TotalAmount, v.CreatedDate, v.ModifiedDate, " +
                "p.FirstName as PatientFirstName, p.LastName as PatientLastName, " +
                "d.FirstName as DoctorFirstName, d.LastName as DoctorLastName, d.Specialization " +
                "FROM Visits v " +
                "LEFT JOIN Patients p ON v.PatientID = p.PatientID " +
                "LEFT JOIN Doctors d ON v.DoctorID = d.DoctorID " +
                "WHERE v.PatientID = @PatientID " +
                "ORDER BY v.VisitDate DESC", 
                (SqlConnection)connection);
            
            command.Parameters.AddWithValue("@PatientID", patientId);
            
            connection.Open();
            using var reader = await command.ExecuteReaderAsync();
            
            while (await reader.ReadAsync())
            {
                visits.Add(MapToVisitWithRelations(reader));
            }
            
            return visits;
        }

        public async Task<IEnumerable<Visit>> GetByDoctorIdAsync(int doctorId)
        {
            var visits = new List<Visit>();
            
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "SELECT v.VisitID, v.PatientID, v.DoctorID, v.VisitDate, v.VisitType, " +
                "v.Status, v.Reason, v.Notes, v.Duration, v.TotalAmount, v.CreatedDate, v.ModifiedDate, " +
                "p.FirstName as PatientFirstName, p.LastName as PatientLastName, " +
                "d.FirstName as DoctorFirstName, d.LastName as DoctorLastName, d.Specialization " +
                "FROM Visits v " +
                "LEFT JOIN Patients p ON v.PatientID = p.PatientID " +
                "LEFT JOIN Doctors d ON v.DoctorID = d.DoctorID " +
                "WHERE v.DoctorID = @DoctorID " +
                "ORDER BY v.VisitDate DESC", 
                (SqlConnection)connection);
            
            command.Parameters.AddWithValue("@DoctorID", doctorId);
            
            connection.Open();
            using var reader = await command.ExecuteReaderAsync();
            
            while (await reader.ReadAsync())
            {
                visits.Add(MapToVisitWithRelations(reader));
            }
            
            return visits;
        }

        public async Task<int> CreateAsync(Visit visit)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "INSERT INTO Visits (PatientID, DoctorID, VisitDate, VisitType, Status, Reason, Notes, Duration, TotalAmount) " +
                "OUTPUT INSERTED.VisitID " +
                "VALUES (@PatientID, @DoctorID, @VisitDate, @VisitType, @Status, @Reason, @Notes, @Duration, @TotalAmount)", 
                (SqlConnection)connection);
            
            command.Parameters.AddWithValue("@PatientID", visit.PatientID);
            command.Parameters.AddWithValue("@DoctorID", visit.DoctorID);
            command.Parameters.AddWithValue("@VisitDate", visit.VisitDate);
            command.Parameters.AddWithValue("@VisitType", visit.VisitType);
            command.Parameters.AddWithValue("@Status", visit.Status);
            command.Parameters.AddWithValue("@Reason", (object?)visit.Reason ?? DBNull.Value);
            command.Parameters.AddWithValue("@Notes", (object?)visit.Notes ?? DBNull.Value);
            command.Parameters.AddWithValue("@Duration", visit.Duration);
            command.Parameters.AddWithValue("@TotalAmount", visit.TotalAmount);
            
            connection.Open();
            var result = await command.ExecuteScalarAsync();
            return result != null ? (int)result : 0;
        }

        public async Task<bool> UpdateStatusAsync(int visitId, string status, string? notes)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "UPDATE Visits SET Status = @Status, Notes = @Notes, ModifiedDate = GETDATE() " +
                "WHERE VisitID = @VisitID", 
                (SqlConnection)connection);
            
            command.Parameters.AddWithValue("@VisitID", visitId);
            command.Parameters.AddWithValue("@Status", status);
            command.Parameters.AddWithValue("@Notes", (object?)notes ?? DBNull.Value);
            
            connection.Open();
            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int visitId)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "DELETE FROM Visits WHERE VisitID = @VisitID", 
                (SqlConnection)connection);
            
            command.Parameters.AddWithValue("@VisitID", visitId);
            
            connection.Open();
            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> CheckDoctorAvailabilityAsync(int doctorId, DateTime visitDate, int duration)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "SELECT COUNT(*) FROM Visits " +
                "WHERE DoctorID = @DoctorID " +
                "AND Status != 'Cancelled' " +
                "AND ( " +
                "    (VisitDate <= @VisitDate AND DATEADD(MINUTE, Duration, VisitDate) > @VisitDate) OR " +
                "    (VisitDate < DATEADD(MINUTE, @Duration, @VisitDate) AND VisitDate >= @VisitDate) " +
                ")", 
                (SqlConnection)connection);
            
            command.Parameters.AddWithValue("@DoctorID", doctorId);
            command.Parameters.AddWithValue("@VisitDate", visitDate);
            command.Parameters.AddWithValue("@Duration", duration);
            
            connection.Open();
            var result = await command.ExecuteScalarAsync();
            var conflictCount = result != null ? (int)result : 0;
            
            return conflictCount == 0; // Available if no conflicts
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
                ModifiedDate = reader.GetDateTime(reader.GetOrdinal("ModifiedDate"))
            };
        }

        private static Visit MapToVisitWithRelations(IDataReader reader)
        {
            var visit = MapToVisit(reader);
            
            // Add patient info if available
            if (!reader.IsDBNull(reader.GetOrdinal("PatientFirstName")) && !reader.IsDBNull(reader.GetOrdinal("PatientLastName")))
            {
                visit.Patient = new Patient
                {
                    PatientID = visit.PatientID,
                    FirstName = reader.GetString(reader.GetOrdinal("PatientFirstName")),
                    LastName = reader.GetString(reader.GetOrdinal("PatientLastName"))
                };
            }
            
            // Add doctor info if available
            if (!reader.IsDBNull(reader.GetOrdinal("DoctorFirstName")) && !reader.IsDBNull(reader.GetOrdinal("DoctorLastName")))
            {
                visit.Doctor = new Doctor
                {
                    DoctorID = visit.DoctorID,
                    FirstName = reader.GetString(reader.GetOrdinal("DoctorFirstName")),
                    LastName = reader.GetString(reader.GetOrdinal("DoctorLastName")),
                    Specialization = reader.IsDBNull(reader.GetOrdinal("Specialization")) ? null : reader.GetString(reader.GetOrdinal("Specialization"))
                };
            }
            
            return visit;
        }
    }
}
