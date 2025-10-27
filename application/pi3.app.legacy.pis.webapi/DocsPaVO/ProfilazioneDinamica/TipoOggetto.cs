// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;

namespace DocsPaVO.ProfilazioneDinamica
{
    [Serializable()]
	public class TipoOggetto
	{
        public int SYSTEM_ID;
        public string DESCRIZIONE_TIPO;

		public TipoOggetto(){}

		public void gestisciCaratteriSpeciali()
		{
            DESCRIZIONE_TIPO = DESCRIZIONE_TIPO.Replace("'", "''");
		}		
	}
}
