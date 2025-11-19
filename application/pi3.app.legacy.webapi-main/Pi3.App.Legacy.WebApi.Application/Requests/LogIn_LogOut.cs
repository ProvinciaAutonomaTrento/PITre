// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.amministrazione;
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
    public record clearHashTableChiaviConfig(string idAmm) : IRequest;
    public record RefreshQueryListResult(bool output);

    public record RefreshQueryList(string password) : IRequest<RefreshQueryListResult>;

    public record getListaChiaviConfigResult(ChiaveConfigurazione[] output);

    public record getListaChiaviConfig(string idAmm) : IRequest<getListaChiaviConfigResult>;
    public record LogoffResult(bool output);

    public record Logoff(string userId, string idAmm, string sessionId, string dst) : IRequest<LogoffResult>;
    public record GestioneMessaggioLoginResult(string output);

    public record GestioneMessaggioLogin(string message, string password) : IRequest<GestioneMessaggioLoginResult>;
    public record ResetPasswordUtenteResult(ValidationResultInfo output);

    public record ResetPasswordUtente(UserLogin userLogin, string otp) : IRequest<ResetPasswordUtenteResult>;
    public record ResetPasswordInviaOTPResult(ResetPasswordResult output, string email);

    public record ResetPasswordInviaOTP(string userId) : IRequest<ResetPasswordInviaOTPResult>;
    public record CheckConnectionResult(bool output, string exceptionMessage);

    public record CheckConnection() : IRequest<CheckConnectionResult>;
}
