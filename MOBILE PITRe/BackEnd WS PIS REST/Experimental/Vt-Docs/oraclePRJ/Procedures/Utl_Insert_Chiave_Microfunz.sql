create or replace                     Procedure @db_user.Utl_Insert_Chiave_Microfunz(
    Codice                 Varchar2 ,
    Descrizione            Varchar2 ,
    Tipo_Chiave            Varchar2 ,
    disabilitata           VARCHAR2 ,
    Forza_Disabilitazione  Varchar2 ,
    Myversione_Cd          Varchar2 ,
    RFU VARCHAR2 )
Is

/* -- esempio blocco di invocazione 
*/
  Pragma Autonomous_Transaction ;
  Cnt Int;
  Stringa_Msg Varchar2 (200); 
     Nomeutente Varchar2 (32); 
BEGIN
  -- controlli lunghezza valori passati
  If Utl_IsValore_Lt_Column(Codice, 'dpa_anagrafica_funzioni', 'COD_FUNZIONE') = 1 Then 
    RAISE_APPLICATION_ERROR(-20001,'parametro CODICE too large for column COD_FUNZIONE')  ; 
  End If;
  If Utl_IsValore_Lt_Column(Descrizione, 'dpa_anagrafica_funzioni', 'VAR_DESC_FUNZIONE') = 1 Then 
    RAISE_APPLICATION_ERROR(-20001,'parametro Descrizione too large for column VAR_DESC_FUNZIONE')  ; 
  End If;
  If Utl_IsValore_Lt_Column(disabilitata, 'dpa_anagrafica_funzioni', 'DISABLED') = 1 Then 
    RAISE_APPLICATION_ERROR(-20001,'parametro Descrizione too large for column DISABLED')  ; 
  End If;
  -- fine controlli lunghezza valori passati
  Select Username Into Nomeutente From User_Users   ; 
  SELECT COUNT(*)  INTO cnt
  FROM dpa_anagrafica_funzioni
  Where COD_FUNZIONE=Codice;
  
  If (Cnt         = 0 ) Then -- inserisco la microfunzione non esistente
    INSERT  INTO dpa_anagrafica_funzioni
        ( COD_FUNZIONE,VAR_DESC_FUNZIONE
        ,CHA_TIPO_FUNZ,DISABLED )
        Values
        ( Codice,Descrizione          
        ,Tipo_Chiave          ,disabilitata  );
        stringa_msg := 'inserita nuova micro: ' || Codice ; 
  End if;           

  
  If (Cnt                  = 1) Then -- chiave già esistente
    IF Forza_disabilitazione = '1' THEN
      UPDATE dpa_anagrafica_funzioni
      SET VAR_DESC_FUNZIONE = descrizione,
        DISABLED        = disabilitata
      Where COD_FUNZIONE    =Codice;
      stringa_msg := 'AGGIORNATO VALORE DISABLED: '||disabilitata ||' PER micro ' ||Codice || ' già esistente'; 
    ELSE -- aggiorno solo la descrizione
      UPDATE dpa_anagrafica_funzioni
      SET VAR_DESC_FUNZIONE = Descrizione 
      Where Cod_Funzione    =Codice;
      
      stringa_msg := 'aggiornata Descrizione '||' PER micro ' ||Codice || ' già esistente'; 
    END IF;
  END IF;
  
  IF SQL%ROWCOUNT=1 THEN
      
      Utl_Insert_Log (Nomeutente, Sysdate, Stringa_Msg, Myversione_Cd ,'ok') ; 
      Commit;  -- can commit since this runs as Autonomous_Transaction 
      
  Else 
      Stringa_Msg := 'errore, troppi record modificati in Utl_Insert_microfunzione. eseguito rollback ' ;  
      Utl_Insert_Log (Nomeutente, sysDATE, stringa_msg, Myversione_Cd ,'ok') ; 
      Rollback; 
      
  END IF;

  
  Exception When Others Then 
    Dbms_Output.put_line('errore da SP Utl_Insert_chiave_microfunz:'||SQLERRM); 
    Rollback; 
    Raise; 
  
END;
/

