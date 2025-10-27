// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.amministrazione;
using DocsPaVO.ExternalServices;
using DocsPaVO.SmartClient;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.DocumentAggregate.ValueObjects;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FattElAttiveDaImportRequest = Pi3.App.Legacy.WebApi.Application.Requests.FattElAttiveDaImport;
using Pi3.Core.Services.Configuration;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Pi3.Core.Services.File.FileValidator;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.FattElAttiveDaImport
{
    public class FattElAttiveDaImportHandler : IRequestHandler<FattElAttiveDaImportRequest, FattElAttiveDaImportResult>
    {
        #region Public Members

        public FattElAttiveDaImportHandler(ILogger<FattElAttiveDaImportHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IDocumentoAmministrativoRepository documentoAmministrativoRepository,
            IDocumentBlobRepository documentBlobRepository,
            IFileValidatorService fileValidatorService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._documentoAmministrativoRepository = documentoAmministrativoRepository;
            this._documentBlobRepository = documentBlobRepository;
            this._fileValidatorService = fileValidatorService;

            this.InitializeMapper();
        }

        public async Task<FattElAttiveDaImportResult> Handle(FattElAttiveDaImportRequest request, CancellationToken cancellationToken)
        {
            var retVal = string.Empty;
            System.Xml.XmlDocument xmlDoc = null;
            DocsPaVO.documento.FileDocumento fileAllFatt = null;
            FornitoreFattAttiva fornitoreFA = null;
            string firmaCX = string.Empty;
            string stringaXml = null;
            var idTenant = request.infoUt.idAmministrazione.AsLong();
            bool isLottoAttivo = false;
            var idGruppo = request.infoUt.idGruppo.AsLong();

            try
            {
                var schedaDocumento = (await _mediator.Send(new Requests.DocumentoGetDettaglioDocumentoNoSecurity(request.infoUt,
                    request.idDoc,
                    request.idDoc))).output;

                foreach (var allFatt in schedaDocumento.documenti.Where(d => !string.IsNullOrEmpty(d.fileName) && d.fileName.ToUpper().Contains(".XML")).ToList())
                {
                    fileAllFatt = (await _mediator.Send(new Requests.DocumentoGetFile(allFatt, request.infoUt))).output;
                    if (fileAllFatt != null && !string.IsNullOrEmpty(fileAllFatt.fullName) && fileAllFatt.fullName.ToUpper().Contains("XML"))
                    {
                        firmaCX = allFatt.fileName.ToUpper().Contains(".P7M") ? "C" : string.Empty;
                        stringaXml = Encoding.UTF8.GetString(fileAllFatt.content).Trim();

                        xmlDoc = new System.Xml.XmlDocument();
                        if (stringaXml.Contains("xml version=\"1.1\""))
                        {
                            _logger.LogInformation("Versione XML 1.1. Provo conversione");
                            stringaXml = stringaXml.Replace("xml version=\"1.1\"", "xml version=\"1.0\"");
                        }
                        try
                        {
                            xmlDoc.LoadXml(stringaXml);
                        }
                        catch (Exception bomUTF8)
                        {
                            string byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
                            if (stringaXml.StartsWith(byteOrderMarkUtf8))
                            {
                                stringaXml = stringaXml.Remove(0, byteOrderMarkUtf8.Length);
                            }

                            xmlDoc.LoadXml(stringaXml);
                        }

                        if (xmlDoc.DocumentElement.NamespaceURI.ToLower().Contains(Resources.www_fatturapa_gov_it_sdi_fatturapa_v1) ||
                                xmlDoc.DocumentElement.NamespaceURI.ToLower().Contains(Resources.ivaservizi_agenziaentrate_gov_it_docs_xsd_fatture))
                        {
                            // controllo se la fattura � di uno dei fornitori in DB
                            var isFattLottoAttivo = false;

                            string mappFornitore = string.Format("//*[name()='{0}']/*[name()='{1}']/*[name()='{2}']/*[name()='{3}']", "CedentePrestatore", "DatiAnagrafici", "IdFiscaleIVA", "IdCodice");
                            string codFornitore = (xmlDoc.DocumentElement.SelectSingleNode(mappFornitore)).InnerXml;

                            if (!string.IsNullOrEmpty(codFornitore))
                            {
                                var fattAttivaCodFornitoreEntities = await _dbContext.FattAttivaCodFornitoreEntities.AsNoTracking()
                                    .Where(f => f.ID_AMM == idTenant && (f.COD_FORNITORE ?? string.Empty).ToUpper().Equals(codFornitore.ToUpper()))
                                    .FirstOrDefaultAsync();

                                if (fattAttivaCodFornitoreEntities == null)
                                    throw new CodiceFornitoreNotFoundPi3Exception(codFornitore);

                                var fornitore = _mapper.Map<FornitoreFattAttiva>(fattAttivaCodFornitoreEntities);
                                isFattLottoAttivo = true;
                                fornitoreFA = fornitore;
                            }

                            if (string.IsNullOrEmpty(firmaCX) && stringaXml.ToUpper().Contains("X509CERTIFICATE"))
                            {
                                firmaCX = "X";
                            }

                            if (string.IsNullOrEmpty(firmaCX))
                            {
                                string tempOutX5 = "";
                                System.Xml.XmlElement root = xmlDoc.DocumentElement;
                                string valore = root.Attributes["versione"].Value;
                                if (valore.ToUpper() != "FPR" && valore.ToUpper() != "FPR12")
                                {
                                    await _mediator.Send(new Requests.DocumentoExecCestina(request.infoUt, schedaDocumento, null, Resources.FatturaNonFirmata));
                                    throw new FatturaNonFirmataPi3Exception();
                                }
                            }

                            System.Xml.XmlNodeList fatture = xmlDoc.DocumentElement.SelectNodes("//*[name()='FatturaElettronicaBody']");
                            isLottoAttivo = fatture.Count > 1;

                            #region Associazione dei campi

                            var descrizioneTipologia = !isLottoAttivo ? Resources.DescTipologiaFatturaElettronicaAttiva : Resources.DescTipologiaLottoFattureAttive;
                            var idTemplate = await _dbContext.TipoAttoEntities.AsNoTracking()
                                .Where(a => a.ID_AMM == idTenant && a.VAR_DESC_ATTO.ToUpper().Equals(descrizioneTipologia.ToUpper()))
                                .Select(a => a.SYSTEM_ID)
                                .FirstOrDefaultAsync();

                            var ruoliTipoDocEntities = await _dbContext.VisTipoDocEntities.AsNoTracking()
                                .Where(r => r.ID_TIPO_DOC == idTemplate && r.ID_RUOLO == idGruppo && r.DIRITTI == 2)
                                .ToListAsync();

                            if (ruoliTipoDocEntities == null || ruoliTipoDocEntities.Count == 0)
                                throw new DirittiTipologiaPi3Exception();

                            var template = (await _mediator.Send(new Requests.getTemplateById(idTemplate.ToString()))).output;
                            if (template == null)
                                throw new TemplateFatturaNotFoundPi3Exception();

                            string notePerElaborazioneXML = "";
                            foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom in template.ELENCO_OGGETTI)
                            {
                                try
                                {
                                    if (!string.IsNullOrEmpty(oggettoCustom.CAMPO_XML_ASSOC))
                                    {
                                        bool associaSecondo = false;
                                        string[] mappings = oggettoCustom.CAMPO_XML_ASSOC.Split('<');
                                        string[] mappingXml = mappings[0].Split('>');
                                        string mappingElemento = String.Format("//*[name()='{0}']", mappingXml[0]);
                                        for (int i = 1; i < mappingXml.Length; i++)
                                        {
                                            mappingElemento += String.Format("/*[name()='{0}']", mappingXml[i]);
                                        }
                                        string valore = "";
                                        try
                                        {
                                            System.Xml.XmlNode node = xmlDoc.DocumentElement.SelectSingleNode(mappingElemento);
                                            valore = node.InnerXml; // valore dell'xml estratto
                                        }
                                        catch (Exception nodo)
                                        {
                                            if (mappings.Length > 1 && !string.IsNullOrEmpty(mappings[1]))
                                            {
                                                associaSecondo = true;
                                            }
                                            else if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("FATT_EL_FORNITORE_1"))
                                            {
                                                valore = "";
                                            }
                                            else if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("FATT_EL_CLIENTE_1"))
                                            {
                                                valore = "";
                                            }
                                            else if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("FATT_EL_NOTE_VER_FIRMA_1"))
                                            {
                                                //valore = "Verifica firma digitale effettuata da SDI, secondo le Specifiche tecniche operative delle Regole tecniche di cui all�allegato B del D.M. n. 55 del 3 aprile 2013 e ss.mm.ii.";
                                                valore = "";
                                            }
                                            else if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("FATT_EL_PIVA_CLIENTE_1"))
                                            {
                                                valore = "999";
                                            }
                                            else if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("FATT_EL_IDSDI"))
                                            {
                                                valore = "";
                                            }
                                            else if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("FATT_EL_VERSIONE_1"))
                                            {
                                                System.Xml.XmlElement root = xmlDoc.DocumentElement;
                                                valore = root.Attributes["versione"].Value;
                                            }
                                            else
                                            {
                                                notePerElaborazioneXML += "XML " + oggettoCustom.DESCRIZIONE + ". ";
                                            }
                                        }

                                        if (!string.IsNullOrEmpty(valore) && !string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("FATT_EL_CODFISC_1"))
                                        {
                                            if (!Char.IsLetter(valore[0]) || !Char.IsLetter(valore[1]) || !Char.IsLetter(valore[2]))
                                                valore = "";
                                        }

                                        if (associaSecondo)
                                        {
                                            int associaSecondoI = 1;
                                            while (string.IsNullOrEmpty(valore) && mappings.Length > associaSecondoI && !string.IsNullOrEmpty(mappings[associaSecondoI]))
                                            {
                                                mappingXml = mappings[associaSecondoI].Split('>');
                                                mappingElemento = String.Format("//*[name()='{0}']", mappingXml[0]);
                                                for (int i = 1; i < mappingXml.Length; i++)
                                                {
                                                    mappingElemento += String.Format("/*[name()='{0}']", mappingXml[i]);
                                                }
                                                valore = "";
                                                try
                                                {
                                                    System.Xml.XmlNode node = xmlDoc.DocumentElement.SelectSingleNode(mappingElemento);
                                                    valore = node.InnerXml; // valore dell'xml estratto
                                                }
                                                catch (Exception nodo)
                                                {

                                                }
                                                associaSecondoI++;
                                            }
                                            if (string.IsNullOrEmpty(valore))
                                            {
                                                notePerElaborazioneXML += "XML " + oggettoCustom.DESCRIZIONE + ". ";
                                            }
                                        }

                                        // Estrazione dei campi CDATA
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

                                                System.Xml.XmlNodeList multinode = xmlDoc.SelectNodes(mappingElemento);
                                                if (multinode.Count > 1)
                                                {
                                                    valore = "";
                                                    foreach (System.Xml.XmlNode nodoX in multinode)
                                                    {
                                                        if (!valore.Contains(nodoX.InnerXml))
                                                        {
                                                            valore += nodoX.InnerXml + separatore;
                                                        }
                                                    }
                                                }
                                                if (valore.Length > 180) valore = valore.Substring(0, 180);

                                            }
                                            #region Cliente fattura elettronica
                                            if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("FATT_EL_CLIENTE_1"))
                                            {
                                                if (string.IsNullOrEmpty(valore))
                                                {
                                                    string mappingNome = "CessionarioCommittente>DatiAnagrafici>Anagrafica>Nome";
                                                    mappingXml = mappingNome.Split('>');
                                                    mappingElemento = String.Format("//*[name()='{0}']", mappingXml[0]);
                                                    for (int i = 1; i < mappingXml.Length; i++)
                                                    {
                                                        mappingElemento += String.Format("/*[name()='{0}']", mappingXml[i]);
                                                    }
                                                    valore = "";
                                                    try
                                                    {
                                                        System.Xml.XmlNode node = xmlDoc.DocumentElement.SelectSingleNode(mappingElemento);
                                                        valore = node.InnerXml; // valore dell'xml estratto
                                                    }
                                                    catch (Exception nodo)
                                                    {
                                                        notePerElaborazioneXML += "XML " + oggettoCustom.DESCRIZIONE + ". ";
                                                    }

                                                    string mappingCognome = "CessionarioCommittente>DatiAnagrafici>Anagrafica>Cognome";
                                                    mappingXml = mappingCognome.Split('>');
                                                    mappingElemento = String.Format("//*[name()='{0}']", mappingXml[0]);
                                                    for (int i = 1; i < mappingXml.Length; i++)
                                                    {
                                                        mappingElemento += String.Format("/*[name()='{0}']", mappingXml[i]);
                                                    }
                                                    try
                                                    {
                                                        System.Xml.XmlNode node = xmlDoc.DocumentElement.SelectSingleNode(mappingElemento);
                                                        valore += (" " + node.InnerXml); // valore dell'xml estratto
                                                    }
                                                    catch (Exception nodo)
                                                    {
                                                        notePerElaborazioneXML += "XML " + oggettoCustom.DESCRIZIONE + ". ";
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
                                                    mappingElemento = String.Format("//*[name()='{0}']", mappingXml[0]);
                                                    for (int i = 1; i < mappingXml.Length; i++)
                                                    {
                                                        mappingElemento += String.Format("/*[name()='{0}']", mappingXml[i]);
                                                    }
                                                    try
                                                    {
                                                        System.Xml.XmlNode node = xmlDoc.DocumentElement.SelectSingleNode(mappingElemento);
                                                        valore = node.InnerXml; // valore dell'xml estratto
                                                    }
                                                    catch (Exception nodo)
                                                    {
                                                        notePerElaborazioneXML += "Nodo XML mancante per il campo " + oggettoCustom.DESCRIZIONE + ". ";
                                                    }
                                                }
                                            }
                                            #endregion
                                            oggettoCustom.VALORE_DATABASE = valore;
                                        }
                                        if (!string.IsNullOrEmpty(valore))
                                        {
                                            if (oggettoCustom.TIPO.DESCRIZIONE_TIPO == "MenuATendina" || oggettoCustom.TIPO.DESCRIZIONE_TIPO == "SelezioneEsclusiva")
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
                                                    notePerElaborazioneXML += "Errore " + oggettoCustom.DESCRIZIONE + ". " + valore + " non valido. ";
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
                                                    notePerElaborazioneXML += "Errore " + oggettoCustom.DESCRIZIONE + ". " + valore + " non valido. ";
                                                }

                                            }
                                            else if (oggettoCustom.TIPO.DESCRIZIONE_TIPO == "Corrispondente")
                                            {
                                                #region Da rifare con metodi da Frontend
                                                DocsPaVO.rubrica.ParametriRicercaRubrica filtriRic = new DocsPaVO.rubrica.ParametriRicercaRubrica();
                                                filtriRic.doRubricaComune = true;
                                                filtriRic.tipoIE = DocsPaVO.addressbook.TipoUtente.ESTERNO;
                                                filtriRic.caller = new DocsPaVO.rubrica.ParametriRicercaRubrica.CallerIdentity();
                                                filtriRic.caller.IdRuolo = request.infoUt.idGruppo;
                                                filtriRic.caller.IdUtente = request.infoUt.idPeople;
                                                filtriRic.caller.filtroRegistroPerRicerca = string.Empty;
                                                string tipoRicerca = oggettoCustom.OPZIONI_XML_ASSOC.Split('�')[0];
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
                                                var objElementiRubrica = (await _mediator.Send(new Requests.rubricaGetElementiRubrica(filtriRic, request.infoUt, smistamentoRubrica))).output;
                                                if (objElementiRubrica != null && objElementiRubrica.Count() > 0)
                                                {
                                                    string sysId = string.Empty;

                                                    if (!string.IsNullOrEmpty(((DocsPaVO.rubrica.ElementoRubrica)objElementiRubrica[0]).systemId))
                                                    {
                                                        sysId = ((DocsPaVO.rubrica.ElementoRubrica)objElementiRubrica[0]).systemId;
                                                    }
                                                    else
                                                    {
                                                        DocsPaVO.utente.Corrispondente corr = (await _mediator.Send(new Requests.AddressbookGetCorrispondenteBySystemId(((DocsPaVO.rubrica.ElementoRubrica)objElementiRubrica[0]).codice))).output;
                                                        if ((corr == null) || (corr != null && string.IsNullOrEmpty(corr.systemId) && string.IsNullOrEmpty(corr.codiceRubrica)))
                                                        {
                                                            corr = (await _mediator.Send(new Requests.GetCorrRubricaComune(((DocsPaVO.rubrica.ElementoRubrica)objElementiRubrica[0]).codice, request.infoUt))).output;

                                                            if (corr != null)
                                                                sysId = corr.systemId;
                                                        }
                                                    }
                                                    oggettoCustom.VALORE_DATABASE = sysId;
                                                }
                                                else
                                                {
                                                    oggettoCustom.VALORE_DATABASE = string.Empty;
                                                    notePerElaborazioneXML += "Errore di associazione del campo " + oggettoCustom.DESCRIZIONE + ". Il valore � presente nel file XML ma non � stato possibile associarlo automaticamente ad un corrispondente.  L�eventuale integrazione avviene manualmente. ";
                                                }

                                                #endregion
                                            }
                                            else if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("CasellaDiSelezione"))
                                            {

                                            }
                                        }
                                        else
                                        {
                                            notePerElaborazioneXML += oggettoCustom.DESCRIZIONE + ". ";
                                        }

                                    }
                                    else if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC))
                                    {
                                        if (oggettoCustom.OPZIONI_XML_ASSOC.ToUpper() == "FATT_EL_ATT_AUTOREPERTORIAZIONE" && fornitoreFA != null && !string.IsNullOrEmpty(fornitoreFA.IdRegistro))
                                        {
                                            if (((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("Contatore") || (oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("ContatoreSottocontatore")))
                                            {
                                                DocsPaVO.utente.Registro reg = (await _mediator.Send(new Requests.GetRegistroBySistemId(fornitoreFA.IdRegistro))).output;

                                                if (reg != null)
                                                {
                                                    oggettoCustom.ID_AOO_RF = reg.systemId;
                                                    oggettoCustom.CONTATORE_DA_FAR_SCATTARE = true;
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex1end)
                                {
                                    oggettoCustom.VALORE_DATABASE = string.Empty;
                                    oggettoCustom.VALORI_SELEZIONATI = null;
                                }
                            }

                            foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom notePerXML in template.ELENCO_OGGETTI)
                            {
                                if (notePerXML.DESCRIZIONE.ToUpper() == "NOTE RELATIVE ALL'ELABORAZIONE XML")
                                {
                                    if (!string.IsNullOrEmpty(notePerElaborazioneXML))
                                    {
                                        notePerElaborazioneXML = "Mancanti: " + notePerElaborazioneXML;
                                        if (notePerElaborazioneXML.Length > 220)
                                        {
                                            notePerElaborazioneXML = notePerElaborazioneXML.Substring(0, 220) + "...";
                                        }
                                        notePerXML.VALORE_DATABASE = notePerElaborazioneXML;
                                    }
                                    else
                                        notePerXML.VALORE_DATABASE = "Elaborazione avvenuta con successo";
                                }
                            }

                            schedaDocumento.template = template;
                            schedaDocumento.tipologiaAtto = new DocsPaVO.documento.TipologiaAtto();
                            schedaDocumento.tipologiaAtto.systemId = template.SYSTEM_ID.ToString();
                            schedaDocumento.tipologiaAtto.descrizione = template.DESCRIZIONE;
                            schedaDocumento.daAggiornareTipoAtto = true;

                            bool daaggiornTemp = false;
                            schedaDocumento = (await _mediator.Send(new Requests.DocumentoSaveDocumento(null, request.infoUt, schedaDocumento, false))).output;

                            #endregion

                            #region Caricamento allegati fattura se presenti
                            System.Xml.XmlNodeList listanodi = xmlDoc.DocumentElement.SelectNodes("//*[name()='Allegati']");
                            System.Xml.XmlNode nodoNome = null;
                            System.Xml.XmlNode nodoContent = null;
                            DocsPaVO.documento.FileRequest allegato = null;
                            DocsPaVO.documento.FileDocumento fileAllegato = null;
                            string erroreMessage = "";
                            bool caricaAllegato = true;
                            foreach (System.Xml.XmlNode nodo in listanodi)
                            {
                                nodoNome = null;
                                nodoContent = null;
                                caricaAllegato = true;

                                foreach (System.Xml.XmlNode nodo1 in nodo.ChildNodes)
                                {
                                    if (nodo1.Name.ToUpper() == "NOMEATTACHMENT") nodoNome = nodo1;
                                    if (nodo1.Name.ToUpper() == "ATTACHMENT") nodoContent = nodo1;
                                }

                                foreach (DocsPaVO.documento.Allegato alltempx1 in schedaDocumento.allegati)
                                {
                                    if (alltempx1.descrizione.ToUpper() == nodoNome.InnerXml.ToUpper()) caricaAllegato = false;
                                }
                                if (caricaAllegato)
                                {
                                    var aggregateAllegato = new DocumentoAmministrativo(idTenant.ToString(),
                                           DateTime.Now,
                                           new OggettoDelDocumento()
                                           {
                                               Descrizione = new TextValue(nodoNome.InnerXml)
                                           },
                                           null,
                                           null, null,
                                           new IdDoc()
                                           {
                                               Identiticativo = schedaDocumento.systemId
                                           });

                                    var sysDateTime = await _dbContext.GetSystemDateTime();
                                    var newDocumentBlobAggregate = new DocumentBlob(idTenant.ToString(), sysDateTime, new TextValue(nodoNome.InnerXml));
                                    newDocumentBlobAggregate.UploadStream(new MemoryStream(Convert.FromBase64String(nodoContent.InnerXml)), nodoNome.InnerXml);
                                    newDocumentBlobAggregate.ComputeHash(Core.AggregateModels.DocumentBlobAggregate.ValueObjects.HashNamesEnum.SHA256);

                                    await _documentBlobRepository.Add(newDocumentBlobAggregate);

                                    aggregateAllegato.AssignDocumentBlobRef(
                                        new DocumentBlobRef()
                                        {
                                            IdBlob = newDocumentBlobAggregate.Id,
                                            FileName = newDocumentBlobAggregate.FileName,
                                            ContentType = newDocumentBlobAggregate.ContentType,
                                            FileSize = newDocumentBlobAggregate.FileSize,
                                            CreationDate = await _dbContext.GetSystemDateTime(),
                                            Hash = newDocumentBlobAggregate.Hash,
                                            HashName = Core.AggregateModels.DocumentAggregate.ValueObjects.HashNamesEnum.SHA256,
                                            Cartaceo = false
                                        },
                                        new TargetVersionBehavior()
                                        {
                                            CreateNewVersion = true
                                        });

                                    await _documentoAmministrativoRepository.Add(aggregateAllegato);

                                    var fileValidateAllegatoResult = await this._fileValidatorService.Validate(new FileToValidate()
                                    {
                                        Name = newDocumentBlobAggregate.FileName,
                                        Stream = newDocumentBlobAggregate.Stream
                                    });

                                    await _mediator.Send(new Requests.DocumentoAddInfoFileRequest(new DocsPaVO.documento.FileRequest()
                                    {
                                        docNumber = aggregateAllegato.Id,
                                        versionId = aggregateAllegato.CurrentVersion.Id,
                                        fileName = newDocumentBlobAggregate.FileName,
                                        dataAcquisizione = newDocumentBlobAggregate.CreationDate.AsDateTimeFormat()
                                    },
                                    aggregateAllegato.IdDocPrimario?.Identiticativo.AsLong(),
                                    fileValidateAllegatoResult));
                                }

                            }
                            #endregion

                            retVal = "OK";
                        }
                    }
                }

            }
            catch (Pi3Exception pi3Ex)
            {
                this._logger.LogError(exception: pi3Ex, message: pi3Ex.Message);
                retVal = pi3Ex.Message;
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
                retVal = ex.Message;
            }

            return new FattElAttiveDaImportResult(retVal);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<FattElAttiveDaImportHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IDocumentoAmministrativoRepository _documentoAmministrativoRepository;
        protected readonly IDocumentBlobRepository _documentBlobRepository;
        protected readonly IFileValidatorService _fileValidatorService;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<FattAttivaCodFornitoreEntity, FornitoreFattAttiva>()
                    .ForMember(dest => dest.IdAmm, src => src.MapFrom(opt => opt.ID_AMM))
                    .ForMember(dest => dest.IdRegistro, src => src.MapFrom(opt => opt.ID_REGISTRO))
                    .ForMember(dest => dest.CodFornitore, src => src.MapFrom(opt => opt.COD_FORNITORE))
                    .ForMember(dest => dest.CodFascicolo, src => src.MapFrom(opt => opt.COD_FASCICOLO))
                    .ForMember(dest => dest.CodAmmIPA, src => src.MapFrom(opt => opt.CODICE_AMM_IPA));
            });

            this._mapper = configuration.CreateMapper();
        }
        #endregion
    }
}