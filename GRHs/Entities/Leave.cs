using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRHs.Entities
{
    public class Leave
    {
        [Key]
        public int LeaveID { get; set; }

        [Required]
        public int EmployeeID { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public LeaveType Type { get; set; } // Enum or other type representing leave type

        public string Reason { get; set; }

        [Required]
        public int NumberOfDays { get; set; }

        public LeaveStatus Status { get; set; } // Enum or other type representing leave status

        // Navigation property
        [ForeignKey(nameof(EmployeeID))]
        public virtual Employee Employee { get; set; }
    }

    // Enumeration for Leave Type
    public enum LeaveType
    {
        Annual,
        Sick,
        Personal,
        Unpaid,
        other
    }

    // Enumeration for Leave Status
    public enum LeaveStatus
    {
        Pending,
        Approved,
        Rejected
    }
}