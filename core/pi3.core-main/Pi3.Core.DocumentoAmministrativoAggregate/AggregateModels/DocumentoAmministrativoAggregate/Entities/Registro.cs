// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Entities
{
    public class Registro : Entity<string>
    {
        #region Public Members

        public Registro(string id, string? codice = null, TextValue? descrizione = null)
        {
            this.Id = id;
            this.Codice = codice;
            this.Descrizione = descrizione;
        }

        [RegularExpression("[A-Za-z0-9_\\.\\-]{1,16}")]
        public string? Codice { get; protected set; } = null;

        public TextValue? Descrizione { get; protected set; } = null;

        #endregion

        #region Private Members

        #endregion
    }

}
