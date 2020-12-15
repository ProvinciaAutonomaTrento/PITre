using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Spedizione
{
    /// <summary>
    /// Classe per report gestione documenti spediti
    /// </summary>
    [Serializable()]
    public class InfoDocumentoSpedito
    {

        public enum TipoInfoSpedizione { Errore, Warning, Effettuato };


        /// <summary>
        /// Tipo alert documento spedito
        /// </summary>
        public TipoInfoSpedizione InfoSpedizione;

        /// <summary>
        /// IDDocumento
        /// </summary>
        public string IDDocumento;

        /// <summary>
        /// Numero  Documento
        /// </summary>
        public string NumeroDocumento;

        /// <summary>
        /// Protocollo
        /// </summary>
        public string Protocollo;

        /// <summary>
        /// Descrizione documento spedito
        /// </summary>
        public string  DescrizioneDocumento;

        /// <summary>
        /// Lista Pedizioni per docmento
        /// </summary>
        public List<InfoSpedizione> Spedizioni;

    }

    public class InfoSpedizione
    { 

        public enum TipologiaAzione {Nessuna,Rispedire,Attendere,OK};
        public enum TipologiaStatoRicevuta {Nessuna, OK, KO, Attendere, AttendereCausaMezzo};

        /// <summary>
        /// NominativoDestinatario 
        /// </summary>
        public string NominativoDestinatario;

        /// <summary>
        /// Tipo Mezzo Spedizione 
        /// </summary>
        public string MezzoSpedizione;

        /// <summary>
        /// Tipo Destinatario: A se popolato, CC se null in DB
        /// </summary>
        public string TipoDestinatario;

        /// <summary>
        /// E-Mail del Destinatario
        /// </summary>
        public string EMailDestinatario;

        /// <summary>
        /// E-mail del mittente
        /// </summary>
        public string EMailMittente;

        /// <summary>
        /// Data di Spedizione
        /// </summary>
        public string DataSpedizione;

        /// <summary>
        /// Tipo Ricevuta - Accettazione
        /// </summary>
        public TipologiaStatoRicevuta TipoRicevuta_Accettazione;

        /// <summary>
        /// Tipo Ricevuta - Consegna
        /// </summary>
        public TipologiaStatoRicevuta TipoRicevuta_Consegna;

        /// <summary>
        /// Tipo Ricevuta - Conferma
        /// </summary>
        public TipologiaStatoRicevuta TipoRicevuta_Conferma;

        /// <summary>
        /// Quando è presente la conferma, scriviamo il protocollo destinatario in un tooltip.
        /// </summary>
        public string ProtocolloDestinatario;

        /// <summary>
        /// Data della protocollazione da parte del destinatario
        /// </summary>
        public string DataProtDest;

        /// <summary>
        /// Tipo Ricevuta - Annullamento
        /// </summary>
        public TipologiaStatoRicevuta TipoRicevuta_Annullamento;

        /// <summary>
        /// Quando è presente la ricevuta di annullamento ne scriviamo il motivo in un tooltip
        /// </summary>
        public string MotivoAnnullEccezione;

        /// <summary>
        /// Tipo Ricevuta - Eccezione
        /// </summary>
        public TipologiaStatoRicevuta TipoRicevuta_Eccezione;


        /// <summary>
        /// Azione / Info 
        /// </summary>
        public TipologiaAzione Azione_Info;

    }

    public class FiltriReportSpedizioni
    {
        public enum TipoFiltroData {ValoreSingolo,Intervallo,Oggi,SettimanaCorrente,MeseCorrente}
        public enum TipoVisibilitaDocumenti {AllDoc,AllDocByRuolo}
        public bool TipoRicevuta_Accettazione;
        public bool TipoRicevuta_AvvenutaConsegna;
        public bool TipoRicevuta_MancataAccettazione;
        public bool TipoRicevuta_MancataConsegna;
        public bool TipoRicevuta_ConfermaRicezione;
        public bool TipoRicevuta_AnnullamentoProtocollazione;
        public bool TipoRicevuta_Eccezione;
        public bool TipoRicevuta_ConErrori;
        public bool TipoRicevuta_EsitoOK;
        public bool TipoRicevuta_EsitoAttesa;
        public bool TipoRicevuta_EsitoKO;
        public TipoFiltroData FiltroData;
        public DateTime DataDa;
        public DateTime DataA;
        public TipoVisibilitaDocumenti VisibilitaDoc;
        public string CodRuolo;
        public string IdDocumento;
        public string IdRegMailMittente;
        public string MailMittente;
  
    }

}
