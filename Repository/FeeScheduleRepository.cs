using System.Data;
using System.Data.SqlClient;
using Assignment6.Infrastructure.Data;
using Assignment6.Models.Domain;
using Assignment6.Repository.Interfaces;

namespace Assignment6.Repository.Implementations
{
    public class FeeScheduleRepository : IFeeScheduleRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public FeeScheduleRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<FeeSchedule?> GetByIdAsync(int feeScheduleId)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "SELECT FeeScheduleID, ServiceName, ServiceCode, Amount, Description, IsActive, CreatedDate, ModifiedDate " +
                "FROM FeeSchedule WHERE FeeScheduleID = @FeeScheduleID AND IsActive = 1", 
                (SqlConnection)connection);
            
            command.Parameters.AddWithValue("@FeeScheduleID", feeScheduleId);
            
            connection.Open();
            using var reader = await command.ExecuteReaderAsync();
            
            if (await reader.ReadAsync())
            {
                return MapToFeeSchedule(reader);
            }
            
            return null;
        }

        public async Task<IEnumerable<FeeSchedule>> GetAllAsync()
        {
            var feeSchedules = new List<FeeSchedule>();
            
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "SELECT FeeScheduleID, ServiceName, ServiceCode, Amount, Description, IsActive, CreatedDate, ModifiedDate " +
                "FROM FeeSchedule WHERE IsActive = 1 ORDER BY ServiceName", 
                (SqlConnection)connection);
            
            connection.Open();
            using var reader = await command.ExecuteReaderAsync();
            
            while (await reader.ReadAsync())
            {
                feeSchedules.Add(MapToFeeSchedule(reader));
            }
            
            return feeSchedules;
        }

        public async Task<FeeSchedule?> GetByServiceTypeAsync(string serviceName)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "SELECT FeeScheduleID, ServiceName, ServiceCode, Amount, Description, IsActive, CreatedDate, ModifiedDate " +
                "FROM FeeSchedule WHERE ServiceName = @ServiceName AND IsActive = 1", 
                (SqlConnection)connection);
            
            command.Parameters.AddWithValue("@ServiceName", serviceName);
            
            connection.Open();
            using var reader = await command.ExecuteReaderAsync();
            
            if (await reader.ReadAsync())
            {
                return MapToFeeSchedule(reader);
            }
            
            return null;
        }

        public async Task<int> CreateAsync(FeeSchedule feeSchedule)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "INSERT INTO FeeSchedule (ServiceName, ServiceCode, Amount, Description) " +
                "OUTPUT INSERTED.FeeScheduleID " +
                "VALUES (@ServiceName, @ServiceCode, @Amount, @Description)", 
                (SqlConnection)connection);
            
            command.Parameters.AddWithValue("@ServiceName", feeSchedule.ServiceName);
            command.Parameters.AddWithValue("@ServiceCode", (object?)feeSchedule.ServiceCode ?? DBNull.Value);
            command.Parameters.AddWithValue("@Amount", feeSchedule.Amount);
            command.Parameters.AddWithValue("@Description", (object?)feeSchedule.Description ?? DBNull.Value);
            
            connection.Open();
            var result = await command.ExecuteScalarAsync();
            return result != null ? (int)result : 0;
        }

        public async Task<bool> UpdateAsync(FeeSchedule feeSchedule)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "UPDATE FeeSchedule SET " +
                "ServiceName = @ServiceName, ServiceCode = @ServiceCode, Amount = @Amount, Description = @Description, ModifiedDate = GETDATE() " +
                "WHERE FeeScheduleID = @FeeScheduleID", 
                (SqlConnection)connection);
            
            command.Parameters.AddWithValue("@FeeScheduleID", feeSchedule.FeeScheduleID);
            command.Parameters.AddWithValue("@ServiceName", feeSchedule.ServiceName);
            command.Parameters.AddWithValue("@ServiceCode", (object?)feeSchedule.ServiceCode ?? DBNull.Value);
            command.Parameters.AddWithValue("@Amount", feeSchedule.Amount);
            command.Parameters.AddWithValue("@Description", (object?)feeSchedule.Description ?? DBNull.Value);
            
            connection.Open();
            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int feeScheduleId)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "UPDATE FeeSchedule SET IsActive = 0, ModifiedDate = GETDATE() WHERE FeeScheduleID = @FeeScheduleID", 
                (SqlConnection)connection);
            
            command.Parameters.AddWithValue("@FeeScheduleID", feeScheduleId);
            
            connection.Open();
            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        private static FeeSchedule MapToFeeSchedule(IDataReader reader)
        {
            return new FeeSchedule
            {
                FeeScheduleID = reader.GetInt32(reader.GetOrdinal("FeeScheduleID")),
                ServiceName = reader.GetString(reader.GetOrdinal("ServiceName")),
                ServiceCode = reader.IsDBNull(reader.GetOrdinal("ServiceCode")) ? null : reader.GetString(reader.GetOrdinal("ServiceCode")),
                Amount = reader.GetDecimal(reader.GetOrdinal("Amount")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                ModifiedDate = reader.GetDateTime(reader.GetOrdinal("ModifiedDate"))
            };
        }
    }
}
