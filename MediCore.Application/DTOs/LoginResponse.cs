using System;
using System.Collections.Generic;
using System.Text;

namespace MediCore.Application.DTOs
{
    public class LoginResponse
    {
        public long UserId { get; set; }

        public string UserCode { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string DisplayName { get; set; } = string.Empty;

        public long UserGroupId { get; set; }

        public string UserGroupCode { get; set; } = string.Empty;

        public string UserGroupName { get; set; } = string.Empty;

        public string? UserImage { get; set; }

        public List<HospitalAccessDto> Hospitals { get; set; } = new();

        public List<BranchAccessDto> Branches { get; set; } = new();

        public bool RequiresHospitalSelection { get; set; }

        public bool RequiresBranchSelection { get; set; }

        public long? SelectedHospitalId { get; set; }

        public long? SelectedBranchId { get; set; }
    }
}
