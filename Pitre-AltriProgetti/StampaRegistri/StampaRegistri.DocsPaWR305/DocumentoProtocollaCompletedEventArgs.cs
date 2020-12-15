using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Web.Services", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough]
	public class DocumentoProtocollaCompletedEventArgs : AsyncCompletedEventArgs
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

		public ResultProtocollazione risultatoProtocollazione
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (ResultProtocollazione)this.results[1];
			}
		}

		internal DocumentoProtocollaCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}
	}
}
