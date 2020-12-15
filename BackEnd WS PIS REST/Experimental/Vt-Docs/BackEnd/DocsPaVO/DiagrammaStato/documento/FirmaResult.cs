using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.documento 
{
	/// <summary>
	/// </summary>
    [Serializable()]
	public class FirmaResult
	{
		public FileRequest fileRequest;
		public string errore;
	}
}