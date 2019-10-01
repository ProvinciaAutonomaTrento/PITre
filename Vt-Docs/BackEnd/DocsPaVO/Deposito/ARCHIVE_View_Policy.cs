using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Deposito
{
    public partial class ARCHIVE_View_Policy
    {
        #region Fields

        private Int32 _Id_policy;
        private String _Descrizione;
        private Int32 _enabled;
        private String _Stato;
        private Int32 _Id_stato;
        private Int32 _Totale_fascicoli;
        private Int32 _Totale_documenti;
        private Int32 _Num_documenti_trasferiti;
        private Int32 _Num_documenti_copiati;

        #endregion


        #region Properties

        public virtual Int32 Id_policy
        {

            get
            {
                return _Id_policy;
            }

            set
            {
                _Id_policy = value;
            }
        }

        public virtual String Descrizione
        {

            get
            {
                return _Descrizione;
            }

            set
            {
                _Descrizione = value;
            }
        }

        public virtual Int32 Enabled
        {

            get
            {
                return _enabled;
            }

            set
            {
                _enabled = value;
            }
        }

        public virtual String Stato
        {

            get
            {
                return _Stato;
            }

            set
            {
                _Stato = value;
            }
        }

        public virtual Int32 Id_stato
        {

            get
            {
                return _Id_stato;
            }

            set
            {
                _Id_stato = value;
            }
        }

        public virtual Int32 Totale_fascicoli
        {

            get
            {
                return _Totale_fascicoli;
            }

            set
            {
                _Totale_fascicoli = value;
            }
        }

        public virtual Int32 Totale_documenti
        {

            get
            {
                return _Totale_documenti;
            }

            set
            {
                _Totale_documenti = value;
            }
        }

        public virtual Int32 Num_documenti_trasferiti
        {

            get
            {
                return _Num_documenti_trasferiti;
            }

            set
            {
                _Num_documenti_trasferiti = value;
            }
        }

        public virtual Int32 Num_documenti_copiati
        {

            get
            {
                return _Num_documenti_copiati;
            }

            set
            {
                _Num_documenti_copiati = value;
            }
        }

        #endregion

        #region Default Constructor

        public ARCHIVE_View_Policy()
        {
        }

        #endregion

        #region Constructors

        public ARCHIVE_View_Policy(Int32 id_policy, String descrizione, Int32 enabled, String stato, Int32 id_stato, Int32 totale_fascicoli,
                                    Int32 totale_documenti, Int32 num_documenti_trasferiti, Int32 num_documenti_copiati)
        {
            Id_policy = id_policy;
            Descrizione = descrizione;
            Enabled = enabled;
            Stato = stato;
            Id_stato = id_stato;
            Totale_fascicoli = totale_fascicoli;
            Totale_documenti = totale_documenti;
            Num_documenti_trasferiti = num_documenti_trasferiti;
            Num_documenti_copiati = num_documenti_copiati;
        }

        #endregion


    }
}
