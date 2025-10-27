// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.RuoloCorrispondenteAggregate.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.AggregateModels.RuoloCorrispondenteAggregate.Exceptions
{
    public class MailCorrispondentePresentePi3Exception : Pi3Exception
    {
        #region Public Members

        public MailCorrispondentePresentePi3Exception(string mail)
            : base(ErrorDescriptions.MailCorrispondentePresente, null, ErrorDescriptions.ResourceManager, mail)
        {
            Mail = mail;
        }

        public string Mail { get; init; }

        #endregion
    }
}
