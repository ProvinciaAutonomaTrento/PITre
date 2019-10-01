using StampaRegistri.Oggetti;
using StampaRegistri.Utils;
using System;
using System.Configuration;
using System.Text;

namespace StampaRegistri
{
	public class Configuration
	{
		private bool _Errore;

		private StringBuilder _DescErrore;

		private int _Docspa_Versione;

		private string _Docspa_UserName = string.Empty;

		private string _Docspa_PWD = string.Empty;

		private bool _Docspa_LoginForzata;

		private string _Docspa_IDAmm = string.Empty;

		private string _Docspa_Ruolo_IDCorr = string.Empty;

		private string _Docspa_IDRegistro = string.Empty;

		private int _Docspa_TimeoutRichiestaWSInMinuti;

		private string _GUI_AppTitle = "Docspa - stampa registro in modalità batch";

		private string _HistoryLog_PathFolder = string.Empty;

		private string _HistoryLog_FilePrefix = string.Empty;

		private string _Log_PathFolder = string.Empty;

		private string _Log_FileName = string.Empty;

		private Costanti.LivelliLog _Log_LevelTrace;

		private Costanti.DispositiviDiLog _Log_Device = Costanti.DispositiviDiLog.EventViewer;

		private string _Log_InEventViewer_LogName = "Docspa Log";

		private string _Work_ForzaChiusuraReg = string.Empty;

		private string _Work_ApriRegDopoProcesso = string.Empty;

		private bool _Work_ConfermaChiusuraDopoProcesso = true;

		private string _App_Name = "DocspaStampaRegistri";

		public bool Errore
		{
			get
			{
				return this._Errore;
			}
		}

		public string DescrizioneErrore
		{
			get
			{
				return this._DescErrore.ToString();
			}
		}

		public string Log_File
		{
			get
			{
				return FSO.GetPathCompletoFile(this._Log_PathFolder, this._Log_FileName);
			}
		}

		public string App_Name
		{
			get
			{
				return this._App_Name;
			}
		}

		public string Log_InEventViewer_LogName
		{
			get
			{
				return this._Log_InEventViewer_LogName;
			}
		}

		public string GUI_AppTitle
		{
			get
			{
				return this._GUI_AppTitle;
			}
		}

		public string HistoryLog_PathFolder
		{
			get
			{
				return this._HistoryLog_PathFolder;
			}
		}

		public string HistoryLog_FilePrefix
		{
			get
			{
				return this._HistoryLog_FilePrefix;
			}
		}

		public string Log_PathFolder
		{
			get
			{
				return this._Log_PathFolder;
			}
		}

		public string Log_FileName
		{
			get
			{
				return this._Log_FileName;
			}
		}

		public Costanti.LivelliLog Log_LevelTrace
		{
			get
			{
				return this._Log_LevelTrace;
			}
		}

		public Costanti.DispositiviDiLog Log_Device
		{
			get
			{
				return this._Log_Device;
			}
			set
			{
				this._Log_Device = value;
			}
		}

		public int Docspa_Versione
		{
			get
			{
				return this._Docspa_Versione;
			}
		}

		public string Docspa_UserName
		{
			get
			{
				return this._Docspa_UserName;
			}
		}

		public string Docspa_PWD
		{
			get
			{
				return this._Docspa_PWD;
			}
		}

		public bool Docspa_LoginForzata
		{
			get
			{
				return this._Docspa_LoginForzata;
			}
		}

		public string Docspa_IDAmm
		{
			get
			{
				return this._Docspa_IDAmm;
			}
		}

		public string Docspa_Ruolo_IDCorr
		{
			get
			{
				return this._Docspa_Ruolo_IDCorr;
			}
		}

		public string Docspa_IDRegistro
		{
			get
			{
				return this._Docspa_IDRegistro;
			}
		}

		public int Docspa_TimeoutRichiestaWSInMinuti
		{
			get
			{
				return this._Docspa_TimeoutRichiestaWSInMinuti;
			}
			set
			{
				this._Docspa_TimeoutRichiestaWSInMinuti = value;
			}
		}

		public string Work_ForzaChiusuraReg
		{
			get
			{
				return this._Work_ForzaChiusuraReg;
			}
		}

		public string Work_ApriRegDopoProcesso
		{
			get
			{
				return this._Work_ApriRegDopoProcesso;
			}
		}

		public bool Work_ConfermaChiusuraDopoProcesso
		{
			get
			{
				return this._Work_ConfermaChiusuraDopoProcesso;
			}
		}

		public Configuration()
		{
			try
			{
				this.LeggiDaConfig();
			}
			catch (Exception)
			{
			}
		}

		public void LeggiDaConfig()
		{
			try
			{
				this._Log_Device = (Costanti.DispositiviDiLog)int.Parse(this.leggiParamentroDaConfig(Costanti.ListaParametriConfig.Log_Device));
			}
			catch (Exception)
			{
			}
			try
			{
				this._Log_LevelTrace = (Costanti.LivelliLog)int.Parse(this.leggiParamentroDaConfig(Costanti.ListaParametriConfig.Log_LevelTrace));
			}
			catch (Exception)
			{
			}
			try
			{
				this._Log_FileName = this.leggiParamentroDaConfig(Costanti.ListaParametriConfig.Log_FileName);
			}
			catch (Exception)
			{
			}
			try
			{
				this._Log_PathFolder = FSO.VerificaECreaFolder(this.leggiParamentroDaConfig(Costanti.ListaParametriConfig.Log_PathFolder));
			}
			catch (Exception)
			{
			}
			try
			{
				this._HistoryLog_FilePrefix = this.leggiParamentroDaConfig(Costanti.ListaParametriConfig.HistoryLog_FileNamePrefix);
			}
			catch (Exception)
			{
			}
			try
			{
				this._HistoryLog_PathFolder = FSO.VerificaECreaFolder(this.leggiParamentroDaConfig(Costanti.ListaParametriConfig.HistoryLog_PathFolder));
			}
			catch (Exception)
			{
			}
			try
			{
				this._Docspa_Versione = (int)Convert.ToInt16(this.leggiParamentroDaConfig(Costanti.ListaParametriConfig.Docspa_Versione));
			}
			catch (Exception)
			{
			}
			try
			{
				this._Docspa_UserName = this.leggiParamentroDaConfig(Costanti.ListaParametriConfig.Docspa_UserName);
			}
			catch (Exception)
			{
			}
			try
			{
				this._Docspa_PWD = this.leggiParamentroDaConfig(Costanti.ListaParametriConfig.Docspa_PWD);
			}
			catch (Exception)
			{
			}
			try
			{
				this._Docspa_LoginForzata = Convert.ToBoolean(this.leggiParamentroDaConfig(Costanti.ListaParametriConfig.Docspa_LoginForzata));
			}
			catch (Exception)
			{
			}
			try
			{
				this._Docspa_IDAmm = this.leggiParamentroDaConfig(Costanti.ListaParametriConfig.Docspa_IDAmm);
			}
			catch (Exception)
			{
			}
			try
			{
				this._Docspa_Ruolo_IDCorr = this.leggiParamentroDaConfig(Costanti.ListaParametriConfig.Docspa_Ruolo_IDCorr);
			}
			catch (Exception)
			{
			}
			try
			{
				this._Docspa_IDRegistro = this.leggiParamentroDaConfig(Costanti.ListaParametriConfig.Docspa_IDRegistro);
			}
			catch (Exception)
			{
			}
			try
			{
				this._Work_ForzaChiusuraReg = this.leggiParamentroDaConfig(Costanti.ListaParametriConfig.Work_ForzaChiusuraReg);
			}
			catch (Exception)
			{
			}
			try
			{
				this._Work_ApriRegDopoProcesso = this.leggiParamentroDaConfig(Costanti.ListaParametriConfig.Work_ApriRegDopoProcesso);
			}
			catch (Exception)
			{
			}
			try
			{
				this._Work_ConfermaChiusuraDopoProcesso = Convert.ToBoolean(this.leggiParamentroDaConfig(Costanti.ListaParametriConfig.Work_ConfermaChiusuraDopoProcesso));
			}
			catch (Exception)
			{
			}
			try
			{
				this._Docspa_TimeoutRichiestaWSInMinuti = Convert.ToInt32(this.leggiParamentroDaConfig(Costanti.ListaParametriConfig.Docspa_TimeoutRichiesta_DocsPaWebService_InMinuti));
			}
			catch (Exception)
			{
			}
		}

		private string leggiParamentroDaConfig(Costanti.ListaParametriConfig param)
		{
			string text = null;
			try
			{
				text = ConfigurationSettings.AppSettings.Get(param.ToString());
				if (text == null)
				{
					text = null;
					this.addErrore(param.ToString());
				}
			}
			catch (Exception)
			{
			}
			return text;
		}

		private void addErrore(string nomeParametro)
		{
			this._Errore = true;
			if (this._DescErrore == null)
			{
				this._DescErrore = new StringBuilder("Errore in fase di caricamento dei seguenti parametri di configurazione:", 500);
			}
			this._DescErrore.AppendFormat("\n {0}", nomeParametro);
		}
	}
}
