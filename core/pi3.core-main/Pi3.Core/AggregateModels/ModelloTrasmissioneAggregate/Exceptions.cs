// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.ModelloTrasmissioneAggregate
{ 
    public class NessunAutoreAssegnatoPi3Exception : Pi3Exception
    {
        #region Public Members

        public NessunAutoreAssegnatoPi3Exception()
            : base(ErrorDescriptions.NessunAutoreAssegnato, null, ErrorDescriptions.ResourceManager)
        {
        }

        #endregion
    }

    public class NessunDestinatarioAssegnatoPi3Exception : Pi3Exception
    {
        #region Public Members

        public NessunDestinatarioAssegnatoPi3Exception()
            : base(ErrorDescriptions.NessunDestinatarioAssegnato, null, ErrorDescriptions.ResourceManager)
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

    public class DestinatarioModelloTrasmissioneNotFoundPi3Exception : NotFoundPi3Exception
    {
        public DestinatarioModelloTrasmissioneNotFoundPi3Exception(string idDestinatario)
             : base(ErrorDescriptions.DestinatarioNonTrovato, null, ErrorDescriptions.ResourceManager)
        {
            this.IdDestinatario = idDestinatario;
        }

        public string IdDestinatario { get; init; }
    }


    public class DestintarioModelloTrasmissioneAlreadyExistsPi3Exception : Pi3Exception
    {
        #region Public Members

        public DestintarioModelloTrasmissioneAlreadyExistsPi3Exception(string idDestinatario)
            : base(ErrorDescriptions.DestinatarioModelloGiaPresente, null, ErrorDescriptions.ResourceManager)
        {
            this.IdDestinatario = idDestinatario;
        }

        public string IdDestinatario { get; init; }

        #endregion
    }

    public class DestintarioModelloTrasmissioneNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public DestintarioModelloTrasmissioneNotFoundPi3Exception(string idDestinatario)
            : base(ErrorDescriptions.DestinatarioModelloGiaPresente, null, ErrorDescriptions.ResourceManager)
        {
            this.IdDestinatario = idDestinatario;
        }

        public string IdDestinatario { get; init; }

        #endregion
    }

    public class UtenteNotificatoNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public UtenteNotificatoNotFoundPi3Exception(string idUtente)
            : base(ErrorDescriptions.UtenteNotificatoNonTrovato, null, ErrorDescriptions.ResourceManager)
        {
            this.IdUtente = idUtente;
        }

        public string IdUtente { get; init; }

        #endregion
    }

    public class RagioneTrasmissionePi3Exception : Pi3Exception
    {
        #region Public Members

        public RagioneTrasmissionePi3Exception(
            string idRagioneTrasmissione, string message, System.Resources.ResourceManager resourceManager, params object[] messageParameters)
            : base(message, resourceManager, messageParameters)
        {
            this.IdRagioneTrasmissione = idRagioneTrasmissione;
        }

        public string IdRagioneTrasmissione { get; init; }

        #endregion
    }
}
