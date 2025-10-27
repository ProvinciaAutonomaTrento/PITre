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
    public class GruppoDestinatarioModelloTrasmissione : DestinatarioModelloTrasmissione
    {
        public GruppoDestinatarioModelloTrasmissione(string id,
            string? codiceGruppo,
            TextValue? descrizioneGruppo,
            RagioneTrasmissione ragioneTrasmissione,
            TipiTrasmissioneSingolaEnum tipoTrasmissioneSingola,
            IReadOnlyList<DatiUtenteNotificato> utentiNotificati,
            TextValue? noteTrasmissioneSingola,
            int? giorniScadenza,
            bool? nascondiVersioniPrecedenti)
            : base(id, ragioneTrasmissione, noteTrasmissioneSingola, giorniScadenza, nascondiVersioniPrecedenti)
        {
            utentiNotificati = utentiNotificati ?? throw new ArgumentNullException(nameof(utentiNotificati));

            CodiceGruppo = codiceGruppo;
            DescrizioneGruppo = descrizioneGruppo;
            TipoTrasmissioneSingola = tipoTrasmissioneSingola;
            UtentiNotificati = utentiNotificati;
        }

        public string? CodiceGruppo { get; protected set; }

        public TextValue? DescrizioneGruppo { get; protected set; }

        public TipiTrasmissioneSingolaEnum TipoTrasmissioneSingola { get; protected set; }

        public virtual void ChangeTipoTrasmissioneSingola(TipiTrasmissioneSingolaEnum tipoTrasmissioneSingola)
        {
            TipoTrasmissioneSingola = tipoTrasmissioneSingola;
        }

        public IReadOnlyList<DatiUtenteNotificato> UtentiNotificati { get; protected set; }

        public virtual void ChangeUtentiNotificati(IReadOnlyList<DatiUtenteNotificato> utentiNotificati)
        {
            UtentiNotificati = utentiNotificati;
        }
    }
}
