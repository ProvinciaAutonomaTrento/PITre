begin
	utl_backup_sp('UTL_INSERT_CHIAVE_CONFIG','3.22');
end;
/


create or replace Procedure @db_user.Utl_Insert_Chiave_Config(
    Codice               VARCHAR2 ,
    Descrizione          VARCHAR2 ,
    Valore               VARCHAR2 ,
    Tipo_Chiave          VARCHAR2 ,
    Visibile             VARCHAR2 ,
    Modificabile         VARCHAR2 ,
    Globale              VARCHAR2 ,
    Myversione_Cd        VARCHAR2 ,
    Codice_Old_Webconfig VARCHAR2 ,
    Forza_Update         VARCHAR2 ,
    RFU                  VARCHAR2 )
IS

/*Casi d'uso possibili:
  -- CASO A, chiave globale --
  A1. chiave globale esiste
  A1a.   se forza_update=1            aggiorno tutti i parametri, tra cui il valore ovviamente
  A1b.   se forza_update NULL o <>1   aggiorno solo la descrizione
  
  A2. chiave globale NON esiste
  A2a.   la inserisco come chiave globale
  
  -- CASO B, chiave locale --
  B1.   se forza_update=1           aggiorno tutti i parametri, tra cui il valore ovviamente, dove esisteva già
  B2.   se forza_update NULL o <>1, aggiorno solo la descrizione, dove esisteva già
  in tutti i casi B(!), inserisco la chiave in quelle amministrazioni dove non esisteva
  
  */
    
  -- la SP è stata semplificata nella parte di insert eliminando il costrutto di EXEC IMMEDIATE
  -- dato che si suppone che tutte le colonne esistano, in questa versione
  
  /* -- esempio blocco di invocazione
  begin
  Utl_Insert_Chiave_Config('FE_COPIA_VISIBILITA','Abilita il pulsante per la copia della visibilità'  -- Codice, Descrizione
  ,'0','F','1'        --Valore               ,Tipo_Chiave        ,Visibile
  ,'1','1','3.20.1'   --Modificabile         ,Globale            ,myversione_CD
  ,NULL, NULL, NULL);  --Codice_Old_Webconfig ,Forza_Update_Valore,RFU
  end;  */
  
  -- Pragma Autonomous_Transaction ;
  Cnt         INT;
  cntamm      INT;
  Maxid       INT;
  Nomeutente  VARCHAR2 (32);
  stringa_msg VARCHAR2 (200);
BEGIN
  -- controlli lunghezza valori passati
  IF Utl_IsValore_Lt_Column(Codice, 'DPA_CHIAVI_CONFIGURAZIONE', 'VAR_CODICE') = 1 THEN
    RAISE_APPLICATION_ERROR(-20001,'parametro CODICE too large for column VAR_CODICE') ;
  END IF;
  IF Utl_IsValore_Lt_Column(Descrizione, 'DPA_CHIAVI_CONFIGURAZIONE', 'VAR_Descrizione') = 1 THEN
    RAISE_APPLICATION_ERROR(-20001,'parametro Descrizione too large for column VAR_CODICE') ;
  END IF;
  IF Utl_IsValore_Lt_Column(Valore, 'DPA_CHIAVI_CONFIGURAZIONE', 'VAR_Valore') = 1 THEN
    RAISE_APPLICATION_ERROR(-20001,'parametro Valore too large for column VAR_CODICE') ;
  END IF;
  -- fine controlli lunghezza valori passati
  SELECT Username  INTO Nomeutente
  From User_Users ;
  
  SELECT COUNT(*)  INTO Cnt
  FROM Cols
  WHERE Table_Name = 'DPA_CHIAVI_CONFIGURAZIONE'
  And Column_Name In ('DTA_INSERIMENTO','VERSIONE_CD');
  
  IF Cnt           < 2 THEN
    Utl_Add_Column(Myversione_Cd,Nomeutente,'DPA_CHIAVI_CONFIGURAZIONE','DTA_INSERIMENTO','DATE' ,'SYSDATE', NULL,NULL,NULL);
    Utl_Add_Column(Myversione_Cd,Nomeutente,'DPA_CHIAVI_CONFIGURAZIONE','VERSIONE_CD' ,'varchar2(32)', NULL , NULL,NULL,NULL);
  END IF;
  
  -- per successivo controllo che sequence sia più avanti del max system_id
  SELECT MAX(System_Id)  INTO Maxid
  FROM Dpa_Chiavi_Configurazione;
  
  Select Count(*)  INTO cnt
  FROM DPA_CHIAVI_CONFIGURAZIONE
  WHERE Var_Codice=Codice;
  
  SELECT COUNT(*) INTO Cntamm FROM Dpa_Amministra ;
  
  If (Globale = 1 And Cnt = 1 ) Then -- caso A1, chiave globale già esistente
    Dbms_Output.Put_Line ('chiave globale ' ||Codice || ' già esistente');
    IF Forza_Update = '1' THEN
      Dbms_Output.Put_Line ('entro nel ramo di update forzato...');
      -- caso A1a
      UPDATE Dpa_Chiavi_Configurazione
      SET VAR_DESCRIZIONE = descrizione,
        Var_Valore        = Valore,
        Cha_Visibile      = Visibile,
        Cha_Modificabile  = Modificabile,
        cha_Tipo_Chiave   = Tipo_Chiave
      WHERE Var_Codice    = Codice
      AND modificabile    = '1';
      Stringa_Msg        := 'AGGIORNATO descrizione, valore, visibilità, modificabilità e tipo, per '||SQL%ROWCOUNT||' chiave globale ' ||Codice || ' modificabile, già esistente' ;
      Utl_Insert_Log (Nomeutente, Sysdate, Stringa_Msg, Myversione_Cd ,'ok') ;
    ELSE -- aggiorno solo la descrizione
      -- caso A1b
      UPDATE Dpa_Chiavi_Configurazione
      SET Var_Descrizione = Descrizione -- , Var_Valore = Valore
      WHERE Var_Codice    = Codice
      AND Modificabile    = '1';
    END IF;
  End If ;
  
  IF (Globale = 1 AND Cnt = 0 ) THEN -- caso A2, inserisco la chiave globale non esistente
    -- caso A2a
    INSERT    INTO Dpa_Chiavi_Configurazione
      ( System_Id ,        
      Id_Amm,        Var_Codice ,
        Var_Descrizione ,        Var_Valore ,        Cha_Tipo_Chiave ,
        Cha_Visibile ,        CHA_MODIFICABILE ,        CHA_GLOBALE,
        VAR_CODICE_OLD_WEBCONFIG,        VERSIONE_CD      )
      VALUES      (        Greatest(Seq_Dpa_Chiavi_Config.Nextval,Maxid)+1,
        0,        Codice ,
        Descrizione ,        valore ,        Tipo_Chiave ,
        Visibile,        Modificabile ,        Globale ,
        Codice_Old_Webconfig,        myVERSIONE_CD      );
    stringa_msg := 'inserita '||SQL%ROWCOUNT||' chiave globale ' ||Codice ;
    Utl_Insert_Log (Nomeutente, Sysdate, Stringa_Msg, Myversione_Cd ,'ok') ;
  END IF;
  
  /*
  -- CASO B, chiave locale --
  B1.   se forza_update=1           aggiorno tutti i parametri, tra cui il valore ovviamente, dove esisteva già
  B2.   se forza_update NULL o <>1, aggiorno solo la descrizione, dove esisteva già
  in tutti i casi, inserisco la chiave in quelle amministrazioni dove non esisteva
  
  */
  
  If Globale = 0  Then -- caso B1 chiave locale esiste su tutte le amministrazioni censite in DPA_AMMINISTRA
    If Forza_Update = '1' Then    
    -- caso B1
      Dbms_Output.Put_Line ('entro nel ramo di update forzato...');
      UPDATE Dpa_Chiavi_Configurazione
      SET VAR_DESCRIZIONE = descrizione,
        Var_Valore        = Valore,
        Cha_Visibile      = Visibile,
        Cha_Modificabile  = Modificabile,
        cha_Tipo_Chiave   = Tipo_Chiave
      WHERE Var_Codice    = Codice
      AND modificabile    = '1';
      Stringa_Msg        := 'AGGIORNATO descrizione, valore, visibilità, modificabilità e tipo, per '||SQL%ROWCOUNT||' chiave globale ' ||Codice || ' modificabile, già esistente' ;
      Utl_Insert_Log (Nomeutente, Sysdate, Stringa_Msg, Myversione_Cd ,'ok') ;
    ELSE -- aggiorno solo la descrizione
    -- caso B2
      UPDATE Dpa_Chiavi_Configurazione
      SET Var_Descrizione = Descrizione -- , Var_Valore = Valore
      WHERE Var_Codice    = Codice
      AND Modificabile    = '1';
    End If;
  
        -- inserisco dove non esiste 
    INSERT    INTO dpa_chiavi_configurazione      (
        System_Id,
        Id_Amm,
        Var_Codice ,
        VAR_DESCRIZIONE ,
        VAR_VALORE ,
        Cha_Tipo_Chiave,
        Cha_Visibile ,
        CHA_MODIFICABILE,
        CHA_GLOBALE,
        VAR_CODICE_OLD_WEBCONFIG ,
        VERSIONE_CD      )
    SELECT greatest(SEQ_DPA_CHIAVI_CONFIG.nextval,maxid) + rownum AS system_id,
      Amm.System_Id                                      AS Id_Amm,
      Codice                                             AS Var_Codice,
      Descrizione                                        AS Var_Descrizione ,
      Valore                                             AS Var_Valore ,
      Tipo_Chiave                                        AS Cha_Tipo_Chiave,
      Visibile                                           AS Cha_Visibile ,
      Modificabile                                       AS Cha_Modificabile,
      Globale                                            AS CHA_GLOBALE ,
      CODICE_OLD_WEBCONFIG                               AS VAR_CODICE_OLD_WEBCONFIG ,
      myVERSIONE_CD                                      AS VERSIONE_CD
    FROM Dpa_Amministra Amm
    WHERE NOT EXISTS
      (SELECT 'x'
      FROM Dpa_Chiavi_Configurazione Cc
      WHERE Cc.Var_Codice = Codice
      AND cc.Id_Amm       =Amm.System_Id
      );
    stringa_msg := 'inserite nuove '||SQL%ROWCOUNT|| ' chiavi locali per le amministrazioni: '|| Codice ;
    Utl_Insert_Log (Nomeutente, Sysdate, Stringa_Msg, Myversione_Cd ,'ok') ;
  
  End If;  
  
EXCEPTION
WHEN OTHERS THEN
  Dbms_Output.Put_Line('errore da SP: Utl_Insert_Chiave_Config'||Sqlerrm);
  Stringa_Msg := 'errore da SP: Utl_Insert_Chiave_Config'||Sqlerrm;
  Utl_Insert_Log (Nomeutente, Sysdate, Stringa_Msg, Myversione_Cd ,'KO') ;
  ROLLBACK;
  Raise;
END;
/