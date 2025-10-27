// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace DocsPaVO.amministrazione
{
    /// <summary>
    /// Definizione oggetto Ruolo 
    /// relativo alla funzionalit� Organigramma in Amministrazione.
    /// </summary>
    [DataContract]
    public class OrgRuolo : ICloneable
	{
        [DataMember]
        public string IDCorrGlobale { get; set; } = string.Empty;

        [DataMember]
        public string IDGruppo { get; set; } = string.Empty;

        [DataMember]
        public string IDTipoRuolo { get; set; } = string.Empty;

        [DataMember]
        public string CodiceTipoRuolo { get; set; } = string.Empty;

        [DataMember]
        public string IDUo { get; set; } = string.Empty;

        [DataMember]
        public string Codice { get; set; } = string.Empty;

        [DataMember]
        public string CodiceRubrica { get; set; } = string.Empty;

        [DataMember]
        public string Descrizione { get; set; } = string.Empty;

        [DataMember]
        public string DiRiferimento { get; set; } = string.Empty;

        [DataMember]
        public string IDAmministrazione { get; set; } = string.Empty;

        [DataMember]
        public string Responsabile { get; set; } = string.Empty;

        [DataMember]
        public string IDPeso { get; set; } = string.Empty;

        [DataMember]
        public string Segretario { get; set; } = string.Empty;

        [DataMember]
        public string DisabledTrasm { get; set; } = string.Empty;

        // Autenticazione Sistemi Esterni
        // Ruolo di sistema
        [DataMember]
        public string RuoloDiSistema { get; set; } = string.Empty;

		[XmlArray()]
		[XmlArrayItem(typeof(DocsPaVO.amministrazione.OrgUtente))]
        [DataMember]
        public ArrayList Utenti { get; set; } = new ArrayList();

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
