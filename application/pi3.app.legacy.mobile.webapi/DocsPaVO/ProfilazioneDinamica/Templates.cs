// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
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
    [SoapInclude(typeof(DocsPaVO.ProfilazioneDinamica.OggettoCustom))]
    [SoapInclude(typeof(DocsPaVO.ProfilazioneDinamica.StoricoProfilatiOldValue))]
    //[Serializable()]
    [DataContract]
    public class Templates
	{
        [DataMember]
        public int SYSTEM_ID { get; set; }
        [DataMember]
        public string DESCRIZIONE { get; set; } = string.Empty;
        [DataMember]
        public string DOC_NUMBER { get; set; } = string.Empty;
        [DataMember]
        public string ID_PROJECT { get; set; } = string.Empty;
        [DataMember]
        public string ABILITATO_SI_NO { get; set; } = string.Empty;
        [DataMember]
        public string IN_ESERCIZIO { get; set; } = string.Empty;
        [DataMember]
        public string NUM_MESI_CONSERVAZIONE { get; set; } = "0";
        [DataMember]
        public char IS_TYPE_INSTANCE { get; set; } = '0';

        [DataMember]
        public string INVIO_CONSERVAZIONE { get; set; } = "0";

        //[DataMember]
        //[XmlArray()]
        //[XmlArrayItem(typeof(DocsPaVO.ProfilazioneDinamica.OggettoCustom))]
        [DataMember]
        public List<DocsPaVO.ProfilazioneDinamica.OggettoCustom> ELENCO_OGGETTI { get; set; } = new List<DocsPaVO.ProfilazioneDinamica.OggettoCustom>();

        /// <summary>
        /// Indica l'operatore logico per la ricerca degli oggetti
        /// </summary>
        [DataMember]
        public OperatoriRicercaOggettiCustomEnum OPERATORE_RICERCA_OGGETTI { get; set; } = OperatoriRicercaOggettiCustomEnum.And;

        [DataMember]
        public string PATH_MODELLO_1 { get; set; } = string.Empty;
        [DataMember]
        public string PATH_MODELLO_2 { get; set; } = string.Empty;
        [DataMember]
        public string PATH_MODELLO_STAMPA_UNIONE { get; set; } = string.Empty;
        [DataMember]
        public string PATH_ALLEGATO_1 { get; set; } = string.Empty;
        [DataMember]
        public string PATH_MODELLO_EXCEL { get; set; } = string.Empty;

        [DataMember]
        public string PATH_XSD_ASSOCIATO { get; set; } = string.Empty;

        [DataMember]
        public string CHA_ASSOC_MANUALE { get; set; } = "0";
        [DataMember]
        public string ID_TIPO_ATTO { get; set; } = string.Empty;
        [DataMember]
        public string ID_TIPO_FASC { get; set; } = string.Empty;
        [DataMember]
        public string SCADENZA { get; set; } = string.Empty;
        [DataMember]
        public string PRE_SCADENZA { get; set; } = string.Empty;
        [DataMember]
        public string PRIVATO { get; set; } = string.Empty;
        [DataMember]
        public string IPER_FASC_DOC { get; set; } = string.Empty;
        [DataMember]
        public string ID_AMMINISTRAZIONE { get; set; } = string.Empty;
        [DataMember]
        public string CODICE_MODELLO_TRASM { get; set; } = string.Empty;
        [DataMember]
        public string CODICE_CLASSIFICA { get; set; } = string.Empty;
        [DataMember]
        public string PATH_MODELLO_1_EXT { get; set; }
        [DataMember]
        public string PATH_MODELLO_2_EXT { get; set; }
        //[DataMember]
        //[XmlArrayItem(typeof(DocsPaVO.ProfilazioneDinamica.StoricoProfilatiOldValue))]
        [DataMember]
        public List<DocsPaVO.ProfilazioneDinamica.StoricoProfilatiOldValue> OLD_OGG_CUSTOM = new List<DocsPaVO.ProfilazioneDinamica.StoricoProfilatiOldValue>();
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

        [DataMember]
        public string ID_TEMPLATE_STRUTTURA { get; set; } = string.Empty;

        [DataMember]
        public string ID_CONTESTO_PROCEDURALE { get; set; } = string.Empty;
    }
}
