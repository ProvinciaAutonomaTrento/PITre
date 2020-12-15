using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Web.Services", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough]
	public class LoginCompletedEventArgs : AsyncCompletedEventArgs
	{
		private object[] results;

		public LoginResult Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (LoginResult)this.results[0];
			}
		}

		public Utente utente
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (Utente)this.results[1];
			}
		}

		public string ipAddress
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (string)this.results[2];
			}
		}

		internal LoginCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}
	}
}
