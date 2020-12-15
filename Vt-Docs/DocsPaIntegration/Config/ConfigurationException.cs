using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaIntegration.Config
{
    public class ConfigurationException : Exception
    {
        private ConfigurationExceptionCode _code;
        private string _message;
        private List<ValidationError> _errors;

        public ConfigurationException(ConfigurationExceptionCode code, string message)
        {
            this._code = code;
            this._errors = new List<ValidationError>();
            this._message = message;
        }

        public ConfigurationException(ConfigurationExceptionCode code, string message, List<ValidationError> errors)
        {
            this._code = code;
            this._errors = errors;
            this._message = message;
        }

        public ConfigurationExceptionCode Code
        {
            get
            {
                return _code;
            }
        }

        public List<ValidationError> Errors
        {
            get
            {
                return _errors;
            }
        }

        public string ErrorMessage
        {
            get
            {
                return _message;
            }
        }

    }

    public enum ConfigurationExceptionCode
    {
        SYSTEM_ERROR,VALIDATION_ERROR
    }

    public class ValidationError
    {
        private ConfigurationParam _param;
        private ValidationErrorCode _code;
        private string _message;

        public ValidationError(ConfigurationParam param, ValidationErrorCode code)
        {
            this._param = param;
            this._code = code;
        }

        public ValidationError(ConfigurationParam param, ValidationErrorCode code,string message)
        {
            this._param = param;
            this._code = code;
            this._message = message;
        }

        public ConfigurationParam Param
        {
            get
            {
                return _param;
            }
        }

        public ValidationErrorCode Code
        {
            get
            {
                return _code;
            }
        }

        public string Message
        {
            get
            {
                return _message;
            }
        }

    }

    public enum ValidationErrorCode{
        MANDATORY_VALUE,NOT_VALID_VALUE
    }
}
