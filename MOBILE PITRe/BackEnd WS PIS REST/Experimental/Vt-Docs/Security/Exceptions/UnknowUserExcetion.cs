using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Security.Core.Exceptions
{
    /// <summary>
    /// Eccezione utilizzata per indicare che un utente non è censito
    /// </summary>
    [Serializable]
    public class UnknowUserException : Exception
    {
        static String mex = "Utente {0} non censito.";

        public UnknowUserException() { }
        public UnknowUserException(string message) : base(String.Format(mex, message)) { }
        public UnknowUserException(string message, Exception inner) : base(String.Format(mex, message), inner) { }
        protected UnknowUserException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
