using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.utente;
using BusinessLogic.Import.Template.Builders;

namespace BusinessLogic.Import
{
    public class CorrispondenteInfo
    {
        private Corrispondente _corrispondente;
        private Corrispondente _indirizzo;
        private TemplateBuilder _builder;

        public CorrispondenteInfo(Corrispondente corrispondente, Corrispondente indirizzo,TemplateBuilder builder)
        {
            _corrispondente = corrispondente;
            _indirizzo = indirizzo;
            _builder = builder;
        }

        public string Descrizione
        {
            get
            {
                return _corrispondente.descrizione;
            }
        }

        public string Indirizzo
        {
            get
            {
                string res = Descrizione;
                if (_indirizzo == null) return res;
                if (!string.IsNullOrEmpty(_indirizzo.indirizzo))
                {
                    res += _builder.NewLine() + _indirizzo.indirizzo;
                }
                if (!string.IsNullOrEmpty(_indirizzo.cap) || !string.IsNullOrEmpty(_indirizzo.citta) || !string.IsNullOrEmpty(_indirizzo.localita))
                {
                    res += _builder.NewLine() + _indirizzo.cap;
                    if(!string.IsNullOrEmpty(_indirizzo.cap) && !string.IsNullOrEmpty(_indirizzo.citta)) res+="-";
                    res += _indirizzo.citta;
                    if (!string.IsNullOrEmpty(_indirizzo.citta) && !string.IsNullOrEmpty(_indirizzo.localita)) res += "-";
                    res += _indirizzo.localita;
                }
                return res;
            }
        }

        public string Telefono
        {
            get
            {
                string res = Descrizione;
                if(!string.IsNullOrEmpty(TelefonoNoDescr)){
                    res+=_builder.NewLine()+TelefonoNoDescr;
                }
                return res;
            }
        }

        private string TelefonoNoDescr
        {
            get
            {
                string res = string.Empty;
                if (_indirizzo == null) return res;
                if (!string.IsNullOrEmpty(_indirizzo.telefono1) || !string.IsNullOrEmpty(_indirizzo.telefono2))
                {
                    res += _indirizzo.telefono1;
                    if (!string.IsNullOrEmpty(_indirizzo.telefono1) && !string.IsNullOrEmpty(_indirizzo.telefono2)) res += "-";
                    res += _indirizzo.telefono2;
                }
                return res;
            }
        }

        public string IndirizzoTelefono
        {
            get
            {
                string res = Indirizzo;
                if(!string.IsNullOrEmpty(TelefonoNoDescr)){
                    res+=_builder.NewLine() + TelefonoNoDescr;
                }
                return res;
            }
        }
    }
}
