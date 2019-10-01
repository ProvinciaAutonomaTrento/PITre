using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Deposito
{
    public partial class ARCHIVE_Profile_TransferPolicy
    {
        #region fields

        private Int32 _TransferPolicy_ID;
        private Int32 _Profile_ID;
        private DateTime? _DataUltimoAccesso;
        private Int32? _NUtentiUltimoAnno;

        private Int32? _NAccessiUltimoAnno;
        private String _TipoTransferimento_Policy;
        private String _TipoTransferimento_Versamento;
        private Int32? _CopiaPerFascicolo_Policy;
        private Int32? _CopiaPerCatenaDoc_Policy;
        private Int32? _CopiaPerConservazione_Policy;
        private Int32? _CopiaPerFascicolo_Versamento;
        private Int32? _CopiaPerCatenaDoc_Versamento;
        private Int32? _CopiaPerConservazione_Versamento;

        private String _OggettoDocumento;
        private String _TipoDocumento;
        private String _Registro;
        private String _UO;
        private String _Tipologia;
        private DateTime? _DataCreazione;
        private Int32? _MantieniCopia;

        #endregion

        #region Properties

        public Int32 TransferPolicy_ID
        {
            get
            {
                return _TransferPolicy_ID;
            }
            set
            {
                _TransferPolicy_ID = value;
            }
        }

        public Int32? NUtentiUltimoAnno
        {
            get
            {
                return _NUtentiUltimoAnno;
            }
            set
            {
                _NUtentiUltimoAnno = value;
            }
        }

        public Int32 Profile_ID
        {
            get
            {
                return _Profile_ID;
            }
            set
            {
                _Profile_ID = value;
            }
        }

        public DateTime? DataUltimoAccesso
        {
            get
            {
                return _DataUltimoAccesso;
            }
            set
            {
                _DataUltimoAccesso = value;
            }
        }

        public Int32? NAccessiUltimoAnno
        {
            get
            {
                return _NAccessiUltimoAnno;
            }
            set
            {
                _NAccessiUltimoAnno = value;
            }
        }

        public String TipoTransferimento_Policy
        {
            get
            {
                return _TipoTransferimento_Policy;
            }
            set
            {
                _TipoTransferimento_Policy = value;
            }
        }

        public String TipoTransferimento_Versamento
        {
            get
            {
                return _TipoTransferimento_Versamento;
            }
            set
            {
                _TipoTransferimento_Versamento = value;
            }
        }

        public Int32? CopiaPerFascicolo_Policy
        {
            get
            {
                return _CopiaPerFascicolo_Policy;
            }
            set
            {
                _CopiaPerFascicolo_Policy = value;
            }
        }

        public Int32? CopiaPerCatenaDoc_Policy
        {
            get
            {
                return _CopiaPerCatenaDoc_Policy;
            }
            set
            {
                _CopiaPerCatenaDoc_Policy = value;
            }
        }

        public Int32? CopiaPerConservazione_Policy
        {
            get
            {
                return _CopiaPerConservazione_Policy;
            }
            set
            {
                _CopiaPerConservazione_Policy = value;
            }
        }

        public Int32? CopiaPerFascicolo_Versamento
        {
            get
            {
                return _CopiaPerFascicolo_Versamento;
            }
            set
            {
                _CopiaPerFascicolo_Versamento = value;
            }
        }

        public Int32? CopiaPerCatenaDoc_Versamento
        {
            get
            {
                return _CopiaPerCatenaDoc_Versamento;
            }
            set
            {
                _CopiaPerCatenaDoc_Versamento = value;
            }
        }

        public Int32? CopiaPerConservazione_Versamento
        {
            get
            {
                return _CopiaPerConservazione_Versamento;
            }
            set
            {
                _CopiaPerConservazione_Versamento = value;
            }
        }

        public String OggettoDocumento
        {
            get
            {
                return _OggettoDocumento;
            }
            set
            {
                _OggettoDocumento = value;
            }
        }

        public String TipoDocumento
        {
            get
            {
                return _TipoDocumento;
            }
            set
            {
                _TipoDocumento = value;
            }
        }

        public String Registro
        {
            get
            {
                return _Registro;
            }
            set
            {
                _Registro = value;
            }
        }

        public String UO
        {
            get
            {
                return _UO;
            }
            set
            {
                _UO = value;
            }
        }

        public String Tipologia
        {
            get
            {
                return _Tipologia;
            }
            set
            {
                _Tipologia = value;
            }
        }

        public DateTime? DataCreazione
        {
            get
            {
                return _DataCreazione;
            }
            set
            {
                _DataCreazione = value;
            }
        }

        public Int32? MantieniCopia
        {
            get
            {
                return _MantieniCopia;
            }
            set
            {
                _MantieniCopia = value;
            }
        }

        #endregion

        #region Default Constructor

        public ARCHIVE_Profile_TransferPolicy()
        {
        }

        #endregion

        #region Constructors

        public ARCHIVE_Profile_TransferPolicy(Int32 transferPolicy_ID,
                                                Int32 profile_ID,
                                                DateTime? dataUltimoAccesso,
                                                Int32? nUtentiUltimoAnno,
                                                Int32? nAccessiUltimoAnno,
                                                String tipoTransferimento_Policy,
                                                String tipoTransferimento_Versamento,
                                                Int32? copiaPerFascicolo_Policy,
                                                Int32? copiaPerCatenaDoc_Policy,
                                                Int32? copiaPerConservazione_Policy,
                                                Int32? copiaPerFascicolo_Versamento,
                                                Int32? copiaPerCatenaDoc_Versamento,
                                                Int32? copiaPerConservazione_Versamento,
                                                String oggettoDocumento,
                                                String tipoDocumento,
                                                String registro,
                                                String uO,
                                                String tipologia,
                                                DateTime? dataCreazione,
                                                Int32? mantieniCopia)
        {
            TransferPolicy_ID = transferPolicy_ID;
            Profile_ID = profile_ID;
            DataUltimoAccesso = dataUltimoAccesso;
            NUtentiUltimoAnno = nUtentiUltimoAnno;
            NAccessiUltimoAnno = nAccessiUltimoAnno;
            TipoTransferimento_Policy = tipoTransferimento_Policy;
            TipoTransferimento_Versamento = tipoTransferimento_Versamento;
            CopiaPerFascicolo_Policy = copiaPerFascicolo_Policy;
            CopiaPerCatenaDoc_Policy = copiaPerCatenaDoc_Policy;
            CopiaPerConservazione_Policy = copiaPerConservazione_Policy;
            CopiaPerFascicolo_Versamento = copiaPerFascicolo_Versamento;
            CopiaPerCatenaDoc_Versamento = copiaPerCatenaDoc_Versamento;
            CopiaPerConservazione_Versamento = copiaPerConservazione_Versamento;
            OggettoDocumento = oggettoDocumento;
            TipoDocumento = tipoDocumento;
            Registro = registro;
            UO = uO;
            Tipologia = tipologia;
            DataCreazione = dataCreazione;
            MantieniCopia = mantieniCopia;
        }

        #endregion

    }
}
