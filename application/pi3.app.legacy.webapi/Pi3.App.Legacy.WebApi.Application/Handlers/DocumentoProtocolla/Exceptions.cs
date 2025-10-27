// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoProtocolla
{
    public class ProjectNotFoundPi3Exception : NotFoundPi3Exception
    {
        public ProjectNotFoundPi3Exception(string id)
            : base(Resources.ProjectNotFoundPi3Exception, null, Resources.ResourceManager, id)
        {
            this.ID = id;
        }

        public string ID { get; init; }

    }

    public class RuoloNonAssociatoAlRegistroPi3Exception : Pi3Exception
    {
        public RuoloNonAssociatoAlRegistroPi3Exception(string idRuolo, string idRegistro )
            : base(ErrorDescription.RuoloNonAssociatoAlRegistro, null, Resources.ResourceManager, idRuolo, idRegistro)
        {
            this.IdRuolo = idRuolo;
            this.IdRegistro = idRegistro;
        }

        public string IdRuolo { get; init; }
        public string IdRegistro{ get; init; }
    }

    public class RegistroChiusoPi3Exception : Pi3Exception
    {
        public RegistroChiusoPi3Exception(string idRegistro)
            : base(ErrorDescription.RegistroChiuso, null, Resources.ResourceManager, idRegistro)
        {
            this.IdRegistro = idRegistro;
        }
        public string IdRegistro { get; init; }
    }

    public class RegistroMancantePi3Exception : Pi3Exception
    {
        public RegistroMancantePi3Exception()
            : base(ErrorDescription.RegistroMancante, null, Resources.ResourceManager)
        { }
    }

    public class AmministrazioneMancantePi3Exception : Pi3Exception
    {
        public AmministrazioneMancantePi3Exception()
            : base(ErrorDescription.AmministrazioneMancante, null, Resources.ResourceManager)
        { }
    }

    public class OggettoMancantePi3Exception : Pi3Exception
    {
        public OggettoMancantePi3Exception()
            : base(ErrorDescription.OggettoMancante, null, Resources.ResourceManager)
        { }
    }


    public class DocumentoInLibroFirmaPassoNonAttesoPi3Exception : Pi3Exception
    {
        public DocumentoInLibroFirmaPassoNonAttesoPi3Exception(string idProfile)
            : base(ErrorDescription.DocumentoInLibroFirmaPassoNonAtteso, null, Resources.ResourceManager, idProfile)
        {
            this.IdProfile = idProfile;
        }

        public string IdProfile { get; init; }
    }

    public class ProtocolloEntrataMittenteMancantePi3Exception : Pi3Exception
    {
        public ProtocolloEntrataMittenteMancantePi3Exception()
            : base(ErrorDescription.ProtocolloEntrataMittenteMancante, null, Resources.ResourceManager)
        {
        }
    }

    public class ProtocolloUscitaDestinatarioMancantePi3Exception : Pi3Exception
    {
        public ProtocolloUscitaDestinatarioMancantePi3Exception()
            : base(ErrorDescription.ProtocolloUscitaDestinatarioMancante, null, Resources.ResourceManager)
        {
        }
    }

    public class ProtocolloInternoMittenteMancantePi3Exception : Pi3Exception
    {
        public ProtocolloInternoMittenteMancantePi3Exception()
            : base(ErrorDescription.ProtocolloInternoMittenteMancante, null, Resources.ResourceManager)
        {
        }
    }

    public class ProtocolloInternoDestinatarioMancantePi3Exception : Pi3Exception
    {
        public ProtocolloInternoDestinatarioMancantePi3Exception()
            : base(ErrorDescription.ProtocolloInternoDestinatarioMancante, null, Resources.ResourceManager)
        {
        }
    }

    public class DocumentoProtocollatoPi3Exception : Pi3Exception
    {
        public DocumentoProtocollatoPi3Exception(string idProfile)
            : base(ErrorDescription.DocumentoProtocollato, null, Resources.ResourceManager, idProfile)
        {
            this.IdProfile = idProfile;
        }

        public string IdProfile { get; init; }
    }
}
