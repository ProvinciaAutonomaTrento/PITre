// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.UploadBigFileInChunks
{
    public class UploadBigFileInChunksCommand: IRequest<UploadBigFileInChunksCommandResponse>
    {
        /// <summary>
        /// Id del documento nel quale acquisire il file
        /// </summary>
        public string IdDocument
        {
            get;
            set;
        }

        /// <summary>
        /// Nome del File da acquisire
        /// </summary>
        public string FileName
        {
            get;
            set;
        }

        /// <summary>
        /// Contenuto binario del chunk
        /// </summary>
        public byte[] ChunkContent { get; set; }

        /// <summary>
        /// Numero Progressivo del chunk
        /// </summary>
        public Int64? ChunkNumber { get; set; }

        /// <summary>
        /// Stringa identificativa della fase. Valori ammessi: START, END, CHUNK. Default CHUNK.
        /// </summary>
        public string Phase { get; set; }

        /// <summary>
        /// Hash del file o del chunk passato
        /// </summary>
        public string Hash
        {
            get;
            set;
        }

    }

    public class UploadBigFileInChunksCommandResponse: MessageResponse
    {
	}
}