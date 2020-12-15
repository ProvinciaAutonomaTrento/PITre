using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocsPaVO.utente;
using DocsPaVO.Mobile;

namespace DocsPaVO.Mobile.Responses
{
    public class VerifyUpdateResponse
    {

        public VerifyUpdateResponse() { }

        public VerifyUpdateResponse(UpdateResponseCode code)
        {
            this.Code = code;
            switch (code)
            {
                case UpdateResponseCode.NO_UPDATE: this.ErrorMessageUpdate = "Nessun aggiornamento non disponibile"; break;
                //case LoginResponseCode.SYSTEM_ERROR: this.ErrorMessageLogin = "Application Error"; break;
                //case LoginResponseCode.USER_NOT_FOUND: this.ErrorMessageLogin = "Utente o password errati"; break;
                //case LoginResponseCode.MULTIAMM: this.ErrorMessageLogin = "MultiAmministrazione"; break;
            }
        }
/*
        public VerifyUpdateResponse(UserLogin.LoginResult loginResult)
        {
            switch (loginResult)
            {
                case UserLogin.LoginResult.UNKNOWN_USER:
                    this.Code = LoginResponseCode.USER_NOT_FOUND;
                    this.ErrorMessageLogin = "Utente o password errati";
                    break;
                case UserLogin.LoginResult.PASSWORD_EXPIRED:
                    this.Code = LoginResponseCode.PASSWORD_EXPIRED;
                    this.ErrorMessageLogin = "Password scaduta";
                    break;
                case UserLogin.LoginResult.NO_AMMIN:
                    this.Code = LoginResponseCode.MULTIAMM;
                    this.ErrorMessageLogin = "MultiAmministrazione";
                    break;
                default:
                    this.Code = LoginResponseCode.SYSTEM_ERROR;
                    this.ErrorMessageLogin = "Application Error";
                    break;
            }
        }

        public VerifyUpdateResponse(UserLogin.ResetPasswordResult otpResult)
        {
            switch (otpResult)
            {
                case UserLogin.ResetPasswordResult.INVALID_USERID: // .otp UNKNOWN_USER:
                    this.Code = LoginResponseCode.OTP_USER_NOT_FOUND;
                    this.ErrorMessageLogin = "Utente non riconosciuto";
                    break;
                case UserLogin.ResetPasswordResult.INVALID_EMAIL : // .otp UNKNOWN_USER:
                    this.Code = LoginResponseCode.OTP_EMAIL_ERROR;
                    this.ErrorMessageLogin = "L'E-mail per recupero password non è imostata, contattare l'amministratore del sistema";
                    break;
                case UserLogin.ResetPasswordResult.ERROR_SEND_MAIL : // .otp UNKNOWN_USER:
                    this.Code = LoginResponseCode.OTP_SEND_EMAIL_ERROR;
                    this.ErrorMessageLogin = "Errore nel'invio del'e-mail per recupero password, se persiste, contattare l'amministratore del sistema";
                    break;
                default:
                    this.Code = LoginResponseCode.SYSTEM_ERROR;
                    this.ErrorMessageLogin = "Application Error";
                    break;
            }
        }
        */
       

        public VerifyUpdateResponse(string url)
        {
            this.url = url;
                 this.Code = UpdateResponseCode.YES_UPDATE;
                this.ErrorMessageUpdate = "";
           

        }


        public static VerifyUpdateResponse NoUpdate
        {
            get
            {
                VerifyUpdateResponse res = new VerifyUpdateResponse(UpdateResponseCode.NO_UPDATE);
                return res;
            }
        }

        public string url
        {
            get;
            set;
        }

        
        public UpdateResponseCode Code
        {
            get;
            set;
        }

        public string ErrorMessageUpdate { get; set; }

       
    }

    public enum UpdateResponseCode
    {
        YES_UPDATE, NO_UPDATE
    }
}