// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Pi3.Core.SeedWork
{
    public abstract class Pi3Exception : ApplicationException
    {
        #region Public Members

        public Pi3Exception()
        {
        }

        public Pi3Exception(string? message)
            : base(message)
        {
        }

        public Pi3Exception(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }

        public Pi3Exception(string message, System.Resources.ResourceManager resourceManager, params object[] messageParameters)
            : this(message, resourceManager, null, messageParameters)
        {
        }

        public Pi3Exception(string message, System.Resources.ResourceManager resourceManager, Exception? innerException, params object[] messageParameters)
            : base(string.Format(message, messageParameters), innerException)
        {
            this.MapError(message, resourceManager, messageParameters);
        }

        public string? ErrorCode 
        { 
            get
            {
                return this._errorCode;
            }
        }

        public override string Message
        {
            get
            {
                return this._message;
            }
        }

        public virtual HttpStatusCode StatusCode => HttpStatusCode.InternalServerError;

        #endregion

        #region Private Members

        protected string? _errorCode = null;
        protected string _message = null;
        private object destinatarioGiaPresente;
        private object value;
        private ResourceManager resourceManager;

        protected virtual void MapError(string message, System.Resources.ResourceManager resourceManager, params object[] messageParameters)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                this._errorCode = ErrorDescriptions.UnexpectedError.GetErrorCode();
                this._message = ErrorDescriptions.UnexpectedError;
            }
            else
            {
                this._errorCode = message.GetErrorCode(resourceManager);
                this._message = string.Format(message, messageParameters);
            }
        }

        #endregion
    }
}
