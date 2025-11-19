// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.InteropPitre.WebApi.Application.Commands.InteroperabilityService
{
    public class InteropCorrespondent : ValueObject
    {
        public string Code { get; set;}

        public string Description { get; set;}

        public string IdGroup { get; set;}
    }

    public static class InteropFunctions
    {
        public const string CanReceivePrivateDocuments = "PRAUISP";
        public const string CanReceiveNonPrivateDocuments = "PRAUISNP";
    }

    public enum ManagementModeEnum
    {
        // Manual
        M,
        // Automatic
        A
    }

    public enum ErrorMessageEnum
    {
        None = 0,
        Error = 1
    }
}
