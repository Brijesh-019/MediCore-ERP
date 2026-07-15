using MediCore.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediCore.Application.Interfaces
{
    public interface IAuthRepository
    {
        Task<LoginResponse?> GetLoginUserAsync(string userName);

        Task<List<HospitalAccessDto>> GetUserHospitalsAsync(long userId);

        Task<List<BranchAccessDto>> GetUserBranchesAsync(
            long userId,
            long hospitalId);
    }
}
