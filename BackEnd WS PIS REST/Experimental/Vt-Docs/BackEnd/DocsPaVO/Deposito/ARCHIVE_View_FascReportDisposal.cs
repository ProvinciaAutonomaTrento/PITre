using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Deposito
{
    public partial class ARCHIVE_View_FascReportDisposal : ARCHIVE_View_FascReport
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

        public ARCHIVE_View_FascReportDisposal()
        {
        }

        public ARCHIVE_View_FascReportDisposal(String registro, String uO, String descrizione, Int32 project_ID, DateTime? startDate,
                                    DateTime? closeDate, String tipologia, String tipoTransfer, Int32 toDisposal)
        {
            Registro = registro;
            UO = uO;
            Project_ID = project_ID;
            Descrizione = descrizione;
            StartDate = startDate;
            CloseDate = closeDate;
            Tipologia = tipologia;
            TipoTransfer = tipoTransfer;
            ToDisposal = toDisposal;
        }
        #endregion

    }
}
