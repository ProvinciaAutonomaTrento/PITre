// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.FascicolazioneGetFolder
{
    public class FascicolazioneGetFolderCommand : IRequest<FascicolazioneGetFolderCommandResponse>
    {
        public string IdPeople{ get; set; }
        public string IdGruppo { get; set; }
        public DocsPaVO.fascicolazione.Fascicolo Fascicolo { get; set; }
    }

    public class FascicolazioneGetFolderCommandResponse
    {
        public DocsPaVO.fascicolazione.Folder Output { get; set; }
    }
}
