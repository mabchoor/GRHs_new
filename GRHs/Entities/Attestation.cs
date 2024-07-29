using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRHs.Entities
{
    public class Attestation
    {
        public int AttestationID { get; set; }
        public int EmployeeID { get; set; }
        public Employee Employee { get; set; }
        public DateTime RequestDate { get; set; }
        public string Status { get; set; }
        public DateTime? IssueDate { get; set; }
    }
}