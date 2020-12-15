using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocsPaVO.utente;
using DocsPaVO.Mobile;

namespace DocsPaVO.Mobile.Responses
{
    public class LoginResponse
    {

        public LoginResponse() { }

        public LoginResponse(LoginResponseCode code)
        {
            this.Code = code;
            switch (code)
            {
                case LoginResponseCode.PASSWORD_EXPIRED: this.ErrorMessageLogin = "Password scaduta"; break;
                case LoginResponseCode.SYSTEM_ERROR: this.ErrorMessageLogin = "Application Error"; break;
                case LoginResponseCode.USER_NOT_FOUND: this.ErrorMessageLogin = "Utente o password errati"; break;
                case LoginResponseCode.MULTIAMM: this.ErrorMessageLogin = "MultiAmministrazione"; break;
                case LoginResponseCode.INVALID_OTP: this.ErrorMessageLogin = "INVALID_OTP Error"; break;
                case LoginResponseCode.PASSWORD_EQUALITY: this.ErrorMessageLogin = "La password deve essere differente rispetto a quella precedente"; break;
                case LoginResponseCode.DOMAIN_AUTH_ENABLED: this.ErrorMessageLogin = "La password non può essere modificata in quanto per l'utente è attivata l'autenticazione di dominio"; break;
            }
        }
       

        public LoginResponse(UserLogin.LoginResult loginResult)
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

        public LoginResponse(UserLogin.ResetPasswordResult otpResult)
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

        public LoginResponse(Utente utente, UserLogin.LoginResult loginResult)
        {
            if (loginResult == DocsPaVO.utente.UserLogin.LoginResult.OK)
            {
                this.UserInfo = UserInfo.buildInstance(utente);
                this.Code = LoginResponseCode.OK;
                this.ErrorMessageLogin = "";
            }
            else if (loginResult == DocsPaVO.utente.UserLogin.LoginResult.UNKNOWN_USER)
            {
                this.Code = LoginResponseCode.USER_NOT_FOUND;
                this.ErrorMessageLogin = "Utente o password errati";
            }
            else if (loginResult == DocsPaVO.utente.UserLogin.LoginResult.PASSWORD_EXPIRED)
            {
                this.Code = LoginResponseCode.PASSWORD_EXPIRED;
                this.ErrorMessageLogin = "Password scaduta";
            }
            else if (loginResult == UserLogin.LoginResult.NO_AMMIN)
            {
                this.Code = LoginResponseCode.MULTIAMM;
                this.ErrorMessageLogin = "MultiAmministrazione";
            }
            else
            {
                this.Code = LoginResponseCode.SYSTEM_ERROR;
                this.ErrorMessageLogin = "Application Error";
            }

        }

        public LoginResponse(string idUtente, string email)
        {
            this.UserInfo = UserInfo.buildInstance(idUtente, email);
            //this.OTPInfo  = new OTPInfo(idUtente , email);
                this.Code = LoginResponseCode.OTP_OK;
                this.ErrorMessageLogin = "";
           

        }


        public static LoginResponse ErrorResponse
        {
            get
            {
                LoginResponse res = new LoginResponse(LoginResponseCode.SYSTEM_ERROR);
                return res;
            }
        }

        public UserInfo UserInfo
        {
            get;
            set;
        }

        public bool OTPAllowed { get; set; }
        public bool ShareAllowed { get; set; }
      //  public OTPInfo OTPInfo { get; }
        public LoginResponseCode Code
        {
            get;
            set;
        }

        public string ErrorMessageLogin { get; set; }

        public Memento InfoMemento { get; set; }

        public string TodoListRemoval { get; set; }
    }

    public enum LoginResponseCode
    {
        OK, USER_NOT_FOUND, PASSWORD_EXPIRED, MULTIAMM, SYSTEM_ERROR , OTP_OK , OTP_USER_NOT_FOUND, OTP_EMAIL_ERROR, OTP_SEND_EMAIL_ERROR, INVALID_OTP, PASSWORD_EQUALITY, DOMAIN_AUTH_ENABLED
    }
}