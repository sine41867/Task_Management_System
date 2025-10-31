using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS_DAL.Models.Common;

namespace TMS_DAL.Models.Task
{
    public class TaskHasStatusModel
    {
        public string? TaskStatus { get; set; }
        public DateTimeOffset? TimeStamp { get; set; }
    }
}
