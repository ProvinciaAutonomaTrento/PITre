begin 
	Utl_Backup_Plsql_code ('PROCEDURE','Utl_insert_chiave_log'); 
end;
/

create or replace
Procedure               Utl_insert_chiave_log(
    Codice               VARCHAR2 ,
    Descrizione          VARCHAR2 ,
    oggetto              VARCHAR2 ,
    Metodo               Varchar2 ,
    Forza_Aggiornamento  Varchar2 ,
    Myversione_cd         Varchar2 ,
    RFU                  VARCHAR2 )
Is

  Pragma Autonomous_Transaction ;
  Cnt Int;
  Maxid Int;
   Nomeutente Varchar2 (32); 
  Stringa_Msg Varchar2 (200); 
  
Begin
 -- controlli lunghezza valori passati
  
 If Utl_IsValore_Lt_Column(Codice, 'dpa_anagrafica_log', 'VAR_CODICE') = 1 Then 
    RAISE_APPLICATION_ERROR(-20001,'parametro CODICE too large for column VAR_CODICE')  ; 
  End If;
  If Utl_IsValore_Lt_Column(Descrizione, 'dpa_anagrafica_log', 'VAR_Descrizione') = 1 Then 
    RAISE_APPLICATION_ERROR(-20001,'parametro Descrizione too large for column VAR_Descrizione')  ; 
  End If;
  If Utl_IsValore_Lt_Column(OGGETTO, 'dpa_anagrafica_log', 'VAR_OGGETTO') = 1 Then 
    RAISE_APPLICATION_ERROR(-20001,'parametro Valore too large for column VAR_OGGETTO')  ; 
  END IF;
  If Utl_IsValore_Lt_Column(metodo, 'dpa_anagrafica_log', 'VAR_METODO') = 1 Then 
    RAISE_APPLICATION_ERROR(-20001,'parametro Valore too large for column VAR_METODO')  ; 
  END IF;
  -- fine controlli lunghezza valori passati
 
  Select Username Into Nomeutente From User_Users   ; 
 
  SELECT COUNT(*)  INTO cnt
  FROM dpa_anagrafica_log
  Where Var_Codice=Codice;
  Dbms_Output.Put_Line('Sql%Rowcount dopo:' ||Sql%Rowcount); 
  
  If (Cnt         = 0 ) Then -- inserisco la chiave globale non esistente
  -- per successivo inserimento
       Insert  Into Dpa_Anagrafica_Log
        ( System_Id,     var_Codice,      Var_Descrizione
        , Var_Oggetto,Var_Metodo ) 
      Select Max(System_Id) +1  As System_Id       ,Codice, Descrizione        
      , OGGETTO, METODO
      From dpa_anagrafica_log; 
      
  Stringa_Msg := 'inserita nuova chiave log ' || Codice ;  
  End if;           
  
  
  If (Cnt                  = 1 and Forza_aggiornamento = '1') Then -- chiave gi esistente
    UPDATE dpa_anagrafica_log
      SET Var_Descrizione = Descrizione, Var_Oggetto = oggetto, Var_Metodo = metodo
      Where Var_Codice    =Codice;
    Stringa_Msg := 'Aggiornati Descrizione, Oggetto, Metodo per chiave log: ' || Codice ;  
  End If;
  
  If Sql%Rowcount = 1 and Stringa_Msg is not null Then
    Dbms_Output.put_line('try to commit with stringa_msg:'||stringa_msg); 
      commit; 
      Utl_Insert_Log (Nomeutente, sysDATE, stringa_msg, Myversione_Cd ,'ok') ; 
   end if; 
  
  Exception When Others Then 
    Dbms_Output.put_line('errore da SP Utl_insert_chiave_log: '||SQLERRM); 
    Rollback; 
    Raise; 
  
END;
/

