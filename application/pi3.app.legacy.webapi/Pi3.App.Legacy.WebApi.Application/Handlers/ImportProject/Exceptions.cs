// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocumentFormat.OpenXml.EMMA;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ImportProject
{
    public class SheetNotFoundPi3Exception : NotFoundPi3Exception
    {
        public SheetNotFoundPi3Exception()
            : base(ErrorDescription.SheetNotFound, null, ErrorDescription.ResourceManager)
        {
            
        }
    }

    public class AdminNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public AdminNotFoundPi3Exception(string adminCode)
            : base(ErrorDescription.AdminNotFoundError, ErrorDescription.ResourceManager, adminCode)
        {
            this._adminCode = adminCode;
        }

        public string _adminCode { get; set; }

        #endregion
    }

    public class TitolarioNodeNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public TitolarioNodeNotFoundPi3Exception(string node)
            : base(ErrorDescription.TitNodeNotFoundError, ErrorDescription.ResourceManager, node)
        {
            this._node = node;
        }

        public string _node { get; set; }

        #endregion
    }

    public class TitolarioNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public TitolarioNotFoundPi3Exception()
            : base(ErrorDescription.TitolarioNotFoundError, ErrorDescription.ResourceManager)
        {
        }

        #endregion
    }

    public class RegisterNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public RegisterNotFoundPi3Exception(string code)
            : base(ErrorDescription.RegisterNotFoundError, ErrorDescription.ResourceManager, code)
        {
            this._code = code;
        }

        public string _code { get; set; }

        #endregion
    }

    public class PianoConservazioneNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public PianoConservazioneNotFoundPi3Exception(string tipologiaFascicolo)
            : base(ErrorDescription.PianoConsNotFoundError, ErrorDescription.ResourceManager, tipologiaFascicolo)
        {
            this._tipologiaFascicolo = tipologiaFascicolo;
        }

        public string _tipologiaFascicolo { get; set; }

        #endregion
    }
    public class CorrespondentNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public CorrespondentNotFoundPi3Exception(string code)
            : base(ErrorDescription.CorrespondentNotFoundError, ErrorDescription.ResourceManager, code)
        {
            this._code = code;
        }

        public string _code { get; set; }

        #endregion
    }
    public class ProjectNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public ProjectNotFoundPi3Exception(string code)
            : base(ErrorDescription.ProjectNotFoundError, ErrorDescription.ResourceManager, code)
        {
            this._code = code;
        }

        public string _code { get; set; }

        #endregion
    }
    public class DocumentNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public DocumentNotFoundPi3Exception(string docnumber)
            : base(ErrorDescription.DocumentNotFoundError, ErrorDescription.ResourceManager, docnumber)
        {
            this._docnumber = docnumber;
        }

        public string _docnumber { get; set; }

        #endregion
    }
    public class TemplateNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public TemplateNotFoundPi3Exception(string tipologiaFascicolo)
            : base(ErrorDescription.TemplateNotFoundError, ErrorDescription.ResourceManager, tipologiaFascicolo)
        {
            this._tipologiaFascicolo = tipologiaFascicolo;
        }

        public string _tipologiaFascicolo { get; set; }

        #endregion
    }
    public class ValueNotValidPi3Exception : Pi3Exception
    {
        #region Public Members

        public ValueNotValidPi3Exception(string value)
            : base(ErrorDescription.ValueNotValid, ErrorDescription.ResourceManager, value)
        {
            this._value = value;
        }

        public string _value { get; set; }

        #endregion
    }

    public class GeneralImportException : Pi3Exception
    {
        #region Public Members

        public GeneralImportException(string message)
            : base(message)
        {
        }

        #endregion
    }
}
