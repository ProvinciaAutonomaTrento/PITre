using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic;
using System.IO;
  
namespace printPen
{
  
    public class printPen
    {
        [DllImport("kernel32", SetLastError = true)]
        private static extern int GetPrivateProfileSection(string lpAppName, byte[] lpszReturnBuffer, int nSize, string lpFileName);

        [DllImport("KERNEL32.DLL", EntryPoint = "GetPrivateProfileStringW",
  SetLastError = true,
  CharSet = CharSet.Unicode, ExactSpelling = true,
  CallingConvention = CallingConvention.StdCall)]
        private static extern int GetPrivateProfileString(
          string lpAppName,
          string lpKeyName,
          string lpDefault,
          string lpReturnString,
          int nSize,
          string lpFilename);
        const char vbNullChar = (char)0;


[DllImport("urlmon.dll", CharSet=CharSet.Auto, SetLastError=true)]
static extern Int32 URLDownloadToFile(
    [MarshalAs(UnmanagedType.IUnknown)] object pCaller,
    [MarshalAs(UnmanagedType.LPWStr)] string szURL,
    [MarshalAs(UnmanagedType.LPWStr)] string szFileName,
    Int32 dwReserved,
    IntPtr lpfnCB);
        // Declare Function IsValidURL Lib "urlmon" (ByVal pBC As Integer, ByRef url As Byte, ByVal dwReserved As Integer) As Integer



//public string UrlFileIni = "";
//public string Dispositivo = "";
//public string Text = "";
//public string Amministrazione = "";
//public string NumeroDocumento = "";
//public string Classifica = "";
//public string Fascicolo = "";
//public string Amministrazione_Etichetta = "";
//public string CodiceUoProtocollatore = "";
//public string CodiceRegistroProtocollo = "";
//public string TipoProtocollo = "";
//public string NumeroProtocollo = "";
//public string AnnoProtocollo = "";
//public string DataProtocollo = "";


private string m_FILEINI_NAME = "DOCSPA.INI";

public string FILEINI_NAME
{
    get { return m_FILEINI_NAME; }
    set { m_FILEINI_NAME = value; }
}
	//string m_PathFile;
const string vbNullString = "";

//const string DYMO_FILE_NAME = "ET_DYMO.LWL";
    const string DYMO_FILE_NAME = "ET_DYMO.label";
    const string DYMO_FASC_FILE_NAME = "ET_DYMO_FASC.label";
    string m_PathFileDymo;
	
	bool m_blnConfigOk; //'flag di test configurazione

    const int BUFFER_SIZE = 3000;
    byte[] buffer = new byte[BUFFER_SIZE];

	udt_INIDeviceSetting m_udtINIDeviceSetting;

	string APPLICATION_NAME = "DocsPa_PrintPen";
	
	string m_PathFile ;

    string m_PathDymo = null;

    public string PathDymo
    {
        get { return m_PathDymo; }
        set { m_PathDymo = value; }
    }
	
    enum enumSezioniIni
    {
        ConfigDispositivo = 0,
        ScriptCommand = 1
    };

        private struct udt_INIDeviceSetting
        {
            //sezione configurazione dispositivo
            ////[VBFixedArray(2)]
            //ID numero porta  1,2,3,4 (USATO PER COM/LPT)
            public string[] Device_ID;
            ////[VBFixedArray(2)]
            //parametri di configurazione della porta (Usato x la com)
            public string[] Device_Setting;
            ////[VBFixedArray(2)]
            //tempo massimo di attesa sulla porta (Usato x la com)
            public string[] Device_TimeOut;
            ////[VBFixedArray(2)]
            //Attendi Conferma dal dispositivo di avvenuta stampa ?
            public string[] Device_WaitConfermaStampa;
            ////[VBFixedArray(2)]
            //Chiedi Conferma prima di stampare?
            public string[] Device_AlertConfermaStampa;
            ////[VBFixedArray(2)]
            //Per identificare velocemente lo script
            public string[] Device_Descrizione;
            //[VBFixedArray(2)]
            //Modalità stampa USB attiva / non attiva
            public string[] USBPrintMode;
            //[VBFixedArray(2)]
            //Device name della stampante utilizzata per la stampa USB
            public string[] USBPrinterDeviceName;
            //[VBFixedArray(2)]
            public string[] PRINTLINENUMBER;
            //sezione script: dati statici
            //[VBFixedArray(2)]
            public string[] BOLD;
            //[VBFixedArray(2)]
            public string[] CRLF;
            //UPGRADE_NOTE: RESET was upgraded to RESET_Renamed. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
            //[VBFixedArray(2)]
            public string[] RESET_Renamed;
            //[VBFixedArray(2)]
            public string[] STX;
            //[VBFixedArray(2)]
            public string[] CR;
            //[VBFixedArray(2)]
            public string[] HANDSHAKING;
            //[VBFixedArray(2)]
            public string[] ITALIA;
            //sezione script: dati dinamici
            //[VBFixedArray(2)]
            public string[] NumeroStampaCorrente;
            //[VBFixedArray(2)]
            public string[] OraCreazione;
            //[VBFixedArray(2)]
            public string[] Amministrazione;
            //[VBFixedArray(2)]
            public string[] testo;
            //[VBFixedArray(2)]
            public string[] NumCopie;
            //TimbroCitec(10) As String
            //INIZIO     __ Maria Pia Benevieri 03.05.2004  --> aggiunte property type per stampa su più righe dell'etichetta
            //[VBFixedArray(2)]
            public string[] Fascicolo;
            //[VBFixedArray(2)]
            public string[] Classifica;
            //[VBFixedArray(2)]
            public string[] Amministrazione_Etichetta;
            ///'''''''''''''''''''''''''''''''
            //stampa fascicoli
            ///'''''''''''''''''''''''''''''''
            //[VBFixedArray(2)]
            public string[] ClassificaFasc;
            //[VBFixedArray(2)]
            public string[] CodiceFasc;
            //[VBFixedArray(2)]
            public string[] DescrizioneFasc;
            //[VBFixedArray(2)]
            public string[] DescrizioneFasc1;
            //[VBFixedArray(2)]
            public string[] DescrizioneFasc2;
            //[VBFixedArray(2)]
            public string[] DescrizioneFasc3;
            //[VBFixedArray(2)]
            public string[] DescrizioneFasc4;
            ///'''''''''''''''''''''''''''''''
            //FINE       __ Maria Pia Benevieri 03.05.2004
            //INIZIO Massimo digregorio 2005-02-24
            //[VBFixedArray(2)]
            // Numero di allegati del documento
            public string[] NumeroAllegati;
            //[VBFixedArray(2)]
            //numero di documento
            public string[] NumeroDocumento;
            //[VBFixedArray(2)]
            //Codice della uo di appartenenza del ruolo che ha protocollato
            public string[] CodiceUoProtocollotore;
            //[VBFixedArray(2)]
            //Codice del registro utilizzato per protocollare
            public string[] CodiceRegistroProtocollo;
            //[VBFixedArray(2)]
            //Descrizione del registro utilizzato per protocollare
            public string[] DescrizioneRegistroProtocollo;
            //[VBFixedArray(2)]
            //tipo di protocollo (A = Arrivo , P=Partenza)
            public string[] TipoProtocollo;
            //[VBFixedArray(2)]
            //Numero di protocollo
            public string[] NumeroProtocollo;
            //[VBFixedArray(2)]
            //Anno di protocollo
            public string[] AnnoProtocollo;
            //[VBFixedArray(2)]
            //Data di protocollazione gg/mm/aaaa
            public string[] DataProtocollo;
            //FINE Massimo digregorio 2005-02-24
            // sezioni per il documento grigio
            //[VBFixedArray(2)]
            //Data di creazione del documento gg/mm/aaaa
            public string[] DataCreazione;
            //[VBFixedArray(2)]
            //Codice della uo di appartenenza del ruolo che ha creato il documento
            public string[] CodiceUoCreatore;
            //[VBFixedArray(2)]
            // Contatore per il numero di stampe del documento da effettuare
            public string[] NumeroStampe;
            //[VBFixedArray(2)]
            //Numero di stampe del documento effettuate finora
            public string[] NumeroStampeEffettuate;
            //[VBFixedArray(2)]
            //Data Arrivo del documento
            public string[] DataArrivo;
            //[VBFixedArray(2)]
            //Data Arrivo con ora
            public string[] DataArrivoEstesa;

            //UPGRADE_TODO: "Initialize" must be called to initialize instances of this structure. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="B4BFF9E0-8631-45CF-910E-62AB3970F27B"'
            public void Initialize()
            {
                Device_ID = new string[3];
                Device_Setting = new string[3];
                Device_TimeOut = new string[3];
                Device_WaitConfermaStampa = new string[3];
                Device_AlertConfermaStampa = new string[3];
                Device_Descrizione = new string[3];
                USBPrintMode = new string[3];
                USBPrinterDeviceName = new string[3];
                PRINTLINENUMBER = new string[3];
                BOLD = new string[3];
                RESET_Renamed = new string[3];
                CRLF = new string[3];
                STX = new string[3];
                CR = new string[3];
                HANDSHAKING = new string[3];
                ITALIA = new string[3];
                NumeroStampaCorrente = new string[3];
                OraCreazione = new string[3];
                Amministrazione = new string[3];
                testo = new string[3];
                NumCopie = new string[3];
                Fascicolo = new string[3];
                Classifica = new string[3];
                Amministrazione_Etichetta = new string[3];
                ClassificaFasc = new string[3];
                CodiceFasc = new string[3];
                DescrizioneFasc = new string[3];
                DescrizioneFasc1 = new string[3];
                DescrizioneFasc2 = new string[3];
                DescrizioneFasc3 = new string[3];
                DescrizioneFasc4 = new string[3];
                NumeroAllegati = new string[3];
                NumeroDocumento = new string[3];
                CodiceUoProtocollotore = new string[3];
                CodiceRegistroProtocollo = new string[3];
                DescrizioneRegistroProtocollo = new string[3];
                TipoProtocollo = new string[3];
                NumeroProtocollo = new string[3];
                AnnoProtocollo = new string[3];
                DataProtocollo = new string[3];
                DataCreazione = new string[3];
                CodiceUoCreatore = new string[3];
                NumeroStampe = new string[3];
                NumeroStampeEffettuate = new string[3];
                DataArrivo = new string[3];
                DataArrivoEstesa = new string[3];
            }
        }

        //udt_INIDeviceSetting m_udtINIDeviceSetting;
	
        //secondi
        const short m_def_TimeOut = 60;
        const short m_def_NumCopie = 1;
        const string m_def_Text = "DFPrintPen Test";
        const string m_def_PortaCOM = "1";
        const string m_def_Dispositivo = "Penna";
        const string m_def_Amministrazione = "";
        const string m_def_UrlFileIni = "";

        //INIZIO     __ Maria Pia Benevieri 03.05.2004 --> aggiunte default property per stampa su più righe dell'etichetta
        const string m_def_Classifica = "";
        const string m_def_Fascicolo = "";
        const string m_def_Amministrazione_Etichetta = "";
        //FINE       __ Maria Pia Benevieri 03.05.2004

        //INIZIO Massimo digregorio 2005-02-24
        const string m_def_NumeroAllegati = "";
        const string m_def_NumeroDocumento = "";
        const string m_def_CodiceUoProtocollotore = "";
        const string m_def_CodiceRegistroProtocollo = "";
        const string m_def_DescrizioneRegistroProtocollo = "";
        const string m_def_TipoProtocollo = "";
        const string m_def_NumeroProtocollo = "";
        const string m_def_AnnoProtocollo = "";
        const string m_def_DataProtocollo = "";
        //FINE Massimo digregorio 2005-02-24

        const string m_def_NumeroStampe = "1";
        const string m_def_NumeroStampeEffettuate = "";
        const string m_def_NumeroStampaCorrente = "";
        const string m_def_stampaOk = "true";
        const string m_def_DataArrivo = "";
        const string m_def_DataArrivoEstesa = "";
        // INIZIO documento grigio
        const string m_def_DataCreazione = "";
        const string m_def_CodiceUoCreatore = "";
        // FINE documento grigio

        //data creazione protocollo
        const string m_def_OraCreazione = "";
        //fine

        //FINE
        //Property Variables:
        int m_TimeOut;
        short m_NumCopie;
        string m_Text;

        public string Text
        {
            get { return m_Text; }
            set { m_Text = Utils.FormatJs(value); }
        }
        string m_PortaCOM;
        string m_Dispositivo;

        public string Dispositivo
        {
            get { return m_Dispositivo; }
            set { m_Dispositivo = value; }
        }

        
        //
        string m_NumeroAllegati;
        public string NumeroAllegati
        {
          get { return m_NumeroAllegati; }
          set { m_NumeroAllegati = value; }
        }

        string m_Amministrazione;

        public string Amministrazione
        {
            get { return m_Amministrazione; }
            set { m_Amministrazione = Utils.FormatJs(value); }
        }
        string m_UrlFileIni;

        public string UrlFileIni
        {
            get { return m_UrlFileIni; }
            set { m_UrlFileIni = value; }
        }
        string m_ModelloDispositivo;

        public string ModelloDispositivo
        {
            get { return m_ModelloDispositivo; }
            set { m_ModelloDispositivo = value; }
        }




        // Campi aggiuntivi
        //m_NumeroAllegati = this.hd_numero_allegati.Value;
        //pp.DataCreazione = this.hd_dataCreazione.Value;
        //pp.CodiceUoCreatore = this.hd_codiceUoCreatore.Value;
        //pp.DescrizioneRegistroProtocollo = this.hd_descreg_proto.Value;
        //pp.DataArrivo = this.hd_dataArrivo.Value;
        //pp.DataArrivoEstesa = this.hd_dataArrivoEstesa.Value;
        //pp.ModelloDispositivo = "unescape(document.forms[0].hd_modello_dispositivo.value)"; //this.hd_modello_dispositivo.Value;
        //pp.NumeroStampe = this.hd_num_stampe.Value;
        //pp.NumeroStampeEffettuate = this.hd_num_stampe_effettuate.Value;
        //pp.NumeroStampaCorrente = this.hd_num_stampe_effettuate.Value;
        //pp.OraCreazione = this.hd_ora_creazione.Value;


        //Campi Stampa Fascicolo
        string m_fasc_classifica;
        public string ClassificazioneFascicolo
        {
            get { return m_fasc_classifica; }
            set { m_fasc_classifica = value; }
        }

        string m_fasc_codice;
        public string CodiceFascicolo
        {
            get { return m_fasc_codice; }
            set { m_fasc_codice = value; }
        }

        string m_fasc_descrizione;
        public string DescrizioneFascicolo
        {
            get { return m_fasc_descrizione; }
            set { m_fasc_descrizione = Utils.FormatJs(value); }
        }

        string m_fasc_apertura;
        public string AperturaFascicolo
        {
            get { return m_fasc_apertura; }
            set { m_fasc_apertura = value; }
        }

        string m_fasc_descrizione1;
        public string FascicoloDesc1
        {
            get { return m_fasc_descrizione1; }
            set { m_fasc_descrizione1 = Utils.FormatJs(value); }
        }

        string m_fasc_descrizione2;
        public string FascicoloDesc2
        {
            get { return m_fasc_descrizione2; }
            set { m_fasc_descrizione2 = Utils.FormatJs(value); }
        }

        string m_fasc_descrizione3;
        public string FascicoloDesc3
        {
            get { return m_fasc_descrizione3; }
            set { m_fasc_descrizione3 = Utils.FormatJs(value); }
        }

        string m_fasc_descrizione4;
        public string FascicoloDesc4
        {
            get { return m_fasc_descrizione4; }
            set { m_fasc_descrizione4 = Utils.FormatJs(value); }
        }

        string m_fasc_classifica_def = "";
        string m_fasc_codice_def = "";
        string m_fasc_apertura_def = "";
        string m_fasc_descrizione_def = "";
        string m_fasc_descrizione1_def;
        string m_fasc_descrizione2_def;
        string m_fasc_descrizione3_def;
        string m_fasc_descrizione4_def;

        //m_fasc_classifica_def = "";
        //m_fasc_codice_def = "";
        //m_fasc_descrizione_def = "";


        //INIZIO     __ Maria Pia Benevieri 03.05.2004 --> aggiunte property per stampa su più righe dell'etichetta
        //classifica del documento
        string m_Classifica;

        public string Classifica
        {
            get { return m_Classifica; }
            set { m_Classifica = value; }
        }
        //fascicolo che contiene il documento (da stabilire, nel caso si a più di uno, se prendere sempre il primo)
        string m_Fascicolo;

        public string Fascicolo
        {
            get { return m_Fascicolo; }
            set { m_Fascicolo = value; }
        }
        //descrizione amministrazione da scrivere sull'etichetta (non sul barcode!!)
        string m_Amministrazione_Etichetta;

        public string Amministrazione_Etichetta
        {
            get { return m_Amministrazione_Etichetta; }
            set { m_Amministrazione_Etichetta = Utils.FormatJs(value); }
        }
        //FINE       __ Maria Pia Benevieri 03.05.2004

        //INIZIO Massimo digregorio 2005-02-24
        //numero di documento
        string m_NumeroDocumento;

        public string NumeroDocumento
        {
            get { return m_NumeroDocumento; }
            set { m_NumeroDocumento = value; }
        }
        //Codice della uo di appartenenza del ruolo che ha protocollato
        string m_CodiceUoProtocollatore;

        public string CodiceUoProtocollatore
        {
            get { return m_CodiceUoProtocollatore; }
            set { m_CodiceUoProtocollatore = value; }
        }
        //Codice del registro utilizzato per protocollare
        string m_CodiceRegistroProtocollo;

        public string CodiceRegistroProtocollo
        {
            get { return m_CodiceRegistroProtocollo; }
            set { m_CodiceRegistroProtocollo = value; }
        }
        // Descrizione del registro utilizzato per protocollare
        string m_DescrizioneRegistroProtocollo;
        public string DescrizioneRegistroProtocollo
        {
          get { return m_DescrizioneRegistroProtocollo; }
          set { m_DescrizioneRegistroProtocollo = Utils.FormatJs(value); }
        }

        //tipo di protocollo (A = Arrivo , P=Partenza)
        string m_TipoProtocollo;

        public string TipoProtocollo
        {
            get { return m_TipoProtocollo; }
            set { m_TipoProtocollo = value; }
        }
        //Numero di protocollo
        string m_NumeroProtocollo;

        public string NumeroProtocollo
        {
            get { return m_NumeroProtocollo; }
            set { m_NumeroProtocollo = value; }
        }
        //Anno di protocollo
        string m_AnnoProtocollo;

        public string AnnoProtocollo
        {
            get { return m_AnnoProtocollo; }
            set { m_AnnoProtocollo = value; }
        }
        //Data di protocollazione gg/mm/aaaa
        string m_DataProtocollo;

        public string DataProtocollo
        {
            get { return m_DataProtocollo; }
            set { m_DataProtocollo = value; }
        }
        //FINE Massimo digregorio 2005-02-24

        //numero di stampe da effettuare
        string m_NumeroStampe = "";

public string NumeroStampe
{
  get { return m_NumeroStampe; }
  set { m_NumeroStampe = value; }
}
        //numero stampe effettuate
        string m_NumeroStampeEffettuate = "";

public string NumeroStampeEffettuate
{
  get { return m_NumeroStampeEffettuate; }
  set { m_NumeroStampeEffettuate = value; }
}
        //numero corrente da inserire nell'etichetta da stampata
        string m_NumeroStampaCorrente = "";

public string NumeroStampaCorrente
{
  get { return m_NumeroStampaCorrente; }
  set { m_NumeroStampaCorrente = value; }
}


        //data di creazione del protocollo
        string m_OraCreazione = "";

public string OraCreazione
{
  get { return m_OraCreazione; }
  set { m_OraCreazione = value; }
}
        //bool che indica l'esito della stampa
        bool m_stampaOK = false;

        //data arrivo documento
        string m_dataArrivo = "";
        public string DataArrivo
        {
          get { return m_dataArrivo; }
          set { m_dataArrivo = value; }
        }

        string m_dataArrivoEstesa = "";
        public string DataArrivoEstesa
        {
          get { return m_dataArrivoEstesa; }
          set { m_dataArrivoEstesa = value; }
        }
        
        // INIZIO Documento grigio
        // Data di creazione gg/mm/aaaa

        // 
        private string m_DataCreazione = "";
        public string DataCreazione
        {
          get { return m_DataCreazione; }
          set { m_DataCreazione = value; }
        }
        // Codice della uo di appartenenza del ruolo che ha creato il documento
        private string m_codiceUoCreatore = "";
        public string CodiceUoCreatore
        {
          get { return m_codiceUoCreatore; }
          set { m_codiceUoCreatore = value; }
        }


        private string m_DymoXML = "";

        public string DymoXML
        {
            get { return m_DymoXML; }
            set { m_DymoXML = value; }
        }

        private string m_DymoFascXML = "";

        public string DymoFascXML
        {
            get { return m_DymoFascXML; }
            set { m_DymoFascXML = value; }
        }


        // FINE

        //Costanti e variabili relative alla stampante etichette
        const string m_def_Q1 = "Q690";
        const string m_def_Q2 = "24+0";
        const string m_def_P1 = "A63";
        const string m_def_P2 = "696";
        const string m_def_P3 = "3";
        const string m_def_P4 = "4";
        const string m_def_P5 = "3";
        const string m_def_P6 = "1";

        string[] m_def_p_vars = new string[7];
        string[] m_def_q_vars = new string[3];

        string m_Q1;
        string m_Q2;
        string m_P1;
        string m_P2;
        string m_P3;
        string m_P4;
        string m_P5;
        string m_P6;
        private string m_PathZebra;

        public string PathZebra
        {
            get { return m_PathZebra; }
            set { m_PathZebra = value; }
        }



        public printPen()
        {
            UserControl_InitProperties();

            m_udtINIDeviceSetting.Initialize();

            //m_PathFile = getINIFileName();
            //m_blnConfigOk = checkFileIni();

        }

        public void init_PrintPen()
        {
            m_PathFile = getINIFileName();
            m_blnConfigOk = checkFileIni();
        }

        //            getDymoPrintScript
        public string getDymoPrintScript(bool debug)
        {

            Dictionary<string, string> dymoMap = getDymoMap();
            String xml = m_DymoXML;

            if (xml == "")
                xml = getDymoFile(DYMO_FILE_NAME);

            return DymoJS.getDymoPrintScript(dymoMap, xml, short.Parse(m_NumeroStampe), debug);
        }

        //            getDymoPrintScript
        public string getDymoFascPrintScript(bool debug)
        {

            Dictionary<string, string> dymoMap = getDymoFascMap();
            String xml = m_DymoFascXML;

            if (xml == "")
                xml = getDymoFile(DYMO_FASC_FILE_NAME);

            return DymoJS.getDymoPrintScript(dymoMap, xml, short.Parse(m_NumeroStampe), debug);
        }

        public string getDymoBaseUrl()
        {
            return DymoJS.getDymoBaseUrl();
        }

        public string getDymoFile(string fileName) {
            string xmlDymoFile = null;

            m_PathFileDymo = getDymoFileName(fileName);
            
            if (System.IO.File.Exists(m_PathFileDymo))
            {
                xmlDymoFile = System.IO.File.ReadAllText(m_PathFileDymo);
            }
            else
                throw new System.Exception("IL file DYMO XML " + m_PathFile + " non esiste");

            return xmlDymoFile;
        }

        private string getDymoFileName(string fileName)
        {

            string sFile = null;
            //string sBuffer = null;
            //long lBuf = 0;

            if (String.IsNullOrEmpty(m_PathDymo))
                sFile = System.IO.Path.GetDirectoryName(Environment.SystemDirectory);// GetWindowsDirectory(sBuffer, 254);
            else
                sFile = m_PathDymo;

            sFile = Path.Combine(sFile, fileName);

            return sFile;
        }

        // Carica nome della stampante Zebra dal file INI da passare all'applet
        public string GetZebraPrinterName()
        {
            try
            {
                string printer = ReadKeyFromIniFile("USB_PRINTER_DEVICE_NAME", "DISPOSITIVO");
                return printer;
            }
            catch (Exception ex)
            {
                return "Generic / Text Only";
            }
        }

        private bool checkFileIni() {
    
            //Verifica che il file ini esiste in locale o esegue il download
            if (!System.IO.File.Exists(m_PathFile)) {
                throw new Exception("loadIniFile() : Impossibile trovare il File di configurazione.");
            }
            return true;

            }

        // ritorna path file ini zebra
        private string getINIFileName()
        {
            string sFile = null;

            if (String.IsNullOrEmpty(m_PathZebra))
                sFile = Path.GetDirectoryName(Environment.SystemDirectory); 
            else
                sFile = m_PathZebra;                                
                       
            sFile = Path.Combine(sFile, m_FILEINI_NAME);
            return sFile;
        }

        public bool Stampa(ref string ret)
        {
            bool functionReturnValue = false;
            // ERROR: Not supported in C#: OnErrorStatement

            try
            {

                switch (m_ModelloDispositivo.ToUpper())
                {
                    case "DYMO_LABEL_WRITER_400":
                                    throw new Exception("la stampa DYMO viene comandata dai metodi getDymoFascPrintScript e getDymoPrintScript");	
        //functionReturnValue = StampaDymo(ref ret);
                        break;
                    default:
                        functionReturnValue = StampaInternal("SCRIPT", ref ret);
                        break;
                }
            }
            catch (Exception e)
            {
                functionReturnValue = false;
                throw new Exception("Errore: Errore in fase di stampa." + Environment.NewLine + "dettaglio Errore :" + e.Message + APPLICATION_NAME, e);
            }
            
            
            return functionReturnValue;

        }


public bool StampaGrigio(ref string ret)
{
	bool functionReturnValue = false;
	 // ERROR: Not supported in C#: OnErrorStatement

    try{
	switch (m_ModelloDispositivo.ToUpper()) {
		case "DYMO_LABEL_WRITER_400":
			            throw new Exception("implementare stampa DYMO");	
        //StampaDymo(ret);
			break;
		default:
			functionReturnValue = StampaInternal("SCRIPT_GRIGIO", ref ret);
			break;
	}
    }
    catch (Exception e)
    {
        functionReturnValue = false;
        //Interaction.MsgBox("Errore:" + Err().Number + Constants.vbCrLf + "Descrizione:" + "Errore in fase di stampa." + Constants.vbCrLf + "dettaglio Errore :" + Err().Description, MsgBoxStyle.Critical, APPLICATION_NAME);
    }


    return functionReturnValue;
}

// Stampa etichetta per dettaglio fascicolo
public bool StampaFascicolo(ref string ret)
{
	bool functionReturnValue = false;
	 // ERROR: Not supported in C#: OnErrorStatement

    try{
	switch (m_ModelloDispositivo.ToUpper()) {
		case "DYMO_LABEL_WRITER_400":
            throw new Exception("implementare stampa DYMO fascicolo");	
        //StampaDymoFasc(ret);
			break;
		default:
			functionReturnValue = StampaInternal("SCRIPT_FASCICOLO", ref ret);
			break;
	    }
    }
    catch (Exception e)
    {
        functionReturnValue = false;
        //Interaction.MsgBox("Errore:" + Err().Number + Constants.vbCrLf + "Descrizione:" + "Errore in fase di stampa." + Constants.vbCrLf + "dettaglio Errore :" + Err().Description, MsgBoxStyle.Critical, APPLICATION_NAME);
}
	return functionReturnValue;
	}

// Task di stampa etichetta
private bool StampaInternal(string scriptKey, ref string ret)
{
	bool functionReturnValue = true;
	 // ERROR: Not supported in C#: OnErrorStatement


    //if (!m_blnConfigOk)
    //{
    //    downLoadIniFile();
    //    m_blnConfigOk = true;
    //}

    if (m_blnConfigOk)
        loadConfiguration(enumSezioniIni.ConfigDispositivo);

    //if (!m_blnConfigOk)
    //    Err().Raise(1001, APPLICATION_NAME, "Dispositivo di stampa non settato");

    //bool alertConfermaStampa = false;
    //alertConfermaStampa = m_udtINIDeviceSetting.Device_AlertConfermaStampa[1] == "N";

    //if (alertConfermaStampa == false) {
    //    alertConfermaStampa = Interaction.MsgBox("Proseguire con la stampa?", MsgBoxStyle.MsgBoxSetForeground + MsgBoxStyle.YesNo + MsgBoxStyle.DefaultButton2) == MsgBoxResult.Yes;
    //}

    //if (alertConfermaStampa == true) {
    //    cmdStampa.Enabled = false;
try{		
    StampaScript(scriptKey, ref ret);
    //    cmdStampa.Enabled = true;
    //}
}
catch (Exception e)
{
    functionReturnValue = false;
    //Err().Raise(Err().Number, APPLICATION_NAME, "Errore in fase di stampa." + Constants.vbCrLf + "dettaglio Errore :" + Err().Description);
	
}
	return functionReturnValue;
}

private bool downLoadIniFile()
{
	bool functionReturnValue = false;
	//Get Destination
	string pathLocalFileName = null;
	pathLocalFileName = m_PathFile;

	if (VerificaUrl(m_UrlFileIni)) {
		functionReturnValue = downLoadFile(m_UrlFileIni, pathLocalFileName);
	}
	return functionReturnValue;

}

private bool VerificaUrl(string url)
{
	byte[] b = null;
	int errcode = 0;
	// Questo è necessario in quanto viene passata una stringa Unicode
	//UPGRADE_TODO: Code was upgraded to use System.Text.UnicodeEncoding.Unicode.GetBytes() which may not have the same behavior. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="93DD716C-10E3-41BE-A4A8-3BA40157905B"'
	b = System.Text.UnicodeEncoding.Unicode.GetBytes(url + vbNullChar);


    
	//errcode = IsValidURL(0, b[0], 0);
    Uri uriResult;
    bool isValidUrl = Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out uriResult);

    //bool isValidUrl = Uri.TryCreate("ppP", UriKind.RelativeOrAbsolute, out uriResult);

	if (errcode != 0)
        throw new Exception("VerificaUrl" + "URL del file di configurazione non valido");

	// URL valido
	return true;


}

private bool downLoadFile(string url, string localFileName)
{

	int errcode = 0;
    errcode = URLDownloadToFile(null, url, localFileName, 0, IntPtr.Zero);
	if (errcode != 0)
        throw new Exception("loadConfiguration()" + "Errore in fase di download del File di configurazione del Dispositivo");
	return true;

}


private bool loadConfiguration(enumSezioniIni sezione)
{
	bool functionReturnValue = false;

	string nomeSezione = null;
	 // ERROR: Not supported in C#: OnErrorStatement
    try{

	switch (sezione) {
		case enumSezioniIni.ConfigDispositivo:

			nomeSezione = "DISPOSITIVO";

			var _with1 = m_udtINIDeviceSetting;
			// Lettura parametri stampa USB
			_with1.USBPrintMode[0] = "USB_PRINT_MODE";
			_with1.USBPrintMode[1] = ReadKeyFromIniFile(_with1.USBPrintMode[0], nomeSezione);

			_with1.USBPrinterDeviceName[0] = "USB_PRINTER_DEVICE_NAME";
			_with1.USBPrinterDeviceName[1] = ReadKeyFromIniFile(_with1.USBPrinterDeviceName[0], nomeSezione);

			_with1.PRINTLINENUMBER[0] = "PRINT_LINE_NUMBER";
			_with1.PRINTLINENUMBER[1] = ReadKeyFromIniFile(_with1.PRINTLINENUMBER[0], nomeSezione);

			if ((string.IsNullOrEmpty(_with1.USBPrintMode[1]))) {
				_with1.USBPrintMode[1] = "N";
			}

			if (_with1.USBPrintMode[1] == "N") {
				// Parametri porta COM, letti solamente se stampa USB non attiva

				_with1.Device_ID[0] = "COM";
				//COM=1
				_with1.Device_ID[1] = ReadKeyFromIniFile(_with1.Device_ID[0], nomeSezione);
				//Verifica campo

				if (_with1.Device_ID[1] == vbNullString) {
                    throw new Exception("loadConfiguration()" + "Impossibile caricare il Nome del dispositivo");
				}

				// Verifca della presenza della porta com
				if (!PortaComDisponibile(Convert.ToInt16(_with1.Device_ID[1]))) {
                    throw new Exception("loadConfiguration()" + "Porta di comunicazione non disponibile");
				}

				//Settings = "9600,N,8,1"
				_with1.Device_Setting[0] = "SETTING";
				_with1.Device_Setting[1] = ReadKeyFromIniFile(_with1.Device_Setting[0], nomeSezione);

				//Verifica campo
				if (_with1.Device_Setting[1] == vbNullString) {
					throw new Exception("loadConfiguration()" + "Impossibile caricare il Setting del dispositivo");
				}
			}

			//TIMEOUT=
			_with1.Device_TimeOut[0] = "TIMEOUT";
			_with1.Device_TimeOut[1] = ReadKeyFromIniFile(_with1.Device_TimeOut[0], nomeSezione);
			//WAIT_CONFERMA_STAMPA per la penna
			_with1.Device_WaitConfermaStampa[0] = "WAIT_CONFERMA_STAMPA";
			_with1.Device_WaitConfermaStampa[1] = ReadKeyFromIniFile(_with1.Device_WaitConfermaStampa[0], nomeSezione);
			//DESC_DISPOSITIVO
			_with1.Device_Descrizione[0] = "DESCRIZIONE_DISPOSITIVO";
			_with1.Device_Descrizione[1] = ReadKeyFromIniFile(_with1.Device_Descrizione[0], nomeSezione);


			//      If LCase(.Device_Descrizione[1]) = LCase("TimbroCitec") Then
			//
			//         .TimbroCitec[0] = "Buffer"
			//         .TimbroCitec[1] = ReadKeyFromIniFile(.TimbroCitec[0], nomeSezione)
			//
			//         .TimbroCitec(2) = "Bold"
			//         .TimbroCitec(3) = ReadKeyFromIniFile(.TimbroCitec(2), nomeSezione)
			//
			//         .TimbroCitec(4) = "Compressed"
			//         .TimbroCitec(5) = ReadKeyFromIniFile(.TimbroCitec(4), nomeSezione)
			//
			//         .TimbroCitec(6) = "DoubleHeight"
			//         .TimbroCitec(7) = ReadKeyFromIniFile(.TimbroCitec(6), nomeSezione)
			//
			//         .TimbroCitec(8) = "DoubleWidth"
			//         .TimbroCitec(9) = ReadKeyFromIniFile(.TimbroCitec(8), nomeSezione)
			//
			//      End If
			//ALERT_CONFERMA_STAMPA
			_with1.Device_AlertConfermaStampa[0] = "ALERT_CONFERMA_STAMPA";
			_with1.Device_AlertConfermaStampa[1] = ReadKeyFromIniFile(_with1.Device_AlertConfermaStampa[0], nomeSezione);
			//Verifica esistenza campo
			if (_with1.Device_AlertConfermaStampa[1] == vbNullString) {
				_with1.Device_AlertConfermaStampa[1] = "S";
			}



			//add costant tags
			_with1.BOLD[0] = "<BOLD>";
			_with1.BOLD[1] = new string((char) 27,1) + "G";
			_with1.CRLF[0] = "<CRLF>";
			_with1.CRLF[1] = Environment.NewLine;
			_with1.ITALIA[0] = "<ITALIA>";
            _with1.ITALIA[1] = new string((char)27, 1) + "R" + "6";
			_with1.HANDSHAKING[0] = "<HANDSHAKING>";
            _with1.HANDSHAKING[1] = new string((char)27, 1) + "X";
			_with1.CR[0] = "<CR>";
            _with1.CR[1] = new string((char)13, 1);
			_with1.STX[0] = "<STX>";
			_with1.STX[1] = new string ((char)2,1);
			_with1.RESET_Renamed[0] = "<RESET>";
            _with1.RESET_Renamed[1] = new string((char)27, 1) + "@";
			//add dinamic tags
			_with1.DataArrivo[0] = "<DATA_ARRIVO>";
			//data arrivo
			_with1.DataArrivo[1] = vbNullString;
			_with1.DataArrivoEstesa[0] = "<DATA_ARRIVO_ESTESA>";
			//data arrivo con ora
			_with1.DataArrivoEstesa[1] = vbNullString;
			_with1.NumeroStampaCorrente[0] = "<NUMERO_STAMPA_CORRENTE>";
			_with1.NumeroStampaCorrente[1] = vbNullString;
			//proprieta passata dal chiamante
			_with1.OraCreazione[0] = "<ORA_CREAZIONE_PROTO>";
			_with1.OraCreazione[1] = vbNullString;
			//proprieta passata dal chiamante
			_with1.Amministrazione[0] = "<AMMINISTRAZIONE>";
			_with1.Amministrazione[1] = vbNullString;
			//proprieta passata dal chiamante
			_with1.testo[0] = "<TESTO>";
			_with1.testo[1] = vbNullString;
			//proprieta passata dal chiamante
			_with1.NumCopie[0] = "<NUMCOPIE>";
			_with1.NumCopie[1] = Convert.ToString(m_NumCopie);

			_with1.Fascicolo[0] = "<FASCICOLO>";
			_with1.Fascicolo[1] = vbNullString;
			//proprieta passata dal chiamante
			_with1.Classifica[0] = "<CLASSIFICA>";
			_with1.Classifica[1] = vbNullString;
			//proprieta passata dal chiamante
			_with1.Amministrazione_Etichetta[0] = "<AMMINISTRAZIONE_ETICHETTA>";
			_with1.Amministrazione_Etichetta[1] = vbNullString;
			//proprieta passata dal chiamante

			_with1.ClassificaFasc[0] = "<CLASSIFICA_FASCICOLO>";
			_with1.ClassificaFasc[1] = vbNullString;

			_with1.CodiceFasc[0] = "<CODICE_FASCICOLO>";
			_with1.CodiceFasc[1] = vbNullString;

			_with1.DescrizioneFasc[0] = "<DESCRIZIONE_FASCICOLO>";
			_with1.DescrizioneFasc[1] = vbNullString;

			_with1.DescrizioneFasc1[0] = "<DESCRIZIONE_FASCICOLO1>";
			_with1.DescrizioneFasc1[1] = vbNullString;

			_with1.DescrizioneFasc2[0] = "<DESCRIZIONE_FASCICOLO2>";
			_with1.DescrizioneFasc2[1] = vbNullString;

			_with1.DescrizioneFasc3[0] = "<DESCRIZIONE_FASCICOLO3>";
			_with1.DescrizioneFasc3[1] = vbNullString;

			_with1.DescrizioneFasc4[0] = "<DESCRIZIONE_FASCICOLO4>";
			_with1.DescrizioneFasc4[1] = vbNullString;




			//INIZIO Massimo digregorio 2005-02-24
			_with1.NumeroAllegati[0] = "<NUMERO_ALLEGATI>";
			_with1.NumeroAllegati[1] = vbNullString;

			_with1.NumeroDocumento[0] = "<NUMERO_DOCUMENTO>";
			_with1.NumeroDocumento[1] = vbNullString;
			//proprieta passata dal chiamante
			_with1.CodiceUoProtocollotore[0] = "<CODICE_UO_PROTOCOLLATORE>";
			_with1.CodiceUoProtocollotore[1] = vbNullString;
			//proprieta passata dal chiamante
			_with1.CodiceRegistroProtocollo[0] = "<CODICE_REGISTRO_PROTOCOLLO>";
			_with1.CodiceRegistroProtocollo[1] = vbNullString;
			//proprieta passata dal chiamante
			_with1.DescrizioneRegistroProtocollo[0] = "<DESCRIZIONE_REGISTRO_PROTOCOLLO>";
			_with1.DescrizioneRegistroProtocollo[1] = vbNullString;
			//proprieta passata dal chiamante
			_with1.TipoProtocollo[0] = "<TIPO_PROTOCOLLO>";
			_with1.TipoProtocollo[1] = vbNullString;
			//proprieta passata dal chiamante
			_with1.NumeroProtocollo[0] = "<NUMERO_PROTOCOLLO>";
			_with1.NumeroProtocollo[1] = vbNullString;
			//proprieta passata dal chiamante
			_with1.AnnoProtocollo[0] = "<ANNO_PROTOCOLLO>";
			_with1.AnnoProtocollo[1] = vbNullString;
			//proprieta passata dal chiamante
			_with1.DataProtocollo[0] = "<DATA_PROTOCOLLO>";
			_with1.DataProtocollo[1] = vbNullString;
			//proprieta passata dal chiamante
			//FINE Massimo digregorio 2005-02-24

			// INIZIO Documento grigio
			_with1.DataCreazione[0] = "<DATA_CREAZIONE>";
			_with1.DataCreazione[1] = vbNullString;

			_with1.CodiceUoCreatore[0] = "<CODICE_UO_CREATORE>";
			_with1.CodiceUoCreatore[1] = "";
			// FINE

			break;

		default:
			break;

	}

	functionReturnValue = true;
}
        catch(Exception e){
        
	functionReturnValue = false;
	throw new Exception (APPLICATION_NAME + "Errore in fase di caricamento della configurazione." + Environment.NewLine  + "dettaglio Errore: " + e.Message);
	}
    return functionReturnValue;

}


private string ReadKeyFromIniFile(string Key, string Group)
{
	string retValue = null;
	int Lenght = 0;
	//UPGRADE_NOTE: Buffer was upgraded to Buffer_Renamed. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
    String VBuffer_Renamed = new string(' ', 1024);

	 // ERROR: Not supported in C#: OnErrorStatement

    try
    {
        if (!string.IsNullOrEmpty(Group) & !string.IsNullOrEmpty(Key))
        {
            Lenght = GetPrivateProfileString(Group, Key, "", VBuffer_Renamed, BUFFER_SIZE, m_PathFile);

            retValue = VBuffer_Renamed.Substring(0, Lenght);
        }
        else
        {
            retValue = "";
        }
    }
    catch(Exception e){
		throw new Exception ("ReadKeyFromIniFile()" + "errore in fase di lettura della key:" + Key);
	}

	return retValue;
}


// Verifica se una data porta seriale COM è disponibile
private bool PortaComDisponibile(short portNum)
{
	bool functionReturnValue = false;
	short fnum = 0;
	 // ERROR: Not supported in C#: OnErrorStatement

    //fnum = FreeFile();
    //FileSystem.FileOpen(fnum, "COM" + Convert.ToString(portNum), OpenMode.Binary, , OpenShare.Shared);
    //if (Err().Number == 0) {
    //    FileSystem.FileClose(fnum);
    //    functionReturnValue = true;
    //}
	return functionReturnValue;
}


private void StampaScript(string scriptKey, ref string ret)
{

    //if (!m_blnConfigOk)
    //    Err().Raise(1401, "Stampa()", "Impossibile caricare la configurazione");

	// Reperimento dei metadati da stampare
	string scriptText = null;
	scriptText = ReadGroupKeysFromIniFile(scriptKey);

	if (scriptText.Equals(""))
		throw new System.Exception("Stampa() + Nessuno comando nel file Ini");

	scriptText = parseCommand(scriptText);

    //if (m_udtINIDeviceSetting.Device_Descrizione[1].ToLower().Equals ("TimbroCitec".ToLower())) {
    ////StampaTimbro scriptText
    //} else if (m_udtINIDeviceSetting.USBPrintMode[1].ToUpper().Equals("S")) {
    //    //Stampa mediante usb
    //    StampaUSB(scriptText);
    //} else {
    //    // Stampa porta com
    //    if ((!sendCommand(scriptText))) {
    //        throw new System.Exception("Stampa() + Stampa non riuscita");
    //    }
    //}
    ret = scriptText;
}

private void StampaUSB(string script)
{
    PrintDirectUsb printUsb = default(PrintDirectUsb);
    printUsb = new PrintDirectUsb();

    printUsb.PrintDirect(m_udtINIDeviceSetting.USBPrinterDeviceName[1], script);

    //UPGRADE_NOTE: Object printUsb may not be destroyed until it is garbage collected. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
    printUsb = null;
}

private Buffer.BufferConfigurationEnum getBuffer(string iniValue)
{
    Buffer.BufferConfigurationEnum retValue = default(Buffer.BufferConfigurationEnum);
    switch (iniValue.ToLower())
    {
        case "5x16":
            retValue = Buffer.BufferConfigurationEnum.Buffer_5x16;
            break;
        case ("5x32"):
            retValue = Buffer.BufferConfigurationEnum.Buffer_5x32;
            break;
        case ("10x16"):
            retValue = Buffer.BufferConfigurationEnum.Buffer_10x16;
            break;
        case ("10x32"):
            retValue = Buffer.BufferConfigurationEnum.Buffer_10x32;
            break;
        default:
            retValue = Buffer.BufferConfigurationEnum.Buffer_10x32;
            break;
    }
    return retValue;
}

private bool getBoolean(string value)
{
    bool retValue = false;
    if (!string.IsNullOrEmpty(value))
    {
        // ERROR: Not supported in C#: OnErrorStatement
        try
        {
            retValue = Convert.ToBoolean(value);
        }
        catch(Exception e){
            retValue = false;
        }
        
    }

    return retValue;
}

private string ReadGroupKeysFromIniFile(string Group)
{
    string retValue = null;
    int Lenght = 0;
    //UPGRADE_NOTE: Buffer was upgraded to Buffer_Renamed. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
    string Buffer_Renamed = "";

    // ERROR: Not supported in C#: OnErrorStatement

    try
    {
        if (!string.IsNullOrEmpty(Group))
        {
            Lenght = GetPrivateProfileSection(Group, buffer, BUFFER_SIZE, m_PathFile);
            retValue = System.Text.Encoding.GetEncoding(1252).GetString(buffer, 0, Lenght).TrimEnd((char)0);
        }
        else
        {
            retValue = "";
        }
    }
    catch (Exception e)
    {
        throw new Exception("ReadGroupKeysFromIniFile(), errore in fase di lettura del gruppo:" + Group + Environment.NewLine + "[" + e.Message + "]");
    }

    return retValue;
}

private string parseCommand(string sourceText)
{
    const int START = 1;
    // def la struttura Metatag
    string strDescReplace = null;
    short lunghezza_riga = 0;
    string[] tempStr = null;
    int i = 0;
    var _with1 = m_udtINIDeviceSetting;
    _with1.DataArrivo[1] = m_dataArrivo;
    _with1.DataArrivoEstesa[1] = m_dataArrivoEstesa;
    _with1.NumeroStampaCorrente[1] = m_NumeroStampaCorrente;
    //proprieta passata dal chiamante
    _with1.OraCreazione[1] = m_OraCreazione;
    _with1.Amministrazione[1] = m_Amministrazione;
    //proprieta passata dal chiamante
    _with1.testo[1] = m_Text;
    //proprieta passata dal chiamante
    _with1.NumCopie[1] = Convert.ToString(m_NumCopie);
    //proprieta passata dal chiamante

    //INIZIO __ Maria Pia Benevieri 03.05.2004
    _with1.Fascicolo[1] = m_Fascicolo;
    //proprieta passata dal chiamante
    _with1.Classifica[1] = m_Classifica;
    //proprieta passata dal chiamante
    _with1.Amministrazione_Etichetta[1] = m_Amministrazione_Etichetta;
    //proprieta passata dal chiamante
    //FINE __ Maria Pia Benevieri 03.05.2004
    //INIZIO Massimo digregorio 2005-02-24
    _with1.NumeroAllegati[1] = m_NumeroAllegati;
    //proprieta passata dal chiamante
    _with1.NumeroDocumento[1] = m_NumeroDocumento;
    //proprieta passata dal chiamante
    _with1.CodiceUoProtocollotore[1] = m_CodiceUoProtocollatore;
    //proprieta passata dal chiamante
    _with1.CodiceRegistroProtocollo[1] = m_CodiceRegistroProtocollo;
    //proprieta passata dal chiamante
    _with1.DescrizioneRegistroProtocollo[1] = m_DescrizioneRegistroProtocollo;
    //proprieta passata dal chiamante
    _with1.TipoProtocollo[1] = m_TipoProtocollo;
    //proprieta passata dal chiamante
    _with1.NumeroProtocollo[1] = m_NumeroProtocollo;
    //proprieta passata dal chiamante
    _with1.AnnoProtocollo[1] = m_AnnoProtocollo;
    //proprieta passata dal chiamante
    _with1.DataProtocollo[1] = m_DataProtocollo;
    //proprieta passata dal chiamante

    //stampa etichetta fascicolo
    ///''''''''''''''''''''''''''''''''''''''
    _with1.ClassificaFasc[1] = m_fasc_classifica.Replace( "\\", "\\\\");
    _with1.CodiceFasc[1] = m_fasc_codice.Replace( "\\", "\\\\");

    strDescReplace = m_fasc_descrizione.Replace("\\", "\\\\");


    if (!string.IsNullOrEmpty(_with1.PRINTLINENUMBER[1]))
    {
        lunghezza_riga = Convert.ToInt16(Convert.ToDouble(_with1.PRINTLINENUMBER[1]));
    }
    else
    {
        lunghezza_riga = 50;
    }


    if (strDescReplace.Length > 0)
    {
        tempStr = spezza_stringa(strDescReplace, lunghezza_riga);

        for (i = tempStr.GetLowerBound(0); i <= tempStr.GetUpperBound(0); i++)
        {
            if (i == 1)
            {
                _with1.DescrizioneFasc[1] = tempStr[i].ToUpper();
            }
            if (i == 2)
            {
                _with1.DescrizioneFasc1[1] = tempStr[i].ToUpper();
            }
            if (i == 3)
            {
                _with1.DescrizioneFasc2[1] = tempStr[i].ToUpper();
            }
            if (i == 4)
            {
                _with1.DescrizioneFasc3[1] = tempStr[i].ToUpper();
            }
            if (i == 5)
            {
                _with1.DescrizioneFasc4[1] = tempStr[i].ToUpper();
            }

        }
    }

    //  .DescrizioneFasc[1] = m_fasc_descrizione
    //  .DescrizioneFasc1[1] = m_fasc_descrizione1
    ///''''''''''''''''''''''''''''''''''''''


    //FINE Massimo digregorio 2005-02-24

    _with1.NumeroStampe[1] = m_NumeroStampe;
    //proprieta passata dal chiamante
    _with1.NumeroStampeEffettuate[1] = m_NumeroStampeEffettuate;
    //proprieta passata dal chiamante


    // INIZIO Documento grigio
    _with1.DataCreazione[1] = m_DataCreazione;
    _with1.CodiceUoCreatore[1] = m_codiceUoCreatore;
    // FINE

    if (sourceText.ToUpper().Substring(START).IndexOf(vbNullChar)>0)
        //Strings.InStr(START, Strings.UCase(sourceText), Constants.vbNullChar, CompareMethod.Binary) > 0)
    {
        sourceText = sourceText.Replace(vbNullChar.ToString(), _with1.CRLF[1]);
    }

    if (sourceText.ToUpper().IndexOf(_with1.BOLD[0], START,StringComparison.Ordinal) > 0)
    {
        sourceText = sourceText.Replace(_with1.BOLD[0], _with1.BOLD[1]);
    }

    if (sourceText.ToUpper().IndexOf( _with1.RESET_Renamed[0], START,StringComparison.Ordinal) > 0)
    {
        sourceText = sourceText.Replace(_with1.RESET_Renamed[0], _with1.RESET_Renamed[1]);
    }

    if (sourceText.ToUpper().IndexOf( _with1.HANDSHAKING[0], START,StringComparison.Ordinal) > 0)
    {
        sourceText = sourceText.Replace(_with1.HANDSHAKING[0], _with1.HANDSHAKING[1]);
    }

    if (sourceText.ToUpper().IndexOf( _with1.ITALIA[0], START,StringComparison.Ordinal) > 0)
    {
        sourceText = sourceText.Replace(_with1.ITALIA[0], _with1.ITALIA[1]);
    }

    if (sourceText.ToUpper().IndexOf( _with1.CRLF[0], START,StringComparison.Ordinal) > 0)
    {
        sourceText = sourceText.Replace(_with1.CRLF[0], _with1.CRLF[1]);
    }

    if (sourceText.ToUpper().IndexOf( _with1.CR[0], START,StringComparison.Ordinal) > 0)
    {
        sourceText = sourceText.Replace(_with1.CR[0], _with1.CR[1]);
    }

    if (sourceText.ToUpper().IndexOf( _with1.STX[0], START,StringComparison.Ordinal) > 0)
    {
        sourceText = sourceText.Replace(_with1.STX[0], _with1.STX[1]);
    }

    if (sourceText.ToUpper().IndexOf( _with1.Amministrazione[0], START,StringComparison.Ordinal) > 0)
    {
        sourceText = sourceText.Replace(_with1.Amministrazione[0], _with1.Amministrazione[1]);
    }

    if (sourceText.ToUpper().IndexOf( _with1.testo[0], START,StringComparison.Ordinal) > 0)
    {
        sourceText = sourceText.Replace(_with1.testo[0], _with1.testo[1]);
    }

    //INIZIO __ Maria Pia Benevieri 03.05.2004
    if (sourceText.ToUpper().IndexOf( _with1.Fascicolo[0], START,StringComparison.Ordinal) > 0)
    {
        sourceText = sourceText.Replace(_with1.Fascicolo[0], _with1.Fascicolo[1]);
    }

    if (sourceText.ToUpper().IndexOf( _with1.Classifica[0], START,StringComparison.Ordinal) > 0)
    {
        sourceText = sourceText.Replace(_with1.Classifica[0], _with1.Classifica[1]);
    }

    if (sourceText.ToUpper().IndexOf( _with1.Amministrazione_Etichetta[0], START,StringComparison.Ordinal) > 0)
    {
        sourceText = sourceText.Replace(_with1.Amministrazione_Etichetta[0], _with1.Amministrazione_Etichetta[1]);
    }
    //FINE __ Maria Pia Benevieri 03.05.2004

    //INIZIO Massimo digregorio 2005-02-24
    if (sourceText.ToUpper().IndexOf(_with1.NumeroAllegati[0], START, StringComparison.CurrentCulture) > 0)
    {
        sourceText = sourceText.Replace(_with1.NumeroAllegati[0], _with1.NumeroAllegati[1]);
    }
    if (sourceText.ToUpper().IndexOf( _with1.NumeroDocumento[0], START,StringComparison.Ordinal) > 0)
    {
        sourceText = sourceText.Replace(_with1.NumeroDocumento[0], _with1.NumeroDocumento[1]);
    }
    if (sourceText.ToUpper().IndexOf( _with1.CodiceUoProtocollotore[0], START,StringComparison.Ordinal) > 0)
    {
        sourceText = sourceText.Replace(_with1.CodiceUoProtocollotore[0], _with1.CodiceUoProtocollotore[1]);
    }
    if (sourceText.ToUpper().IndexOf( _with1.CodiceRegistroProtocollo[0], START,StringComparison.Ordinal) > 0)
    {
        sourceText = sourceText.Replace(_with1.CodiceRegistroProtocollo[0], _with1.CodiceRegistroProtocollo[1]);
    }
    if (sourceText.ToUpper().IndexOf( _with1.DescrizioneRegistroProtocollo[0], START,StringComparison.Ordinal) > 0)
    {
        sourceText = sourceText.Replace(_with1.DescrizioneRegistroProtocollo[0], _with1.DescrizioneRegistroProtocollo[1]);
    }
    if (sourceText.ToUpper().IndexOf( _with1.TipoProtocollo[0], START,StringComparison.Ordinal) > 0)
    {
        sourceText = sourceText.Replace(_with1.TipoProtocollo[0], _with1.TipoProtocollo[1]);
    }
    if (sourceText.ToUpper().IndexOf( _with1.NumeroProtocollo[0], START,StringComparison.Ordinal) > 0)
    {
        sourceText = sourceText.Replace(_with1.NumeroProtocollo[0], _with1.NumeroProtocollo[1]);
    }
    if (sourceText.ToUpper().IndexOf( _with1.AnnoProtocollo[0], START,StringComparison.Ordinal) > 0)
    {
        sourceText = sourceText.Replace(_with1.AnnoProtocollo[0], _with1.AnnoProtocollo[1]);
    }
    if (sourceText.ToUpper().IndexOf( _with1.DataProtocollo[0], START,StringComparison.Ordinal) > 0)
    {
        sourceText = sourceText.Replace(_with1.DataProtocollo[0], _with1.DataProtocollo[1]);
    }
    //FINE Massimo digregorio 2005-02-24

    // INIZIO Documento grigio
    if (sourceText.ToUpper().IndexOf( _with1.DataCreazione[0], START,StringComparison.Ordinal) > 0)
    {
        sourceText = sourceText.Replace(_with1.DataCreazione[0], _with1.DataCreazione[1]);
    }

    if (sourceText.ToUpper().IndexOf( _with1.CodiceUoCreatore[0], START,StringComparison.Ordinal) > 0)
    {
        sourceText = sourceText.Replace(_with1.CodiceUoCreatore[0], _with1.CodiceUoCreatore[1]);
    }
    //FINE
    //stampa etichetta fascicolo
    ///'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    if (sourceText.ToUpper().IndexOf( _with1.ClassificaFasc[0], START,StringComparison.Ordinal) > 0)
    {
        sourceText = sourceText.Replace(_with1.ClassificaFasc[0], _with1.ClassificaFasc[1]);
    }

    if (sourceText.ToUpper().IndexOf( _with1.CodiceFasc[0], START,StringComparison.Ordinal) > 0)
    {
        sourceText = sourceText.Replace(_with1.CodiceFasc[0], _with1.CodiceFasc[1]);
    }

    if (sourceText.ToUpper().IndexOf( _with1.DescrizioneFasc[0], START,StringComparison.Ordinal) > 0)
    {
        sourceText = sourceText.Replace(_with1.DescrizioneFasc[0], _with1.DescrizioneFasc[1]);
    }

    if (sourceText.ToUpper().IndexOf( _with1.DescrizioneFasc1[0], START,StringComparison.Ordinal) > 0)
    {
        sourceText = sourceText.Replace(_with1.DescrizioneFasc1[0], _with1.DescrizioneFasc1[1]);
    }

    if (sourceText.ToUpper().IndexOf( _with1.DescrizioneFasc2[0], START,StringComparison.Ordinal) > 0)
    {
        sourceText = sourceText.Replace(_with1.DescrizioneFasc2[0], _with1.DescrizioneFasc2[1]);
    }

    if (sourceText.ToUpper().IndexOf( _with1.DescrizioneFasc3[0], START,StringComparison.Ordinal) > 0)
    {
        sourceText = sourceText.Replace(_with1.DescrizioneFasc3[0], _with1.DescrizioneFasc3[1]);
    }

    if (sourceText.ToUpper().IndexOf( _with1.DescrizioneFasc4[0], START,StringComparison.Ordinal) > 0)
    {
        sourceText = sourceText.Replace(_with1.DescrizioneFasc4[0], _with1.DescrizioneFasc4[1]);
    }
    if (sourceText.ToUpper().IndexOf( _with1.NumeroStampaCorrente[0], START,StringComparison.Ordinal) > 0)
    {
        sourceText = sourceText.Replace(_with1.NumeroStampaCorrente[0], Convert.ToInt16(_with1.NumeroStampaCorrente[1]) + "");
        //incremento il valore di NumeroStampaCorrente
        _with1.NumeroStampaCorrente[1] = (Convert.ToInt16(_with1.NumeroStampaCorrente[1]) + 1) + "";
    }
    if (sourceText.ToUpper().IndexOf( _with1.OraCreazione[0], START,StringComparison.Ordinal) > 0)
    {
        sourceText = sourceText.Replace(_with1.OraCreazione[0], _with1.OraCreazione[1]);
    }
    if (sourceText.ToUpper().IndexOf( _with1.DataArrivo[0], START,StringComparison.Ordinal) > 0)
    {
        sourceText = sourceText.Replace(_with1.DataArrivo[0], _with1.DataArrivo[1]);
    }
    if (sourceText.ToUpper().IndexOf( _with1.DataArrivoEstesa[0], START,StringComparison.Ordinal) > 0)
    {
        sourceText = sourceText.Replace(_with1.DataArrivoEstesa[0], _with1.DataArrivoEstesa[1]);
    }



    return sourceText;

}


private string[] spezza_stringa(string testo, short nchar)
{


	int cont = 0;
	int i;
	int inizio = 1;
	int chr_finali = 0;
	string car = null;
	bool chr_superati = false;
	
	string[] array_testo = new string[1];

	
	for (i = 1; i <= testo.Length; i++) {
		
        car = testo.Substring(i, 1);
		
        cont = cont + 1;
		
        if (cont >= nchar)
			chr_superati = true;
		if (chr_superati & car == " ") {
			Array.Resize(ref array_testo, array_testo.GetUpperBound(0) + 2);
		
            chr_finali = testo.Substring(i).Length;
			
            array_testo[array_testo.GetUpperBound(0)] = testo.Substring(inizio, testo.Length - inizio - chr_finali + 1);
			
            inizio = i + 1;
			chr_superati = false;
			
            cont = 0;
		}
	}
	Array.Resize(ref array_testo, array_testo.GetUpperBound(0) + 2);
	
    array_testo[array_testo.GetUpperBound(0)] = testo.Substring(inizio);
	return array_testo;
}



private void UserControl_InitProperties()
{

    //Fascicolo
    m_fasc_classifica = m_fasc_classifica_def;
    m_fasc_codice = m_fasc_codice_def;
    m_fasc_descrizione = m_fasc_descrizione_def;
    m_fasc_descrizione1 = m_fasc_descrizione1_def;

    //  m_TimeOut = m_def_TimeOut
    //  m_TimeOut = m_def_TimeOut
    m_PortaCOM = m_def_PortaCOM;
    m_NumCopie = m_def_NumCopie;
    m_TimeOut = m_def_TimeOut;
    m_Dispositivo = m_def_Dispositivo;
    m_Amministrazione = m_def_Amministrazione;
    m_UrlFileIni = m_def_UrlFileIni;

    //INIZIO   __ Maria Pia Benevieri 03.05.2004 --> aggiunte init property per stampa su più righe dell'etichetta
    m_Fascicolo = m_def_Fascicolo;
    m_Classifica = m_def_Classifica;
    m_Amministrazione_Etichetta = m_def_Amministrazione_Etichetta;
    //FINE     __ Maria Pia Benevieri 03.05.2004

    //INIZIO Massimo digregorio 2005-02-24
    m_NumeroAllegati = m_def_NumeroAllegati;
    m_NumeroDocumento = m_def_NumeroDocumento;
    m_CodiceUoProtocollatore = m_def_CodiceUoProtocollotore;
    m_CodiceRegistroProtocollo = m_def_CodiceRegistroProtocollo;
    m_DescrizioneRegistroProtocollo = m_def_DescrizioneRegistroProtocollo;
    m_TipoProtocollo = m_def_TipoProtocollo;
    m_NumeroProtocollo = m_def_NumeroProtocollo;
    m_AnnoProtocollo = m_def_AnnoProtocollo;
    m_DataProtocollo = m_def_DataProtocollo;
    //FINE Massimo digregorio 2005-02-24

    m_NumeroStampe = m_def_NumeroStampe;
    m_NumeroStampeEffettuate = m_def_NumeroStampeEffettuate;
    m_NumeroStampaCorrente = m_def_NumeroStampaCorrente;
    m_OraCreazione = m_def_OraCreazione;
    m_stampaOK = Convert.ToBoolean(m_def_stampaOk);
    m_dataArrivo = m_def_DataArrivo;
    m_dataArrivoEstesa = m_def_DataArrivoEstesa;
    // INIZIO Documento grigio
    m_DataCreazione = m_def_DataCreazione;
    m_codiceUoCreatore = m_def_CodiceUoCreatore;
    // FINE

    m_Q1 = m_def_Q1;
    m_Q2 = m_def_Q2;
    m_P1 = m_def_P1;
    m_P2 = m_def_P2;
    m_P3 = m_def_P3;
    m_P4 = m_def_P4;
    m_P5 = m_def_P5;
    m_P6 = m_def_P6;
}

  /*  	Private Function StampaDymo() As Boolean
		On Error GoTo errorHandler
		
		Dim i As Short
		Dim NumPartenzaStampa As Object
		Dim totStampe As Double
		Dim retValue As Boolean
		'UPGRADE_NOTE: IDymoLabels was upgraded to IDymoLabels_Renamed. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim IDymoLabels_Renamed As Dymo.DymoLabels
		'UPGRADE_NOTE: IDymoAddIn was upgraded to IDymoAddIn_Renamed. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim IDymoAddIn_Renamed As Dymo.DymoAddIn
		If (DownLoadDymoFile()) Then
			
			'Dim IDymoAddIn As Dymo.DymoAddIn
			'Set IDymoAddIn = New Dymo.DymoAddIn
			'Dim IDymoLabels As Dymo.DymoLabels
			'Set IDymoLabels = New Dymo.DymoLabels
			'IDymoAddIn.Open2 (m_PathFileDymo)
			
			
			
			IDymoAddIn_Renamed = CreateObject("Dymo.DymoAddIn")
			IDymoLabels_Renamed = CreateObject("Dymo.DymoLabels")
			IDymoAddIn_Renamed.Open2(m_PathFileDymo)
			
			
			'<AMMINISTRAZIONE>
			IDymoLabels_Renamed.SetField("<AMMINISTRAZIONE>", m_Amministrazione)
			
			'<TESTO>
			IDymoLabels_Renamed.SetField("<TESTO>", m_Text)
			
			'<NUMERO_COPIE>
			IDymoLabels_Renamed.SetField("<NUMERO_COPIE>", CStr(m_NumCopie))
			
			'<FASCICOLO>
			IDymoLabels_Renamed.SetField("<FASCICOLO>", m_Fascicolo)
			
			'<CLASSIFICA>
			IDymoLabels_Renamed.SetField("<CLASSIFICA>", m_Classifica)
			
			'<AMMINISTRAZIONE_ETICHETTA>
			IDymoLabels_Renamed.SetField("<AMMINISTRAZIONE_ETICHETTA>", m_Amministrazione_Etichetta)
			
			'<NUMERO_ALLEGATI>
			IDymoLabels_Renamed.SetField("<NUMERO_ALLEGATI>", m_NumeroAllegati)
			
			'<NUMERO_DOCUMENTO>
			IDymoLabels_Renamed.SetField("<NUMERO_DOCUMENTO>", m_NumeroDocumento)
			
			'<CODICE_UO_PROTOCOLLATORE>
			IDymoLabels_Renamed.SetField("<CODICE_UO_PROTOCOLLATORE>", m_CodiceUoProtocollotore)
			
			'<CODICE_REGISTRO_PROTOCOLLO>
			IDymoLabels_Renamed.SetField("<CODICE_REGISTRO_PROTOCOLLO>", m_CodiceRegistroProtocollo)
			
			'<DESCRIZIONE_REGISTRO_PROTOCOLLO>
			IDymoLabels_Renamed.SetField("<DESCRIZIONE_REGISTRO_PROTOCOLLO>", m_DescrizioneRegistroProtocollo)
			
			'<TIPO_PROTOCOLLO>
			IDymoLabels_Renamed.SetField("<TIPO_PROTOCOLLO>", m_TipoProtocollo)
			
			'<NUMERO_PROTOCOLLO>
			IDymoLabels_Renamed.SetField("<NUMERO_PROTOCOLLO>", m_NumeroProtocollo)
			
			'<ANNO_PROTOCOLLO>
			IDymoLabels_Renamed.SetField("<ANNO_PROTOCOLLO>", m_AnnoProtocollo)
			
			'<DATA_PROTOCOLLO>
			IDymoLabels_Renamed.SetField("<DATA_PROTOCOLLO>", m_DataProtocollo)
			
			'<DATA_CREAZIONE>
			IDymoLabels_Renamed.SetField("<DATA_CREAZIONE>", m_DataCreazione)
			
			'<CODICE_UO_CREATORE>
			IDymoLabels_Renamed.SetField("<CODICE_UO_CREATORE>", m_codiceUoCreatore)
			
			'UPGRADE_WARNING: Couldn't resolve default property of object NumPartenzaStampa. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			NumPartenzaStampa = Val(m_NumeroStampeEffettuate) + 1
			
			'UPGRADE_WARNING: Couldn't resolve default property of object NumPartenzaStampa. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			totStampe = Val(m_NumeroStampe) + NumPartenzaStampa - 1
			
			'UPGRADE_WARNING: Couldn't resolve default property of object NumPartenzaStampa. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			For i = NumPartenzaStampa To totStampe
				
				'<NUM_STAMPA>
				IDymoLabels_Renamed.SetField("<NUM_STAMPA>", CStr(i))
				
				retValue = IDymoAddIn_Renamed.Print(1, True)
				
				If (Not StampaDymo) Then
					StampaDymo = retValue
				End If
			Next i
			
			'IDymoAddIn.EndPrintJob
			'UPGRADE_NOTE: Object IDymoAddIn_Renamed may not be destroyed until it is garbage collected. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
			IDymoAddIn_Renamed = Nothing
			'UPGRADE_NOTE: Object IDymoLabels_Renamed may not be destroyed until it is garbage collected. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
			IDymoLabels_Renamed = Nothing
			'Set q = Nothing
			
		Else
			Err.Raise(-1,  , "File di configurazione Dymo non scaricato")
		End If
		
		Exit Function
errorHandler: 
		StampaDymo = False
		
		Err.Raise(Err.Number, APPLICATION_NAME, "Errore in fase di stampa." & vbCrLf & "dettaglio Errore :" & Err.Description)
	End Function
	
	' Task di stampa etichetta con DYMO
	Private Function StampaDymoFasc() As Boolean
		On Error GoTo errorHandler
		
		Dim i As Short
		Dim retValue As Boolean
		'UPGRADE_NOTE: IDymoLabels was upgraded to IDymoLabels_Renamed. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim IDymoLabels_Renamed As Dymo.DymoLabels
		'UPGRADE_NOTE: IDymoAddIn was upgraded to IDymoAddIn_Renamed. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim IDymoAddIn_Renamed As Dymo.DymoAddIn
		If (DownLoadDymoFile()) Then
			
			
			
			IDymoAddIn_Renamed = CreateObject("Dymo.DymoAddIn")
			IDymoLabels_Renamed = CreateObject("Dymo.DymoLabels")
			IDymoAddIn_Renamed.Open2(m_PathFileDymo)
			
			
			'<AMMINISTRAZIONE>
			IDymoLabels_Renamed.SetField("<CLASSIFICA_FASCICOLO>", m_fasc_classifica)
			
			
			'<TESTO>
			IDymoLabels_Renamed.SetField("<CODICE_FASCICOLO>", m_fasc_codice)
			
			'<NUMERO_COPIE>
			IDymoLabels_Renamed.SetField("<DESCRIZIONE_FASCICOLO>", m_fasc_descrizione)
			
			
			
			retValue = IDymoAddIn_Renamed.Print(1, True)
			
			If (Not StampaDymoFasc) Then
				StampaDymoFasc = retValue
			End If
			
			
			'IDymoAddIn.EndPrintJob
			'UPGRADE_NOTE: Object IDymoAddIn_Renamed may not be destroyed until it is garbage collected. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
			IDymoAddIn_Renamed = Nothing
			'UPGRADE_NOTE: Object IDymoLabels_Renamed may not be destroyed until it is garbage collected. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
			IDymoLabels_Renamed = Nothing
			'Set q = Nothing
			
		Else
			Err.Raise(-1,  , "File di configurazione Dymo non scaricato")
		End If
		
		Exit Function
errorHandler: 
		StampaDymoFasc = False
		
		Err.Raise(Err.Number, APPLICATION_NAME, "Errore in fase di stampa." & vbCrLf & "dettaglio Errore :" & Err.Description)
	End Function
*/
        private Dictionary<string, string> getDymoFascMap()
        {
            Dictionary<string, string> dymoMap = new Dictionary<string, string>();

            dymoMap.Add("CLASSIFICA_FASCICOLO", m_fasc_classifica);

            dymoMap.Add("CODICE_FASCICOLO", m_fasc_codice);

            dymoMap.Add("DESCRIZIONE_FASCICOLO", m_fasc_descrizione);


            return dymoMap;
        }

        private Dictionary<string, string> getDymoMap()
        {
            Dictionary<string, string> dymoMap = new Dictionary<string, string>();

            dymoMap.Add("AMMINISTRAZIONE", m_Amministrazione);

            dymoMap.Add("TESTO", m_Text);

            dymoMap.Add("NUMERO_COPIE", m_NumCopie.ToString());

            dymoMap.Add("FASCICOLO", m_Fascicolo);

            dymoMap.Add("CLASSIFICA", m_Classifica);

            dymoMap.Add("AMMINISTRAZIONE_ETICHETTA", m_Amministrazione_Etichetta);

            dymoMap.Add("NUMERO_ALLEGATI", m_NumeroAllegati);

            dymoMap.Add("NUMERO_DOCUMENTO", m_NumeroDocumento);

            dymoMap.Add("CODICE_UO_PROTOCOLLATORE", m_CodiceUoProtocollatore);

            dymoMap.Add("CODICE_REGISTRO_PROTOCOLLO", m_CodiceRegistroProtocollo);

            dymoMap.Add("DESCRIZIONE_REGISTRO_PROTOCOLLO", m_DescrizioneRegistroProtocollo);

            dymoMap.Add("TIPO_PROTOCOLLO", m_TipoProtocollo);

            dymoMap.Add("NUMERO_PROTOCOLLO", m_NumeroProtocollo);

            dymoMap.Add("ANNO_PROTOCOLLO", m_AnnoProtocollo);

            dymoMap.Add("DATA_PROTOCOLLO", m_DataProtocollo);

            dymoMap.Add("DATA_CREAZIONE", m_DataCreazione);

            dymoMap.Add("CODICE_UO_CREATORE", m_codiceUoCreatore);

            dymoMap.Add("NUM_STAMPA", m_NumeroStampaCorrente);

            return dymoMap;
        }

    }
}
