using Fraud.DTO.Card;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraud.DTO.Auth
{
    public class MeResponseDto
    {
        public int Id { get; set; }
        public string UserCode { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public List<CardDto> Cards { get; set; } = new();

    }
}
