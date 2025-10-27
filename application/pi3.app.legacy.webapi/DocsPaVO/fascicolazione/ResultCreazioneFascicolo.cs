// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
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
