// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate
{
    public class TipoAggregazioneNonDefinitaPi3Exception : Pi3Exception
    {
        #region Public Members

        public TipoAggregazioneNonDefinitaPi3Exception()
            : base(ErrorDescriptions.TipoAggregazioneNonDefinita, null, ErrorDescriptions.ResourceManager)
        {
        }

        #endregion
    }

    public class TipologiaFascicoloNonDefinitaPi3Exception : Pi3Exception
    {
        #region Public Members

        public TipologiaFascicoloNonDefinitaPi3Exception()
            : base(ErrorDescriptions.TipologiaFascicoloNonDefinita, null, ErrorDescriptions.ResourceManager)
        {
        }

        #endregion
    }

    public class SerieDocumentaleNonDefinitaPi3Exception : Pi3Exception
    {
        #region Public Members

        public SerieDocumentaleNonDefinitaPi3Exception()
            : base(ErrorDescriptions.SerieDocumentaleNonDefinita, null, ErrorDescriptions.ResourceManager)
        {
        }

        #endregion
    }

    public class NessunaClassificazionePresentePi3Exception : Pi3Exception
    {
        #region Public Members

        public NessunaClassificazionePresentePi3Exception()
            : base(ErrorDescriptions.NessunaClassificazionePresentePi3Exception, null, ErrorDescriptions.ResourceManager)
        {
        }

        #endregion
    }

    public class RegistroNonDefinitoPi3Exception : Pi3Exception
    {
        #region Public Members

        public RegistroNonDefinitoPi3Exception()
            : base(ErrorDescriptions.RegistroNonDefinito, null, ErrorDescriptions.ResourceManager)
        {
        }

        #endregion
    }

    

    public class DataAperturaNonValidaPi3Exception : Pi3Exception
    {
        #region Public Members

        public DataAperturaNonValidaPi3Exception(DateTime dataApertura)
            : base(ErrorDescriptions.DataAperturaNonValida, null, ErrorDescriptions.ResourceManager)
        {
            this.DataApertura = dataApertura;
        }

        public DateTime DataApertura { get; init; }

        #endregion
    }

    public class RichiestaRegistrazioneNonEffettuataPi3Exception : Pi3Exception
    {
        #region Public Members

        public RichiestaRegistrazioneNonEffettuataPi3Exception()
            : base(ErrorDescriptions.RichiestaRegistrazioneNonEffettuata, null, ErrorDescriptions.ResourceManager)
        {
        }

        #endregion
    }

    public class IdDocPi3Exception : Pi3Exception
    {
        #region Public Members

        public IdDocPi3Exception(string message, System.Resources.ResourceManager resourceManager, IdDoc idDoc)
            : base(message, null, resourceManager, idDoc)
        {
            this.IdDoc = idDoc;
        }

        public IdDoc IdDoc { get; init; }

        #endregion
    }


    public class IdDocNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public IdDocNotFoundPi3Exception(IdDoc idDoc)
            : base(ErrorDescriptions.DocumentoNonTrovato, null, ErrorDescriptions.ResourceManager, null)
        {
            this.IdDoc = idDoc;
        }

        public IdDoc IdDoc { get; init; }

        #endregion
    }

    public class FolderNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public FolderNotFoundPi3Exception(string idFolder)
            : base(ErrorDescriptions.FolderNotFound, null, ErrorDescriptions.ResourceManager, idFolder)
        {
            this.IdFolder = idFolder;
        }

        public string IdFolder { get; init; }

        #endregion
    }

}
