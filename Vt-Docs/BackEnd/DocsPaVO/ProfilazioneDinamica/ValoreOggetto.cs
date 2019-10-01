using System;
using System.ComponentModel;

namespace DocsPaVO.ProfilazioneDinamica
{
    [Serializable()]
	public class ValoreOggetto
	{
        public int SYSTEM_ID;
        public string DESCRIZIONE_VALORE;
        public string VALORE;
        public string VALORE_DI_DEFAULT;
        public string COLOR_BG;
        [DefaultValue(1)] 
        public int ABILITATO = 1;
		
		public ValoreOggetto(){}

		public void gestisciCaratteriSpeciali()
		{
            DESCRIZIONE_VALORE = DESCRIZIONE_VALORE.Replace("'", "''");
            VALORE = VALORE.Replace("'", "''");
            VALORE_DI_DEFAULT = VALORE_DI_DEFAULT.Replace("'", "''");
		}
	}
}
