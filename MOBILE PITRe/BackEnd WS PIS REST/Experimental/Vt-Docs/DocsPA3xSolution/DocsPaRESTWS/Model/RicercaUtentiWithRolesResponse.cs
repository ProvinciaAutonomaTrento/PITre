using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocsPaVO.Mobile;
namespace DocsPaRESTWS.Model
{
    public class RicercaUtentiWithRolesResponse
    {
        public RicercaUtentiWithRolesResponseCode Code { get; set; }

        public List<UserInfo> Risultati
        {
            get;
            set;
        }

        public static RicercaUtentiWithRolesResponse ErrorResponse
        {
            get
            {
                RicercaUtentiWithRolesResponse resp = new RicercaUtentiWithRolesResponse();
                resp.Code = RicercaUtentiWithRolesResponseCode.SYSTEM_ERROR;
                return resp;
            }
        }
    }

    public enum RicercaUtentiWithRolesResponseCode
    {
        OK,SYSTEM_ERROR
    }
    
}