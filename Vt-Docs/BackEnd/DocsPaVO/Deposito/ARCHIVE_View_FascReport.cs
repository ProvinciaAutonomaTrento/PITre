using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Deposito
{
    public partial class ARCHIVE_View_FascReport: ARCHIVE_View_DocFascReport
    {

        #region Fields

        private Int32 _Project_ID;
        private String _Descrizione;
        private DateTime? _StartDate;
        private DateTime? _CloseDate;
        

        #endregion


        #region Properties

        

        public Int32 Project_ID
        {
            get { return _Project_ID; }
            set { _Project_ID = value; }
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
        
        public DateTime? StartDate
        {
            get { return _StartDate; }
            set { _StartDate = value; }
        }
        public DateTime? CloseDate
        {
            get { return _CloseDate; }
            set { _CloseDate = value; }
        }
        

        #endregion

        #region Default Constructor

        public ARCHIVE_View_FascReport()
        {
        }

        #endregion

        #region Constructors

        public ARCHIVE_View_FascReport(String registro,  String uO, String descrizione, Int32 project_ID, DateTime? startDate,
                                    DateTime? closeDate, String tipologia, String tipoTransfer)
        {
            Registro = registro;
            UO = uO;
            Project_ID = project_ID;
            Descrizione = descrizione;
            StartDate = startDate;
            CloseDate = closeDate;
            Tipologia = tipologia;
            TipoTransfer = tipoTransfer;
        }

        #endregion



    }
}
