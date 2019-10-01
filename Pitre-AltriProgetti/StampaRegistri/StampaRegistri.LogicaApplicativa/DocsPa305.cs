//using StampaRegistri.DocsPaWR305; //Luisa Antonelli 18112016 tolto riferimento a WS interno
using StampaRegistri.Oggetti;
using StampaRegistri.Utils;
using System;
using DocsPaWR; //Luisa Antonelli 18112016 aggiunto riferimento a WebService del  BackEnd 

namespace StampaRegistri.LogicaApplicativa
{
	public class DocsPa305 : Docspa
	{

        private DocsPaWebService WS;

        private InfoUtente myInfoUtente;

        private Ruolo myRuoloSelezionato;

        private Registro myRegistroSelezionato;

		private Configuration _conf;

		private bool _Errore;

		private Costanti.Errori _CodiceErrore;

		private bool _AutenticatoSuDocsPa;

		private string SessionIDDocsPa = "StampaRegistri";

		private string IpAddressClient = "???.???.???.???";

		public override bool Errore
		{
			get
			{
				return this._Errore;
			}
		}

		public override Costanti.Errori CodiceErrore
		{
			get
			{
				return this._CodiceErrore;
			}
		}

		public override bool AutenticatoSuDocsPa
		{
			get
			{
				return this._AutenticatoSuDocsPa;
			}
		}

		public DocsPa305(Configuration conf)
		{
			this._conf = conf;
			Trace.Traccia(207, "DocsPa305()", Costanti.TipoMessaggio.DEBUG, this._conf);
			this.resetErrore();
			try
			{
                this.WS = new DocsPaWebService(); 
				int num = 60000;
				int num2 = this._conf.Docspa_TimeoutRichiestaWSInMinuti * num;
				int num3 = 86400000;
				if (num2 > num3)
				{
					this._conf.Docspa_TimeoutRichiestaWSInMinuti = num3 - 1000;
				}
				if (num2 > this.WS.Timeout)
				{
					this.WS.Timeout = num2;
				}
			}
			catch (Exception ex)
			{
				this._Errore = true;
				this._CodiceErrore = Costanti.Errori.Docspa_WSNonRaggiungibile;
				Trace.Traccia(ex, this._conf);
			}
			finally
			{
				Trace.Traccia(208, "DocsPa305()", Costanti.TipoMessaggio.DEBUG, this._conf);
			}
		}

		public override bool Close()
		{
			Trace.Traccia(207, "Close()", Costanti.TipoMessaggio.DEBUG, this._conf);
			this.resetErrore();
			try
			{
				this.Logout();
				this.WS.Dispose();
			}
			catch (Exception ex)
			{
				this._Errore = true;
				this._CodiceErrore = Costanti.Errori.Docspa_LogoutNonRiuscita;
				Trace.Traccia(ex, this._conf);
			}
			finally
			{
				Trace.Traccia(208, "Close()", Costanti.TipoMessaggio.DEBUG, this._conf);
			}
			return true;
		}

		public override bool Logout()
		{
			Trace.Traccia(207, "Logout()", Costanti.TipoMessaggio.DEBUG, this._conf);
			this.resetErrore();
			try
			{
				if (this._AutenticatoSuDocsPa)
				{
					this.WS.Logoff(this.myInfoUtente.userId, this.myInfoUtente.idAmministrazione, this.SessionIDDocsPa, this.myInfoUtente.dst);
					this._AutenticatoSuDocsPa = false;
				}
			}
			catch (Exception ex)
			{
				this._Errore = true;
				this._CodiceErrore = Costanti.Errori.Docspa_LogoutNonRiuscita;
				Trace.Traccia(ex, this._conf);
			}
			finally
			{
				Trace.Traccia(208, "logout()", Costanti.TipoMessaggio.DEBUG, this._conf);
			}
			return true;
		}

		public override bool Login(string userName, string password, string idAmm, bool forzaLogin, string idRuolo)
		{
			Trace.Traccia(207, "Login()", Costanti.TipoMessaggio.DEBUG, this._conf);
			this.resetErrore();
			bool result;
			try
			{
				this._AutenticatoSuDocsPa = false;
                Utente utente = null; 
				this.IpAddressClient = null;
                UserLogin userLogin = new UserLogin(); 
				userLogin.UserName = userName;
				userLogin.Password = password;
				userLogin.IdAmministrazione = idAmm;
				userLogin.Update = forzaLogin;
                LoginResult loginResult; 
				try
				{
					loginResult = this.WS.Login(userLogin, forzaLogin, this.SessionIDDocsPa, out utente, out this.IpAddressClient);
				}
				catch (Exception ex)
				{
					this._Errore = true;
					this._CodiceErrore = Costanti.Errori.Docspa_LoginNonRiuscita;
					Trace.Traccia(ex, this._conf);
					result = false;
					return result;
				}
				switch (loginResult)
				{
                case LoginResult.OK:  
					this.resetErrore();
					break;
				case LoginResult.UNKNOWN_USER:  
					this._Errore = true;
					this._CodiceErrore = Costanti.Errori.Docspa_LoginNonRiuscita;
					Trace.Traccia(this._CodiceErrore, this._conf);
					break;
				case LoginResult.USER_ALREADY_LOGGED_IN:  
					this._Errore = true;
					this._CodiceErrore = Costanti.Errori.Docspa_LoginNonRiuscita;
					Trace.Traccia(this._CodiceErrore, this._conf);
					break;
				case LoginResult.APPLICATION_ERROR:  
					this._Errore = true;
					this._CodiceErrore = Costanti.Errori.Docspa_LoginNonRiuscita;
					Trace.Traccia(this._CodiceErrore, this._conf);
					break;
				default:
					this._Errore = true;
					this._CodiceErrore = Costanti.Errori.Docspa_LoginNonRiuscita;
					Trace.Traccia(this._CodiceErrore, this._conf);
					break;
				}
				if (this._Errore)
				{
					result = false;
				}
				else
				{
					try
					{
						this._AutenticatoSuDocsPa = this.getInfoUtente(utente, idRuolo);
						result = this._AutenticatoSuDocsPa;
					}
					catch (Exception ex2)
					{
						this._Errore = true;
						this._CodiceErrore = Costanti.Errori.Docspa_CaricamentoUtenteFallito;
						Trace.Traccia(ex2, this._conf);
						result = false;
					}
				}
			}
			catch (Exception ex3)
			{
				this._Errore = true;
				this._CodiceErrore = Costanti.Errori.Docspa_LoginNonRiuscita;
				Trace.Traccia(ex3, this._conf);
				result = false;
			}
			finally
			{
				Trace.Traccia(208, "Login()", Costanti.TipoMessaggio.DEBUG, this._conf);
			}
			return result;
		}

		public override bool GetRegistro(string idRegistro)
		{
			Trace.Traccia(207, "GetRegistro()", Costanti.TipoMessaggio.DEBUG, this._conf);
			this.resetErrore();
			bool result = false;
			try
			{
				result = this.getRegistro(this.myRuoloSelezionato, idRegistro);
			}
			catch (Exception ex)
			{
				this._Errore = true;
				this._CodiceErrore = Costanti.Errori.Docspa_CaricamentoRegistroFallito;
				Trace.Traccia(ex, this._conf);
				result = false;
			}
			finally
			{
				Trace.Traccia(208, "GetRegistro()", Costanti.TipoMessaggio.DEBUG, this._conf);
			}
			return result;
		}

		public override bool GetStatoRegistro(out string statoRegistro)
		{
			Trace.Traccia(207, "GetStatoRegistro()", Costanti.TipoMessaggio.DEBUG, this._conf);
			this.resetErrore();
			bool result = false;
			statoRegistro = "";
			try
			{
				statoRegistro = this.myRegistroSelezionato.stato;
				result = true;
			}
			catch (Exception ex)
			{
				this._Errore = true;
				this._CodiceErrore = Costanti.Errori.Docspa_CaricamentoRegistroFallito;
				Trace.Traccia(ex, this._conf);
				result = false;
			}
			finally
			{
				Trace.Traccia(208, "GetStatoRegistro()", Costanti.TipoMessaggio.DEBUG, this._conf);
			}
			return result;
		}

		public override bool CambiaStatoRegistro(string nuovoStatoReg)
		{
			Trace.Traccia(207, "CambiaStatoRegistro(" + nuovoStatoReg + ")", Costanti.TipoMessaggio.DEBUG, this._conf);
			this.resetErrore();
			bool result = false;
			try
			{
				this.myRegistroSelezionato = this.WS.RegistriCambiaStato(this.myInfoUtente, this.myRegistroSelezionato);
				if (this.myRegistroSelezionato.stato == nuovoStatoReg)
				{
					result = true;
				}
			}
			catch (Exception ex)
			{
				this._Errore = true;
				this._CodiceErrore = Costanti.Errori.Docspa_CambioStatoRegistroFallito;
				Trace.Traccia(ex, this._conf);
				result = false;
			}
			finally
			{
				Trace.Traccia(208, "CambiaStatoRegistro()", Costanti.TipoMessaggio.DEBUG, this._conf);
			}
			return result;
		}

		public override bool StampaRegistro(out string descReg, out string idDoc, out string descRuolo)
		{
			Trace.Traccia(207, "StampaRegistro()", Costanti.TipoMessaggio.DEBUG, this._conf);
			this.resetErrore();
			bool result = false;
			descReg = this.myRegistroSelezionato.descrizione;
			descRuolo = this.myRuoloSelezionato.descrizione;
			idDoc = "";
			try
			{
				
                StampaRegistroResult stampaRegistroResult = this.WS.RegistriStampa(this.myInfoUtente, this.myRuoloSelezionato, this.myRegistroSelezionato);
				
                if (stampaRegistroResult.errore != null && !stampaRegistroResult.errore.Equals(""))
				{
					this._Errore = true;
					this._CodiceErrore = Costanti.Errori.Docspa_StampaRegistroNonRiuscita;
					Trace.Traccia(stampaRegistroResult.errore, Costanti.TipoMessaggio.ERRORE, this._conf);
					result = false;
				}
				else
				{
					result = true;
					idDoc = stampaRegistroResult.docNumber;
				}
			}
			catch (Exception ex)
			{
				this._Errore = true;
				this._CodiceErrore = Costanti.Errori.Docspa_StampaRegistroNonRiuscita;
				Trace.Traccia(ex, this._conf);
				result = false;
			}
			finally
			{
				Trace.Traccia(208, "StampaRegistro()", Costanti.TipoMessaggio.DEBUG, this._conf);
			}
			return result;
		}

		private void resetErrore()
		{
			this._Errore = false;
			this._CodiceErrore = Costanti.Errori.NessunErrore;
		}

		private bool getRegistro(Ruolo ruolo) 
		{
			if (ruolo.registri.Length == 0)
			{
				this._Errore = true;
				this._CodiceErrore = Costanti.Errori.Docspa_RuoloNoHaRegistriAssociati;
				Trace.Traccia(this._CodiceErrore, this._conf);
				return false;
			}
			this.myRegistroSelezionato = ruolo.registri[0];
			return true;
		}

		private bool getRegistro(Ruolo ruolo, string idRegistro)  
		{
			if (idRegistro.Equals(string.Empty))
			{
				return this.getRegistro(ruolo);
			}
			if (ruolo.registri.Length == 0)
			{
				this._Errore = true;
				this._CodiceErrore = Costanti.Errori.Docspa_RuoloNoHaRegistriAssociati;
				return false;
			}
			this.myRegistroSelezionato = null;
			Registro[] registri = ruolo.registri;  //Luisa 18112016
			for (int i = 0; i < registri.Length; i++)
			{
                Registro registro = registri[i]; 
				if (registro.systemId == idRegistro)
				{
					this.myRegistroSelezionato = registro;
					break;
				}
			}
			if (this.myRegistroSelezionato == null)
			{
				this._Errore = true;
				this._CodiceErrore = Costanti.Errori.Docspa_Registro_NonValido;
				return false;
			}
			return true;
		}

		private bool getInfoUtente(Utente utente)  
		{
			if (utente.ruoli.Length == 0)
			{
				this._Errore = true;
				this._CodiceErrore = Costanti.Errori.Docspa_UtenteNoHaRuoliAssociati;
				Trace.Traccia(this._CodiceErrore, this._conf);
				return false;
			}
            Ruolo ruolo = utente.ruoli[0];  
            return this.getInfoUtente(utente, ruolo); 
		}

        private bool getInfoUtente(Utente utente, string idRuolo) 
		{
			if (idRuolo.Equals(string.Empty))
			{
				return this.getInfoUtente(utente);
			}
			if (utente.ruoli.Length == 0)
			{
				this._Errore = true;
				this._CodiceErrore = Costanti.Errori.Docspa_UtenteNoHaRuoliAssociati;
				Trace.Traccia(this._CodiceErrore, this._conf);
				return false;
			}
            Ruolo ruolo = null;  
            Ruolo[] ruoli = utente.ruoli;  
			for (int i = 0; i < ruoli.Length; i++)
			{
                Ruolo ruolo2 = ruoli[i]; 
				if (ruolo2.systemId == idRuolo)
				{
					ruolo = ruolo2;
					break;
				}
			}
			if (ruolo != null)
			{
				return this.getInfoUtente(utente, ruolo);
			}
			this._Errore = true;
			this._CodiceErrore = Costanti.Errori.Docspa_RuoloUtenteNonValido;
			Trace.Traccia(this._CodiceErrore, this._conf);
			return false;
		}

        private bool getInfoUtente(Utente utente, Ruolo ruolo)  
		{
            this.myInfoUtente = new InfoUtente(); 
			this.myInfoUtente.idCorrGlobali = ruolo.systemId;
			this.myInfoUtente.idPeople = utente.idPeople;
			this.myInfoUtente.idGruppo = ruolo.idGruppo;
			this.myInfoUtente.dst = utente.dst;
			this.myInfoUtente.idAmministrazione = utente.idAmministrazione;
			this.myInfoUtente.userId = utente.userId;
			this.myRuoloSelezionato = ruolo;
			return true;
		}
	}
}
