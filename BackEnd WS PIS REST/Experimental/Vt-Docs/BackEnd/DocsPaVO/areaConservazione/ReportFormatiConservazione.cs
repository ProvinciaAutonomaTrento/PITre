using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.areaConservazione
{
    [Serializable()]
    public partial class ReportFormatiConservazione
    {
        #region Fields

        private String _System_ID;
        private String _ID_Istanza;
        private String _ID_Item;
        private String _DocNumber;
        private String _ID_Project;
        private String _ID_DocPrincipale;
        private String _Version_ID;
        private String _TipoFile;
        private String _Estensione;
        private String _Consolidato;
        private String _Esito;
        private String _Convertibile;
        private String _Modifica;
        private String _UtProp;
        private String _RuoloProp;
        private String _Valido;
        private String _Ammesso;
        private String _Firmata;
        private String _Marcata;
        private String _Errore;
        private String _TipoErrore;
        private String _DaConverire;
        private String _Convertito;
        public DettaglioDocumento dettaglioDocumento;
        #endregion

        #region struct
        public struct DettaglioDocumento
        {
            public String Segnatura;
            public String Oggetto;
            public String DataCreazione;
            public String DataProto;
            public String TipoDocumento;
            public String DescrUtProp;
            public String DescrRuoloProp;
            
        }
        #endregion

        #region Properties

        #region struct

        

        #endregion

        public virtual String System_ID
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

        public virtual String ID_Istanza
        {

            get
            {
                return _ID_Istanza;
            }

            set
            {
                _ID_Istanza = value;
            }
        }

        public virtual String ID_Item
        {

            get
            {
                return _ID_Item;
            }

            set
            {
                _ID_Item = value;
            }
        }

  
        public virtual String DocNumber
        {

            get
            {
                return _DocNumber;
            }

            set
            {
                _DocNumber = value;
            }
        }

        public virtual String ID_Project
        {

            get
            {
                return _ID_Project;
            }

            set
            {
                _ID_Project = value;
            }
        }

        public virtual String ID_DocPrincipale
        {

            get
            {
                return _ID_DocPrincipale;
            }

            set
            {
                _ID_DocPrincipale = value;
            }
        }

        public virtual String Version_ID
        {

            get
            {
                return _Version_ID;
            }

            set
            {
                _Version_ID = value;
            }
        }


        public virtual String TipoFile
        {

            get
            {
                return _TipoFile;
            }

            set
            {
                _TipoFile = value;
            }
        }

    
        public virtual String Esito
        {

            get
            {
                return _Esito;
            }

            set
            {
                _Esito = value;
            }
        }

        public virtual String Convertibile
        {

            get
            {
                return _Convertibile;
            }

            set
            {
                _Convertibile = value;
            }
        }


        public virtual String Modifica
        {

            get
            {
                return _Modifica;
            }

            set
            {
                _Modifica = value;
            }
        }

       
        public virtual String UtProp
        {

            get
            {
                return _UtProp;
            }

            set
            {
                _UtProp = value;
            }
        }

        
        public virtual String RuoloProp
        {

            get
            {
                return _RuoloProp;
            }

            set
            {
                _RuoloProp = value;
            }
        }

        public virtual String Valido
        {

            get
            {
                return _Valido;
            }

            set
            {
                _Valido = value;
            }
        }

        public virtual String Ammesso
        {

            get
            {
                return _Ammesso;
            }

            set
            {
                _Ammesso = value;
            }
        }

        public virtual String Consolidato
        {
            get
            {
                return _Consolidato;
            }
            set
            {
                _Consolidato = value;
            }
        }

        public virtual String Firmata
        {

            get
            {
                return _Firmata;
            }

            set
            {
                _Firmata = value;
            }
        }

        public virtual String Marcata
        {
            get
            {
                return _Marcata;
            }
            set
            {
                _Marcata = value;
            }
        }

        public virtual String Errore
        {
            get
            {
                return _Errore;
            }
            set
            {
                _Errore = value;
            }
        }

        public virtual String TipoErrore
        {
            get
            {
                return _TipoErrore;
            }
            set
            {
                _TipoErrore = value;
            }
        }

        public virtual String DaConverire
        {
            get
            {
                return _DaConverire;
            }
            set
            {
                _DaConverire = value;
            }
        }

        public virtual String Convertito
        {
            get
            {
                return _Convertito;
            }
            set
            {
                _Convertito = value;
            }
        }

        public virtual String Estensione
        {
            get
            {
                return _Estensione;
            }
            set
            {
                _Estensione = value;
            }
        }



        #endregion

        #region Default Constructor

        public ReportFormatiConservazione()
        {
        }

        #endregion

        #region Constructors

        public ReportFormatiConservazione(String system_ID,
                                    String iD_Istanza,
                                    String iD_Item,
                                    String docNumber,
                                    String iD_Project,
                                    String iD_DocPrincipale,
                                    String version_ID,
                                    String tipoFile,
                                    String estensione,
                                    String ammesso,
                                    String valido,
                                    String consolidato,
                                    String convertibile,
                                    String firmata,
                                    String marcata,
                                    String modifica,
                                    String utProp,
                                    String ruoloProp,
                                    String esito,
                                    String daConverire,
                                    String convertito,
                                    String errore,
                                    String tipoErrore)
        {
            System_ID = system_ID;
            ID_Istanza = iD_Istanza;
            ID_Item = iD_Item;
            DocNumber = docNumber;
            ID_Project = iD_Project;
            ID_DocPrincipale = iD_DocPrincipale;
            Version_ID = version_ID;
            TipoFile = tipoFile;
            Estensione = estensione;
            Ammesso = ammesso;
            Valido = valido;
            Consolidato = consolidato;
            Convertibile = convertibile;
            Firmata = firmata;
            Marcata = marcata;
            Modifica = modifica;
            UtProp = utProp;
            RuoloProp = ruoloProp;
            Esito = esito;
            Errore = errore;
            DaConverire = daConverire;
            Convertito = convertito;
            TipoErrore = tipoErrore;
            
        }


        public ReportFormatiConservazione(String system_ID,
                                  String iD_Istanza,
                                  String iD_Item,
                                  String docNumber,
                                  String iD_Project,
                                  String iD_DocPrincipale,
                                  String version_ID,
                                  String tipoFile,
                                  String estensione,
                                  String ammesso,
                                  String valido,
                                  String consolidato,
                                  String convertibile,
                                  String firmata,
                                  String marcata,
                                  String modifica,
                                  String utProp,
                                  String ruoloProp,
                                  String esito,
                                  String daConverire,
                                  String convertito,
                                  String errore,
                                  String tipoErrore,
                                  String segnatura,
                                  String oggetto,
                                  String dataCreazione,
                                  String dataProto,
                                  String tipoDocumento,
                                  String descrUtProp,            
                                  String descrRuoloProp)
        {
            System_ID = system_ID;
            ID_Istanza = iD_Istanza;
            ID_Item = iD_Item;
            DocNumber = docNumber;
            ID_Project = iD_Project;
            ID_DocPrincipale = iD_DocPrincipale;
            Version_ID = version_ID;
            TipoFile = tipoFile;
            Estensione = estensione;
            Ammesso = ammesso;
            Valido = valido;
            Consolidato = consolidato;
            Convertibile = convertibile;
            Firmata = firmata;
            Marcata = marcata;
            Modifica = modifica;
            UtProp = utProp;
            RuoloProp = ruoloProp;
            Esito = esito;
            Errore = errore;
            DaConverire = daConverire;
            Convertito = convertito;
            TipoErrore = tipoErrore;
            dettaglioDocumento= new DettaglioDocumento();
            dettaglioDocumento.Segnatura = segnatura;
            dettaglioDocumento.Oggetto = oggetto;
            dettaglioDocumento.DataCreazione = dataCreazione;
            dettaglioDocumento.DataProto = dataProto;
            dettaglioDocumento.TipoDocumento = tipoDocumento;
            dettaglioDocumento.DescrUtProp = descrUtProp;
            dettaglioDocumento.DescrRuoloProp = descrRuoloProp;
        }
        #endregion
    }
}
