// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.utente;
using DocsPaVO.Validations;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DocsPaVO.utente.UserLogin;

namespace Pi3.App.Legacy.WebApi.Application.Requests
{
    public record LoginResult(DocsPaVO.utente.UserLogin.LoginResult output, Utente utente, string ipAddress);

    public record Login(UserLogin login, bool forced, string webSessionId) : IRequest<LoginResult>;

    public record UserChangePasswordResult(ValidationResultInfo output);

    public record UserChangePassword(UserLogin user, string oldPassword) : IRequest<UserChangePasswordResult>;

    public record amministrazioneGetAmministrazioniByUserResult(Amministrazione[] output, string returnMsg);

    public record amministrazioneGetAmministrazioniByUser(string userId, bool controllo) : IRequest<amministrazioneGetAmministrazioniByUserResult>;

    public record ModificaPasswordUtenteMultiAmmResult(bool output);

    public record ModificaPasswordUtenteMultiAmm(string userId, string idAmm) : IRequest<ModificaPasswordUtenteMultiAmmResult>;
}
