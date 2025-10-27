// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.DiagrammaStato
{
	public class DiagrammaStato
	{
		public int SYSTEM_ID = 0;
		public string DESCRIZIONE = "";
		public int ID_AMM = 0;

		public DocsPaVO.DiagrammaStato.Stato[] STATI = new DocsPaVO.DiagrammaStato.Stato[0];

		public DocsPaVO.DiagrammaStato.Passo[] PASSI = new DocsPaVO.DiagrammaStato.Passo[0];		
	}
}
