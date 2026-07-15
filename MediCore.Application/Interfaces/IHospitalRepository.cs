using MediCore.Application.DTOs;

namespace MediCore.Application.Interfaces;

public interface IHospitalRepository
{
    Task<(bool Success, string Message, long Id)> CreateAsync(
        CreateHospitalRequest request,
        long? createdBy,
        string? createdIp);
}