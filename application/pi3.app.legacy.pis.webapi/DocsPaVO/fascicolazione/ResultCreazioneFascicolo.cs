// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;

namespace DocsPaVO.fascicolazione
{
	/// <summary>
	/// Possibili esiti delle creazione di un fascicolo.
	/// </summary>
	public enum ResultCreazioneFascicolo
	{
		OK,
		GENERIC_ERROR,
		FASCICOLO_GIA_PRESENTE,
        FORMATO_FASCICOLATURA_NON_PRESENTE
	}
}
