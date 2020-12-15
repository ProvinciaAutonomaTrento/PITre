using System;
using System.Xml.Serialization;

namespace DocsPaVO.DiagrammaStato
{
	public class Stato
	{
		public int SYSTEM_ID = 0;
		public int ID_DIAGRAMMA = 0;
		public string DESCRIZIONE = "";
		public bool STATO_INIZIALE= false;
		public bool STATO_FINALE = false;
        public bool CONVERSIONE_PDF = false;
        public bool NON_RICERCABILE = false;
        public bool STATO_SISTEMA = false;
        public DocsPaVO.documento.DocumentConsolidationStateEnum STATO_CONSOLIDAMENTO = documento.DocumentConsolidationStateEnum.None;
        public string ID_PROCESSO_FIRMA = string.Empty;
        public bool PUBBLICAZIONE_FILES = false;
	}
}
