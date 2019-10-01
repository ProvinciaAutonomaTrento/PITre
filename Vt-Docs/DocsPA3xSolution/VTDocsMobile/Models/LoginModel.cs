using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using VTDocsMobile.VTDocsWSMobile;
using VTDocs.mobile.fe;

namespace VTDocs.mobile.fe.model
{
    #region Model
    public class LoginModel
    {
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

    }

    #endregion

    #region Validation
    public class LoginValidation
    {

        public static string ErrorCodeToString(LoginResponseCode code)
        {
            switch (code)
            {
                case LoginResponseCode.SYSTEM_ERROR:
                    return Resources.Errors.Common_SystemError;

                case LoginResponseCode.USER_NOT_FOUND:
                    return Resources.Errors.Login_UserNotFound;

                case LoginResponseCode.PASSWORD_EXPIRED:
                    return Resources.Errors.Login_PasswordExpired;

                default:
                   return Resources.Errors.Common_UnknownError;
            }
        }

    }

    #endregion


}
