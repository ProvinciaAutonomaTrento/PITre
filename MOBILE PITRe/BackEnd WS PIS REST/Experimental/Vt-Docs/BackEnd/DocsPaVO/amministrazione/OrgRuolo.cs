using System;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

namespace DocsPaVO.amministrazione
{
	/// <summary>
	/// Definizione oggetto Ruolo 
	/// relativo alla funzionalità Organigramma in Amministrazione.
	/// </summary>
	public class OrgRuolo : ICloneable
	{
		public string IDCorrGlobale = string.Empty;

		public string IDGruppo = string.Empty;

		public string IDTipoRuolo = string.Empty;

        public string CodiceTipoRuolo = string.Empty;

		public string IDUo = string.Empty;

		public string Codice = string.Empty;

		public string CodiceRubrica = string.Empty;

		public string Descrizione = string.Empty;

		public string DiRiferimento = string.Empty;

		public string IDAmministrazione = string.Empty;

        public string Responsabile = string.Empty;

        public string IDPeso = string.Empty;

        public string Segretario = string.Empty;

        public string DisabledTrasm = string.Empty;

        // Autenticazione Sistemi Esterni
        // Ruolo di sistema
        public string RuoloDiSistema = string.Empty;

		[XmlArray()]
		[XmlArrayItem(typeof(DocsPaVO.amministrazione.OrgUtente))]
		public ArrayList Utenti=new ArrayList();

        public object Clone()
        {
            return new OrgRuolo()
            {
                IDCorrGlobale = this.IDCorrGlobale,
                IDGruppo = this.IDGruppo,
                IDTipoRuolo = this.IDTipoRuolo,
                IDUo = this.IDUo,
                Codice = this.Codice,
                CodiceRubrica = this.CodiceRubrica,
                Descrizione = this.Descrizione,
                DiRiferimento = this.DiRiferimento,
                IDAmministrazione = this.IDAmministrazione,
                Responsabile = this.Responsabile,
                IDPeso = this.IDPeso,
                Segretario = this.Segretario,
                DisabledTrasm = this.DisabledTrasm
            };
        }
	}
}
