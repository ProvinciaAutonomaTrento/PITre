// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.documento;
using DocsPaVO.fascicolazione;
using DocsPaVO.LibroFirma;
using DocsPaVO.utente;
using DocsPaVO.Validations;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Requests
{ 
    public record FascicolazioneGetFascicoliDaDocNoSecurityResult(Fascicolo[] output);

    public record FascicolazioneGetFascicoliDaDocNoSecurity(InfoUtente infoUtente, string idProfile) : IRequest<FascicolazioneGetFascicoliDaDocNoSecurityResult>;
    public record FascicolazioneGetFascicoliDaDocResult(Fascicolo[] output);

    public record FascicolazioneGetFascicoliDaDoc(InfoUtente infoUtente, string idProfile) : IRequest<FascicolazioneGetFascicoliDaDocResult>;
    
    public record GetIstanzaPassoFirmaWithSegnaturaPermanenteResult(IstanzaPassoDiFirma output);

    public record GetIstanzaPassoFirmaWithSegnaturaPermanente(string docnumber) : IRequest<GetIstanzaPassoFirmaWithSegnaturaPermanenteResult>;

    public record CambiaFascicolazionePrimariaResult(bool output);

    public record CambiaFascicolazionePrimaria(InfoUtente infoUtente, string idProject, string idProfile) : IRequest<CambiaFascicolazionePrimariaResult>;
    public record FascicolazioneGetFoldersDocumentResult(Folder[] output);

    public record FascicolazioneGetFoldersDocument(string systemIdDocumento) : IRequest<FascicolazioneGetFoldersDocumentResult>;

    public record FascicolazioneGetFascicoloInClassificaResult(Fascicolo output);

    public record FascicolazioneGetFoldersDocumentFascicolo(string systemIdDocumento, string systemIdFascicolo) : IRequest<FascicolazioneGetFoldersDocumentFascicoloResult>;
    public record FascicolazioneGetFoldersDocumentFascicoloResult(Folder[] output);

    public record FascicolazioneGetFascicoloInClassifica(InfoUtente infoUtente, string codiceFascicolo, string idRegistro, bool enableUffRef, string idTitolario, bool enableProfilazione, string systemId) : IRequest<FascicolazioneGetFascicoloInClassificaResult>;
    public record FascicolazioneGetFolderResult(Folder output);

    public record FascicolazioneGetFolder(string idPeople, string idGruppo, Fascicolo fascicolo) : IRequest<FascicolazioneGetFolderResult>;
    public record GetFascicolazioneTipiDocumentoResult(FascicolazioneTipiDocumento[] output);

    public record GetFascicolazioneTipiDocumento(string idAmm, InfoUtente infoutente) : IRequest<GetFascicolazioneTipiDocumentoResult>;

    public record FascicolazioneAddDocFolderResult(bool output, string msg);

    public record FascicolazioneAddDocFolder(InfoUtente infoutente, string idProfile, Folder Folder, string descrFasc) : IRequest<FascicolazioneAddDocFolderResult>;
    public record FascicolazioneDeleteDocFromProjectResult(ValidationResultInfo output, string msg);
    public record FascicolazioneDeleteDocFromProject(InfoUtente infoUtente, string idProfile, DocsPaVO.fascicolazione.Folder folder, string fascRapida, Fascicolo fasc, string msg) : IRequest<FascicolazioneDeleteDocFromProjectResult>;                                                
}