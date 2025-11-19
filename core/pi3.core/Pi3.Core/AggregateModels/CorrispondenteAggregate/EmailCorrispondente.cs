// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.CorrispondenteAggregate
{
    public class EmailCorrispondente : Entity<string>
    {
        #region Public Members

        internal EmailCorrispondente(string IdEmail, string indirizzoEmail, TextValue? note, bool principale = false)
        {
            this.Id = IdEmail;
            this.IndirizzoEmail = indirizzoEmail;
            this.Note = note;
            this.Principale = principale;
        }

        public string IndirizzoEmail { get; protected set; }

        public TextValue? Note { get; protected set; }

        public bool Principale { get; protected set; }

        internal void Modify(string indirizzoEmail, TextValue? note)
        {
            this.IndirizzoEmail = indirizzoEmail;
            this.Note = note;
        }

        internal void SetPrincipale(bool principale)
        {
            this.Principale = principale;
        }
        #endregion

        #region Private Members

        #endregion
    }
}
