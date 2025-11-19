// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
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
