using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace StampaRegistri.DocsPaWR25
{
	[GeneratedCode("System.Web.Services", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough]
	public class fascicolazioneGetListaFascicoliPagingCompletedEventArgs : AsyncCompletedEventArgs
	{
		private object[] results;

		public Fascicolo[] Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (Fascicolo[])this.results[0];
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

		public int rTot
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (int)this.results[2];
			}
		}

		internal fascicolazioneGetListaFascicoliPagingCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}
	}
}
