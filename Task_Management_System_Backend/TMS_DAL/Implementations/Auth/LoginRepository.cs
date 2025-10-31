using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS_DAL.Common;
using TMS_DAL.Entities;
using TMS_DAL.Interfaces.Auth;
using TMS_DAL.Models.Auth;

namespace TMS_DAL.Implementations.Auth
{
    public class LoginRepository : ILoginRepository
    {
        private readonly DbTaskManagementSystemContext _dbContext;

        public LoginRepository(DbTaskManagementSystemContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<LoginDataModel> GetLoginData(string username)
        {
            try
            {
                var user = await _dbContext.Users
                    .Where(u => u.Username == username && u.StatusId == (int)StatusEnum.Active)
                    .Select(u => new LoginDataModel
                    {
                        UserId = u.UserId,
                        PasswordHash = u.PasswordHash,
                        FirstName = u.FirstName,
                        LastName = u.LastName
                    })
                    .FirstOrDefaultAsync();

                return user;
            }
            catch
            {
                return null;
            }
        }
    }
}
