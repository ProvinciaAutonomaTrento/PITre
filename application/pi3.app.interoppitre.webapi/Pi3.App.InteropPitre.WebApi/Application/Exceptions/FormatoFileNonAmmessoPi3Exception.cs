// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.App.InteropPitre.WebApi.Resources;
using Pi3.Core.SeedWork;

namespace Pi3.App.InteropPitre.WebApi.Application.Exceptions
{
    public class FormatoFileNonAmmessoPi3Exception : Pi3Exception
    {
        public FormatoFileNonAmmessoPi3Exception(string ext)
            : base(ErrorDescriptions.FormatoFileNonAmmesso, ErrorDescriptions.ResourceManager, ext)
        {
        }
    }
}
