using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS_DAL.Models.Common
{
    public class ListFilterBaseModel
    {
        public int? Start { get; set; }
        public int? Length { get; set; }
        public string? CurrentUserId { get; set; }
    }
}
