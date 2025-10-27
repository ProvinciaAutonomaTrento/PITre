// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pi3.Infrastructure.ParER.Services.DigitalPreservation.ValueObjects;
using Refit;

namespace Pi3.Infrastructure.ParER.Services.Versamento
{
    public interface IVersamentoService
    {
        [Multipart]
        [Post("/VersamentoSync")]
        Task<EsitoVersamento> Send(
            [AliasAs("VERSIONE")] string version,
            [AliasAs("LOGINNAME")] string userName,
            [AliasAs("PASSWORD")] string password,
            [AliasAs("XMLSIP")] string index,
            IEnumerable<StreamPart> files
            );

        [Multipart]
        [Post("/RecDIPStatoConservazioneSync")]
        Task<StatoConservazione> Get(
            [AliasAs("VERSIONE")] string version,
            [AliasAs("LOGINNAME")] string userName,
            [AliasAs("PASSWORD")] string password,
            [AliasAs("XMLSIP")] string index
            );
    }
}
