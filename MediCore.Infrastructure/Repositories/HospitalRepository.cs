using System.Data;
using MediCore.Application.DTOs;
using MediCore.Application.Interfaces;
using Microsoft.Data.SqlClient;

namespace MediCore.Infrastructure.Repositories;

public class HospitalRepository : IHospitalRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public HospitalRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<(bool Success, string Message, long Id)> CreateAsync(
        CreateHospitalRequest request,
        long? createdBy,
        string? createdIp)
    {
        using var connection = (SqlConnection)_connectionFactory.CreateConnection();

        using var command = new SqlCommand("i_conf_hospital_mst_insert", connection);

        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("@status", request.Status);
        command.Parameters.AddWithValue("@create_by", (object?)createdBy ?? DBNull.Value);
        command.Parameters.AddWithValue("@create_ip", (object?)createdIp ?? DBNull.Value);
        command.Parameters.AddWithValue("@hosp_code", request.HospitalCode);
        command.Parameters.AddWithValue("@hosp_name", request.HospitalName);
        command.Parameters.AddWithValue("@hosp_short_name", (object?)request.HospitalShortName ?? DBNull.Value);
        command.Parameters.AddWithValue("@hosp_email", (object?)request.Email ?? DBNull.Value);
        command.Parameters.AddWithValue("@hosp_mobile", (object?)request.Mobile ?? DBNull.Value);
        command.Parameters.AddWithValue("@hosp_alternate_mobile", (object?)request.AlternateMobile ?? DBNull.Value);
        command.Parameters.AddWithValue("@hosp_address", (object?)request.Address ?? DBNull.Value);
        command.Parameters.AddWithValue("@hosp_city", (object?)request.City ?? DBNull.Value);
        command.Parameters.AddWithValue("@hosp_state", (object?)request.State ?? DBNull.Value);
        command.Parameters.AddWithValue("@hosp_country", (object?)request.Country ?? DBNull.Value);
        command.Parameters.AddWithValue("@hosp_pincode", (object?)request.Pincode ?? DBNull.Value);
        command.Parameters.AddWithValue("@hosp_logo", (object?)request.Logo ?? DBNull.Value);
        command.Parameters.AddWithValue("@hosp_gst_no", (object?)request.GstNumber ?? DBNull.Value);
        command.Parameters.AddWithValue("@hosp_pan_no", (object?)request.PanNumber ?? DBNull.Value);
        command.Parameters.AddWithValue("@hosp_start_date", (object?)request.StartDate ?? DBNull.Value);
        command.Parameters.AddWithValue("@hosp_expiry_date", (object?)request.ExpiryDate ?? DBNull.Value);

        await connection.OpenAsync();

        using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return
            (
                Convert.ToInt32(reader["status"]) == 1,
                reader["message"].ToString()!,
                Convert.ToInt64(reader["id"])
            );
        }

        return (false, "Unknown error.", 0);
    }
}