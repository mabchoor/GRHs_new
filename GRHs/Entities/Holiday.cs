using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRHs.Entities
{
    public class Holiday
    {
        public int HolidayID { get; set; }
        public string Name { get; set; }
        public int Daysnumber { get; set; }
        public DateTime Date { get; set; }
        
    }
}