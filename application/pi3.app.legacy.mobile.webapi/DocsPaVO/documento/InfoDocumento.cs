// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DocsPaVO.documento
{
    /// <summary>
    /// </summary>
    [Serializable()]
    [DataContract]
    public class InfoDocumento
    {
        /// <summary>
        /// id univoco del documento
        /// </summary>
        [DataMember]
        public string idProfile { get; set; }
        /// <summary>
        /// id univoco del documento
        /// </summary>
        /// <remarks>Profile.docnumber; pu� coincidere con la idprofile</remarks>
        [DataMember]
        public string docNumber { get; set; }
        /// <summary>
        /// numero protocollo
        /// </summary>
        [DataMember]
        public string numProt { get; set; }
        /// <summary>
        /// oggetto del documento
        /// </summary>
        [DataMember]
        public string oggetto { get; set; }
        /// <summary>
        /// data arrivo del documento
        /// </summary>
        [DataMember]
        public string dataApertura { get; set; }
        /// <summary>
        /// tipo documento
        /// </summary>
        /// <remarks>[A=arrivo;P=Partenza;G=Grigio;I=Interno;R=stamparegistro]</remarks>
        [DataMember]
        public string tipoProto { get; set; }
        /// <summary>
        /// codice del registro del documento
        /// </summary>
        /// <remarks>valido solo se protocollo;predisposto</remarks>
        [DataMember]
        public string codRegistro { get; set; }
        /// <summary>
        /// id univoco del registro AOO del documento
        /// </summary>
        [DataMember]
        public string idRegistro { get; set; }
        /// <summary>
        /// segnatura del protocollo
        /// </summary>
        [DataMember]
        public string segnatura { get; set; }
        /// <summary>
        /// se 1 indica che il documento predisposto � ancora da protocollare
        /// </summary>
        [DataMember]
        public string daProtocollare { get; set; }
        /// <summary>
        /// indica se un documento � in evidenza
        /// </summary>
        [DataMember]
        public string evidenza { get; set; }
        /// <summary>
        /// data di annullamento del protocollo
        /// </summary>
        [DataMember]
        public string dataAnnullamento { get; set; }
        [DataMember]
        public string mittDoc { get; set; }
        /// <summary>
        /// Se 1, indica che l'ultima versione del documento ha un file immagine
        /// </summary>
        [DataMember]
        public string acquisitaImmagine { get; set; }
        /// <summary>
        /// inidca se il documento � privato
        /// </summary>
        [DataMember]
        public string privato { get; set; }
        /// <summary>
        /// indica se un documento � personale
        /// </summary>
        [DataMember]
        public string personale { get; set; }
        [DataMember]
        public string noteCestino { get; set; }
        /// <summary>
        /// autore del documento (userId)
        /// </summary>
        [DataMember]
        public string autore { get; set; }
        /// <summary>
        /// indica se il documento � in cestino
        /// </summary>
        [DataMember]
        public string inCestino { get; set; }
        /// <summary>
        /// indica se il documento � nella area di lavoro del ruolo/utente visualizzatore
        /// </summary>
        [DataMember]
        public string inADL { get; set; }
        /// <summary>
        /// id univoco del tipo documento profilato
        /// </summary>
        [DataMember]
        public string idTipoAtto { get; set; }
        /// <summary>
        /// Descrizione del tipo documento profilato
        /// </summary>
        [DataMember]
        public string tipoAtto { get; set; }
        [DataMember]
        public string isRimovibile { get; set; }
        /// <summary>
        /// indica se il documento � nell'area di conservazione del ruolo/utente visualizzatore
        /// </summary>
        [DataMember]
        public string inConservazione { get; set; }
        /// <summary>
        /// indica se il documento in deposito
        /// </summary>
        [DataMember]
        public string inArchivio { get; set; }
        /// <summary>
        /// opzionale
        /// </summary>
        [DataMember]
        public string numSerie { get; set; }
        /// <summary>
        /// L'ultima nota inserita con visibilit� generale
        /// </summary>
        [DataMember]
        public string ultimaNota { get; set; } = string.Empty;
       
        /// <summary>
        /// array delle descrizioni dei mittenti/destinarati associati al documento
        /// </summary>
        [XmlArray()]
        [XmlArrayItem(typeof(string))]
        [DataMember]
        public ArrayList mittDest { get; set; } // array list di stringhe e non di oggetti corrispondenti

        /// <summary>
        /// Data di archiviazione del documento in fascicolo cartaceo
        /// </summary>
        [DataMember]
        public string dataArchiviazione { get; set; }

        /// <summary>
        /// Se true, indica che il documento rappresenta un allegato
        /// </summary>
        [DataMember]
        public bool allegato { get; set; } = false;

        /// <summary>
        /// Il protocollo titolario del documento
        /// </summary>
        [DataMember]
        public string protocolloTitolario { get; set; } = string.Empty;

        [DataMember]
        public string cha_firmato { get; set; }

        //MODIFICA DEL 20/05/2009 
        [DataMember]
        public string contatore { get; set; }
        //FINE MODIFICA DEL 20/05/2009

        /// <summary>
        /// Codice univoco dell'applicazione di appartenenza
        /// </summary>
        [DataMember]
        public string codiceApplicazione { get; set; } = string.Empty;

        [DataMember]
        public List<String> Mittenti { get; set; }
        [DataMember]
        public List<String> Destinatari { get; set; }

        [DataMember]
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
