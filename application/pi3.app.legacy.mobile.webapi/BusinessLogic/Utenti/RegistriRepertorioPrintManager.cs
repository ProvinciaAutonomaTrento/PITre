// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.utente.Repertori;

namespace BusinessLogic.Utenti;

public class RegistriRepertorioPrintManager
{
    public static List<RegistroRepertorio> GetRegisteredRegistries(String administrationId)
    {
        DocsPaDB.Query_DocsPAWS.RegistriRepertorioPrintManager manager = new DocsPaDB.Query_DocsPAWS.RegistriRepertorioPrintManager();
        return manager.GetRegisteredRegisters(administrationId);
    }

    public static RegistroRepertorioSingleSettings GetRegisterSettings(String counterId, String registryId, String rfId, RegistroRepertorio.TipologyKind tipologyKind, RegistroRepertorio.SettingsType settingsType)
    {
        DocsPaDB.Query_DocsPAWS.RegistriRepertorioPrintManager manager = new DocsPaDB.Query_DocsPAWS.RegistriRepertorioPrintManager();
        return manager.GetRegisterSettings(counterId, registryId, rfId, tipologyKind, settingsType);
    }

    public static bool SaveRuoloRespConservazione(string idGroup, string idUser, string idAmm)
    {
        DocsPaDB.Query_DocsPAWS.RegistriRepertorioPrintManager manager = new DocsPaDB.Query_DocsPAWS.RegistriRepertorioPrintManager();
        return manager.SaveRuoloRespConservazione(idGroup, idUser, idAmm);
    }

    /// <summary>
    /// Metodo per il recupero delle impostazioni minimali relative ad un repertorio
    /// </summary>
    /// <param name="counterId">Id del repertorio</param>
    /// <returns>Lista delle impostazioni minumali</returns>
    public static List<RegistroRepertorioSettingsMinimalInfo> GetSettingsMinimalInfo(String counterId, RegistroRepertorio.TipologyKind tipologyKind, string idAmm)
    {
        DocsPaDB.Query_DocsPAWS.RegistriRepertorioPrintManager manager = new DocsPaDB.Query_DocsPAWS.RegistriRepertorioPrintManager();
        return manager.GetSettingsMinimalInfo(counterId, tipologyKind, idAmm);

    }
}
