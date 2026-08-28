using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraud.DTO.Card
{
    public class CardDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; }
        public decimal Balance { get; set; }
        public DateTime ValidDate { get; set; }
        public byte TransferLimit { get; set; }
        public int UserId { get; set; }

    }
}
