// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.TrasmissioneAggregate
{
    public class TrasmissionePi3Exception : Pi3Exception
    {
        #region Public Members

        public TrasmissionePi3Exception(string message, System.Resources.ResourceManager resourceManager, string idTrasmissione)
            : base(message, resourceManager, ErrorDescriptions.ResourceManager, idTrasmissione)
        {
            this.IdTrasmissione = idTrasmissione;
        }

        public string IdTrasmissione { get; init; }

        #endregion
    }


    public class TrasmissioneSingolaPi3Exception : Pi3Exception
    {
        #region Public Members

        public TrasmissioneSingolaPi3Exception(string message, System.Resources.ResourceManager resourceManager, string idTrasmissioneSingola)
            : base(message, resourceManager, ErrorDescriptions.ResourceManager, idTrasmissioneSingola)
        {
            this.IdTrasmissioneSingola = idTrasmissioneSingola;
        }

        public string IdTrasmissioneSingola { get; init; }

        #endregion
    }

    public class TrasmissioneSingolaNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public TrasmissioneSingolaNotFoundPi3Exception(string idTrasmissioneSingola)
            : base(ErrorDescriptions.TrasmissioneSingolaNonTrovata, null, ErrorDescriptions.ResourceManager, idTrasmissioneSingola)
        {
            this.IdTrasmissioneSingola = idTrasmissioneSingola;
        }

        public string IdTrasmissioneSingola { get; init; }

        #endregion
    }

    public class TrasmissioneUtentePi3Exception : Pi3Exception
    {
        #region Public Members

        public TrasmissioneUtentePi3Exception(string message, System.Resources.ResourceManager resourceManager,  string idTrasmissioneUtente)
            : base(message, resourceManager, ErrorDescriptions.ResourceManager, idTrasmissioneUtente)
        {
            this.IdTrasmissioneUtente = idTrasmissioneUtente;
        }

        public string IdTrasmissioneUtente { get; init; }

        #endregion
    }

    public class TrasmissioneUtenteNotFoundPi3Exception : Pi3Exception
    {
        #region Public Members

        public TrasmissioneUtenteNotFoundPi3Exception(string idTrasmissioneUtente)
            : base(ErrorDescriptions.TrasmissioneUtenteNonTrovata, ErrorDescriptions.ResourceManager, idTrasmissioneUtente)
        {
            this.IdTrasmissioneUtente = idTrasmissioneUtente;
        }

        public string IdTrasmissioneUtente { get; init; }

        #endregion
    }

    public class TrasmissioneSingolaNonAssegnataPi3Exception : Pi3Exception
    {
        #region Public Members

        public TrasmissioneSingolaNonAssegnataPi3Exception()
            : base(ErrorDescriptions.TrasmissioneSingolaNonAssegnata, null, ErrorDescriptions.ResourceManager)
        {
        }

        #endregion
    }

    public class TrasmissioneUtenteNonAssegnataPi3Exception : Pi3Exception
    {
        #region Public Members

        public TrasmissioneUtenteNonAssegnataPi3Exception()
            : base(ErrorDescriptions.TrasmissioneUtenteNonAssegnata, null, ErrorDescriptions.ResourceManager)
        {
        }

        #endregion
    }

    public class RagioneTrasmissioneSenzaCessioneDirittiPi3Exception : Pi3Exception
    {
        #region Public Members

        public RagioneTrasmissioneSenzaCessioneDirittiPi3Exception()
            : base(ErrorDescriptions.RagioneTrasmissioneSenzaCessioneDiritti, null, ErrorDescriptions.ResourceManager)
        {
        }

        #endregion
    }

    public class NessunaCessioneDirittiAssegnataPi3Exception : Pi3Exception
    {
        #region Public Members

        public NessunaCessioneDirittiAssegnataPi3Exception()
            : base(ErrorDescriptions.NessunaCessioneDirittiAssegnata, null, ErrorDescriptions.ResourceManager)
        {
        }

        #endregion
    }

    public class CessioneDirittiConPiuTrasmissioniSingolePi3Exception : Pi3Exception
    {
        #region Public Members

        public CessioneDirittiConPiuTrasmissioniSingolePi3Exception()
            : base(ErrorDescriptions.CessioneDirittiConPiuTrasmissioniSingole, null, ErrorDescriptions.ResourceManager)
        {
        }

        #endregion
    }

    public class CessioneDirittiConPiuTrasmissioniUtentePi3Exception : Pi3Exception
    {
        #region Public Members

        public CessioneDirittiConPiuTrasmissioniUtentePi3Exception()
            : base(ErrorDescriptions.CessioneDirittiConPiuTrasmissioniUtente, null, ErrorDescriptions.ResourceManager)
        {
        }

        #endregion
    }
}
