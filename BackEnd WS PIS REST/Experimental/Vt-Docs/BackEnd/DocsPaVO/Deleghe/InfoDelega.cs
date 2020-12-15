using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace DocsPaVO.Deleghe
{
    [XmlType("InfoDelega")]
    public class InfoDelega
    {
        public string id_delega = string.Empty;
        public string id_utente_delegato = string.Empty;
        public string cod_utente_delegato = string.Empty;
        public string id_ruolo_delegato = string.Empty;
        public string cod_ruolo_delegato = string.Empty;
        public string id_utente_delegante = string.Empty;
        public string cod_utente_delegante = string.Empty;
        public string id_ruolo_delegante = string.Empty;
        public string cod_ruolo_delegante = string.Empty;
        public string id_people_corr_globali = string.Empty;
        public string id_uo_delegato = string.Empty;
        public string dataDecorrenza = string.Empty;
        public string dataScadenza = string.Empty;
        public string inEsercizio = string.Empty;
        public string utDelegatoDismesso = "0";
        public string utDeleganteDismesso = "0";
        public string stato = string.Empty;
        public string codiceDelegante = string.Empty;
    }
}
