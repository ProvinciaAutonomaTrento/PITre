// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pi3.App.RubricaComune.Infrastructure.EF.Entities;
using Pi3.App.RubricaComune.WebApi.Application.Commands.Corrispondenti;
using Pi3.App.RubricaComune.WebApi.Application.Queries.Corrispondenti.Search;
using Pi3.App.RubricaComune.WebApi.Application.Queries.Corrispondenti.SearchIpa.Models;
using Pi3.App.RubricaComune.WebApi.Application.Queries.Corrispondenti.SearchIpa.Models.Response;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using System.DirectoryServices.Protocols;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;
using Pi3.App.RubricaComune.WebApi.Application.Queries.Corrispondenti.GetEmails;


namespace Pi3.App.RubricaComune.WebApi.Application.Queries.Corrispondenti.SearchIpa
{
    public class SearchIpaHandler : IRequestHandler<SearchIpaRequest, SearchIpaResponse>
    {

        public SearchIpaHandler(
            ILogger<SearchIpaHandler> logger, 
            IMediator mediator,
            IConfiguration configuration)
        {
            this._logger = logger;
            this._mediator = mediator;
            this._mapper = this.InitializeMapper();            

            this._httpClient = new HttpClient()
            {
                BaseAddress = new Uri(configuration.GetSection("IpaOptions:IpaBaseUrl").Value ?? string.Empty)
            };
            this._authId = configuration.GetSection("IpaOptions:AuthId").Value ?? string.Empty;
        }


        public async Task<SearchIpaResponse> Handle(SearchIpaRequest request, CancellationToken cancellationToken)
        {
            SearchIpaResponse response = new();
            List<Corrispondente> cors = new();
            try
            {
                string? email = request.CriteriRicerca.FirstOrDefault(c => c.Campo.Equals(CampiRicercaEnum.Email))?.Valore;
                string? codice = request.CriteriRicerca.FirstOrDefault(c => c.Campo.Equals(CampiRicercaEnum.Codice))?.Valore;
                string? codiceFiscale = request.CriteriRicerca.FirstOrDefault(c => c.Campo.Equals(CampiRicercaEnum.CodiceFiscale))?.Valore;
                string? partitaIva = request.CriteriRicerca.FirstOrDefault(c => c.Campo.Equals(CampiRicercaEnum.PartitaIva))?.Valore;
                string? descrizione = request.CriteriRicerca.FirstOrDefault(c => c.Campo.Equals(CampiRicercaEnum.Denominazione))?.Valore;


                if (!string.IsNullOrEmpty(email))
                    cors = await this.GetAooAndUooByEmail(email);

                if (!string.IsNullOrEmpty(codice))
                    cors = await GetUoAooByCodice(codice);

                if (!string.IsNullOrEmpty(codiceFiscale))
                    cors = await this.GetUoByCodiceFiscale(codiceFiscale);

                if (!string.IsNullOrEmpty(partitaIva))
                    cors = await this.GetUoByCodiceFiscale(partitaIva);

                if (!string.IsNullOrEmpty(descrizione))
                    cors =  await this.GetListEntiByDescrizione(descrizione);

            }
            catch ( Exception ex )
            {
                this._logger.LogError(exception:ex,message: ex.Message);
            }
            response = new()
            {
                Corrispondenti = cors,
                TotaleCorrispondenti = cors.Count,
                TotalePagine = 1
            };

            return response;
        }


        private async Task<List<Corrispondente>> GetUoByCodiceFiscale(string codiceFiscale)
        {
            EnteResponse? ufficioResponse = new();
            HttpResponseMessage response = null;
            List<Corrispondente> cors = new();

            Dictionary<string, string> parameters = this.GetBaseParameters();
            parameters.Add("CF", codiceFiscale);

            var content = new FormUrlEncodedContent(parameters);

            try
            {
                response = await this._httpClient.PostAsync(Resource.UriGetUoByCodFis, content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResult = await response.Content.ReadAsStringAsync();
                    ufficioResponse = JsonConvert.DeserializeObject<EnteResponse>(jsonResult);
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }
            finally
            {
                content?.Dispose();
                response?.Dispose();
            }

            if (ufficioResponse != null && ufficioResponse.data != null && ufficioResponse.data.Count > 0)
                foreach (var data in ufficioResponse.data)
                {
                    foreach(var uo in data.OU)
                    {
                        cors.Add(this.BuildCorrFromEnte(await this.GetUoByCodiceUnivocoUo(uo.CodUniOu)));
                    }
                }
            return cors;
        }


        private async Task<List<Corrispondente>> GetListEntiByDescrizione(string descrizione)
        {
            EnteResponse? enteResponse = null;
            FormUrlEncodedContent content = null;
            HttpResponseMessage response = null;
            Dictionary<string, string> parameters = this.GetBaseParameters();
            List<Corrispondente> corrs = new();
            ListAooResponse datiAooResponse = new();

            try
            {
                parameters.Add("DESCR", descrizione);
                content = new FormUrlEncodedContent(parameters);
                response = await this._httpClient.PostAsync(Resource.UriWsListEntiByDesc, content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResult = await response.Content.ReadAsStringAsync();
                    enteResponse = JsonConvert.DeserializeObject<EnteResponse>(jsonResult);
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }
            finally
            {
                content?.Dispose();
                response?.Dispose();
            }

            if (enteResponse != null && enteResponse.data != null && enteResponse.data.Count > 0)
            {
                foreach(var d in enteResponse.data)
                {
                    var cors = await this.GetListAooByCodiceAmm(d.CodAmm);
                    corrs.AddRange(cors);
                }
            }
            return corrs;
        }


        private async Task<List<Corrispondente>> GetAooAndUooByEmail(string email)
        {
            List<Corrispondente> corrispondenti = new ();

            EnteResponse resp = await this.GetEnteByEmail(email);
            if (resp != null && resp.data != null && resp.data.Count > 0)
            {
                foreach(var ent in resp.data)
                {
                    switch (ent.TipoEntita)
                    {
                        case "UO":
                            corrispondenti.Add(this.BuildCorrFromEnte(await GetUoByCodiceUnivocoUo(ent.CodEntita)));
                            break;
                        case "AOO":
                        default:
                            corrispondenti.Add(await this.GetAooByCodiceAmm(ent.CodAmm,ent.CodEntita));
                            break;
                    }
                };
            }
            return corrispondenti;

        }

        private Dictionary<string,string> GetBaseParameters()
        {
            Dictionary<string,string> par = new()
            {
                { "AUTH_ID", this._authId}
            };

            return par;
        }

        private async Task<Corrispondente> GetAooByCodiceAmm(string codiceAmm, string codiceAOO)
        {
            EnteResponse? enteResponse = null;
            FormUrlEncodedContent content = null;
            HttpResponseMessage response = null;
            Dictionary<string, string> parameters = this.GetBaseParameters();
            Corrispondente corr = new();
            ListAooResponse datiAooResponse = new();

            try
            {
                parameters.Add("COD_AMM" , codiceAmm );
                content = new FormUrlEncodedContent(parameters);
                response = await this._httpClient.PostAsync(Resource.UriGetAooByCodAmm, content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResult = await response.Content.ReadAsStringAsync();
                    var jsonParse = JObject.Parse(jsonResult);

                    if (Convert.ToInt32(jsonParse["result"]["num_items"].ToString()) > 1)
                    {
                        datiAooResponse = JsonConvert.DeserializeObject<ListAooResponse>(jsonResult);
                    }
                    else
                    {
                        var aoo = JsonConvert.DeserializeObject<AooResponse>(jsonResult);
                        datiAooResponse.data = new List<AOO>() { aoo.data };
                    }
                    AOO ao = (from a in datiAooResponse.data where a.CodAoo.Equals(codiceAOO) && a.CodAmm.Equals(codiceAmm) select a).FirstOrDefault();
                    corr = this.BuildCorrFromAoo(ao);
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }
            finally
            {
                content?.Dispose();
                response?.Dispose();
            }

            return corr;
        }



        private async Task<EnteResponse> GetEnteByEmail(string email)
        {
            EnteResponse? enteResponse = null;
            FormUrlEncodedContent content = null;
            HttpResponseMessage response = null;
            Dictionary<string, string> parameters = this.GetBaseParameters();
            try
            {
                parameters.Add("EMAIL", email);
                content = new FormUrlEncodedContent(parameters);
                response = await this._httpClient.PostAsync(Resource.UriGetEnteByEmail, content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResult = await response.Content.ReadAsStringAsync();
                    enteResponse = JsonConvert.DeserializeObject<EnteResponse>(jsonResult);
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception:ex,message:ex.Message);
            }
            finally
            {
                content?.Dispose();
                response?.Dispose();
            }
            return enteResponse;
        }

        private async Task<UnitaOrganizzativa> GetUoByCodiceUnivocoUo(string codice)
        {
            CodiceUnivocoUOResponse? uo = new();
            HttpResponseMessage response = null;

            Dictionary<string, string> parameters = this.GetBaseParameters();
            parameters.Add("COD_UNI_OU", codice);

            var content = new FormUrlEncodedContent(parameters);

            try
            {
                response = await this._httpClient.PostAsync(Resource.UriGetUoByCod, content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResult = await response.Content.ReadAsStringAsync();
                    uo = JsonConvert.DeserializeObject<CodiceUnivocoUOResponse>(jsonResult);
                }
            }
            catch(Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }
            finally
            {
                content?.Dispose();
                response?.Dispose();
            }
            

            return uo?.data;
        }

        private async Task<AOO> GetAooByCodiceUnivocoAoo(string codice)
        {
            AooResponse? aoo = new();
            HttpResponseMessage response = null;

            Dictionary<string, string> parameters = this.GetBaseParameters();
            parameters.Add("COD_UNI_AOO", codice);

            var content = new FormUrlEncodedContent(parameters);

            try
            {
                response = await this._httpClient.PostAsync(Resource.UriGetAooByCod, content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResult = await response.Content.ReadAsStringAsync();
                    aoo = JsonConvert.DeserializeObject<AooResponse>(jsonResult);
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }
            finally
            {
                content?.Dispose();
                response?.Dispose();
            }


            return aoo?.data;
        }


        private async Task<List<Corrispondente>> GetListAooByCodiceAmm(string codiceAmm)
        {
            AooResponse? aoo = new();
            HttpResponseMessage response = null;
            ListAooResponse datiAooResponse = null;
            List<Corrispondente> corr = new();

            Dictionary<string, string> parameters = this.GetBaseParameters();
            parameters.Add("COD_AMM", codiceAmm);

            var content = new FormUrlEncodedContent(parameters);

            try
            {
                response = await this._httpClient.PostAsync(Resource.UriGetListAooByCodAmm, content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResult = await response.Content.ReadAsStringAsync();
                    var jsonParse = JObject.Parse(jsonResult);

                    if (Convert.ToInt32(jsonParse["result"]["num_items"].ToString()) > 1)
                        datiAooResponse = JsonConvert.DeserializeObject<ListAooResponse>(jsonResult);
                    else
                    {
                        aoo = JsonConvert.DeserializeObject<AooResponse>(jsonResult);

                        datiAooResponse = new ListAooResponse();
                        datiAooResponse.data = new List<AOO>()
                        {
                            aoo?.data
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }
            finally
            {
                content?.Dispose();
                response?.Dispose();
            }

            if(datiAooResponse != null)
            {
                datiAooResponse.data.ForEach(a => corr.Add(this.BuildCorrFromAoo(a)));
            }

            return corr;
        }


        private Corrispondente BuildCorrFromEnte(UnitaOrganizzativa entity)
        {
            if (entity == null)
            {
                return null;
            }

            bool amm = false;
            bool aoo = false;
            bool emails = false;
            bool urls = false;
            string canale = "";

            if (!string.IsNullOrEmpty(entity.CodAmm))
                amm = true;

            if (!string.IsNullOrEmpty(entity.CodAoo))
                aoo = true;


            if (!string.IsNullOrEmpty(entity.Mail1))
                emails = true;

            urls = false;

            if (amm && aoo && urls)
                canale = "INTEROPERABILITA PITRE";
            else
                if (amm && aoo && emails && !urls)
                canale = "INTEROPERABILITA";
            else
                    if (emails && !urls && !(amm && aoo))
                canale = "MAIL";
            else
                        if (!emails && !urls && !(amm && aoo))
                canale = "LETTERA";



            var em = new List<Email>();
            if (!string.IsNullOrEmpty(entity.MailResp))
                em.Add(new Email() { Indirizzo = entity.MailResp, Note = string.Empty, Preferita = false });
            if (!string.IsNullOrEmpty(entity.Mail1))
                em.Add(new Email() { Indirizzo = entity.Mail1, Note = string.Empty, Preferita = false });
            if (!string.IsNullOrEmpty(entity.Mail2))
                em.Add(new Email() { Indirizzo = entity.Mail2, Note = string.Empty, Preferita = false });
            if (!string.IsNullOrEmpty(entity.Mail3))
                em.Add(new Email() { Indirizzo = entity.Mail3, Note = string.Empty, Preferita = false });


            var corr = new Corrispondente()
            {
                Id = "0",
                Codice = entity.CodUniOu,
                Denominazione = entity.DesOu,
                CodiceFiscale = !string.IsNullOrEmpty(entity.Cf) ? entity.Cf : string.Empty,
                AOO = entity.CodAoo,
                Amministrazione = entity.CodAmm,
                Indirizzo = entity.Indirizzo,
                CAP = entity.Cap,
                Citta = entity.Comune,
                Fax = !string.IsNullOrEmpty(entity.Fax) ? entity.Fax : string.Empty,
                Nazione = string.Empty,
                Provincia = entity.Provincia,
                Telefono = entity.Tel,
                Tipo = Tipi.UnitaOrganizzativa,
                RubricaEsterna = "IPA",
                Canale = canale,
                Emails = em
            };
            if (!string.IsNullOrEmpty(entity.Mail1))
                corr.Email = entity.Mail1;

            return corr;
        }
        private Corrispondente BuildCorrFromAoo(AOO entity)
        {
            bool amm = false;
            bool aoo = false;
            bool emails = false;
            bool urls = false;
            string canale = "";

            if (entity == null)
            {
                return null;
            }

            if (entity != null && !string.IsNullOrEmpty(entity.CodAmm))
                amm = true;

            if (entity != null && !string.IsNullOrEmpty(entity.CodAoo))
                aoo = true;


            if (!string.IsNullOrEmpty(entity.Mail1))
                emails = true;

            urls = false;

            if (amm && aoo && urls)
                canale = "INTEROPERABILITA PITRE";
            else
                if (amm && aoo && emails && !urls)
                canale = "INTEROPERABILITA";
            else
                    if (emails && !urls && !(amm && aoo))
                canale = "MAIL";
            else
                        if (!emails && !urls && !(amm && aoo))
                canale = "LETTERA";

            var em = new List<Email>();
            if (!string.IsNullOrEmpty(entity.MailResp))
                em.Add(new Email() { Indirizzo = entity.MailResp, Note = string.Empty, Preferita = false });
            if (!string.IsNullOrEmpty(entity.Mail1))
                em.Add(new Email() { Indirizzo = entity.Mail1, Note = string.Empty, Preferita = false });
            if (!string.IsNullOrEmpty(entity.Mail2))
                em.Add(new Email() { Indirizzo = entity.Mail2, Note = string.Empty, Preferita = false });
            if (!string.IsNullOrEmpty(entity.Mail3))
                em.Add(new Email() { Indirizzo = entity.Mail3, Note = string.Empty, Preferita = false });

            var corr = new Corrispondente()
            {
                Id = "0",
                Codice = entity.CodAmm + "-" + entity.CodUniAoo,
                Denominazione = entity.DesAoo,
                AOO = entity.CodUniAoo,
                Amministrazione = entity.CodAmm,
                Indirizzo = entity.Indirizzo,
                CAP = entity.Cap,
                Citta = entity.Comune,
                Fax = !string.IsNullOrEmpty(entity.Fax) ? entity.Fax : string.Empty,
                Nazione = string.Empty,
                Provincia = entity.Provincia,
                Telefono = entity.Tel,
                Tipo = Tipi.UnitaOrganizzativa,
                RubricaEsterna = "IPA",
                Canale = canale,
                Emails = em
            };
            if (!string.IsNullOrEmpty(entity.Mail1))
                corr.Email = entity.Mail1;
            return corr;
        }
        private async Task<List<Corrispondente>> GetUoAooByCodice(string codice)
        {
            List<Corrispondente> elementi = new();
            try
            {
                UnitaOrganizzativa uo = await this.GetUoByCodiceUnivocoUo(codice);
                if (uo != null)
                {
                    elementi.Add(this.BuildCorrFromEnte(uo));
                }
                else
                {
                    string codiceUnivocoAoo = codice;
                    if (codice.Contains("-"))
                        codiceUnivocoAoo = codice.Split('-')[1];

                    var aoo = await this.GetAooByCodiceUnivocoAoo(codiceUnivocoAoo);

                    if (aoo != null)
                    {
                        elementi.Add(this.BuildCorrFromAoo(aoo));
                    }
                    else
                    {
                        elementi = await this.GetListAooByCodiceAmm(codice);
                    }

                }


            }
            catch( Exception ex )
            {
                this._logger.LogError(exception:ex,message:ex.Message);
            }
            return elementi;
        }

        

        protected readonly ILogger<SearchIpaHandler> _logger;
        protected readonly IMediator _mediator;
        protected IMapper _mapper;
        protected HttpClient _httpClient = null;
        protected readonly string _authId;


        protected IMapper InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                Corrispondente.CreateMapFromElementoRubricaEntity(cfg);
            });

            return configuration.CreateMapper();
        }
    }
}
