using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Deposito
{
    public partial class ARCHIVE_View_DocReport : ARCHIVE_View_DocFascReport
    {

        #region Fields

        
        private Int32 _Profile_ID;
        private String _ProjectCode;
        private Int32? _Proto;
        private String _Oggetto;
        private DateTime? _CreateDate;
        private Int32? _ProtoYear;
        private String _Tipo;
        private String _MittDest;

       

        #endregion


        #region Properties

        

        public String ProjectCode
        {
            get { return _ProjectCode; }
            set { _ProjectCode = value; }
        }

        public virtual String Oggetto
        {

            get
            {
                return _Oggetto;
            }

            set
            {
                _Oggetto = value;
            }
        }
        
        public DateTime? CreateDate
        {
            get { return _CreateDate; }
            set { _CreateDate = value; }
        }
        public Int32? ProtoDate
        {
            get { return _ProtoYear; }
            set { _ProtoYear = value; }
        }
        
        public Int32 Profile_ID
        {
            get { return _Profile_ID; }
            set { _Profile_ID = value; }
        }

        public Int32? Proto
        {
            get { return _Proto; }
            set { _Proto = value; }
        }
        public String Tipo
        {
            get { return _Tipo; }
            set { _Tipo = value; }
        }

        public String MittDest
        {
            get { return _MittDest; }
            set { _MittDest = value; }
        }

        #endregion

        #region Default Constructor

        public ARCHIVE_View_DocReport()
        {
        }

        #endregion

        #region Constructors

        public ARCHIVE_View_DocReport(String registro,     
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
                                    String mittDest)       

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
            MittDest=mittDest;
        }

        #endregion

    }
}
