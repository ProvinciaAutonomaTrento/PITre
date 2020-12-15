--------------------------------------------------------
--  DDL for Procedure IS_ISELEMENTINTEROPERABLE
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."IS_ISELEMENTINTEROPERABLE" (
  -- Id dell'oggetto per cui verificare se  interoperante
  p_ObjectId  Number,
  -- Flag (1, 0) indicante se si sta verificando l'interoperabilit di un RF
  p_IsRf Number,
  -- Flag (1,0) che indica se l'AOO collegata all'RF specificato  interoperante
  p_IsInteroperable Out Number
) As
Begin
  
  /******************************************************************************
  
      AUTHOR:   Samuele Furnari
  
      NAME:     IsElementInteroperable
      
      PURPOSE:  Store per la verifica dello stato di abilitazione di un elemento
                (UO o RF) all'Interoperabilit Semplificata
  
    ******************************************************************************/


  -- Valore estratto dalla tabella con le impostazioni sull'IS
  Declare isInteroperable VarChar (1);
  
  Begin
    -- Se  un RF, viene verificato se  interoperante la AOO collegata
    If p_IsRf = 1 Then
      Select IsEnabledInteroperability  Into isInteroperable
        From dpa_el_registri
        Left Join InteroperabilitySettings 
        On id_aoo_collegata = RegistryId
      Where system_id = p_ObjectId;
    Else
      -- Altrimenti bisogna verificare se  abilitato all'interoperabilit l'AOO
      -- selezionata come interoperante per la UO
      Select IsEnabledInteroperability Into isInteroperable
        From dpa_corr_globali
        Left Join InteroperabilitySettings
        On InteropRegistryId = RegistryId
        Where system_id = p_ObjectId;
      
    End If;    
         
    -- Se non  stato estratto alcun valore (magari perch non sono mai state
    -- salvate informazioni sul registro legato all'RF specificato, l'RF non
    --  interoperante
    if(isInteroperable Is Null) Then
      isInteroperable := 0;
    End If;
    
    p_IsInteroperable := isInteroperable;
  End;
End IS_IsElementInteroperable;

/
