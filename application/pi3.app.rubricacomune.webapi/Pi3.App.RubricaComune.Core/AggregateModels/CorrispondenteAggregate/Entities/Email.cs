// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.RubricaComune.Core.AggregateModels.CorrispondenteAggregate.Entities
{

    public class Email : Entity<string>
    {
        #region Public Members

        internal Email(string id, bool? preferita = false, string? note = null)
        {
            id = id ?? throw new ArgumentNullException(nameof(id));
            
            this.Id = id;
            this.Preferita = preferita ?? false;
            this.Note = note;
        }

        public bool Preferita { get; protected set; }

        public string? Note { get; protected set; }

        public void ChangePreferita(bool? newPreferita)
        {
            this.Preferita = newPreferita ?? false;
        }

        public void ChangeNote(string? newNote)
        {
            this.Note = newNote;
        }

        #endregion

        #region Private Members

        #endregion
    }

}
