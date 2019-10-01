using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Rubrica.Library.Security
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class SecurityCredentials
    {
        /// <summary>
        /// 
        /// </summary>
        public SecurityCredentials()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        public SecurityCredentials(string userName)
        {
            this.UserName = userName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public SecurityCredentials(string userName, string password) : this(userName)
        {
            this.Password = password;
        }

        public string UserName { get; set; }

        public string Password { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>        
        /// <returns></returns>
        public static string GetPasswordHash(string userName, string password)
        {
            SHA1 sha = new SHA1CryptoServiceProvider();

            byte[] hash = sha.ComputeHash(Encoding.Default.GetBytes(string.Concat(userName, password)));

            return BitConverter.ToString(hash).Replace("-", string.Empty);
        }
    }
}