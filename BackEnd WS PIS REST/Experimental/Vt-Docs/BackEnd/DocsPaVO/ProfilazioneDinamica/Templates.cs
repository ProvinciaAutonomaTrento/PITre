using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.ProfilazioneDinamica
{
    /// <summary>
    /// Operatori logici utilizzabili per la ricerca degli oggetti custom nel template di profilazione
    /// </summary>
    public enum OperatoriRicercaOggettiCustomEnum
    {
        And,
        Or
    }

    [XmlInclude(typeof(DocsPaVO.ProfilazioneDinamica.OggettoCustom))]
    [Serializable()]
    public class Templates
	{
        public int SYSTEM_ID;
        public string DESCRIZIONE = string.Empty;
        public string DOC_NUMBER = string.Empty;
        public string ID_PROJECT = string.Empty;
        public string ABILITATO_SI_NO = string.Empty;
        public string IN_ESERCIZIO = string.Empty;
        public string NUM_MESI_CONSERVAZIONE = "0";
        public char IS_TYPE_INSTANCE = '0';

        public string INVIO_CONSERVAZIONE = "0";

        [XmlArray()]
        [XmlArrayItem(typeof(DocsPaVO.ProfilazioneDinamica.OggettoCustom))]
        public ArrayList ELENCO_OGGETTI = new ArrayList();

        /// <summary>
        /// Indica l'operatore logico per la ricerca degli oggetti 
        /// </summary>
        public OperatoriRicercaOggettiCustomEnum OPERATORE_RICERCA_OGGETTI = OperatoriRicercaOggettiCustomEnum.And;

        public string PATH_MODELLO_1 = string.Empty;
        public string PATH_MODELLO_2 = string.Empty;
        public string PATH_MODELLO_STAMPA_UNIONE = string.Empty;
        public string PATH_ALLEGATO_1 = string.Empty;
        public string PATH_MODELLO_EXCEL = string.Empty;

        public string PATH_XSD_ASSOCIATO = string.Empty;

        public string CHA_ASSOC_MANUALE = "0";
        public string ID_TIPO_ATTO = string.Empty;
        public string ID_TIPO_FASC = string.Empty;
        public string SCADENZA = string.Empty;
        public string PRE_SCADENZA = string.Empty;
        public string PRIVATO = string.Empty;
        public string IPER_FASC_DOC = string.Empty;
        public string ID_AMMINISTRAZIONE = string.Empty;
        public string CODICE_MODELLO_TRASM = string.Empty;
        public string CODICE_CLASSIFICA = string.Empty;
        public string PATH_MODELLO_1_EXT { get; set; }
        public string PATH_MODELLO_2_EXT { get; set; }
        [XmlArrayItem(typeof(DocsPaVO.ProfilazioneDinamica.StoricoProfilatiOldValue))]
        public ArrayList OLD_OGG_CUSTOM = new ArrayList();		
		public Templates(){}

		public void gestisciCaratteriSpeciali()
		{
		    DESCRIZIONE = DESCRIZIONE.Replace("'", "''");
		}

        public OggettoCustom getOggettoCustom(string descr)
        {
            foreach (object temp in ELENCO_OGGETTI)
            {
                OggettoCustom oggCust = (OggettoCustom)temp;
                if (descr.ToUpper().Equals(oggCust.DESCRIZIONE.ToUpper())) return oggCust;
            }
            return null;
        }

        public string ID_TEMPLATE_STRUTTURA = string.Empty;

        public string ID_CONTESTO_PROCEDURALE = string.Empty;
    }
}
