// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ModificaListaCorr
{
    public class ListaDistribuzioneNotFoundException : NotFoundPi3Exception
    {
        public ListaDistribuzioneNotFoundException(string idLista) 
            : base(ErrorDescriptions.ListaDistribuzioneNonTrovata, null, ErrorDescriptions.ResourceManager, idLista)
        {
            this.IdLista = idLista;
        }

        public string IdLista { get; init; }


    }

    public class TipoCorrispondenteNotSupportedException : NotSupportedPi3Exception
    {
        public TipoCorrispondenteNotSupportedException(string tipo)
            : base(ErrorDescriptions.TipoCorrispondenteNonSupportato, null, ErrorDescriptions.ResourceManager, tipo)
        {
                this.Tipo = tipo;
        }

        public string Tipo { get; init; }
    }
}
