using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraud.DTO.DTOs
{
    public class UserDto
    {
        public string? Name { get; set; }
        public string? Surname { get; set; }

        public string Email { get; set; }

        public string UniqueCode { get; set; }

        // ISO 3166-1 alpha-2
        // Example: AZ, TR, DE, US
        public string CountryCode { get; set; } = string.Empty;

        public decimal Balance { get; set; }
    }
}
