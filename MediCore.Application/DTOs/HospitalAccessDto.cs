using System;
using System.Collections.Generic;
using System.Text;

namespace MediCore.Application.DTOs
{
    public class HospitalAccessDto
    {
        public long HospitalId { get; set; }

        public string HospitalName { get; set; } = string.Empty;

        public string? HospitalShortName { get; set; }

        public bool IsDefault { get; set; }
    }
}
