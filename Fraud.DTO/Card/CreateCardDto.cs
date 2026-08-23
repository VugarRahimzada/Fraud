using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraud.DTO.Card
{
    public class CreateCardDto
    {
        public string Name { get; set; } = string.Empty;
        public int Code { get; set; }
        public DateTime ValidDate { get; set; }
    }
}
