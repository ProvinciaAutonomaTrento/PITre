using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Deposito
{
    public partial class ARCHIVE_View_DocFascReport
    {
        
        #region Fields
        
        private String _Registro;
        private String _UO;
        private String _Tipologia;
        private String _TipoTransfer;

        #endregion

        #region Properties
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

        public String TipoTransfer
        {
            get 
            { 
                return _TipoTransfer; 
            }
            set 
            { 
                _TipoTransfer = value; 
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
        #endregion

        #region Default Constructor

        public ARCHIVE_View_DocFascReport()
        {
        }

        #endregion

        #region Constructors

         public ARCHIVE_View_DocFascReport(String registro,     
                                    String uO,              
                                    String tipologia,      
                                    String tipoTransfer)       

        {
            Registro = registro;
            UO = uO;
            Tipologia = tipologia;
            TipoTransfer = tipoTransfer;
        }

        #endregion
    }
}
