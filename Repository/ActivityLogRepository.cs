using System.Data;
using System.Data.SqlClient;
using Assignment6.Infrastructure.Data;
using Assignment6.Models.Domain;
using Assignment6.Repository.Interfaces;

namespace Assignment6.Repository.Implementations
{
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
            
            connection.Open();
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
            
            connection.Open();
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
            
            connection.Open();
            await command.ExecuteNonQueryAsync();
        }
    }
}
