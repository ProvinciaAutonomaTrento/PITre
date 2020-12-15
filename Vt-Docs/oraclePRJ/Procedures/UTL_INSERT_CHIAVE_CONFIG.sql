--------------------------------------------------------
--  DDL for Procedure UTL_INSERT_CHIAVE_CONFIG
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."UTL_INSERT_CHIAVE_CONFIG" (
    Codice               VARCHAR2 ,
    Descrizione          VARCHAR2 ,
    Valore               VARCHAR2 ,
    Tipo_Chiave          Varchar2 ,
    Visibile             VARCHAR2 ,
    Modificabile         VARCHAR2 ,
    Globale              VARCHAR2 ,
    Myversione_Cd          Varchar2 ,
    Codice_Old_Webconfig Varchar2 ,
    Forza_Update  VARCHAR2 , RFU VARCHAR2 )
Is

/* -- esempio blocco di invocazione 
Utl_Insert_Chiave_Config(
	'FE_COPIA_VISIBILITA'								--    Codice               VARCHAR2 ,
	,'Abilita il pulsante per la copia della visibilita' --    Descrizione          VARCHAR2 ,
	,'0' --    Valore               VARCHAR2 ,
	,'F' --    Tipo_Chiave          Varchar2 ,
	,'1' --    Visibile             VARCHAR2 ,
    ,'1' --    Modificabile         VARCHAR2 ,
    ,'1' --	Globale              VARCHAR2 ,
    , '3.20.1'  -- myversione_CD
    , NULL	--Codice_Old_Webconfig Varchar2 ,
    , NULL, NULL --Forza_Update_Valore  VARCHAR2 , RFU VARCHAR2 
	)
*/
  -- Pragma Autonomous_Transaction ;
  Cnt Int;
  Maxid Int;
  Nomeutente Varchar2 (32); 
  stringa_msg Varchar2 (200); 
BEGIN
  -- controlli lunghezza valori passati
  If Utl_IsValore_Lt_Column(Codice, 'DPA_CHIAVI_CONFIGURAZIONE', 'VAR_CODICE') = 1 Then 
    RAISE_APPLICATION_ERROR(-20001,'parametro CODICE too large for column VAR_CODICE')  ; 
  End If;
  If Utl_IsValore_Lt_Column(Descrizione, 'DPA_CHIAVI_CONFIGURAZIONE', 'VAR_Descrizione') = 1 Then 
    RAISE_APPLICATION_ERROR(-20001,'parametro Descrizione too large for column VAR_CODICE')  ; 
  End If;
  If Utl_IsValore_Lt_Column(Valore, 'DPA_CHIAVI_CONFIGURAZIONE', 'VAR_Valore') = 1 Then 
    RAISE_APPLICATION_ERROR(-20001,'parametro Valore too large for column VAR_CODICE')  ; 
  END IF;
  -- fine controlli lunghezza valori passati
  
Select Username Into Nomeutente From User_Users   ; 
 UTL_ADD_COLUMN(    myVERSIONE_CD,    Nomeutente ,    'DPA_CHIAVI_CONFIGURAZIONE',
    'DTA_INSERIMENTO',    'DATE',    'SYSDATE',    Null,    Null,   Null  );  
 UTL_ADD_COLUMN(    myVERSIONE_CD,    Nomeutente ,    'DPA_CHIAVI_CONFIGURAZIONE',
    'VERSIONE_CD',    'varchar2(32)',    NULL,    Null,    Null,   Null  );  
  
  -- per successivo controllo che sequence sia piu avanti del max system_id
  Select Max(System_Id) Into Maxid From Dpa_Chiavi_Configurazione; 
  
  SELECT COUNT(*)  INTO cnt
  FROM DPA_CHIAVI_CONFIGURAZIONE
  Where Var_Codice=Codice;
  
  If (Cnt         = 0 And Globale    = 1) Then -- inserisco la chiave globale non esistente
      -- must use exec immediate since column VERSIONE_CD may not exist yet at compile time
      execute immediate 'INSERT  INTO DPA_CHIAVI_CONFIGURAZIONE
        ( System_Id,          Id_Amm,          
          Var_Codice , VAR_DESCRIZIONE ,          VAR_VALORE ,
          Cha_Tipo_Chiave,          Cha_Visibile ,
          CHA_MODIFICABILE,          CHA_GLOBALE, 
          VAR_CODICE_OLD_WEBCONFIG, VERSIONE_CD )
        Values
        ( greatest(SEQ_DPA_CHIAVI_CONFIG.nextval,'||maxid||'),          0,'''
          ||Codice||''' ,'''||Descrizione   ||''', '''||valore  ||''' ,'''
          ||Tipo_Chiave   ||''', '''||Visibile||''' ,'''
          ||Modificabile  ||''', '''||Globale||''' ,  '''
          ||Codice_Old_Webconfig||''', '''||myVERSIONE_CD||'''   )';
  
  stringa_msg := 'inserita nuova chiave locale ' || Codice ; 
  Utl_Insert_Log (Nomeutente, Sysdate, Stringa_Msg, Myversione_Cd ,'ok') ; 
	End if;           
  
  If (Cnt         = 0 And Globale    = 0) Then -- inserisco la chiave non globale non esistente
      -- questa forma evita di dover eseguire la procedura CREA_KEYS_AMMINISTRA di riversamento !
      -- must use exec immediate since column VERSIONE_CD may not exist yet at compile time
      execute immediate 'INSERT      INTO dpa_chiavi_configurazione
        ( System_Id,          Id_Amm,          
          Var_Codice ,          VAR_DESCRIZIONE ,          VAR_VALORE ,
          Cha_Tipo_Chiave,          Cha_Visibile ,
          CHA_MODIFICABILE,          CHA_GLOBALE, 
          VAR_CODICE_OLD_WEBCONFIG , VERSIONE_CD)
      SELECT greatest(SEQ_DPA_CHIAVI_CONFIG.nextval,'||maxid||') AS system_id,
        Amm.System_Id                      As Id_Amm,'''
        ||Codice||''' As Var_Codice,'''        ||Descrizione||''' As Var_Descrizione ,      '''||Valore||''' As Var_Valore ,'''
        ||Tipo_Chiave||''' As Cha_Tipo_Chiave,       '''||Visibile||''' As Cha_Visibile ,'''
        ||Modificabile||''' As Cha_Modificabile,     '''||Globale||''' as CHA_GLOBALE ,'''
        ||CODICE_OLD_WEBCONFIG||''' as VAR_CODICE_OLD_WEBCONFIG , '''||myVERSIONE_CD||''' as VERSIONE_CD
      FROM Dpa_Amministra Amm
      WHERE NOT EXISTS        (SELECT Id_Amm FROM Dpa_Chiavi_Configurazione Where Var_Codice = '''||Codice||'''  )';
  
  stringa_msg := 'inserita nuove '||SQL%ROWCOUNT ||' chiavi locali per le amministrazioni: ' ||Codice ; 
  Utl_Insert_Log (Nomeutente, sysDATE, stringa_msg, Myversione_Cd ,'ok') ; 
	End If;
  
  If (Cnt                  = 1) Then -- chiave gia esistente
    DBMS_OUTPUT.PUT_LINE ('chiave ' || Codice || ' gia esistente'); 
    
	IF Forza_Update = '1' THEN
      UPDATE Dpa_Chiavi_Configurazione
      SET VAR_DESCRIZIONE = descrizione,
        VAR_VALORE        = valore, 
		Cha_Visibile	  = Visibile,
		Cha_Modificabile  = Modificabile,
		cha_Tipo_Chiave	  = Tipo_Chiave	
      Where Var_Codice    = Codice       and modificabile = '1';

    Stringa_Msg := 'AGGIORNATO VALORE, visibilita, modificabilita e tipo, per la CHIAVE: ' ||Codice || ' gia esistente' ; 
    Utl_Insert_Log (Nomeutente, Sysdate, Stringa_Msg, Myversione_Cd ,'ok') ; 
    
	ELSE -- aggiorno solo la descrizione
      UPDATE Dpa_Chiavi_Configurazione
      SET Var_Descrizione = Descrizione -- , Var_Valore = Valore
      WHERE Var_Codice    = Codice       and modificabile = '1';
    END IF;
  END IF;
  
  
  Exception When Others Then 
    Dbms_Output.put_line('errore da SP: Utl_Insert_Chiave_Config'||SQLERRM); 
    Rollback; 
    Raise; 
  
END;

/
