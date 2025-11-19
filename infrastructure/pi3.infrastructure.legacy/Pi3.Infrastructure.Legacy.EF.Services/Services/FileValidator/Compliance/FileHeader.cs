// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Text;

namespace Pi3.Infrastructure.Legacy.EF.Services.FileValidator.Compliance
{
    internal class FileHeader
    {
        /// <summary>
        /// Compound document file identifier: D0H CFH 11H E0H A1H B1H 1AH E1H
        /// </summary>
        public byte[] FileTypeIdentifier;

        /// <summary>
        /// Unique identifier (UID) of this file (not of interest in the following, may be all 0)
        /// </summary>
        public Guid FileIdentifier;

        /// <summary>
        /// Revision number of the file format (most used is 003EH)
        /// </summary>
        public ushort RevisionNumber;

        /// <summary>
        /// Version number of the file format (most used is 0003H)
        /// </summary>
        public ushort VersionNumber;

        /// <summary>
        /// Byte order identifier: FEH FFH = Little-Endian FFH FEH = Big-Endian
        /// </summary>
        public byte[] ByteOrderMark;

        /// <summary>
        /// Size of a sector in the compound document file in power-of-two (ssz),
        /// (most used value is 9 which means 512 bytes, minimum value is 7 which means 128 bytes)
        /// </summary>
        public ushort SectorSizeInPot;

        /// <summary>
        /// Size of a short-sector in the short-stream container stream in power-of-two (ssz),
        /// (most used value is 6 which means 64 bytes, maximum value is sector size  in power-of-two)
        /// </summary>
        public ushort ShortSectorSizeInPot;

        /// <summary>
        /// Not used
        /// </summary>
        public byte[] UnUsed10;

        /// <summary>
        /// Total number of sectors used for the sector allocation table
        /// </summary>
        public int NumberOfSATSectors;

        /// <summary>
        /// SID of first sector of the directory stream
        /// </summary>
        public int FirstSectorIDofDirectoryStream;

        /// <summary>
        /// Not used
        /// </summary>
        public byte[] UnUsed4;

        /// <summary>
        /// Minimum size of a standard stream (in bytes, most used size is 4096 bytes),
        /// streams smaller than this value are stored as short-streams
        /// </summary>
        public int MinimumStreamSize;

        /// <summary>
        /// SID of first sector of the short-sector allocation table,
        /// or ¨C2 (End Of Chain SID) if not extant
        /// </summary>
        public int FirstSectorIDofShortSectorAllocationTable;

        /// <summary>
        /// Total number of sectors used for the short-sector allocation table
        /// </summary>
        public int NumberOfShortSectors;

        /// <summary>
        /// SID of first sector of the master sector allocation table,
        /// or ¨C2 (End Of Chain SID) if no additional sectors used
        /// </summary>
        public int FirstSectorIDofMasterSectorAllocationTable;

        /// <summary>
        /// Total number of sectors used for the master sector allocation table
        /// </summary>
        public int NumberOfMasterSectors;

        /// <summary>
        /// First part of the master sector allocation table containing 109 SIDs
        /// </summary>
        public int[] MasterSectorAllocationTable;
    }
}