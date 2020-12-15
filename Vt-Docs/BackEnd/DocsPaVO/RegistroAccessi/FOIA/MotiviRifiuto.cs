using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace DocsPaVO.RegistroAccessi.FOIA
{
    [Serializable]
    public class MotiviRifiuto
    {
        [XmlElement(ElementName = "sicurezza_pubblica_e_ordine_pubblico")]
        public Boolean SicurezzaPubblica { get; set; }

        [XmlElement(ElementName = "sicurezza_nazionale")]
        public Boolean SicurezzaNazionale { get; set; }

        [XmlElement(ElementName = "difesa_e_questioni_militari")]
        public Boolean Difesa { get; set; }

        [XmlElement(ElementName = "relazioni_internazionali")]
        public Boolean RelazioniInternazionali { get; set; }

        [XmlElement(ElementName = "politica_e_stabilita_finanziaria_ed_economica_dello_stato")]
        public Boolean Politica { get; set; }

        [XmlElement(ElementName = "conduzione_di_indagini_sui_reati_e_loro_perseguimento")]
        public Boolean ConduzioneIndaginiReati { get; set; }

        [XmlElement(ElementName = "regolare_svolgimento_di_attivita_ispettive")]
        public Boolean AttivitaIspettive { get; set; }

        [XmlElement(ElementName = "protezione_dei_dati_personali")]
        public Boolean ProtezioneDatiPersonali { get; set; }

        [XmlElement(ElementName = "liberta_e_segretezza_della_corrispondenza")]
        public Boolean LibertaSegretezzaCorrispondenza { get; set; }

        [XmlElement(ElementName = "interessi_economici_e_commerciali")]
        public Boolean InteressiEconomiciCommerciali { get; set; }

        [XmlElement(ElementName = "segreto_di_stato")]
        public Boolean SegretoDiStato { get; set; }

        [XmlElement(ElementName = "altri_divieti_di_accesso_o_divulgazione")]
        public Boolean AltriMotivi { get; set; }

        [XmlElement(ElementName = "richiesta_manifestamente_onerosa")]
        public Boolean RichiestaOnerosa { get; set; }

        [XmlElement(ElementName = "informazione_non_esistente_o_non_in_possesso")]
        public Boolean InformazioneNonEsistente { get; set; }
    }
}
