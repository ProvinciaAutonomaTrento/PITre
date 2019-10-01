--------------------------------------------------------
--  DDL for Procedure IS_SAVESETTINGS
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."IS_SAVESETTINGS" 
(
  -- Id del registro / RF
  p_RegistryId Number, 
  -- Id del ruolo da utilizzare per la creazione del predisposto
  p_RoleId Number, 
  -- Id dell'utente da utilizzare per creazione del predisposto
  p_UserId Number, 
  -- Flag (1, 0) indicante se per il registro / rf deve essere abilitata l'interoperabilit
  p_IsEnabledInteroperability Number,
  -- Modalit di gestione (M per manuale, A per automatica)
  p_ManagementMode Varchar2,
  -- Flag (1, 0) indicante se i documenti in ingresso devono essere manetenuti pendenti
  p_KeepPrivate Number
) As
Begin

  /******************************************************************************

    AUTHOR:   Samuele Furnari

    NAME:     SaveInteroperabilitySettings
    
    PURPOSE:  Store per il salvataggio delle impostazioni relative ad un registro

  ******************************************************************************/

  -- Flag utilizzato per indicare se esistono gi delle impostazioni relative
  -- al registro rf
  Declare alreadyExists Number := 0;
  
  Begin
    -- Verifica se esisono gi delle impostazioni per per il registro / RF
    Select Count(*) Into alreadyExists From InteroperabilitySettings Where RegistryId = p_RegistryId;
    
    -- Se non esistono impostazioni per il registro, viene creata una nuova tupla
    -- nella tabella delle impostazioni altrimenti viene aggiornata quella esistente
    If(alreadyExists = 0) Then
      Begin
        Insert
        Into InteroperabilitySettings
          (
            RegistryId,
            RoleId,
            UserId,
            IsEnabledInteroperability,
            ManagementMode,
            KeepPrivate
          )
          VALUES
          (
            p_RegistryId,
            p_RoleId,
            p_UserId,
            p_IsEnabledInteroperability,
            p_ManagementMode,
            p_KeepPrivate
          );

      End;
    Else
      Begin
        Update InteroperabilitySettings
        Set IsEnabledInteroperability = p_IsEnabledInteroperability,
            RoleId = p_RoleId,
            UserId = p_UserId,
            ManagementMode = p_ManagementMode,
            KeepPrivate = p_KeepPrivate
        Where RegistryId = p_RegistryId;
      End;
    End If;
  
  End;
End IS_SaveSettings ;

/
