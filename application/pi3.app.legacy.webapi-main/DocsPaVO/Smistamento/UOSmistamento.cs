// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

namespace DocsPaVO.Smistamento
{
	/// <summary>
	/// Definizione oggetto UnitaOrganizzativa 
	/// relativo alla funzionalit� di smistamento documenti.
	/// </summary>
	public class UOSmistamento
	{
		public string ID=string.Empty;
		public string Codice=string.Empty;
		public string Descrizione=string.Empty;

		public DocsPaVO.Smistamento.RuoloSmistamento[] Ruoli =new DocsPaVO.Smistamento.RuoloSmistamento[0];

        public DocsPaVO.Smistamento.UOSmistamento[] UoInferiori = new DocsPaVO.Smistamento.UOSmistamento[0];

        public DocsPaVO.Smistamento.UOSmistamento[] UoSmistaTrasAutomatica = new DocsPaVO.Smistamento.UOSmistamento[0];

		public bool FlagCompetenza=false;
		public bool FlagConoscenza=false;
        public bool Selezionata = false;

        public string ragioneTrasmRapida = string.Empty;
        public bool modelloNoNotify = false;
	}
}
