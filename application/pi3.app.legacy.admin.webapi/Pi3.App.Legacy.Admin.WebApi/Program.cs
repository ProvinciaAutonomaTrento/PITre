// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaUtils.Logging;
using DocsPaVO.Settings;
using DocsPaWS;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Pi3.App.Legacy.Admin.WebApi;
using Pi3.App.Legacy.Admin.WebApi.Infrastructure.Services.OracleDbContextFactory;
using Pi3.Core.Services.File.ReportGenerator;
using Pi3.Core.Services.File.Spreadsheet;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.DocumentFormat.OpenXml.Services.ReportGenerator;
using Pi3.Infrastructure.DocumentFormat.OpenXml.Services.Spreadsheet;
using Pi3.Infrastructure.Legacy.EF;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Services.AAC;
using Refit;
using Pi3.App.Legacy.Admin.WebApi.Infrastructure.Services.RubricaComune;
using Serilog;
using SoapCore;
using SoapCore.ServiceModel;
using System.Configuration;
using System.Globalization;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Elastic.Apm.NetCoreAll;
using Pi3.Core.Services.File.Converters;
using Pi3.App.Legacy.Admin.WebApi.Infrastructure.Services.MockFileConverter;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
builder.Logging.AddSerilog();

var elasticApmEnabled = builder.Configuration.GetValue<bool>("ElasticApmEnabled");

if (elasticApmEnabled)
    builder.Services.AddAllElasticApm();

builder.Services.AddHealthChecks()
    .AddCheck("BaseHealthCheck", () => HealthCheckResult.Healthy());

var parametri = new AppSettings();
builder.Configuration.GetSection("Parametri").Bind(parametri);
AppSettings.Instance = parametri;

builder.Services.RemoveAll<ISpreadsheetService>().AddSingleton<ISpreadsheetService, OpenXmlShreadsheetService>();
builder.Services.RemoveAll<IReportGeneratorService>().AddSingleton<IReportGeneratorService, OpenXmlReportGeneratorService>();

builder.Services.AddScoped<IFileConverterService, MockFileConverterService>();

//// Aggiunta servizio per Token AAC
builder.Services.AddInfrastructureAAC(builder.Configuration.GetSection("InfrastructureAACOptions:UrlJWK").Value);

builder.Services.AddRefitClient<IRubricaComuneService>().ConfigureHttpClient(c => c.BaseAddress = new Uri(builder.Configuration.GetSection("AddressBookOptions:Url").Value));

//builder.Services.AddReverseProxy()
//    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
builder.Services.AddSoapCore();
builder.Services.TryAddSingleton<IDocsPaWS, DocsPaWS.DocsPaWS>();
builder.Services.AddCustomSoapMessageSerializer<CustomSerializer>();
builder.Services.AddMvc();

Chilkat.Global global = new Chilkat.Global();
var serialKey = builder.Configuration["Parametri:KilKatSerialKey"];
var unlock = global.UnlockBundle(serialKey);
//Console.WriteLine("unlock: " + unlock);
//global.LastErrorText

var _configuration = builder.Configuration;

AppSettings.Instance.ConnectionStrings = _configuration.GetConnectionStrings();

//foreach (var kvp in connectionStrings)
//{
//    Console.WriteLine($"Name: {kvp.Key}, Connection String: {kvp.Value}");
//}

builder.Services.AddSoapMessageProcessor(async (soapMsg, httpcontext, next) =>
{
    var bufferedMessage = soapMsg.CreateBufferedCopy(int.MaxValue);
    var msg = bufferedMessage.CreateMessage();
    //var reader = msg.GetReaderAtBodyContents();
    //var content = reader.ReadInnerXml();

    //now you can inspect and modify the content at will.
    //if you want to pass on the original message, use bufferedMessage.CreateMessage(); otherwise use one of the overloads of Message.CreateMessage() to create a new message
    var message = bufferedMessage.CreateMessage();

    if (msg.Headers.Count() > 0)
    {

        var msgBody = msg.Headers[0].ToString();
        XDocument xDoc = XDocument.Parse(msgBody);
        XNamespace ns = "http://localhost";

        var instanceName = xDoc.Descendants(ns + "InstanceName").FirstOrDefault()?.Value;

        if (instanceName != null)
        {
            Console.WriteLine("InstanceName: " + instanceName);
            HeaderValue.Instance.ConnectionString.Value = _configuration.GetConnectionString(instanceName);
            HeaderValue.Instance.ConnectionName.Value = instanceName;
        }
        else
        {
            Console.WriteLine("InstanceName not found.");
        }
        if (msg.Headers.Count() > 1)
        {

            var tokenBody = msg.Headers[1].ToString();
            XDocument xToken = XDocument.Parse(tokenBody);
            var addressBookAuthToken = xToken.Descendants(ns + "AddressBookAuthToken").FirstOrDefault()?.Value;

            if (addressBookAuthToken != null)
            {
                HeaderValue.AddressBookAuthToken.Token.Value = addressBookAuthToken;
            }
        }


    }
    else
    {
        HeaderValue.Instance.ConnectionString.Value = _configuration.GetConnectionString("PAT_INSTANCE");
    }


    //pass the modified message on to the rest of the pipe.
    var responseMessage = await next(message);
    ////Inspect and modify the contents of returnMessage in the same way as the incoming message.
    ////finish by returning the modified message.
    //var urls = new List<string> { "\"http://localhost/getTemplates\"",
    //            "\"http://localhost/GetAmmRightMailRegistro\"",
    //            "\"http://localhost/getTemplatesFasc\"",
    //            "\"http://localhost/getModelliByDdlAmmPaging\"",
    //            "\"http://localhost/getListeDistribuzioneAmm\""
    //            };


    //var soapActionHeader = httpcontext.Request.Headers["SOAPAction"].FirstOrDefault();
    //if (urls.Contains(soapActionHeader))
    //{
    //    var reader = responseMessage.GetReaderAtBodyContents();

    //    var content = await reader.ReadOuterXmlAsync();

    //    // Prepara un MemoryStream e un XmlWriter per scrivere il nuovo contenuto XML.
    //    var ms = new MemoryStream();
    //    var settings = new XmlWriterSettings { Encoding = Encoding.UTF8 };
    //    using (var writer = XmlWriter.Create(ms, settings))
    //    {
    //        // Scrivi l'elemento radice e dichiara gli spazi dei nomi necessari.
    //        writer.WriteStartElement("s", "Envelope", "http://schemas.xmlsoap.org/soap/envelope/");
    //        writer.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
    //        //writer.WriteAttributeString("xmlns", "s", null, "http://schemas.xmlsoap.org/soap/envelope/");
    //        writer.WriteAttributeString("xmlns", "xsd", null, "http://www.w3.org/2001/XMLSchema");
    //        // Qui puoi copiare altri spazi dei nomi dal messaggio originale se necessario.

    //        // Modifica il contenuto come necessario.
    //        var modifiedContent = soapActionHeader switch
    //        {
    //            "\"http://localhost/getTemplates\"" => 
    //                content.Replace("<Templates>", "<anyType xsi:type=\"Templates\">").Replace("</Templates>", "</anyType>"),
    //            // Aggiungi altri casi qui se necessario
    //            _ => content // Restituisce il contenuto originale per default se nessun caso corrisponde
    //        };

    //        XmlReaderSettings readerSettings = new XmlReaderSettings
    //        {
    //            ValidationType = ValidationType.None
    //        };
    //        // Utilizza un XmlReader per leggere il contenuto modificato.
    //        using (var contentReader = XmlReader.Create(new StringReader(modifiedContent), readerSettings))
    //        {
    //            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(contentReader.NameTable);
    //            namespaceManager.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
    //            namespaceManager.AddNamespace("xsd", "http://www.w3.org/2001/XMLSchema");
    //            // Copia il contenuto nel nuovo documento XML.
    //            while (contentReader.Read())
    //            {
    //             //   writer.WriteNode(contentReader, true);
    //            }
    //        }

    //        writer.WriteEndElement(); // Chiude l'elemento radice.
    //    }

    //    // Crea un nuovo XmlReader dal MemoryStream per il nuovo messaggio SOAP.
    //    ms.Position = 0; // Resetta la posizione del MemoryStream per la lettura.
    //    var xmlReader = XmlReader.Create(ms);

    //    // Crea e restituisce il nuovo messaggio SOAP.
    //    var finalMessage = Message.CreateMessage(responseMessage.Version, null, xmlReader);
    //    return finalMessage;


    //    //var ms = soapActionHeader switch
    //    //{
    //    //    "\"http://localhost/getTemplates\"" => new MemoryStream(Encoding.UTF8.GetBytes(
    //    //        content.Replace("<Templates>", "<anyType xsi:type=\"Templates\">").Replace("</Templates>", "</anyType>"))),
    //    //    // Aggiungi altri casi qui se necessario
    //    //    _ => new MemoryStream(Encoding.UTF8.GetBytes(content)) // Restituisce il contenuto originale per default se nessun caso corrisponde
    //    //};
    //    //var xmlReader = XmlReader.Create(ms);

    //    //var resultMessage = Message.CreateMessage(responseMessage.Version, null, xmlReader);

    //    //XmlDictionaryString namespaces;
    //    //reader.TryGetNamespaceUriAsDictionaryString(out namespaces);
    //    ////resultMessage.WriteStartEnvelope(namespaces);
    //    //return resultMessage;
    //}

    return responseMessage;
});

static string ModifySoapContent(string originalContent, string action)
{
    if (action == "http://localhost/getTemplates")
    {
        string modifiedContent = originalContent.Replace("<Templates>", "<anyType xsi:type=\"Templates\">");
        modifiedContent = modifiedContent.Replace("</Templates>", "</anyType>");
        return modifiedContent;
    }
    else if (action == "http://localhost/GetAmmRightMailRegistro")
    {
        string modifiedContent = originalContent.Replace("<DataSet>", "<GetAmmRightMailRegistroResult>");
        modifiedContent = modifiedContent.Replace("</DataSet>", "</GetAmmRightMailRegistroResult>");
        return modifiedContent;
    }
    else if (action == "http://localhost/getTemplatesFasc")
    {
        string modifiedContent = originalContent.Replace("<Templates>", "<anyType xsi:type=\"Templates\">");
        modifiedContent = modifiedContent.Replace("</Templates>", "</anyType>");
        return modifiedContent;
    }
    else if (action == "http://localhost/getModelliByDdlAmmPaging")
    {
        string modifiedContent = originalContent;
        while (modifiedContent.Contains("<ModelloTrasmissione>"))
        {
            modifiedContent = modifiedContent.Replace("<ModelloTrasmissione>", "<anyType xsi:type=\"ModelloTrasmissione\">");
            modifiedContent = modifiedContent.Replace("</ModelloTrasmissione>", "</anyType>");
        }
        return modifiedContent;
    }
    else if (action == "http://localhost/getListeDistribuzioneAmm")
    {
        string modifiedContent = originalContent.Replace("<DataSet>", "<getListeDistribuzioneAmmResult>");
        modifiedContent = modifiedContent.Replace("</DataSet>", "</getListeDistribuzioneAmmResult>");
        return modifiedContent;
    }
    else
        return originalContent;
}//XmlConfigurator.Configure(new FileInfo("log4net.config"));


// Registrazione di IHttpContextAccessor
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IClaimsPrincipalService, ClaimsPrincipalService>();
builder.Services.AddInfrastructureLegacyEFServices();

// Registra Infrastructure per accesso ad Oracle tramite EF
builder.Services.Configure<OracleDbContextFactoryServiceOptions>(builder.Configuration.GetSection(key: nameof(OracleDbContextFactoryServiceOptions)));
builder.Services.AddScoped<IInstanceProvider, ClaimsPrincipalScopedInstanceProvider>();
builder.Services.AddScoped<IOracleDbContextFactoryService, OracleDbContextFactoryService>();
builder.Services.AddScoped<IPi3DbContext>(provider => provider.GetService<IOracleDbContextFactoryService>().CreateDbContext());
builder.Services.AddScoped<IPi3DbContextEntities>(provider => provider.GetService<IOracleDbContextFactoryService>().CreateDbContext());
builder.Services.AddScoped<IPi3DbContextFunctions>(provider => provider.GetService<IOracleDbContextFactoryService>().CreateDbContext());


var app = builder.Build();

if (elasticApmEnabled)
    app.UseAllElasticApm(builder.Configuration);

//var supportedCultures = new[] { "en-US", "it-IT" };
//var localizationOptions = new RequestLocalizationOptions
//{
//    DefaultRequestCulture = new RequestCulture("it-IT"),
//    SupportedCultures = supportedCultures.Select(c => new CultureInfo(c)).ToList(),
//    SupportedUICultures = supportedCultures.Select(c => new CultureInfo(c)).ToList()
//};

//app.UseRequestLocalization(localizationOptions);

//app.MapReverseProxy();
app.MapGet("/", () => "Hello World!");
app.MapGet("/Test/{id:int}", (HttpContext context, IWebHostEnvironment webHostEnvironment, int id) =>
{
    var logger = Serilog.Log.ForContext(typeof(DocsPaWS.DocsPaWS));
    var errorMethod = string.Empty;
    try
    {
        switch (id)
        {
            case 1:
                errorMethod = "GetRfByIdAmm"; // dati Ok

                DocsPaVO.Settings.AppSettings.Instance.WebRoot = webHostEnvironment.ContentRootPath;
                //config = BusinessLogic.Documenti.CacheFileManager.GetInstance("361");

                var result = BusinessLogic.Amministrazione.RegistroManager.GetRfByIdAmm(361, "1");

                return Results.Json(result);
            case 2:
                errorMethod = "DO_GetIdAmmByCodice";

                ProspettiRiepilogativi.Model objM = new ProspettiRiepilogativi.Model();
                var result2 = objM.DO_GetIdAmmByCodice("361");
                return Results.Ok(result2);
            case 3:
                errorMethod = "GetPolicyById";
                var result3 = BusinessLogic.Conservazione.Policy.PolicyManager.GetPolicyById("1");

                return Results.Ok(result3);
            case 4:
                errorMethod = "GetIdRuoloRespConservazione"; // dati Ok
                var manager = new BusinessLogic.Conservazione.ConservazioneManager();
                var result4 = manager.GetIdRuoloRespConservazione("361", string.Empty);

                return Results.Ok(result4);
            case 5:
                errorMethod = "GetIdUtenteRespConservazione"; // dati Ok
                BusinessLogic.Conservazione.ConservazioneManager manager2 = new BusinessLogic.Conservazione.ConservazioneManager();
                var result5 = manager2.GetIdUtenteRespConservazione("361", string.Empty);
                return Results.Ok(result5);
            case 6:
                errorMethod = "GetStatoAttivazione"; // dati Ok
                var cons = new BusinessLogic.Conservazione.ConservazioneManager();
                var result6 = cons.GetStatoAttivazione("361");
                return Results.Ok(result6);
            case 7:
                var result7 = BusinessLogic.Conservazione.PARER.PolicyPARERManager.getListaPolicy("361", string.Empty);
                return Results.Ok(result7);
            case 8:
                var result8 = BusinessLogic.Amministrazione.RegistroManager.GetMailRegistro("86107");
                return Results.Ok(result8);

        }
    }
    catch (Exception e)
    {
        logger.Debug($"errore nel web method {errorMethod} errore: " + e.Message);
    }

    return Results.NotFound();
});

Logger.LoggingEnabled = true;
Logger.SetSize(1500);
Logger.SetLongRunning(TimeSpan.FromSeconds(10));
app.MapGet("/LogReport", (HttpContext context) => {
    var orderBy = context.Request.Query["orderBy"].ToString();
    var onlyLong = context.Request.Query["onlyLong"].ToString();
    var logs = Logger.GetLogs();

    var result = onlyLong == "true" ? logs.Where(itm => itm.LongRunning) : logs;

    if (orderBy == "durata")
    {
        return Results.Json(result.OrderByDescending(itm => itm.Durata));
    }
    else
    {
        return Results.Json(result.OrderByDescending(itm => itm.Fine));
    }
});
app.MapGet("/LogClear", () => Logger.ClearLogs());


app.UseRouting();

//app.UseMiddleware<ReadHeaderMiddleware>();

const string healthCheckPattern = "/healthz";
const string readinessCheckPattern = "/readiness";
const string startupCheckPattern = "/startup";

app.MapHealthChecks(healthCheckPattern);
app.MapHealthChecks(readinessCheckPattern);
app.MapHealthChecks(startupCheckPattern);

//app.UseWhen(
//    context => !context.Request.Path.StartsWithSegments(healthCheckPattern)
//    && !context.Request.Path.StartsWithSegments(readinessCheckPattern)
//    && !context.Request.Path.StartsWithSegments(startupCheckPattern)
//    && !context.Request.Path.StartsWithSegments(metricsPattern),
//    builder =>
//        builder.UseMiddleware<ClaimsPrincipalActivatorMiddleware>());


app.UseEndpoints(endpoints => {
    endpoints.UseSoapEndpoint<IDocsPaWS>("/DocsPaWS.svc", new SoapEncoderOptions(), SoapSerializer.DataContractSerializer);
    //endpoints.UseSoapEndpoint<IDocsPaWS>("/DocsPaWS.asmx", new SoapEncoderOptions { MessageVersion = MessageVersion.Soap12 }, SoapSerializer.XmlSerializer);
    endpoints.UseSoapEndpoint<IDocsPaWS>("/DocsPaWS.asmx", new SoapEncoderOptions(), SoapSerializer.XmlSerializer);
});

app.Run();

