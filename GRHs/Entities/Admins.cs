using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRHs.Entities
{
    public class Admins
    {
        public int AdminID { get; set; }
        public int UserID { get; set; }
        public Users User { get; set; }
    }
}