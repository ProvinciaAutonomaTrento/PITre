using System;
using System.Collections;
using System.Xml.Serialization;
using DocsPaVO.ProfilazioneDinamica;

namespace DocsPaVO.areaConservazione
{
    [Serializable()]
    public class ItemsConservazione
    {
        public string SystemID;
        public string ID_Conservazione;
        public string ID_Profile;
        public string ID_Project;
        public string TipoDoc;
        public string desc_oggetto;
        public string ID_Registro;
        public string Data_Ins;
        public string StatoConservazione;
        public string esitoLavorazione = string.Empty;
        //Il campo XML-Metadati non viene utilizzato perchè viene scritto direttamente
        //su fileSystem...
        public string SizeItem;
        public string CodFasc;
        public string DocNumber;
        //i campi che seguono non vengono sempre valorizzati!!!
        public string numProt_or_id = string.Empty;
        public string data_prot_or_create = string.Empty;
        public string numProt = string.Empty;
        //questo attributo viene valorizzato solo in fase di creazione della cartella
        public string pathCD = string.Empty;
        public string pathTimeStamp = string.Empty;
        public ArrayList path_allegati = new ArrayList();
        
        public string tipoFile;
        public string numAllegati;
        public string immagineAcquisita;
        //campi utilizzati solo per la creazione dei files HTML
        public string mittente;
        public string creatoreDocumento;
        public string tipo_oggetto;
        public string tipo_atto;
        public Templates template = null;
        public string descContatore = string.Empty;
        public string segnaturaContatore = string.Empty;
        public string dirittiDocumento = string.Empty;
        public string policyValida = string.Empty;

        public string Check_Dim = string.Empty;
        public string Check_Firma = string.Empty;
        public string Check_Marcatura = string.Empty;
        public string Check_Formato = string.Empty;
        public string Check_Mask_Policy = string.Empty;

        
        [XmlArray()]
        [XmlArrayItem(typeof(DocsPaVO.documento.TimestampDoc))]
        public System.Collections.ArrayList timestampDoc;
        
        /// <summary>
        /// Se true, indica che il formato file non è valido per la conservazione
        /// </summary>
        public bool invalidFileFormat = false;

        /// <summary>
        /// Indica l'esito della verifica firma
        /// </summary>
        public EsitoValidazioneFirmaEnum esitoValidazioneFirma = EsitoValidazioneFirmaEnum.NonVerificata;

        /// <summary>
        /// 
        /// </summary>
        public enum EsitoValidazioneFirmaEnum
        {
            NonVerificata,                      //0
            Valida,                             //1
            FirmaNonValida,                     //2
            MarcaNonValida,                     //3
            FormatoNonValido,                   //4
            FormatoNonValido_Valida,            //5
            FormatoNonValido_FirmaNonValida,    //6
            FormatoNonValido_MarcaNonValida,    //7
        }

        public ItemsConservazione Clone()
        {
            ItemsConservazione item = new ItemsConservazione();
            item.CodFasc = this.CodFasc;
            item.creatoreDocumento = this.creatoreDocumento;
            item.Data_Ins = this.Data_Ins;
            item.data_prot_or_create = this.data_prot_or_create;
            item.desc_oggetto = this.desc_oggetto;
            item.descContatore = this.descContatore;
            item.DocNumber = this.DocNumber;
            item.esitoLavorazione = this.esitoLavorazione;
            item.ID_Conservazione = this.ID_Conservazione;
            item.ID_Profile = this.ID_Profile;
            item.ID_Project = this.ID_Project;
            item.ID_Registro = this.ID_Registro;
            item.immagineAcquisita = this.immagineAcquisita;
            item.mittente = this.mittente;
            item.numAllegati = this.numAllegati;
            item.numProt = this.numProt;
            item.numProt_or_id = this.numProt_or_id;
            item.path_allegati = this.path_allegati;
            item.pathCD = this.pathCD;
            item.SizeItem = this.SizeItem;
            item.StatoConservazione = this.StatoConservazione;
            item.SystemID = this.SystemID;
            item.template = this.template;
            item.tipo_atto = this.tipo_atto;
            item.tipo_oggetto = this.tipo_oggetto;
            item.TipoDoc = this.TipoDoc;
            item.tipoFile = this.tipoFile;
            item.segnaturaContatore = this.segnaturaContatore;
            return item;
        }
    }


    /// <summary>
    /// Classe per la gestionecampo del mask di validazione
    /// Significato del mask di validazione della policy:
	/// MASK[0] = TipologiaDocumento;
    /// MASK[1] = StatoDocumento;
    /// MASK[2] = AooCreator;
    /// MASK[3] = Rf_Creator;
    /// MASK[4] = Uo_Creator;
    /// MASK[5] = Titoloario;
    /// MASK[6] = Classificazione;
    /// MASK[7] = DocArrivo;
    /// MASK[8] = DocPartenza;
    /// MASK[9] = DocInterno;
    /// MASK[10] = DocNP;
    /// MASK[11] = DocDigitale;
    /// MASK[12] = DocFirmato;
    /// MASK[13] = DocDataCreazione;
    /// MASK[14] = DocDataProtocollazione;
    /// MASK[15] = DocFormato; 
    /// 
    /// </summary>
    [Serializable()]
    public class ItemPolicyValidator
    {
        /// <summary>
        /// Stato elemento mask di validazione:
        /// X : Unsettings
        /// 0 : Valid
        /// 1 : NotValid
        /// </summary>
        public enum StatusPolicyValidator { Unsetting, Valid, NotValid }

        public StatusPolicyValidator TipologiaDocumento;
        public StatusPolicyValidator StatoDocumento;
        public StatusPolicyValidator AooCreator;
        public StatusPolicyValidator Rf_Creator;
        public StatusPolicyValidator Uo_Creator;
        public StatusPolicyValidator Titolario;
        public StatusPolicyValidator Classificazione;
        public StatusPolicyValidator DocArrivo;
        public StatusPolicyValidator DocPartenza;
        public StatusPolicyValidator DocInterno;
        public StatusPolicyValidator DocNP;
        public StatusPolicyValidator DocDigitale;
        public StatusPolicyValidator DocFirmato;
        public StatusPolicyValidator DocDataCreazione;
        public StatusPolicyValidator DocDataProtocollazione;
        public StatusPolicyValidator DocFormato;

        /// <summary>
        /// Decodifica la stringa del mask di validazione policy
        /// </summary>
        /// <param name="maskValidationPolicy"></param>
        /// <returns>oggetto filtri di stato validazione</returns>
        public static ItemPolicyValidator getItemPolicyValidator(string maskValidationPolicy)
        {
            try
            {
                if (string.IsNullOrEmpty(maskValidationPolicy)) return null;
                return new ItemPolicyValidator
                {
                    TipologiaDocumento = getStatusValidation(maskValidationPolicy, 0),
                    StatoDocumento = getStatusValidation(maskValidationPolicy, 1),
                    AooCreator = getStatusValidation(maskValidationPolicy, 2),
                    Rf_Creator = getStatusValidation(maskValidationPolicy, 3),
                    Uo_Creator = getStatusValidation(maskValidationPolicy, 4),
                    Titolario = getStatusValidation(maskValidationPolicy, 5),
                    Classificazione = getStatusValidation(maskValidationPolicy, 6),
                    DocArrivo = getStatusValidation(maskValidationPolicy, 7),
                    DocPartenza = getStatusValidation(maskValidationPolicy, 8),
                    DocInterno = getStatusValidation(maskValidationPolicy, 9),
                    DocNP = getStatusValidation(maskValidationPolicy, 10),
                    DocDigitale = getStatusValidation(maskValidationPolicy, 11),
                    DocFirmato = getStatusValidation(maskValidationPolicy, 12),
                    DocDataCreazione = getStatusValidation(maskValidationPolicy, 13),
                    DocDataProtocollazione = getStatusValidation(maskValidationPolicy, 14),
                    DocFormato = getStatusValidation(maskValidationPolicy, 15)
                };
            }
            catch { return null; }
        }

        /// <summary>
        /// Decodifica il filtro dalla stringa del mask di validazione  
        /// </summary>
        /// <param name="maskValidationPolicy">Stringa mask di validazione</param>
        /// <param name="maskPosition">Posizione del carattere rappresentante il filtro della policy</param>
        /// <returns></returns>
        private static StatusPolicyValidator getStatusValidation(string maskValidationPolicy, int maskPosition)
        {
           return decodeCharStatusValidation(maskValidationPolicy.Substring(maskPosition, 1));

        }

        /// <summary>
        /// Decodoifica il filtro nella stringa del mask di validazione  
        /// </summary>
        /// <param name="maskValidationPolicy">Stringa mask di validazione</param>
        /// <returns></returns>
        private static StatusPolicyValidator decodeCharStatusValidation(string maskCharValidationPolicy)
        {
            switch (maskCharValidationPolicy)
            {
                case "_": return StatusPolicyValidator.Unsetting;
                case "0": return StatusPolicyValidator.NotValid;
                case "1": return StatusPolicyValidator.Valid;
                default: return StatusPolicyValidator.Unsetting;
            }
        }

        /// <summary>
        /// Codifica il filtro nella stringa del mask di validazione  
        /// </summary>
        /// <param name="maskValidationPolicy">Stringa mask di validazione</param>
        /// <returns></returns>
        private static string encodeCharStatusValidation(StatusPolicyValidator statusValidation)
        {
            switch (statusValidation)
            {
                case StatusPolicyValidator.Unsetting: return "_";
                case StatusPolicyValidator.NotValid: return "0";
                case StatusPolicyValidator.Valid: return "1";
                default: return "_";
            }
        }

        /// <summary>
        /// Costruisce la mask string dall'oggetto policyValidator
        /// </summary>
        /// <param name="itemPolicyValidator">Filtri di una policy</param>
        /// <returns></returns>
        public static string getMaskItemPolicyValidator(ItemPolicyValidator itemPolicyValidator)
        { 
            string maskString = string.Empty;
            maskString += encodeCharStatusValidation(itemPolicyValidator.TipologiaDocumento);
            maskString += encodeCharStatusValidation(itemPolicyValidator.StatoDocumento);
            maskString += encodeCharStatusValidation(itemPolicyValidator.AooCreator);
            maskString += encodeCharStatusValidation(itemPolicyValidator.Rf_Creator);
            maskString += encodeCharStatusValidation(itemPolicyValidator.Uo_Creator);
            maskString += encodeCharStatusValidation(itemPolicyValidator.Titolario);
            maskString += encodeCharStatusValidation(itemPolicyValidator.Classificazione);
            maskString += encodeCharStatusValidation(itemPolicyValidator.DocArrivo);
            maskString += encodeCharStatusValidation(itemPolicyValidator.DocPartenza);
            maskString += encodeCharStatusValidation(itemPolicyValidator.DocInterno);
            maskString += encodeCharStatusValidation(itemPolicyValidator.DocNP);
            maskString += encodeCharStatusValidation(itemPolicyValidator.DocDigitale);
            maskString += encodeCharStatusValidation(itemPolicyValidator.DocFirmato);
            maskString += encodeCharStatusValidation(itemPolicyValidator.DocDataCreazione);
            maskString += encodeCharStatusValidation(itemPolicyValidator.DocDataProtocollazione);
            maskString += encodeCharStatusValidation(itemPolicyValidator.DocFormato);
            return maskString;
        }

    }
}
