// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.AggregateModels.ModelloTrasmissioneAggregate.ValueObjects;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.ModelloTrasmissioneAggregate.Entities
{
    public class UfficioDestinatarioModelloTrasmissione : DestinatarioModelloTrasmissione
    {
        public UfficioDestinatarioModelloTrasmissione(string id,
            string? codiceUfficio,
            TextValue? descrizioneUfficio,
            RagioneTrasmissione ragioneTrasmissione,
            TipiTrasmissioneSingolaEnum tipoTrasmissioneSingola,
            TextValue? noteTrasmissioneSingola,
            int? giorniScadenza,
            bool? nascondiVersioniPrecedenti)
            : base(id, ragioneTrasmissione, noteTrasmissioneSingola, giorniScadenza, nascondiVersioniPrecedenti)
        {
            CodiceUfficio = codiceUfficio;
            DescrizioneUfficio = descrizioneUfficio;
            TipoTrasmissioneSingola = tipoTrasmissioneSingola;
        }

        public string? CodiceUfficio { get; protected set; }

        public TextValue? DescrizioneUfficio { get; protected set; }

        public TipiTrasmissioneSingolaEnum TipoTrasmissioneSingola { get; protected set; }

        public virtual void ChangeTipoTrasmissioneSingola(TipiTrasmissioneSingolaEnum tipoTrasmissioneSingola)
        {
            TipoTrasmissioneSingola = tipoTrasmissioneSingola;
        }
    }
}
