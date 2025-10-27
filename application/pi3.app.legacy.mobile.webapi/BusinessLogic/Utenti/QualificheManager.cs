// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.Qualifica;
using DocsPaVO.Validations;

namespace BusinessLogic.Utenti;

public class QualificheManager
{
    private enum DBActionTypeRegistroEnum
    {
        InsertMode,
        UpdateMode,
        DeleteMode
    }

    public static List<DocsPaVO.Qualifica.Qualifica> GetQualifiche(int id_amm)
    {
        DocsPaDB.Query_DocsPAWS.Utenti utDb = new DocsPaDB.Query_DocsPAWS.Utenti();
        return utDb.GetQualifiche(id_amm);
    }

    public static List<DocsPaVO.Qualifica.PeopleGroupsQualifiche> GetPeopleGroupsQualifiche(String idAmm, String idUo, String idGruppo, String idPeople)
    {
        DocsPaDB.Query_DocsPAWS.Utenti utDb = new DocsPaDB.Query_DocsPAWS.Utenti();
        return utDb.GetPeopleGroupsQualifiche(idAmm, idUo, idGruppo, idPeople);
    }

    public static ValidationResultInfo InsertPeopleGroupsQual(PeopleGroupsQualifiche pgq)
    {
        ValidationResultInfo retValue = new ValidationResultInfo();
        DocsPaDB.Query_DocsPAWS.Utenti dbUtenti = new DocsPaDB.Query_DocsPAWS.Utenti();

        //Verifica presenza codice univoco

        retValue.Value = dbUtenti.CheckUniquePeopleGroups(pgq.ID_AMM.ToString(), pgq.ID_UO.ToString(), pgq.ID_GRUPPO.ToString(), pgq.ID_PEOPLE.ToString(), pgq.ID_QUALIFICA.ToString());

        if (!retValue.Value)
        {
            BrokenRule brokenRule = new BrokenRule();
            brokenRule.ID = "ASSOCIAZIONE_QUALIFICA";
            brokenRule.Description = "Qualifica già associata all'utente";
            retValue.BrokenRules.Add(brokenRule);
        }
        else
        {
            dbUtenti.InsertPeopleGroupsQual(pgq.ID_AMM.ToString(), pgq.ID_UO.ToString(), pgq.ID_GRUPPO.ToString(), pgq.ID_PEOPLE.ToString(), pgq.ID_QUALIFICA.ToString());
        }
        return retValue;
    }


    public static List<DocsPaVO.Qualifica.PeopleGroupsQualifiche> GetPeopleGroupsQualificheByIdPeople(String idPeople)
    {
        DocsPaDB.Query_DocsPAWS.Utenti utDb = new DocsPaDB.Query_DocsPAWS.Utenti();
        return utDb.GetPeopleGroupsQualificheByIdPeople(idPeople);
    }

    /// Verifica vincoli in inserimento di una qualifica, campi validi e univocità codice
    public static ValidationResultInfo InsertQual(Qualifica qual)
    {
        ValidationResultInfo retValue = IsValidRequiredFieldsQualifica(DBActionTypeRegistroEnum.InsertMode, qual);

        // Verifica presenza codice univoco
        if (retValue.Value)
        {
            DocsPaDB.Query_DocsPAWS.Utenti dbUtenti = new DocsPaDB.Query_DocsPAWS.Utenti();
            retValue.Value = dbUtenti.CheckUniqueCodiceQualifica(qual.CODICE, qual.ID_AMM.ToString());

            if (!retValue.Value)
            {
                BrokenRule brokenRule = new BrokenRule();
                brokenRule.ID = "CODICE_QUALIFICA";
                brokenRule.Description = "Codice qualifica già presente";
                retValue.BrokenRules.Add(brokenRule);
            }
            else
            {
                dbUtenti.InsertQual(qual.CODICE, qual.DESCRIZIONE, qual.ID_AMM.ToString());
                retValue.Value = true;
            }
        }
        return retValue;
    }

    /// Verifica presenza dati obbligatori della qualifica
    private static ValidationResultInfo IsValidRequiredFieldsQualifica(
                                DBActionTypeRegistroEnum actionType,
                                DocsPaVO.Qualifica.Qualifica qual)
    {
        ValidationResultInfo retValue = new ValidationResultInfo();
        BrokenRule brokenRule = null;

        if (actionType == DBActionTypeRegistroEnum.InsertMode ||
            actionType == DBActionTypeRegistroEnum.UpdateMode)
        {
            if (qual.CODICE == null || qual.CODICE == string.Empty)
            {
                retValue.Value = false;
                brokenRule = new BrokenRule();
                brokenRule.ID = "CODICE_QUALIFICA";
                brokenRule.Description = "Codice qualifica mancante";
                retValue.BrokenRules.Add(brokenRule);
            }

            if (qual.DESCRIZIONE == null || qual.DESCRIZIONE == string.Empty)
            {
                retValue.Value = false;
                brokenRule = new BrokenRule();
                brokenRule.ID = "DESCRIZIONE_QUALIFICA";
                brokenRule.Description = "Descrizione qualifica mancante";
                retValue.BrokenRules.Add(brokenRule);
            }

        }

        retValue.Value = (retValue.BrokenRules.Count == 0);

        return retValue;
    }

    public static ValidationResultInfo UpdateQual(String idQualifica, String descrizione)
    {
        ValidationResultInfo retValue = IsValidRequiredFieldsQualificaUpdate(descrizione);

        if (retValue.Value)
        {
            DocsPaDB.Query_DocsPAWS.Utenti dbUtenti = new DocsPaDB.Query_DocsPAWS.Utenti();
            dbUtenti.UpdateQual(idQualifica, descrizione);
            retValue.Value = true;
        }

        return retValue;
    }

    /// Verifica presenza descrizione per aggiornamento qualifica
    private static ValidationResultInfo IsValidRequiredFieldsQualificaUpdate(String descrizione)
    {
        ValidationResultInfo retValue = new ValidationResultInfo();
        BrokenRule brokenRule = null;

        if (descrizione == null || descrizione == string.Empty)
        {
            retValue.Value = false;
            brokenRule = new BrokenRule();
            brokenRule.ID = "DESCRIZIONE_QUALIFICA";
            brokenRule.Description = "Descrizione qualifica mancante";
            retValue.BrokenRules.Add(brokenRule);
        }

        retValue.Value = (retValue.BrokenRules.Count == 0);
        return retValue;

    }

    /// Cancellazione qualifica
    public static ValidationResultInfo DeleteQual(String idQualifica, int idAmministrazione)
    {
        ValidationResultInfo retValue = new ValidationResultInfo();
        DocsPaDB.Query_DocsPAWS.Utenti dbUtenti = new DocsPaDB.Query_DocsPAWS.Utenti();
        dbUtenti.DeleteQual(idQualifica, idAmministrazione.ToString());
        return retValue;
    }

}
