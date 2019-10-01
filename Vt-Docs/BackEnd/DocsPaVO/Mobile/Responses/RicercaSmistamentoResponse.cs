using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Responses
{
    public class RicercaSmistamentoResponse
    {
        public RicercaSmistamentoResponseCode Code
        {
            get;
            set;
        }

        public List<RicercaSmistamentoElement> Elements
        {
            get;
            set;
        }

        public static RicercaSmistamentoResponse ErrorResponse
        {
            get
            {
                RicercaSmistamentoResponse resp = new RicercaSmistamentoResponse();
                resp.Code = RicercaSmistamentoResponseCode.SYSTEM_ERROR;
                return resp;
            }
        }

    }

    public class RicercaSmistamentoElement
    {
        public string IdUtente
        {
            get;
            set;
        }

        public string IdUO
        {
            get;
            set;
        }

        public string IdRuolo
        {
            get;
            set;
        }

        public string DescrizioneUtente
        {
            get;
            set;
        }

        public string DescrizioneRuolo
        {
            get;
            set;
        }

        public string DescrizioneUO
        {
            get;
            set;
        }

        public SmistamentoNodeType Type
        {
            get;
            set;
        }

    }

    public enum RicercaSmistamentoResponseCode
    {
        OK, SYSTEM_ERROR
    }
}
