// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pi3.Core.Contracts.DocumentoAmministrativo.Query;

public class Element : ValueObject
{
    public string Id { get; init; }

    public string IdTenant { get; init; }

    public string TypeName { get; init; }

    public TextValue Name { get; init; }

    public TextValue? Description { get; init; }

    public DateTime CreationDate { get; init; }
}
