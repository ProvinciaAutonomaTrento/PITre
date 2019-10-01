using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace StampaRegistri.DocsPaWR25
{
	[GeneratedCode("System.Web.Services", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough]
	public class documentoGetQueryDocumentoPagingCompletedEventArgs : AsyncCompletedEventArgs
	{
		private object[] results;

		public InfoDocumento[] Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (InfoDocumento[])this.results[0];
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

		public int risultTot
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (int)this.results[2];
			}
		}

		internal documentoGetQueryDocumentoPagingCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}
	}
}
