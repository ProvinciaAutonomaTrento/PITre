using System;
using System.Collections.Generic;
using System.Text;

namespace DocsPaVO.amministrazione
{
    /// <summary>
    /// Rappresenta la struttura di classificazione del titolario
    /// </summary>
    [Serializable()]
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
        public string ID = string.Empty;

        /// <summary>
        /// Codice del titolario
        /// </summary>
        public string Codice = string.Empty;

        /// <summary>
        /// Stato cui può venirsi a trovare una struttura di titolario
        /// </summary>
        public OrgStatiTitolarioEnum Stato = OrgStatiTitolarioEnum.InDefinizione;

        /// <summary>
        /// Note del titolario
        /// </summary>
        public string Commento = string.Empty;

        /// <summary>
        /// Data di attivazione del titolario
        /// </summary>
        public string DataAttivazione = string.Empty;

        /// <summary>
        /// Data di cessazione della validità del titolario
        /// </summary>
        public string DataCessazione = string.Empty;

        /// <summary>
        /// Codice dell'amministrazione cui la gerarchia di classificazione fa parte
        /// </summary>
        public string CodiceAmministrazione = string.Empty;

        /// <summary>
        /// Descrizione del titolario con lo stato 
        /// </summary>
        public string Descrizione = string.Empty;

        /// <summary>
        /// Descrizione del titolario senza lo stato
        /// </summary>
        public string DescrizioneLite = string.Empty;

        /// <summary>
        /// Massimo livello di profondità
        /// </summary>
        public string MaxLivTitolario = string.Empty;

        /// <summary>
        /// Etichetta titolario
        /// </summary>
        public string EtichettaTit = string.Empty;

        /// <summary>
        /// Etichetta livello 1 titolario
        /// </summary>
        public string EtichettaLiv1 = string.Empty;

        /// <summary>
        /// Etichetta livello 2 titolario
        /// </summary>
        public string EtichettaLiv2 = string.Empty;

        /// <summary>
        /// Etichetta livello 3 titolario
        /// </summary>
        public string EtichettaLiv3 = string.Empty;

        /// <summary>
        /// Etichetta livello 4 titolario
        /// </summary>
        public string EtichettaLiv4 = string.Empty;

        /// <summary>
        /// Etichetta livello 5 titolario
        /// </summary>
        public string EtichettaLiv5 = string.Empty;

        /// <summary>
        /// Etichetta livello 6 titolario
        /// </summary>
        public string EtichettaLiv6 = string.Empty;


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
        /// Funzione di utilità per la trascodifica dello stato del titolario da stringa
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