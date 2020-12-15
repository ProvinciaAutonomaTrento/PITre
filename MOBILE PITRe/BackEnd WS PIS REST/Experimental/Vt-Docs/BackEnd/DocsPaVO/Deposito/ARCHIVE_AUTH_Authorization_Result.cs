using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Deposito
{
    public partial class ARCHIVE_AUTH_Authorization_Result
    {

        #region Fields

        private Int32 _System_ID;
        private Int32 _People_ID;
        private String _CodiceRubrica;
        private String _DescrizioneRubrica;
        private Int32 _Ruolo_ID;
        private String _DescrizioneRuolo;
        private DateTime _DtaDecorrenza;
        private DateTime _DtaScadenza;
        private Int32? _Livello;
        private String _DescrizioneLivello;

        #endregion

        #region Properties

        public virtual Int32 System_ID
        {

            get
            {
                return _System_ID;
            }

            set
            {
                _System_ID = value;
            }
        }

        public virtual Int32 People_ID
        {

            get
            {
                return _People_ID;
            }

            set
            {
                _People_ID = value;
            }
        }

        public virtual String CodiceRubrica
        {

            get
            {
                return _CodiceRubrica;
            }

            set
            {
                _CodiceRubrica = value;
            }
        }

        public virtual String DescrizioneRubrica
        {

            get
            {
                return _DescrizioneRubrica;
            }

            set
            {
                _DescrizioneRubrica = value;
            }
        }

        public virtual Int32 Ruolo_ID
        {

            get
            {
                return _Ruolo_ID;
            }

            set
            {
                _Ruolo_ID = value;
            }
        }

        public virtual String DescrizioneRuolo
        {

            get
            {
                return _DescrizioneRuolo;
            }

            set
            {
                _DescrizioneRuolo = value;
            }
        }

        public virtual DateTime DtaDecorrenza
        {

            get
            {
                return _DtaDecorrenza;
            }

            set
            {
                _DtaDecorrenza = value;
            }
        }

        public virtual DateTime DtaScadenza
        {

            get
            {
                return _DtaScadenza;
            }

            set
            {
                _DtaScadenza = value;
            }
        }

        public virtual Int32? Livello
        {

            get
            {
                return _Livello;
            }

            set
            {
                _Livello = value;
            }
        }

        public virtual String DescrizioneLivello
        {

            get
            {
                return _DescrizioneLivello;
            }

            set
            {
                _DescrizioneLivello = value;
            }
        }

        #endregion

        #region Default Constructor

        public ARCHIVE_AUTH_Authorization_Result()
        {
        }

        #endregion

        #region Constructors

        public ARCHIVE_AUTH_Authorization_Result(Int32 system_ID, Int32 people_ID, String codiceRubrica, String descrizioneRubrica, Int32 ruolo_ID, String descrizioneRuolo,
                                                DateTime dtaDecorrenza, DateTime dtaScadenza, Int32? livello, String descrizioneLivello)
        {
            System_ID = system_ID;
            People_ID = people_ID;
            CodiceRubrica = codiceRubrica;
            DescrizioneRubrica = descrizioneRubrica;
            Ruolo_ID = ruolo_ID;
            DescrizioneRuolo = descrizioneRuolo;
            DtaDecorrenza = dtaDecorrenza;
            DtaScadenza = dtaScadenza;
            Livello = livello;
            DescrizioneLivello = descrizioneLivello;
        }
        #endregion

    }
}
