using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NttDatalLibrary
{
    // 
    // Codice sorgente generato automaticamente da xsd, versione=4.6.1055.0.
    //

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    [System.Xml.Serialization.XmlRootAttribute("FatturaElettronica", Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2", IsNullable = false)]
    public partial class FatturaElettronicaType
    {

        private FatturaElettronicaHeaderType fatturaElettronicaHeaderField;

        private FatturaElettronicaBodyType[] fatturaElettronicaBodyField;

        private FormatoTrasmissioneType versioneField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public FatturaElettronicaHeaderType FatturaElettronicaHeader
        {
            get
            {
                return this.fatturaElettronicaHeaderField;
            }
            set
            {
                this.fatturaElettronicaHeaderField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("FatturaElettronicaBody", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public FatturaElettronicaBodyType[] FatturaElettronicaBody
        {
            get
            {
                return this.fatturaElettronicaBodyField;
            }
            set
            {
                this.fatturaElettronicaBodyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public FormatoTrasmissioneType versione
        {
            get
            {
                return this.versioneField;
            }
            set
            {
                this.versioneField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public partial class FatturaElettronicaHeaderType
    {

        private DatiTrasmissioneType datiTrasmissioneField;

        private CedentePrestatoreType cedentePrestatoreField;

        private RappresentanteFiscaleType rappresentanteFiscaleField;

        private CessionarioCommittenteType cessionarioCommittenteField;

        private TerzoIntermediarioSoggettoEmittenteType terzoIntermediarioOSoggettoEmittenteField;

        private SoggettoEmittenteType soggettoEmittenteField;

        private bool soggettoEmittenteFieldSpecified;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public DatiTrasmissioneType DatiTrasmissione
        {
            get
            {
                return this.datiTrasmissioneField;
            }
            set
            {
                this.datiTrasmissioneField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public CedentePrestatoreType CedentePrestatore
        {
            get
            {
                return this.cedentePrestatoreField;
            }
            set
            {
                this.cedentePrestatoreField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public RappresentanteFiscaleType RappresentanteFiscale
        {
            get
            {
                return this.rappresentanteFiscaleField;
            }
            set
            {
                this.rappresentanteFiscaleField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public CessionarioCommittenteType CessionarioCommittente
        {
            get
            {
                return this.cessionarioCommittenteField;
            }
            set
            {
                this.cessionarioCommittenteField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public TerzoIntermediarioSoggettoEmittenteType TerzoIntermediarioOSoggettoEmittente
        {
            get
            {
                return this.terzoIntermediarioOSoggettoEmittenteField;
            }
            set
            {
                this.terzoIntermediarioOSoggettoEmittenteField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public SoggettoEmittenteType SoggettoEmittente
        {
            get
            {
                return this.soggettoEmittenteField;
            }
            set
            {
                this.soggettoEmittenteField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool SoggettoEmittenteSpecified
        {
            get
            {
                return this.soggettoEmittenteFieldSpecified;
            }
            set
            {
                this.soggettoEmittenteFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public partial class DatiTrasmissioneType
    {

        private IdFiscaleType idTrasmittenteField;

        private string progressivoInvioField;

        private FormatoTrasmissioneType formatoTrasmissioneField;

        private string codiceDestinatarioField;

        private ContattiTrasmittenteType contattiTrasmittenteField;

        private string pECDestinatarioField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public IdFiscaleType IdTrasmittente
        {
            get
            {
                return this.idTrasmittenteField;
            }
            set
            {
                this.idTrasmittenteField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "normalizedString")]
        public string ProgressivoInvio
        {
            get
            {
                return this.progressivoInvioField;
            }
            set
            {
                this.progressivoInvioField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public FormatoTrasmissioneType FormatoTrasmissione
        {
            get
            {
                return this.formatoTrasmissioneField;
            }
            set
            {
                this.formatoTrasmissioneField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CodiceDestinatario
        {
            get
            {
                return this.codiceDestinatarioField;
            }
            set
            {
                this.codiceDestinatarioField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public ContattiTrasmittenteType ContattiTrasmittente
        {
            get
            {
                return this.contattiTrasmittenteField;
            }
            set
            {
                this.contattiTrasmittenteField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string PECDestinatario
        {
            get
            {
                return this.pECDestinatarioField;
            }
            set
            {
                this.pECDestinatarioField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public partial class IdFiscaleType
    {

        private string idPaeseField;

        private string idCodiceField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string IdPaese
        {
            get
            {
                return this.idPaeseField;
            }
            set
            {
                this.idPaeseField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string IdCodice
        {
            get
            {
                return this.idCodiceField;
            }
            set
            {
                this.idCodiceField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public partial class AllegatiType
    {

        private string nomeAttachmentField;

        private string algoritmoCompressioneField;

        private string formatoAttachmentField;

        private string descrizioneAttachmentField;

        private byte[] attachmentField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "normalizedString")]
        public string NomeAttachment
        {
            get
            {
                return this.nomeAttachmentField;
            }
            set
            {
                this.nomeAttachmentField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "normalizedString")]
        public string AlgoritmoCompressione
        {
            get
            {
                return this.algoritmoCompressioneField;
            }
            set
            {
                this.algoritmoCompressioneField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "normalizedString")]
        public string FormatoAttachment
        {
            get
            {
                return this.formatoAttachmentField;
            }
            set
            {
                this.formatoAttachmentField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "normalizedString")]
        public string DescrizioneAttachment
        {
            get
            {
                return this.descrizioneAttachmentField;
            }
            set
            {
                this.descrizioneAttachmentField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "base64Binary")]
        public byte[] Attachment
        {
            get
            {
                return this.attachmentField;
            }
            set
            {
                this.attachmentField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public partial class DettaglioPagamentoType
    {

        private string beneficiarioField;

        private ModalitaPagamentoType modalitaPagamentoField;

        private System.DateTime dataRiferimentoTerminiPagamentoField;

        private bool dataRiferimentoTerminiPagamentoFieldSpecified;

        private string giorniTerminiPagamentoField;

        private System.DateTime dataScadenzaPagamentoField;

        private bool dataScadenzaPagamentoFieldSpecified;

        private decimal importoPagamentoField;

        private string codUfficioPostaleField;

        private string cognomeQuietanzanteField;

        private string nomeQuietanzanteField;

        private string cFQuietanzanteField;

        private string titoloQuietanzanteField;

        private string istitutoFinanziarioField;

        private string iBANField;

        private string aBIField;

        private string cABField;

        private string bICField;

        private decimal scontoPagamentoAnticipatoField;

        private bool scontoPagamentoAnticipatoFieldSpecified;

        private System.DateTime dataLimitePagamentoAnticipatoField;

        private bool dataLimitePagamentoAnticipatoFieldSpecified;

        private decimal penalitaPagamentiRitardatiField;

        private bool penalitaPagamentiRitardatiFieldSpecified;

        private System.DateTime dataDecorrenzaPenaleField;

        private bool dataDecorrenzaPenaleFieldSpecified;

        private string codicePagamentoField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "normalizedString")]
        public string Beneficiario
        {
            get
            {
                return this.beneficiarioField;
            }
            set
            {
                this.beneficiarioField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public ModalitaPagamentoType ModalitaPagamento
        {
            get
            {
                return this.modalitaPagamentoField;
            }
            set
            {
                this.modalitaPagamentoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "date")]
        public System.DateTime DataRiferimentoTerminiPagamento
        {
            get
            {
                return this.dataRiferimentoTerminiPagamentoField;
            }
            set
            {
                this.dataRiferimentoTerminiPagamentoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool DataRiferimentoTerminiPagamentoSpecified
        {
            get
            {
                return this.dataRiferimentoTerminiPagamentoFieldSpecified;
            }
            set
            {
                this.dataRiferimentoTerminiPagamentoFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "integer")]
        public string GiorniTerminiPagamento
        {
            get
            {
                return this.giorniTerminiPagamentoField;
            }
            set
            {
                this.giorniTerminiPagamentoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "date")]
        public System.DateTime DataScadenzaPagamento
        {
            get
            {
                return this.dataScadenzaPagamentoField;
            }
            set
            {
                this.dataScadenzaPagamentoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool DataScadenzaPagamentoSpecified
        {
            get
            {
                return this.dataScadenzaPagamentoFieldSpecified;
            }
            set
            {
                this.dataScadenzaPagamentoFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public decimal ImportoPagamento
        {
            get
            {
                return this.importoPagamentoField;
            }
            set
            {
                this.importoPagamentoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "normalizedString")]
        public string CodUfficioPostale
        {
            get
            {
                return this.codUfficioPostaleField;
            }
            set
            {
                this.codUfficioPostaleField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "normalizedString")]
        public string CognomeQuietanzante
        {
            get
            {
                return this.cognomeQuietanzanteField;
            }
            set
            {
                this.cognomeQuietanzanteField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "normalizedString")]
        public string NomeQuietanzante
        {
            get
            {
                return this.nomeQuietanzanteField;
            }
            set
            {
                this.nomeQuietanzanteField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CFQuietanzante
        {
            get
            {
                return this.cFQuietanzanteField;
            }
            set
            {
                this.cFQuietanzanteField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "normalizedString")]
        public string TitoloQuietanzante
        {
            get
            {
                return this.titoloQuietanzanteField;
            }
            set
            {
                this.titoloQuietanzanteField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "normalizedString")]
        public string IstitutoFinanziario
        {
            get
            {
                return this.istitutoFinanziarioField;
            }
            set
            {
                this.istitutoFinanziarioField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string IBAN
        {
            get
            {
                return this.iBANField;
            }
            set
            {
                this.iBANField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ABI
        {
            get
            {
                return this.aBIField;
            }
            set
            {
                this.aBIField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CAB
        {
            get
            {
                return this.cABField;
            }
            set
            {
                this.cABField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string BIC
        {
            get
            {
                return this.bICField;
            }
            set
            {
                this.bICField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public decimal ScontoPagamentoAnticipato
        {
            get
            {
                return this.scontoPagamentoAnticipatoField;
            }
            set
            {
                this.scontoPagamentoAnticipatoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ScontoPagamentoAnticipatoSpecified
        {
            get
            {
                return this.scontoPagamentoAnticipatoFieldSpecified;
            }
            set
            {
                this.scontoPagamentoAnticipatoFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "date")]
        public System.DateTime DataLimitePagamentoAnticipato
        {
            get
            {
                return this.dataLimitePagamentoAnticipatoField;
            }
            set
            {
                this.dataLimitePagamentoAnticipatoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool DataLimitePagamentoAnticipatoSpecified
        {
            get
            {
                return this.dataLimitePagamentoAnticipatoFieldSpecified;
            }
            set
            {
                this.dataLimitePagamentoAnticipatoFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public decimal PenalitaPagamentiRitardati
        {
            get
            {
                return this.penalitaPagamentiRitardatiField;
            }
            set
            {
                this.penalitaPagamentiRitardatiField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool PenalitaPagamentiRitardatiSpecified
        {
            get
            {
                return this.penalitaPagamentiRitardatiFieldSpecified;
            }
            set
            {
                this.penalitaPagamentiRitardatiFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "date")]
        public System.DateTime DataDecorrenzaPenale
        {
            get
            {
                return this.dataDecorrenzaPenaleField;
            }
            set
            {
                this.dataDecorrenzaPenaleField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool DataDecorrenzaPenaleSpecified
        {
            get
            {
                return this.dataDecorrenzaPenaleFieldSpecified;
            }
            set
            {
                this.dataDecorrenzaPenaleFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "normalizedString")]
        public string CodicePagamento
        {
            get
            {
                return this.codicePagamentoField;
            }
            set
            {
                this.codicePagamentoField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public enum ModalitaPagamentoType
    {

        /// <remarks/>
        MP01,

        /// <remarks/>
        MP02,

        /// <remarks/>
        MP03,

        /// <remarks/>
        MP04,

        /// <remarks/>
        MP05,

        /// <remarks/>
        MP06,

        /// <remarks/>
        MP07,

        /// <remarks/>
        MP08,

        /// <remarks/>
        MP09,

        /// <remarks/>
        MP10,

        /// <remarks/>
        MP11,

        /// <remarks/>
        MP12,

        /// <remarks/>
        MP13,

        /// <remarks/>
        MP14,

        /// <remarks/>
        MP15,

        /// <remarks/>
        MP16,

        /// <remarks/>
        MP17,

        /// <remarks/>
        MP18,

        /// <remarks/>
        MP19,

        /// <remarks/>
        MP20,

        /// <remarks/>
        MP21,

        /// <remarks/>
        MP22,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public partial class DatiPagamentoType
    {

        private CondizioniPagamentoType condizioniPagamentoField;

        private DettaglioPagamentoType[] dettaglioPagamentoField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public CondizioniPagamentoType CondizioniPagamento
        {
            get
            {
                return this.condizioniPagamentoField;
            }
            set
            {
                this.condizioniPagamentoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("DettaglioPagamento", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public DettaglioPagamentoType[] DettaglioPagamento
        {
            get
            {
                return this.dettaglioPagamentoField;
            }
            set
            {
                this.dettaglioPagamentoField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public enum CondizioniPagamentoType
    {

        /// <remarks/>
        TP01,

        /// <remarks/>
        TP02,

        /// <remarks/>
        TP03,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public partial class DatiVeicoliType
    {

        private System.DateTime dataField;

        private string totalePercorsoField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "date")]
        public System.DateTime Data
        {
            get
            {
                return this.dataField;
            }
            set
            {
                this.dataField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "normalizedString")]
        public string TotalePercorso
        {
            get
            {
                return this.totalePercorsoField;
            }
            set
            {
                this.totalePercorsoField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public partial class DatiRiepilogoType
    {

        private decimal aliquotaIVAField;

        private NaturaType naturaField;

        private bool naturaFieldSpecified;

        private decimal speseAccessorieField;

        private bool speseAccessorieFieldSpecified;

        private decimal arrotondamentoField;

        private bool arrotondamentoFieldSpecified;

        private decimal imponibileImportoField;

        private decimal impostaField;

        private EsigibilitaIVAType esigibilitaIVAField;

        private bool esigibilitaIVAFieldSpecified;

        private string riferimentoNormativoField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public decimal AliquotaIVA
        {
            get
            {
                return this.aliquotaIVAField;
            }
            set
            {
                this.aliquotaIVAField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public NaturaType Natura
        {
            get
            {
                return this.naturaField;
            }
            set
            {
                this.naturaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool NaturaSpecified
        {
            get
            {
                return this.naturaFieldSpecified;
            }
            set
            {
                this.naturaFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public decimal SpeseAccessorie
        {
            get
            {
                return this.speseAccessorieField;
            }
            set
            {
                this.speseAccessorieField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool SpeseAccessorieSpecified
        {
            get
            {
                return this.speseAccessorieFieldSpecified;
            }
            set
            {
                this.speseAccessorieFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public decimal Arrotondamento
        {
            get
            {
                return this.arrotondamentoField;
            }
            set
            {
                this.arrotondamentoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ArrotondamentoSpecified
        {
            get
            {
                return this.arrotondamentoFieldSpecified;
            }
            set
            {
                this.arrotondamentoFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public decimal ImponibileImporto
        {
            get
            {
                return this.imponibileImportoField;
            }
            set
            {
                this.imponibileImportoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public decimal Imposta
        {
            get
            {
                return this.impostaField;
            }
            set
            {
                this.impostaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public EsigibilitaIVAType EsigibilitaIVA
        {
            get
            {
                return this.esigibilitaIVAField;
            }
            set
            {
                this.esigibilitaIVAField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool EsigibilitaIVASpecified
        {
            get
            {
                return this.esigibilitaIVAFieldSpecified;
            }
            set
            {
                this.esigibilitaIVAFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "normalizedString")]
        public string RiferimentoNormativo
        {
            get
            {
                return this.riferimentoNormativoField;
            }
            set
            {
                this.riferimentoNormativoField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public enum NaturaType
    {

        /// <remarks/>
        N1,

        /// <remarks/>
        N2,

        /// <remarks/>
        N3,

        /// <remarks/>
        N4,

        /// <remarks/>
        N5,

        /// <remarks/>
        N6,

        /// <remarks/>
        N7,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public enum EsigibilitaIVAType
    {

        /// <remarks/>
        D,

        /// <remarks/>
        I,

        /// <remarks/>
        S,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public partial class AltriDatiGestionaliType
    {

        private string tipoDatoField;

        private string riferimentoTestoField;

        private string riferimentoNumeroField;

        private bool riferimentoNumeroFieldSpecified;

        private System.DateTime riferimentoDataField;

        private bool riferimentoDataFieldSpecified;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "normalizedString")]
        public string TipoDato
        {
            get
            {
                return this.tipoDatoField;
            }
            set
            {
                this.tipoDatoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "normalizedString")]
        public string RiferimentoTesto
        {
            get
            {
                return this.riferimentoTestoField;
            }
            set
            {
                this.riferimentoTestoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string RiferimentoNumero
        {
            get
            {
                return this.riferimentoNumeroField;
            }
            set
            {
                this.riferimentoNumeroField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool RiferimentoNumeroSpecified
        {
            get
            {
                return this.riferimentoNumeroFieldSpecified;
            }
            set
            {
                this.riferimentoNumeroFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "date")]
        public System.DateTime RiferimentoData
        {
            get
            {
                return this.riferimentoDataField;
            }
            set
            {
                this.riferimentoDataField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool RiferimentoDataSpecified
        {
            get
            {
                return this.riferimentoDataFieldSpecified;
            }
            set
            {
                this.riferimentoDataFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public partial class CodiceArticoloType
    {

        private string codiceTipoField;

        private string codiceValoreField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "normalizedString")]
        public string CodiceTipo
        {
            get
            {
                return this.codiceTipoField;
            }
            set
            {
                this.codiceTipoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "normalizedString")]
        public string CodiceValore
        {
            get
            {
                return this.codiceValoreField;
            }
            set
            {
                this.codiceValoreField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public partial class DettaglioLineeType
    {

        private string numeroLineaField;

        private TipoCessionePrestazioneType tipoCessionePrestazioneField;

        private bool tipoCessionePrestazioneFieldSpecified;

        private CodiceArticoloType[] codiceArticoloField;

        private string descrizioneField;

        private decimal quantitaField;

        private bool quantitaFieldSpecified;

        private string unitaMisuraField;

        private System.DateTime dataInizioPeriodoField;

        private bool dataInizioPeriodoFieldSpecified;

        private System.DateTime dataFinePeriodoField;

        private bool dataFinePeriodoFieldSpecified;

        private decimal prezzoUnitarioField;

        private ScontoMaggiorazioneType[] scontoMaggiorazioneField;

        private decimal prezzoTotaleField;

        private decimal aliquotaIVAField;

        private RitenutaType ritenutaField;

        private bool ritenutaFieldSpecified;

        private NaturaType naturaField;

        private bool naturaFieldSpecified;

        private string riferimentoAmministrazioneField;

        private AltriDatiGestionaliType[] altriDatiGestionaliField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "integer")]
        public string NumeroLinea
        {
            get
            {
                return this.numeroLineaField;
            }
            set
            {
                this.numeroLineaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public TipoCessionePrestazioneType TipoCessionePrestazione
        {
            get
            {
                return this.tipoCessionePrestazioneField;
            }
            set
            {
                this.tipoCessionePrestazioneField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool TipoCessionePrestazioneSpecified
        {
            get
            {
                return this.tipoCessionePrestazioneFieldSpecified;
            }
            set
            {
                this.tipoCessionePrestazioneFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("CodiceArticolo", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public CodiceArticoloType[] CodiceArticolo
        {
            get
            {
                return this.codiceArticoloField;
            }
            set
            {
                this.codiceArticoloField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "normalizedString")]
        public string Descrizione
        {
            get
            {
                return this.descrizioneField;
            }
            set
            {
                this.descrizioneField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public decimal Quantita
        {
            get
            {
                return this.quantitaField;
            }
            set
            {
                this.quantitaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool QuantitaSpecified
        {
            get
            {
                return this.quantitaFieldSpecified;
            }
            set
            {
                this.quantitaFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "normalizedString")]
        public string UnitaMisura
        {
            get
            {
                return this.unitaMisuraField;
            }
            set
            {
                this.unitaMisuraField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "date")]
        public System.DateTime DataInizioPeriodo
        {
            get
            {
                return this.dataInizioPeriodoField;
            }
            set
            {
                this.dataInizioPeriodoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool DataInizioPeriodoSpecified
        {
            get
            {
                return this.dataInizioPeriodoFieldSpecified;
            }
            set
            {
                this.dataInizioPeriodoFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "date")]
        public System.DateTime DataFinePeriodo
        {
            get
            {
                return this.dataFinePeriodoField;
            }
            set
            {
                this.dataFinePeriodoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool DataFinePeriodoSpecified
        {
            get
            {
                return this.dataFinePeriodoFieldSpecified;
            }
            set
            {
                this.dataFinePeriodoFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public decimal PrezzoUnitario
        {
            get
            {
                return this.prezzoUnitarioField;
            }
            set
            {
                this.prezzoUnitarioField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ScontoMaggiorazione", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public ScontoMaggiorazioneType[] ScontoMaggiorazione
        {
            get
            {
                return this.scontoMaggiorazioneField;
            }
            set
            {
                this.scontoMaggiorazioneField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public decimal PrezzoTotale
        {
            get
            {
                return this.prezzoTotaleField;
            }
            set
            {
                this.prezzoTotaleField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public decimal AliquotaIVA
        {
            get
            {
                return this.aliquotaIVAField;
            }
            set
            {
                this.aliquotaIVAField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public RitenutaType Ritenuta
        {
            get
            {
                return this.ritenutaField;
            }
            set
            {
                this.ritenutaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool RitenutaSpecified
        {
            get
            {
                return this.ritenutaFieldSpecified;
            }
            set
            {
                this.ritenutaFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public NaturaType Natura
        {
            get
            {
                return this.naturaField;
            }
            set
            {
                this.naturaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool NaturaSpecified
        {
            get
            {
                return this.naturaFieldSpecified;
            }
            set
            {
                this.naturaFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "normalizedString")]
        public string RiferimentoAmministrazione
        {
            get
            {
                return this.riferimentoAmministrazioneField;
            }
            set
            {
                this.riferimentoAmministrazioneField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("AltriDatiGestionali", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public AltriDatiGestionaliType[] AltriDatiGestionali
        {
            get
            {
                return this.altriDatiGestionaliField;
            }
            set
            {
                this.altriDatiGestionaliField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public enum TipoCessionePrestazioneType
    {

        /// <remarks/>
        SC,

        /// <remarks/>
        PR,

        /// <remarks/>
        AB,

        /// <remarks/>
        AC,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public partial class ScontoMaggiorazioneType
    {

        private TipoScontoMaggiorazioneType tipoField;

        private decimal percentualeField;

        private bool percentualeFieldSpecified;

        private decimal importoField;

        private bool importoFieldSpecified;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public TipoScontoMaggiorazioneType Tipo
        {
            get
            {
                return this.tipoField;
            }
            set
            {
                this.tipoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public decimal Percentuale
        {
            get
            {
                return this.percentualeField;
            }
            set
            {
                this.percentualeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool PercentualeSpecified
        {
            get
            {
                return this.percentualeFieldSpecified;
            }
            set
            {
                this.percentualeFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public decimal Importo
        {
            get
            {
                return this.importoField;
            }
            set
            {
                this.importoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ImportoSpecified
        {
            get
            {
                return this.importoFieldSpecified;
            }
            set
            {
                this.importoFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public enum TipoScontoMaggiorazioneType
    {

        /// <remarks/>
        SC,

        /// <remarks/>
        MG,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public enum RitenutaType
    {

        /// <remarks/>
        SI,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public partial class DatiBeniServiziType
    {

        private DettaglioLineeType[] dettaglioLineeField;

        private DatiRiepilogoType[] datiRiepilogoField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("DettaglioLinee", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public DettaglioLineeType[] DettaglioLinee
        {
            get
            {
                return this.dettaglioLineeField;
            }
            set
            {
                this.dettaglioLineeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("DatiRiepilogo", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public DatiRiepilogoType[] DatiRiepilogo
        {
            get
            {
                return this.datiRiepilogoField;
            }
            set
            {
                this.datiRiepilogoField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public partial class FatturaPrincipaleType
    {

        private string numeroFatturaPrincipaleField;

        private System.DateTime dataFatturaPrincipaleField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "normalizedString")]
        public string NumeroFatturaPrincipale
        {
            get
            {
                return this.numeroFatturaPrincipaleField;
            }
            set
            {
                this.numeroFatturaPrincipaleField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "date")]
        public System.DateTime DataFatturaPrincipale
        {
            get
            {
                return this.dataFatturaPrincipaleField;
            }
            set
            {
                this.dataFatturaPrincipaleField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public partial class DatiAnagraficiVettoreType
    {

        private IdFiscaleType idFiscaleIVAField;

        private string codiceFiscaleField;

        private AnagraficaType anagraficaField;

        private string numeroLicenzaGuidaField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public IdFiscaleType IdFiscaleIVA
        {
            get
            {
                return this.idFiscaleIVAField;
            }
            set
            {
                this.idFiscaleIVAField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CodiceFiscale
        {
            get
            {
                return this.codiceFiscaleField;
            }
            set
            {
                this.codiceFiscaleField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public AnagraficaType Anagrafica
        {
            get
            {
                return this.anagraficaField;
            }
            set
            {
                this.anagraficaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "normalizedString")]
        public string NumeroLicenzaGuida
        {
            get
            {
                return this.numeroLicenzaGuidaField;
            }
            set
            {
                this.numeroLicenzaGuidaField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public partial class AnagraficaType
    {

        private string[] itemsField;

        private ItemsChoiceType[] itemsElementNameField;

        private string titoloField;

        private string codEORIField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Cognome", typeof(string), Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "normalizedString")]
        [System.Xml.Serialization.XmlElementAttribute("Denominazione", typeof(string), Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "normalizedString")]
        [System.Xml.Serialization.XmlElementAttribute("Nome", typeof(string), Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "normalizedString")]
        [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemsElementName")]
        public string[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ItemsElementName")]
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public ItemsChoiceType[] ItemsElementName
        {
            get
            {
                return this.itemsElementNameField;
            }
            set
            {
                this.itemsElementNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "normalizedString")]
        public string Titolo
        {
            get
            {
                return this.titoloField;
            }
            set
            {
                this.titoloField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CodEORI
        {
            get
            {
                return this.codEORIField;
            }
            set
            {
                this.codEORIField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2", IncludeInSchema = false)]
    public enum ItemsChoiceType
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute(":Cognome")]
        Cognome,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute(":Denominazione")]
        Denominazione,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute(":Nome")]
        Nome,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public partial class DatiTrasportoType
    {

        private DatiAnagraficiVettoreType datiAnagraficiVettoreField;

        private string mezzoTrasportoField;

        private string causaleTrasportoField;

        private string numeroColliField;

        private string descrizioneField;

        private string unitaMisuraPesoField;

        private decimal pesoLordoField;

        private bool pesoLordoFieldSpecified;

        private decimal pesoNettoField;

        private bool pesoNettoFieldSpecified;

        private System.DateTime dataOraRitiroField;

        private bool dataOraRitiroFieldSpecified;

        private System.DateTime dataInizioTrasportoField;

        private bool dataInizioTrasportoFieldSpecified;

        private string tipoResaField;

        private IndirizzoType indirizzoResaField;

        private System.DateTime dataOraConsegnaField;

        private bool dataOraConsegnaFieldSpecified;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public DatiAnagraficiVettoreType DatiAnagraficiVettore
        {
            get
            {
                return this.datiAnagraficiVettoreField;
            }
            set
            {
                this.datiAnagraficiVettoreField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "normalizedString")]
        public string MezzoTrasporto
        {
            get
            {
                return this.mezzoTrasportoField;
            }
            set
            {
                this.mezzoTrasportoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "normalizedString")]
        public string CausaleTrasporto
        {
            get
            {
                return this.causaleTrasportoField;
            }
            set
            {
                this.causaleTrasportoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "integer")]
        public string NumeroColli
        {
            get
            {
                return this.numeroColliField;
            }
            set
            {
                this.numeroColliField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "normalizedString")]
        public string Descrizione
        {
            get
            {
                return this.descrizioneField;
            }
            set
            {
                this.descrizioneField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "normalizedString")]
        public string UnitaMisuraPeso
        {
            get
            {
                return this.unitaMisuraPesoField;
            }
            set
            {
                this.unitaMisuraPesoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public decimal PesoLordo
        {
            get
            {
                return this.pesoLordoField;
            }
            set
            {
                this.pesoLordoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool PesoLordoSpecified
        {
            get
            {
                return this.pesoLordoFieldSpecified;
            }
            set
            {
                this.pesoLordoFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public decimal PesoNetto
        {
            get
            {
                return this.pesoNettoField;
            }
            set
            {
                this.pesoNettoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool PesoNettoSpecified
        {
            get
            {
                return this.pesoNettoFieldSpecified;
            }
            set
            {
                this.pesoNettoFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public System.DateTime DataOraRitiro
        {
            get
            {
                return this.dataOraRitiroField;
            }
            set
            {
                this.dataOraRitiroField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool DataOraRitiroSpecified
        {
            get
            {
                return this.dataOraRitiroFieldSpecified;
            }
            set
            {
                this.dataOraRitiroFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "date")]
        public System.DateTime DataInizioTrasporto
        {
            get
            {
                return this.dataInizioTrasportoField;
            }
            set
            {
                this.dataInizioTrasportoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool DataInizioTrasportoSpecified
        {
            get
            {
                return this.dataInizioTrasportoFieldSpecified;
            }
            set
            {
                this.dataInizioTrasportoFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string TipoResa
        {
            get
            {
                return this.tipoResaField;
            }
            set
            {
                this.tipoResaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public IndirizzoType IndirizzoResa
        {
            get
            {
                return this.indirizzoResaField;
            }
            set
            {
                this.indirizzoResaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public System.DateTime DataOraConsegna
        {
            get
            {
                return this.dataOraConsegnaField;
            }
            set
            {
                this.dataOraConsegnaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool DataOraConsegnaSpecified
        {
            get
            {
                return this.dataOraConsegnaFieldSpecified;
            }
            set
            {
                this.dataOraConsegnaFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public partial class IndirizzoType
    {

        private string indirizzoField;

        private string numeroCivicoField;

        private string cAPField;

        private string comuneField;

        private string provinciaField;

        private string nazioneField;

        public IndirizzoType()
        {
            this.nazioneField = "IT";
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "normalizedString")]
        public string Indirizzo
        {
            get
            {
                return this.indirizzoField;
            }
            set
            {
                this.indirizzoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "normalizedString")]
        public string NumeroCivico
        {
            get
            {
                return this.numeroCivicoField;
            }
            set
            {
                this.numeroCivicoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CAP
        {
            get
            {
                return this.cAPField;
            }
            set
            {
                this.cAPField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "normalizedString")]
        public string Comune
        {
            get
            {
                return this.comuneField;
            }
            set
            {
                this.comuneField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Provincia
        {
            get
            {
                return this.provinciaField;
            }
            set
            {
                this.provinciaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Nazione
        {
            get
            {
                return this.nazioneField;
            }
            set
            {
                this.nazioneField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public partial class DatiDDTType
    {

        private string numeroDDTField;

        private System.DateTime dataDDTField;

        private string[] riferimentoNumeroLineaField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "normalizedString")]
        public string NumeroDDT
        {
            get
            {
                return this.numeroDDTField;
            }
            set
            {
                this.numeroDDTField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "date")]
        public System.DateTime DataDDT
        {
            get
            {
                return this.dataDDTField;
            }
            set
            {
                this.dataDDTField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("RiferimentoNumeroLinea", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "integer")]
        public string[] RiferimentoNumeroLinea
        {
            get
            {
                return this.riferimentoNumeroLineaField;
            }
            set
            {
                this.riferimentoNumeroLineaField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public partial class DatiSALType
    {

        private string riferimentoFaseField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "integer")]
        public string RiferimentoFase
        {
            get
            {
                return this.riferimentoFaseField;
            }
            set
            {
                this.riferimentoFaseField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public partial class DatiDocumentiCorrelatiType
    {

        private string riferimentoNumeroLineaField;

        private string idDocumentoField;

        private System.DateTime dataField;

        private bool dataFieldSpecified;

        private string numItemField;

        private string codiceCommessaConvenzioneField;

        private string codiceCUPField;

        private string codiceCIGField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("RiferimentoNumeroLinea", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "integer")]
        public string RiferimentoNumeroLinea
        {
            get
            {
                return this.riferimentoNumeroLineaField;
            }
            set
            {
                this.riferimentoNumeroLineaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "normalizedString")]
        public string IdDocumento
        {
            get
            {
                return this.idDocumentoField;
            }
            set
            {
                this.idDocumentoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "date")]
        public System.DateTime Data
        {
            get
            {
                return this.dataField;
            }
            set
            {
                this.dataField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool DataSpecified
        {
            get
            {
                return this.dataFieldSpecified;
            }
            set
            {
                this.dataFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "normalizedString")]
        public string NumItem
        {
            get
            {
                return this.numItemField;
            }
            set
            {
                this.numItemField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "normalizedString")]
        public string CodiceCommessaConvenzione
        {
            get
            {
                return this.codiceCommessaConvenzioneField;
            }
            set
            {
                this.codiceCommessaConvenzioneField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "normalizedString")]
        public string CodiceCUP
        {
            get
            {
                return this.codiceCUPField;
            }
            set
            {
                this.codiceCUPField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "normalizedString")]
        public string CodiceCIG
        {
            get
            {
                return this.codiceCIGField;
            }
            set
            {
                this.codiceCIGField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public partial class DatiCassaPrevidenzialeType
    {

        private TipoCassaType tipoCassaField;

        private decimal alCassaField;

        private decimal importoContributoCassaField;

        private decimal imponibileCassaField;

        private bool imponibileCassaFieldSpecified;

        private decimal aliquotaIVAField;

        private RitenutaType ritenutaField;

        private bool ritenutaFieldSpecified;

        private NaturaType naturaField;

        private bool naturaFieldSpecified;

        private string riferimentoAmministrazioneField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public TipoCassaType TipoCassa
        {
            get
            {
                return this.tipoCassaField;
            }
            set
            {
                this.tipoCassaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public decimal AlCassa
        {
            get
            {
                return this.alCassaField;
            }
            set
            {
                this.alCassaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public decimal ImportoContributoCassa
        {
            get
            {
                return this.importoContributoCassaField;
            }
            set
            {
                this.importoContributoCassaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public decimal ImponibileCassa
        {
            get
            {
                return this.imponibileCassaField;
            }
            set
            {
                this.imponibileCassaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ImponibileCassaSpecified
        {
            get
            {
                return this.imponibileCassaFieldSpecified;
            }
            set
            {
                this.imponibileCassaFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public decimal AliquotaIVA
        {
            get
            {
                return this.aliquotaIVAField;
            }
            set
            {
                this.aliquotaIVAField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public RitenutaType Ritenuta
        {
            get
            {
                return this.ritenutaField;
            }
            set
            {
                this.ritenutaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool RitenutaSpecified
        {
            get
            {
                return this.ritenutaFieldSpecified;
            }
            set
            {
                this.ritenutaFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public NaturaType Natura
        {
            get
            {
                return this.naturaField;
            }
            set
            {
                this.naturaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool NaturaSpecified
        {
            get
            {
                return this.naturaFieldSpecified;
            }
            set
            {
                this.naturaFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "normalizedString")]
        public string RiferimentoAmministrazione
        {
            get
            {
                return this.riferimentoAmministrazioneField;
            }
            set
            {
                this.riferimentoAmministrazioneField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public enum TipoCassaType
    {

        /// <remarks/>
        TC01,

        /// <remarks/>
        TC02,

        /// <remarks/>
        TC03,

        /// <remarks/>
        TC04,

        /// <remarks/>
        TC05,

        /// <remarks/>
        TC06,

        /// <remarks/>
        TC07,

        /// <remarks/>
        TC08,

        /// <remarks/>
        TC09,

        /// <remarks/>
        TC10,

        /// <remarks/>
        TC11,

        /// <remarks/>
        TC12,

        /// <remarks/>
        TC13,

        /// <remarks/>
        TC14,

        /// <remarks/>
        TC15,

        /// <remarks/>
        TC16,

        /// <remarks/>
        TC17,

        /// <remarks/>
        TC18,

        /// <remarks/>
        TC19,

        /// <remarks/>
        TC20,

        /// <remarks/>
        TC21,

        /// <remarks/>
        TC22,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public partial class DatiBolloType
    {

        private BolloVirtualeType bolloVirtualeField;

        private decimal importoBolloField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public BolloVirtualeType BolloVirtuale
        {
            get
            {
                return this.bolloVirtualeField;
            }
            set
            {
                this.bolloVirtualeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public decimal ImportoBollo
        {
            get
            {
                return this.importoBolloField;
            }
            set
            {
                this.importoBolloField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public enum BolloVirtualeType
    {

        /// <remarks/>
        SI,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public partial class DatiRitenutaType
    {

        private TipoRitenutaType tipoRitenutaField;

        private decimal importoRitenutaField;

        private decimal aliquotaRitenutaField;

        private CausalePagamentoType causalePagamentoField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public TipoRitenutaType TipoRitenuta
        {
            get
            {
                return this.tipoRitenutaField;
            }
            set
            {
                this.tipoRitenutaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public decimal ImportoRitenuta
        {
            get
            {
                return this.importoRitenutaField;
            }
            set
            {
                this.importoRitenutaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public decimal AliquotaRitenuta
        {
            get
            {
                return this.aliquotaRitenutaField;
            }
            set
            {
                this.aliquotaRitenutaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public CausalePagamentoType CausalePagamento
        {
            get
            {
                return this.causalePagamentoField;
            }
            set
            {
                this.causalePagamentoField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public enum TipoRitenutaType
    {

        /// <remarks/>
        RT01,

        /// <remarks/>
        RT02,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public enum CausalePagamentoType
    {

        /// <remarks/>
        A,

        /// <remarks/>
        B,

        /// <remarks/>
        C,

        /// <remarks/>
        D,

        /// <remarks/>
        E,

        /// <remarks/>
        G,

        /// <remarks/>
        H,

        /// <remarks/>
        I,

        /// <remarks/>
        L,

        /// <remarks/>
        M,

        /// <remarks/>
        N,

        /// <remarks/>
        O,

        /// <remarks/>
        P,

        /// <remarks/>
        Q,

        /// <remarks/>
        R,

        /// <remarks/>
        S,

        /// <remarks/>
        T,

        /// <remarks/>
        U,

        /// <remarks/>
        V,

        /// <remarks/>
        W,

        /// <remarks/>
        X,

        /// <remarks/>
        Y,

        /// <remarks/>
        Z,

        /// <remarks/>
        L1,

        /// <remarks/>
        M1,

        /// <remarks/>
        O1,

        /// <remarks/>
        V1,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public partial class DatiGeneraliDocumentoType
    {

        private TipoDocumentoType tipoDocumentoField;

        private string divisaField;

        private System.DateTime dataField;

        private string numeroField;

        private DatiRitenutaType datiRitenutaField;

        private DatiBolloType datiBolloField;

        private DatiCassaPrevidenzialeType[] datiCassaPrevidenzialeField;

        private ScontoMaggiorazioneType[] scontoMaggiorazioneField;

        private decimal importoTotaleDocumentoField;

        private bool importoTotaleDocumentoFieldSpecified;

        private decimal arrotondamentoField;

        private bool arrotondamentoFieldSpecified;

        private string[] causaleField;

        private Art73Type art73Field;

        private bool art73FieldSpecified;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public TipoDocumentoType TipoDocumento
        {
            get
            {
                return this.tipoDocumentoField;
            }
            set
            {
                this.tipoDocumentoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Divisa
        {
            get
            {
                return this.divisaField;
            }
            set
            {
                this.divisaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "date")]
        public System.DateTime Data
        {
            get
            {
                return this.dataField;
            }
            set
            {
                this.dataField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "normalizedString")]
        public string Numero
        {
            get
            {
                return this.numeroField;
            }
            set
            {
                this.numeroField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public DatiRitenutaType DatiRitenuta
        {
            get
            {
                return this.datiRitenutaField;
            }
            set
            {
                this.datiRitenutaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public DatiBolloType DatiBollo
        {
            get
            {
                return this.datiBolloField;
            }
            set
            {
                this.datiBolloField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("DatiCassaPrevidenziale", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public DatiCassaPrevidenzialeType[] DatiCassaPrevidenziale
        {
            get
            {
                return this.datiCassaPrevidenzialeField;
            }
            set
            {
                this.datiCassaPrevidenzialeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ScontoMaggiorazione", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public ScontoMaggiorazioneType[] ScontoMaggiorazione
        {
            get
            {
                return this.scontoMaggiorazioneField;
            }
            set
            {
                this.scontoMaggiorazioneField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public decimal ImportoTotaleDocumento
        {
            get
            {
                return this.importoTotaleDocumentoField;
            }
            set
            {
                this.importoTotaleDocumentoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ImportoTotaleDocumentoSpecified
        {
            get
            {
                return this.importoTotaleDocumentoFieldSpecified;
            }
            set
            {
                this.importoTotaleDocumentoFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public decimal Arrotondamento
        {
            get
            {
                return this.arrotondamentoField;
            }
            set
            {
                this.arrotondamentoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ArrotondamentoSpecified
        {
            get
            {
                return this.arrotondamentoFieldSpecified;
            }
            set
            {
                this.arrotondamentoFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Causale", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "normalizedString")]
        public string[] Causale
        {
            get
            {
                return this.causaleField;
            }
            set
            {
                this.causaleField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public Art73Type Art73
        {
            get
            {
                return this.art73Field;
            }
            set
            {
                this.art73Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool Art73Specified
        {
            get
            {
                return this.art73FieldSpecified;
            }
            set
            {
                this.art73FieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public enum TipoDocumentoType
    {

        /// <remarks/>
        TD01,

        /// <remarks/>
        TD02,

        /// <remarks/>
        TD03,

        /// <remarks/>
        TD04,

        /// <remarks/>
        TD05,

        /// <remarks/>
        TD06,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public enum Art73Type
    {

        /// <remarks/>
        SI,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public partial class DatiGeneraliType
    {

        private DatiGeneraliDocumentoType datiGeneraliDocumentoField;

        private DatiDocumentiCorrelatiType[] datiOrdineAcquistoField;

        private DatiDocumentiCorrelatiType[] datiContrattoField;

        private DatiDocumentiCorrelatiType[] datiConvenzioneField;

        private DatiDocumentiCorrelatiType[] datiRicezioneField;

        private DatiDocumentiCorrelatiType[] datiFattureCollegateField;

        private DatiSALType[] datiSALField;

        private DatiDDTType[] datiDDTField;

        private DatiTrasportoType datiTrasportoField;

        private FatturaPrincipaleType fatturaPrincipaleField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public DatiGeneraliDocumentoType DatiGeneraliDocumento
        {
            get
            {
                return this.datiGeneraliDocumentoField;
            }
            set
            {
                this.datiGeneraliDocumentoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("DatiOrdineAcquisto", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public DatiDocumentiCorrelatiType[] DatiOrdineAcquisto
        {
            get
            {
                return this.datiOrdineAcquistoField;
            }
            set
            {
                this.datiOrdineAcquistoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("DatiContratto", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public DatiDocumentiCorrelatiType[] DatiContratto
        {
            get
            {
                return this.datiContrattoField;
            }
            set
            {
                this.datiContrattoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("DatiConvenzione", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public DatiDocumentiCorrelatiType[] DatiConvenzione
        {
            get
            {
                return this.datiConvenzioneField;
            }
            set
            {
                this.datiConvenzioneField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("DatiRicezione", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public DatiDocumentiCorrelatiType[] DatiRicezione
        {
            get
            {
                return this.datiRicezioneField;
            }
            set
            {
                this.datiRicezioneField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("DatiFattureCollegate", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public DatiDocumentiCorrelatiType[] DatiFattureCollegate
        {
            get
            {
                return this.datiFattureCollegateField;
            }
            set
            {
                this.datiFattureCollegateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("DatiSAL", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public DatiSALType[] DatiSAL
        {
            get
            {
                return this.datiSALField;
            }
            set
            {
                this.datiSALField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("DatiDDT", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public DatiDDTType[] DatiDDT
        {
            get
            {
                return this.datiDDTField;
            }
            set
            {
                this.datiDDTField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public DatiTrasportoType DatiTrasporto
        {
            get
            {
                return this.datiTrasportoField;
            }
            set
            {
                this.datiTrasportoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public FatturaPrincipaleType FatturaPrincipale
        {
            get
            {
                return this.fatturaPrincipaleField;
            }
            set
            {
                this.fatturaPrincipaleField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public partial class FatturaElettronicaBodyType
    {

        private DatiGeneraliType datiGeneraliField;

        private DatiBeniServiziType datiBeniServiziField;

        private DatiVeicoliType datiVeicoliField;

        private DatiPagamentoType[] datiPagamentoField;

        private AllegatiType[] allegatiField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public DatiGeneraliType DatiGenerali
        {
            get
            {
                return this.datiGeneraliField;
            }
            set
            {
                this.datiGeneraliField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public DatiBeniServiziType DatiBeniServizi
        {
            get
            {
                return this.datiBeniServiziField;
            }
            set
            {
                this.datiBeniServiziField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public DatiVeicoliType DatiVeicoli
        {
            get
            {
                return this.datiVeicoliField;
            }
            set
            {
                this.datiVeicoliField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("DatiPagamento", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public DatiPagamentoType[] DatiPagamento
        {
            get
            {
                return this.datiPagamentoField;
            }
            set
            {
                this.datiPagamentoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Allegati", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public AllegatiType[] Allegati
        {
            get
            {
                return this.allegatiField;
            }
            set
            {
                this.allegatiField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public partial class DatiAnagraficiTerzoIntermediarioType
    {

        private IdFiscaleType idFiscaleIVAField;

        private string codiceFiscaleField;

        private AnagraficaType anagraficaField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public IdFiscaleType IdFiscaleIVA
        {
            get
            {
                return this.idFiscaleIVAField;
            }
            set
            {
                this.idFiscaleIVAField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CodiceFiscale
        {
            get
            {
                return this.codiceFiscaleField;
            }
            set
            {
                this.codiceFiscaleField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public AnagraficaType Anagrafica
        {
            get
            {
                return this.anagraficaField;
            }
            set
            {
                this.anagraficaField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public partial class TerzoIntermediarioSoggettoEmittenteType
    {

        private DatiAnagraficiTerzoIntermediarioType datiAnagraficiField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public DatiAnagraficiTerzoIntermediarioType DatiAnagrafici
        {
            get
            {
                return this.datiAnagraficiField;
            }
            set
            {
                this.datiAnagraficiField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public partial class RappresentanteFiscaleCessionarioType
    {

        private IdFiscaleType idFiscaleIVAField;

        private string[] itemsField;

        private ItemsChoiceType1[] itemsElementNameField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public IdFiscaleType IdFiscaleIVA
        {
            get
            {
                return this.idFiscaleIVAField;
            }
            set
            {
                this.idFiscaleIVAField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Cognome", typeof(string), Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "normalizedString")]
        [System.Xml.Serialization.XmlElementAttribute("Denominazione", typeof(string), Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "normalizedString")]
        [System.Xml.Serialization.XmlElementAttribute("Nome", typeof(string), Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "normalizedString")]
        [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemsElementName")]
        public string[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ItemsElementName")]
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public ItemsChoiceType1[] ItemsElementName
        {
            get
            {
                return this.itemsElementNameField;
            }
            set
            {
                this.itemsElementNameField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2", IncludeInSchema = false)]
    public enum ItemsChoiceType1
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute(":Cognome")]
        Cognome,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute(":Denominazione")]
        Denominazione,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute(":Nome")]
        Nome,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public partial class DatiAnagraficiCessionarioType
    {

        private IdFiscaleType idFiscaleIVAField;

        private string codiceFiscaleField;

        private AnagraficaType anagraficaField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public IdFiscaleType IdFiscaleIVA
        {
            get
            {
                return this.idFiscaleIVAField;
            }
            set
            {
                this.idFiscaleIVAField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CodiceFiscale
        {
            get
            {
                return this.codiceFiscaleField;
            }
            set
            {
                this.codiceFiscaleField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public AnagraficaType Anagrafica
        {
            get
            {
                return this.anagraficaField;
            }
            set
            {
                this.anagraficaField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public partial class CessionarioCommittenteType
    {

        private DatiAnagraficiCessionarioType datiAnagraficiField;

        private IndirizzoType sedeField;

        private IndirizzoType stabileOrganizzazioneField;

        private RappresentanteFiscaleCessionarioType rappresentanteFiscaleField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public DatiAnagraficiCessionarioType DatiAnagrafici
        {
            get
            {
                return this.datiAnagraficiField;
            }
            set
            {
                this.datiAnagraficiField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public IndirizzoType Sede
        {
            get
            {
                return this.sedeField;
            }
            set
            {
                this.sedeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public IndirizzoType StabileOrganizzazione
        {
            get
            {
                return this.stabileOrganizzazioneField;
            }
            set
            {
                this.stabileOrganizzazioneField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public RappresentanteFiscaleCessionarioType RappresentanteFiscale
        {
            get
            {
                return this.rappresentanteFiscaleField;
            }
            set
            {
                this.rappresentanteFiscaleField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public partial class DatiAnagraficiRappresentanteType
    {

        private IdFiscaleType idFiscaleIVAField;

        private string codiceFiscaleField;

        private AnagraficaType anagraficaField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public IdFiscaleType IdFiscaleIVA
        {
            get
            {
                return this.idFiscaleIVAField;
            }
            set
            {
                this.idFiscaleIVAField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CodiceFiscale
        {
            get
            {
                return this.codiceFiscaleField;
            }
            set
            {
                this.codiceFiscaleField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public AnagraficaType Anagrafica
        {
            get
            {
                return this.anagraficaField;
            }
            set
            {
                this.anagraficaField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public partial class RappresentanteFiscaleType
    {

        private DatiAnagraficiRappresentanteType datiAnagraficiField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public DatiAnagraficiRappresentanteType DatiAnagrafici
        {
            get
            {
                return this.datiAnagraficiField;
            }
            set
            {
                this.datiAnagraficiField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public partial class ContattiType
    {

        private string telefonoField;

        private string faxField;

        private string emailField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "normalizedString")]
        public string Telefono
        {
            get
            {
                return this.telefonoField;
            }
            set
            {
                this.telefonoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "normalizedString")]
        public string Fax
        {
            get
            {
                return this.faxField;
            }
            set
            {
                this.faxField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Email
        {
            get
            {
                return this.emailField;
            }
            set
            {
                this.emailField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public partial class IscrizioneREAType
    {

        private string ufficioField;

        private string numeroREAField;

        private decimal capitaleSocialeField;

        private bool capitaleSocialeFieldSpecified;

        private SocioUnicoType socioUnicoField;

        private bool socioUnicoFieldSpecified;

        private StatoLiquidazioneType statoLiquidazioneField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Ufficio
        {
            get
            {
                return this.ufficioField;
            }
            set
            {
                this.ufficioField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "normalizedString")]
        public string NumeroREA
        {
            get
            {
                return this.numeroREAField;
            }
            set
            {
                this.numeroREAField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public decimal CapitaleSociale
        {
            get
            {
                return this.capitaleSocialeField;
            }
            set
            {
                this.capitaleSocialeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool CapitaleSocialeSpecified
        {
            get
            {
                return this.capitaleSocialeFieldSpecified;
            }
            set
            {
                this.capitaleSocialeFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public SocioUnicoType SocioUnico
        {
            get
            {
                return this.socioUnicoField;
            }
            set
            {
                this.socioUnicoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool SocioUnicoSpecified
        {
            get
            {
                return this.socioUnicoFieldSpecified;
            }
            set
            {
                this.socioUnicoFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public StatoLiquidazioneType StatoLiquidazione
        {
            get
            {
                return this.statoLiquidazioneField;
            }
            set
            {
                this.statoLiquidazioneField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public enum SocioUnicoType
    {

        /// <remarks/>
        SU,

        /// <remarks/>
        SM,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public enum StatoLiquidazioneType
    {

        /// <remarks/>
        LS,

        /// <remarks/>
        LN,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public partial class DatiAnagraficiCedenteType
    {

        private IdFiscaleType idFiscaleIVAField;

        private string codiceFiscaleField;

        private AnagraficaType anagraficaField;

        private string alboProfessionaleField;

        private string provinciaAlboField;

        private string numeroIscrizioneAlboField;

        private System.DateTime dataIscrizioneAlboField;

        private bool dataIscrizioneAlboFieldSpecified;

        private RegimeFiscaleType regimeFiscaleField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public IdFiscaleType IdFiscaleIVA
        {
            get
            {
                return this.idFiscaleIVAField;
            }
            set
            {
                this.idFiscaleIVAField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CodiceFiscale
        {
            get
            {
                return this.codiceFiscaleField;
            }
            set
            {
                this.codiceFiscaleField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public AnagraficaType Anagrafica
        {
            get
            {
                return this.anagraficaField;
            }
            set
            {
                this.anagraficaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "normalizedString")]
        public string AlboProfessionale
        {
            get
            {
                return this.alboProfessionaleField;
            }
            set
            {
                this.alboProfessionaleField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ProvinciaAlbo
        {
            get
            {
                return this.provinciaAlboField;
            }
            set
            {
                this.provinciaAlboField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "normalizedString")]
        public string NumeroIscrizioneAlbo
        {
            get
            {
                return this.numeroIscrizioneAlboField;
            }
            set
            {
                this.numeroIscrizioneAlboField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "date")]
        public System.DateTime DataIscrizioneAlbo
        {
            get
            {
                return this.dataIscrizioneAlboField;
            }
            set
            {
                this.dataIscrizioneAlboField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool DataIscrizioneAlboSpecified
        {
            get
            {
                return this.dataIscrizioneAlboFieldSpecified;
            }
            set
            {
                this.dataIscrizioneAlboFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public RegimeFiscaleType RegimeFiscale
        {
            get
            {
                return this.regimeFiscaleField;
            }
            set
            {
                this.regimeFiscaleField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public enum RegimeFiscaleType
    {

        /// <remarks/>
        RF01,

        /// <remarks/>
        RF02,

        /// <remarks/>
        RF03,

        /// <remarks/>
        RF04,

        /// <remarks/>
        RF05,

        /// <remarks/>
        RF06,

        /// <remarks/>
        RF07,

        /// <remarks/>
        RF08,

        /// <remarks/>
        RF09,

        /// <remarks/>
        RF10,

        /// <remarks/>
        RF11,

        /// <remarks/>
        RF12,

        /// <remarks/>
        RF13,

        /// <remarks/>
        RF14,

        /// <remarks/>
        RF15,

        /// <remarks/>
        RF16,

        /// <remarks/>
        RF17,

        /// <remarks/>
        RF19,

        /// <remarks/>
        RF18,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public partial class CedentePrestatoreType
    {

        private DatiAnagraficiCedenteType datiAnagraficiField;

        private IndirizzoType sedeField;

        private IndirizzoType stabileOrganizzazioneField;

        private IscrizioneREAType iscrizioneREAField;

        private ContattiType contattiField;

        private string riferimentoAmministrazioneField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public DatiAnagraficiCedenteType DatiAnagrafici
        {
            get
            {
                return this.datiAnagraficiField;
            }
            set
            {
                this.datiAnagraficiField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public IndirizzoType Sede
        {
            get
            {
                return this.sedeField;
            }
            set
            {
                this.sedeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public IndirizzoType StabileOrganizzazione
        {
            get
            {
                return this.stabileOrganizzazioneField;
            }
            set
            {
                this.stabileOrganizzazioneField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public IscrizioneREAType IscrizioneREA
        {
            get
            {
                return this.iscrizioneREAField;
            }
            set
            {
                this.iscrizioneREAField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public ContattiType Contatti
        {
            get
            {
                return this.contattiField;
            }
            set
            {
                this.contattiField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "normalizedString")]
        public string RiferimentoAmministrazione
        {
            get
            {
                return this.riferimentoAmministrazioneField;
            }
            set
            {
                this.riferimentoAmministrazioneField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public partial class ContattiTrasmittenteType
    {

        private string telefonoField;

        private string emailField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "normalizedString")]
        public string Telefono
        {
            get
            {
                return this.telefonoField;
            }
            set
            {
                this.telefonoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Email
        {
            get
            {
                return this.emailField;
            }
            set
            {
                this.emailField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public enum FormatoTrasmissioneType
    {

        /// <remarks/>
        FPA12,

        /// <remarks/>
        FPR12,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2")]
    public enum SoggettoEmittenteType
    {

        /// <remarks/>
        CC,

        /// <remarks/>
        TZ,
    }
}
