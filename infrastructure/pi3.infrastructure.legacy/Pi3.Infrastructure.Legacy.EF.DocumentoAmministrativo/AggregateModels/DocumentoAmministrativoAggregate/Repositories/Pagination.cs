// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories
{
    public class Pagination : ValueObject
    {
        public Pagination()
        { }

        public int? Skip { get; set; } = 0;

        public int? Take { get; set; } = 5000;

        public static Pagination Default
        {
            get
            {
                return new Pagination();
            }
        }
    }

}
