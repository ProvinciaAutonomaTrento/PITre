// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using DocsPaVO.Modelli_Trasmissioni;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Transmissions.GetModelliPerTrasmLite
{
    
    public class GetModelliPerTrasmLiteCommand : IRequest<GetModelliPerTrasmLiteCommandResponse>
    {
        public string IdAmm { get; set; }
        public DocsPaVO.utente.Registro[] Registri { get; set; }
        public string IdPeople { get; set; }
        public string IdCorrGlobali { get; set; }
        public string IdTipoDoc { get; set; }
        public string IdDiagramma { get; set; }
        public string IdStato { get; set; }
        public string ChaTipoOggetto { get; set; }
        public string SystemId { get; set; }
        public string IdRuoloUtente { get; set; }
        public bool AllReg { get; set; }
        public string accessrights { get; set; }
    }
    public class GetModelliPerTrasmLiteCommandResponse
    {
        public ModelloTrasmissione[] Output { get; set; }
    }
}
