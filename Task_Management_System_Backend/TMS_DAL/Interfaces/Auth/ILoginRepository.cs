using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS_DAL.Models.Auth;

namespace TMS_DAL.Interfaces.Auth
{
    public interface ILoginRepository
    {
        Task<LoginDataModel> GetLoginData(string username);
    }
}
