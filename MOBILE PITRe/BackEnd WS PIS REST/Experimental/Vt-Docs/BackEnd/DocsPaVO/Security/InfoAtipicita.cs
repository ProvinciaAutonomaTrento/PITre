using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Security
{
    [Serializable()]
    public class InfoAtipicita
    {
        /// <summary>
        /// DocNumber della PROFILE o System_Id della PROJECT
        /// </summary>
        public string IdDocFasc { get; set; }

        /// <summary>
        /// Identificativo del codice di atipicità del documento, stringa combinazione dei seguenti codici separati da "-"
        /// - T - Documento tipico
        /// - ARP - Atipicità ruolo proprietario
        /// - AGRP - Atipicità gerarchia ruolo proprietario
        /// - ARDT - Atipicità ruoli destinatari di trasmissioni
        /// - ARPF - Atipicità ruolo proprietario del fascicolo in cui è classifficato il documento
        /// - AGRPF - Atipicità gerarchia ruolo proprietario del fascicolo in cui è classificato il documento  
        /// - ARDTF - Atipicità ruoli destinatari di trasmissioni del fascicolo in cui è classificato il documento
        /// </summary>
        public string CodiceAtipicita { get; set; }

        /// <summary>
        /// Identificativo del tipo oggetto a cui si riferisce l'atipicità. Documento / Fascicolo
        /// </summary>
        public TipoOggettoAtipico TipoOggetto { get; set; }

        /// <summary>
        /// Descrizione della atipicità del documento o fascicolo.
        /// Questa proprietà viene costruita in fuzione del codice di atipicità
        /// </summary>
        public string DescrizioneAtipicita 
        {
            get
            {
                string descrizione = string.Empty;

                if (!string.IsNullOrEmpty(CodiceAtipicita))
                {
                    string[] listaCodiciAtipicita = CodiceAtipicita.Split('-');

                    foreach (string cod in listaCodiciAtipicita)
                    {
                        switch (cod)
                        {
                            case "T":
                                descrizione += "-Tipico</br>";
                                break;

                            case "AGRP":
                                descrizione += String.Format("-Visibilità atipica nella catena gerarchica del ruolo proprietario (esiste almeno un ruolo superiore al ruolo creatore che non ha visibilità sul {0})</br>", this.TipoOggetto == TipoOggettoAtipico.DOCUMENTO ? "documento" : "fascicolo");
                                break;

                            case "AGDT":
                                descrizione += String.Format("-Visibilità atipica nella catena gerarchica del ruolo destinatario di trasmissioni (esiste almeno un ruolo superiore ad uno dei ruoli destinatari di trasmissione che non ha visibilità sul {0})</br>", this.TipoOggetto == TipoOggettoAtipico.DOCUMENTO ? "documento" : "fascicolo");
                                break;

                            case "AFCD":
                                descrizione += "-Atipicità del fascicolo in cui è classificato il documento</br>";
                                break;

                            case "AGCV":
                                descrizione += String.Format("-Visibilità atipica nella catena gerarchica del ruolo destinatario di copia visibilità (esiste almeno un ruolo superiore al ruolo destinatario di copia di visibilità che non ha visibilità sul {0})</br>", this.TipoOggetto == TipoOggettoAtipico.DOCUMENTO ? "documento" : "fascicolo");
                                break;                            
                        }
                    }
                }

                return descrizione;                
            }

            set { }
        }

        //Tipologia di oggetto
        public enum TipoOggettoAtipico
        {
            DOCUMENTO = 0,
            FASCICOLO = 1,
        }
    }
}
