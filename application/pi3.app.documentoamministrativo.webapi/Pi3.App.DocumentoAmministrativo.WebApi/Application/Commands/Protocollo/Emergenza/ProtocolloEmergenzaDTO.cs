// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Protocollo.Emergenza;

public class ProtocolloEmergenzaDTO
{
    public string? Segnatura { get; init; }

    public DateTime? Data { get; init; }

    public string Cognome { get; init; }
    public string Nome { get; init; }
}
