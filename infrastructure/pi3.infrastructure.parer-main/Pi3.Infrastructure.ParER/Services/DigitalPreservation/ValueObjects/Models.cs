// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Pi3.Infrastructure.ParER.Services.DigitalPreservation.ValueObjects
{
    public enum TipologiaUnitaDocumentariaEnum
    {
        [Description("Documento Protocollato")]
        DocumentoProtocollato,

        [Description("Documento Repertoriato")]
        DocumentoRepertoriato,

        [Description("Documento Non Protocollato")]
        DocumentoNonProtocollato,

        [Description("Stampa registro")]
        StampaRegistro,

        [Description("FATTURA PASSIVA")]
        FatturaElettronica,

        [Description("LOTTO DI FATTURA")]
        LottoDiFatture,

        [Description("VERBALE SINTETICO DI SEDUTA")]
        VerbaleSinteticoDiSeduta,

        [Description("FATTURA ELETTRONICA ATTIVA")]
        FatturaAttiva,

        [Description("LOTTO DI FATTURE")]
        LottoDiFattureAttive
    }

    public enum TipoComponenteEnum
    {
        [Description("Contenuto")]
        Contenuto,

        [Description("Marca")]
        Marca,

        [Description("Firma elettronica")]
        FirmaElettronica
    }

    public enum TipoDocumentoEnum
    {
        [Description("Documento Principale")]
        DocumentoPrincipale,

        [Description("LOTTO DI FATTURE")]
        LottoDiFatture,

        [Description("LOTTO DI FATTURE ATTIVE")]
        LottoDiFattureAttive,

        [Description("VERBALE SINTETICO DI SEDUTA")]
        VerbaleSinteticoDiSeduta
    }

    public enum TipoAllegatoConservazioneEnum
    {
        [Description("Allegato utente")]
        AllegatoUtente,

        [Description("Allegato utente - non acquisito")]
        AllegatoUtenteNonAcquisito,

        [Description("Allegato PEC")]
        AllegatoPEC,

        [Description("Allegato Pitre")]
        AllegatoPITre,

        [Description("Altri sistemi")]
        AltriSistemi,

        [Description("Metadati PITre")]
        MetadatiPITre,

        [Description("Segnatura xml")]
        Segnatura,

        [Description("Notifica di esito committente")]
        NotificaEsitoCommittente,

        [Description("Notifica di decorrenza dei termini")]
        NotificaDecorrenzaTermini,

        [Description("Rappresentazione Fattura")]
        RappresentazioneFattura
    }

    public enum TipoRiferimentoTemporaleEnum
    {
        [Description("Marca temporale")]
        MarcaTemporale,

        [Description("Data di protocollazione")]
        DataProtocollazione,

        [Description("Data di repertoriazione")]
        DataRepertoriazione,

        [Description("Data di creazione")]
        DataCreazione,

        [Description("Data di versamento")]
        DataVersamento
    }

    public enum TipoAllegatoEnum
    {
        [Description("Allegato Utente")]
        AllegatoUtente,

        [Description("Allegato PEC")]
        AllegatoPec,

        [Description("Allegato Pitre")]
        AllegatoPitre,

        [Description("Allegato Altri Sistemi")]
        AllegatoSistemiEsterni
    }



    public class IdComponente
    {
        private int _id;
        private string _docnumber;

        public IdComponente(string docnumber)
        {
            this._id = 1;
            this._docnumber = docnumber;
        }

        public string Current()
            => $"{this._docnumber}_C{this._id}";

        public string Next()
            => $"{this._docnumber}_C{this._id++}";
    }

    public class IdSottocomponente
    {
        private int _id;
        private string _idComponente;

        public IdSottocomponente(string _idComponente)
        {
            this._id = 0;
            this._idComponente = _idComponente;
        }

        public string Current()
            => $"{this._idComponente}_SC{this._id}";

        public string Next()
            => $"{this._idComponente}_SC{this._id++}";

        public int Id()
            => this._id;
    }

    public class RiferimentoTemporale
    {
        public DateTime? DataRiferimentoTemporale { get; set; }

        public TipoRiferimentoTemporaleEnum? TipoRiferimentoTemporale { get; set; }
    }
}
