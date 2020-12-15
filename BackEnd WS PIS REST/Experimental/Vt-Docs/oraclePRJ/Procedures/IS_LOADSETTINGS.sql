--------------------------------------------------------
--  DDL for Procedure IS_LOADSETTINGS
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."IS_LOADSETTINGS" 
(
  -- Id del registro / RF
  p_RegistryId Number, 
  -- Id del ruolo da utilizzare per la creazione del predisposto
  p_RoleId out Number, 
  -- Id dell'utente da utilizzare per creazione del predisposto
  p_UserId out Number, 
  -- Flag (1, 0) indicante se per il registro / rf deve essere abilitata l'interoperabilit
  p_IsEnabledInteroperability out Number,
  -- Flag (1, 0) indicante se i documenti ricevuti per IS devono essere mantenuti pendenti
  p_KeepPrivate out Number,
  -- Modalit (M per manuale, A per automatica) per la gestione dei document ricevuti per interoperabilit
  p_ManagementMode out varchar2
  
) As
Begin

  /******************************************************************************

    AUTHOR:   Samuele Furnari

    NAME:     LoadInteroperabilitySettings
    
    PURPOSE:  Store per il caricamento delle impostazioni relative ad un registro

  ******************************************************************************/

  Begin
    Select RoleId, UserId, IsEnabledInteroperability, KeepPrivate, ManagementMode Into p_RoleId, p_UserId, p_IsEnabledInteroperability, p_KeepPrivate, p_ManagementMode
    From InteroperabilitySettings 
    Where RegistryId = p_RegistryId;
    
  End;
End IS_LoadSettings;

/
