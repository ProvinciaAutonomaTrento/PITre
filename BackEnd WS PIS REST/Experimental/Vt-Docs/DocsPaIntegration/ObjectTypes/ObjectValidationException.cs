using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaIntegration.ObjectTypes
{
    public class ObjectValidationException : Exception
    {
        private string _errorMessage;

        public ObjectValidationException(string errorMessage)
        {
            this._errorMessage = errorMessage;
        }

        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
        }
    }
}
