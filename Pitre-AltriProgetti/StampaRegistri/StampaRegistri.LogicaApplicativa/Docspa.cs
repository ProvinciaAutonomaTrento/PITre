using StampaRegistri.Oggetti;
using System;

namespace StampaRegistri.LogicaApplicativa
{
	public abstract class Docspa
	{
		public virtual bool Errore
		{
			get
			{
				return false;
			}
		}

		public virtual Costanti.Errori CodiceErrore
		{
			get
			{
				return Costanti.Errori.NessunErrore;
			}
		}

		public virtual bool AutenticatoSuDocsPa
		{
			get
			{
				return false;
			}
		}

		public abstract bool Login(string userName, string password, string idAmm, bool forzaLogin, string idRuolo);

		public abstract bool Logout();

		public abstract bool Close();

		public abstract bool GetRegistro(string idRegistro);

		public abstract bool GetStatoRegistro(out string statoReg);

		public abstract bool CambiaStatoRegistro(string nuovoStatoReg);

		public abstract bool StampaRegistro(out string descReg, out string idDoc, out string descRuolo);
	}
}
