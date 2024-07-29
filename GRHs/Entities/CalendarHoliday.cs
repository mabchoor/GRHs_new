using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRHs.Entities
{
    public class CalendarHoliday
    {
        public int CalendarID { get; set; }
        public Calendar Calendar { get; set; }
        public int HolidayID { get; set; }
        public Holiday Holiday { get; set; }
    }
}