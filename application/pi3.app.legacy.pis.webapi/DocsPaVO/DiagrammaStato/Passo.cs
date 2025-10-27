// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.DiagrammaStato
{
	public class Passo
	{
		public int ID_DIAGRAMMA;
		public Stato STATO_PADRE;
		public string ID_STATO_AUTOMATICO;
        public string ID_STATO_AUTOMATICO_LF;
		public string DESCRIZIONE_STATO_AUTOMATICO = "";

		public DocsPaVO.DiagrammaStato.Stato[] SUCCESSIVI = new DocsPaVO.DiagrammaStato.Stato[0];
	}
}
