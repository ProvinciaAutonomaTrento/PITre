using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Web.Services", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough]
	public class GetRagioneTrasmissioneCompletedEventArgs : AsyncCompletedEventArgs
	{
		private object[] results;

		public RagioneTrasmissione Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (RagioneTrasmissione)this.results[0];
			}
		}

		public bool RagioniVerificate
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (bool)this.results[1];
			}
		}

		internal GetRagioneTrasmissioneCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}
	}
}
