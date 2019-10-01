using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Deposito
{
    public partial class ARCHIVE_View_DocReportDisposal : ARCHIVE_View_DocReport
    {
        #region Fields

        Int32 _ToDisposal;

        #endregion

        #region Properties

        public Int32 ToDisposal
        {
            get { return _ToDisposal; }
            set { _ToDisposal = value; }
        }

        #endregion

        #region Constructors

        public ARCHIVE_View_DocReportDisposal()
        {
        }

        public ARCHIVE_View_DocReportDisposal(String registro,
                                    String uO,
                                    Int32 profile_ID,
                                    String projectCode,
                                    Int32? proto,
                                    String oggetto,
                                    DateTime? createDate,
                                    Int32? protoDate,
                                    String tipo,
                                    String tipologia,
                                    String tipoTransfer,
                                    String mittDest,
                                    Int32 toDisposal)
        {
            Registro = registro;
            UO = uO;
            Profile_ID = profile_ID;
            ProjectCode = projectCode;
            Proto = proto;
            Oggetto = oggetto;
            CreateDate = createDate;
            ProtoDate = protoDate;
            Tipo = tipo;
            Tipologia = tipologia;
            TipoTransfer = tipoTransfer;
            MittDest = mittDest;
            ToDisposal = toDisposal;
        }

        #endregion
    }
}
