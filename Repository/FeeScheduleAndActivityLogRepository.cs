// Author: Your Name
// Date: August 19, 2025

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
            
            await connection.OpenAsync();
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
            
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            
            while (await reader.ReadAsync())
            {
                feeSchedules.Add(MapToFeeSchedule(reader));
            }
            
            return feeSchedules;
        }

        public async Task<int> CreateAsync(FeeSchedule feeSchedule)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "INSERT INTO FeeSchedule (ServiceName, ServiceCode, Amount, Description) " +
                "OUTPUT INSERTED.FeeScheduleID " +
                "VALUES (@ServiceName, @ServiceCode, @Amount, @Description)", 
                (SqlConnection)connection);
            
            AddFeeScheduleParameters(command, feeSchedule);
            
            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<bool> UpdateAsync(FeeSchedule feeSchedule)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "UPDATE FeeSchedule SET ServiceName = @ServiceName, ServiceCode = @ServiceCode, " +
                "Amount = @Amount, Description = @Description, ModifiedDate = GETDATE() " +
                "WHERE FeeScheduleID = @FeeScheduleID", 
                (SqlConnection)connection);
            
            command.Parameters.AddWithValue("@FeeScheduleID", feeSchedule.FeeScheduleID);
            AddFeeScheduleParameters(command, feeSchedule);
            
            await connection.OpenAsync();
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
            
            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        private static void AddFeeScheduleParameters(SqlCommand command, FeeSchedule feeSchedule)
        {
            command.Parameters.AddWithValue("@ServiceName", feeSchedule.ServiceName);
            command.Parameters.AddWithValue("@ServiceCode", (object?)feeSchedule.ServiceCode ?? DBNull.Value);
            command.Parameters.AddWithValue("@Amount", feeSchedule.Amount);
            command.Parameters.AddWithValue("@Description", (object?)feeSchedule.Description ?? DBNull.Value);
        }

        private static FeeSchedule MapToFeeSchedule(IDataReader reader)
        {
            return new FeeSchedule
            {
                FeeScheduleID = reader.GetInt32("FeeScheduleID"),
                ServiceName = reader.GetString("ServiceName"),
                ServiceCode = reader.IsDBNull("ServiceCode") ? null : reader.GetString("ServiceCode"),
                Amount = reader.GetDecimal("Amount"),
                Description = reader.IsDBNull("Description") ? null : reader.GetString("Description"),
                IsActive = reader.GetBoolean("IsActive"),
                CreatedDate = reader.GetDateTime("CreatedDate"),
                ModifiedDate = reader.GetDateTime("ModifiedDate")
            };
        }
    }

    public class ActivityLogRepository : IActivityLogRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ActivityLogRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<ActivityLog>> GetAsync(string? entityType, DateTime? startDate, DateTime? endDate, int pageNumber, int pageSize)
        {
            var activityLogs = new List<ActivityLog>();
            
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("sp_GetActivityLog", (SqlConnection)connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            
            command.Parameters.AddWithValue("@EntityType", (object?)entityType ?? DBNull.Value);
            command.Parameters.AddWithValue("@StartDate", (object?)startDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@EndDate", (object?)endDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@PageNumber", pageNumber);
            command.Parameters.AddWithValue("@PageSize", pageSize);
            
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            
            while (await reader.ReadAsync())
            {
                activityLogs.Add(new ActivityLog
                {
                    ActivityLogID = reader.GetInt32("ActivityLogID"),
                    EntityType = reader.GetString("EntityType"),
                    EntityID = reader.GetInt32("EntityID"),
                    Action = reader.GetString("Action"),
                    Details = reader.IsDBNull("Details") ? null : reader.GetString("Details"),
                    IPAddress = reader.IsDBNull("IPAddress") ? null : reader.GetString("IPAddress"),
                    CreatedDate = reader.GetDateTime("CreatedDate"),
                    User = reader.IsDBNull("UserName") ? null : new User
                    {
                        FirstName = reader.GetString("UserName"),
                        Role = reader.GetString("Role")
                    }
                });
            }
            
            return activityLogs;
        }

        public async Task<int> GetCountAsync(string? entityType, DateTime? startDate, DateTime? endDate)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("sp_GetActivityLog", (SqlConnection)connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            
            command.Parameters.AddWithValue("@EntityType", (object?)entityType ?? DBNull.Value);
            command.Parameters.AddWithValue("@StartDate", (object?)startDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@EndDate", (object?)endDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@PageNumber", 1);
            command.Parameters.AddWithValue("@PageSize", 1);
            
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            
            // Skip first result set (activity logs)
            await reader.NextResultAsync();
            
            if (await reader.ReadAsync())
            {
                return reader.GetInt32("TotalRecords");
            }
            
            return 0;
        }

        public async Task CreateAsync(ActivityLog activityLog)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "INSERT INTO ActivityLogs (UserID, EntityType, EntityID, Action, Details, IPAddress) " +
                "VALUES (@UserID, @EntityType, @EntityID, @Action, @Details, @IPAddress)", 
                (SqlConnection)connection);
            
            command.Parameters.AddWithValue("@UserID", (object?)activityLog.UserID ?? DBNull.Value);
            command.Parameters.AddWithValue("@EntityType", activityLog.EntityType);
            command.Parameters.AddWithValue("@EntityID", activityLog.EntityID);
            command.Parameters.AddWithValue("@Action", activityLog.Action);
            command.Parameters.AddWithValue("@Details", (object?)activityLog.Details ?? DBNull.Value);
            command.Parameters.AddWithValue("@IPAddress", (object?)activityLog.IPAddress ?? DBNull.Value);
            
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }
    }
}
