// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices.ActiveDirectory;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.CreateFolder
{
    public class CreateFolderCommand: IRequest<CreateFolderCommandResponse>
    {
        /// <summary>
        /// Id del fascicolo di provenienza
        /// </summary>
        public string IdProject
        {
            get;
            set;
        }

        /// <summary>
        /// Id del titolario
        /// </summary>
        public string ClassificationSchemeId
        {
            get;
            set;
        }

        /// <summary>
        /// Codice del fascicolo
        /// </summary>
        public string CodeProject
        {
            get;
            set;
        }

        /// <summary>
        /// Id della cartella parent. Se omesso inserisce come sottofascicolo del fascicolo principale.
        /// Altrimenti della cartella indicata.
        /// </summary>
        public string IdParentFolder
        {
            get;
            set;
        }

        /// <summary>
        /// Descrizione della sottocartella
        /// </summary>
        public string FolderDescription
        {
            get;
            set;
        }
    }

    public class CreateFolderCommandResponse
    {
        public Folder Folder { get; set; }
        public string ErrorMessage { get; set; }
        public GetFolderResponseCode Code { get; set; }
    }
    public enum GetFolderResponseCode { OK, SYSTEM_ERROR }

}