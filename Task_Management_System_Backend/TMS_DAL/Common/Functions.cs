using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TMS_DAL.Common
{
    public static class Functions
    {
        private static string HashPassword(string password)
        {
            var key = "486424c5b2734116a2a8976d66764fc9UNUHNJKLW=";

            password += key;

            using (SHA256 sha = SHA256.Create())
            {
                byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hash);
            }
        }

        public static bool VerifyPassword(string input, string storedHash)
        {
            return HashPassword(input) == storedHash;
        }

        public static string GetTaskStatusString(int taskStatusId)
        {
            try
            {
                return ((TaskStatusEnum)taskStatusId).ToString();
            }
            catch
            {
                return "N/A";
            }
        }
    }
}
