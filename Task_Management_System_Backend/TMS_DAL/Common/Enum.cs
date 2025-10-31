using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS_DAL.Common
{
    public enum StatusEnum
    {
        Active = 1,
        Inactive = 2,
        Deleted = 3
    }

    public enum  TaskStatusEnum
    {
        Pending = 1,
        Started = 2,
        Completed = 3,
    }

    public enum ExecutionResultEnum
    {
        Success = 1,
        DuplicateResource = 2,
        ResourceNotFound = 3,
        InternalServerError = 4,
        ValidationError = 5,
        Exception = 6,
        InsufficientPermision = 7,
        RecordExist = 8,
        VersionMismatch = 9,
        InvalidCredentials = 10
    }
}
