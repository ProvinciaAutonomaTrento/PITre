// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.ElementAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml;
using Microsoft.EntityFrameworkCore;
using SUAPEnte;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.SuapManager.ExtractXmlSuap
{
    public class ExtractXmlSuapHandler : IRequestHandler<ExtractXmlSuapRequest, ExtractXmlSuapResult>
    {
        #region Public Members

        public ExtractXmlSuapHandler(ILogger<ExtractXmlSuapHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IDocumentoAmministrativoRepository documentoAmministrativoRepository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._documentoAmministrativoRepository = documentoAmministrativoRepository;
        }

        public async Task<ExtractXmlSuapResult> Handle(ExtractXmlSuapRequest request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.fileName.ToLower().EndsWith(Descriptions.SuapXML))
                {
                    await ImportSuapEnteXMLIntoTemplate(request.schedaDoc, request.filecontents);
                }
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
            }

            return new ExtractXmlSuapResult();
        }

        #endregion

        #region Private Members

        protected readonly ILogger<ExtractXmlSuapHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IDocumentoAmministrativoRepository _documentoAmministrativoRepository;
        protected List<string> ValidationErrorList = new List<string>();

        protected async Task<bool> ImportSuapEnteXMLIntoTemplate(DocsPaVO.documento.SchedaDocumento schedaDoc, byte[] xmlcontent)
        {
            var result = true;
            var idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
            try
            {
                var xml = System.Text.UTF8Encoding.UTF8.GetString(xmlcontent);

                //if (!ValidateXmlString(xml, Resources.pratica_suap_1_0_1, null))
                //    throw new ValidazioneXmlPi3Exception();

                var idTemplate = await _dbContext.TipoAttoEntities.AsNoTracking()
                                .Where(t => t.VAR_DESC_ATTO.ToUpper() == Descriptions.Suapente && t.ID_AMM == idTenant)
                                .Select(t => t.SYSTEM_ID)
                                .FirstOrDefaultAsync();

                if (idTemplate == 0)
                    throw new TipologiaNotFoundPi3Exception(Descriptions.Suapente);

                var templateSuap = (await _mediator.Send(new Requests.getTemplateById(idTemplate.ToString()))).output;

                SUAPPratica.RiepilogoPraticaSUAP riep = null;
                using (var reader = new StringReader(xml))
                {
                    var serializer = new XmlSerializer(typeof(SUAPPratica.RiepilogoPraticaSUAP));
                    riep = (SUAPPratica.RiepilogoPraticaSUAP)serializer.Deserialize(reader);
                }

                popolaValoreTipologiaGenerico(templateSuap, Descriptions.Procedimento, ComponiProcedimento(riep.struttura));
                popolaValoreTipologiaGenerico(templateSuap, Descriptions.TipoProcedimento, riep.intestazione.oggettocomunicazione.tipoprocedimento.ToString());
                popolaValoreTipologiaGenerico(templateSuap, Descriptions.TipoIntervento, riep.intestazione.oggettocomunicazione.tipointervento.ToString());
                popolaValoreTipologiaGenerico(templateSuap, Descriptions.CodicePratica, riep.intestazione.codicepratica);
                popolaValoreTipologiaGenerico(templateSuap, Descriptions.Impresa, ComponiImpresa(riep.intestazione.impresa));
                popolaValoreTipologiaGenerico(templateSuap, Descriptions.CodiceREA, riep.intestazione.impresa.codiceREA.Value);
                popolaValoreTipologiaGenerico(templateSuap, Descriptions.LegaleRappresentante, ComponiLegaleRapp(riep.intestazione.impresa.legalerappresentante));
                popolaValoreTipologiaGenerico(templateSuap, Descriptions.Dichiarante, ComponiDichiarante(riep.intestazione.dichiarante));
                popolaValoreTipologiaGenerico(templateSuap, Descriptions.DomicilioElettronico, riep.intestazione.domicilioelettronico);
                popolaValoreTipologiaGenerico(templateSuap, Descriptions.ImpiantoProduttivo, ComponiIndirizzo(riep.intestazione.impiantoproduttivo.indirizzo));
                popolaValoreTipologiaGenerico(templateSuap, Descriptions.ProcuraSpeciale, riep.intestazione.procuraspeciale.nomefile);
                popolaValoreTipologiaGenerico(templateSuap, Descriptions.Allegati, ComponiAllegati(riep.struttura));

                var nuovoOggetto = CostruisciNuovoOggetto(schedaDoc, riep);

                var documentoAmministrativoAggregate = await this._documentoAmministrativoRepository.Get(idTenant.ToString(), schedaDoc.systemId, new ILoadBehavior[1]
                {
                    new GetDocumentoAmministrativoLoadBehavior()
                    {
                        LoadProfiles = false,
                        LoadProfilesMetadata = false,
                        LoadClassifications = false,
                        LoadAllegati = false,
                        LoadAggregazioni = true,
                        LoadVersions = true,
                        LoadPermissions = false,
                        LoadMittentiDestinatari = true,
                        LoadKeywords = true,
                        LoadNote = true,
                        MittentiDestinatariPagination = new Pagination() { Skip = 0, Take = 10000 }
                    }
                });

                documentoAmministrativoAggregate.AddProfile(templateSuap.SYSTEM_ID.ToString(), new TextValue(templateSuap.DESCRIZIONE));

                foreach (var oggettoCustom in templateSuap.ELENCO_OGGETTI)
                {
                    switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
                    {
                        case "Contatore":
                        case "ContatoreSottocontatore":
                            if (oggettoCustom.TIPO_CONTATORE == "T" && (string.IsNullOrEmpty(oggettoCustom.ID_AOO_RF) || oggettoCustom.ID_AOO_RF == "0"))
                                oggettoCustom.ID_AOO_RF = schedaDoc.registro.systemId;

                            documentoAmministrativoAggregate.AddProfileField(
                            templateSuap.SYSTEM_ID.ToString(),
                            oggettoCustom.SYSTEM_ID.ToString(),
                            new TextValue(oggettoCustom.DESCRIZIONE),
                            oggettoCustom.TIPO.DESCRIZIONE_TIPO,
                            new ContatoreRepertorioFieldValue(oggettoCustom.ID_AOO_RF, oggettoCustom.CONTATORE_DA_FAR_SCATTARE, oggettoCustom.RESETTA_CONTATORE_INIZIO_ANNO == "SI"));
                            break;
                        case "CasellaDiSelezione":
                            documentoAmministrativoAggregate.AddProfileField(
                            templateSuap.SYSTEM_ID.ToString(),
                            oggettoCustom.SYSTEM_ID.ToString(),
                            new TextValue(oggettoCustom.DESCRIZIONE),
                            oggettoCustom.TIPO.DESCRIZIONE_TIPO,
                            new ElementFieldMultiValue(oggettoCustom.VALORI_SELEZIONATI.Select(s => new TextValue(s)).ToArray()));
                            break;
                        default:
                            documentoAmministrativoAggregate.AddProfileField(
                            templateSuap.SYSTEM_ID.ToString(),
                            oggettoCustom.SYSTEM_ID.ToString(),
                            new TextValue(oggettoCustom.DESCRIZIONE),
                            oggettoCustom.TIPO.DESCRIZIONE_TIPO,
                            new ElementFieldSingleValue(new TextValue(oggettoCustom.VALORE_DATABASE)));
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(nuovoOggetto))
                {
                    if (!string.IsNullOrEmpty(nuovoOggetto))
                    {
                        documentoAmministrativoAggregate.ChangeOggettoDelDocumento(new OggettoDelDocumento()
                        {
                            Descrizione = new TextValue(nuovoOggetto)
                        });
                    }
                }

                await _documentoAmministrativoRepository.Update(documentoAmministrativoAggregate);
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
                result = false;
            }

            return result;
        }
        private string CostruisciNuovoOggetto(DocsPaVO.documento.SchedaDocumento sd, SUAPPratica.RiepilogoPraticaSUAP riepilogoPratica)
        {
            try
            {
                if (riepilogoPratica == null)
                    return null;

                if (riepilogoPratica.struttura == null)
                    return null;

                List<string> nomiProcList = new List<string>();
                List<string> mailentiList = new List<string>();
                string procedimentoCompleto = string.Empty;
                string procedimento = "";
                foreach (SUAPPratica.AdempimentoSUAP struttura in riepilogoPratica.struttura)
                {
                    procedimentoCompleto = struttura.nome;
                    string[] nomiProcedimento = procedimentoCompleto.Replace("++", "§").Split('§');
                    foreach (string np in nomiProcedimento)
                        nomiProcList.Add(np.Trim());

                    foreach (SUAPPratica.EstremiEnte EnteConinvoto in struttura.entecoinvolto)
                        mailentiList.Add(EnteConinvoto.pec);
                }

                string mailDest = GetMailDestinatarioPredisposto(sd);
                if (!string.IsNullOrEmpty(mailDest))
                {
                    bool found = false;
                    if (nomiProcList.Count == mailentiList.Count)
                    {
                        int counter = 0;
                        foreach (string email in mailentiList)
                        {
                            if (email.ToUpper().Trim() == mailDest.ToUpper().Trim())
                            {
                                found = true;
                                break;
                            }
                            counter++;
                        }
                        if (nomiProcList.Count > counter)
                        {
                            if (found)
                                procedimento = nomiProcList[counter];
                        }

                        if (found == false)
                            procedimento = procedimentoCompleto;
                    }
                    else
                    {
                        procedimento = procedimentoCompleto;
                    }
                }
                else
                {
                    procedimento = procedimentoCompleto;
                }

                string impresa = "";
                string ImpiantoProduttivo = "";
                if (riepilogoPratica.intestazione != null)
                {
                    if (riepilogoPratica.intestazione.impresa != null)
                        if (!string.IsNullOrEmpty(riepilogoPratica.intestazione.impresa.ragionesociale))
                            impresa = riepilogoPratica.intestazione.impresa.ragionesociale;



                    ImpiantoProduttivo = ComponiIndirizzo(riepilogoPratica.intestazione.impiantoproduttivo.indirizzo);
                }

                string retval = string.Format("{0} | {1} | {2}", procedimento, impresa, ImpiantoProduttivo);

                return retval;
            }
            catch (Exception e)
            {
            }
            return null;
        }

        private string GetMailDestinatarioPredisposto(DocsPaVO.documento.SchedaDocumento sd)
        {
            string retval = null;
            try
            {
                retval = _dbContext.AssDocMailInteropEntities.AsNoTracking()
                    .Where(a => a.ID_PROFILE == sd.docNumber.AsLong())
                    .Select(x => x.VAR_EMAIL_REGISTRO)
                    .FirstOrDefault();
            }
            catch (Exception e)
            {

            }
            return retval;
        }


        #region compositoriCampiSuap
        private static string ComponiProcedimento(List<SUAPPratica.AdempimentoSUAP> strutturaList)
        {
            string retval = string.Empty;
            foreach (SUAPPratica.AdempimentoSUAP struttura in strutturaList)
            {
                string[] nomiProcedimento = struttura.nome.Replace("++", "§").Split('§');
                foreach (string np in nomiProcedimento)
                    retval += np.Trim() + "\r\n";
            }

            return retval.EndsWith("\r\n") ? retval.Substring(0, retval.Length - 2) : retval;
        }

        private static string ComponiAllegati(List<SUAPPratica.AdempimentoSUAP> strutturaList)
        {
            string retval = string.Empty;
            foreach (SUAPPratica.AdempimentoSUAP struttura in strutturaList)
            {
                foreach (SUAPPratica.AllegatoGenerico allegato in struttura.documentoallegato)
                    retval += allegato.nomefile.Trim() + "\r\n";

                if (struttura.distintamodelloattivita != null)
                {
                    if (!string.IsNullOrEmpty(struttura.distintamodelloattivita.nomefile))
                        retval += struttura.distintamodelloattivita.nomefile + "\r\n";

                    if (struttura.distintamodelloattivita.tracciatoxml != null)
                        if (!string.IsNullOrEmpty(struttura.distintamodelloattivita.tracciatoxml.nomefile))
                            retval += struttura.distintamodelloattivita.tracciatoxml.nomefile + "\r\n";
                }
            }
            return retval.EndsWith("\r\n") ? retval.Substring(0, retval.Length - 2) : retval;
        }

        private static string RemoveLastCRLF(string retval)
        {
            if (retval.EndsWith("\r\n"))
                retval = retval.Substring(0, retval.Length - 2);
            return retval;
        }

        private static string ComponiImpresa(SUAPPratica.AnagraficaImpresa impresa)
        {
            string PIString = string.Empty;
            string CFString = string.Empty;
            if (!string.IsNullOrEmpty(impresa.codicefiscale))
                CFString = string.Format("CF:{0}", impresa.codicefiscale);

            if (!string.IsNullOrEmpty(impresa.partitaiva))
            {
                CFString += "  -  ";
                PIString = string.Format("PI:{0}", impresa.partitaiva);
            }

            return string.Format("{0}\r\n{2}{3}\r\n{1}\r\n{4}", impresa.ragionesociale, impresa.formagiuridica.Value, CFString, PIString, ComponiIndirizzo(impresa.indirizzo));
        }

        private static string ComponiDichiarante(SUAPPratica.EstremiDichiarante dichiarante)
        {
            string retval = string.Format("{0} {1} - {2}\r\n{3}\r\n{4} {5}", dichiarante.cognome, dichiarante.nome, dichiarante.codicefiscale, dichiarante.qualifica, dichiarante.pec, dichiarante.telefono);
            return retval;
        }

        private static string ComponiLegaleRapp(SUAPPratica.AnagraficaRappresentante legaleRapp)
        {
            string retval = string.Format("{0} {1} - {2} - {3}", legaleRapp.cognome, legaleRapp.nome, legaleRapp.codicefiscale, legaleRapp.carica.Value);
            return retval;
        }

        private static string ComponiIndirizzo(SUAPPratica.Indirizzo indirizzo)
        {
            string comune = "";
            string provincia = "";
            foreach (object o in indirizzo.Items)
            {
                SUAPPratica.Comune comunestr = o as SUAPPratica.Comune;
                SUAPPratica.Provincia provinciastr = o as SUAPPratica.Provincia;

                if (comunestr != null)
                    comune = " " + comunestr.Value;

                if (provinciastr != null)
                    provincia = "(" + provinciastr.sigla + ")";

            }

            string retval = string.Format("{0} {1} {2} {3} {4} {5}", indirizzo.toponimo, indirizzo.denominazionestradale, indirizzo.numerocivico, indirizzo.cap, comune, provincia);
            return retval;
        }
        #endregion

        #region popolatori campi profilati

        private bool popolaValoreTipologiaCampoTestuale(DocsPaVO.ProfilazioneDinamica.Templates t, string nome, string valore)
        {
            if (string.IsNullOrEmpty(valore))
                return false;

            DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg = trovaOggettoPerNome(t, nome);
            if (ogg == null)
                return false;

            if (ogg != null)
            {
                var maxLength = !string.IsNullOrEmpty(ogg.NUMERO_DI_CARATTERI) && ogg.NUMERO_DI_CARATTERI != "0" ? Convert.ToInt32(ogg.NUMERO_DI_CARATTERI) : 255;
                ogg.VALORE_DATABASE = valore.Length > maxLength ? valore.Substring(0, maxLength - 3) + "..." : valore;
                return true;
            }
            return false;
        }

        private bool popolaValoreTipologiaDropDown(DocsPaVO.ProfilazioneDinamica.Templates t, string nome, string valore)
        {
            if (string.IsNullOrEmpty(valore))
                return false;

            if (valore.Contains('-'))
                valore = valore.Replace('-', '_');

            DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg = trovaOggettoPerNome(t, nome);

            if (ogg == null)
                return false;

            DocsPaVO.ProfilazioneDinamica.ValoreOggetto valoDef = null;
            foreach (DocsPaVO.ProfilazioneDinamica.ValoreOggetto valo in ogg.ELENCO_VALORI)
            {
                if (valo.VALORE.ToLower().Equals(valore.ToLower()) &&
                    valo.ABILITATO == 1
                    )
                {
                    if (valo.VALORE_DI_DEFAULT == "SI")
                        valoDef = valo;
                    ogg.VALORE_DATABASE = valo.VALORE;
                    return true;
                }
            }
            //popolo il default
            ogg.VALORE_DATABASE = valoDef.VALORE;

            return false;
        }

        private DocsPaVO.ProfilazioneDinamica.OggettoCustom trovaOggettoPerNome(DocsPaVO.ProfilazioneDinamica.Templates t, string nome)
        {
            foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg in t.ELENCO_OGGETTI)
                if (ogg.DESCRIZIONE.ToLower().Equals(nome.ToLower()))
                    return ogg;

            return null;
        }

        private bool popolaValoreTipologiaGenerico(DocsPaVO.ProfilazioneDinamica.Templates t, string nome, string valore)
        {
            if (string.IsNullOrEmpty(valore))
                return false;

            DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg = trovaOggettoPerNome(t, nome);

            if (ogg == null)
                return false;

            if (ogg.TIPO.DESCRIZIONE_TIPO.Equals("MenuATendina"))
                return popolaValoreTipologiaDropDown(t, nome, valore);

            if (ogg.TIPO.DESCRIZIONE_TIPO.Equals("CampoDiTesto"))
                return popolaValoreTipologiaCampoTestuale(t, nome, valore);

            return false;
        }

        private string getValoreOggettoGenerico(DocsPaVO.ProfilazioneDinamica.Templates t, string nome)
        {
            string retval = string.Empty;
            DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg = trovaOggettoPerNome(t, nome);
            if (ogg.TIPO.DESCRIZIONE_TIPO.Equals("MenuATendina"))
            {
                retval = ogg.VALORE_DATABASE;
                if (retval.Contains('_'))
                    retval = retval.Replace('_', '-');

            }
            else

                retval = ogg.VALORE_DATABASE;

            return retval;
        }

        #endregion

        protected bool ValidateXmlString(string xml, string xsd, string targetNameSpace)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.XmlResolver = null;
            XmlReader xreader = XmlTextReader.Create(new MemoryStream(System.Text.ASCIIEncoding.ASCII.GetBytes(xml)), settings);
            XmlDocument doc = new XmlDocument();
            doc.Load(xreader);

            try
            {
                XDocument xdoc = null;
                var settings2 = new XmlReaderSettings();
                settings2.DtdProcessing = DtdProcessing.Ignore;
                settings2.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);
                StringReader sr2 = new StringReader(xml);

                using (XmlReader xr = XmlReader.Create(sr2, settings2))
                {
                    xdoc = XDocument.Load(xr);
                    var schemas = new XmlSchemaSet();
                    using (MemoryStream ms = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(xsd)))
                    using (var reader = XmlReader.Create(ms, new XmlReaderSettings()
                    {
                        DtdProcessing = DtdProcessing.Ignore
                    }))
                        schemas.Add(targetNameSpace, reader);

                    xdoc.Validate(schemas, ValidationCallBack);
                }
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
            }
            finally
            {
                xreader.Close();
                doc.Clone();
            }

            return ValidationErrorList.Count() > 0;
        }

        private void ValidationCallBack(object sender, ValidationEventArgs e)
        {
            if (e.Severity == XmlSeverityType.Error)
                ValidationErrorList.Add(e.Message);
        }

        protected T FromXmlString<T>(T value, string xml)
        {
            using (var reader = new StringReader(xml))
            {
                var serializer = new XmlSerializer(typeof(T));

                return (T)serializer.Deserialize(reader);
            }
        }
        #endregion
    }
}
