// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Xml.Serialization;

namespace DocsPaVO.Modelli_Trasmissioni
{
	public class RagioneDest
	{
		public string RAGIONE;
		public string CHA_TIPO_RAGIONE;


		public DocsPaVO.Modelli_Trasmissioni.MittDest[] DESTINATARI = new DocsPaVO.Modelli_Trasmissioni.MittDest[0];	
	}
}
