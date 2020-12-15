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
