using System;
using System.Collections.Generic;
using System.Text;

namespace MediCore.Application.DTOs
{
    public class BranchAccessDto
    {
        public long HospitalId { get; set; }

        public long BranchId { get; set; }

        public string BranchName { get; set; } = string.Empty;

        public string? BranchShortName { get; set; }

        public bool IsDefault { get; set; }
    }
}
