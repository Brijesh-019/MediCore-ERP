using System.Data;
using MediCore.Application.DTOs;
using MediCore.Application.Interfaces;
using Microsoft.Data.SqlClient;

namespace MediCore.Infrastructure.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public AuthRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<LoginResponse?> GetLoginUserAsync(string userName)
    {
        using var connection = (SqlConnection)_connectionFactory.CreateConnection();

        using var command = new SqlCommand("i_conf_user_login_select", connection);

        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("@user_name", userName);

        await connection.OpenAsync();

        using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        return new LoginResponse
        {
            UserId = Convert.ToInt64(reader["user_id"]),
            UserCode = reader["user_code"].ToString() ?? "",
            UserName = reader["user_name"].ToString() ?? "",
            PasswordHash = reader["password_hash"].ToString() ?? string.Empty,
            DisplayName = reader["display_name"].ToString() ?? "",
            UserGroupId = Convert.ToInt64(reader["user_group_id"]),
            UserGroupCode = reader["user_group_code"].ToString() ?? "",
            UserGroupName = reader["user_group_name"].ToString() ?? "",
            UserImage = reader["user_image"] == DBNull.Value
                ? null
                : reader["user_image"].ToString()
        };
    }

    public async Task<List<HospitalAccessDto>> GetUserHospitalsAsync(long userId)
    {
        var hospitals = new List<HospitalAccessDto>();

        using var connection = (SqlConnection)_connectionFactory.CreateConnection();

        using var command = new SqlCommand("i_conf_user_hospital_mapping_select", connection);

        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("@user_id", userId);

        await connection.OpenAsync();

        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            hospitals.Add(new HospitalAccessDto
            {
                HospitalId = Convert.ToInt64(reader["hospital_id"]),
                HospitalName = reader["hospital_name"].ToString() ?? "",
                HospitalShortName = reader["hospital_short_name"] == DBNull.Value
                    ? null
                    : reader["hospital_short_name"].ToString(),
                IsDefault = Convert.ToBoolean(reader["is_default"])
            });
        }

        return hospitals;
    }

    public async Task<List<BranchAccessDto>> GetUserBranchesAsync(long userId, long hospitalId)
    {
        var branches = new List<BranchAccessDto>();

        using var connection = (SqlConnection)_connectionFactory.CreateConnection();

        using var command = new SqlCommand("i_conf_user_branch_mapping_select", connection);

        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("@user_id", userId);
        command.Parameters.AddWithValue("@hospital_id", hospitalId);

        await connection.OpenAsync();

        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            branches.Add(new BranchAccessDto
            {
                HospitalId = Convert.ToInt64(reader["hospital_id"]),
                BranchId = Convert.ToInt64(reader["branch_id"]),
                BranchName = reader["branch_name"].ToString() ?? "",
                BranchShortName = reader["branch_short_name"] == DBNull.Value
                    ? null
                    : reader["branch_short_name"].ToString(),
                IsDefault = Convert.ToBoolean(reader["is_default"])
            });
        }

        return branches;
    }
}