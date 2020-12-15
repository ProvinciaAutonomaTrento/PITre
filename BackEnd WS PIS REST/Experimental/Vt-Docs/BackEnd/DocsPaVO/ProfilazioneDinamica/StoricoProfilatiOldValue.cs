using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DocsPaVO.ProfilazioneDinamica
{
    [Serializable()]
    public class StoricoProfilatiOldValue
    {
        private string id_template = string.Empty;
        private string id_doc_fasc = string.Empty;
        private string valore = string.Empty;
        private string tipo_ogg_custom = string.Empty;
        private string id_oggetto = string.Empty;
        private string id_people = string.Empty;
        private string id_ruolo_in_uo = string.Empty;

        public string IDTemplate
        {
            get { return id_template; }
            set { id_template = value; }
        }
        public string ID_Doc_Fasc
        {
            get { return id_doc_fasc; }
            set { id_doc_fasc = value; }
        }
        public string Valore
        {
            get { return valore; }
            set { valore = value; }
        }
        public string Tipo_Ogg_Custom
        {
            get { return tipo_ogg_custom; }
            set { tipo_ogg_custom = value; }
        }
        public string ID_Oggetto
        {
            get { return id_oggetto; }
            set { id_oggetto = value; }
        }
        public string ID_People
        {
            get { return id_people; }
            set { id_people = value; }
        }
        public string ID_Ruolo_In_UO
        {
            get { return id_ruolo_in_uo; }
            set { id_ruolo_in_uo = value; }
        }
        public StoricoProfilatiOldValue() { }
    }
}
