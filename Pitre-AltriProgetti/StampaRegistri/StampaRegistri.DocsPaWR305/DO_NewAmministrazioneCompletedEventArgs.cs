using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Web.Services", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough]
	public class DO_NewAmministrazioneCompletedEventArgs : AsyncCompletedEventArgs
	{
		private object[] results;

		public PR_Amministrazione Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (PR_Amministrazione)this.results[0];
			}
		}

		internal DO_NewAmministrazioneCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}
	}
}
