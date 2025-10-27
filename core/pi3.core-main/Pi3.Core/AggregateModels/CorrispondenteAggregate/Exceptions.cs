// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.CorrispondenteAggregate
{
    public class EmailAddressNotValidPi3Exception : Pi3Exception
    {
        #region Public Members

        public EmailAddressNotValidPi3Exception(string emailAddress)
            : base(ErrorDescriptions.EmailAddressNotValid, ErrorDescriptions.ResourceManager, emailAddress)
        {
            this.EmailAddress = emailAddress;
        }

        public string EmailAddress { get; init; }

        #endregion
    }

    public class CorrispondenteStoricizzatoPi3Exception : Pi3Exception
    {
        #region Public Members

        public CorrispondenteStoricizzatoPi3Exception()
            : base(ErrorDescriptions.CorrispondenteStoricizzato, ErrorDescriptions.ResourceManager)
        {
        }

        #endregion
    }

    public class ChangeNameCorrispondenteNonSupportatoPi3Exception : Pi3Exception
    {
        #region Public Members

        public ChangeNameCorrispondenteNonSupportatoPi3Exception()
            : base(ErrorDescriptions.ChangeNameCorrispondenteNonSupportato, ErrorDescriptions.ResourceManager)
        {
        }

        #endregion
    }

    public class EmailNotBeRemovedPi3Exception : Pi3Exception
    {
        #region Public Members

        public EmailNotBeRemovedPi3Exception()
            : base(ErrorDescriptions.EmailNotBeRemoved, ErrorDescriptions.ResourceManager)
        {
        }

        #endregion
    }

    public class EmailCorrispondenteRequredPi3Exception : Pi3Exception
    {
        #region Public Members

        public EmailCorrispondenteRequredPi3Exception()
            : base(ErrorDescriptions.EmailCorrispondenteRequred, ErrorDescriptions.ResourceManager)
        {
        }

        #endregion
    }

    public class CodiceAOORequredPi3Exception : Pi3Exception
    {
        #region Public Members

        public CodiceAOORequredPi3Exception()
            : base(ErrorDescriptions.CodiceAOORequred, ErrorDescriptions.ResourceManager)
        {
        }

        #endregion
    }

    public class CodiceAmministrazioneRequredPi3Exception : Pi3Exception
    {
        #region Public Members

        public CodiceAmministrazioneRequredPi3Exception()
            : base(ErrorDescriptions.CodiceAmministrazioneRequred, ErrorDescriptions.ResourceManager)
        {
        }

        #endregion
    }

    public class NameRequredPi3Exception : Pi3Exception
    {
        #region Public Members

        public NameRequredPi3Exception()
            : base(ErrorDescriptions.NameRequired, ErrorDescriptions.ResourceManager)
        {
        }

        #endregion
    }

    public class DescriptionRequredPi3Exception : Pi3Exception
    {
        #region Public Members

        public DescriptionRequredPi3Exception()
            : base(ErrorDescriptions.DescriptionRequired, ErrorDescriptions.ResourceManager)
        {
        }

        #endregion
    }
}
