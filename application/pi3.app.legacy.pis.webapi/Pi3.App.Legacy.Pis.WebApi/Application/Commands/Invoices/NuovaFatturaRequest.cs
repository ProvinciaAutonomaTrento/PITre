// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Invoices
{
    public class NuovaFatturaRequest
    {
        /// <summary>
        /// Nel caso di protocollo specificare il registro
        /// </summary>
        public string CodeRegister
        {
            get;
            set;
        }

        /// <summary>
        /// Documento che si vuole creare
        /// </summary>
        public Document Document
        {
            get;
            set;
        }

        /// <summary>
        /// Codice dell'RF in cui si vuole protocollare (opzionale)
        /// </summary>
        public string CodeRF
        {
            get;
            set;
        }

        /// <summary>
        /// Codice del fascicolo nel quale fascicolare il documento, il codice prende soltanto i fascicoli nei titolari attivi
        /// </summary>
        public string CodeProject
        {
            get;
            set;
        }

        /// <summary>
        /// Id Sdi della fattura
        /// </summary>
        public string IdentificativoSdI
        {
            get;
            set;
        }

        /// <summary>
        /// Id del lotto del quale fa parte la fattura. Facoltativo.
        /// </summary>
        public string IdLottoPITre
        { get; set; }

        /// <summary>
        /// Ragione di trasmissione
        /// </summary>
        public string TransmissionReason
        {
            get;
            set;
        }

        /// <summary>
        /// Destinatario
        /// </summary>
        public Correspondent TransmissionReceiver
        {
            get;
            set;
        }

        /// <summary>
        /// Indica se la trasmissione viene inserita nella todoList.
        /// </summary>
        public bool TransmissionNotify
        {
            get;
            set;
        }

        /// <summary>
        /// Codice del registro
        /// </summary>
        public string TransmissionCodeReg
        {
            get;
            set;
        }

        /// <summary>
        /// Tipo Trasmissione. S= Uno, T=tutti
        /// </summary>
        public string TransmissionType
        {
            get;
            set;
        }


    }
}
