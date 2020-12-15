using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Deposito
{
    public partial class ARCHIVE_Result_Transfer
    {
        #region Fields

        private Int32 _System_id_Versamento;
        private String _Descrizione;
        private String _Stato;
        private DateTime _DataEsecuzione;
        private Int32 _Totale_documenti;
        private Int32 _Num_documenti_trasferiti;
        private Int32 _Num_documenti_copiati;

        #endregion


        #region Properties

        public virtual Int32 System_id_Versamento
        {

            get
            {
                return _System_id_Versamento;
            }

            set
            {
                _System_id_Versamento = value;
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

        public virtual DateTime DataEsecuzione
        {

            get
            {
                return _DataEsecuzione;
            }

            set
            {
                _DataEsecuzione = value;
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

        public ARCHIVE_Result_Transfer()
        {
        }

        #endregion

        #region Constructors

        public ARCHIVE_Result_Transfer(Int32 system_id_Versamento, String descrizione, String stato, DateTime dataEsecuzione,
                                    Int32 totale_documenti, Int32 num_documenti_trasferiti, Int32 num_documenti_copiati)
        {
            System_id_Versamento = system_id_Versamento;
            Descrizione = descrizione;
            Stato = stato;
            DataEsecuzione = dataEsecuzione;
            Totale_documenti = totale_documenti;
            Num_documenti_trasferiti = num_documenti_trasferiti;
            Num_documenti_copiati = num_documenti_copiati;
        }

        #endregion


    }
}
