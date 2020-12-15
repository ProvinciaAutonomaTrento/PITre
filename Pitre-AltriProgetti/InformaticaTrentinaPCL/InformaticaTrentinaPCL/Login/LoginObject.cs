using InformaticaTrentinaPCL.ChangePassword;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformaticaTrentinaPCL.Login
{
    public class LoginObject
    {
        public string username = "";
        public string password = "";
        public string oldPassword = "";
        public string repeatedPassword = "";
        public AmministrazioneModel administration;
        public LoginAdministrationState labelAdministrationState = LoginAdministrationState.DEFAULT;
        public LoginObject() { }
    }
}
