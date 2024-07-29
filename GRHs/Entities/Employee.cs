using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRHs.Entities
{
    public class Employee
    {
        public int EmployeeID { get; set; }
        public int UserID { get; set; }
        public Users User { get; set; }
        public string EmployeeNumber { get; set; }
        public string CIN { get; set; }
        public int PositionID { get; set; }
        public Position Position { get; set; }
        public int DepartmentID { get; set; }
        public Departement Department { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

    }
}