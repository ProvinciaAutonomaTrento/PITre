// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents
{
    public class Field
    {
        /// <summary>
        /// System id del campo profilato
        /// </summary>
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Descrizione del campo profilato
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Indica se il campo è obbligatorio oppure no
        /// </summary>
        public bool Required
        {
            get;
            set;
        }

        /// <summary>
        /// Valore del campo profilato
        /// </summary>
        public string Value
        {
            get;
            set;
        }

        ///// <summary>
        ///// Valori multiplo nel caso delle caselle di selezione
        ///// </summary>
        //        //public List<string> MultipleChoiceList
        //{
        //    get;
        //    set;
        //}

        /// <summary>
        /// Valori multiplo nel caso delle caselle di selezione
        /// </summary>
        public string[] MultipleChoice
        {
            get;
            set;
        }

        /// <summary>
        /// Tipologia del campo profilato
        /// </summary>
        public string Type
        {
            get;
            set;
        }

        /// <summary>
        /// Se true indica al contatore che deve scattare
        /// </summary>
        public bool CounterToTrigger
        {
            get;
            set;
        }

        /// <summary>
        /// Id del registro o dell'RF del contatore da far scattare
        /// </summary>
        public string CodeRegisterOrRF
        {
            get;
            set;
        }

        /// <summary>
        /// Diritti che il ruolo dell'utente connesso ha sul campo.
        /// </summary>
        public string Rights
        {
            get;
            set;
        }
    }
}
