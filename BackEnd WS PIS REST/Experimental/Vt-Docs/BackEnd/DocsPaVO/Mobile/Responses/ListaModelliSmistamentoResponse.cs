using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Responses
{
    public class ListaModelliSmistamentoResponse
    {
        public List<ModelloSmistamento> Modelli
        {
            get;
            set;
        }

        public ListaModelliSmistamentoResponseCode Code
        {
            get;
            set;
        }

        public static ListaModelliSmistamentoResponse ErrorResponse
        {
            get
            {
                ListaModelliSmistamentoResponse res = new ListaModelliSmistamentoResponse();
                res.Code = ListaModelliSmistamentoResponseCode.SYSTEM_ERROR;
                return res;
            }
        }
    }

    public enum ListaModelliSmistamentoResponseCode
    {
        OK, SYSTEM_ERROR
    }
}
