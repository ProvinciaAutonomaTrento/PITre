using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Web.Services", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough]
	public class DocumentoGetDettaglioDocumentoCompletedEventArgs : AsyncCompletedEventArgs
	{
		private object[] results;

		public SchedaDocumento Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (SchedaDocumento)this.results[0];
			}
		}

		internal DocumentoGetDettaglioDocumentoCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}
	}
}
