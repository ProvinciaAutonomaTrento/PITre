using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Permissions;
using System.Runtime.Serialization;
using Rubrica.Library.Data;

namespace Rubrica.Library
{
    /// <summary>
    /// Classe generica che rappresenta un'eccezione verificatasi nel sistema rubrica
    /// </summary>
    [Serializable]
    public class RubricaException : ApplicationException
    {
        #region Constructors

		/// <summary>
		/// Constructor with no params.
		/// </summary>
		public RubricaException() : base()
		{
		}
		/// <summary>
		/// Constructor allowing the Message property to be set.
		/// </summary>
		/// <param name="message">String setting the message of the exception.</param>
		public RubricaException(string message) : base(message) 
		{
		}
		/// <summary>
		/// Constructor allowing the Message and InnerException property to be set.
		/// </summary>
		/// <param name="message">String setting the message of the exception.</param>
		/// <param name="inner">Sets a reference to the InnerException.</param>
		public RubricaException(string message,Exception inner) : base(message, inner)
		{
		}

		/// <summary>
		/// Constructor used for deserialization of the exception class.
		/// </summary>
		/// <param name="info">Represents the SerializationInfo of the exception.</param>
		/// <param name="context">Represents the context information of the exception.</param>
        protected RubricaException(SerializationInfo info, StreamingContext context)
            : base(info, context)
		{
            // this.Utente = info.GetString("utente");
		}

		#endregion

		/// <summary>
		/// Override the GetObjectData method to serialize custom values.
		/// </summary>
		/// <param name="info">Represents the SerializationInfo of the exception.</param>
		/// <param name="context">Represents the context information of the exception.</param>
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData( SerializationInfo info, StreamingContext context ) 
		{
            // info.AddValue("utente", this.Utente, typeof(string));

			base.GetObjectData(info,context);
		}

		#region Public Properties

		/// <summary>
		/// 
		/// </summary>
        public string Utente
        {
            get
            {
                if (Security.SecurityHelper.AuthenticatedPrincipal != null && Security.SecurityHelper.AuthenticatedPrincipal.Identity != null)
                    return Security.SecurityHelper.AuthenticatedPrincipal.Identity.Name;
                else
                    return string.Empty;
            }
        }

		#endregion
    }

    /// <summary>
    /// Classe che rappresenta un'eccezione verificatasi nelle ricerche del sistema rubrica
    /// </summary>
    [Serializable]
    public class RubricaSearchException : RubricaException
    {
        #region Constructors

		/// <summary>
		/// Constructor with no params.
		/// </summary>
		public RubricaSearchException() : base()
		{
		}
		/// <summary>
		/// Constructor allowing the Message property to be set.
		/// </summary>
		/// <param name="message">String setting the message of the exception.</param>
		public RubricaSearchException(string message) : base(message) 
		{
		}
		/// <summary>
		/// Constructor allowing the Message and InnerException property to be set.
		/// </summary>
		/// <param name="message">String setting the message of the exception.</param>
		/// <param name="inner">Sets a reference to the InnerException.</param>
        public RubricaSearchException(string message, Exception inner)
            : base(message, inner)
		{
		}

		/// <summary>
		/// Constructor used for deserialization of the exception class.
		/// </summary>
		/// <param name="info">Represents the SerializationInfo of the exception.</param>
		/// <param name="context">Represents the context information of the exception.</param>
        protected RubricaSearchException(SerializationInfo info, StreamingContext context)
            : base(info, context)
		{
            this.OpzioniRicerca = info.GetValue("opzioniRicerca", typeof(OpzioniRicerca)) as OpzioniRicerca;
		}

		#endregion

		/// <summary>
		/// Override the GetObjectData method to serialize custom values.
		/// </summary>
		/// <param name="info">Represents the SerializationInfo of the exception.</param>
		/// <param name="context">Represents the context information of the exception.</param>
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData( SerializationInfo info, StreamingContext context ) 
		{
            info.AddValue("opzioniRicerca", this.OpzioniRicerca, typeof(OpzioniRicerca));
            
			base.GetObjectData(info,context);
		}

		#region Public Properties

        /// <summary>
        /// Riporta le opzioni di ricerca specificate
        /// </summary>
        public OpzioniRicerca OpzioniRicerca
        {
            get;
            set;
        }

		#endregion
    }
}