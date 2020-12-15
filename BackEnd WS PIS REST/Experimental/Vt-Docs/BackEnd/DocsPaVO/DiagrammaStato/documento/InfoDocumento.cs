using System;
using System.Collections;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace DocsPaVO.documento
{
    /// <summary>
    /// </summary>
    [Serializable()]
    public class InfoDocumento
    {
        /// <summary>
        /// id univoco del documento
        /// </summary>
        public string idProfile;
        /// <summary>
        /// id univoco del documento
        /// </summary>
        /// <remarks>Profile.docnumber; può coincidere con la idprofile</remarks>
        public string docNumber;
        /// <summary>
        /// numero protocollo
        /// </summary>
        public string numProt;
        /// <summary>
        /// oggetto del documento
        /// </summary>
        public string oggetto;
        /// <summary>
        /// data arrivo del documento
        /// </summary>
        public string dataApertura;
        /// <summary>
        /// tipo documento
        /// </summary>
        /// <remarks>[A=arrivo;P=Partenza;G=Grigio;I=Interno;R=stamparegistro]</remarks>
        public string tipoProto;
        /// <summary>
        /// codice del registro del documento
        /// </summary>
        /// <remarks>valido solo se protocollo;predisposto</remarks>
        public string codRegistro;
        /// <summary>
        /// id univoco del registro AOO del documento
        /// </summary>
        public string idRegistro;
        /// <summary>
        /// segnatura del protocollo
        /// </summary>
        public string segnatura;
        /// <summary>
        /// se 1 indica che il documento predisposto è ancora da protocollare
        /// </summary>
        public string daProtocollare;
        /// <summary>
        /// indica se un documento è in evidenza
        /// </summary>
        public string evidenza;
        /// <summary>
        /// data di annullamento del protocollo
        /// </summary>
        public string dataAnnullamento;
        public string mittDoc;
        /// <summary>
        /// Se 1, indica che l'ultima versione del documento ha un file immagine
        /// </summary>
        public string acquisitaImmagine;
        /// <summary>
        /// inidca se il documento è privato
        /// </summary>
        public string privato;
        /// <summary>
        /// indica se un documento è personale
        /// </summary>
        public string personale;
        public string noteCestino;
        /// <summary>
        /// autore del documento (userId)
        /// </summary>
        public string autore;
        /// <summary>
        /// indica se il documento è in cestino
        /// </summary>
        public string inCestino;
        /// <summary>
        /// indica se il documento è nella area di lavoro del ruolo/utente visualizzatore
        /// </summary>
        public string inADL;
        /// <summary>
        /// id univoco del tipo documento profilato
        /// </summary>
        public string idTipoAtto;
        /// <summary>
        /// Descrizione del tipo documento profilato
        /// </summary>
        public string tipoAtto;
        public string isRimovibile;
        /// <summary>
        /// indica se il documento è nell'area di conservazione del ruolo/utente visualizzatore
        /// </summary>
        public string inConservazione;
        /// <summary>
        /// indica se il documento in deposito
        /// </summary>
        public string inArchivio;
        /// <summary>
        /// opzionale
        /// </summary>
        public string numSerie;
        /// <summary>
        /// L'ultima nota inserita con visibilità generale
        /// </summary>
        public string ultimaNota = string.Empty;

       
        /// <summary>
        /// array delle descrizioni dei mittenti/destinarati associati al documento
        /// </summary>
        [XmlArray()]
        [XmlArrayItem(typeof(string))]
        public ArrayList mittDest; // array list di stringhe e non di oggetti corrispondenti

        /// <summary>
        /// Data di archiviazione del documento in fascicolo cartaceo
        /// </summary>
        public string dataArchiviazione;

        /// <summary>
        /// Se true, indica che il documento rappresenta un allegato
        /// </summary>
        public bool allegato = false;

        /// <summary>
        /// Il protocollo titolario del documento
        /// </summary>
        public string protocolloTitolario = string.Empty;

        public string cha_firmato;

        //MODIFICA DEL 20/05/2009 
        public string contatore;
        //FINE MODIFICA DEL 20/05/2009

        /// <summary>
        /// Codice univoco dell'applicazione di appartenenza
        /// </summary>
        public string codiceApplicazione = string.Empty;

        public List<String> Mittenti { get; set; }
        public List<String> Destinatari { get; set; }

        public string isCatenaTrasversale = string.Empty;
        /// <summary>
        /// </summary>
        public InfoDocumento()
        {
            this.mittDest = new ArrayList();
        }



        [XmlIgnore()]
        public System.Collections.Generic.List<CampoProfiloInfoDocumento> CampiProfilati = new System.Collections.Generic.List<CampoProfiloInfoDocumento>();

        [Serializable()]
        public class CampoProfiloInfoDocumento
        {
            public string NomeCampo { get; set; }
            public string ValoreCampo { get; set; }
        }

        /// <summary>
        /// </summary>
        /// <param name="schedaDocumento"></param>
        public InfoDocumento(SchedaDocumento schedaDocumento)
        {
            this.mittDest = new ArrayList();
            this.idProfile = schedaDocumento.systemId;
            this.oggetto = schedaDocumento.oggetto.descrizione;
            this.docNumber = schedaDocumento.docNumber;
            this.evidenza = schedaDocumento.evidenza;
            this.privato = schedaDocumento.privato;
            this.inCestino = schedaDocumento.inCestino;
            this.allegato = (schedaDocumento.documentoPrincipale != null);
            this.tipoProto = schedaDocumento.tipoProto;
            this.dataApertura = schedaDocumento.dataCreazione;

            if (schedaDocumento.registro != null)
            {
                this.codRegistro = schedaDocumento.registro.codice;
                this.idRegistro = schedaDocumento.registro.systemId;
            }

            if (schedaDocumento.protocollo != null)
            {
                this.numProt = schedaDocumento.protocollo.numero;
                this.daProtocollare = schedaDocumento.protocollo.daProtocollare;
                this.dataApertura = schedaDocumento.protocollo.dataProtocollazione;
                this.segnatura = schedaDocumento.protocollo.segnatura;

                if (schedaDocumento.protocollo.GetType().Equals(typeof(DocsPaVO.documento.ProtocolloEntrata)))
                {
                    DocsPaVO.documento.ProtocolloEntrata pe = (DocsPaVO.documento.ProtocolloEntrata)schedaDocumento.protocollo;
                    this.mittDest.Add(pe.mittente.descrizione);
                }
                else if (schedaDocumento.protocollo.GetType().Equals(typeof(DocsPaVO.documento.ProtocolloUscita)))
                {
                    DocsPaVO.documento.ProtocolloUscita pu = (DocsPaVO.documento.ProtocolloUscita)schedaDocumento.protocollo;

                    for (int i = 0; i < pu.destinatari.Count; i++)
                    {
                        this.mittDest.Add(((DocsPaVO.utente.Corrispondente)pu.destinatari[i]).descrizione);
                    }
                }
            }
            if (schedaDocumento.tipologiaAtto != null)
            {
                this.idTipoAtto = schedaDocumento.tipologiaAtto.systemId;
                this.tipoAtto = schedaDocumento.tipologiaAtto.descrizione;
            }

            this.codiceApplicazione = schedaDocumento.codiceApplicazione;
        }
    }
}
