using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocsPaVO.utente;

namespace DocsPaVO.Mobile.Requests
{
    public class LoginRequest
    {

        public string UserName
        {
            get; 
            set;
        }

        public string Password
        {
            get; 
            set; 
        }

        public UserLogin UserLogin
        {
            get {
                UserLogin res = new UserLogin();
                res.UserName = this.UserName;
                res.Password = this.Password;
                return res;
            }
        }
    }
}