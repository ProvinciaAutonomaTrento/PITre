// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.LibroFirma;
using DocsPaVO.utente;
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.SignBook.GetIstanzaProcessiDiFirmaByFilter
{
    public class GetIstanzaProcessiDiFirmaByFilterCommand : IRequest<GetIstanzaProcessiDiFirmaByFilterCommandResponse>
    {
        public FiltroIstanzeProcessoFirma[] filtro { get; set; }
        public InfoUtente infoUtente { get; set; }
        public int numPage { get; set; }
        public int pageSize { get; set; }
    }
    public class GetIstanzaProcessiDiFirmaByFilterCommandResponse
    {
        public IstanzaProcessoDiFirma[] output { get; set; }
        public int numTotPage { get; set; }
        public int nRec { get; set; }
        public System.Data.DataSet istanzeProcessi { get; set; }
    }
}
