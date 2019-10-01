using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.InstanceAccess.Metadata
{
    public partial class Instance
    {
        private string idIstanza;
        private string descrizione;
        private Richiedente richiedente;
        private string dataRichiesta;
        private string dataChiusura;
        private ProtocolloRichiesto protocolloRichiesto;
        private string note;

        public string IdIstanza
        {
            get
            {
                return idIstanza;
            }

            set
            {
                idIstanza = value;
            }
        }

        public string Descrizione
        {
            get
            {
                return descrizione;
            }
            set
            {
                descrizione = value;
            }
        }

        public string DataRichiesta
        {
            get
            {
                return dataRichiesta;
            }
            set
            {
                dataRichiesta = value;
            }
        }

        public string DataChiusura
        {
            get
            {
                return dataChiusura;
            }
            set
            {
                dataChiusura = value;
            }
        }


        public string Note
        {
            get
            {
                return note;
            }
            set
            {
                note = value;
            }
        }
        public Richiedente RichiedenteIstanza
        {
            get
            {
                return richiedente;
            }

            set
            {
                richiedente = value;
            }
        }

        public ProtocolloRichiesto ProtocolloRichiesto
        {
            get
            {
                return protocolloRichiesto;
            }
            set
            {
                protocolloRichiesto = value;
            }
        }
    }

    public partial class Richiedente
    {
        private string codiceRichiedente;
        private string descrizioneRichiedente;

        public string CodiceRichiedente
        {
            get
            {
                return codiceRichiedente;
            }
            set
            {
                codiceRichiedente = value;
            }
        }


        public string DescrizioneRichiedente
        {
            get
            {
                return descrizioneRichiedente;
            }
            set
            {
                descrizioneRichiedente = value;
            }
        }

    }

    public partial class ProtocolloRichiesto
    {
        private string segnatura;
        private string oggetto;
        private string dataCreazione;
        private string idDocumento;

        public string Segnatura
        {
            get
            {
                return segnatura;
            }

            set
            {
                segnatura = value;
            }
        }
        public string Oggetto
        {
            get
            {
                return oggetto;
            }

            set
            {
                oggetto = value;
            }
        }
        public string DataCreazione
        {
            get
            {
                return dataCreazione;
            }

            set
            {
                dataCreazione = value;
            }
        }
        public string IdDocumento
        {
            get
            {
                return idDocumento;
            }

            set
            {
                idDocumento = value;
            }
        }
    }
}
