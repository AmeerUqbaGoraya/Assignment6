using System.Data;
using System.Data.SqlClient;
using Assignment6.Infrastructure.Data;
using Assignment6.Models.Domain;
using Assignment6.Repository.Interfaces;

namespace Assignment6.Repository.Implementations
{
    public class PatientRepository : IPatientRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public PatientRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Patient?> GetByIdAsync(int patientId)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "SELECT PatientID, FirstName, LastName, DateOfBirth, Gender, Email, Phone, " +
                "Address, EmergencyContact, EmergencyPhone, IsActive, CreatedDate, ModifiedDate " +
                "FROM Patients WHERE PatientID = @PatientID AND IsActive = 1", 
                (SqlConnection)connection);
            
            command.Parameters.AddWithValue("@PatientID", patientId);
            
            connection.Open();
            using var reader = await command.ExecuteReaderAsync();
            
            if (await reader.ReadAsync())
            {
                return MapToPatient(reader);
            }
            
            return null;
        }

        public async Task<IEnumerable<Patient>> GetAllAsync()
        {
            var patients = new List<Patient>();
            
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "SELECT PatientID, FirstName, LastName, DateOfBirth, Gender, Email, Phone, " +
                "Address, EmergencyContact, EmergencyPhone, IsActive, CreatedDate, ModifiedDate " +
                "FROM Patients WHERE IsActive = 1 ORDER BY LastName, FirstName", 
                (SqlConnection)connection);
            
            connection.Open();
            using var reader = await command.ExecuteReaderAsync();
            
            while (await reader.ReadAsync())
            {
                patients.Add(MapToPatient(reader));
            }
            
            return patients;
        }

        public async Task<IEnumerable<Patient>> SearchAsync(string? searchTerm, int pageNumber, int pageSize)
        {
            var patients = new List<Patient>();
            
            using var connection = _connectionFactory.CreateConnection();
            
            var sql = "SELECT PatientID, FirstName, LastName, DateOfBirth, Gender, Email, Phone, " +
                     "Address, EmergencyContact, EmergencyPhone, IsActive, CreatedDate, ModifiedDate " +
                     "FROM Patients WHERE IsActive = 1 ";
            
            if (!string.IsNullOrEmpty(searchTerm))
            {
                sql += "AND (FirstName LIKE @SearchTerm OR LastName LIKE @SearchTerm OR Email LIKE @SearchTerm) ";
            }
            
            sql += "ORDER BY LastName, FirstName OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            
            using var command = new SqlCommand(sql, (SqlConnection)connection);
            
            if (!string.IsNullOrEmpty(searchTerm))
            {
                command.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");
            }
            
            command.Parameters.AddWithValue("@Offset", (pageNumber - 1) * pageSize);
            command.Parameters.AddWithValue("@PageSize", pageSize);
            
            connection.Open();
            using var reader = await command.ExecuteReaderAsync();
            
            while (await reader.ReadAsync())
            {
                patients.Add(MapToPatient(reader));
            }
            
            return patients;
        }

        public async Task<int> GetSearchCountAsync(string? searchTerm)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var sql = "SELECT COUNT(*) FROM Patients WHERE IsActive = 1 ";
            
            if (!string.IsNullOrEmpty(searchTerm))
            {
                sql += "AND (FirstName LIKE @SearchTerm OR LastName LIKE @SearchTerm OR Email LIKE @SearchTerm)";
            }
            
            using var command = new SqlCommand(sql, (SqlConnection)connection);
            
            if (!string.IsNullOrEmpty(searchTerm))
            {
                command.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");
            }
            
            connection.Open();
            var result = await command.ExecuteScalarAsync();
            return result != null ? (int)result : 0;
        }

        public async Task<int> CreateAsync(Patient patient)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "INSERT INTO Patients (FirstName, LastName, DateOfBirth, Gender, Email, Phone, Address, EmergencyContact, EmergencyPhone) " +
                "OUTPUT INSERTED.PatientID " +
                "VALUES (@FirstName, @LastName, @DateOfBirth, @Gender, @Email, @Phone, @Address, @EmergencyContact, @EmergencyPhone)", 
                (SqlConnection)connection);
            
            command.Parameters.AddWithValue("@FirstName", patient.FirstName);
            command.Parameters.AddWithValue("@LastName", patient.LastName);
            command.Parameters.AddWithValue("@DateOfBirth", patient.DateOfBirth);
            command.Parameters.AddWithValue("@Gender", patient.Gender);
            command.Parameters.AddWithValue("@Email", (object?)patient.Email ?? DBNull.Value);
            command.Parameters.AddWithValue("@Phone", (object?)patient.Phone ?? DBNull.Value);
            command.Parameters.AddWithValue("@Address", (object?)patient.Address ?? DBNull.Value);
            command.Parameters.AddWithValue("@EmergencyContact", (object?)patient.EmergencyContact ?? DBNull.Value);
            command.Parameters.AddWithValue("@EmergencyPhone", (object?)patient.EmergencyPhone ?? DBNull.Value);
            
            connection.Open();
            var result = await command.ExecuteScalarAsync();
            return result != null ? (int)result : 0;
        }

        public async Task<bool> UpdateAsync(Patient patient)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "UPDATE Patients SET " +
                "FirstName = @FirstName, LastName = @LastName, DateOfBirth = @DateOfBirth, Gender = @Gender, " +
                "Email = @Email, Phone = @Phone, Address = @Address, EmergencyContact = @EmergencyContact, " +
                "EmergencyPhone = @EmergencyPhone, ModifiedDate = GETDATE() " +
                "WHERE PatientID = @PatientID", 
                (SqlConnection)connection);
            
            command.Parameters.AddWithValue("@PatientID", patient.PatientID);
            command.Parameters.AddWithValue("@FirstName", patient.FirstName);
            command.Parameters.AddWithValue("@LastName", patient.LastName);
            command.Parameters.AddWithValue("@DateOfBirth", patient.DateOfBirth);
            command.Parameters.AddWithValue("@Gender", patient.Gender);
            command.Parameters.AddWithValue("@Email", (object?)patient.Email ?? DBNull.Value);
            command.Parameters.AddWithValue("@Phone", (object?)patient.Phone ?? DBNull.Value);
            command.Parameters.AddWithValue("@Address", (object?)patient.Address ?? DBNull.Value);
            command.Parameters.AddWithValue("@EmergencyContact", (object?)patient.EmergencyContact ?? DBNull.Value);
            command.Parameters.AddWithValue("@EmergencyPhone", (object?)patient.EmergencyPhone ?? DBNull.Value);
            
            connection.Open();
            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int patientId)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "UPDATE Patients SET IsActive = 0, ModifiedDate = GETDATE() WHERE PatientID = @PatientID", 
                (SqlConnection)connection);
            
            command.Parameters.AddWithValue("@PatientID", patientId);
            
            connection.Open();
            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        private static Patient MapToPatient(IDataReader reader)
        {
            return new Patient
            {
                PatientID = reader.GetInt32(reader.GetOrdinal("PatientID")),
                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                DateOfBirth = reader.GetDateTime(reader.GetOrdinal("DateOfBirth")),
                Gender = reader.GetString(reader.GetOrdinal("Gender")),
                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email")),
                Phone = reader.IsDBNull(reader.GetOrdinal("Phone")) ? null : reader.GetString(reader.GetOrdinal("Phone")),
                Address = reader.IsDBNull(reader.GetOrdinal("Address")) ? null : reader.GetString(reader.GetOrdinal("Address")),
                EmergencyContact = reader.IsDBNull(reader.GetOrdinal("EmergencyContact")) ? null : reader.GetString(reader.GetOrdinal("EmergencyContact")),
                EmergencyPhone = reader.IsDBNull(reader.GetOrdinal("EmergencyPhone")) ? null : reader.GetString(reader.GetOrdinal("EmergencyPhone")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                ModifiedDate = reader.GetDateTime(reader.GetOrdinal("ModifiedDate"))
            };
        }
    }
}
