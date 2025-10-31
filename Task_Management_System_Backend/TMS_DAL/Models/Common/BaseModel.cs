using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS_DAL.Models.Common
{
    public class BaseModel
    {
        public string? CurrentUserId { get; set; }
        public DateTimeOffset? CreatedDateTime { get; set; }
        public DateTimeOffset? UpdatedDateTime { get; set; }
        public DateTimeOffset? DeletedDateTime { get; set; }
        public int? EntryVersion { get; set; }
        public string? EntryIdentifier { get; set; }
    }
}
