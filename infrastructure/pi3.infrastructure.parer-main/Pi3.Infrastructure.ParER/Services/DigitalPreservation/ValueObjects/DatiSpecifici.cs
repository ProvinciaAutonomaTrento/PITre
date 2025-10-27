// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Pi3.Infrastructure.ParER.Services.DigitalPreservation.ValueObjects
{
    public abstract class DatiSpecifici
    {
        public DatiSpecifici()
        {

        }
        public DatiSpecifici(string dataCreazione, string utenteCreatore, string ruoloCreatore, string uoCreatrice)
        {
            this._dataCreazione = dataCreazione;
            this._utenteCreatore = utenteCreatore;
            this._ruoloCreatore = ruoloCreatore;
            this._uoCreatrice = uoCreatrice;
        }

        protected string _dataCreazione;

        protected string _utenteCreatore;

        protected string _ruoloCreatore;

        protected string _uoCreatrice;
    }

    [XmlRoot("DatiSpecifici")]
    public class DatiSpecificiDocumentoProtocollato : DatiSpecifici
    {
        public DatiSpecificiDocumentoProtocollato()
           : base()
        {

        }

        public DatiSpecificiDocumentoProtocollato(string dataCreazione, string utenteCreatore, string ruoloCreatore, string uoCreatrice)
            : base(dataCreazione, utenteCreatore, ruoloCreatore, uoCreatrice)
        {
            this._dataCreazione = dataCreazione;
            this._utenteCreatore = utenteCreatore;
            this._ruoloCreatore = ruoloCreatore;
            this._uoCreatrice = uoCreatrice;
        }

        public string? NumeroProtocollo { get; set; }

        public string? AnnoProtocollazione { get; set; }

        public string? TipoRegistroProtocollo { get; set; }

        public string? SegnaturaProtocollo { get; set; }

        public string? TipoProtocollo { get; set; }

        public string? CodiceRegistro { get; set; }

        public string? DescrizioneRegistro { get; set; }

        public string? Mittente { get; set; }

        public string? MezzoDiSpedizioneMittente { get; set; }

        public string? ProtocolloMittente { get; set; }

        public string? DataProtocolloMittente { get; set; }

        public string? DataArrivo { get; set; }

        public string? OraArrivo { get; set; }

        public string? Destinatari { get; set; }

        public string? TipologiaDocumentalePITre { get; set; }

        public string DataCreazioneProfiloDocumento
        {
            get => _dataCreazione;
            set => _dataCreazione = value;
        }

        public string UtenteCreatore
        {
            get => _utenteCreatore;
            set => _utenteCreatore = value;
        }

        public string RuoloCreatore
        {
            get => _ruoloCreatore;
            set => _ruoloCreatore = value;
        }

        public string UOCreatrice
        {
            get => _uoCreatrice;
            set => _uoCreatrice = value;
        }
    }

    [XmlRoot("DatiSpecifici")]
    public class DatiSpecificiDocumentoNonProtocollato : DatiSpecifici
    {
        public DatiSpecificiDocumentoNonProtocollato()
            : base()
        {

        }
        public DatiSpecificiDocumentoNonProtocollato(string dataCreazione, string utenteCreatore, string ruoloCreatore, string uoCreatrice)
            : base(dataCreazione, utenteCreatore, ruoloCreatore, uoCreatrice)
        {
            this._dataCreazione = dataCreazione;
            this._utenteCreatore = utenteCreatore;
            this._ruoloCreatore = ruoloCreatore;
            this._uoCreatrice = uoCreatrice;
        }

        public string? TipologiaDocumentalePITre { get; set; }

        public string DataCreazioneProfiloDocumento
        {
            get => _dataCreazione;
            set => _dataCreazione = value;
        }

        public string UtenteCreatore
        {
            get => _utenteCreatore;
            set => _utenteCreatore = value;
        }

        public string RuoloCreatore
        {
            get => _ruoloCreatore;
            set => _ruoloCreatore = value;
        }

        public string UOCreatrice
        {
            get => _uoCreatrice;
            set => _uoCreatrice = value;
        }
    }

    [XmlRoot("DatiSpecifici")]
    public class DatiSpecificiDocumentoRepertoriato : DatiSpecifici
    {
        public DatiSpecificiDocumentoRepertoriato()
            :base()
        {
                
        }

        public DatiSpecificiDocumentoRepertoriato(string dataCreazione, string utenteCreatore, string ruoloCreatore, string uoCreatrice)
            :base(dataCreazione, utenteCreatore, ruoloCreatore, uoCreatrice)
        {
                
        }

        public string? SegnaturaRepertorio { get; set; }

        public string? CodiceRegistro_REP { get; set; }

        public string? DescrizioneRegistro_REP { get; set; }

        public string? CodiceRF_REP { get; set; }

        public string DescrizioneRF_REP { get; set; }

        public string? NumeroProtocollo { get; set; }

        public string? AnnoProtocollazione { get; set; }

        public string? DataProtocollazione { get; set; }

        public string? TipoRegistroProtocollo { get; set; }

        public string? SegnaturaProtocollo { get; set; }

        public string? TipoProtocollo { get; set; }

        public string? CodiceRegistro_PROT { get; set; }

        public string? DescrizioneRegistro_PROT { get; set; }

        public string? CodiceRF_PROT { get; set; }

        public string? DescrizioneRF_PROT { get; set; }

        public string? Mittente { get; set; }

        public string? MezzoDiSpedizioneMittente { get; set; }

        public string? ProtocolloMittente { get; set; }

        public string? DataProtocolloMittente { get; set; }

        public string? DataArrivo { get; set; }

        public string? OraArrivo { get; set; }

        public string? Destinatari { get; set; }

        public string? TipologiaDocumentalePITre { get; set; }

        public string DataCreazioneProfiloDocumento
        {
            get => _dataCreazione;
            set => _dataCreazione = value;
        }

        public string UtenteCreatore
        {
            get => _utenteCreatore;
            set => _utenteCreatore = value;
        }

        public string RuoloCreatore
        {
            get => _ruoloCreatore;
            set => _ruoloCreatore = value;
        }

        public string UOCreatrice
        {
            get => _uoCreatrice;
            set => _uoCreatrice = value;
        }
    }

    [XmlRoot("DatiSpecifici")]
    public class DatiSpecificiStampaRegistro : DatiSpecifici
    {
        public DatiSpecificiStampaRegistro()
            : base()
        {

        }

        public DatiSpecificiStampaRegistro(string dataCreazione, string utenteCreatore, string ruoloCreatore, string uoCreatrice)
            : base(dataCreazione, utenteCreatore, ruoloCreatore, uoCreatrice)
        {
            this._dataCreazione = dataCreazione;
            this._utenteCreatore = utenteCreatore;
            this._ruoloCreatore = ruoloCreatore;
            this._uoCreatrice = uoCreatrice;
        }

        public string TipoRegistro { get; set; }

        public string? CodiceRegistro_PROT { get; set; }

        public string? DescrizioneRegistro_PROT { get; set; }

        public string? CodiceRegistro_REP { get; set; }

        public string? DescrizioneRegistro_REP { get; set; }

        public string? CodiceRF_REP { get; set; }

        public string? DescrizioneRF_REP { get; set; }

        public string? RuoloResponsabileRegistro { get; set; }

        public string? FrequenzaDiStampa { get; set; }

        public string? PrimoElementoRegistrato { get; set; }

        public string? DataPrimaRegistrazione { get; set; }

        public string? UltimoElementoRegistrato { get; set; }

        public string? DataUltimaRegistrazione { get; set; }

        public string DataCreazioneProfiloDocumento
        {
            get => this._dataCreazione;
            set => this._dataCreazione = value;
        }

        public string UtenteCreatore
        {
            get => this._utenteCreatore;
            set => this._utenteCreatore = value;
        }

        public string RuoloCreatore
        {
            get => this._ruoloCreatore;
            set => this._ruoloCreatore = value;
        }
            
        public string UOCreatrice
        {
            get => this._uoCreatrice;
            set => this._uoCreatrice = value;
        }
            
    }

    [XmlRoot("DatiSpecifici")]
    public class DatiSpecificiFatturaElettronica : DatiSpecifici
    {
        public DatiSpecificiFatturaElettronica()
            : base()
        {

        }

        public DatiSpecificiFatturaElettronica(string dataCreazione, string utenteCreatore, string ruoloCreatore, string uoCreatrice)
            : base(dataCreazione, utenteCreatore, ruoloCreatore, uoCreatrice)
        {
            this._dataCreazione = dataCreazione;
            this._utenteCreatore = utenteCreatore;
            this._ruoloCreatore = ruoloCreatore;
            this._uoCreatrice = uoCreatrice;
        }
        public string? SegnaturaRepertorio { get; set; }
        public string? NumeroRepertorio { get; set; }
        public string? DataRepertorio { get; set; }

        public string? CodiceRegistro_REP { get; set; }

        public string? DescrizioneRegistro_REP { get; set; }

        public string? CodiceRF_REP { get; set; }

        public string DescrizioneRF_REP { get; set; }
        public string? NumeroProtocollo { get; set; }

        public string? AnnoProtocollazione { get; set; }

        public string? DataProtocollazione { get; set; }

        public string? TipoRegistroProtocollo { get; set; }

        public string? SegnaturaProtocollo { get; set; }

        public string? TipoProtocollo { get; set; }

        public string? CodiceRegistro_PROT { get; set; }

        public string? DescrizioneRegistro_PROT { get; set; }

        public string? CodiceRF_PROT { get; set; }

        public string? DescrizioneRF_PROT { get; set; }

        public string? Mittente { get; set; }

        public string? MezzoDiSpedizioneMittente { get; set; }

        public string? ProtocolloMittente { get; set; }

        public string? DataProtocolloMittente { get; set; }

        public string? DataArrivo { get; set; }

        public string? OraArrivo { get; set; }
        public string? NumeroEmissione { get; set; }
        public string? DataEmissione { get; set; }
        public string? DenominazioneMittente { get; set; }
        public string? PartitaIvaMittente { get; set; }
        public string? CodiceFiscaleMittente { get; set; }
        public string? CIG { get; set; }
        public string? CUP { get; set; }
        public string? IdentificativoSdI { get; set; }
        public string? AliquotaIvaReverseCharge { get; set; }
        public string? IvaTotaleReverseCharge { get; set; }
        public string? TipologiaDocumentalePITre { get; set; }

        public string DataCreazioneProfiloDocumento
        {
            get => _dataCreazione;
            set => _dataCreazione = value;
        }

        public string UtenteCreatore
        {
            get => _utenteCreatore;
            set => _utenteCreatore = value;
        }

        public string RuoloCreatore
        {
            get => _ruoloCreatore;
            set => _ruoloCreatore = value;
        }

        public string UOCreatrice
        {
            get => _uoCreatrice;
            set => _uoCreatrice = value;
        }
    }

    [XmlRoot("DatiSpecifici")]
    public class DatiSpecificiLottoDiFatture : DatiSpecifici
    {
        public DatiSpecificiLottoDiFatture()
            : base()
        {

        }

        public DatiSpecificiLottoDiFatture(string dataCreazione, string utenteCreatore, string ruoloCreatore, string uoCreatrice)
            : base(dataCreazione, utenteCreatore, ruoloCreatore, uoCreatrice)
        {
            this._dataCreazione = dataCreazione;
            this._utenteCreatore = utenteCreatore;
            this._ruoloCreatore = ruoloCreatore;
            this._uoCreatrice = uoCreatrice;
        }
        public string? SegnaturaRepertorio { get; set; }
        public string? NumeroRepertorio { get; set; }
        public string? DataRepertorio { get; set; }

        public string? CodiceRegistro_REP { get; set; }

        public string? DescrizioneRegistro_REP { get; set; }

        public string? CodiceRF_REP { get; set; }

        public string DescrizioneRF_REP { get; set; }
        public string? NumeroProtocollo { get; set; }

        public string? AnnoProtocollazione { get; set; }

        public string? DataProtocollazione { get; set; }

        public string? TipoRegistroProtocollo { get; set; }

        public string? SegnaturaProtocollo { get; set; }

        public string? TipoProtocollo { get; set; }

        public string? CodiceRegistro_PROT { get; set; }

        public string? DescrizioneRegistro_PROT { get; set; }

        public string? CodiceRF_PROT { get; set; }

        public string? DescrizioneRF_PROT { get; set; }

        public string? Mittente { get; set; }

        public string? MezzoDiSpedizioneMittente { get; set; }

        public string? ProtocolloMittente { get; set; }

        public string? DataProtocolloMittente { get; set; }

        public string? DataArrivo { get; set; }

        public string? OraArrivo { get; set; }

        public string? Destinatari { get; set; }

        public string? DenominazioneMittente { get; set; }
        public string? PartitaIvaMittente { get; set; }
        public string? CodiceFiscaleMittente { get; set; }
        public string? IdentificativoSdI { get; set; }
      
        public string? TipologiaDocumentalePITre { get; set; }
        public string DataCreazioneProfiloDocumento
        {
            get => _dataCreazione;
            set => _dataCreazione = value;
        }

        public string UtenteCreatore
        {
            get => _utenteCreatore;
            set => _utenteCreatore = value;
        }

        public string RuoloCreatore
        {
            get => _ruoloCreatore;
            set => _ruoloCreatore = value;
        }

        public string UOCreatrice
        {
            get => _uoCreatrice;
            set => _uoCreatrice = value;
        }
    }

    [XmlRoot("DatiSpecifici")]
    public class DatiSpecificiFatturaAttiva : DatiSpecifici
    {
        public DatiSpecificiFatturaAttiva()
            : base()
        {

        }

        public DatiSpecificiFatturaAttiva(string dataCreazione, string utenteCreatore, string ruoloCreatore, string uoCreatrice)
            : base(dataCreazione, utenteCreatore, ruoloCreatore, uoCreatrice)
        {
            this._dataCreazione = dataCreazione;
            this._utenteCreatore = utenteCreatore;
            this._ruoloCreatore = ruoloCreatore;
            this._uoCreatrice = uoCreatrice;
        }

        public string? SegnaturaRepertorio { get; set; }
        public string? NumeroRepertorio { get; set; }

        public string? DataRepertorio { get; set; }

        public string? CodiceRegistro_REP { get; set; }

        public string? DescrizioneRegistro_REP { get; set; }

        public string? CodiceRF_REP { get; set; }

        public string DescrizioneRF_REP { get; set; }
        public string? NumeroProtocollo { get; set; }

        public string? AnnoProtocollazione { get; set; }

        public string? DataProtocollazione { get; set; }

        public string? TipoRegistroProtocollo { get; set; }

        public string? SegnaturaProtocollo { get; set; }

        public string? TipoProtocollo { get; set; }

        public string? CodiceRegistro_PROT { get; set; }

        public string? DescrizioneRegistro_PROT { get; set; }

        public string? CodiceRF_PROT { get; set; }

        public string? DescrizioneRF_PROT { get; set; }

        public string? Mittente { get; set; }

        public string? MezzoDiSpedizioneMittente { get; set; }

        public string? ProtocolloMittente { get; set; }

        public string? DataProtocolloMittente { get; set; }

        public string? DataArrivo { get; set; }

        public string? OraArrivo { get; set; }

        public string? Destinatari { get; set; }

        public string? NumeroEmissione { get; set; }

        public string? DataEmissione { get; set; }
        public string? IdPaese { get; set; }

        public string? DenominazioneDestinatario { get; set; }
        public string? IdentificativoDestinatario { get; set; }
        public string? TipoIdentificativoDestinatario { get; set; }
        public string? ImportoTotale { get; set; }
        public string? ScadenzaFattura { get; set; }
        public string? IdentificativoSdI { get; set; }
        public string? AliquotaIvaReverseCharge { get; set; }
        public string? IvaTotaleReverseCharge { get; set; }
        public string? TipologiaDocumentalePITre { get; set; }

        public string DataCreazioneProfiloDocumento
        {
            get => _dataCreazione;
            set => _dataCreazione = value;
        }

        public string UtenteCreatore
        {
            get => _utenteCreatore;
            set => _utenteCreatore = value;
        }

        public string RuoloCreatore
        {
            get => _ruoloCreatore;
            set => _ruoloCreatore = value;
        }

        public string UOCreatrice
        {
            get => _uoCreatrice;
            set => _uoCreatrice = value;
        }
    }

    [XmlRoot("DatiSpecifici")]
    public class DatiSpecificiLottoDiFattureAttive : DatiSpecifici
    {
        public DatiSpecificiLottoDiFattureAttive()
            : base()
        {

        }

        public DatiSpecificiLottoDiFattureAttive(string dataCreazione, string utenteCreatore, string ruoloCreatore, string uoCreatrice)
            : base(dataCreazione, utenteCreatore, ruoloCreatore, uoCreatrice)
        {
            this._dataCreazione = dataCreazione;
            this._utenteCreatore = utenteCreatore;
            this._ruoloCreatore = ruoloCreatore;
            this._uoCreatrice = uoCreatrice;
        }
        public string? SegnaturaRepertorio { get; set; }
        public string? NumeroRepertorio { get; set; }

        public string? DataRepertorio { get; set; }

        public string? CodiceRegistro_REP { get; set; }

        public string? DescrizioneRegistro_REP { get; set; }

        public string? CodiceRF_REP { get; set; }

        public string DescrizioneRF_REP { get; set; }
        public string? NumeroProtocollo { get; set; }

        public string? AnnoProtocollazione { get; set; }

        public string? DataProtocollazione { get; set; }

        public string? TipoRegistroProtocollo { get; set; }

        public string? SegnaturaProtocollo { get; set; }

        public string? TipoProtocollo { get; set; }

        public string? CodiceRegistro_PROT { get; set; }

        public string? DescrizioneRegistro_PROT { get; set; }

        public string? CodiceRF_PROT { get; set; }

        public string? DescrizioneRF_PROT { get; set; }

        public string? Mittente { get; set; }

        public string? MezzoDiSpedizioneMittente { get; set; }

        public string? ProtocolloMittente { get; set; }

        public string? DataProtocolloMittente { get; set; }

        public string? DataArrivo { get; set; }

        public string? OraArrivo { get; set; }

        public string? Destinatari { get; set; }

        public string? NumeroEmissione { get; set; }

        public string? DataEmissione { get; set; }
        public string? IdPaese { get; set; }

        public string? DenominazioneDestinatario { get; set; }

        public string? IdentificativoDestinatario { get; set; }

        public string? TipoIdentificativoDestinatario { get; set; }

        public string? IdentificativoSdI { get; set; }
     
        public string? TipologiaDocumentalePITre { get; set; }

        public string DataCreazioneProfiloDocumento
        {
            get => _dataCreazione;
            set => _dataCreazione = value;
        }

        public string UtenteCreatore
        {
            get => _utenteCreatore;
            set => _utenteCreatore = value;
        }

        public string RuoloCreatore
        {
            get => _ruoloCreatore;
            set => _ruoloCreatore = value;
        }

        public string UOCreatrice
        {
            get => _uoCreatrice;
            set => _uoCreatrice = value;
        }
    }

    [XmlRoot("DatiSpecifici")]
    public class DatiSpecificiVerbaleSinteticoDiSeduta : DatiSpecifici
    {
        public DatiSpecificiVerbaleSinteticoDiSeduta()
            : base()
        {

        }

        public string? Ente { get; set; }

        public string? Organo { get; set; }

        public string? NumeroSeduta { get; set; }

        public string? DataSeduta { get; set; }

        public string? DescrizioneEnte { get; set; }
    }
}
