using System;
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.popup.RubricaDocsPA
{

	public interface IResultHandler
	{
		void execute (ElementoRubrica[] a, ElementoRubrica[] cc, DocsPAWA.DocsPaWR.RubricaCallType calltype);
	}
}
