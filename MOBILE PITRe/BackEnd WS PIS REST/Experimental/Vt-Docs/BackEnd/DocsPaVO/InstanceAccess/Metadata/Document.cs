using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.InstanceAccess.Metadata
{
    public partial class Document
    {

        private SoggettoProduttore soggettoProduttoreField;
        private Registrazione registrazioneField;
        private ContestoArchivistico contestoArchivisticoField;
        private Tipologia tipologiaField;
        private Allegato[] allegatiField;
        private object[] eventiField;
        private File fileField;
        private string iDdocumentoField;
        private string dataCreazioneField;
        private string oggettoField;
        private string tipoField;
        private string livelloRiservatezzaField;
        private string tipoRichiesta;

        /// <remarks/>
        public SoggettoProduttore SoggettoProduttore
        {
            get
            {
                return this.soggettoProduttoreField;
            }
            set
            {
                this.soggettoProduttoreField = value;
            }
        }

        /// <remarks/>
        public Registrazione Registrazione
        {
            get
            {
                return this.registrazioneField;
            }
            set
            {
                this.registrazioneField = value;
            }
        }

        /// <remarks/>
        public ContestoArchivistico ContestoArchivistico
        {
            get
            {
                return this.contestoArchivisticoField;
            }
            set
            {
                this.contestoArchivisticoField = value;
            }
        }

        /// <remarks/>
        public Tipologia Tipologia
        {
            get
            {
                return this.tipologiaField;
            }
            set
            {
                this.tipologiaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable = false)]
        public Allegato[] Allegati
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

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Evento", IsNullable = false)]
        public object[] Eventi
        {
            get
            {
                return this.eventiField;
            }
            set
            {
                this.eventiField = value;
            }
        }

        /// <remarks/>
        public File File
        {
            get
            {
                return this.fileField;
            }
            set
            {
                this.fileField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string IDdocumento
        {
            get
            {
                return this.iDdocumentoField;
            }
            set
            {
                this.iDdocumentoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string DataCreazione
        {
            get
            {
                return this.dataCreazioneField;
            }
            set
            {
                this.dataCreazioneField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Oggetto
        {
            get
            {
                return this.oggettoField;
            }
            set
            {
                this.oggettoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Tipo
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
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string LivelloRiservatezza
        {
            get
            {
                return this.livelloRiservatezzaField;
            }
            set
            {
                this.livelloRiservatezzaField = value;
            }
        }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string TipoRichiesta
        {
            get
            {
                return this.tipoRichiesta;
            }
            set
            {
                this.tipoRichiesta = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class SoggettoProduttore
    {

        private Amministrazione amministrazioneField;
        private GerarchiaUO gerarchiaUOField;
        private Creatore creatoreField;

        /// <remarks/>
        public Amministrazione Amministrazione
        {
            get
            {
                return this.amministrazioneField;
            }
            set
            {
                this.amministrazioneField = value;
            }
        }

        /// <remarks/>
        public GerarchiaUO GerarchiaUO
        {
            get
            {
                return this.gerarchiaUOField;
            }
            set
            {
                this.gerarchiaUOField = value;
            }
        }

        /// <remarks/>
        public Creatore Creatore
        {
            get
            {
                return this.creatoreField;
            }
            set
            {
                this.creatoreField = value;
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class Creatore
    {

        private string codiceRuoloField;
        private string descrizioneRuoloField;
        private string codiceUtenteField;
        private string descrizioneUtenteField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string CodiceRuolo
        {
            get
            {
                return this.codiceRuoloField;
            }
            set
            {
                this.codiceRuoloField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string DescrizioneRuolo
        {
            get
            {
                return this.descrizioneRuoloField;
            }
            set
            {
                this.descrizioneRuoloField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string CodiceUtente
        {
            get
            {
                return this.codiceUtenteField;
            }
            set
            {
                this.codiceUtenteField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string DescrizioneUtente
        {
            get
            {
                return this.descrizioneUtenteField;
            }
            set
            {
                this.descrizioneUtenteField = value;
            }
        }
    }


    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class Amministrazione
    {

        private string codiceAmministrazioneField;
        private string descrizioneAmministrazioneField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string CodiceAmministrazione
        {
            get
            {
                return this.codiceAmministrazioneField;
            }
            set
            {
                this.codiceAmministrazioneField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string DescrizioneAmministrazione
        {
            get
            {
                return this.descrizioneAmministrazioneField;
            }
            set
            {
                this.descrizioneAmministrazioneField = value;
            }
        }
    }


    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class Allegato
    {

        private File fileField;
        private string tipoField;
        private string idField;
        private string descrizioneField;

        /// <remarks/>
        public File File
        {
            get
            {
                return this.fileField;
            }
            set
            {
                this.fileField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Tipo
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
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
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
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class File
    {

        private FirmaDigitale firmaDigitaleField;
        private MarcaTemporale marcaTemporaleField;
        private string formatoField;
        private string dimensioneField;
        private string improntaField;
        private string algoritmoHashField;

        /// <remarks/>
        public FirmaDigitale FirmaDigitale
        {
            get
            {
                return this.firmaDigitaleField;
            }
            set
            {
                this.firmaDigitaleField = value;
            }
        }

        /// <remarks/>
        public MarcaTemporale MarcaTemporale
        {
            get
            {
                return this.marcaTemporaleField;
            }
            set
            {
                this.marcaTemporaleField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Formato
        {
            get
            {
                return this.formatoField;
            }
            set
            {
                this.formatoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Dimensione
        {
            get
            {
                return this.dimensioneField;
            }
            set
            {
                this.dimensioneField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Impronta
        {
            get
            {
                return this.improntaField;
            }
            set
            {
                this.improntaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string AlgoritmoHash
        {
            get
            {
                return this.algoritmoHashField;
            }
            set
            {
                this.algoritmoHashField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class FirmaDigitale
    {

        private Titolare titolareField;
        private object certificatoField;
        private object datiFirmaField;

        /// <remarks/>
        public Titolare Titolare
        {
            get
            {
                return this.titolareField;
            }
            set
            {
                this.titolareField = value;
            }
        }

        /// <remarks/>
        public object Certificato
        {
            get
            {
                return this.certificatoField;
            }
            set
            {
                this.certificatoField = value;
            }
        }

        /// <remarks/>
        public object DatiFirma
        {
            get
            {
                return this.datiFirmaField;
            }
            set
            {
                this.datiFirmaField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class Titolare
    {

        private string nomeField;
        private string cognomeField;
        private string codiceFiscaleField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Nome
        {
            get
            {
                return this.nomeField;
            }
            set
            {
                this.nomeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Cognome
        {
            get
            {
                return this.cognomeField;
            }
            set
            {
                this.cognomeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
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
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class MarcaTemporale
    {

        private string numeroSerieField;
        private string dataField;
        private string oraField;
        private string sNCertificatoField;
        private string dataInizioValiditaField;
        private string dataFineValiditaField;
        private string improntaDocumentoAssociatoField;
        private string timeStampingAuthorityField;
        private string codiceFiscaleField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string NumeroSerie
        {
            get
            {
                return this.numeroSerieField;
            }
            set
            {
                this.numeroSerieField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Data
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
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Ora
        {
            get
            {
                return this.oraField;
            }
            set
            {
                this.oraField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string SNCertificato
        {
            get
            {
                return this.sNCertificatoField;
            }
            set
            {
                this.sNCertificatoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string DataInizioValidita
        {
            get
            {
                return this.dataInizioValiditaField;
            }
            set
            {
                this.dataInizioValiditaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string DataFineValidita
        {
            get
            {
                return this.dataFineValiditaField;
            }
            set
            {
                this.dataFineValiditaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ImprontaDocumentoAssociato
        {
            get
            {
                return this.improntaDocumentoAssociatoField;
            }
            set
            {
                this.improntaDocumentoAssociatoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string TimeStampingAuthority
        {
            get
            {
                return this.timeStampingAuthorityField;
            }
            set
            {
                this.timeStampingAuthorityField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
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
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class GerarchiaUO
    {

        private UnitaOrganizzativa[] unitaOrganizzativaField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("UnitaOrganizzativa")]
        public UnitaOrganizzativa[] UnitaOrganizzativa
        {
            get
            {
                return this.unitaOrganizzativaField;
            }
            set
            {
                this.unitaOrganizzativaField = value;
            }
        }
    }


    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class UnitaOrganizzativa
    {

        private UnitaOrganizzativa[] unitaOrganizzativa1Field;
        private string codiceUOField;
        private string descrizioneUOField;
        private string livelloField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("UnitaOrganizzativa")]
        public UnitaOrganizzativa[] UnitaOrganizzativa1
        {
            get
            {
                return this.unitaOrganizzativa1Field;
            }
            set
            {
                this.unitaOrganizzativa1Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string CodiceUO
        {
            get
            {
                return this.codiceUOField;
            }
            set
            {
                this.codiceUOField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string DescrizioneUO
        {
            get
            {
                return this.descrizioneUOField;
            }
            set
            {
                this.descrizioneUOField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Livello
        {
            get
            {
                return this.livelloField;
            }
            set
            {
                this.livelloField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class Registrazione
    {
        private Mittente[] mittenteField;
        private Destinatario[] destinatarioField;
        private ProtocolloMittente protocolloMittenteField;
        private Protocollista protocollistaField;
        private string codiceAOOField;
        private string descrizioneAOOField;
        private string codiceRFField;
        private string descrizioneRFField;
        private string segnaturaProtocolloField;
        private string numeroProtocolloField;
        private string tipoProtocolloField;
        private string dataProtocolloField;
        private string oraProtocolloField;
        private string segnaturaEmergenzaField;
        private string numeroProtocolloEmergenzaField;
        private string dataProtocolloEmergenzaField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Mittente")]
        public Mittente[] Mittente
        {
            get
            {
                return this.mittenteField;
            }
            set
            {
                this.mittenteField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Destinatario")]
        public Destinatario[] Destinatario
        {
            get
            {
                return this.destinatarioField;
            }
            set
            {
                this.destinatarioField = value;
            }
        }

        /// <remarks/>
        public ProtocolloMittente ProtocolloMittente
        {
            get
            {
                return this.protocolloMittenteField;
            }
            set
            {
                this.protocolloMittenteField = value;
            }
        }

        /// <remarks/>
        public Protocollista Protocollista
        {
            get
            {
                return this.protocollistaField;
            }
            set
            {
                this.protocollistaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string CodiceAOO
        {
            get
            {
                return this.codiceAOOField;
            }
            set
            {
                this.codiceAOOField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string DescrizioneAOO
        {
            get
            {
                return this.descrizioneAOOField;
            }
            set
            {
                this.descrizioneAOOField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string CodiceRF
        {
            get
            {
                return this.codiceRFField;
            }
            set
            {
                this.codiceRFField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string DescrizioneRF
        {
            get
            {
                return this.descrizioneRFField;
            }
            set
            {
                this.descrizioneRFField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string SegnaturaProtocollo
        {
            get
            {
                return this.segnaturaProtocolloField;
            }
            set
            {
                this.segnaturaProtocolloField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string NumeroProtocollo
        {
            get
            {
                return this.numeroProtocolloField;
            }
            set
            {
                this.numeroProtocolloField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string TipoProtocollo
        {
            get
            {
                return this.tipoProtocolloField;
            }
            set
            {
                this.tipoProtocolloField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string DataProtocollo
        {
            get
            {
                return this.dataProtocolloField;
            }
            set
            {
                this.dataProtocolloField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string OraProtocollo
        {
            get
            {
                return this.oraProtocolloField;
            }
            set
            {
                this.oraProtocolloField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string SegnaturaEmergenza
        {
            get
            {
                return this.segnaturaEmergenzaField;
            }
            set
            {
                this.segnaturaEmergenzaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string NumeroProtocolloEmergenza
        {
            get
            {
                return this.numeroProtocolloEmergenzaField;
            }
            set
            {
                this.numeroProtocolloEmergenzaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string DataProtocolloEmergenza
        {
            get
            {
                return this.dataProtocolloEmergenzaField;
            }
            set
            {
                this.dataProtocolloEmergenzaField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class Mittente
    {

        private string codiceField;
        private string descrizioneField;
        private string dataArrivoField;
        private string protocolloMittenteField;
        private string dataProtocolloMittenteField;
        private string indirizzoMailField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Codice
        {
            get
            {
                return this.codiceField;
            }
            set
            {
                this.codiceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
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
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string DataArrivo
        {
            get
            {
                return this.dataArrivoField;
            }
            set
            {
                this.dataArrivoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ProtocolloMittente
        {
            get
            {
                return this.protocolloMittenteField;
            }
            set
            {
                this.protocolloMittenteField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string DataProtocolloMittente
        {
            get
            {
                return this.dataProtocolloMittenteField;
            }
            set
            {
                this.dataProtocolloMittenteField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string IndirizzoMail
        {
            get
            {
                return this.indirizzoMailField;
            }
            set
            {
                this.indirizzoMailField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class Destinatario
    {
        private string codiceField;
        private string descrizioneField;
        private string mezzoSpedizioneField;
        private string indirizzoMailField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Codice
        {
            get
            {
                return this.codiceField;
            }
            set
            {
                this.codiceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
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
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string MezzoSpedizione
        {
            get
            {
                return this.mezzoSpedizioneField;
            }
            set
            {
                this.mezzoSpedizioneField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string IndirizzoMail
        {
            get
            {
                return this.indirizzoMailField;
            }
            set
            {
                this.indirizzoMailField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class ProtocolloMittente
    {

        private string protocolloField;
        private string dataField;
        private string mezzoSpedizioneField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Protocollo
        {
            get
            {
                return this.protocolloField;
            }
            set
            {
                this.protocolloField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Data
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
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string MezzoSpedizione
        {
            get
            {
                return this.mezzoSpedizioneField;
            }
            set
            {
                this.mezzoSpedizioneField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class Protocollista
    {
        private string codiceUtenteField;
        private string descrizioneUtenteField;
        private string codiceRuoloField;
        private string descrizioneRuoloField;
        private string uOAppartenenzaField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string CodiceUtente
        {
            get
            {
                return this.codiceUtenteField;
            }
            set
            {
                this.codiceUtenteField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string DescrizioneUtente
        {
            get
            {
                return this.descrizioneUtenteField;
            }
            set
            {
                this.descrizioneUtenteField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string CodiceRuolo
        {
            get
            {
                return this.codiceRuoloField;
            }
            set
            {
                this.codiceRuoloField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string DescrizioneRuolo
        {
            get
            {
                return this.descrizioneRuoloField;
            }
            set
            {
                this.descrizioneRuoloField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string UOAppartenenza
        {
            get
            {
                return this.uOAppartenenzaField;
            }
            set
            {
                this.uOAppartenenzaField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class ContestoArchivistico
    {
        private Classificazione[] classificazioneField;
        private Fascicolazione[] fascicolazioneField;
        private DocumentoCollegato[] documentoCollegatoField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Classificazione")]
        public Classificazione[] Classificazione
        {
            get
            {
                return this.classificazioneField;
            }
            set
            {
                this.classificazioneField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Fascicolazione")]
        public Fascicolazione[] Fascicolazione
        {
            get
            {
                return this.fascicolazioneField;
            }
            set
            {
                this.fascicolazioneField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("DocumentoCollegato")]
        public DocumentoCollegato[] DocumentoCollegato
        {
            get
            {
                return this.documentoCollegatoField;
            }
            set
            {
                this.documentoCollegatoField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class Classificazione
    {
        private string codiceClassificazioneField;
        private string titolarioDiRiferimentoField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string CodiceClassificazione
        {
            get
            {
                return this.codiceClassificazioneField;
            }
            set
            {
                this.codiceClassificazioneField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string TitolarioDiRiferimento
        {
            get
            {
                return this.titolarioDiRiferimentoField;
            }
            set
            {
                this.titolarioDiRiferimentoField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class Fascicolazione
    {

        private string codiceFascicoloField;
        private string descrizioneFascicoloField;
        private string titolarioDiRierimentoField;
        private string codiceSottofascicoloField;
        private string descrizioneSottofascicoloField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string CodiceFascicolo
        {
            get
            {
                return this.codiceFascicoloField;
            }
            set
            {
                this.codiceFascicoloField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string DescrizioneFascicolo
        {
            get
            {
                return this.descrizioneFascicoloField;
            }
            set
            {
                this.descrizioneFascicoloField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string TitolarioDiRierimento
        {
            get
            {
                return this.titolarioDiRierimentoField;
            }
            set
            {
                this.titolarioDiRierimentoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string CodiceSottofascicolo
        {
            get
            {
                return this.codiceSottofascicoloField;
            }
            set
            {
                this.codiceSottofascicoloField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string DescrizioneSottofascicolo
        {
            get
            {
                return this.descrizioneSottofascicoloField;
            }
            set
            {
                this.descrizioneSottofascicoloField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class DocumentoCollegato
    {

        private string iDdocumentoField;
        private string dataCreazioneField;
        private string oggettoField;
        private string segnaturaProtocolloField;
        private string numeroProtocolloField;
        private string dataProtocolloField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string IDdocumento
        {
            get
            {
                return this.iDdocumentoField;
            }
            set
            {
                this.iDdocumentoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string DataCreazione
        {
            get
            {
                return this.dataCreazioneField;
            }
            set
            {
                this.dataCreazioneField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Oggetto
        {
            get
            {
                return this.oggettoField;
            }
            set
            {
                this.oggettoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string SegnaturaProtocollo
        {
            get
            {
                return this.segnaturaProtocolloField;
            }
            set
            {
                this.segnaturaProtocolloField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string NumeroProtocollo
        {
            get
            {
                return this.numeroProtocolloField;
            }
            set
            {
                this.numeroProtocolloField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string DataProtocollo
        {
            get
            {
                return this.dataProtocolloField;
            }
            set
            {
                this.dataProtocolloField = value;
            }
        }
    }


    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(TypeName = "Allegati")]
    [System.Xml.Serialization.XmlRootAttribute("Allegati", Namespace = "", IsNullable = false)]
    public partial class Allegati1
    {

        private Allegato[] allegatoField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Allegato")]
        public Allegato[] Allegato
        {
            get
            {
                return this.allegatoField;
            }
            set
            {
                this.allegatoField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class Eventi
    {

        private object[] eventoField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Evento")]
        public object[] Evento
        {
            get
            {
                return this.eventoField;
            }
            set
            {
                this.eventoField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class Certifiato
    {
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class Firma
    {

        private string datiFirmaField;

        private string algoritmoFirmaField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string DatiFirma
        {
            get
            {
                return this.datiFirmaField;
            }
            set
            {
                this.datiFirmaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string AlgoritmoFirma
        {
            get
            {
                return this.algoritmoFirmaField;
            }
            set
            {
                this.algoritmoFirmaField = value;
            }
        }
    }

    public partial class Tipologia
    {

        private CampoTipologia[] campoTipologiaField;
        private string nomeTipologiaField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("CampoTipologia")]
        public CampoTipologia[] CampoTipologia
        {
            get
            {
                return this.campoTipologiaField;
            }
            set
            {
                this.campoTipologiaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string NomeTipologia
        {
            get
            {
                return this.nomeTipologiaField;
            }
            set
            {
                this.nomeTipologiaField = value;
            }
        }
    }

        public partial class CampoTipologia
    {

        private string nomeCampoField;
        private string valoreCampoField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string NomeCampo
        {
            get
            {
                return this.nomeCampoField;
            }
            set
            {
                this.nomeCampoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ValoreCampo
        {
            get
            {
                return this.valoreCampoField;
            }
            set
            {
                this.valoreCampoField = value;
            }
        }
    }
}