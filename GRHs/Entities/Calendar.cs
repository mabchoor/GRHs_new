using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRHs.Entities
{
    public class Calendar
    {
        public int CalendarID { get; set; }
        public int Year { get; set; }
        public ICollection<CalendarHoliday> CalendarHolidays { get; set; }
    }
}