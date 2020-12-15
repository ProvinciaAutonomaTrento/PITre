using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Sanita.VO.Responses
{
    public class InteropCreazioneDocResponse
    {
        public InteropCreazioneDocResponseCode Code
        {
            get;
            set;
        }

        public string ErrorMessage
        {
            get;
            set;
        }
    }

       
    public enum InteropCreazioneDocResponseCode
    {
        OK,KO
    }

}