// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents
{
    public class SendingResult
    {
        public string CorrespondentId
        {
            get;
            set;
        }
        public string CorrespondentDescription
        {
            get;
            set;
        }
        public string Mail
        { get; set; }

        public string PrefChannel
        { get; set; }

        public string SendResult
        { get; set; }
    }
}
