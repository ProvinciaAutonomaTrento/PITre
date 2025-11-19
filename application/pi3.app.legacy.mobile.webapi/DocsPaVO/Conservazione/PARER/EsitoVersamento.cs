// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocsPaVO.Conservazione.PARER
{
    public class EsitoVersamento
    {
        private readonly string _esito;
        private readonly string _codErrore;
        private readonly string _descErrore;
        private readonly bool _warning = false;
        private string _stato;

        public EsitoVersamento()
        {

        }

        public EsitoVersamento(string stato)
        {
            _stato = stato;
        }

        public EsitoVersamento(string esito, string codErrore, string descErrore)
        {
            _esito = esito;
            _codErrore= codErrore;
            _descErrore= descErrore;

            if(esito == "WARNING") { _warning = true; }

            this.setEsitoVersamento();
        }

        /// <summary>
        /// Esito generale restituito dal conservatore
        /// </summary>
        public string EsitoGenerale { get { return _esito; } }    

        /// <summary>
        /// Codice errore
        /// </summary>
        public string CodiceErrore { get { return _codErrore; } }

        /// <summary>
        /// Descrizione errore
        /// </summary>
        public string DescrizioneErrore { get { return _descErrore; } }

        /// <summary>
        /// Indica se il sistema di conservazione ha restituito un warning
        /// </summary>
        public bool Warning { get { return _warning; } }

        /// <summary>
        /// Stato del versamento in PITRE
        /// </summary>
        public string Stato
        {
            get { return _stato; }
            set { _stato = value; }
        }

        private void setEsitoVersamento()
        {
            if(_esito == "POSITIVO")
            {
                _stato = StatoVersamento.PRESO_IN_CARICO;
            }
            else if(_esito == "WARNING")
            {
                _stato = StatoVersamento.PRESO_IN_CARICO;
            }
            else if(_esito == "NEGATIVO")
            {
                _stato = StatoVersamento.RIFIUTATO;
            }

        }
    }
}
