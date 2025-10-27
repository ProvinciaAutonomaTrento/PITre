// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.ProfilazioneDinamica;
using DocsPaVO.ProfilazioneDinamicaLite;
using DocsPaVO.utente;
using MediatR;
using Org.BouncyCastle.Crypto;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.AddressbookGetCorrispondenteBySystemId;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.GetCorrispondenteByCodRubricaRubricaComune;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.rubricaGetElementiRubrica;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.CreateDocumentAndAddInProject;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.VerificaValiditaFirma;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Xml;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetTemplateFromPisVisibility
{
    public class GetTemplateFromPisVisibilityCommandHandler : IRequestHandler<GetTemplateFromPisVisibilityCommand, GetTemplateFromPisVisibilityCommandResponse>
    {
        public GetTemplateFromPisVisibilityCommandHandler(
            IMediator mediator, 
            IPi3DbContext pi3DbContext,
            ILogger<GetTemplateFromPisVisibilityCommandHandler> logger)
        {
            this._logger = logger;
            this._mediator = mediator;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<GetTemplateFromPisVisibilityCommandResponse> Handle(GetTemplateFromPisVisibilityCommand request, CancellationToken cancellationToken)
        {
            DocsPaVO.ProfilazioneDinamica.Templates output = new DocsPaVO.ProfilazioneDinamica.Templates();
            try
            {

                if (string.IsNullOrEmpty(request.template.PATH_XSD_ASSOCIATO) || request.editDocument)
                {
                    if (request.templatePis == null || request.template == null)
                    {
                        output = null;
                    }
                    else
                    {
                        if (request.template != null)
                        {
                            output.DESCRIZIONE = request.template.DESCRIZIONE;
                            output.SYSTEM_ID = request.template.SYSTEM_ID;
                        }
                        else
                        {
                            output.DESCRIZIONE = request.templatePis.Name;
                            output.SYSTEM_ID = Int32.Parse(request.templatePis.Id);
                        }
                        DocsPaVO.ProfilazioneDinamica.OggettoCustom[] oggettiCustom = (DocsPaVO.ProfilazioneDinamica.OggettoCustom[])
                                                                request.template.ELENCO_OGGETTI;
                        DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli[] dirittiOggetti = null;

                        switch (request.DoP)
                        {
                            case "D":
                                dirittiOggetti = DBUtils.GetDirittiCampiTipologiaDoc(request.idRuolo, request.template.SYSTEM_ID.ToString(), this._pi3DbContext).ToArray();
                                break;
                            case "P":
                                dirittiOggetti = DBUtils.GetDirittiCampiTipologiaFasc(request.idRuolo, request.template.SYSTEM_ID.ToString(), this._pi3DbContext).ToArray();
                                break;
                        }
                        foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom in oggettiCustom)
                        {
                            Field campo = null;
                            if (!DBUtils.ApplicationIgnoresRights(request.codeApplication, this._pi3DbContext) && !(oggettoCustom.TIPO.DESCRIZIONE_TIPO).ToUpper().Equals("SEPARATORE"))
                            {
                                if (dirittiOggetti.FirstOrDefault(e => e.ID_OGGETTO_CUSTOM.ToUpperInvariant() == oggettoCustom.SYSTEM_ID.ToString().ToUpperInvariant()) != null && dirittiOggetti.FirstOrDefault(e => e.ID_OGGETTO_CUSTOM.ToUpperInvariant() == oggettoCustom.SYSTEM_ID.ToString().ToUpperInvariant()).INS_MOD_OGG_CUSTOM != "0")
                                    campo = request.templatePis.Fields.FirstOrDefault(e => e.Name.ToUpperInvariant() == oggettoCustom.DESCRIZIONE.ToUpperInvariant());
                                else
                                {
                                    if (request.templatePis.Fields != null && request.templatePis.Fields.FirstOrDefault(e => e.Name.ToUpperInvariant() == oggettoCustom.DESCRIZIONE.ToUpperInvariant()) != null && !string.IsNullOrEmpty(request.templatePis.Fields.FirstOrDefault(e => e.Name.ToUpperInvariant() == oggettoCustom.DESCRIZIONE.ToUpperInvariant()).Value))
                                        throw new RestException("TEMPLATE_FIELD_NOT_ROLE_EDITABLE");
                                }
                            }
                            else
                            {
                                campo = request.templatePis.Fields.FirstOrDefault(e => e.Name.ToUpperInvariant() == oggettoCustom.DESCRIZIONE.ToUpperInvariant());

                            }

                            if (campo != null && string.IsNullOrEmpty(campo.Value) ||
                                ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("CasellaDiSelezione") &&
                                (campo.MultipleChoice == null || campo.MultipleChoice.Length == 0)))
                            {

                                if (campo.Required && !request.search)
                                {
                                    throw new RestException("FIELD_REQUIRED");
                                }

                                if (((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("Contatore") || (oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("ContatoreSottocontatore")))
                                {
                                    oggettoCustom.CONTATORE_DA_FAR_SCATTARE = campo.CounterToTrigger;
                                }
                            }
                            else
                            {
                                if (campo != null)
                                {
                                    oggettoCustom.VALORE_DATABASE = campo.Value;
                                }

                                switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
                                {
                                    case "Contatore":
                                    case "ContatoreSottocontatore":
                                        if (campo != null)
                                        {
                                            if (oggettoCustom.TIPO_CONTATORE.Equals("A") || oggettoCustom.TIPO_CONTATORE.Equals("R"))
                                            {
                                                if (campo != null && !string.IsNullOrEmpty(campo.CodeRegisterOrRF))
                                                {
                                                    DocsPaVO.utente.Registro reg = DBUtils.getRegistroByCodAOO(campo.CodeRegisterOrRF, request.infoUtente.idAmministrazione, this._pi3DbContext);

                                                    if (reg != null)
                                                    {
                                                        oggettoCustom.ID_AOO_RF = reg.systemId;
                                                    }
                                                    else
                                                    {
                                                        throw new RestException("REGISTER_NOT_FOUND");
                                                    }
                                                }
                                                else
                                                {
                                                    if (!request.search)
                                                        throw new RestException("REQUIRED_ID_REGISTER");
                                                }

                                            }
                                            oggettoCustom.CONTATORE_DA_FAR_SCATTARE = true;
                                        }
                                        else
                                        {
                                            oggettoCustom.CONTATORE_DA_FAR_SCATTARE = false;
                                        }
                                        break;
                                    case "CasellaDiSelezione":
                                        for (int i = 0; i < oggettoCustom.ELENCO_VALORI.Count(); i++)
                                        {
                                            if (campo != null)
                                            {
                                                foreach (string word in campo.MultipleChoice)
                                                {
                                                    if (((DocsPaVO.ProfilazioneDinamica.ValoreOggetto)oggettoCustom.ELENCO_VALORI[i]).VALORE.Equals(word))
                                                    {
                                                        oggettoCustom.VALORI_SELEZIONATI[i] = ((DocsPaVO.ProfilazioneDinamica.ValoreOggetto)oggettoCustom.ELENCO_VALORI[i]).VALORE;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                }
                            }
                        }
                        output = request.template;

                    }
                }
                else
                {
                    if (request.template.PATH_XSD_ASSOCIATO.ToUpper() == "ASSOC_NO_VALIDAZIONE")
                    {
                        if (request.docPrincipale == null)
                        {
                            throw new RestException("ELAB_XML_FILE_NOT_FOUND");
                        }
                        else if (!request.docPrincipale.Name.ToUpper().EndsWith("XML") && !request.docPrincipale.Name.ToUpper().EndsWith("P7M"))
                        {
                            throw new RestException("ELAB_XML_FILE_NOT_VALID");
                        }
                        string stringaXml = string.Empty;
                        if (request.docPrincipale.Name.ToUpper().EndsWith("P7M"))
                        {
                            try
                            {
                                var verifyRequest = new VerificaValiditaFirmaCommand()
                                {
                                    FileDoc = new()
                                    {
                                        content = request.docPrincipale.Content,
                                        fullName = request.docPrincipale.Name,
                                    },
                                    DataDiVerifica = null,
                                };

                                var verResp = await this._mediator.Send(verifyRequest);
                                if (verResp == null || !verResp.Output)
                                {
                                    throw new Exception(Resources.ControlloFirmaExc);
                                }
                            }
                            catch (Exception ex)
                            {
                                this._logger.LogError(exception: ex, message: ex.Message);
                            }

                            if (!request.docPrincipale.Name.ToUpper().EndsWith("XML"))
                            {
                                throw new RestException("ELAB_XML_FILE_NOT_VALID");
                            }
                            else
                            {
                                stringaXml = Encoding.UTF8.GetString(request.docPrincipale.Content);
                            }

                        }

                        if (string.IsNullOrEmpty(stringaXml))
                            stringaXml = Encoding.UTF8.GetString(request.docPrincipale.Content);
                        stringaXml = RemoveUtf8ByteOrderMark(stringaXml);
                        output = await this.AssociazioneXMLperTemplate(request.template, request.templatePis, stringaXml, request.DoP, request.codeApplication, request.idRuolo, request.search, request.infoUtente, request.codeRegister, request.codeRF, request.idSdi);
                    }
                    else
                    {
                        if (request.docPrincipale == null)
                        {
                            throw new RestException("ELAB_XML_FILE_NOT_FOUND");
                        }
                        else if (!request.docPrincipale.Name.ToUpper().EndsWith("XML"))
                        {
                            throw new RestException("ELAB_XML_FILE_NOT_VALID");
                        }
                        string stringaXml = Encoding.UTF8.GetString(request.docPrincipale.Content);
                        stringaXml = RemoveUtf8ByteOrderMark(stringaXml);
                        bool validato = ValidateXmlWithXsd(stringaXml, request.template.PATH_XSD_ASSOCIATO);
                        output = await AssociazioneXMLperTemplate(request.template, request.templatePis, stringaXml, request.DoP, request.codeApplication, request.idRuolo, request.search, request.infoUtente, request.codeRegister, request.codeRF, request.idSdi);

                    }
                }
            }
            catch(Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }

            return new(output);
        }


        #region Private Members

        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _pi3DbContext;
        protected readonly ILogger<GetTemplateFromPisVisibilityCommandHandler> _logger;

        private bool ValidateXmlWithXsd(string stringaXml, string pathXsd)
        {
            bool retval = true;
            System.Xml.Schema.XmlSchemaSet schemas = new System.Xml.Schema.XmlSchemaSet();
            string[] coppie = pathXsd.Split('>');
            for (int i = 0; i < coppie.Length; i++)
            {
                string[] coppiaXsdTrgtNS = coppie[i].Split('<');
                if (coppiaXsdTrgtNS.Length > 0)
                {
                    schemas.Add(coppiaXsdTrgtNS[1], coppiaXsdTrgtNS[0]);
                }
            }

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(stringaXml);
            xmlDoc.Schemas.Add(schemas);
            try
            {
                xmlDoc.Validate(null);
            }
            catch (Exception ex)
            {
                throw new RestException("ELAB_XML_XSD_VALIDATION_ERROR");
            }
            return retval;
        }
        private string RemoveUtf8ByteOrderMark(string xml)
        {
            return xml.Trim();
        }
        private async Task<DocsPaVO.ProfilazioneDinamica.Templates> AssociazioneXMLperTemplate(DocsPaVO.ProfilazioneDinamica.Templates template, Template templatePis, 
            string stringaXml, string DoP, string codeApplication, string idRuolo, bool search,
            DocsPaVO.utente.InfoUtente infoUtente, string codeRegister, string codeRF, string idSdI)
        {
            XmlDocument xmlDoc = new XmlDocument();
            if (stringaXml.Contains("xml version=\"1.1\""))
            {
                stringaXml = stringaXml.Replace("xml version=\"1.1\"", "xml version=\"1.0\"");
            }
            try
            {
                xmlDoc.LoadXml(stringaXml);
            }
            catch (Exception exXml)
            {
                try
                {
                    string byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
                    if (stringaXml.StartsWith(byteOrderMarkUtf8))
                    {
                        stringaXml = stringaXml.Remove(0, byteOrderMarkUtf8.Length);
                    }
                    xmlDoc.LoadXml(stringaXml);
                }
                catch (Exception bomUTF8)
                {
                    throw new Exception(Resources.ElaborazioneXmlExc);
                }

            }
            DocsPaVO.ProfilazioneDinamica.OggettoCustom[] oggettiCustom = (DocsPaVO.ProfilazioneDinamica.OggettoCustom[])
                                                   template.ELENCO_OGGETTI;
            DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli[] dirittiOggetti = null;

            string notePerElaborazioneXML = "";
            if (DoP == "D")
                dirittiOggetti = DBUtils.GetDirittiCampiTipologiaDoc(idRuolo, template.SYSTEM_ID.ToString(),this._pi3DbContext).ToArray();
            else if (DoP == "P")
                dirittiOggetti = DBUtils.GetDirittiCampiTipologiaFasc(idRuolo, template.SYSTEM_ID.ToString(), this._pi3DbContext).ToArray();

            foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom in oggettiCustom)
            {
                bool campoDaIns = false;
                if (!DBUtils.ApplicationIgnoresRights(codeApplication,this._pi3DbContext) && !(oggettoCustom.TIPO.DESCRIZIONE_TIPO).ToUpper().Equals("SEPARATORE"))
                {
                    if (dirittiOggetti.FirstOrDefault(e => e.ID_OGGETTO_CUSTOM.ToUpperInvariant() == oggettoCustom.SYSTEM_ID.ToString().ToUpperInvariant()).INS_MOD_OGG_CUSTOM != "0")
                        campoDaIns = true;
                    else
                    {
                        if (templatePis.Fields != null && 
                            templatePis.Fields.FirstOrDefault(e => e.Name.ToUpperInvariant() == oggettoCustom.DESCRIZIONE.ToUpperInvariant()) != null
                            && !string.IsNullOrEmpty(templatePis.Fields.FirstOrDefault(e => e.Name.ToUpperInvariant() == oggettoCustom.DESCRIZIONE.ToUpperInvariant()).Value))
                            throw new RestException("TEMPLATE_FIELD_NOT_ROLE_EDITABLE");
                    }
                }
                else
                {
                    campoDaIns = true;
                }

                switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
                {
                    case "Contatore":
                    case "ContatoreSottocontatore":
                        #region oggetti custom contatore - sottocont.
                        if (campoDaIns)
                        {
                            if (oggettoCustom.TIPO_CONTATORE.Equals("A"))
                            {
                                if (!string.IsNullOrEmpty(codeRegister))
                                {
                                    DocsPaVO.utente.Registro reg = DBUtils.getRegistroByCodAOO(codeRegister,infoUtente.idAmministrazione,this._pi3DbContext);

                                    if (reg != null)
                                    {
                                        oggettoCustom.ID_AOO_RF = reg.systemId;
                                        oggettoCustom.CONTATORE_DA_FAR_SCATTARE = true;
                                    }
                                    else
                                    {
                                        //Registro|RF mancante
                                        throw new RestException("REGISTER_NOT_FOUND");
                                    }
                                }
                                else
                                {
                                    throw new RestException("REQUIRED_CODEREGISTER");
                                }
                            }
                            else if (oggettoCustom.TIPO_CONTATORE.Equals("R"))
                            {
                                if (!string.IsNullOrEmpty(codeRF))
                                {
                                    DocsPaVO.utente.Registro reg = DBUtils.getRegistroByCodAOO(codeRF, infoUtente.idAmministrazione, this._pi3DbContext);

                                    if (reg != null)
                                    {
                                        oggettoCustom.ID_AOO_RF = reg.systemId;
                                        oggettoCustom.CONTATORE_DA_FAR_SCATTARE = true;
                                    }
                                    else
                                    {
                                        //Registro|RF mancante
                                        throw new RestException("REGISTER_NOT_FOUND");
                                    }
                                }
                                else
                                {
                                    throw new RestException("REQUIRED_CODERF");
                                }
                            }
                            else
                                oggettoCustom.CONTATORE_DA_FAR_SCATTARE = true;
                        }
                        else
                        {
                            oggettoCustom.CONTATORE_DA_FAR_SCATTARE = false;
                        }
                        #endregion
                        break;
                }

                if (campoDaIns)
                {
                    if (string.IsNullOrEmpty(oggettoCustom.CAMPO_XML_ASSOC))
                    {
                        if (oggettoCustom.CAMPO_OBBLIGATORIO == "SI" && !search)
                        {
                            throw new Exception("Elaborazione XML: manca il mapping per il campo obbligatorio " + oggettoCustom.DESCRIZIONE);
                        }
                        else
                        {
                            oggettoCustom.VALORE_DATABASE = string.Empty;
                        }
                    }
                    else
                    {
                        try
                        {
                            bool associaSecondo = false;
                            string[] mappings = oggettoCustom.CAMPO_XML_ASSOC.Split('<');
                            string[] mappingXml = mappings[0].Split('>');
                            string mappingElemento = String.Format("//*[local-name()='{0}']", mappingXml[0]);
                            for (int i = 1; i < mappingXml.Length; i++)
                            {
                                mappingElemento += String.Format("/*[local-name()='{0}']", mappingXml[i]);
                            }
                            string valore = "";
                            string tipoLiquidazione = "", tipoLiq2 = "", mapTipoLIQ = "//*[name()='ns0:Liquidazione']/*[name()='ns0:Testata']/*[name()='ns0:Int_Tipo']";
                            try
                            {
                                XmlNode node = xmlDoc.DocumentElement.SelectSingleNode(mappingElemento);
                                valore = node.InnerXml; // valore dell'xml estratto
                            }
                            catch (Exception nodo)
                            {
                                if (mappings.Length > 1 && !string.IsNullOrEmpty(mappings[1]))
                                    associaSecondo = true;
                                else if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("FATT_EL_FORNITORE_1"))
                                {
                                    valore = "";
                                }
                                else if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("FATT_EL_CLIENTE_1"))
                                {
                                    valore = "";
                                }
                                else if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("FATT_EL_PIVA_CLIENTE_1"))
                                {
                                    valore = "999";
                                }
                                else if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("FATT_EL_NOTE_VER_FIRMA_1"))
                                {
                                    valore = "Verifica firma digitale effettuata da SDI, secondo le Specifiche tecniche operative delle Regole tecniche di cui all’allegato B del D.M. n. 55 del 3 aprile 2013 e ss.mm.ii.";
                                }
                                else if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("FATT_EL_IDSDI"))
                                {
                                    valore = idSdI;
                                }
                                else if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("FATT_EL_VERSIONE_1"))
                                {
                                    XmlElement root = xmlDoc.DocumentElement;
                                    valore = root.Attributes["versione"].Value;
                                }
                                else if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("DEF-VALUE_"))
                                {
                                    valore = oggettoCustom.OPZIONI_XML_ASSOC.Replace("DEF-VALUE_", "");
                                }
                                else if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("LIQ_ESTENSORE_1"))
                                {
                                    valore = "";

                                    string mappCampoLiq = "//*[local-name()='Flusso_Documenti']/*[local-name()='Documenti']/*[local-name()='{0}']";
                                    if (xmlDoc.DocumentElement.SelectSingleNode(string.Format(mappCampoLiq, "Liquidazione")) != null)
                                    {
                                        tipoLiq2 = "LIQUIDAZIONE";
                                    }
                                    else if (xmlDoc.DocumentElement.SelectSingleNode(string.Format(mappCampoLiq, "Storno")) != null)
                                    {
                                        tipoLiq2 = "STORNO";
                                    }
                                    else if (xmlDoc.DocumentElement.SelectSingleNode(string.Format(mappCampoLiq, "Integrazione")) != null)
                                    {
                                        tipoLiq2 = "INTEGRAZIONE";
                                    }
                                    else if (xmlDoc.DocumentElement.SelectSingleNode(string.Format(mappCampoLiq, "Richiesta_Storno")) != null)
                                    {
                                        tipoLiq2 = "RICHIESTA STORNO";
                                    }
                                    else if (xmlDoc.DocumentElement.SelectSingleNode(string.Format(mappCampoLiq, "NotaRettifica")) != null)
                                    {
                                        tipoLiq2 = "NOTA RETTIFICA";
                                    }
                                    else if (xmlDoc.DocumentElement.SelectSingleNode(string.Format(mappCampoLiq, "StornoNotaRettifica")) != null)
                                    {
                                        tipoLiq2 = "STORNO NOTA RETTIFICA";
                                    }

                                    //XmlNode nodeTipoLIQ = xmlDoc.DocumentElement.SelectSingleNode(mappCampoLiq);
                                    //tipoLiquidazione = nodeTipoLIQ.InnerXml; 
                                }
                                else if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("LIQ_FUNZIONARIO_1"))
                                {
                                    valore = "";
                                    string mappCampoLiq = "//*[local-name()='Flusso_Documenti']/*[local-name()='Documenti']/*[local-name()='{0}']";
                                    if (xmlDoc.DocumentElement.SelectSingleNode(string.Format(mappCampoLiq, "Liquidazione")) != null)
                                    {
                                        tipoLiq2 = "LIQUIDAZIONE";
                                    }
                                    else if (xmlDoc.DocumentElement.SelectSingleNode(string.Format(mappCampoLiq, "Storno")) != null)
                                    {
                                        tipoLiq2 = "STORNO";
                                    }
                                    else if (xmlDoc.DocumentElement.SelectSingleNode(string.Format(mappCampoLiq, "Integrazione")) != null)
                                    {
                                        tipoLiq2 = "INTEGRAZIONE";
                                    }
                                    else if (xmlDoc.DocumentElement.SelectSingleNode(string.Format(mappCampoLiq, "Richiesta_Storno")) != null)
                                    {
                                        tipoLiq2 = "RICHIESTA STORNO";
                                    }
                                    else if (xmlDoc.DocumentElement.SelectSingleNode(string.Format(mappCampoLiq, "NotaRettifica")) != null)
                                    {
                                        tipoLiq2 = "NOTA RETTIFICA";
                                    }
                                    else if (xmlDoc.DocumentElement.SelectSingleNode(string.Format(mappCampoLiq, "StornoNotaRettifica")) != null)
                                    {
                                        tipoLiq2 = "STORNO NOTA RETTIFICA";
                                    }

                                }
                                else
                                {
                                    notePerElaborazioneXML += "Nodo XML mancante per il campo " + oggettoCustom.DESCRIZIONE + ". ";
                                    if (oggettoCustom.CAMPO_OBBLIGATORIO == "SI")
                                    {
                                        throw new Exception("Elaborazione XML: Nodo XML mancante per il campo " + oggettoCustom.DESCRIZIONE);
                                    }
                                }
                            }

                            if (!string.IsNullOrEmpty(valore) && !string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("FATT_EL_CODFISC_1"))
                            {
                                if (!Char.IsLetter(valore[0]) || !Char.IsLetter(valore[1]) || !Char.IsLetter(valore[2]))
                                    valore = "";
                            }

                            if (!string.IsNullOrEmpty(valore) && !string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("DIVIDI100"))
                            {
                                Int64 cifra = 0;
                                double cifradivisa = 0;
                                if (Int64.TryParse(valore, out cifra))
                                {
                                    cifradivisa = ((double)cifra / 100);
                                    valore = (cifradivisa).ToString("N", System.Globalization.CultureInfo.GetCultureInfo("it-IT"));
                                }

                            }
                            if (!string.IsNullOrEmpty(valore) && !string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("PARSEINT"))
                            {
                                Int64 cifra = 0;
                                if (Int64.TryParse(valore, out cifra))
                                {
                                    valore = cifra.ToString();
                                }
                            }
                            if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("LIQ_NUMLIQCOLL"))
                            {
                                if (tipoLiq2 == "NOTA RETTIFICA")
                                {
                                    string mappCampoLiqColl = "//*[local-name()='Documenti_Collegati']/*[local-name()='Elenco_Documenti']/*[local-name()='Estremi_Doc']/*[local-name()='Num_Doc']";
                                    try
                                    {
                                        string valueX = xmlDoc.DocumentElement.SelectSingleNode(mappCampoLiqColl).InnerXml;
                                        Int64 cifra = 0;
                                        if (Int64.TryParse(valueX, out cifra))
                                        {
                                            valore = cifra.ToString();
                                        }
                                    }
                                    catch (Exception clcEx) { }

                                }
                                else
                                {
                                    Int64 cifra = 0;
                                    if (Int64.TryParse(valore, out cifra))
                                    {
                                        valore = cifra.ToString();
                                    }
                                }
                            }
                            if (associaSecondo)
                            {
                                int associaSecondoI = 1;
                                while (string.IsNullOrEmpty(valore) && mappings.Length > associaSecondoI && !string.IsNullOrEmpty(mappings[associaSecondoI]))
                                {
                                    mappingXml = mappings[associaSecondoI].Split('>');
                                    mappingElemento = String.Format("//*[local-name()='{0}']", mappingXml[0]);
                                    for (int i = 1; i < mappingXml.Length; i++)
                                    {
                                        mappingElemento += String.Format("/*[local-name()='{0}']", mappingXml[i]);
                                    }
                                    valore = "";
                                    try
                                    {
                                        XmlNode node = xmlDoc.DocumentElement.SelectSingleNode(mappingElemento);
                                        valore = node.InnerXml; // valore dell'xml estratto
                                    }
                                    catch (Exception nodo)
                                    {

                                    }
                                    associaSecondoI++;
                                }
                                if (string.IsNullOrEmpty(valore))
                                {
                                    notePerElaborazioneXML += "Nodo XML mancante per il campo " + oggettoCustom.DESCRIZIONE + ". ";
                                    if (oggettoCustom.CAMPO_OBBLIGATORIO == "SI")
                                    {
                                        throw new Exception("Elaborazione XML: Nodo XML mancante per il campo " + oggettoCustom.DESCRIZIONE);
                                    }

                                }
                            }

                            if (valore.Contains("<![CDATA["))
                            {
                                valore = valore.Replace("<![CDATA[", "");
                                valore = valore.Replace("]]>", "");
                            }

                            oggettoCustom.VALORE_DATABASE = valore;

                            if (oggettoCustom.TIPO.DESCRIZIONE_TIPO == "CampoDiTesto")
                            {
                                if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("MULTINODE"))
                                {
                                    string separatore = oggettoCustom.OPZIONI_XML_ASSOC.Split('>')[1];

                                    XmlNodeList multinode = xmlDoc.SelectNodes(mappingElemento);
                                    if (multinode.Count > 1)
                                    {
                                        valore = "";
                                        foreach (XmlNode nodoX in multinode)
                                        {
                                            if (!valore.Contains(nodoX.InnerXml))
                                            {
                                                valore += nodoX.InnerXml + separatore;
                                            }
                                        }
                                    }
                                    if (valore.Length > 180) valore = valore.Substring(0, 180);

                                }
                                #region Fornitore fattura elettronica
                                if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("FATT_EL_FORNITORE_1"))
                                {
                                    if (string.IsNullOrEmpty(valore))
                                    {
                                        string mappingNome = "CedentePrestatore>DatiAnagrafici>Anagrafica>Nome";
                                        mappingXml = mappingNome.Split('>');
                                        mappingElemento = String.Format("//*[local-name()='{0}']", mappingXml[0]);
                                        for (int i = 1; i < mappingXml.Length; i++)
                                        {
                                            mappingElemento += String.Format("/*[local-name()='{0}']", mappingXml[i]);
                                        }
                                        valore = "";
                                        try
                                        {
                                            XmlNode node = xmlDoc.DocumentElement.SelectSingleNode(mappingElemento);
                                            valore = node.InnerXml; 
                                        }
                                        catch (Exception nodo)
                                        {
                                            notePerElaborazioneXML += "Nodo XML mancante per il campo " + oggettoCustom.DESCRIZIONE + ". ";
                                            if (oggettoCustom.CAMPO_OBBLIGATORIO == "SI")
                                            {
                                                throw new Exception("Elaborazione XML: Nodo XML mancante per il campo " + oggettoCustom.DESCRIZIONE);
                                            }
                                        }

                                        string mappingCognome = "CedentePrestatore>DatiAnagrafici>Anagrafica>Cognome";
                                        mappingXml = mappingCognome.Split('>');
                                        mappingElemento = String.Format("//*[local-name()='{0}']", mappingXml[0]);
                                        for (int i = 1; i < mappingXml.Length; i++)
                                        {
                                            mappingElemento += String.Format("/*[local-name()='{0}']", mappingXml[i]);
                                        }
                                        try
                                        {
                                            XmlNode node = xmlDoc.DocumentElement.SelectSingleNode(mappingElemento);
                                            valore += (" " + node.InnerXml); 
                                        }
                                        catch (Exception nodo)
                                        {
                                            notePerElaborazioneXML += "Nodo XML mancante per il campo " + oggettoCustom.DESCRIZIONE + ". ";
                                            if (oggettoCustom.CAMPO_OBBLIGATORIO == "SI")
                                            {
                                                throw new Exception("Elaborazione XML: Nodo XML mancante per il campo " + oggettoCustom.DESCRIZIONE);
                                            }
                                        }
                                    }
                                }
                                #endregion
                                #region Cliente Fattura elettronica attiva
                                if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("FATT_EL_CLIENTE_1"))
                                {
                                    if (string.IsNullOrEmpty(valore))
                                    {
                                        string mappingNome = "CessionarioCommittente>DatiAnagrafici>Anagrafica>Nome";
                                        mappingXml = mappingNome.Split('>');
                                        mappingElemento = String.Format("//*[local-name()='{0}']", mappingXml[0]);
                                        for (int i = 1; i < mappingXml.Length; i++)
                                        {
                                            mappingElemento += String.Format("/*[local-name()='{0}']", mappingXml[i]);
                                        }
                                        valore = "";
                                        try
                                        {
                                            XmlNode node = xmlDoc.DocumentElement.SelectSingleNode(mappingElemento);
                                            valore = node.InnerXml; 
                                        }
                                        catch (Exception nodo)
                                        {
                                            notePerElaborazioneXML += "Nodo XML mancante per il campo " + oggettoCustom.DESCRIZIONE + ". ";
                                            if (oggettoCustom.CAMPO_OBBLIGATORIO == "SI")
                                            {
                                                throw new Exception("Elaborazione XML: Nodo XML mancante per il campo " + oggettoCustom.DESCRIZIONE);
                                            }
                                        }

                                        string mappingCognome = "CessionarioCommittente>DatiAnagrafici>Anagrafica>Cognome";
                                        mappingXml = mappingCognome.Split('>');
                                        mappingElemento = String.Format("//*[local-name()='{0}']", mappingXml[0]);
                                        for (int i = 1; i < mappingXml.Length; i++)
                                        {
                                            mappingElemento += String.Format("/*[local-name()='{0}']", mappingXml[i]);
                                        }
                                        try
                                        {
                                            XmlNode node = xmlDoc.DocumentElement.SelectSingleNode(mappingElemento);
                                            valore += (" " + node.InnerXml); 
                                        }
                                        catch (Exception nodo)
                                        {
                                            notePerElaborazioneXML += "Nodo XML mancante per il campo " + oggettoCustom.DESCRIZIONE + ". ";
                                            if (oggettoCustom.CAMPO_OBBLIGATORIO == "SI")
                                            {
                                                throw new Exception("Elaborazione XML: Nodo XML mancante per il campo " + oggettoCustom.DESCRIZIONE);
                                            }
                                        }
                                    }
                                }
                                #endregion
                                #region Partita IVA CessionarioCommittente
                                if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("FATT_EL_PIVA_CLIENTE_1"))
                                {
                                    // CessionarioCommittente>DatiAnagrafici>CodiceFiscale
                                    if (string.IsNullOrEmpty(valore) || valore == "999")
                                    {
                                        string mappingNome = "CessionarioCommittente>DatiAnagrafici>CodiceFiscale";
                                        mappingXml = mappingNome.Split('>');
                                        mappingElemento = String.Format("//*[local-name()='{0}']", mappingXml[0]);
                                        for (int i = 1; i < mappingXml.Length; i++)
                                        {
                                            mappingElemento += String.Format("/*[local-name()='{0}']", mappingXml[i]);
                                        }
                                        try
                                        {
                                            XmlNode node = xmlDoc.DocumentElement.SelectSingleNode(mappingElemento);
                                            valore = node.InnerXml; // valore dell'xml estratto
                                        }
                                        catch (Exception nodo)
                                        {
                                            notePerElaborazioneXML += "Nodo XML mancante per il campo " + oggettoCustom.DESCRIZIONE + ". ";

                                        }
                                    }
                                }
                                #endregion
                                #region Liquidazione Estensore
                                if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("LIQ_ESTENSORE_1"))
                                {
                                    string mappLIQEst = "";
                                    if (tipoLiq2 == "LIQUIDAZIONE" || tipoLiq2 == "NOTA RETTIFICA")
                                    {
                                        mappLIQEst = "//*[local-name()='Dati_DocLIQ']/*[local-name()='Estensore']";
                                    }
                                    else if (tipoLiq2 == "RICHIESTA STORNO")
                                    {
                                        mappLIQEst = "//*[local-name()='Dati_Doc_Motivo']/*[local-name()='Estensore']";
                                    }
                                    else
                                    {
                                        mappLIQEst = "//*[local-name()='Dati_Doc']/*[local-name()='Estensore']";
                                    }

                                    try
                                    {
                                        XmlNode node = xmlDoc.DocumentElement.SelectSingleNode(mappLIQEst);
                                        valore = node.InnerXml; 
                                    }
                                    catch (Exception nodo)
                                    { }
                                }
                                #endregion
                                #region Liquidazione Funzionario
                                if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("LIQ_FUNZIONARIO_1"))
                                {
                                    string mappLIQFun = "";
                                    if (tipoLiq2 == "LIQUIDAZIONE" || tipoLiq2 == "NOTA RETTIFICA")
                                    {
                                        mappLIQFun = "//*[local-name()='Dati_DocLIQ']/*[local-name()='Funzionario']";
                                    }
                                    else if (tipoLiq2 == "RICHIESTA STORNO")
                                    {
                                        mappLIQFun = "//*[local-name()='Dati_Doc_Motivo']/*[local-name()='Funzionario']";
                                    }
                                    else
                                    {
                                        mappLIQFun = "//*[local-name()='Dati_Doc']/*[local-name()='Funzionario']";
                                    }

                                    try
                                    {
                                        XmlNode node = xmlDoc.DocumentElement.SelectSingleNode(mappLIQFun);
                                        valore = node.InnerXml; 
                                    }
                                    catch (Exception nodo)
                                    { }
                                }
                                #endregion
                                oggettoCustom.VALORE_DATABASE = valore;
                            }
                            if (!string.IsNullOrEmpty(valore)) 
                            {
                                if (oggettoCustom.TIPO.DESCRIZIONE_TIPO == "MenuATendina" || oggettoCustom.TIPO.DESCRIZIONE_TIPO == "SelezioneEsclusiva")
                                {
                                    if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.Contains('>') && oggettoCustom.OPZIONI_XML_ASSOC.Contains('<'))
                                    {
                                        string[] valoriAssociati1 = oggettoCustom.OPZIONI_XML_ASSOC.Split('>');
                                        bool trovato = false;
                                        for (int i = 0; i < valoriAssociati1.Length; i++)
                                        {
                                            string[] valoriAssociati2 = valoriAssociati1[i].Split('<');
                                            if (valoriAssociati2[1] == valore)
                                            {
                                                oggettoCustom.VALORE_DATABASE = valoriAssociati2[0];
                                                trovato = true;
                                            }
                                        }
                                        if (!trovato)
                                        {
                                            notePerElaborazioneXML += "Errore di associazione del campo " + oggettoCustom.DESCRIZIONE + ". Valore " + valore + " non valido. ";
                                            if (oggettoCustom.CAMPO_OBBLIGATORIO == "SI")
                                            {
                                                throw new Exception("Elaborazione XML: Errore di associazione del campo " + oggettoCustom.DESCRIZIONE + ". Il valore " + valore + " non ha un campo associato configurato");
                                            }
                                        }
                                    }


                                }
                                else if (oggettoCustom.TIPO.DESCRIZIONE_TIPO == "Data")
                                {
                                    string conversione = oggettoCustom.OPZIONI_XML_ASSOC;
                                    try
                                    {

                                        DateTime dtp = DateTime.ParseExact(valore.Substring(0, conversione.Length), conversione, System.Globalization.CultureInfo.InvariantCulture);

                                        oggettoCustom.VALORE_DATABASE = dtp.ToString("dd/MM/yyyy");
                                        if (!string.IsNullOrEmpty(oggettoCustom.FORMATO_ORA))
                                        {
                                            oggettoCustom.VALORE_DATABASE = dtp.ToString("dd/MM/yyyy " + oggettoCustom.FORMATO_ORA);
                                        }
                                    }
                                    catch (Exception exData)
                                    {
                                        oggettoCustom.VALORE_DATABASE = "";
                                        notePerElaborazioneXML += "Errore di associazione del campo " + oggettoCustom.DESCRIZIONE + ". Il valore " + valore + " non è compatibile con la stringa di conversione " + conversione + ". ";
                                        if (oggettoCustom.CAMPO_OBBLIGATORIO == "SI")
                                        {
                                            throw new Exception("Elaborazione XML: Errore di associazione del campo " + oggettoCustom.DESCRIZIONE + ". Il valore " + valore + " non è compatibile con la stringa di conversione " + conversione);
                                        }
                                    }

                                }
                                else if (oggettoCustom.TIPO.DESCRIZIONE_TIPO == "Corrispondente")
                                {
                                    DocsPaVO.rubrica.ParametriRicercaRubrica filtriRic = new DocsPaVO.rubrica.ParametriRicercaRubrica();
                                    filtriRic.doRubricaComune = true;
                                    filtriRic.tipoIE = DocsPaVO.addressbook.TipoUtente.ESTERNO;
                                    filtriRic.caller = new DocsPaVO.rubrica.ParametriRicercaRubrica.CallerIdentity();
                                    filtriRic.caller.IdRuolo = infoUtente.idGruppo;
                                    filtriRic.caller.IdUtente = infoUtente.idPeople;
                                    filtriRic.caller.filtroRegistroPerRicerca = string.Empty;
                                    string tipoRicerca = oggettoCustom.OPZIONI_XML_ASSOC.Split('§')[0];
                                    switch (tipoRicerca)
                                    {
                                        case "CODE":
                                            filtriRic.codice = valore;
                                            break;
                                        case "DESCRIZIONE":
                                            filtriRic.descrizione = valore;
                                            break;
                                        case "PIVA":
                                            filtriRic.partitaIva = valore;
                                            break;
                                        case "CF":
                                            filtriRic.codiceFiscale = valore;
                                            break;
                                        case "MAIL":
                                            filtriRic.email = valore;
                                            break;
                                    }
                                    DocsPaVO.rubrica.SmistamentoRubrica smistamentoRubrica = new DocsPaVO.rubrica.SmistamentoRubrica();
                                    var searchRequest = new rubricaGetElementiRubricaCommand()
                                    {
                                        Qc = filtriRic,
                                        U = infoUtente,
                                        SmistamentoRubrica = smistamentoRubrica
                                    };
                                    var searchResp = await this._mediator.Send(searchRequest);
                                    var objElementiRubrica = searchResp.Output;
                                    if (objElementiRubrica != null && objElementiRubrica.Count() > 0)
                                    {
                                        string sysId = "";
                                        if (!string.IsNullOrEmpty(((DocsPaVO.rubrica.ElementoRubrica)objElementiRubrica[0]).systemId))
                                        {
                                            sysId = ((DocsPaVO.rubrica.ElementoRubrica)objElementiRubrica[0]).systemId;
                                        }
                                        else
                                        {
                                            DocsPaVO.utente.Corrispondente corr = (await this._mediator.Send(
                                                new AddressbookGetCorrispondenteBySystemIdCommand()
                                            {
                                                SystemId = objElementiRubrica[0].codice
                                            })).Output;

                                            if ((corr == null) || 
                                                (corr != null && string.IsNullOrEmpty(corr.systemId) 
                                                && string.IsNullOrEmpty(corr.codiceRubrica)))
                                            {
                                                corr = (await this._mediator.Send(new GetCorrispondenteByCodRubricaRubricaComuneCommand()
                                                {
                                                    Codice = objElementiRubrica[0].codice,
                                                    InfoUtente = infoUtente,
                                                })).Output;
                                            }
                                        }
                                        oggettoCustom.VALORE_DATABASE = sysId;
                                    }
                                    else
                                    {
                                        oggettoCustom.VALORE_DATABASE = string.Empty;
                                        if (oggettoCustom.CAMPO_OBBLIGATORIO == "SI")
                                            throw new Exception("Elaborazione XML: Errore nell'associazione del campo " + oggettoCustom.DESCRIZIONE + ", Corrispondente non trovato con i dati presenti nello XML.");
                                        else
                                            notePerElaborazioneXML += "Errore di associazione del campo " + oggettoCustom.DESCRIZIONE + ". Il valore è presente nel file XML ma non è stato possibile associarlo automaticamente ad un corrispondente.  L’eventuale integrazione avviene manualmente. ";
                                    }

                                }
                            }
                            else
                            {
                                notePerElaborazioneXML += "Valore per il campo " + oggettoCustom.DESCRIZIONE + " non presente nel file XML. ";
                            }

                        }
                        catch (Exception e)
                        {
                            if (e.Message.Contains("Elaborazione XML:"))
                            {
                                throw new Exception(e.Message);
                            }
                            else
                            {
                                oggettoCustom.VALORE_DATABASE = string.Empty;
                                oggettoCustom.VALORI_SELEZIONATI = null;
                            }
                        }

                        if (oggettoCustom.CAMPO_OBBLIGATORIO == "SI" && (string.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE) || ((oggettoCustom.VALORI_SELEZIONATI == null || oggettoCustom.VALORI_SELEZIONATI.Count() < 1) && oggettoCustom.TIPO.DESCRIZIONE_TIPO == "CasellaDiSelezione")))
                        {
                            throw new Exception("Elaborazione XML: Valore assente per il campo obbligatorio " + oggettoCustom.DESCRIZIONE);
                        }
                    }
                }
            }

            for (int i = 0; i < oggettiCustom.Length; i++)
            {
                if (oggettiCustom[i].DESCRIZIONE.ToUpper() == "NOTE RELATIVE ALL'ELABORAZIONE XML")
                {
                    if (!string.IsNullOrEmpty(notePerElaborazioneXML))
                    {
                        if (notePerElaborazioneXML.Length > 220)
                        {
                            notePerElaborazioneXML = notePerElaborazioneXML.Substring(0, 220) + "...";
                        }
                        oggettiCustom[i].VALORE_DATABASE = notePerElaborazioneXML;
                    }
                    else
                        oggettiCustom[i].VALORE_DATABASE = "Elaborazione avvenuta con successo";
                }
            }


            return template;
        }

        #endregion

    }
}
