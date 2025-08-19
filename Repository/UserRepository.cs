using System.Data;
using System.Data.SqlClient;
using Assignment6.Infrastructure.Data;
using Assignment6.Models.Domain;
using Assignment6.Repository.Interfaces;

namespace Assignment6.Repository.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public UserRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "SELECT UserID, Email, PasswordHash, PasswordSalt, Role, FirstName, LastName, IsActive, CreatedDate, ModifiedDate " +
                "FROM Users WHERE Email = @Email AND IsActive = 1", 
                (SqlConnection)connection);
            
            command.Parameters.AddWithValue("@Email", email);
            
            connection.Open();
            using var reader = await command.ExecuteReaderAsync();
            
            if (await reader.ReadAsync())
            {
                return MapToUser(reader);
            }
            
            return null;
        }

        public async Task<User?> GetByIdAsync(int userId)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "SELECT UserID, Email, PasswordHash, PasswordSalt, Role, FirstName, LastName, IsActive, CreatedDate, ModifiedDate " +
                "FROM Users WHERE UserID = @UserID AND IsActive = 1", 
                (SqlConnection)connection);
            
            command.Parameters.AddWithValue("@UserID", userId);
            
            connection.Open();
            using var reader = await command.ExecuteReaderAsync();
            
            if (await reader.ReadAsync())
            {
                return MapToUser(reader);
            }
            
            return null;
        }

        public async Task<int> CreateAsync(User user)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "INSERT INTO Users (Email, PasswordHash, PasswordSalt, Role, FirstName, LastName) " +
                "OUTPUT INSERTED.UserID " +
                "VALUES (@Email, @PasswordHash, @PasswordSalt, @Role, @FirstName, @LastName)", 
                (SqlConnection)connection);
            
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
            command.Parameters.AddWithValue("@PasswordSalt", user.PasswordSalt);
            command.Parameters.AddWithValue("@Role", user.Role);
            command.Parameters.AddWithValue("@FirstName", user.FirstName);
            command.Parameters.AddWithValue("@LastName", user.LastName);
            
            connection.Open();
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<bool> UpdatePasswordAsync(int userId, string passwordHash, string passwordSalt)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "UPDATE Users SET PasswordHash = @PasswordHash, PasswordSalt = @PasswordSalt, ModifiedDate = GETDATE() " +
                "WHERE UserID = @UserID", 
                (SqlConnection)connection);
            
            command.Parameters.AddWithValue("@UserID", userId);
            command.Parameters.AddWithValue("@PasswordHash", passwordHash);
            command.Parameters.AddWithValue("@PasswordSalt", passwordSalt);
            
            connection.Open();
            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "SELECT COUNT(1) FROM Users WHERE Email = @Email AND IsActive = 1", 
                (SqlConnection)connection);
            
            command.Parameters.AddWithValue("@Email", email);
            
            connection.Open();
            var result = await command.ExecuteScalarAsync();
            var count = result != null ? (int)result : 0;
            return count > 0;
        }

        private static User MapToUser(IDataReader reader)
        {
            return new User
            {
                UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                PasswordSalt = reader.GetString(reader.GetOrdinal("PasswordSalt")),
                Role = reader.GetString(reader.GetOrdinal("Role")),
                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                ModifiedDate = reader.GetDateTime(reader.GetOrdinal("ModifiedDate"))
            };
        }
    }
}
