// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Text;

namespace Pi3.Infrastructure.Legacy.EF.Services.FileValidator.Compliance
{
    /// <summary>
    /// The master sector allocation table (MSAT) is an array of SecIDs of all sectors
    /// used by the sector allocation table (SAT).
    /// </summary>
    internal class MasterSectorAllocation
    {
        CompoundDocument Document;

        int NumberOfSecIDs;

        int CurrentMSATSector;

        int SecIDCapacity;

        List<int> MasterSectorAllocationTable;

        public MasterSectorAllocation(CompoundDocument document)
        {
            Document = document;
            NumberOfSecIDs = document.Header.NumberOfSATSectors;
            CurrentMSATSector = document.Header.FirstSectorIDofMasterSectorAllocationTable;
            SecIDCapacity = document.SectorSize / 4 - 1;
            InitializeMasterSectorAllocationTable();
        }

        private void InitializeMasterSectorAllocationTable()
        {
            MasterSectorAllocationTable = new List<int>(NumberOfSecIDs);
            SelectSIDs(Document.Header.MasterSectorAllocationTable);
            int msid = Document.Header.FirstSectorIDofMasterSectorAllocationTable;
            int timeout = 0;
            while (msid != SID.EOC)
            {
                timeout++;
                CurrentMSATSector = msid;
                int[] SIDs = Document.ReadSectorDataAsIntegers(msid);
                SelectSIDs(SIDs);
                msid = SIDs[SIDs.Length - 1];
                //15/05/2014 MeEsso timeout  per problemi a unfile doc, che non usciva dal ciclo..
                if (timeout > 2000000)
                    break;
            }
        }

        private void SelectSIDs(int[] SIDs)
        {
            for (int i = 0; i < SIDs.Length; i++)
            {
                int sid = SIDs[i];
                if (MasterSectorAllocationTable.Count < NumberOfSecIDs)
                {
                    MasterSectorAllocationTable.Add(sid);
                }
                else
                {
                    break;
                }
            }
        }

        public int GetSATSectorID(int SATSectorIndex)
        {
            if (SATSectorIndex < NumberOfSecIDs)
            {
                return MasterSectorAllocationTable[SATSectorIndex];
            }
            else if (SATSectorIndex == NumberOfSecIDs)
            {
                return AllocateSATSector();
            }
            else
            {
                throw new ArgumentOutOfRangeException("SATSectorIndex");
            }
        }

        public int AllocateSATSector()
        {
            int[] sids = new int[SecIDCapacity + 1];
            for (int i = 0; i < sids.Length; i++)
            {
                sids[i] = SID.Free;
            }
            int secID = Document.AllocateNewSector(sids);
            MasterSectorAllocationTable.Add(secID);
            NumberOfSecIDs++;
            int SATSectorIndex = NumberOfSecIDs - 1;
            if (NumberOfSecIDs <= 109)
            {
                Document.Header.MasterSectorAllocationTable[SATSectorIndex] = secID;
                Document.Write(76 + SATSectorIndex * 4, secID);
            }
            else
            {
                if (CurrentMSATSector == SID.EOC)
                {
                    CurrentMSATSector = AllocateMSATSector();
                    Document.Header.FirstSectorIDofMasterSectorAllocationTable = CurrentMSATSector;
                }
                int index = (SATSectorIndex - 109) % SecIDCapacity;
                Document.WriteInSector(CurrentMSATSector, index * 4, secID);
                if (index == SecIDCapacity - 1)
                {
                    int newMSATSector = AllocateMSATSector();
                    Document.WriteInSector(CurrentMSATSector, SecIDCapacity * 4, newMSATSector);
                    CurrentMSATSector = newMSATSector;
                }
            }
            Document.SectorAllocation.LinkSectorID(secID, SID.SAT);
            Document.Header.NumberOfSATSectors++;
            return secID;
        }

        public int AllocateMSATSector()
        {
            int[] secIDs = new int[SecIDCapacity + 1];
            for (int i = 0; i < SecIDCapacity; i++)
            {
                secIDs[i] = SID.Free;
            }
            secIDs[SecIDCapacity] = SID.EOC;

            int newMSATSector = Document.AllocateNewSector(secIDs);

            Document.SectorAllocation.LinkSectorID(newMSATSector, SID.MSAT);
            Document.Header.NumberOfMasterSectors++;

            return newMSATSector;
        }
    }
}
