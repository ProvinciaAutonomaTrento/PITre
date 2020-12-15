using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocsPaVO.filtri;

namespace DocsPaWS.Sanita.VO.Requests
{
    public class InteropCreazioneDocRequest
    {
        public string IdRegistro
        {
            get;
            set;
        }

        public UserInfoVO InfoUtente
        {
            get;
            set;
        }

        public RuoloInfoVO Ruolo
        {
            get;
            set;
        }

        public string ServerName
        {
            get;
            set;
        }

        public StatusTemplateVO StatusTemplate
        {
            get;
            set;
        }

        public List<FiltroMail> FiltriMail
        {
            get;
            set;
        }

    }
}