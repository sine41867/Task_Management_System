using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS_DAL.Models.Common
{
    public class ExecutionResponseModel
    {
        public int? ExecutionResultId { get; set; }
        public string? ResponseText { get; set; }
        public object? Data { get; set; }
    }
}
