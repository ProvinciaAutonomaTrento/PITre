using System;
using DocsPaVO.utente;

namespace DocsPaVO.Interoperabilita
{
	/// <summary>
	/// Rappresentazione del singolo log del controllo della mail
	/// </summary>
	public class LogElementCheckMail
	{
		public DateTime LogDateTime=DateTime.Now;
		public string CodiceRegistro=string.Empty;
		public string MailRegistro=string.Empty;
		public MailAccountCheckResponse CheckResponse=null;

		public LogElementCheckMail()
		{
		}

		public LogElementCheckMail( Registro registro,
									MailAccountCheckResponse checkResponse)
		{
			this.CodiceRegistro=registro.codRegistro;
			this.MailRegistro=registro.email;
			this.CheckResponse=checkResponse;
		}

		public LogElementCheckMail( Registro registro,
			MailAccountCheckResponse checkResponse,
			DateTime logDateTime) : this(registro,checkResponse)
		{
			this.LogDateTime=logDateTime;
		}
	}
}
