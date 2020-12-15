using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Web.Services", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough]
	public class AddressbookGetRuoliSuperioriInUOCompletedEventArgs : AsyncCompletedEventArgs
	{
		private object[] results;

		public Ruolo[] Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (Ruolo[])this.results[0];
			}
		}

		internal AddressbookGetRuoliSuperioriInUOCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}
	}
}
