using System;

namespace DocsPaVO.ProfilazioneDinamica
{
    [Serializable()]
	public class TipoOggetto
	{
        public int SYSTEM_ID;
        public string DESCRIZIONE_TIPO;

		public TipoOggetto(){}

		public void gestisciCaratteriSpeciali()
		{
            DESCRIZIONE_TIPO = DESCRIZIONE_TIPO.Replace("'", "''");
		}		
	}
}
