// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;

namespace DocsPaVO.HMDiritti
{
	/// <summary>
	/// Summary description for HMdiritti.
	/// </summary>
	public class HMdiritti
	{
        public int HDdiritti_Waiting = 20;  // trasmissione con Workflow in attesa di ACCETTAZIONE / RIFIUTO
        public int HMdiritti_Read    = 45;  // solo lettura
        public int HMdiritti_Write   = 63;  // lettura + scrittura
		public int HMdiritti_Eredita = 63;  // eredita diritto
	}
}
