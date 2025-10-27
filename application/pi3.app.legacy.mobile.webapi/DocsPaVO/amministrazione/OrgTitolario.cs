// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace DocsPaVO.amministrazione
{
    /// <summary>
    /// Rappresenta la struttura di classificazione del titolario
    /// </summary>
    [Serializable()]
    [DataContract]
    public class OrgTitolario
    {
        /// <summary>
        /// 
        /// </summary>
        public OrgTitolario()
        { }

        /// <summary>
        /// id univoco del titolario
        /// </summary>
        [DataMember]
        public string ID { get; set; } = string.Empty;

        /// <summary>
        /// Codice del titolario
        /// </summary>
        [DataMember]
        public string Codice { get; set; } = string.Empty;

        /// <summary>
        /// Stato cui pu� venirsi a trovare una struttura di titolario
        /// </summary>
        [DataMember]
        public OrgStatiTitolarioEnum Stato { get; set; } = OrgStatiTitolarioEnum.InDefinizione;

        /// <summary>
        /// Note del titolario
        /// </summary>
        [DataMember]
        public string Commento { get; set; } = string.Empty;

        /// <summary>
        /// Data di attivazione del titolario
        /// </summary>
        [DataMember]
        public string DataAttivazione { get; set; } = string.Empty;

        /// <summary>
        /// Data di cessazione della validit� del titolario
        /// </summary>
        [DataMember]
        public string DataCessazione { get; set; } = string.Empty;

        /// <summary>
        /// Codice dell'amministrazione cui la gerarchia di classificazione fa parte
        /// </summary>
        [DataMember]
        public string CodiceAmministrazione { get; set; } = string.Empty;

        /// <summary>
        /// Descrizione del titolario con lo stato 
        /// </summary>
        [DataMember]
        public string Descrizione { get; set; } = string.Empty;

        /// <summary>
        /// Descrizione del titolario senza lo stato
        /// </summary>
        [DataMember]
        public string DescrizioneLite { get; set; } = string.Empty;

        /// <summary>
        /// Massimo livello di profondit�
        /// </summary>
        [DataMember]
        public string MaxLivTitolario { get; set; } = string.Empty;

        /// <summary>
        /// Etichetta titolario
        /// </summary>
        [DataMember]
        public string EtichettaTit { get; set; } = string.Empty;

        /// <summary>
        /// Etichetta livello 1 titolario
        /// </summary>
        [DataMember]
        public string EtichettaLiv1 { get; set; } = string.Empty;

        /// <summary>
        /// Etichetta livello 2 titolario
        /// </summary>
        [DataMember]
        public string EtichettaLiv2 { get; set; } = string.Empty;

        /// <summary>
        /// Etichetta livello 3 titolario
        /// </summary>
        [DataMember]
        public string EtichettaLiv3 { get; set; } = string.Empty;

        /// <summary>
        /// Etichetta livello 4 titolario
        /// </summary>
        [DataMember]
        public string EtichettaLiv4 { get; set; } = string.Empty;

        /// <summary>
        /// Etichetta livello 5 titolario
        /// </summary>
        [DataMember]
        public string EtichettaLiv5 { get; set; } = string.Empty;

        /// <summary>
        /// Etichetta livello 6 titolario
        /// </summary>
        [DataMember]
        public string EtichettaLiv6 { get; set; } = string.Empty;


        /// <summary>
        /// Creazione di una copia dell'oggetto titolario
        /// impostandone lo stato come "InDefinizione"
        /// </summary>
        /// <returns></returns>
        public OrgTitolario CopyInDefinizione()
        {
            OrgTitolario newTitolario = new OrgTitolario();

            newTitolario.ID = string.Empty;
            newTitolario.Codice = this.Codice;
            newTitolario.CodiceAmministrazione = this.CodiceAmministrazione;
            newTitolario.Commento = this.Commento;
            newTitolario.Descrizione = this.Descrizione;
            newTitolario.Stato = OrgStatiTitolarioEnum.InDefinizione;
            newTitolario.DataAttivazione = DateTime.Now.ToString("dd/MM/yyyy");
            newTitolario.DataCessazione = string.Empty;
            newTitolario.MaxLivTitolario = this.MaxLivTitolario;

            return newTitolario;
        }

        /// <summary>
        /// Funzione di utilit� per la trascodifica dello stato del titolario da stringa
        /// </summary>
        /// <param name="stato"></param>
        public void SetStatoTitolario(string stato)
        {
            if (stato == "A")
                this.Stato = OrgStatiTitolarioEnum.Attivo;
            else if (stato == "C")
                this.Stato = OrgStatiTitolarioEnum.Chiuso;
            else if (stato == "D")
                this.Stato = OrgStatiTitolarioEnum.InDefinizione;
            else
                throw new ApplicationException(string.Format("Il valore specificato '{0}' non rappresenta uno stato valido per il titolario"));
        }

        /// <summary>
        /// Rappresentazione stringa dell'oggetto Titolario
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string retValue = string.Empty;

            if (Stato == OrgStatiTitolarioEnum.Chiuso)
            {
                DateTime dta;

                string dataAttivazione = string.Empty;
                if (DateTime.TryParse(this.DataAttivazione, out dta))
                    dataAttivazione = dta.ToString("dd-MM-yyyy");

                string dataCessazione = string.Empty;
                if (DateTime.TryParse(this.DataCessazione, out dta))
                    dataCessazione = dta.ToString("dd-MM-yyyy");

                retValue = string.Format("Titolario in vigore dal {0} al {1}", dataAttivazione, dataCessazione);
            }
            else if (Stato == OrgStatiTitolarioEnum.Attivo)
            {
                retValue = "Titolario attivo";
            }
            else if (Stato == OrgStatiTitolarioEnum.InDefinizione)
            {
                retValue = "Titolario in definizione";
            }
            else
                retValue = base.ToString();

            return string.Format("{0} ({1})", retValue, this.ID);
        }
    }

    /// <summary>
    /// Stati della struttura del titolario
    /// </summary>
    public enum OrgStatiTitolarioEnum
    {
        Attivo,
        Chiuso,
        InDefinizione
    }
}