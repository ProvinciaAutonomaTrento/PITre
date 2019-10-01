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

        public LoginResponse(LoginResponseCode code){
            this.Code=code;
        }

        public LoginResponse(Utente utente,UserLogin.LoginResult loginResult){
            if (loginResult == DocsPaVO.utente.UserLogin.LoginResult.OK){
                this.UserInfo = UserInfo.buildInstance(utente);
                this.Code=LoginResponseCode.OK;
            }else if(loginResult== DocsPaVO.utente.UserLogin.LoginResult.UNKNOWN_USER){
                this.Code=LoginResponseCode.USER_NOT_FOUND;
            }else if(loginResult == DocsPaVO.utente.UserLogin.LoginResult.PASSWORD_EXPIRED) {
                this.Code = LoginResponseCode.PASSWORD_EXPIRED;
            }else{
                this.Code=LoginResponseCode.SYSTEM_ERROR;
            }

        }

        public static LoginResponse ErrorResponse{
            get{ 
               LoginResponse res=new LoginResponse(LoginResponseCode.SYSTEM_ERROR);
               return res;
            }
        }

        public UserInfo UserInfo
        {
            get; 
            set;
        }

        public LoginResponseCode Code{
            get;
            set;
        }
    }

    public enum LoginResponseCode
    {
        OK,USER_NOT_FOUND,PASSWORD_EXPIRED,SYSTEM_ERROR
    }
}