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

    public async Task<(bool Success, string Message, long Id)>
    CreateUserSessionAsync(
        CreateUserSessionRequest request)
    {
        using var connection =
            (SqlConnection)_connectionFactory.CreateConnection();

        using var command = new SqlCommand(
            "i_conf_user_session_mst_insert",
            connection
        );

        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue(
            "@session_id",
            request.SessionId
        );

        command.Parameters.AddWithValue(
            "@user_id",
            request.UserId
        );

        command.Parameters.AddWithValue(
            "@active_hospital_id",
            (object?)request.ActiveHospitalId ?? DBNull.Value
        );

        command.Parameters.AddWithValue(
            "@active_branch_id",
            (object?)request.ActiveBranchId ?? DBNull.Value
        );

        command.Parameters.AddWithValue(
            "@refresh_token_hash",
            request.RefreshTokenHash
        );

        command.Parameters.AddWithValue(
            "@token_version",
            request.TokenVersion
        );

        command.Parameters.AddWithValue(
            "@refresh_expiry_dnt",
            request.RefreshExpiryDate
        );

        command.Parameters.AddWithValue(
            "@ip_address",
            (object?)request.IpAddress ?? DBNull.Value
        );

        command.Parameters.AddWithValue(
            "@user_agent",
            (object?)request.UserAgent ?? DBNull.Value
        );

        await connection.OpenAsync();

        using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return
            (
                Success:
                    Convert.ToInt32(reader["status"]) == 1,

                Message:
                    reader["message"]?.ToString()
                    ?? string.Empty,

                Id:
                    Convert.ToInt64(reader["id"])
            );
        }

        return
        (
            Success: false,
            Message: "Unable to create user session.",
            Id: 0
        );
    }

    public async Task<(bool Success, string Message)>
    RotateRefreshTokenAsync(
        string sessionId,
        string oldRefreshTokenHash,
        string newRefreshTokenHash,
        DateTime newRefreshExpiryDate,
        string? modifyIp)
    {
        using var connection =
            (SqlConnection)_connectionFactory.CreateConnection();

        using var command = new SqlCommand(
            "i_conf_user_session_refresh_update",
            connection
        );

        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue(
            "@session_id",
            sessionId
        );

        command.Parameters.AddWithValue(
            "@old_refresh_token_hash",
            oldRefreshTokenHash
        );

        command.Parameters.AddWithValue(
            "@new_refresh_token_hash",
            newRefreshTokenHash
        );

        command.Parameters.AddWithValue(
            "@new_refresh_expiry_dnt",
            newRefreshExpiryDate
        );

        command.Parameters.AddWithValue(
            "@modify_ip",
            (object?)modifyIp ?? DBNull.Value
        );

        await connection.OpenAsync();

        using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return
            (
                Convert.ToInt32(reader["status"]) == 1,
                reader["message"]?.ToString() ?? string.Empty
            );
        }

        return
        (
            false,
            "Unable to refresh authentication session."
        );
    }

    public async Task<RefreshSessionDto?> GetRefreshSessionAsync(
    string refreshTokenHash)
    {
        using var connection =
            (SqlConnection)_connectionFactory.CreateConnection();

        using var command = new SqlCommand(
            "i_conf_user_session_refresh_select",
            connection
        );

        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue(
            "@refresh_token_hash",
            refreshTokenHash
        );

        await connection.OpenAsync();

        using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new RefreshSessionDto
        {
            SessionId =
                reader["session_id"]?.ToString() ?? string.Empty,

            UserId =
                Convert.ToInt64(reader["user_id"]),

            UserName =
                reader["user_name"]?.ToString() ?? string.Empty,

            ActiveHospitalId =
                reader["active_hospital_id"] == DBNull.Value
                    ? null
                    : Convert.ToInt64(
                        reader["active_hospital_id"]
                    ),

            ActiveBranchId =
                reader["active_branch_id"] == DBNull.Value
                    ? null
                    : Convert.ToInt64(
                        reader["active_branch_id"]
                    ),

            TokenVersion =
                Convert.ToInt32(reader["token_version"]),

            RefreshExpiryDate =
                Convert.ToDateTime(
                    reader["refresh_expiry_dnt"]
                ),

            IsRevoked =
                Convert.ToBoolean(reader["is_revoked"])
        };
    }
}