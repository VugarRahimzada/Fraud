using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraud.Core.Entities
{
    public class Card : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; }

        public DateTime ValidDate { get; set; }

        public byte TransferLimit { get; set; }

        public int UserId { get; set; }

        public User User { get; set; } = null!;

    }
}
