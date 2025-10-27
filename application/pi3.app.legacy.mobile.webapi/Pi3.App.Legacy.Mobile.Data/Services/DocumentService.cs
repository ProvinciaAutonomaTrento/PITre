// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using Pi3.Core.Services.Principal;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;

using Pi3.App.Legacy.Mobile.Shared.Extensions;
using Pi3.App.Legacy.Mobile.Data.Repositories.Interfaces;
using Pi3.App.Legacy.Mobile.Data.Services.Interfaces;

using Microsoft.Extensions.Logging;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Microsoft.EntityFrameworkCore;
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.Services.File.CAdES;
using Pi3.Core.Services.File.PAdES;
using System.Xml;

using SERVICE_REQUEST = Pi3.App.Legacy.Mobile.Models.ServiceRequests;
using SERVICE_DTO = Pi3.App.Legacy.Mobile.Models.ServiceDtos;
using SELECT_TEMPLATES = Pi3.App.Legacy.Mobile.Models.SelectTemplates;
using EXCEPTIONS = Pi3.App.Legacy.Mobile.Shared.Exceptions;

namespace Pi3.App.Legacy.Mobile.Data.Services;

/**
 * 
 * ATTENZIONE !!!
 * Codice non ottimizzato
 * Riportato da Pi3.App.Legacy.WebApi
 *
*/

public class DocumentService(
    ILogger<DocumentService> logger,
    IMapper mapper,
    IClaimsPrincipalService claimsPrincipalService,
    IProfileRepository profileRepository,
    ISecurityRepository securityRepository,
    INoteRepository noteRepository,
    IProjectRepository projectRepository,
    IPi3DbContext pi3DbContext,
    IDocumentBlobRepository documentBlobRepository,
    IPAdESService pAdESService,
    ICAdESService cAdESService ) : IDocumentService
{
    private readonly ILogger<DocumentService> _logger = logger;
    private readonly IMapper _mapper = mapper;
    private readonly IClaimsPrincipalService _claimsPrincipalService = claimsPrincipalService;
    private readonly IProfileRepository _profileRepository = profileRepository;
    private readonly ISecurityRepository _securityRepository = securityRepository;
    private readonly INoteRepository _noteRepository = noteRepository;
    private readonly IProjectRepository _projectRepository = projectRepository;
    private readonly IPi3DbContext _dbContext = pi3DbContext;
    private readonly IDocumentBlobRepository _documentBlobRepository = documentBlobRepository;
    private readonly IPAdESService _pAdESService = pAdESService;
    private readonly ICAdESService _cAdESService = cAdESService;

    public async Task<SERVICE_DTO.Documento> GetDocumentAsync(
        SERVICE_REQUEST.GetDocumentRequest request,
        CancellationToken cancellationToken )
    {
        long idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);
        long idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, true);
        long idCorrGlobali = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypesExtended.IdCorrGlobali, true);

        IList<long?> thingSecurityCheck = [idPeople, idGroup];
        if ( request.IdRuoloPubblico.HasValue )
        {
            thingSecurityCheck.Add(request.IdRuoloPubblico.Value);
        }

        long accessRight = await this._securityRepository.GetAccessRigthByIdObjectAsync(
            request.IdDocumento,
            thingSecurityCheck,
            cancellationToken);

        if ( accessRight == 0 )
        {
            throw new EXCEPTIONS.UnauthorizedException();
        }

        bool isPubblico = accessRight > 0;

        SELECT_TEMPLATES.DettagliDocumento? documento = await this._profileRepository
                .GetDocumentoUltimaVersioneByDocNumberAsync(request.IdDocumento, cancellationToken)
                    ?? throw new EXCEPTIONS.UnexpectedException("Non e' stato possibile recuperare i dettagli del documento dal DB");
        documento.AccessRight = accessRight;

        SERVICE_DTO.Documento result = this._mapper.Map<SERVICE_DTO.Documento>(documento);

        List<SELECT_TEMPLATES.DettagliProtocollo> dettagliProtocollo = [];
        switch(documento.TipoProto?.ToUpper())
        {
            case "A":
                SELECT_TEMPLATES.DettagliProtocollo pA = await this._profileRepository.GetDettagliProtocolloInArrivoAsync(documento.SystemId, cancellationToken)
                    ?? throw new EXCEPTIONS.ExpectedResultNotFoundException("Recupero dettagli protocollo in Arrivo");
                dettagliProtocollo.Add(pA);
                result.IsProtocollato = String.IsNullOrEmpty(documento.DaProtocollare) || documento.DaProtocollare.ToUpper().Equals("0");
                break;
            case "P":
                IEnumerable<SELECT_TEMPLATES.DettagliProtocollo> pP = await this._profileRepository.GetDettagliProtocolloInUscitaAsync(documento.SystemId, cancellationToken)
                    ?? throw new EXCEPTIONS.ExpectedResultNotFoundException("Recupero dettagli protocollo in Uscita");
                dettagliProtocollo.AddRange(pP);
                result.IsProtocollato = String.IsNullOrEmpty(documento.DaProtocollare) || documento.DaProtocollare.ToUpper().Equals("0");
                break;
            case "I":
                result.IsProtocollato = String.IsNullOrEmpty(documento.DaProtocollare) || documento.DaProtocollare.ToUpper().Equals("0");
                break;
        }

        foreach ( var item in dettagliProtocollo )
        {
            if ( item.Tipo?.Equals("M") ?? false )
            {
                result.Mittente = item.TipoUrp switch
                {
                    "P" => $"{item.Cognome} {item.Nome}",
                    _ => item.Descrizione
                } ?? item.Descrizione;
                break;
            }
            if ( item.Tipo?.Equals("D") ?? false )
            {
                result.Destinatari ??= [];
                string d = item.TipoUrp switch
                {
                    "P" => $"{item.Cognome} {item.Nome}",
                    _ => item.Descrizione
                } ?? item.Descrizione ?? String.Empty;
                result.Destinatari.Add(d);
            }
        }

        SELECT_TEMPLATES.Nota? ultimaNota = await this._noteRepository.GetUltimaNota(
            request.IdDocumento,
            idPeople,
            idGroup,
            idCorrGlobali, cancellationToken);
        result.Note = ultimaNota?.Testo;

        IEnumerable<SELECT_TEMPLATES.Project> fascicoli 
            = await this._projectRepository.GetProjectByIdComponentWithSecurityAsync(
                documento.SystemId,
                thingSecurityCheck,
                cancellationToken);

        result.Fascicoli = fascicoli
            .Select(s => new List<string> {
                s.Codice ?? String.Empty,
                s.Description ?? String.Empty, 
                s.SystemId.ToString(), 
                s.TipoFascicolo ?? String.Empty 
            })
            .ToList();

        return result;
    }

    public async Task<IEnumerable<SERVICE_DTO.Documento>> GetDocumentAttachmentsAsync(
        SERVICE_REQUEST.GetDocumentRequest request, CancellationToken cancellationToken )
    {
        long idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);
        long idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, true);

        IList<long?> thingSecurityCheck = [idPeople, idGroup];
        if ( request.IdRuoloPubblico.HasValue )
        {
            thingSecurityCheck.Add(request.IdRuoloPubblico.Value);
        }
        long accessRight = await this._securityRepository.GetAccessRigthByIdObjectAsync(
            request.IdDocumento,
            thingSecurityCheck,
            cancellationToken);

        if ( accessRight == 0 )
        {
            throw new EXCEPTIONS.UnauthorizedException();
        }

        IEnumerable<SELECT_TEMPLATES.DettagliDocumento> attachments = await this._profileRepository
                .GetAllegatiDocumentoUltimaVersioneByIdDocumentoPrincipaleAsync(request.IdDocumento, cancellationToken);
        foreach ( SELECT_TEMPLATES.DettagliDocumento item in attachments )
        {
            item.AccessRight = accessRight;
        }

        IEnumerable<SERVICE_DTO.Documento> result = this._mapper.Map<IEnumerable<SERVICE_DTO.Documento>>(attachments);
        return result;
    }


    public async Task<SERVICE_DTO.FileInfo> GetFileByIdDocument(long idDocumento, CancellationToken cancellationToken = default)
    {
        var idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
        SERVICE_DTO.FileInfo file = new SERVICE_DTO.FileInfo();

        var componentsEntity = await this._dbContext.ComponentEntities.AsNoTracking()
            .Where(e => e.DOCNUMBER.Equals(idDocumento))
            .OrderByDescending(e => e.VERSION_ID)
            .Select(c => new
            {
                c.PATH,
                c.VAR_NOMEORIGINALE,
                c.EXT
            })
            .FirstOrDefaultAsync(cancellationToken);

        file.Path = componentsEntity.PATH;
        file.OriginalFileName = componentsEntity.VAR_NOMEORIGINALE;
        file.EstensioneFile = componentsEntity.EXT;
        file.FullName = componentsEntity.VAR_NOMEORIGINALE;
        file.Name = componentsEntity.VAR_NOMEORIGINALE;

        DocumentBlob blob = await this._documentBlobRepository.Get(idTenant, file.Path);
        using ( var memoryStream = new MemoryStream() )
        {
            blob.Stream.CopyTo(memoryStream);
            file.Content = memoryStream.ToArray();
        }
        file.ContentType = blob.ContentType;
        file.Lenght = file.Content.Length;

        if ( (file.FullName.ToUpper().EndsWith("P7M")) || //cades
                   (file.FullName.ToUpper().EndsWith("TSD")) || //timestamp
                   (file.FullName.ToUpper().EndsWith("M7M")) || //timestamp
                   (file.FullName.ToUpper().EndsWith("PDF") && await _pAdESService.IsPAdESFile(new MemoryStream(file.Content))) ||
                   (file.FullName.ToUpper().EndsWith("XML") && await IsSignedXades(file)) ) // XADES
        {
            try
            {
                using ( var msSignedFile = new MemoryStream(file.Content) )
                {
                    using ( var msOriginalFile = new MemoryStream() )
                    {
                        await this._cAdESService.LoadOriginalFile(Path.GetExtension(file.FullName), msSignedFile, msOriginalFile);
                        file.Content = msOriginalFile.ToArray();
                    }
                }
                file.EstensioneFile = GetEstensioneIntoSignedFile(file.OriginalFileName);
                file.Name = Path.GetFileNameWithoutExtension(file.Name);
                file.Lenght = file.Content.Length;
                file.ContentType = await GetMimeType(file.EstensioneFile);
                file.FullName = file.Name;


            }
            catch ( Exception ex )
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }
        }

        return file;
    }

    private async Task<bool> IsSignedXades( SERVICE_DTO.FileInfo fileDoc )
    {
        bool result = false;
        XmlDocument Xmlfile = new XmlDocument();
        XmlTextReader tr = new XmlTextReader(new System.IO.MemoryStream(fileDoc.Content));
        tr.XmlResolver = null;
        try
        {
            Xmlfile.Load(tr);
            XmlNodeList signature = Xmlfile.DocumentElement.GetElementsByTagName("ds:Signature");
            if ( signature != null && signature.Count > 0 )
            {
                result = true;
            }
        }
        catch ( Exception e )
        {
            _logger.LogError("Errore nel metodo IsSignedXades " + e.Message);
            result = false;
        }
        finally
        {
            tr.Close();
        }

        return result;
    }

    private string GetEstensioneIntoSignedFile( string fullname )
    {
        string retValue = string.Empty;

        // Reperimento del nome del file con estensione
        string fileName = new System.IO.FileInfo(fullname).Name;

        string[] items = fileName.Split('.');

        for ( int i = (items.Length - 1); i >= 0; i-- )
        {
            if ( !(items[i].ToUpper().EndsWith("P7M") ||
                items[i].ToUpper().EndsWith("TSD") ||
                items[i].ToUpper().EndsWith("M7M"))
                )
            {
                retValue = items[i];
                break;
            }
        }
        return retValue;
    }

    protected async Task<string> GetMimeType( string ext )
    {
        string mimeType = string.Empty;

        if ( !string.IsNullOrEmpty(ext) )
        {
            var appsEntity = await this._dbContext.AppEntities.AsNoTracking()
                .Where(a => a.DEFAULT_EXTENSION.ToUpper() == ext.ToUpper())
                .Select(a => new
                {
                    a.DEFAULT_EXTENSION,
                    a.MIME_TYPE
                })
                .FirstOrDefaultAsync();
            mimeType = appsEntity != null ? appsEntity.MIME_TYPE : "application/x-" + ext;

        }

        return mimeType;
    }


}
