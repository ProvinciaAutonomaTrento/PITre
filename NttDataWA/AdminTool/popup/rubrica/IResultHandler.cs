using System;
using SAAdminTool.DocsPaWR;

namespace SAAdminTool.popup.RubricaDocsPA
{

	public interface IResultHandler
	{
		void execute (ElementoRubrica[] a, ElementoRubrica[] cc, SAAdminTool.DocsPaWR.RubricaCallType calltype);
	}
}
