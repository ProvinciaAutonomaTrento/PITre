using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Domain.SignBook
{
    /// <summary>
    /// IstanzaPassoFirma
    /// </summary>
    [DataContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public class SignStepInstance
    {
        public SignStepInstance() { }

        public SignStepInstance(DocsPaVO.LibroFirma.IstanzaPassoDiFirma input)
        {
            if (input != null)
            {
                this.IdStepInstance = input.idIstanzaPasso;
                this.IdProcessInstance = input.idIstanzaProcesso;
                this.IdStep = input.idPasso;
                if (input.statoPasso != null)
                {
                    switch (input.statoPasso)
                    {
                        case DocsPaVO.LibroFirma.TipoStatoPasso.NEW: this.StepState = "NEW"; break;
                        case DocsPaVO.LibroFirma.TipoStatoPasso.CLOSE: this.StepState = "CLOSE"; break;
                        case DocsPaVO.LibroFirma.TipoStatoPasso.CUT: this.StepState = "CUT"; break;
                        case DocsPaVO.LibroFirma.TipoStatoPasso.LOOK: this.StepState = "LOOK"; break;
                        case DocsPaVO.LibroFirma.TipoStatoPasso.STUCK: this.StepState = "STUCK"; break;
                    }
                }
                this.StepStateDescription = input.descrizioneStatoPasso;
                this.SignatureType = input.TipoFirma;
                this.EventTypeId = input.IdTipoEvento;
                this.EventTypeCode = input.CodiceTipoEvento;
                this.ExecutionDate = input.dataEsecuzione;
                this.ExpirationDate = input.dataScadenza;
                this.RefusalReason = input.motivoRespingimento;

                if (input.RuoloCoinvolto != null)
                {
                    this.InvolvedRole = VtDocsWS.WebServices.Utils.getRole(input.RuoloCoinvolto);
                }
                if (input.UtenteCoinvolto != null)
                {
                    this.InvolvedUser = VtDocsWS.WebServices.Utils.getUser(input.UtenteCoinvolto);
                }

                this.ExecutedNotificationId = input.idNotificaEffettuata;
                this.Note = input.Note;
                this.SequenceNumber = input.numeroSequenza;
            }
        }

        /// <summary>
        /// System id dell'istanza di passo
        /// </summary>
        [DataMember]
        public string IdStepInstance
        {
            get;
            set;
        }
        /// <summary>
        /// System id dell'istanza di processo di firma
        /// </summary>
        [DataMember]
        public string IdProcessInstance
        {
            get;
            set;
        }

        /// <summary>
        /// System id del passo
        /// </summary>
        [DataMember]
        public string IdStep
        {
            get;
            set;
        }

        /// <summary>
        /// System id dello stato passo
        /// </summary>
        [DataMember]
        public string StepState
        {
            get;
            set;
        }

        /// <summary>
        /// Descrizione dello stato passo
        /// </summary>
        [DataMember]
        public string StepStateDescription
        {
            get;
            set;
        }

        /// <summary>
        /// Tipo firma (Elettronica/Digitale/Pades)
        /// </summary>
        [DataMember]
        public string SignatureType
        {
            get;
            set;
        }

        /// <summary>
        /// system id del tipo evento associato al passo
        /// </summary>
        [DataMember]
        public string EventTypeId
        {
            get;
            set;
        }

        /// <summary>
        /// Codice tipo evento presente anche in DPA_LOG
        /// </summary>
        [DataMember]
        public string EventTypeCode
        {
            get;
            set;
        }

        /// <summary>
        /// Data in cui è stato eseguito il passo
        /// </summary>
        [DataMember]
        public string ExecutionDate
        {
            get;
            set;
        }

        /// <summary>
        /// Data entro cui poter eseguito il passo
        /// </summary>
        [DataMember]
        public string ExpirationDate
        {
            get;
            set;
        }
        /// <summary>
        /// Descriznoe motivo del respingimento
        /// </summary>
        [DataMember]
        public string RefusalReason
        {
            get;
            set;
        }

        /// <summary>
        /// Ruolo titolare del passo
        /// </summary>
        [DataMember]
        public Domain.Role InvolvedRole
        {
            get;
            set;
        }

        /// <summary>
        /// Utente titolare del passo
        /// </summary>
        [DataMember]
        public Domain.User InvolvedUser
        {
            get;
            set;
        }

        /// <summary>
        /// System id della notifica relativa all'avanzamento di passo
        /// </summary>
        [DataMember]
        public string ExecutedNotificationId
        {
            get;
            set;
        }

        /// <summary>
        /// Numero relativo all'ordine del passo
        /// </summary>
        [DataMember]
        public int SequenceNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Note relative al passo
        /// </summary>
        [DataMember]
        public string Note
        {
            get;
            set;
        }
    }
}