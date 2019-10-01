using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace StampaRegistri.DocsPaWR25
{
	[GeneratedCode("System.Web.Services", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough]
	public class trasmissioneGetQueryRicevutePagingCompletedEventArgs : AsyncCompletedEventArgs
	{
		private object[] results;

		public Trasmissione[] Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (Trasmissione[])this.results[0];
			}
		}

		public int risultati
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (int)this.results[1];
			}
		}

		internal trasmissioneGetQueryRicevutePagingCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}
	}
}