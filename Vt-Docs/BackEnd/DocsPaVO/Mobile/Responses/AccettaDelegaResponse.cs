using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Responses
{
    public class AccettaDelegaResponse
    {
        public AccettaDelegaResponseCode Code
        {
            get;
            set;
        }

        public UserInfo UserInfo
        {
            get;
            set;
        }

        public Delega DelegaAccettata
        {
            get;
            set;
        }

        public static AccettaDelegaResponse ErrorResponse
        {
            get
            {
                AccettaDelegaResponse resp = new AccettaDelegaResponse();
                resp.Code = AccettaDelegaResponseCode.SYSTEM_ERROR;
                return resp;
            }
        }

        public static AccettaDelegaResponse buildErrorInstance(DocsPaVO.utente.UserLogin.LoginResult input){
            AccettaDelegaResponse resp = new AccettaDelegaResponse();
            AccettaDelegaResponseCode code;
            if(input==DocsPaVO.utente.UserLogin.LoginResult.NO_RUOLI){
                code=AccettaDelegaResponseCode.NO_RUOLI;
            }
            else if (input == DocsPaVO.utente.UserLogin.LoginResult.USER_ALREADY_LOGGED_IN)
            {
                code = AccettaDelegaResponseCode.USER_ALREADY_LOGGED_IN;
            }
            else
            {
                code = AccettaDelegaResponseCode.SYSTEM_ERROR;
            }
            resp.Code=code;
            return resp;
        }
    }

    public enum AccettaDelegaResponseCode
    {
        OK,NO_RUOLI,USER_ALREADY_LOGGED_IN,SYSTEM_ERROR
    }
}
