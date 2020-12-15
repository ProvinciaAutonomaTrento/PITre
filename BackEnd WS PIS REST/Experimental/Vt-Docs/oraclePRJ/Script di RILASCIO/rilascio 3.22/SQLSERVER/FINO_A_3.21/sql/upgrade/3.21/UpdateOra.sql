-- file sql di update per il CD --
---- utl_add_column.ORA.SQL  marcatore per ricerca ----
/*
-- =============================================
-- Author:        Gabriele Serpi
-- Create date: 11/07/2011
-- Description:    
-- =============================================

-- es. invocazione:
Declare
  Versione_Cd Varchar2(200)  := '3.19.1';
  Nomeutente Varchar2(200)   := 'INFOTN_COLL';
  Nome_Tabella Varchar2(200) := 'TEST';
  Nome_Colonna Varchar2(200) := 'TEST_ADD2';
  Tipo_Dato Varchar2(200)    := 'CHAR(1)'; 
  VAL_DEFAULT VARCHAR2(200)  := '1';
  Condizione_Modifica_Pregresso Varchar2(200)
                                ;
  CONDIZIONE_CHECK VARCHAR2(200);
  RFU VARCHAR2(200);
BEGIN
  UTL_ADD_COLUMN(
    VERSIONE_CD => VERSIONE_CD,
    NOMEUTENTE => NOMEUTENTE,
    NOME_TABELLA => NOME_TABELLA,
    NOME_COLONNA => NOME_COLONNA,
    TIPO_DATO => TIPO_DATO,
    VAL_DEFAULT => VAL_DEFAULT,
    CONDIZIONE_MODIFICA_PREGRESSO => CONDIZIONE_MODIFICA_PREGRESSO,
    CONDIZIONE_CHECK => CONDIZIONE_CHECK,
    RFU => RFU
  );
End;

*/

CREATE OR REPLACE PROCEDURE @db_user.utl_add_column (
   versione_CD                      VARCHAR2,
   nomeutente                       VARCHAR2,
   nome_tabella                     VARCHAR2,
   nome_colonna                     VARCHAR2,
   tipo_dato                        VARCHAR2,
   val_default                      VARCHAR2,
   condizione_modifica_pregresso    VARCHAR2,
   condizione_check                 VARCHAR2,
   RFU                              VARCHAR2)
IS
   cnt   INT;
   istruzsql varchar2(200); 
BEGIN
   SELECT COUNT ( * )
     INTO cnt
     FROM all_tables
    WHERE table_name = UPPER (nome_tabella) AND owner = UPPER(nomeutente);
    
   IF (cnt = 1)    THEN    -- ok la tabella esiste
      SELECT COUNT ( * )
        INTO cnt
        FROM all_tab_columns
       WHERE     table_name = UPPER (nome_tabella)
             AND column_name = UPPER (nome_colonna)
             AND UPPER(owner) = nomeutente;
      IF (cnt = 0)       THEN       -- ok la colonna non esiste, la aggiungo
		 if val_default  IS null then 
		  istruzsql  := 'ALTER TABLE '  || nomeutente || '.' || nome_tabella
						|| ' ADD '      || nome_colonna|| ' '|| tipo_dato ; 
		  ELSE --  val_default  IS NOT null 
		  istruzsql  := 'ALTER TABLE '  || nomeutente || '.' || nome_tabella
					|| ' ADD '      || nome_colonna|| ' '|| tipo_dato || ' default '|| val_default   ; 

		end if ; 
         EXECUTE IMMEDIATE  istruzsql  ;
                          
                         utl_insert_log (nomeutente  -- nome utente
                         , NULL -- if null, data di oggi
                         ,'Added column '||nome_colonna||' on table '||nome_tabella
                         , versione_CD
                         , 'esito positivo' ); 
                         
        ELSE  -- cnt=1,    la colonna esiste gi 
             utl_modify_column (versione_CD                      
				   ,nomeutente  ,nome_tabella                     ,nome_colonna                     
				   ,tipo_dato                        
				   ,val_default                      
				   ,condizione_check                 
				   ,RFU                              ) ; 
			  
			  utl_insert_log (nomeutente  -- nome utente
                         , NULL -- data di oggi
                         ,'modified column '||nome_colonna||' on table '||nome_tabella
                         , versione_CD
                         , 'column modified, was already there' );
			  
      END IF;
   END IF;
   
            
EXCEPTION  -- qui cominciano le istruzioni da eseguire se le SP va in errore

   WHEN OTHERS
   THEN
      -- Consider logging the error and then re-raise
      
               dbms_output.put_line ('errore da SP utl_add_column: '||SQLERRM);
                  utl_insert_log (nomeutente  -- nome utente
                         , NULL -- data
                         ,'Adding column '||nome_colonna||' on table '||nome_tabella
                         , versione_CD
                         , 'esito negativo' ); 
                        -- RAISE;
               
END utl_add_column;
/              
---- Utl_Insert_Chiave_Config.ORA.sql  marcatore per ricerca ----
-- function used by Utl_Insert_Chiave_Config and other 2 Utl_Insert_Chiave%
Create Or Replace Function @db_user.Utl_Isvalore_Lt_Column
(Valore Varchar2,Mytable Varchar2,mycol varchar2)
Return Integer -- returns 0 if OK, that is: lentgh(valore) less then Data_Length of the column mycol
Is Cnt Int;
returnvalue int :=1;
Begin
Select  Data_Length - Length(Valore) Into Cnt   From Cols
  Where Lower(Table_Name)=Lower(Mytable) And Lower(Column_Name) = Lower(Mycol) ;

If Cnt>=0 Then Returnvalue := 0 ; End If;

Return Returnvalue ;
Exception When Others Then Raise;
end;
/


CREATE OR REPLACE Procedure @db_user.Utl_Insert_Chiave_Config(
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
	,'Abilita il pulsante per la copia della visibilit' --    Descrizione          VARCHAR2 ,
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
  
  -- per successivo controllo che sequence sia pi avanti del max system_id
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
  
  If (Cnt                  = 1) Then -- chiave gi esistente
    DBMS_OUTPUT.PUT_LINE ('chiave ' || Codice || ' gi esistente'); 
    
	IF Forza_Update = '1' THEN
      UPDATE Dpa_Chiavi_Configurazione
      SET VAR_DESCRIZIONE = descrizione,
        VAR_VALORE        = valore, 
		Cha_Visibile	  = Visibile,
		Cha_Modificabile  = Modificabile,
		cha_Tipo_Chiave	  = Tipo_Chiave	
      Where Var_Codice    = Codice       and modificabile = '1';

    Stringa_Msg := 'AGGIORNATO VALORE, visibilit, modificabilit e tipo, per la CHIAVE: ' ||Codice || ' gi esistente' ; 
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


              
---- Utl_insert_chiave_log.ORA.sql  marcatore per ricerca ----
create or replace Procedure @db_user.Utl_insert_chiave_log(
    Codice               VARCHAR2 ,
    Descrizione          VARCHAR2 ,
    oggetto              VARCHAR2 ,
    Metodo               Varchar2 ,
    Forza_Aggiornamento  Varchar2 , 
    Myversione_cd         Varchar2 ,
    RFU                  VARCHAR2 )
Is

/* -- esempio blocco di invocazione 
*/
  Pragma Autonomous_Transaction ;
  Cnt Int;
  Maxid Int;
   Nomeutente Varchar2 (32); 
  stringa_msg Varchar2 (200); 
  
BEGIN
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
  
  If (Cnt         = 0 ) Then -- inserisco la chiave globale non esistente
  -- per successivo inserimento
       Insert  Into Dpa_Anagrafica_Log
        ( System_Id,     var_Codice,      Var_Descrizione
        , Var_Oggetto,Var_Metodo ) 
      Select Max(System_Id) +1 As System_Id       ,Codice, Descrizione        
      , OGGETTO, METODO
      From dpa_anagrafica_log; 
      
  stringa_msg := 'inserita nuova chiave log ' || Codice ;  
  End if;           
  
  
  If (Cnt                  = 1 and Forza_aggiornamento = '1') Then -- chiave gi esistente
    UPDATE dpa_anagrafica_log
      SET Var_Descrizione = Descrizione, Var_Oggetto = oggetto, Var_Metodo = metodo
      WHERE Var_Codice    =Codice;
    stringa_msg := 'Aggiornati Descrizione, Oggetto, Metodo per chiave log: ' || Codice ;  
  END IF;
  
  if SQL%ROWCOUNT = 1 THEN 
      commit; 
      Utl_Insert_Log (Nomeutente, sysDATE, stringa_msg, Myversione_Cd ,'ok') ; 
  Else 
      Rollback; 
      Stringa_Msg  := 'errore, troppi record modificati in Utl_insert_chiave_log. Eseguito rollback ' ; 
      Utl_Insert_Log (Nomeutente, sysDATE, stringa_msg, Myversione_Cd ,'KO!') ; 
  end if; 
  
  Exception When Others Then 
    Dbms_Output.put_line('errore da SP Utl_insert_chiave_log: '||SQLERRM); 
    Rollback; 
    Raise; 
  
END;
/

              
---- Utl_Insert_Chiave_Microfunz.ORA.sql  marcatore per ricerca ----
create or replace Procedure @db_user.Utl_Insert_Chiave_Microfunz(
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

  
  If (Cnt                  = 1) Then -- chiave gi esistente
    IF Forza_disabilitazione = '1' THEN
      UPDATE dpa_anagrafica_funzioni
      SET VAR_DESC_FUNZIONE = descrizione,
        DISABLED        = disabilitata
      Where COD_FUNZIONE    =Codice;
      stringa_msg := 'AGGIORNATO VALORE DISABLED: '||disabilitata ||' PER micro ' ||Codice || ' gi esistente'; 
    ELSE -- aggiorno solo la descrizione
      UPDATE dpa_anagrafica_funzioni
      SET VAR_DESC_FUNZIONE = Descrizione 
      WHERE COD_FUNZIONE    =Codice;
	  stringa_msg := 'AGGIORNATO VAR_DESC_FUNZIONE: '||Descrizione ||' PER micro ' ||Codice || ' gi esistente'; 
    END IF;
  END IF;
  
  IF SQL%ROWCOUNT=1 THEN
      Commit;  -- can commit since this runs as Autonomous_Transaction 
      Utl_Insert_Log (Nomeutente, sysDATE, stringa_msg, Myversione_Cd ,'ok') ; 
  Else 
      Rollback; 
      stringa_msg := 'errore, troppi record modificati in Utl_Insert_microfunzione. eseguito rollback ' ;  
      Utl_Insert_Log (Nomeutente, sysDATE, stringa_msg, Myversione_Cd ,'ok') ; 
  END IF;

  
  Exception When Others Then 
    Dbms_Output.put_line('errore da SP Utl_Insert_chiave_microfunz:'||SQLERRM); 
    Rollback; 
    Raise; 
  
END;
/

              
---- utl_INSERT_LOG.ORA.SQL  marcatore per ricerca ----
CREATE OR REPLACE PROCEDURE @db_user.utl_insert_log (nomeutente VARCHAR2,
 data_eseguito DATE, comando_eseguito VARCHAR2, versione_CD VARCHAR2, esito VARCHAR2)
IS  
Pragma Autonomous_Transaction ;
cnt int;
MYUSERNAME VARCHAR2(200);
BEGIN
 
SELECT COUNT(*) INTO cnt FROM all_sequences where sequence_name='SEQ_INSTALL_LOG' 
	and sequence_owner= upper(nomeutente);
If (Cnt = 0) Then
        Execute Immediate 'CREATE SEQUENCE '||nomeutente||'.SEQ_INSTALL_LOG START WITH 1   MAXVALUE 99999999999     
					MINVALUE 1  NOCYCLE   NOCACHE   NOORDER';
end IF;

SELECT COUNT(*) INTO cnt FROM user_tables where table_name='DPA_LOG_INSTALL';
IF (cnt = 0) THEN
	select USERNAME INTO MYUSERNAME from user_users;
    RAISE_APPLICATION_ERROR(-20001,'Missing table '||MYUSERNAME||'.DPA_LOG_INSTALL');
end IF;

INSERT INTO DPA_LOG_INSTALL (ID,		DATA_OPERAZIONE,
	COMANDO_RICHIESTO,	VERSIONE_CD,	ESITO_OPERAZIONE) 
VALUES ( SEQ_INSTALL_LOG.nextval  ,NVL (data_eseguito, sysdate)
 ,comando_eseguito ,versione_CD ,esito ) ;
commit;  
 -- pls leave this output here, this is intended for tracing, not for debugging
   Dbms_Output.Put_Line (Comando_Eseguito ) ;  
     
EXCEPTION  WHEN OTHERS  THEN
 DBMS_OUTPUT.put_line ('errore da SP utl_insert_log: ' || SQLERRM);
 RAISE; --manda errore a sp chiamante
END utl_insert_log;
/

              
 
-------------------cartella  TABLE -------------------
              
---- ALTER_DPA_CORR_GLOBALI.ORA.sql  marcatore per ricerca ----
BEGIN 
-- by Furnari per IS 
 @db_user.utl_add_column('3.21.1', '@db_user', 
	'DPA_CORR_GLOBALI', 'INTEROPRFID', 'NUMBER', NULL, NULL, NULL, NULL); 
END;
/


-- modifiche by S. Frezza:
-- su tabella DPA_CORR_GLOBALI, aggiunta del campo CLASSIFICA_UO in cui sono inseriti 
--	i dati aggiuntivi che qualificano una unit organizzativa.
BEGIN
 @db_user.utl_add_column('3.21.1', '@db_user', 'DPA_CORR_GLOBALI', 'CLASSIFICA_UO', 'VARCHAR2(50)', NULL, NULL, NULL, NULL) ; 
END;
/



BEGIN
declare 
cntcol int; 
BEGIN

-- l'operazione di inizializzazione successiva va fatta solo all'atto della creazione della nuova colonna! 
-- quindi prima di creare la colonna controllo se la colonna non c'
  SELECT COUNT ( * )        INTO cntcol
        FROM all_tab_columns
       WHERE     table_name = UPPER ('DPA_CORR_GLOBALI')
             AND column_name = UPPER ('INTEROPURL')
             AND UPPER(owner) = '@db_user';

-- aggiungo colonna ...
@db_user.utl_add_column ('3.21.1', '@db_user'	
			, 'DPA_CORR_GLOBALI', 'INTEROPURL', 'VARCHAR2(4000)'	, NULL, NULL, NULL, NULL);

IF cntcol = 0 then -- inizializzo solo se la colonna non c'era....
-- va usato exec immeidate perch la colonna a compile time non c' ancora !!		
		execute immediate 'update dpa_corr_globali
		set cha_dettagli = ''1''
		where id_rf is not null'    ;

		execute immediate 'insert into dpa_dett_globali(system_id, id_corr_globali)
		(select		seq.nextval,		system_id
		from dpa_corr_globali
		where id_rf is not null) ';
END IF; 




@db_user.utl_add_column ('3.21.1', '@db_user'
			, 'DPA_CORR_GLOBALI', 'INTEROPREGISTRYID', 'NUMBER'	, NULL, NULL, NULL, NULL);


-- l'operazione di inizializzazione della nuova colonna
-- va fatta solo all'atto della creazione! 
-- quindi prima di creare la colonna controllo se la colonna non c'
  SELECT COUNT ( * )        INTO cntcol
        FROM all_tab_columns
       WHERE     table_name = UPPER ('DPA_CORR_GLOBALI')
             AND column_name = UPPER ('VAR_DESC_CORR_OLD')
             AND UPPER(owner) = '@db_user';

-- aggiungo colonna
@db_user.utl_add_column ('3.21.1', '@db_user'
			, 'DPA_CORR_GLOBALI', 'VAR_DESC_CORR_OLD', 'VARCHAR2(256)', NULL, NULL, NULL, NULL);

IF cntcol = 0 then -- eseguo solo se la colonna non c'era....
	execute immediate 'update DPA_CORR_GLOBALI 
	set  VAR_DESC_CORR_OLD = VAR_DESC_CORR 
	where VAR_DESC_CORR_OLD is null ';
END IF; 

END;
END;
/
              
----------- FINE -
              
---- ALTER_DPA_PAROLE.ORA.sql  marcatore per ricerca ----
-- ALTER TABLE @db_user.DPA_PAROLE	ADD ID_REGISTRO INT;
BEGIN
@db_user.utl_add_column ('3.21.1', '@db_user'	
			, 'DPA_PAROLE', 'ID_REGISTRO', 'INTEGER'	, NULL, NULL, NULL, NULL);
END;
/
/*   DECLARE
      nometabella   VARCHAR2 (32)  := 'DPA_PAROLE';
      nomecolonna   VARCHAR2 (32)  := 'ID_REGISTRO';
      tipodato      VARCHAR2 (200) := ' NUMBER(10) ';
      cnt           INT;
	nomeutente		VARCHAR2 (200) := upper('@db_user');

   BEGIN
      SELECT COUNT (*)  INTO cnt FROM all_tables
       WHERE table_name = UPPER (nometabella) and owner=nomeutente;

      IF (cnt = 1)
      -- ok la tabella esiste
      THEN
         SELECT COUNT (*) INTO cnt FROM all_tab_columns
          WHERE table_name = UPPER (nometabella)
            AND column_name = UPPER (nomecolonna)
			and owner=nomeutente;

         IF (cnt = 0)
         THEN
            EXECUTE IMMEDIATE    'ALTER TABLE '|| nomeutente ||'.'|| nometabella
                              || ' ADD '|| nomecolonna
                              || ' '|| tipodato;
         END IF;
      END IF;
   END;
END;
*/

              
----------- FINE -
              
---- ALTER_PEOPLE.ORA.sql  marcatore per ricerca ----
/* -- versione MSSQL
IF NOT EXISTS( SELECT * FROM INFORMATION_SCHEMA.COLUMNS              
                  WHERE TABLE_NAME = 'PEOPLE'             
                  AND  COLUMN_NAME = 'MATRICOLA') 
BEGIN         
    ALTER TABLE [DOCSADM].PEOPLE
      ADD MATRICOLA varchar(32) NULL
END   
GO    
*/
-- by S. Frezza su tabella PEOPLE, aggiunta del campo MATRICOLA, che ha la stessa dimensione del campo userid.
BEGIN
 @db_user.utl_add_column('3.21.1', '@db_user'
 , 'PEOPLE', 'MATRICOLA', 'VARCHAR2(32)', NULL, NULL, NULL, NULL) ; 
 
 @db_user.utl_add_column('3.21.1', '@db_user'
 , 'PEOPLE', 'ABILITATO_CHIAVI_CONFIG', 'INTEGER', '0', NULL, NULL, NULL) ; 
END;
/

begin
update @db_user.PEOPLE 
	set ABILITATO_CHIAVI_CONFIG = 1 
	where  cha_amministratore = '1' ; 

END;
/


              
----------- FINE -
              
---- CREATE_DPA_ASS_DOC_MAIL_INTEROP.ORA.SQL  marcatore per ricerca ----
begin
    declare   cnt int;
    begin        
select count(*) into cnt from all_tables where owner='@db_user' AND table_name='DPA_ASS_DOC_MAIL_INTEROP';
             if (cnt = 0) then
                    execute immediate   
                           'CREATE TABLE @db_user.DPA_ASS_DOC_MAIL_INTEROP
  (
    SYSTEM_ID          NUMBER(10, 0) NOT NULL ,
    ID_PROFILE         NUMBER(10, 0) NOT NULL ,
	  ID_REGISTRO		   NUMBER(10,0)  NOT NULL ,
    VAR_EMAIL_REGISTRO VARCHAR2(128 BYTE)
  )';
  END IF;
  END;
  END;
  /              
----------- FINE -
              
---- CREATE_DPA_MAIL_CORR_ESTERNI.ORA.SQL  marcatore per ricerca ----
begin
    declare   cnt int;
    begin        
select count(*) into cnt from all_tables where owner='@db_user' AND table_name='DPA_MAIL_CORR_ESTERNI';
             if (cnt = 0) then
                    execute immediate   
                'CREATE TABLE @db_user.DPA_MAIL_CORR_ESTERNI
				( SYSTEM_ID              NUMBER(10, 0) NOT NULL ,
				ID_CORR		           NUMBER(10, 0) NOT NULL ,    
				VAR_EMAIL			   VARCHAR2(128 BYTE) ,
				VAR_PRINCIPALE         VARCHAR2(1 BYTE) ,
				VAR_NOTE               VARCHAR2(50 BYTE)  )';
  END IF;
  END;
  END;
  /              
----------- FINE -
              
---- CREATE_DPA_MAIL_REGISTRI.ORA.SQL  marcatore per ricerca ----
-- by C. Ferlito 

begin
    declare   
	cnt int;

    begin        
	select count(*) into cnt from all_tables 
		WHERE 	owner='@db_user' AND table_name='DPA_MAIL_REGISTRI';
             
			 if (cnt = 0) then                    execute immediate   
                           'CREATE TABLE @db_user.DPA_MAIL_REGISTRI	(
								SYSTEM_ID              integer NOT NULL ,
								ID_REGISTRO            integer NOT NULL ,
								VAR_PRINCIPALE         VARCHAR2(1 BYTE) ,
								VAR_EMAIL_REGISTRO     VARCHAR2(128 BYTE) ,
								VAR_USER_MAIL          VARCHAR2(128 BYTE) ,
								VAR_PWD_MAIL           VARCHAR2(64 BYTE) ,
								VAR_SERVER_SMTP        VARCHAR2(64 BYTE) ,
								CHA_SMTP_SSL           VARCHAR2(1 BYTE) ,
								CHA_POP_SSL            VARCHAR2(1 BYTE) ,
								NUM_PORTA_SMTP         integer,
								CHA_SMTP_STA           VARCHAR2(1 BYTE) ,
								VAR_SERVER_POP         VARCHAR2(64 BYTE) ,
								NUM_PORTA_POP          integer ,
								VAR_USER_SMTP          VARCHAR2(128 BYTE) ,
								VAR_PWD_SMTP           VARCHAR2(128 BYTE) ,
								VAR_INBOX_IMAP         VARCHAR2(128 BYTE) ,
								VAR_SERVER_IMAP        VARCHAR2(128 BYTE) ,
								NUM_PORTA_IMAP         integer,
								VAR_TIPO_CONNESSIONE   VARCHAR2(10 BYTE) ,
								VAR_BOX_MAIL_ELABORATE VARCHAR2(50 BYTE) ,
								VAR_MAIL_NON_ELABORATE VARCHAR2(50 BYTE) ,
								CHA_IMAP_SSL           VARCHAR2(1 BYTE) ,
								VAR_SOLO_MAIL_PEC      VARCHAR2(1 BYTE) DEFAULT ''0'' ,
								CHA_RICEVUTA_PEC       VARCHAR2(2 BYTE) ,
								VAR_NOTE               VARCHAR2(50 BYTE)
							  )';

							  execute immediate   
							  'create unique index @db_user.IDX_Dpa_Mail_Registri_01  
							  on Dpa_Mail_Registri(ID_REGISTRO, VAR_EMAIL_REGISTRO) ' ; 

							  
	else 
	execute immediate   
							  'alter table  @db_user.Dpa_mail_Registri modify 
							  var_pwd_Mail varchar2(64)';
	end if; 
   
  END;
  END;
  /              
----------- FINE -
              
---- CREATE_DPA_PEOPLEGROUPS_QUALIFICHE.ORA.SQL  marcatore per ricerca ----
--Creazione Tabella DPA_PEOPLEGROUPS_QUALIFICHE
begin
    declare cnt int;
    begin
        select count(*) into cnt from all_tables 
			where owner='@db_user' and table_name='DPA_PEOPLEGROUPS_QUALIFICHE';
        if (cnt = 0) then
          execute immediate    
        'CREATE TABLE DPA_PEOPLEGROUPS_QUALIFICHE(
			SYSTEM_ID int PRIMARY KEY NOT NULL,
			ID_AMM int NULL,
			ID_UO int NULL,
			ID_GRUPPO int NULL,
			ID_PEOPLE int NULL,
			ID_QUALIFICA int NULL)';
end if;
end;    
end;    
/              
----------- FINE -
              
---- CREATE_DPA_QUALIFICHE.ORA.SQL  marcatore per ricerca ----
--Creazione Tabella DPA_QUALIFICHE
begin
    declare cnt int;
    begin
        select count(*) into cnt from all_tables 
		where owner='@db_user' and table_name='DPA_QUALIFICHE';
        if (cnt = 0) then
          execute immediate    
        'CREATE TABLE DPA_QUALIFICHE(
SYSTEM_ID int PRIMARY KEY NOT NULL,
CHA_COD varchar(64) NULL,
CHA_DESC varchar(255) NULL,
ID_AMM int NULL)';
end if;
    end;    
end;    
/
              
----------- FINE -
              
---- CREATE_DPA_VIS_MAIL_REGISTRI.ORA.SQL  marcatore per ricerca ----
begin
    declare   cnt int;
    begin        
select count(*) into cnt from all_tables where owner='@db_user' AND table_name='DPA_VIS_MAIL_REGISTRI';
             if (cnt = 0) then
                    execute immediate   
                           'CREATE TABLE @db_user.DPA_VIS_MAIL_REGISTRI
  (
    SYSTEM_ID          NUMBER(10, 0) NOT NULL ,
    ID_REGISTRO        NUMBER(10, 0) NOT NULL ,
    ID_RUOLO_IN_UO     NUMBER(10, 0) NOT NULL ,
    VAR_EMAIL_REGISTRO VARCHAR2(128 BYTE) ,
    CHA_CONSULTA       VARCHAR2(1 BYTE) DEFAULT ''1'' ,
    CHA_NOTIFICA       VARCHAR2(1 BYTE) DEFAULT ''1'' ,
    CHA_SPEDISCI       VARCHAR2(1 BYTE) DEFAULT ''1''
  )';
  END IF;
  END;
  END;
  /              
----------- FINE -
              
---- CREATE_INDEX_DPA_ITEMS_CONSERVAZIONE_ID_PROFILE.MSSQL.sql  marcatore per ricerca ----
--create index PAT_PROD.IND_ITEMS_CNSRVZ_K3 on PAT_PROD.DPA_ITEMS_CONSERVAZIONE (Id_Profile)

begin 
 @db_user.utl_add_index('3.21.1'   	
	,'@db_user'  	,'DPA_ITEMS_CONSERVAZIONE' ,'IND_ITEMS_CNSRVZ_K3'  ,NULL  
	,'ID_PROFILE'  	,NULL  	,NULL ,NULL 	,'NORMAL'  ,NULL  ,NULL  ,NULL  );
end ;
/



              
----------- FINE -
              
---- CREATE_INDEX_DPA_NOTIFICA_DOCNUMBER.ORA.sql  marcatore per ricerca ----
begin
utl_add_index('3.21.1',
	'@db_user',
	'DPA_NOTIFICA',
	'IND_NOTIFICA_K3',
	'',
	'DOCNUMBER',
	'',
	'',
	'',
	'NORMAL',
	NULL, -- if Index_Type == DOMAIN, must be supplied, can be CTXCAT or CONTEXT
	NULL, -- es sync (on commit) stoplist ctxsys.empty_stoplist
	NULL); 
end;
/

              
----------- FINE -
              
---- CREATE_INTEROPERABILITYSETTINGS.ORA.SQL  marcatore per ricerca ----
--Creazione Tabella INTEROPERABILITYSETTINGS
begin
    declare cnt int;
    begin
        select count(*) into cnt from all_tables 
			where owner='@db_user' and table_name='INTEROPERABILITYSETTINGS';
        if (cnt = 0) then
          execute immediate    
        'Create Table @db_user.INTEROPERABILITYSETTINGS
		(RegistryId               integer,
		RoleId                    integer,
		UserId                    integer,
		IsEnabledInteroperability integer,
		ManagementMode            Varchar(1),
		KeepPrivate               integer)';

		 execute immediate 'CREATE INDEX @db_user.IX_INTEROPSETTINGS_K1 
			ON @db_user.INTEROPERABILITYSETTINGS   (REGISTRYID )';

END IF;
END;
END;
/

              
----------- FINE -
              
---- Create_NotificationChannel.ORA.sql  marcatore per ricerca ----
--Creazione Tabella NOTIFICATIONCHANNEL
begin
    declare cnt int;
    begin
        select count(*) into cnt from all_tables 
			where owner='@db_user' and table_name='NOTIFICATIONCHANNEL';

        if (cnt = 0) then
          execute immediate    
        'Create Table @db_user.NotificationChannel 
			( Id Number, 
			  Label VarChar2(100), 
			  Description VarChar2(2000) )' ; 

		execute immediate  
				'CREATE SEQUENCE @db_user.SEQ_NotificationChannel 
				 MINVALUE 1 MAXVALUE 9999999999999999999999999999
				 INCREMENT BY 1 START WITH 1 CACHE 20 NOORDER NOCYCLE ';
 		END IF;
	END;
END;
/

              
----------- FINE -
              
---- Create_NOTIFICATIONINSTANCECHANNELS.ORA.sql  marcatore per ricerca ----
--Creazione Tabella NOTIFICATIONSETTINGS
begin
    declare cnt int;
    begin
        select count(*) into cnt from all_tables 
			where owner='@db_user' and table_name='NOTIFICATIONINSTANCECHANNELS';

        if (cnt = 0) then
          execute immediate 'Create Table @db_user.NotificationInstanceChannels
								( InstanceId integer,
								  ChannelId integer							)' ; 
		END IF;
	END;
END;
/
              
----------- FINE -
              
---- Create_NotificationInstance.ORA.sql  marcatore per ricerca ----
--Creazione Tabella NOTIFICATIONINSTANCE
begin
    declare cnt int;
    begin
        select count(*) into cnt from all_tables 
			where owner='@db_user' and table_name='NOTIFICATIONINSTANCE';

        if (cnt = 0) then
          execute immediate    
        'Create Table @db_user.NOTIFICATIONINSTANCE
			( Id integer,
			  Description VarChar2(100) ) '; 
			 
			 execute immediate  
				'CREATE SEQUENCE @db_user.SEQ_NotificationInstance 
				MINVALUE 1 MAXVALUE 9999999999999999999999999999 
				INCREMENT BY 1 START WITH 1 CACHE 20 NOORDER NOCYCLE' ;



		END IF;
	END;
END;
/
              
----------- FINE -
              
---- Create_NotificationItemCategories.ORA.sql  marcatore per ricerca ----
	--Creazione Tabella NOTIFICATIONITEMCATEGORIES
begin
    declare cnt int;
    begin
        select count(*) into cnt from all_tables 
			where owner='@db_user' and table_name='NOTIFICATIONITEMCATEGORIES';

        if (cnt = 0) then
              execute immediate    
				'Create Table @db_user.NotificationItemCategories
				( ItemId integer,
				  CategoryId integer) '; 
			   execute immediate  
				'CREATE SEQUENCE @db_user.SEQ_NotificationItemCategories 
				MINVALUE 1 MAXVALUE 9999999999999999999999999999 
				INCREMENT BY 1 START WITH 1 CACHE 20 NOORDER NOCYCLE' ;

				  		END IF;
	END;
END;
/
              
----------- FINE -
              
---- Create_NotificationItem.ORA.sql  marcatore per ricerca ----
--Creazione Tabella NOTIFICATIONITEM
begin
    declare cnt int;
    begin
        select count(*) into cnt from all_tables 
			where owner='@db_user' and table_name='NOTIFICATIONITEM';

        if (cnt = 0) then
          execute immediate 'Create Table @db_user.NOTIFICATIONITEM
								( Id integer,
								  Author VarChar2(100),
								  Title VarChar2(2000), 
								  Text VarChar2(2000), 
								  FeedLink VarChar2(1000), 
								  LastUpdate Date, 
								  PublishDate Date,
								 MessageId integer, 
								 MESSAGENUMBER integer ) ' ; 

		@db_user.utl_add_column('3.21.1', '@db_user'
 , 'NOTIFICATIONITEM', 'MESSAGENUMBER', ' integer', NULL, NULL, NULL, NULL) ; 
 
				 execute immediate  
				'CREATE SEQUENCE @db_user.SEQ_NotificationItem 
				MINVALUE 1 MAXVALUE 9999999999999999999999999999 
				INCREMENT BY 1 START WITH 1130 CACHE 20 NOORDER NOCYCLE' ;


		END IF;
	END;
END;
/
              
----------- FINE -
              
---- Create_NotificationUser.ORA.sql  marcatore per ricerca ----
--Creazione Tabella NOTIFICATIONITEMCATEGORIES
begin
    declare cnt int;
    begin
        select count(*) into cnt from all_tables 
			where owner='@db_user' and table_name='NOTIFICATIONUSER';

        if (cnt = 0) then
              execute immediate    
				'Create Table @db_user.NotificationUser
					( ItemId integer,
					  UserId integer,
					  ViewDate Date,
					  InstanceId integer) ' ; 
		END IF;
@db_user.utl_add_column('3.21.1', '@db_user'
 , 'NotificationUser', 'InstanceId', ' integer', NULL, NULL, NULL, NULL) ; 
	END;
END;
/
              
----------- FINE -
              
---- CREATE_SimpInteropDbLog.ORA.sql  marcatore per ricerca ----
--Creazione Tabella SimpInteropDbLog
begin
    declare cnt int;
    begin
        select count(*) into cnt from all_tables 
			where owner='@db_user' and table_name='SIMPINTEROPDBLOG';
        if (cnt = 0) then
          execute immediate    
        'Create Table @db_user.SimpInteropDbLog
			( ProfileId integer,
			ErrorMessage Number,
			Text Varchar2(4000)  ) ';
		END IF;
	END;
END;
/




              
----------- FINE -
              
---- CREATE_SimpInteropReceivedMessage.ORA.sql  marcatore per ricerca ----
--Creazione Tabella SimpInteropDbLog
begin
    declare cnt int;
    begin
        select count(*) into cnt from all_tables 
			where owner='@db_user' and table_name='SIMPINTEROPRECEIVEDMESSAGE';
        if (cnt = 0) then
          execute immediate    
        'Create Table @db_user.SimpInteropReceivedMessage
			( ProfileId			integer,
			  MessageId			VarChar2 (1000),
			  ReceivedPrivate	integer,
			  ReceivedDate		Date,
			  Subject			VarChar2(4000),
			  SenderDescription VarChar2(4000),
			  SenderUrl			VarChar2(2000),
			  SenderAdministrationCode VarChar(2000), 
			  AOOCode			VarChar(2000), 
			  RecordNumber		Number, 
			  RecordDate		Date, '||
 -- added lately
  'ReceiverCode VarChar2 (2000))';
		 execute immediate    
        'CREATE INDEX @db_user.IX_SIMPINTEROPMSG_K1  '||
		 'ON @db_user.SIMPINTEROPRECEIVEDMESSAGE (PROFILEID  )';

		END IF;
		
@db_user.utl_add_column ('3.21.1', '@db_user'
 , 'SimpInteropReceivedMessage', 'ReceiverCode', ' VarChar2(2000)', NULL, NULL, NULL, NULL );


	END;
END;
/

              
----------- FINE -
              
---- UTL_SYSTEM_LOG.ORA.sql  marcatore per ricerca ----

begin
    declare cnt int;
    begin
        select count(*) into cnt from all_tables 
			where owner='@db_user' and table_name='UTL_SYSTEM_LOG';

        if (cnt = 0) then
              execute immediate    
				'CREATE TABLE @db_user.UTL_SYSTEM_LOG
					  (ID int NOT NULL ENABLE,
						DATA_OPERAZIONE DATE NOT NULL ENABLE,
						COMANDO_RICHIESTO VARCHAR2(2000 BYTE) NOT NULL ENABLE,
						CATEGORIA_COMANDO VARCHAR2(2000 BYTE) NOT NULL ENABLE,
						ESITO_OPERAZIONE  Varchar2(2000 Byte) Not Null Enable  ) ' ; 
		END IF;

	END;
END;
/
              
----------- FINE -
              
 
-------------------cartella  SEQUENCE -------------------
              
---- SEQ_DPA_ASS_DOC_MAIL_INTEROP.ORA.SQL  marcatore per ricerca ----
BEGIN
    DECLARE cnt int;

  BEGIN
    
    SELECT COUNT(*) INTO cnt FROM all_sequences 
		where sequence_name='SEQ_DPA_ASS_DOC_MAIL_INTEROP';

    IF (cnt = 0) THEN        
          execute immediate ' 
CREATE SEQUENCE "@db_user"."SEQ_DPA_ASS_DOC_MAIL_INTEROP" 
MINVALUE 1 
MAXVALUE 9999999999999999999999999999 
INCREMENT BY 1 
START WITH 1250 
CACHE 20 
NOORDER NOCYCLE';
END IF;
END;
END;
/              
----------- FINE -
              
---- SEQ_DPA_MAIL_CORR_ESTERNI.ORA.SQL  marcatore per ricerca ----
BEGIN
    DECLARE cnt int;

  BEGIN
    
    SELECT COUNT(*) INTO cnt FROM all_sequences 
		where sequence_name='SEQ_DPA_MAIL_CORR_ESTERNI';

    IF (cnt = 0) THEN        
          execute immediate ' 
CREATE SEQUENCE "@db_user"."SEQ_DPA_MAIL_CORR_ESTERNI" 
MINVALUE 1 
MAXVALUE 9999999999999999999999999999 
INCREMENT BY 1 
START WITH 1250 
CACHE 20 
NOORDER NOCYCLE';
END IF;
END;
END;
/              
----------- FINE -
              
---- SEQ_DPA_MAIL_REGISTRI.ORA.SQL  marcatore per ricerca ----
BEGIN
    DECLARE cnt int;

  BEGIN
    
    SELECT COUNT(*) INTO cnt FROM all_sequences 
		where sequence_name='SEQ_DPA_MAIL_REGISTRI';

    IF (cnt = 0) THEN        
          execute immediate ' 
CREATE SEQUENCE "@db_user"."SEQ_DPA_MAIL_REGISTRI" 
MINVALUE 1 
MAXVALUE 9999999999999999999999999999 
INCREMENT BY 1 
START WITH 1250 
CACHE 20 
NOORDER NOCYCLE';
END IF;
END;
END;
/              
----------- FINE -
              
---- SEQ_DPA_PEOPLEGROUPS_QUALIF.ORA.SQL  marcatore per ricerca ----
BEGIN
    DECLARE cnt int;
BEGIN    
    SELECT COUNT(*) INTO cnt FROM user_sequences 
                              where sequence_name='SEQ_DPA_PEOPLEGROUPS_QUALIF';
    IF (cnt = 0) THEN        
       execute immediate '	
CREATE SEQUENCE SEQ_DPA_PEOPLEGROUPS_QUALIF START WITH 1 INCREMENT BY 1 MINVALUE 1';
END IF;
END;        
END;
/              
----------- FINE -
              
---- SEQ_DPA_QUALIFICHE.ORA.SQL  marcatore per ricerca ----
BEGIN
    DECLARE cnt int;
BEGIN    
    SELECT COUNT(*) INTO cnt FROM user_sequences 
                              where sequence_name='SEQ_DPA_QUALIFICHE';
    IF (cnt = 0) THEN        
       execute immediate ' 
CREATE SEQUENCE @db_user.SEQ_DPA_QUALIFICHE START WITH 1 INCREMENT BY 1 MINVALUE 1';
    END IF;
END;        
END;
/              
----------- FINE -
              
---- SEQ_DPA_VIS_MAIL_REGISTRI.ORA.SQL  marcatore per ricerca ----
BEGIN
    DECLARE cnt int;

  BEGIN
    
    SELECT COUNT(*) INTO cnt FROM all_sequences 
		where sequence_name='SEQ_DPA_VIS_MAIL_REGISTRI';

    IF (cnt = 0) THEN        
          execute immediate ' 
CREATE SEQUENCE "@db_user"."SEQ_DPA_VIS_MAIL_REGISTRI" 
MINVALUE 1 
MAXVALUE 9999999999999999999999999999 
INCREMENT BY 1 
START WITH 1250 
CACHE 20 
NOORDER NOCYCLE';
END IF;
END;
END;
/              
----------- FINE -
              
 
-------------------cartella  INSERT_UPDATE_DATA -------------------
              
---- INIT_PEC3.ORA.sql  marcatore per ricerca ----
-- spostata dopo la creazione della SP INITPEC3, vedi file relativo 
              
----------- FINE -
              
---- INSERT_DPA_ANAGRAFICA_FUNZIONI.ORA.SQL  marcatore per ricerca ----
BEGIN
declare 
cnt int;
attiva char(1); 
begin


-- by Furnari per IS
Utl_Insert_Chiave_Microfunz('NOTIFICATION_CENTER', -- Codice                 
'Visualizza gli item del centro notifiche nella to do list e ne abilita la gestione',-- Descrizione 
NULL, 'Y',				--Tipo_Chiave , disabilitata         
NULL, '3.21.1', NULL) ; --Forza_Disabilitazione , Myversione_Cd, RFU  

Utl_Insert_Chiave_Microfunz('PRAUISNP', -- Codice                 
'PRotocollazione AUtomatica Interoperabilit Semplificata documenti Non Privati',-- Descrizione 
NULL, 'Y',				--Tipo_Chiave , disabilitata         
NULL, '3.21.1', NULL) ; --Forza_Disabilitazione , Myversione_Cd, RFU  

Utl_Insert_Chiave_Microfunz('PRAUISP', -- Codice                 
'PRotocollazione AUtomatica Interoperabilit Semplificata documenti Privati',-- Descrizione 
NULL, 'Y',				--Tipo_Chiave , disabilitata         
NULL, '3.21.1', NULL) ; --Forza_Disabilitazione , Myversione_Cd, RFU  

Utl_Insert_Chiave_Microfunz('DELPREDIS', -- Codice                 
'Elimina Predisposto Interoperabilit Semplificata',-- Descrizione 
NULL, 'Y',				--Tipo_Chiave , disabilitata         
NULL, '3.21.1', NULL) ; --Forza_Disabilitazione , Myversione_Cd, RFU  
-- fine chiavi by Furnari per IS



Utl_Insert_Chiave_Microfunz('ELIMINA_TIPOLOGIA_DOC',
'Consente l''eliminazione di una tipologia di un documento.',
null,	'Y',
NULL, '3.21.1', NULL) ; --Forza_Disabilitazione , Myversione_Cd, RFU  



Utl_Insert_Chiave_Microfunz('GEST_REG_MODIFICA',
'Abilita il pulsante Modifica nella pagina di gestione di un registro',
null,	'Y',
NULL, '3.21.1', NULL) ; --Forza_Disabilitazione , Myversione_Cd, RFU  


Select Count(*) 	into cnt
From Dpa_Docspa 
	Where Id_Versions_I Like '%PITRE%' 		;

IF (cnt = 1) THEN  -- attiva solo su ambienti PITRE !
	
	update dpa_anagrafica_funzioni
	set disabled='N'
	where cod_funzione
		in ('NOTIFICATION_CENTER','PRAUISNP', 'PRAUISP','DELPREDIS' -- micro IS
		--,'ELIMINA_TIPOLOGIA_DOC','GEST_REG_MODIFICA' -- chiavi CDC ! 
		)
		and  disabled='Y';
END IF; 

end;
end;
/

	--select * from dpa_anagrafica_funzioni
	--where cod_funzione in ('NOTIFICATION_CENTER','PRAUISNP', 'PRAUISP', 'DELPREDIS')




              
----------- FINE -
              
---- INSERT_DPA_CHIAVI_CONFIGURAZIONE.ORA.SQL  marcatore per ricerca ----
Begin
declare 
cnt int;
attiva char(1) :=0; 
begin
-- Chiave per il web service con il certificato e senza password (di default con il valore 0) by Veltri
Utl_Insert_Chiave_Config( 'BE_SSOLOGIN', 
                'Utilizzata per il login con il certificato per i web services PIS',
                '0',                'B',                '1',
                '1',                '1', '3.21.1'
				,	NULL, NULL, NULL );


Utl_Insert_Chiave_Config('USE_TEXT_INDEX'
      , 'Chiave utilizzata per abilitare uso degli indici testuali su oggetto nelle ricerche documenti.'
      , '0', 'B', '1', '1', '1', '3.21.1', NULL, NULL, NULL);


-- aggiornamento descrizione by Lorusso
Utl_Insert_Chiave_Config('FE_MAX_LENGTH_DESC_TRASM'
, 'Chiave utilizzata per impostazione del numero massimo di caratteri digitabili nelle note generali della Trasmissione (valori accettabili: da 0 a 250)'
, '250', 'F', '1', '1', '0', '3.21.1',	NULL, NULL, NULL );

Utl_Insert_Chiave_Config('FE_PROTOIN_ACQ_DOC_OBBLIGATORIA '
   , 'Acquisizione file obbligatoria sulla protocollazione semplificata'
   , 'false', 'F', '1', '1', '0', '3.21.1',
	NULL, NULL, NULL );

-- by Luciani 
Utl_Insert_Chiave_Config('BE_MAIL_PROVIDER'
, 'Mail Provider [c=chilkat; m=ms.net]'
, 'c', 'B', '1', '1', '1','3.21.1',
	NULL, NULL, NULL );

-- by Abbatangeli
 Utl_Insert_Chiave_Config('ENABLE_LOW_SECURITY'
 , 'Verifica i diritti solo sul titolario e non sui suoi nodi'
 , '0', 'B', '1', '1', '1'  ,'3.21.1',
	NULL, NULL, NULL );


-- aggiornamento descrizione by A. Marta
Utl_Insert_Chiave_Config('FE_ENABLE_PROT_RAPIDA_NO_UO' ,
    'Permette (valore chiave = true), nella protocollazione semplice, di protocollare senza aver scelto una UO a cui smistare o di aver selezionato un modello di trasmissione per la trasmissione rapida',
    'false',    'F',    '1',    '1',    '0' ,'3.21.1',
	NULL, NULL, NULL );


Select Count(*) 	into cnt
From Dpa_Docspa 	Where Id_Versions_I Like '%PITRE%' 		;

--IF (cnt = 1) THEN  attiva:='1'; -- attiva solo su ambienti PITRE !
--	else 	attiva:='0'; 
--END IF; 

		-- by Furnari
		Utl_Insert_Chiave_Config('INTEROP_SERVICE_ACTIVE' ,
			'Stato di attivazione dell Interoperabilit Semplificata. 1 per attivare, 0 altrimenti',
			attiva,    'B',    '1',    '1'
			,'0' ,'3.21.1',
			NULL, NULL, NULL );   -- Codice_Old_Webconfig ,Forza_Update_Valore, RFU

		Utl_Insert_Chiave_Config('ENABLED_NOTIFICATION_CENTER', 'Attivazione del centro notifiche'
			, attiva,'B', '1', '1'  -- valore, Tipo_Chiave
			,'0', '3.21.1'			--Globale, versioneCD
			, NULL, NULL, NULL );   -- Codice_Old_Webconfig ,Forza_Update_Valore, RFU


		-- by Furnari -- globali
		Utl_Insert_Chiave_Config('FILE_SERVICE_URL', 'Url del servizio di gestione file'
			, 'n.d.','B', '1', '0'  -- valore, Tipo_Chiave
			,'1', '3.21.1'			--Globale, versioneCD
			, NULL, NULL, NULL );   -- Codice_Old_Webconfig ,Forza_Update_Valore, RFU

		-- by Furnari
		Utl_Insert_Chiave_Config('INTEROP_SERVICE_URL', 'Url del servizio di interoperabilit semplificata'
			, 'n.d.','B', '1', '0'  -- valore, Tipo_Chiave
		--si pu mettere anche come chiave locale alla singola amministrazione
		--, sovrascrivendo il valore della chiave globale
			,'1', '3.21.1'			--Globale, versioneCD
			, NULL, NULL, NULL);    -- Codice_Old_Webconfig ,Forza_Update_Valore, RFU


-- by Frezza 
Utl_Insert_Chiave_Config('FE_GESTIONE_MATRICOLE', 'La chiave abilita o meno la gestione delle matricole utente'
, '0','F', '1', '1'          -- valore, Tipo_Chiave
,'1', '3.21.1' -- Globale, versioneCD
, NULL, NULL, NULL); -- Codice_Old_Webconfig ,Forza_Update_Valore, RFU

-- by Frezza 
Utl_Insert_Chiave_Config('GESTIONE_QUALIFICHE'
, 'La chiave abilita o meno la gestione delle qualifiche utente'
, '0','B'          -- valore, Tipo_Chiave
, '1', '1','1' --Modificabile, Globale
, '3.21.1'
, NULL, NULL, NULL );-- Codice_Old_Webconfig ,Forza_Update_Valore, RFU

	
-- by Buono-Alibranti
--FE_PAROLE_CHIAVI_AVANZATE  chiave che PUO' essere locale! 
Utl_Insert_Chiave_Config('FE_PAROLE_CHIAVI_AVANZATE', 'Abilita la nuova gestione delle Parole Chiave'
, '0','F' , '1', '1'         -- valore, Tipo_Chiave
,'0', '3.21.1' --Globale
, NULL, NULL, NULL );-- Codice_Old_Webconfig ,Forza_Update_Valore, RFU



-- FE_FASC_RAPIDA_REQUIRED
Utl_Insert_Chiave_Config('FE_FASC_RAPIDA_REQUIRED', 'Obbligatorieta della classificazione o fascicolazione rapida'
, 'false','F' , '1', '1'         -- valore, Tipo_Chiave
,'0', '3.21.1' --Globale
, NULL, NULL, NULL );-- Codice_Old_Webconfig ,Forza_Update_Valore, RFU


end;
end;
/

--select * from dpa_chiavi_configurazione  where var_codice in
--  ('INTEROP_SERVICE_ACTIVE' ,'ENABLED_NOTIFICATION_CENTER','FILE_SERVICE_URL', 'INTEROP_SERVICE_URL');



              
----------- FINE -
              
---- INSERT_DPA_RAGIONE_TRASM.ORA.sql  marcatore per ricerca ----
BEGIN
    DECLARE cnt int;
  BEGIN
    
	--eseguire solo su PITRE !
	/*Update @db_user.Dpa_Docspa 
		set Id_Versions_I = Id_Versions_I||'-PITRE'
		where dta_install is not null
		and Id_Versions_I not like '%PITRE%';		*/
		
	
	-- solo PITRE !
	Select Count(*) 	into cnt
	From Dpa_Docspa 
		Where Id_Versions_I Like '%PITRE%' 		;

    IF (cnt = 1) THEN  -- eseguo solo su ambienti PITRE !
	
		Select Count(*) into cnt	
		From Dpa_Ragione_Trasm Rt 
		Where rt.VAR_DESC_RAGIONE='INTEROPERABILITA PITRE';
		
		IF (cnt = 0) THEN 

			INSERT INTO DPA_RAGIONE_TRASM  ( SYSTEM_ID,
					VAR_DESC_RAGIONE,					CHA_TIPO_RAGIONE,
					CHA_VIS,					CHA_TIPO_DIRITTI,
					CHA_TIPO_DEST,
					CHA_RISPOSTA,
					VAR_NOTE,
					CHA_EREDITA,
					ID_AMM,
					CHA_TIPO_RISPOSTA,
					VAR_NOTIFICA_TRASM,
					VAR_TESTO_MSG_NOTIFICA_DOC,
					VAR_TESTO_MSG_NOTIFICA_FASC,
					CHA_CEDE_DIRITTI,
					CHA_RAG_SISTEMA,
					CHA_MANTIENI_LETT
				  ) 				 select seq.nextval,					'INTEROPERABILITA PITRE',					'S',					'0',					'W',
					'T',					'0',					'Ragione per trasmissione interoperabilita'' PITRE',
					0,					System_Id,					Null,					Null,					Null,					Null,					'N',
					1,					Null 
          from dpa_amministra;
			END IF;
		END IF;
	END; 
END; 
/
              
----------- FINE -
              
 
-------------------cartella  FUNCTION -------------------
              
---- 8.UtenteHasQualifica.ORA.sql  marcatore per ricerca ----
CREATE OR REPLACE FUNCTION @db_user.UtenteHasQualifica
(
	codiceQualifica varchar,
	idPeople int
) 
RETURN int 
IS
mycount int ; 
retValue int ; 
BEGIN
	
	if codiceQualifica  is null 
	THEN
		retValue :=  0 ; 	return retValue ; 
	ELSE 
		select COUNT(pgq.SYSTEM_ID) into mycount
		from DPA_PEOPLEGROUPS_QUALIFICHE pgq
			inner join DPA_QUALIFICHE q on pgq.ID_QUALIFICA = q.SYSTEM_ID
		where q.CHA_COD = codiceQualifica and pgq.ID_PEOPLE = idPeople ; 
	END	IF;

	if (mycount > 0)
	THEN	retValue := 1 ; 
	ELSE
			retValue :=  0 ; 
	END IF; 
		
	return retValue ; 
END; 
/



              
----------- FINE -
              
---- Getdocnameorcodfasc.ORA.sql  marcatore per ricerca ----
create or replace Function @db_user.Getdocnameorcodfasc (Id Integer)
return varchar IS 
returnvalue varchar2(2000); 
Begin

/* dato un ID, o  un documento oppure un fascicolo 
INFATTI QUESTA QUERY TORNA ZERO RECORD: 
Select System_Id From ProJECT
  Intersect
Select System_Id From Profile */

Select Docname into returnvalue From Profile 
  Where System_Id = Id; 

return  Returnvalue; 
Exception 
When No_Data_Found Then  -- SE LA QUERY NON HA TORNATO VALORI, dovrebbe essere un fascicolo
Select Var_Codice into returnvalue From Project 
  Where System_Id = Id; 
return  Returnvalue; 
When Others Then Returnvalue := Null; -- richiesta esplicita che non si intercetti l'eccezione
return  Returnvalue; 
End; 
/              
----------- FINE -
              
---- getseoggattivo.ORA.sql  marcatore per ricerca ----
CREATE OR REPLACE FUNCTION @db_user.getseoggattivo (systemid NUMBER, idtemplate NUMBER)
   RETURN CHAR 
IS  
   tmpvar   CHAR;
   cnt   INT;

BEGIN

         SELECT COUNT (*)
           INTO cnt
           FROM dpa_associazione_templates
          WHERE id_template = idtemplate
            AND doc_number IS NULL
            AND id_oggetto = systemid;


            IF (cnt > 0)
            THEN
               tmpvar := '1';
            ELSE
               tmpvar := '0';
            END IF;

      RETURN tmpvar;      
      EXCEPTION
         WHEN OTHERS
         THEN
            cnt := 0;
END getseoggattivo;
/


              
----------- FINE -
              
---- getValCampoProfDoc.ORA.SQL  marcatore per ricerca ----
create or replace FUNCTION             @db_user.getValCampoProfDoc(DocNumber INT, CustomObjectId INT)
RETURN VARCHAR IS result VARCHAR(255);

tipoOggetto varchar(255);tipoCont varchar(1);repert number:=0; tipologiaDoc number:=0;
BEGIN

-- restituisce 1 se il documento DocNumber associato alla tipologia di documento contenente il contatore di repertorio con
-- id = CustomObjectId
SELECT (case when id_oggetto is not null then 1 else 0 end) as res
into tipologiaDoc
from dpa_associazione_templates
where doc_number=DocNumber and id_oggetto=CustomObjectId and rownum = 1;

select b.descrizione,cha_tipo_Tar, a.repertorio
into tipoOggetto,tipoCont,repert
from 
dpa_oggetti_custom a, dpa_tipo_oggetto b 
where 
a.system_id = CustomObjectId
and
a.id_tipo_oggetto = b.system_id;

if (tipoOggetto = 'Corrispondente') then
      select cg.var_cod_rubrica||' - '||cg.var_DESC_CORR into result 
      from dpa_CORR_globali cg where cg.SYSTEM_ID = (
      select valore_oggetto_db from dpa_associazione_templates where id_oggetto = CustomObjectId and doc_number = to_char(DocNumber));
     else
      
--Casella di selezione (Per la casella di selezione serve un caso particolare perch?? i valori sono multipli)
if(tipoOggetto = 'CasellaDiSelezione') then
    BEGIN
        declare item varchar(255);
        CURSOR curCasellaDiSelezione IS select  valore_oggetto_db into result from dpa_associazione_templates where id_oggetto = CustomObjectId and doc_number = to_char(DocNumber) and valore_oggetto_db is not null; 
        BEGIN
            OPEN curCasellaDiSelezione;
            LOOP
            FETCH curCasellaDiSelezione INTO item;
            EXIT WHEN curCasellaDiSelezione%NOTFOUND;
                IF(result IS NOT NULL) THEN
                result := result||'; '||item ;
                ELSE
                result := result||item;
                END IF;        
            END LOOP;
            CLOSE curCasellaDiSelezione;
        END;    
    END;
    elsif(tipoOggetto = 'Contatore' AND repert = 1 And tipologiadoc = 1)
    then
    begin
    RETURN '#CONTATORE_DI_REPERTORIO#';
    end;
    elsif(tipoOggetto = 'Contatore' OR tipoOggetto = 'ContatoreSottocontatore')  then
    begin
        select getContatoreDoc(DocNumber,tipoCont)  into result from dpa_associazione_templates where id_oggetto = CustomObjectId and doc_number = DocNumber; 

    end;
else 
--Tutti gli altri
    select valore_oggetto_db into result from dpa_associazione_templates where id_oggetto = CustomObjectId and doc_number = to_char(DocNumber); 
end if;	
end if;

RETURN result;

exception
when no_data_found
then
result := null; --'id_oggetto = '||CustomObjectId|| 'doc_number = '||DocNumber; 
RETURN result;
when others
then
result := SQLERRM; 
RETURN result;

End Getvalcampoprofdoc; 
/

              
----------- FINE -
              
 
-------------------cartella  STORED_PROCEDURE -------------------
              
---- 32.IS_SaveInteroperabilitySettings.ORA.sql  marcatore per ricerca ----
create or replace Procedure @db_user.IS_SaveSettings 
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
              
----------- FINE -
              
---- 33.IS_LoadInteroperabiiltySettings.ORA.sql  marcatore per ricerca ----
create or replace Procedure @db_user.IS_LoadSettings 
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
              
----------- FINE -
              
---- 34.IS_IsElementInteroperable.ORA.sql  marcatore per ricerca ----
create or replace Procedure @db_user.IS_IsElementInteroperable(
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
              
----------- FINE -
              
---- 61.IS_LoadReceivedPrivateFlag.ORA.sql  marcatore per ricerca ----
Create Or Replace Procedure @db_user.IS_LoadReceivedPrivateFlag
(
  p_ProfileId Number,
  p_ReceivedPrivate Out Number
) As
Begin

  Select ReceivedPrivate Into p_ReceivedPrivate From SimpInteropReceivedMessage Where ProfileId = p_ProfileId;

End IS_LoadReceivedPrivateFlag;
/
              
----------- FINE -
              
---- 72.IS_InsertDataInReceivedMessage.ORA.sql  marcatore per ricerca ----
create or replace Procedure         @db_user.IS_InsertDataInReceivedMsg (
  p_MessageId VarChar2,
  p_ReceivedPrivate Number,
  p_Subject Varchar2,
  p_SenderDescription VarChar2,
  p_SenderUrl VarChar2,
  p_SenderAdministrationCode VarChar2, 
  p_AOOCode VarChar2, 
  p_RecordNumber Number, 
  p_RecordDate Date,
  p_ReceiverCode VarChar2
) As
Begin
  -- Inserimento informazioni sul messaggio ricevuto
  Insert Into SimpInteropReceivedMessage
  ( MessageId,
    ReceivedPrivate,
    ReceivedDate,
    Subject,
    SenderDescription,
    SenderUrl,
    SenderAdministrationCode, 
    AOOCode, 
    RecordNumber, 
    RecordDate,
    ReceiverCode  )
  VALUES  (
    p_MessageId,
    p_ReceivedPrivate,
    SysDate,
    p_Subject,
    p_SenderDescription,
    p_SenderUrl,
    p_SenderAdministrationCode, 
    p_AOOCode, 
    p_RecordNumber, 
    p_RecordDate,
    p_ReceiverCode  );

  
End IS_InsertDataInReceivedMsg;
/              
----------- FINE -
              
---- 73.IS_InsertDataInSimpInteropLog.ORA.sql  marcatore per ricerca ----
create or replace Procedure @db_user.IS_InsertDataInSimpInteropLog(
  p_ProfileId Number,
  p_ErrorMessage Number,
  p_Text VarChar2
) As
Begin
  -- Inserimento voce di log
  Insert Into SimpInteropDbLog
  (
    ProfileId,
    ErrorMessage,
    Text
  )
  VALUES
  (
    p_ProfileId,
    p_ErrorMessage,
    p_Text
  );

  
End IS_InsertDataInSimpInteropLog;
/
              
----------- FINE -
              
---- 74.IS_SetIdProfForSimpInteropMessage.ORA.sql  marcatore per ricerca ----
create or replace Procedure @db_user.IS_SetIdProfForSimpInteropMsg (
  p_ProfileId Number,
  p_MessageId VarChar2
) As
Begin
  -- Aggiornamento del campo ProfileId dove compare il MessageId passato per parametro
  Update SimpInteropReceivedMessage
  Set ProfileId = p_ProfileId
  Where MessageId = p_MessageId;

  
End IS_SetIdProfForSimpInteropMsg;
/
              
----------- FINE -
              
---- 81.IS_LoadSimpInteropRecordInfo.sql  marcatore per ricerca ----
create or replace Procedure @db_user.IS_LoadSimpInteropRecordInfo(
  p_DocumentId integer,
  p_SenderAdministrationCode Out VarChar2,
  p_SenderAOOCode Out VarChar2,
  p_SenderRecordNumber Out Number,
  p_SenderRecordDate Out Date,
  p_ReceiverAdministrationCode Out VarChar2,
  p_ReceiverAOOCode Out VarChar2,
  p_ReceiverRecordNumber Out Number,
  p_ReceiverRecordDate Out Date,
  p_SenderUrl Out VarChar2  ,
-- new parameter added lately  
  p_ReceiverCode Out VarChar2
) As
Begin
  -- Caricamento delle informazioni sul protocollo mittente
  Select SenderAdministrationCode, AOOCode, RecordNumber, RecordDate, SenderUrl, ReceiverCode
  Into p_SenderAdministrationCode, p_SenderAOOCode, p_SenderRecordNumber, p_SenderRecordDate, p_SenderUrl, p_ReceiverCode
  From SimpInteropReceivedMessage
  Where ProfileId = p_DocumentId;
  
  -- Caricamento informazioni sul protocollo creato nell'amministrazione destinataria
  Select (Select var_codice From dpa_el_registri Where System_Id = Id_Registro), 
          Num_Proto, 
          Dta_proto, 
          (Select var_codice_amm From dpa_amministra Where system_id = (Select id_amm From dpa_corr_globali Where system_id = Id_Uo_Prot))
  Into p_ReceiverAOOCode, p_ReceiverRecordNumber, p_ReceiverRecordDate, p_ReceiverAdministrationCode           
  From profile 
  Where System_id = p_DocumentId;

End IS_LoadSimpInteropRecordInfo;
/
              
----------- FINE -
              
---- 82.Replace_spsetdatavista_tv.sql  marcatore per ricerca ----
create or replace PROCEDURE         @db_user.spsetdatavista_tv (
   p_idpeople      IN       NUMBER,
   p_idoggetto     IN       NUMBER,
   p_idgruppo      IN       NUMBER,
   p_tipooggetto   IN       CHAR,
   p_iddelegato    IN       NUMBER,
   p_resultvalue   OUT      NUMBER
)
IS
/*
----------------------------------------------------------------------------------------

RICHIAMATA SOLO DAL TASTO VISTO, agisce solo sulle trasmissioni NO WKFL. TOGLIENDOLE DALLA TDL 
NON SETTA DATA VISTA PERCH LO FA la SP_SET_DATAVISTA_V2


dpa_trasm_singola.cha_tipo_trasm = 'S''
-  se DPA_RAGIONE_TRASM.cha_tipo_ragione = 'N' OR  DPA_RAGIONE_TRASM.cha_tipo_ragione= 'I''
(ovvero senza WorkFlow): se accetta un utene in un ruolo sparisce a tutti nella ToDoList
-  se DPA_RAGIONE_TRASM.cha_tipo_ragione = 'W' : non deve sparire a nessuno, si setta solo la dta_vista
relativa all'utente corrente

dpa_trasm_singola.cha_tipo_trasm = 'T''
-  se DPA_RAGIONE_TRASM.cha_tipo_ragione = 'N' OR  DPA_RAGIONE_TRASM.cha_tipo_ragione = 'I''
(ovvero senza WorkFlow): se accetta un utene in un ruolo deve sparire solo all'utente che accetta, non a tutti
-  se DPA_RAGIONE_TRASM.cha_tipo_ragione = 'W' : non deve sparire a nessuno, si setta solo la dta_vista
relativa all'utente corrente

*/
   p_cha_tipo_trasm   CHAR (1) := NULL;
   p_chatipodest      NUMBER;
BEGIN
   p_resultvalue := 0;

   DECLARE
      CURSOR cursortrasmsingoladocumento
      IS
         SELECT b.system_id, b.cha_tipo_trasm, c.cha_tipo_ragione,
                b.cha_tipo_dest
           FROM dpa_trasmissione a, dpa_trasm_singola b, dpa_ragione_trasm c
          WHERE a.dta_invio IS NOT NULL
            AND a.system_id = b.id_trasmissione
            AND (   b.id_corr_globale = (SELECT system_id
                                           FROM dpa_corr_globali
                                          WHERE id_gruppo = p_idgruppo)
                 OR b.id_corr_globale = (SELECT system_id
                                           FROM dpa_corr_globali
                                          WHERE id_people = p_idpeople)
                )
            AND a.id_profile = p_idoggetto
            AND b.id_ragione = c.system_id;

      CURSOR cursortrasmsingolafascicolo
      IS
         SELECT b.system_id, b.cha_tipo_trasm, c.cha_tipo_ragione,
                b.cha_tipo_dest
           FROM dpa_trasmissione a, dpa_trasm_singola b, dpa_ragione_trasm c
          WHERE a.dta_invio IS NOT NULL
            AND a.system_id = b.id_trasmissione
            AND (   b.id_corr_globale = (SELECT system_id
                                           FROM dpa_corr_globali
                                          WHERE id_gruppo = p_idgruppo)
                 OR b.id_corr_globale = (SELECT system_id
                                           FROM dpa_corr_globali
                                          WHERE id_people = p_idpeople)
                )
            AND a.id_project = p_idoggetto
            AND b.id_ragione = c.system_id;
   BEGIN
      IF (p_tipooggetto = 'D')
      THEN
         FOR currenttrasmsingola IN cursortrasmsingoladocumento
         LOOP
            BEGIN
               IF (   currenttrasmsingola.cha_tipo_ragione = 'N'
                   OR currenttrasmsingola.cha_tipo_ragione = 'I'
-- modifica della 3.21 by S. Furnari
                   OR currenttrasmsingola.cha_tipo_ragione = 'S' -- Interoperabilit semplificata
                  )
               THEN
-- SE ? una trasmissione senza workFlow
                  IF (p_iddelegato = 0)
                  THEN
                     BEGIN
-- nella trasmissione utente relativa all'utente che sta vedendo il documento
-- setto la data di vista
                        UPDATE dpa_trasm_utente
                           SET --dpa_trasm_utente.cha_vista = '1',
                              -- dpa_trasm_utente.dta_vista =
                               --   (CASE
                               --       WHEN dta_vista IS NULL
                               --          THEN SYSDATE
                               --       ELSE dta_vista
                               --    END
                               --   ),
                               dpa_trasm_utente.cha_in_todolist = '0'
                         WHERE -- dpa_trasm_utente.dta_vista IS NULL AND
                          id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                           AND dpa_trasm_utente.id_people = p_idpeople;
                     EXCEPTION
                        WHEN OTHERS
                        THEN
                           p_resultvalue := 1;
                           RETURN;
                     END;
                  ELSE
--in caso di delega
                     BEGIN
-- nella trasmissione utente relativa all'utente che sta vedendo il documento
-- setto la data di vista
                        UPDATE dpa_trasm_utente
                           SET --dpa_trasm_utente.cha_vista = '1',
                               dpa_trasm_utente.cha_vista_delegato = '1',
                               dpa_trasm_utente.id_people_delegato =
                                                                  p_iddelegato,
                              -- dpa_trasm_utente.dta_vista =
                              --    (CASE
                              --        WHEN dta_vista IS NULL
                             -- --           THEN SYSDATE
                              --        ELSE dta_vista
                             --      END
                             --     ),
                               dpa_trasm_utente.cha_in_todolist = '0'
                         WHERE-- dpa_trasm_utente.dta_vista IS NULL AND
                            id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                           AND dpa_trasm_utente.id_people = p_idpeople;
                     EXCEPTION
                        WHEN OTHERS
                        THEN
                           p_resultvalue := 1;
                           RETURN;
                     END;
                  END IF;

                 /* BEGIN
                     UPDATE dpa_todolist
                        SET dta_vista = SYSDATE
                      WHERE id_trasm_singola = currenttrasmsingola.system_id
                        AND id_people_dest = p_idpeople
                        AND id_profile = p_idoggetto;
                  EXCEPTION
                     WHEN OTHERS
                     THEN
                        p_resultvalue := 1;
                        RETURN;
                  END; */

                  BEGIN
                     IF (    currenttrasmsingola.cha_tipo_trasm = 'S'
                         AND currenttrasmsingola.cha_tipo_dest = 'R'
                        )
                     THEN
-- se ? una trasmissione ? di tipo SINGOLA a un RUOLO allora devo aggiornare
-- anche le trasmissioni singole relative agli altri utenti del ruolo
                        IF (p_iddelegato = 0)
                        THEN
                           BEGIN
-- nelle trasmissioni utente relative agli altri utenti del ruolo
-- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList
                              UPDATE dpa_trasm_utente dpa_trasm_utente
                                 SET --dpa_trasm_utente.cha_vista = '1',
                                     dpa_trasm_utente.cha_in_todolist = '0'
                               WHERE --dpa_trasm_utente.dta_vista IS NULL AND
                                  id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                                 AND dpa_trasm_utente.id_people != p_idpeople
                                 AND EXISTS (
                                        SELECT 'x'
                                          FROM dpa_trasm_utente a
                                         WHERE a.id_trasm_singola =
                                                  dpa_trasm_utente.id_trasm_singola
                                           AND a.id_people = p_idpeople);
                                  --ovvero solo se io sono tra i notificati!!!
                           EXCEPTION
                              WHEN OTHERS
                              THEN
                                 p_resultvalue := 1;
                                 RETURN;
                           END;
                        ELSE
                           BEGIN
-- nelle trasmissioni utente relative agli altri utenti del ruolo
-- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList
                              UPDATE dpa_trasm_utente dpa_trasm_utente
                                 SET -- dpa_trasm_utente.cha_vista = '1',
                                     dpa_trasm_utente.cha_vista_delegato = '1',
                                     dpa_trasm_utente.id_people_delegato =
                                                                  p_iddelegato,
                                     dpa_trasm_utente.cha_in_todolist = '0'
                               WHERE --dpa_trasm_utente.dta_vista IS NULL AND
                                  id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                                 AND dpa_trasm_utente.id_people != p_idpeople
                                 AND EXISTS (
                                        SELECT 'x'
                                          FROM dpa_trasm_utente a
                                         WHERE a.id_trasm_singola =
                                                  dpa_trasm_utente.id_trasm_singola
                                           AND a.id_people = p_idpeople);
                                  --ovvero solo se io sono tra i notificati!!!
                           EXCEPTION
                              WHEN OTHERS
                              THEN
                                 p_resultvalue := 1;
                                 RETURN;
                           END;
                        END IF;
                     END IF;
                  END;
               ELSE
-- la ragione di trasmissione prevede workflow
                  IF (p_iddelegato = 0)
                  THEN
                     BEGIN
                        UPDATE dpa_trasm_utente
                           SET dpa_trasm_utente.cha_vista = '1',
                               dpa_trasm_utente.dta_vista =
                                  (CASE
                                      WHEN dta_vista IS NULL
                                         THEN SYSDATE
                                      ELSE dta_vista
                                   END
                                  )
                         WHERE dpa_trasm_utente.dta_vista IS NULL
                           AND id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                           AND dpa_trasm_utente.id_people = p_idpeople;
                     EXCEPTION
                        WHEN OTHERS
                        THEN
                           p_resultvalue := 1;
                           RETURN;
                     END;
                  ELSE
--in caso di delega
                     BEGIN
                        UPDATE dpa_trasm_utente
                           SET dpa_trasm_utente.cha_vista = '1',
                               dpa_trasm_utente.cha_vista_delegato = '1',
                               dpa_trasm_utente.id_people_delegato =
                                                                  p_iddelegato,
                               dpa_trasm_utente.dta_vista =
                                  (CASE
                                      WHEN dta_vista IS NULL
                                         THEN SYSDATE
                                      ELSE dta_vista
                                   END
                                  )
                         WHERE dpa_trasm_utente.dta_vista IS NULL
                           AND id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                           AND dpa_trasm_utente.id_people = p_idpeople;
                     EXCEPTION
                        WHEN OTHERS
                        THEN
                           p_resultvalue := 1;
                           RETURN;
                     END;
                  END IF;

                  BEGIN
-- Rimozione trasmissione da todolist solo se ? stata gi? accettata o rifiutata
                     UPDATE dpa_trasm_utente
                        SET cha_in_todolist = '0'
                      WHERE id_trasm_singola = currenttrasmsingola.system_id
                        AND NOT dpa_trasm_utente.dta_vista IS NULL
                        AND (cha_accettata = '1' OR cha_rifiutata = '1');

--AND dpa_trasm_utente.id_people = p_idpeople;
                     UPDATE dpa_todolist
                        SET dta_vista = SYSDATE
                      WHERE id_trasm_singola = currenttrasmsingola.system_id
                        AND id_people_dest = p_idpeople
                        AND id_profile = p_idoggetto;
                  EXCEPTION
                     WHEN OTHERS
                     THEN
                        p_resultvalue := 1;
                        RETURN;
                  END;
               END IF;
            END;
         END LOOP;
      END IF;

      IF (p_tipooggetto = 'F')
      THEN
         FOR currenttrasmsingola IN cursortrasmsingolafascicolo
         LOOP
            BEGIN
               IF (   currenttrasmsingola.cha_tipo_ragione = 'N'
                   OR currenttrasmsingola.cha_tipo_ragione = 'I'
-- modifica della 3.21 by S. Furnari
                   OR currenttrasmsingola.cha_tipo_ragione = 'S' -- Interoperabilit semplificata
                  )
               THEN
-- SE ? una trasmissione senza workFlow
                  IF (p_iddelegato = 0)
                  THEN
                     BEGIN
-- nella trasmissione utente relativa all'utente che sta vedendo il documento
-- setto la data di vista
                        UPDATE dpa_trasm_utente
                           SET --dpa_trasm_utente.cha_vista = '1',
                               --dpa_trasm_utente.dta_vista =
                              --    (CASE
                              --        WHEN dta_vista IS NULL
                              --           THEN SYSDATE
                              --        ELSE dta_vista
                              --     END
                              --    ),
                               dpa_trasm_utente.cha_in_todolist = '0'
                         WHERE --dpa_trasm_utente.dta_vista IS NULL AND
                            id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                           AND dpa_trasm_utente.id_people = p_idpeople;
                     EXCEPTION
                        WHEN OTHERS
                        THEN
                           p_resultvalue := 1;
                           RETURN;
                     END;
                  ELSE
--caso in cui si sta esercitando una delega
                     BEGIN
-- nella trasmissione utente relativa all'utente che sta vedendo il documento
-- setto la data di vista
                        UPDATE dpa_trasm_utente
                           SET  --dpa_trasm_utente.cha_vista = '1',
                               dpa_trasm_utente.cha_vista_delegato = '1',
                               dpa_trasm_utente.id_people_delegato =
                                                                  p_iddelegato,
                               --dpa_trasm_utente.dta_vista =
                               --   (CASE
                              --        WHEN dta_vista IS NULL
                              --           THEN SYSDATE
                              --        ELSE dta_vista
                             --      END
                             --     ),
                               dpa_trasm_utente.cha_in_todolist = '0'
                         WHERE --dpa_trasm_utente.dta_vista IS NULL AND
                            id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                           AND dpa_trasm_utente.id_people = p_idpeople;
                     EXCEPTION
                        WHEN OTHERS
                        THEN
                           p_resultvalue := 1;
                           RETURN;
                     END;
                  END IF;

                /*  BEGIN
                     UPDATE dpa_todolist
                        SET dta_vista = SYSDATE
                      WHERE id_trasm_singola = currenttrasmsingola.system_id
                        AND id_people_dest = p_idpeople
--and ID_RUOLO_DEST in (p_idGruppo,0)
                        AND id_project = p_idoggetto;
                  EXCEPTION
                     WHEN OTHERS
                     THEN
                        p_resultvalue := 1;
                        RETURN;
                  END; */

                  BEGIN
                     IF (    currenttrasmsingola.cha_tipo_trasm = 'S'
                         AND currenttrasmsingola.cha_tipo_dest = 'R'
                        )
                     THEN
                        IF (p_iddelegato = 0)
                        THEN
-- se ? una trasmissione ? di tipo SINGOLA a un RUOLO allora devo aggiornare
-- anche le trasmissioni singole relative agli altri utenti del ruolo
                           BEGIN
-- nelle trasmissioni utente relative agli altri utenti del ruolo
-- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList
                              UPDATE dpa_trasm_utente dpa_trasm_utente
                                 SET --dpa_trasm_utente.cha_vista = '1',
                                     dpa_trasm_utente.cha_in_todolist = '0'
                               WHERE --dpa_trasm_utente.dta_vista IS NULL AND
                                  id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                                 AND dpa_trasm_utente.id_people != p_idpeople
                                 AND EXISTS (
                                        SELECT 'x'
                                          FROM dpa_trasm_utente a
                                         WHERE a.id_trasm_singola =
                                                  dpa_trasm_utente.id_trasm_singola
                                           AND a.id_people = p_idpeople);
                                  --ovvero solo se io sono tra i notificati!!!
                           EXCEPTION
                              WHEN OTHERS
                              THEN
                                 p_resultvalue := 1;
                                 RETURN;
                           END;
                        ELSE
                           BEGIN
-- nelle trasmissioni utente relative agli altri utenti del ruolo
-- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList
                              UPDATE dpa_trasm_utente dpa_trasm_utente
                                 SET -- dpa_trasm_utente.cha_vista = '1',
                                     dpa_trasm_utente.cha_vista_delegato = '1',
                                     dpa_trasm_utente.id_people_delegato =
                                                                  p_iddelegato,
                                     dpa_trasm_utente.cha_in_todolist = '0'
                               WHERE --dpa_trasm_utente.dta_vista IS NULL AND
                                                                id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                                 AND dpa_trasm_utente.id_people != p_idpeople
                                 AND EXISTS (
                                        SELECT 'x'
                                          FROM dpa_trasm_utente a
                                         WHERE a.id_trasm_singola =
                                                  dpa_trasm_utente.id_trasm_singola
                                           AND a.id_people = p_idpeople);
                                  --ovvero solo se io sono tra i notificati!!!
                           EXCEPTION
                              WHEN OTHERS
                              THEN
                                 p_resultvalue := 1;
                                 RETURN;
                           END;
                        END IF;
                     END IF;
                  END;
               ELSE
-- se la ragione di trasmissione prevede workflow
                  IF (p_iddelegato = 0)
                  THEN
                     BEGIN
                        UPDATE dpa_trasm_utente
                           SET dpa_trasm_utente.cha_vista = '1',
                              dpa_trasm_utente.dta_vista =
                                  (CASE
                                     WHEN dta_vista IS NULL
                                        THEN SYSDATE
                                     ELSE dta_vista
                                  END
                                 )
                         WHERE dpa_trasm_utente.dta_vista IS NULL and
                            id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                           AND dpa_trasm_utente.id_people = p_idpeople;
                     EXCEPTION
                        WHEN OTHERS
                        THEN
                           p_resultvalue := 1;
                           RETURN;
                     END;
                  ELSE
-- caso in cui si sta esercitando una delega
                     BEGIN
                        UPDATE dpa_trasm_utente
                           SET -- dpa_trasm_utente.cha_vista = '1',
                               dpa_trasm_utente.cha_vista_delegato = '1',
                               dpa_trasm_utente.id_people_delegato =
                                                                  p_iddelegato--,
                              -- dpa_trasm_utente.dta_vista =
                              --    (CASE
                               --       WHEN dta_vista IS NULL
                               ---          THEN SYSDATE
                               --       ELSE dta_vista
                               --    END
                               --   )
                         WHERE-- dpa_trasm_utente.dta_vista IS NULL AND
                            id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                           AND dpa_trasm_utente.id_people = p_idpeople;
                     EXCEPTION
                        WHEN OTHERS
                        THEN
                           p_resultvalue := 1;
                           RETURN;
                     END;
                  END IF;

                  BEGIN
-- Rimozione trasmissione da todolist solo se ? stata gi? accettata o rifiutata
                     UPDATE dpa_trasm_utente
                        SET cha_in_todolist = '0'
                      WHERE id_trasm_singola = currenttrasmsingola.system_id
                        AND NOT dpa_trasm_utente.dta_vista IS NULL
                        AND (cha_accettata = '1' OR cha_rifiutata = '1')
                        AND dpa_trasm_utente.id_people = p_idpeople;

                     UPDATE dpa_todolist
                        SET dta_vista = SYSDATE
                      WHERE id_trasm_singola = currenttrasmsingola.system_id
                        AND id_people_dest = p_idpeople
--and ID_RUOLO_DEST in (p_idGruppo,0)
                        AND id_project = p_idoggetto;
                  EXCEPTION
                     WHEN OTHERS
                     THEN
                        p_resultvalue := 1;
                        RETURN;
                  END;
               END IF;
            END;
         END LOOP;
      END IF;
   END;
END spsetdatavista_tv;
/              
----------- FINE -
              
---- 83.Replace_spsetdatavista_v2.sql  marcatore per ricerca ----
create or replace PROCEDURE         @db_user.spsetdatavista_v2 (
   p_idpeople      IN       NUMBER,
   p_idoggetto     IN       NUMBER,
   p_idgruppo      IN       NUMBER,
   p_tipooggetto   IN       CHAR,
   p_iddelegato    IN       NUMBER,
   p_resultvalue   OUT      NUMBER
)
IS
/*
----------------------------------------------------------------------------------------
dpa_trasm_singola.cha_tipo_trasm = 'S''
-  se DPA_RAGIONE_TRASM.cha_tipo_ragione = 'N' OR  DPA_RAGIONE_TRASM.cha_tipo_ragione= 'I''
(ovvero senza WorkFlow): se accetta un utene in un ruolo sparisce a tutti nella ToDoList
-  se DPA_RAGIONE_TRASM.cha_tipo_ragione = 'W' : non deve sparire a nessuno, si setta solo la dta_vista
relativa all'utente corrente

dpa_trasm_singola.cha_tipo_trasm = 'T''
-  se DPA_RAGIONE_TRASM.cha_tipo_ragione = 'N' OR  DPA_RAGIONE_TRASM.cha_tipo_ragione = 'I''
(ovvero senza WorkFlow): se accetta un utene in un ruolo deve sparire solo all'utente che accetta, non a tutti
-  se DPA_RAGIONE_TRASM.cha_tipo_ragione = 'W' : non deve sparire a nessuno, si setta solo la dta_vista
relativa all'utente corrente

*/
   p_cha_tipo_trasm   CHAR (1) := NULL;
   p_chatipodest      NUMBER;
BEGIN
   p_resultvalue := 0;

   DECLARE
      CURSOR cursortrasmsingoladocumento
      IS
         SELECT b.system_id, b.cha_tipo_trasm, c.cha_tipo_ragione,
                b.cha_tipo_dest
           FROM dpa_trasmissione a, dpa_trasm_singola b, dpa_ragione_trasm c
          WHERE a.dta_invio IS NOT NULL
            AND a.system_id = b.id_trasmissione
            AND (   b.id_corr_globale = (SELECT system_id
                                           FROM dpa_corr_globali
                                          WHERE id_gruppo = p_idgruppo)
                 OR b.id_corr_globale = (SELECT system_id
                                           FROM dpa_corr_globali
                                          WHERE id_people = p_idpeople)
                )
            AND a.id_profile = p_idoggetto
            AND b.id_ragione = c.system_id;

      CURSOR cursortrasmsingolafascicolo
      IS
         SELECT b.system_id, b.cha_tipo_trasm, c.cha_tipo_ragione,
                b.cha_tipo_dest
           FROM dpa_trasmissione a, dpa_trasm_singola b, dpa_ragione_trasm c
          WHERE a.dta_invio IS NOT NULL
            AND a.system_id = b.id_trasmissione
            AND (   b.id_corr_globale = (SELECT system_id
                                           FROM dpa_corr_globali
                                          WHERE id_gruppo = p_idgruppo)
                 OR b.id_corr_globale = (SELECT system_id
                                           FROM dpa_corr_globali
                                          WHERE id_people = p_idpeople)
                )
            AND a.id_project = p_idoggetto
            AND b.id_ragione = c.system_id;
   BEGIN
      IF (p_tipooggetto = 'D')
      THEN
         FOR currenttrasmsingola IN cursortrasmsingoladocumento
         LOOP
            BEGIN
               IF (   currenttrasmsingola.cha_tipo_ragione = 'N'
                   OR currenttrasmsingola.cha_tipo_ragione = 'I'
-- modifica della 3.21 by S. Furnari
                   Or currenttrasmsingola.cha_tipo_ragione = 'S' -- Interoperabilit semplificata
                  )
               THEN
-- SE ? una trasmissione senza workFlow
                  IF (p_iddelegato = 0)
                  THEN
                     BEGIN
-- nella trasmissione utente relativa all'utente che sta vedendo il documento
-- setto la data di vista
                        UPDATE dpa_trasm_utente
                           SET dpa_trasm_utente.cha_vista = '1',
                               dpa_trasm_utente.dta_vista =
                                  (CASE
                                      WHEN dta_vista IS NULL
                                         THEN SYSDATE
                                      ELSE dta_vista
                                   END
                                  )--,
                             --  dpa_trasm_utente.cha_in_todolist = '0'
                         WHERE dpa_trasm_utente.dta_vista IS NULL
                           AND id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                           AND dpa_trasm_utente.id_people = p_idpeople;
                     EXCEPTION
                        WHEN OTHERS
                        THEN
                           p_resultvalue := 1;
                           RETURN;
                     END;
                  ELSE
--in caso di delega
                     BEGIN
-- nella trasmissione utente relativa all'utente che sta vedendo il documento
-- setto la data di vista
                        UPDATE dpa_trasm_utente
                           SET dpa_trasm_utente.cha_vista = '1',
                               dpa_trasm_utente.cha_vista_delegato = '1',
                               dpa_trasm_utente.id_people_delegato =
                                                                  p_iddelegato,
                               dpa_trasm_utente.dta_vista =
                                  (CASE
                                      WHEN dta_vista IS NULL
                                         THEN SYSDATE
                                      ELSE dta_vista
                                   END
                                  )--,
                              -- dpa_trasm_utente.cha_in_todolist = '0'
                         WHERE dpa_trasm_utente.dta_vista IS NULL
                           AND id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                           AND dpa_trasm_utente.id_people = p_idpeople;
                     EXCEPTION
                        WHEN OTHERS
                        THEN
                           p_resultvalue := 1;
                           RETURN;
                     END;
                  END IF;

                  BEGIN
                     UPDATE dpa_todolist
                        SET dta_vista = SYSDATE
                      WHERE id_trasm_singola = currenttrasmsingola.system_id
                        AND id_people_dest = p_idpeople
                        AND id_profile = p_idoggetto;
                  EXCEPTION
                     WHEN OTHERS
                     THEN
                        p_resultvalue := 1;
                        RETURN;
                  END;

                  BEGIN
                     IF (    currenttrasmsingola.cha_tipo_trasm = 'S'
                         AND currenttrasmsingola.cha_tipo_dest = 'R'
                        )
                     THEN
-- se ? una trasmissione ? di tipo SINGOLA a un RUOLO allora devo aggiornare
-- anche le trasmissioni singole relative agli altri utenti del ruolo
                        IF (p_iddelegato = 0)
                        THEN
                           BEGIN
-- nelle trasmissioni utente relative agli altri utenti del ruolo
-- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList
                              UPDATE dpa_trasm_utente dpa_trasm_utente
                                 SET dpa_trasm_utente.cha_vista = '1'--,
                                   --  dpa_trasm_utente.cha_in_todolist = '0'
                               WHERE dpa_trasm_utente.dta_vista IS NULL
                                 AND id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                                 AND dpa_trasm_utente.id_people != p_idpeople
                                 AND EXISTS (
                                        SELECT 'x'
                                          FROM dpa_trasm_utente a
                                         WHERE a.id_trasm_singola =
                                                  dpa_trasm_utente.id_trasm_singola
                                           AND a.id_people = p_idpeople);
                                  --ovvero solo se io sono tra i notificati!!!
                           EXCEPTION
                              WHEN OTHERS
                              THEN
                                 p_resultvalue := 1;
                                 RETURN;
                           END;
                        ELSE
                           BEGIN
-- nelle trasmissioni utente relative agli altri utenti del ruolo
-- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList
                              UPDATE dpa_trasm_utente dpa_trasm_utente
                                 SET dpa_trasm_utente.cha_vista = '1',
                                     dpa_trasm_utente.cha_vista_delegato = '1',
                                     dpa_trasm_utente.id_people_delegato =
                                                                  p_iddelegato--,
                                    -- dpa_trasm_utente.cha_in_todolist = '0'
                               WHERE dpa_trasm_utente.dta_vista IS NULL
                                 AND id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                                 AND dpa_trasm_utente.id_people != p_idpeople
                                 AND EXISTS (
                                        SELECT 'x'
                                          FROM dpa_trasm_utente a
                                         WHERE a.id_trasm_singola =
                                                  dpa_trasm_utente.id_trasm_singola
                                           AND a.id_people = p_idpeople);
                                  --ovvero solo se io sono tra i notificati!!!
                           EXCEPTION
                              WHEN OTHERS
                              THEN
                                 p_resultvalue := 1;
                                 RETURN;
                           END;
                        END IF;
                     END IF;
                  END;
               ELSE
-- la ragione di trasmissione prevede workflow
                  IF (p_iddelegato = 0)
                  THEN
                     BEGIN
                        UPDATE dpa_trasm_utente
                           SET dpa_trasm_utente.cha_vista = '1',
                               dpa_trasm_utente.dta_vista =
                                  (CASE
                                      WHEN dta_vista IS NULL
                                         THEN SYSDATE
                                      ELSE dta_vista
                                   END
                                  )
                         WHERE dpa_trasm_utente.dta_vista IS NULL
                           AND id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                           AND dpa_trasm_utente.id_people = p_idpeople;
                     EXCEPTION
                        WHEN OTHERS
                        THEN
                           p_resultvalue := 1;
                           RETURN;
                     END;
                  ELSE
--in caso di delega
                     BEGIN
                        UPDATE dpa_trasm_utente
                           SET dpa_trasm_utente.cha_vista = '1',
                               dpa_trasm_utente.cha_vista_delegato = '1',
                               dpa_trasm_utente.id_people_delegato =
                                                                  p_iddelegato,
                               dpa_trasm_utente.dta_vista =
                                  (CASE
                                      WHEN dta_vista IS NULL
                                         THEN SYSDATE
                                      ELSE dta_vista
                                   END
                                  )
                         WHERE dpa_trasm_utente.dta_vista IS NULL
                           AND id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                           AND dpa_trasm_utente.id_people = p_idpeople;
                     EXCEPTION
                        WHEN OTHERS
                        THEN
                           p_resultvalue := 1;
                           RETURN;
                     END;
                  END IF;

                  BEGIN
-- Rimozione trasmissione da todolist solo se ? stata gi? accettata o rifiutata
                     UPDATE dpa_trasm_utente
                        SET cha_in_todolist = '0'
                      WHERE id_trasm_singola = currenttrasmsingola.system_id
                        AND NOT dpa_trasm_utente.dta_vista IS NULL
                        AND (cha_accettata = '1' OR cha_rifiutata = '1');

--AND dpa_trasm_utente.id_people = p_idpeople;
                     UPDATE dpa_todolist
                        SET dta_vista = SYSDATE
                      WHERE id_trasm_singola = currenttrasmsingola.system_id
                        AND id_people_dest = p_idpeople
                        AND id_profile = p_idoggetto;
                  EXCEPTION
                     WHEN OTHERS
                     THEN
                        p_resultvalue := 1;
                        RETURN;
                  END;
               END IF;
            END;
         END LOOP;
      END IF;

      IF (p_tipooggetto = 'F')
      THEN
         FOR currenttrasmsingola IN cursortrasmsingolafascicolo
         LOOP
            BEGIN
               IF (   currenttrasmsingola.cha_tipo_ragione = 'N'
                   OR currenttrasmsingola.cha_tipo_ragione = 'I'
-- modifica della 3.21 by S. Furnari
                   OR currenttrasmsingola.cha_tipo_ragione = 'S' -- Interoperabilit semplificata
                  )
               THEN
-- SE ? una trasmissione senza workFlow
                  IF (p_iddelegato = 0)
                  THEN
                     BEGIN
-- nella trasmissione utente relativa all'utente che sta vedendo il documento
-- setto la data di vista
                        UPDATE dpa_trasm_utente
                           SET dpa_trasm_utente.cha_vista = '1',
                               dpa_trasm_utente.dta_vista =
                                  (CASE
                                      WHEN dta_vista IS NULL
                                         THEN SYSDATE
                                      ELSE dta_vista
                                   END
                                  )--,
                               --dpa_trasm_utente.cha_in_todolist = '0'
                         WHERE dpa_trasm_utente.dta_vista IS NULL
                           AND id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                           AND dpa_trasm_utente.id_people = p_idpeople;
                     EXCEPTION
                        WHEN OTHERS
                        THEN
                           p_resultvalue := 1;
                           RETURN;
                     END;
                  ELSE
--caso in cui si sta esercitando una delega
                     BEGIN
-- nella trasmissione utente relativa all'utente che sta vedendo il documento
-- setto la data di vista
                        UPDATE dpa_trasm_utente
                           SET dpa_trasm_utente.cha_vista = '1',
                               dpa_trasm_utente.cha_vista_delegato = '1',
                               dpa_trasm_utente.id_people_delegato =
                                                                  p_iddelegato,
                               dpa_trasm_utente.dta_vista =
                                  (CASE
                                      WHEN dta_vista IS NULL
                                         THEN SYSDATE
                                      ELSE dta_vista
                                   END
                                  )--,
                              -- dpa_trasm_utente.cha_in_todolist = '0'
                         WHERE dpa_trasm_utente.dta_vista IS NULL
                           AND id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                           AND dpa_trasm_utente.id_people = p_idpeople;
                     EXCEPTION
                        WHEN OTHERS
                        THEN
                           p_resultvalue := 1;
                           RETURN;
                     END;
                  END IF;

                  BEGIN
                     UPDATE dpa_todolist
                        SET dta_vista = SYSDATE
                      WHERE id_trasm_singola = currenttrasmsingola.system_id
                        AND id_people_dest = p_idpeople
--and ID_RUOLO_DEST in (p_idGruppo,0)
                        AND id_project = p_idoggetto;
                  EXCEPTION
                     WHEN OTHERS
                     THEN
                        p_resultvalue := 1;
                        RETURN;
                  END;

                  BEGIN
                     IF (    currenttrasmsingola.cha_tipo_trasm = 'S'
                         AND currenttrasmsingola.cha_tipo_dest = 'R'
                        )
                     THEN
                        IF (p_iddelegato = 0)
                        THEN
-- se ? una trasmissione ? di tipo SINGOLA a un RUOLO allora devo aggiornare
-- anche le trasmissioni singole relative agli altri utenti del ruolo
                           BEGIN
-- nelle trasmissioni utente relative agli altri utenti del ruolo
-- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList
                              UPDATE dpa_trasm_utente dpa_trasm_utente
                                 SET dpa_trasm_utente.cha_vista = '1'--,
                                    -- dpa_trasm_utente.cha_in_todolist = '0'
                               WHERE dpa_trasm_utente.dta_vista IS NULL
                                 AND id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                                 AND dpa_trasm_utente.id_people != p_idpeople
                                 AND EXISTS (
                                        SELECT 'x'
                                          FROM dpa_trasm_utente a
                                         WHERE a.id_trasm_singola =
                                                  dpa_trasm_utente.id_trasm_singola
                                           AND a.id_people = p_idpeople);
                                  --ovvero solo se io sono tra i notificati!!!
                           EXCEPTION
                              WHEN OTHERS
                              THEN
                                 p_resultvalue := 1;
                                 RETURN;
                           END;
                        ELSE
                           BEGIN
-- nelle trasmissioni utente relative agli altri utenti del ruolo
-- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList
                              UPDATE dpa_trasm_utente dpa_trasm_utente
                                 SET dpa_trasm_utente.cha_vista = '1',
                                     dpa_trasm_utente.cha_vista_delegato = '1',
                                     dpa_trasm_utente.id_people_delegato =
                                                                  p_iddelegato--,
                                    -- dpa_trasm_utente.cha_in_todolist = '0'
                               WHERE dpa_trasm_utente.dta_vista IS NULL
                                 AND id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                                 AND dpa_trasm_utente.id_people != p_idpeople
                                 AND EXISTS (
                                        SELECT 'x'
                                          FROM dpa_trasm_utente a
                                         WHERE a.id_trasm_singola =
                                                  dpa_trasm_utente.id_trasm_singola
                                           AND a.id_people = p_idpeople);
                                  --ovvero solo se io sono tra i notificati!!!
                           EXCEPTION
                              WHEN OTHERS
                              THEN
                                 p_resultvalue := 1;
                                 RETURN;
                           END;
                        END IF;
                     END IF;
                  END;
               ELSE
-- se la ragione di trasmissione prevede workflow
                  IF (p_iddelegato = 0)
                  THEN
                     BEGIN
                        UPDATE dpa_trasm_utente
                           SET dpa_trasm_utente.cha_vista = '1',
                               dpa_trasm_utente.dta_vista =
                                  (CASE
                                      WHEN dta_vista IS NULL
                                         THEN SYSDATE
                                      ELSE dta_vista
                                   END
                                  )
                         WHERE dpa_trasm_utente.dta_vista IS NULL
                           AND id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                           AND dpa_trasm_utente.id_people = p_idpeople;
                     EXCEPTION
                        WHEN OTHERS
                        THEN
                           p_resultvalue := 1;
                           RETURN;
                     END;
                  ELSE
-- caso in cui si sta esercitando una delega
                     BEGIN
                        UPDATE dpa_trasm_utente
                           SET dpa_trasm_utente.cha_vista = '1',
                               dpa_trasm_utente.cha_vista_delegato = '1',
                               dpa_trasm_utente.id_people_delegato =
                                                                  p_iddelegato,
                               dpa_trasm_utente.dta_vista =
                                  (CASE
                                      WHEN dta_vista IS NULL
                                         THEN SYSDATE
                                      ELSE dta_vista
                                   END
                                  )
                         WHERE dpa_trasm_utente.dta_vista IS NULL
                           AND id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                           AND dpa_trasm_utente.id_people = p_idpeople;
                     EXCEPTION
                        WHEN OTHERS
                        THEN
                           p_resultvalue := 1;
                           RETURN;
                     END;
                  END IF;

                  BEGIN
-- Rimozione trasmissione da todolist solo se ? stata gi? accettata o rifiutata
                     UPDATE dpa_trasm_utente
                        SET cha_in_todolist = '0'
                      WHERE id_trasm_singola = currenttrasmsingola.system_id
                        AND NOT dpa_trasm_utente.dta_vista IS NULL
                        AND (cha_accettata = '1' OR cha_rifiutata = '1')
                        AND dpa_trasm_utente.id_people = p_idpeople;

                     UPDATE dpa_todolist
                        SET dta_vista = SYSDATE
                      WHERE id_trasm_singola = currenttrasmsingola.system_id
                        AND id_people_dest = p_idpeople
--and ID_RUOLO_DEST in (p_idGruppo,0)
                        AND id_project = p_idoggetto;
                  EXCEPTION
                     WHEN OTHERS
                     THEN
                        p_resultvalue := 1;
                        RETURN;
                  END;
               END IF;
            END;
         END LOOP;
      END IF;
   END;
END spsetdatavista_v2;
/
              
----------- FINE -
              
---- execute_only_ONCE_INITPEC3.ORA.sql  marcatore per ricerca ----
/*
Questa SP inizializza le tabelle coinvolte nello sviluppo PEC3 by C. Ferlito 
E' eseguito SOLO una volta, in INIT_PEC3.ORA.sql, nel folder INSERT_UPDATE_DATA

*/


create or replace
PROCEDURE INITPEC3 is
--DECLARE
  cntinteropinterna                        NUMBER(10,0);
  system_id              NUMBER(10,0);
  var_email_registro     VARCHAR2(128 BYTE);
  var_user_mail          VARCHAR2(128 BYTE);
  var_pwd_mail           VARCHAR2(64 BYTE);
  var_server_smtp        VARCHAR2(64 BYTE);
  num_porta_smtp         NUMBER(10,0);
  var_server_pop         VARCHAR2(64 BYTE);
  num_porta_pop          NUMBER(10,0);
  var_user_smtp          VARCHAR2(128 BYTE);
  var_pwd_smtp           VARCHAR2(128 BYTE);
  cha_smtp_ssl           VARCHAR2(1 BYTE);
  cha_pop_ssl            VARCHAR2(1 BYTE);
  cha_smtp_sta           VARCHAR2(1 BYTE);
  var_server_imap        VARCHAR2(128 BYTE);
  num_porta_imap         NUMBER(10,0);
  var_tipo_connessione   VARCHAR2(10 BYTE);
  var_inbox_imap         VARCHAR2(128 BYTE);
  var_box_mail_elaborate VARCHAR2(50 BYTE);
  var_mail_non_elaborate VARCHAR2(50 BYTE);
  cha_imap_ssl           VARCHAR2(1 BYTE);
  cha_ricevuta_pec       VARCHAR2(2 BYTE);
  var_solo_mail_pec      VARCHAR2(1 BYTE);
  cha_consulta           VARCHAR2(1 BYTE);
  cha_spedisci           VARCHAR2(1 BYTE);
  cha_notifica           VARCHAR2(1 BYTE);
  system_id_reg          NUMBER(10,0);
  id_ruolo               NUMBER(10,0);
  id_amm                 NUMBER(10,0);
  email                  VARCHAR2(128 BYTE);
  cha_rf                 VARCHAR2(1 BYTE);
  --per dpa_ass_doc_mail_interop
  id_profile NUMBER(10,0);
  var_email  VARCHAR2(128 BYTE);
  id_reg     NUMBER(10,0);
  --per dpa_mail_corr_esterni
  id_corr_esterno        NUMBER(10,0);
  var_email_corr_esterno VARCHAR2(128 BYTE);
  --cursore per popolare DPA_MAIL_REGISTRI
  CURSOR INFO_MAIL_REG
  IS
    SELECT SYSTEM_ID ,
      VAR_EMAIL_REGISTRO ,
      VAR_USER_MAIL ,
      VAR_PWD_MAIL ,
      VAR_SERVER_SMTP ,
      NUM_PORTA_SMTP ,
      VAR_SERVER_POP ,
      NUM_PORTA_POP ,
      VAR_USER_SMTP ,
      VAR_PWD_SMTP ,
      CHA_SMTP_SSL ,
      CHA_POP_SSL ,
      CHA_SMTP_STA ,
      VAR_SERVER_IMAP ,
      NUM_PORTA_IMAP ,
      VAR_TIPO_CONNESSIONE ,
      VAR_INBOX_IMAP ,
      VAR_BOX_MAIL_ELABORATE ,
      VAR_MAIL_NON_ELABORATE ,
      CHA_IMAP_SSL ,
      CHA_RICEVUTA_PEC ,
      VAR_SOLO_MAIL_PEC
    FROM DPA_EL_REGISTRI
    WHERE VAR_EMAIL_REGISTRO IS NOT NULL
    	UNION
		SELECT SYSTEM_ID ,
		  NULL ,
		  NULL ,
		  NULL ,
		  NULL ,
		  0 ,
		  NULL ,
		  0 ,
		  NULL ,
		  NULL ,
		  NULL ,
		  NULL ,
		  NULL ,
		  NULL ,
		  0 ,
		  NULL ,
		  NULL ,
		  NULL ,
		  NULL ,
		  NULL ,
		  NULL ,
		  NULL
		FROM DPA_EL_REGISTRI;
  -- cursore per popolare DPA_VIS_MAIL_REGISTRI
  CURSOR INFO_VIS_REG
  IS
    SELECT rr.id_registro ,
      rr.id_ruolo_in_uo ,
      el.id_amm ,
      el.var_email_registro ,
      el.cha_rf
    FROM DPA_L_RUOLO_REG rr,
      DPA_EL_REGISTRI el
    WHERE rr.id_registro       = el.system_id
    AND el.var_email_registro IS NOT NULL;
  --cursore per popolare DPA_ASS_DOC_MAIL_INTEROP
  CURSOR ASS_DOC_MAIL
  IS
    SELECT p.docnumber,
      r.var_email_registro,
      r.system_id
    FROM profile p,
      dpa_doc_arrivo_par d,
      dpa_corr_globali c,
      dpa_el_registri r
    WHERE p.cha_interop      = 'S'
    AND p.docnumber          = d.id_profile
    AND d.cha_tipo_mitt_dest = 'M'
    AND d.id_mitt_dest       = c.system_id
    AND c.id_registro        = r.system_id;
  --cursore per DPA_MAIL_CORR_ESTERNI
  CURSOR MAIL_CORR_ESTERNI
  IS
    SELECT system_id,
      var_email
    FROM DPA_CORR_GLOBALI
    WHERE CHA_TIPO_IE = 'E'
    AND var_email    IS NOT NULL
    AND var_email    <>' ';

BEGIN
  -- Popola la DPA_MAIL_REGISTRI
  OPEN INFO_MAIL_REG;
  LOOP
    FETCH INFO_MAIL_REG
    INTO system_id ,
      var_email_registro ,
      var_user_mail ,
      var_pwd_mail ,
      var_server_smtp ,
      num_porta_smtp ,
      var_server_pop ,
      num_porta_pop ,
      var_user_smtp ,
      var_pwd_smtp ,
      cha_smtp_ssl ,
      cha_pop_ssl ,
      cha_smtp_sta ,
      var_server_imap ,
      num_porta_imap ,
      var_tipo_connessione ,
      var_inbox_imap ,
      var_box_mail_elaborate ,
      var_mail_non_elaborate ,
      cha_imap_ssl ,
      cha_ricevuta_pec ,
      var_solo_mail_pec;
   EXIT when INFO_MAIL_REG%NOTFOUND ;
    INSERT    INTO DPA_MAIL_REGISTRI VALUES
      (        SEQ_DPA_MAIL_REGISTRI.nextval ,
        system_id ,
        '1' ,
        var_email_registro ,
        var_user_mail ,
        var_pwd_mail ,
        var_server_smtp ,
        cha_smtp_ssl ,
        cha_pop_ssl ,
        num_porta_smtp ,
        cha_smtp_sta ,
        var_server_pop ,
        num_porta_pop ,
        var_user_smtp ,
        var_pwd_smtp ,
        var_inbox_imap ,
        var_server_imap ,
        num_porta_imap ,
        var_tipo_connessione ,
        var_box_mail_elaborate ,
        var_mail_non_elaborate ,
        cha_imap_ssl ,
        var_solo_mail_pec ,
        cha_ricevuta_pec,
        ''
      );
    
  END LOOP;
  CLOSE INFO_MAIL_REG;

dbms_output.put_line('finito insert DPA_MAIL_REGISTRI ');
 
 
 
 
 
  --Popolo la DPA_VIS_MAIL_REGISTRI
  OPEN INFO_VIS_REG;
  LOOP
    FETCH	  INFO_VIS_REG INTO system_id_reg ,id_ruolo ,id_amm ,email ,cha_rf;
	EXIT WHEN INFO_VIS_REG%NOTFOUND ;

    cha_consulta := '0';
    cha_spedisci := '0';
    cha_notifica := '0';
    --aggiorna flag notifica per fegistro/rf
    IF cha_rf = '1' THEN
      SELECT COUNT(*)
      INTO cha_notifica
      FROM dpa_tipo_f_ruolo fr,
        dpa_tipo_funzione tf
      WHERE fr.id_ruolo_in_uo    = id_ruolo
      AND fr.id_tipo_funz        = tf.system_id
      AND UPPER(tf.var_cod_tipo) = 'PRAU_RF';
    ELSE
      SELECT COUNT(*)
      INTO cha_notifica
      FROM dpa_tipo_f_ruolo fr,
        dpa_tipo_funzione tf
      WHERE fr.id_ruolo_in_uo    = id_ruolo
      And Fr.Id_Tipo_Funz        = Tf.System_Id
      AND UPPER(tf.var_cod_tipo) = 'PRAU';
    End If;
    -- chiude
    
    
    IF cha_notifica > 1 THEN
      cha_notifica := 1;
    END IF;

    --aggiorna flag spedisci per fegistro/rf
    SELECT COUNT(*)
    INTO cha_spedisci
    FROM dpa_tipo_f_ruolo fr,
      dpa_tipo_funzione tf,
      dpa_funzioni f
    WHERE fr.id_ruolo_in_uo    = id_ruolo
    AND fr.id_tipo_funz        = tf.system_id
    AND  tf.system_id = f.id_tipo_funzione
    AND f.cod_funzione      = 'DO_OUT_SPEDISCI';

	IF cha_spedisci > 1 THEN
      cha_spedisci := 1;
    END IF;

    --aggiorna flag consulta per fegistro/rf
    IF cha_rf = '1' THEN
      SELECT COUNT(*)
      INTO cha_consulta
      FROM dpa_tipo_f_ruolo fr,
        dpa_tipo_funzione tf,
        dpa_funzioni f
      WHERE fr.id_ruolo_in_uo = id_ruolo
      AND fr.id_tipo_funz     = tf.system_id
      AND tf.system_id        = f.id_tipo_funzione
      AND f.cod_funzione      = 'GEST_CASELLA_IST_RF';
    ELSE
      SELECT COUNT(*)
      INTO cha_consulta
      FROM dpa_tipo_f_ruolo fr,
        dpa_tipo_funzione tf,
        dpa_funzioni f
      WHERE fr.id_ruolo_in_uo = id_ruolo
      AND fr.id_tipo_funz     = tf.system_id
      AND tf.system_id        = f.id_tipo_funzione
      AND f.cod_funzione      = 'GEST_CASELLA_IST';
    END IF;
    
    
    IF cha_consulta > 1 THEN
      cha_consulta := 1;
    END IF;
    
    
    INSERT
    INTO DPA_VIS_MAIL_REGISTRI VALUES
      (
        SEQ_DPA_VIS_MAIL_REGISTRI.nextval ,
        system_id_reg ,
        id_ruolo ,
        email ,
        cha_consulta ,
        cha_notifica,
		cha_spedisci
        );




          SELECT COUNT(*) into cntinteropinterna
                FROM DPA_VIS_MAIL_REGISTRI WHERE ID_REGISTRO = system_id_reg
	             AND ID_RUOLO_IN_UO = id_ruolo
		     AND VAR_EMAIL_REGISTRO is not null ;

        	If Cntinteropinterna = 1 Then
         If  System_Id_Reg = 1110 Then Dbms_Output.Put_Line('cntinteropinterna =1  Id_Ruolo:'||Id_Ruolo);
         end if;
					Insert Into Dpa_Vis_Mail_Registri
						Values( Seq_Dpa_Vis_Mail_Registri.Nextval ,
              system_id_reg ,
						    Id_Ruolo ,
								'' ,-- 4° campo mail
								Cha_Consulta ,
								Cha_Notifica ,
                Cha_Spedisci								);
               --  If  System_Id_Reg = 1110 Then Dbms_Output.Put_Line('cntinteropinterna =1  insert done for Id_Ruolo :'||Id_Ruolo);
         --end if;
        END IF;





  END LOOP;
  Close Info_Vis_Reg;
dbms_output.put_line('finito insert DPA_VIS_MAIL_REGISTRI ');

  OPEN ASS_DOC_MAIL;
  LOOP
    FETCH ASS_DOC_MAIL INTO id_profile, var_email, id_reg;
    EXIT WHEN ASS_DOC_MAIL%NOTFOUND ;

	INSERT    INTO DPA_ASS_DOC_MAIL_INTEROP VALUES
      (        SEQ_DPA_ASS_DOC_MAIL_INTEROP.nextval ,
        id_profile ,
        id_reg ,
        var_email      );

  END LOOP;
  Close Ass_Doc_Mail;
  dbms_output.put_line('finito insert DPA_ASS_DOC_MAIL_INTEROP ');
  
  OPEN MAIL_CORR_ESTERNI;
  LOOP
    FETCH MAIL_CORR_ESTERNI INTO id_corr_esterno, var_email_corr_esterno;
    EXIT WHEN MAIL_CORR_ESTERNI%NOTFOUND ;
    
	INSERT
    INTO    DPA_MAIL_CORR_ESTERNI VALUES
      ( SEQ_DPA_MAIL_CORR_ESTERNI.nextval ,
        id_corr_esterno ,
        var_email_corr_esterno,
        '1',
        ''
      );

  END LOOP;
  CLOSE MAIL_CORR_ESTERNI;
  dbms_output.put_line('finito insert DPA_MAIL_CORR_ESTERNI ');
  Commit;
Dbms_Output.Put_Line('fatto  commit ');

    Exception When Others Then Rollback; Raise;
    dbms_output.put_line('other'||SQLERRM);
END;
/


begin
declare cnt			int; 
 cntmail		int; 
begin
	select count(*) into cnt 
	from DPA_DOCSPA 
	where ID_VERSIONS_U like '3.21%' OR ID_VERSIONS_U like '3.22%'; 

	select count(*) into cntmail	
	from DPA_MAIL_REGISTRI	; 



if (cnt + cntmail) = 0 then 
	-- impostare a NULL il canale preferenziale per i corrispondenti interni
	UPDATE @db_user.DPA_T_CANALE_CORR
		SET ID_DOCUMENTTYPE = NULL 
		WHERE ID_CORR_GLOBALE IN (
			SELECT C.SYSTEM_ID
			FROM DPA_CORR_GLOBALI C, DPA_T_CANALE_CORR B, DOCUMENTTYPES T
			WHERE C.SYSTEM_ID = B.ID_CORR_GLOBALE AND
			B.ID_DOCUMENTTYPE = T.SYSTEM_ID AND
			UPPER(C.CHA_TIPO_IE) = 'I'
			);

	-- esegui INITPEC3
	INITPEC3; 

end if; 
End;
End;
/


-- RESET
--/* Begin 
--  Delete Dpa_Mail_Registri ;
--  Delete Dpa_Vis_Mail_Registri ;
--  Delete Dpa_Ass_Doc_Mail_Interop ; 
--  Delete Dpa_Mail_Corr_Esterni ;
--End;
--/

              
----------- FINE -
              
---- package_NotificationCenter.ORA.sql  marcatore per ricerca ----
create or replace PACKAGE NC AS 
  
  TYPE T_CURSOR IS REF Cursor; 
  
  -- Procedura per l'inserimento di un nuovo canale
  Procedure InsertChannel(p_Label NVarChar2, p_Description VarChar2);
  
  -- Procedura per il reperimento dei canali
  Procedure LoadChannels(channels Out t_Cursor);
  
  -- Procedura per il reperimento di un canale a partire dalla sua etichetta
  Procedure LoadChannelByLabel(p_LabelToSearch NVarChar2, p_Id Out Integer, p_Label Out NVarChar2, p_Description Out NVarChar2);
  
  -- Procedura per l'inserimento di una nuova istanza
  Procedure InsertInstance(p_Description NVarChar2);
  
  -- Procedura per il recupero delle istanze
  Procedure LoadInstances(Inst Out t_Cursor);
  
  -- Procedura per il recupero di istanze a partire dal nome
  Procedure SearchInstancesByName(p_Name NVarChar2, Inst Out t_Cursor);
  
  -- Procedura per il salvataggio delle impostazioni relative ad un canale
  Procedure AssociateChannelToInstance(p_ChannelId Integer, p_InstanceId Integer);
  
  -- Caricamento delle impostazioni relative ad un canale
  Procedure LoadChannelsRelatedToInstance(p_InstanceId Number, Channels Out t_Cursor);
  
  -- Inserimento di un item nella lista degli item
  Procedure InsertItem(p_Author NVarchar2, p_Title NVarChar2, p_Text NVarChar2, p_ChannelId Number, p_MessageId Number, p_MessageNumber Integer, p_ItemId Out Integer);
  
  -- Associazione di un item ad una categoria
  Procedure AssociateItemToChannel(p_ItemId Number, p_ChannelId Integer);
  
  -- Recupero delle informazioni su di un item
  Procedure LoadItem(p_ItemId Integer, p_Author Out NVarchar2, p_Title Out NVarChar2, p_Text Out NVarChar2, p_LastUpdate Out Date, p_PublishDate Out Date, p_MessageId Out Number, p_MessageNumber Out Number);
  
  -- Recupero dei canali associati ad un item
  Procedure LoadChannelsRelatedToItem(p_ItemId Number, Channel Out T_Cursor);
  
  -- Salvatggio del riferimento ad un id di utente che deve visualizzare una notifica
  Procedure InsertUser(p_UserId Number, p_ItemId Number, p_InstanceId Number);
  
  -- Impostazione della data di visualizzazione di una notifica relativa ad uno specifico utente
  Procedure SetItemViewed(p_ItemId Integer, p_UserId Integer, p_InstanceId Integer);
  
  -- Ricerca di item per Canale
  Procedure SearchItemByChannelId(p_ChannelId Number, p_InstanceId Number, p_UserId Integer, Items Out T_Cursor);
  
  -- Ricerca di item per intervallo di date di ricezione
  Procedure SearchItemByDateRange(p_LowDate Date, p_HightDate Date, p_InstanceId Integer, p_UserId Integer, Items Out T_Cursor);
  
  -- Ricerca di item per intervallo di id
  Procedure SearchItemByMessageIdRange(p_LowMessageId Integer, p_HightMessageId Integer, p_InstanceId Number, p_UserId Integer, Items Out T_Cursor);
  
  -- Ricerca degli item che devono ancora essere visualizzati da un utente relativamente ad una specifica categoria
  Procedure SearchItemsNotViewedByUser(p_UserId Integer, p_ChannelId Integer, p_PageSize Integer, p_PageNumber Integer, p_Count Out Integer, p_InstanceId Number, Item Out T_Cursor);
  
  -- Conteggio degli item che devono ancora essere visualizzati da un utente
  Procedure CountNotViewedItems(p_UserId Integer, p_InstanceId Integer, p_Count Out Integer);
  
  -- Ricerca di item con filtri su data, id messaggio, testo contenuto nella notifica
  Procedure SearchItem(p_UserId Integer, p_SearchForMessageNumber Integer, p_LowMessageNumber Integer, p_HightMessageNumber Integer, 
    p_SearchForDate Number, p_LowDate Date, p_HightDate Date, 
    p_SearchForTitle Integer, p_ItemText VarChar2, p_InstanceId Integer,
    Items Out T_Cursor);
    
  -- Pulizia delle tabelle legate al centro notifiche
  Procedure CleanData;
  
END NC;
/

create or replace Package Body NC AS

-- Procedura per l'inserimento di un nuovo canale
  Procedure InsertChannel(p_Label NVarChar2, p_Description VarChar2) Is Begin
    
    INSERT
    INTO NOTIFICATIONCHANNEL
      (
        ID,
        LABEL,
        DESCRIPTION
      )
      VALUES
      (
        SEQ_NOTIFICATIONCHANNEL.nextVal,
        p_Label,
        p_Description
      );

  End InsertChannel;

  -- Procedura per il reperimento dei canali
  Procedure LoadChannels(channels Out t_Cursor) Is Begin
    Open channels For 'Select * From NotificationChannel';
  
  End LoadChannels;
  
  -- Procedura per il reperimento di un canale a partire dalla sua etichetta
  Procedure LoadChannelByLabel(p_LabelToSearch NVarChar2, p_Id Out Integer, p_Label Out NVarChar2, p_Description Out NVarChar2) Is Begin
    Select Id, Label, Description Into p_Id, p_Label, p_Description From NotificationChannel Where Label = p_LabelToSearch;
  End LoadChannelByLabel;
  
  -- Procedura per l'inserimento di una nuova istanza
  Procedure InsertInstance(p_Description NVarChar2) Is Begin
    INSERT INTO NOTIFICATIONINSTANCE
    (ID, DESCRIPTION
    ) VALUES
    (
      SEQ_NOTIFICATIONINSTANCE.Nextval, 
      p_Description
    );
    
  End InsertInstance;
  
  -- Procedura per il recupero delle istanze
  Procedure LoadInstances(Inst Out t_Cursor) Is Begin
    Open Inst For Select * From NotificationInstance;
    
  End LoadInstances;
  
  -- Procedura per il recupero di istanze a partire dal nome
  Procedure SearchInstancesByName(p_Name NVarChar2, Inst Out t_Cursor) Is Begin
    Open Inst For Select * From NotificationInstance Where Upper(Description) = Upper(p_Name);
  
  End SearchInstancesByName;
  
  -- Procedura per l'associazione di un canale ad una istanza
  Procedure AssociateChannelToInstance(p_ChannelId Integer, p_InstanceId Integer) Is Begin
    Declare exist Number;
    
    Begin
   
    -- L'associazione fra canale e istanza viene fatta solo se non esiste gi
    Select Count(*) Into exist From NotificationInstanceChannels Where InstanceId = p_InstanceId And ChannelId = p_ChannelId;
    
    If exist = 0 Then
      INSERT INTO NotificationInstanceChannels
      (INSTANCEID, CHANNELID
      ) VALUES
      (
        p_InstanceId,
        p_ChannelId
      );
    End If;
    End;
  End AssociateChannelToInstance;
  
  -- Caricamento dei canali associati ad una istanza
  Procedure LoadChannelsRelatedToInstance(p_InstanceId Number, Channels Out t_Cursor) Is Begin
  
    Open Channels For Select * From NotificationChannel Where Exists (Select 'x' From NotificationInstanceChannels Where InstanceId = p_InstanceId);
  End LoadChannelsRelatedToInstance;

  -- Inserimento di un item nella lista degli item
  Procedure InsertItem(p_Author NVarchar2, p_Title NVarChar2, p_Text NVarChar2, p_ChannelId Number, p_MessageId Number, p_MessageNumber Integer, p_ItemId Out Integer) Is Begin
  
    Begin
    
    INSERT
    INTO NOTIFICATIONITEM
      (
        ID,
        AUTHOR,
        TITLE,
        TEXT,
        LASTUPDATE,
        PUBLISHDATE,
        MESSAGEID,
        MessageNumber
      )
      VALUES
      (
        SEQ_NOTIFICATIONITEM.nextval,
        p_Author,
        p_Title,
        p_Text,
        sysdate,
        sysdate,
        p_MessageId,
        p_MessageNumber
      );
      
      Select Max(Id) Into p_itemId From NotificationItem;
      
      INSERT INTO NOTIFICATIONITEMCATEGORIES
      (ITEMID, CATEGORYID
      ) VALUES
      (
        p_itemId,
        p_ChannelId
      );
      
      End;
      
  End InsertItem;
  
  -- Associazione di un item ad un canale
  Procedure AssociateItemToChannel(p_ItemId Number, p_ChannelId Integer) Is Begin
    INSERT INTO NOTIFICATIONITEMCATEGORIES
    (ITEMID, CATEGORYID
    ) VALUES
    (
      p_ItemId,
      p_ChannelId
    );

  End AssociateItemToChannel;
  
  -- Recupero delle informazioni su di un item
  Procedure LoadItem(p_ItemId Integer, p_Author Out NVarchar2, p_Title Out NVarChar2, p_Text Out NVarChar2, p_LastUpdate Out Date, p_PublishDate Out Date, p_MessageId Out Number, p_MessageNumber Out Number) Is Begin
    SELECT
      AUTHOR,
      TITLE,
      TEXT,
      LASTUPDATE,
      PUBLISHDATE,
      MESSAGEID,
      MessageNumber Into
      p_Author,
      p_Title,
      p_Text,
      p_LastUpdate,
      p_PublishDate,
      p_MessageId,
      p_MessageNumber
    FROM NOTIFICATIONITEM ;

  End LoadItem;
  
  Procedure LoadChannelsRelatedToItem(p_ItemId Number, Channel Out T_Cursor) Is Begin
    Open Channel For Select * From NotificationChannel Where Exists (Select 'x' From NotificationItemCategories Where NotificationChannel.Id = NotificationItemCategories.CategoryId And NotificationItemCategories.ItemId = p_ItemId);
    
  End LoadChannelsRelatedToItem;
  
  -- Salvatggio del riferimento ad un id di utente di una data istanza che deve visualizzare una notifica
  Procedure InsertUser(p_UserId Number, p_ItemId Number, p_InstanceId Number) Is Begin
    INSERT INTO NOTIFICATIONUSER
    (ITEMID, USERID, InstanceId
    ) VALUES
    (
      p_ItemId,
      p_UserId,
      p_InstanceId
    );

  End InsertUser;
  
  -- Impostazione della data di visualizzazione di una notifica relativa ad uno specifico utente
  Procedure SetItemViewed(p_ItemId Integer, p_UserId Integer, p_InstanceId Integer) Is Begin
    Update NOTIFICATIONUSER
    Set ViewDate = sysdate
    Where ItemId = p_ItemId And UserId = p_UserId And InstanceId = p_InstanceId; 
  End SetItemViewed;
  
  -- Ricerca di item per Canale e id istanza
  Procedure SearchItemByChannelId(p_ChannelId Number, p_InstanceId Number, p_UserId Integer, Items Out T_Cursor) Is Begin

    Open Items For Select *
                    From NotificationItem ni
                    Where Exists
                      (Select 'x'
                      From NotificationItemCategories nic
                      Where nic.CategoryId = p_ChannelId
                      And nic.ItemId       = ni.Id
                      And Exists
                        (Select 'x'
                        From NotificationInstanceChannels nic1
                        Where nic1.InstanceId  = p_InstanceId
                        And  nic1.ChannelId = nic.CategoryId
                        And Exists
                        (Select 'x'
                         From NotificationUser nu
                         Where nu.UserId = p_userId And nu.InstanceId = p_InstanceId )))
                      order by ni.PublishDate desc;

  End SearchItemByChannelId;
  
  -- Ricerca di item per intervallo di date di ricezione per una data istanza
  Procedure SearchItemByDateRange(p_LowDate Date, p_HightDate Date, p_InstanceId Integer, p_UserId Integer, Items Out T_Cursor) Is Begin
    Open Items For ' Select * 
        From NotificationItem
        Inner Join NotificationItemCategories
        On Id = ItemId
        Inner Join NotificationInstanceChannels
        On ChannelId = CategoryId
        Where PublishDate >= to_date(''' || to_char(p_LowDate, 'dd/mm/yyyy') || ' 00:00:00'', ''dd/mm/yyyy HH24:mi:ss'')
        And PublishDate <= to_date(''' || to_char(p_HightDate, 'dd/mm/yyyy') || ' 23:59:59'', ''dd/mm/yyyy HH24:mi:ss'')
        And InstanceId = ' || p_InstanceId || ' And Exists 
        (Select ''x'' From NotificationUser nu Where nu.UserId = ' || p_UserId || ' And nu.InstanceId = ' ||
				p_InstanceId || ') order by NotificationItem.PublishDate desc';
    
  End SearchItemByDateRange;

  -- Ricerca di item per intervallo di id
  Procedure SearchItemByMessageIdRange(p_LowMessageId Integer, p_HightMessageId Integer, p_InstanceId Number, p_UserId Integer, Items Out T_Cursor) Is Begin
    Open Items For Select * 
      From NotificationItem 
      Inner Join NotificationItemCategories
      On Id = ItemId
      Inner Join NotificationInstanceChannels
      On ChannelId = CategoryId
      Where MessageId >= p_LowMessageId And MessageId <= p_HightMessageId And InstanceId = p_InstanceId 
      And Exists (Select 'x' From NotificationUser nu Where nu.UserId = p_UserId
      And nu.InstanceId = p_InstanceId)
      Order By NotificationItem.PublishDate desc;

    
  End SearchItemByMessageIdRange;

  -- Ricerca degli item che devono ancora essere visualizzati da un utente relativamente ad una specifica categoria
  Procedure SearchItemsNotViewedByUser(p_UserId Integer, p_ChannelId Integer, p_PageSize Integer, p_PageNumber Integer, p_Count Out Integer, p_InstanceId Number, Item Out T_Cursor) Is Begin
    -- Calcolo degli indici minimo e massimo degli elementi da visualizzare
    Declare lowRowNum Integer;
    hightRowNum Integer;
    
    Begin
      hightRowNum := (p_PageNumber * p_PageSize);
      lowRowNum := (hightRowNum - p_PageSize) + 1;
      
      -- Calcolo dl numero di item totali
      Select count(*) Into p_Count
      From NotificationItem ni
      Inner Join NotificationItemCategories nic
      On ni.Id = nic.ItemId
      Inner Join NotificationUser nu
      On nu.ItemId = ni.Id
      Where nu.ViewDate Is Null And nic.CategoryId =  p_ChannelId And nu.UserId = p_UserId  And nu.InstanceId = p_InstanceId;
      
      Open Item For   Select * 
                      From ( Select /*+ FIRST_ROWS(n) */ 
                      a.*, ROWNUM rnum 
                      From ( Select * 
                      From NotificationItem ni
                      Inner Join NotificationItemCategories nic
                      On ni.Id = nic.ItemId
                      Inner Join NotificationUser nu
                      On nu.ItemId = ni.Id
                      Where nu.ViewDate Is Null And nic.CategoryId = p_ChannelId And nu.UserId = p_UserId
                      And nu.InstanceId = p_InstanceId Order By ni.PublishDate desc) a
                      Where ROWNUM <= hightRowNum )
                      Where rnum  >= lowRowNum;
    End;    
  End SearchItemsNotViewedByUser;
  
    -- Conteggio degli item che devono ancora essere visualizzati da un utente
  Procedure CountNotViewedItems(p_UserId Integer, p_InstanceId Integer, p_Count Out Integer) Is Begin
    Select count(*) Into p_Count
      From NotificationItem ni
      Inner Join NotificationItemCategories nic
      On ni.Id = nic.ItemId
      Inner Join NotificationUser nu
      On nu.ItemId = ni.Id
      Where nu.ViewDate Is Null And nu.UserId = p_UserId And nu.InstanceId = p_InstanceId;
  End CountNotViewedItems;
  
  -- Ricerca di item con filtri su data, id messaggio, testo contenuto nella notifica
  Procedure SearchItem(p_UserId Integer, p_SearchForMessageNumber Integer, p_LowMessageNumber Integer, p_HightMessageNumber Integer, 
    p_SearchForDate Number, p_LowDate Date, p_HightDate Date, 
    p_SearchForTitle Integer, p_ItemText VarChar2, p_InstanceId Integer,
    Items Out T_Cursor) Is Begin
    -- Query da eseguire per effettuare la ricerca 
    Declare queryToExecute VarChar2 (2000) := 'Select ni.* From NotificationItem ni Inner Join NotificationUser nu On ni.Id = nu.ItemId Where nu.UserId = ' || p_UserId || ' And nu.InstanceId = ' || p_InstanceId;
    
    Begin    
    -- Inserimento condizione sull'id del messaggio
    If p_SearchForMessageNumber = 1 Then
      queryToExecute := queryToExecute || ' And MessageNumber >= ' || p_LowMessageNumber;
      
      -- Inserimento condizione sul massimo id se specificato e maggiore del minimo
      If p_HightMessageNumber >= p_LowMessageNumber Then
        queryToExecute := queryToExecute || ' And MessageNumber <= ' || p_HightMessageNumber;
      End If;
      
    End If;
    
    -- Inserimento della condizione per data
    If p_SearchForDate = 1 Then
      queryToExecute := queryToExecute || ' And PublishDate >= to_date(''' || to_char(p_LowDate, 'dd/mm/yyyy') || ' 00:00:00'', ''dd/mm/yyyy HH24:mi:ss'')';
      
      -- Inserimento condizione sul massimo id se specificato e maggiore del minimo
      If p_HightDate >= p_LowDate Then
        queryToExecute := queryToExecute || ' And PublishDate <= to_date(''' || to_char(p_HightDate, 'dd/mm/yyyy') || ' 23:59:59'', ''dd/mm/yyyy HH24:mi:ss'')';
      End If;
      
    End If;
    
    -- Inserimento della condizione sul contenuto dell'item
    If p_SearchForTitle = 1 Then
      queryToExecute := queryToExecute || ' And upper(Title) Like upper(''%' || p_ItemText || '%'')';
    End If;
    
    -- Aggiunta dell'ordinamento per data di pubblicazione
    queryToExecute := queryToExecute || ' Order By ni.PublishDate desc';
    
    Open Items For queryToExecute;
    
    End;
  End SearchItem;
  
  Procedure CleanData Is Begin
      -- Pulizia della tabella degli utenti
    Delete NOTIFICATIONUSER;
    
    -- Pulizia della tabella che lega una istanza ad un canale
    Delete NotificationInstanceChannels;
    
    -- Pulizia della tabella che lega un item ad una categoria
    Delete NOTIFICATIONITEMCATEGORIES;
    
    -- Pulizia della tabella degli item
    Delete NOTIFICATIONITEM;
    
    -- Pulizia della tabella dei canali
    Delete NotificationChannel;
    
    -- Pulizia della tabella delle istanza
    Delete NOTIFICATIONINSTANCE;
  End CleanData;
End NC;
/
              
----------- FINE -
              
---- PACKAGE_UTILITA.ORA.sql  marcatore per ricerca ----
create or replace PACKAGE UTilita authid CURRENT_USER As

PROCEDURE utl_insert_log_w_autcommit ( -- autcommit = autonomous commit --
    nomeutente VARCHAR2,
    data_eseguito DATE,
    comando_eseguito  VARCHAR2,
    CATEGORIA_COMANDO VARCHAR2,
    esito             VARCHAR2) ;

Procedure Utl_Storicizza_Log (Interval_Of_Days Int -- will insert into DPA_LOG_STORICO, delete from DPA_LOG
            --  of oldest record for #Interval_Of_Days days
                                ); 

procedure UTL_OPTIMIZE_CONTEXT_INDEX (
          log_level int -- 0 = disabled, 1= only error messages, 2 = gives total elapsed time, 3 gives elapsed per single index
          , OPTIMIZE_mode VARCHAR2 -- can be 'FULL' or 'FAST'
          );

Procedure Utl_Setsecurityruoloreg ; 
end UTilita;
/

create or replace package body UTilita  as 

Procedure             Utl_Insert_Log_W_Autcommit (Nomeutente Varchar2,
 data_eseguito DATE,
 comando_eseguito VARCHAR2,
 CATEGORIA_COMANDO VARCHAR2,
 esito VARCHAR2)

Is
cnt int;
MYUSERNAME VARCHAR2(200);
UTL_LOG_VARCHAR_DATA_LENGTH INT; 
PRAGMA AUTONOMOUS_TRANSACTION ;

BEGIN
select USERNAME INTO MYUSERNAME from user_users;

SELECT COUNT(*) INTO cnt FROM user_sequences where sequence_name='SEQ_UTL_SYSTEM_LOG';
If (Cnt = 0) Then
        Execute Immediate 'CREATE SEQUENCE SEQ_UTL_SYSTEM_LOG '||
                        'START WITH 1                       '||
                        ' MAXVALUE 99999999999     MINVALUE 1  '||
                        ' NOCYCLE   NOCACHE   NOORDER';
end IF;

SELECT COUNT(*) INTO cnt FROM user_tables where table_name='UTL_SYSTEM_LOG';
IF (cnt = 0) THEN
    RAISE_APPLICATION_ERROR(-20001,'Missing table '||MYUSERNAME||'.UTL_SYSTEM_LOG');
end IF;

select min(data_length) into UTL_LOG_VARCHAR_DATA_LENGTH 
    from cols 
    where table_name = 'UTL_SYSTEM_LOG'
    and column_name in ('COMANDO_RICHIESTO', 'CATEGORIA_COMANDO', 'ESITO_OPERAZIONE') ;  

IF UTL_LOG_VARCHAR_DATA_LENGTH < 2000 THEN 
    EXECUTE IMMEDIATE 'ALTER TABLE UTL_SYSTEM_LOG MODIFY COMANDO_RICHIESTO VARCHAR2(2000) '; 
    EXECUTE IMMEDIATE 'ALTER TABLE UTL_SYSTEM_LOG MODIFY CATEGORIA_COMANDO VARCHAR2(2000) ';
    EXECUTE IMMEDIATE 'ALTER TABLE UTL_SYSTEM_LOG MODIFY ESITO_OPERAZIONE  VARCHAR2(2000) ';
end if; 

 INSERT INTO UTL_SYSTEM_LOG
 ( ID , DATA_OPERAZIONE
  , COMANDO_RICHIESTO
  , CATEGORIA_COMANDO
  , ESITO_OPERAZIONE   )
 VALUES ( SEQ_UTL_SYSTEM_LOG.nextval, nvl(data_eseguito, SYSTIMESTAMP)
 , substr(comando_eseguito  ,1,2000)
 , substr(CATEGORIA_COMANDO ,1,2000)
 , substr(esito             ,1,2000)  ) ;

 commit; -- si pu fare perch c' PRAGMA AUTONOMOUS_TRANSACTION prima in DECLARE
EXCEPTION
 WHEN OTHERS THEN
 DBMS_OUTPUT.put_line ('errore generico in insert log' || SQLERRM);
 RAISE; --manda errore a sp chiamante
End Utl_Insert_Log_W_Autcommit ;


PROCEDURE UTL_STORICIZZA_LOG (interval_of_days int) IS
Pragma Autonomous_Transaction ;
idx_esistente   EXCEPTION;
Pragma Exception_Init (Idx_Esistente  , -955);
-- ORA-01408 esiste gi un indice per questa lista di colonne
Idx_Esistente_su_colonne   Exception;
Pragma Exception_Init (Idx_Esistente_su_colonne  , -01408);

Myusername Varchar2(200);
istruz_sql Varchar2(200);
Is_Compression_Enabled Varchar2(200);
Stringa_Msg   Varchar2(2000);
Min_Dta_Azione_Log         Date; 
Max_Dta_Azione_log_Storico Date; 
dummy varchar2(32); 
Btimestamp Timestamp; secs_elapsed number(8,1);
BEGIN
Select Username Into Myusername From User_Users;
--Select Systimestamp Into Btimestamp From Dual ;

Select 'Prima di aver iniziato esecuzione della procedura, alle '||Systimestamp||', la tabella STORICO ha min dta_azione:'
    ||To_Char(Min_Dta_Azione_S,'dd/mm/yyyy hh24:mi:ss')||' e Max Dta_Azione: ' ||To_Char(Max_Dta_Azione_S,'dd/mm/yyyy hh24:mi:ss')     
    Into Stringa_Msg
From (Select Min(Dta_Azione) As Min_Dta_Azione_S From Dpa_Log_Storico)
   , (Select Max(Dta_Azione) As Max_Dta_Azione_S From Dpa_Log_Storico)   ; 
utl_insert_log_w_autcommit (MYUSERNAME      ,sysdate       ,'estrazione intervallo date da DPA_LOG e DPA_LOG_STORICO' 
    ,'procedura UTL_STORICIZZA_LOG, package UTILITA'             ,stringa_msg     ) ; 

Select 'Prima di aver iniziato esecuzione della procedura, alle '||Systimestamp||', la tabella DPA_LOG ha min dta_azione:'
    ||to_char(Min_Dta_Azione_L,'dd/mm/yyyy hh24:mi:ss')||' e Max Dta_Azione: ' ||to_char(Max_Dta_Azione_L,'dd/mm/yyyy hh24:mi:ss') 
    Into Stringa_Msg
From (Select Min(Dta_Azione) As Min_Dta_Azione_L From Dpa_Log)
   , (Select Max(Dta_Azione) As Max_Dta_Azione_L From Dpa_Log) ; 
utl_insert_log_w_autcommit (MYUSERNAME      ,sysdate       ,'estrazione intervallo date da DPA_LOG e DPA_LOG_STORICO' 
    ,'procedura UTL_STORICIZZA_LOG, package UTILITA'             ,stringa_msg     ) ; 

select compression into is_compression_enabled
    from user_tables 
    where table_name = 'DPA_LOG_STORICO' ;

if is_compression_enabled = 'DISABLED' THEN 
-- il comando abilita solo la compressione per i nuovi record, non la effettua su record esistenti
-- se si volesse comprimere i record esistenti, eseguire alter table DPA_LOG_STORICO move compress; 
    Istruz_Sql := 'alter table '||MYUSERNAME||'.DPA_LOG_STORICO compress storage (buffer_pool recycle)'  ;
    Execute Immediate  Istruz_Sql ;
    utl_insert_log_w_autcommit (      MYUSERNAME        ,Sysdate    
      ,Istruz_Sql --comando_eseguito VARCHAR2,
      ,'procedura UTL_STORICIZZA_LOG, package UTILITA'         --CATEGORIA_COMANDO VARCHAR2,
      ,'abilitata compressione per tabella '||MYUSERNAME||'.DPA_LOG_STORICO' 
      ) ; 
End If; 
  
-- create indexes INDX_DTA_AZIONE on DPA_LOG(DTA_AZIONE) and DPA_LOG_STORICO(DTA_AZIONE) if do not exist
   Begin
      Istruz_Sql := 'create index INDX_DTA_AZIONE on DPA_LOG (DTA_AZIONE) compress storage (buffer_pool recycle)';
      EXECUTE IMMEDIATE  istruz_sql ;
      utl_insert_log_w_autcommit (
            Myusername        --nomeutente VARCHAR2,
            ,Sysdate          --data_eseguito DATE,
            ,Istruz_Sql       --comando_eseguito VARCHAR2,
            ,'procedura UTL_STORICIZZA_LOG, in package UTILITA'         --CATEGORIA_COMANDO VARCHAR2,
            ,'OK'  --esito VARCHAR2
            ) ;
   EXCEPTION
      WHEN idx_esistente       THEN
         Dbms_Output.Put_Line ('indice gi esistente');
      When Idx_Esistente_Su_Colonne Then
         Dbms_Output.Put_Line ('indice gi esistente');
      When others then 
            utl_insert_log_w_autcommit ( Myusername,Sysdate ,Istruz_Sql       
            ,'procedura UTL_STORICIZZA_LOG, in package UTILITA'            ,'KO!'              ) ;
            RAISE;
   End;
-- now with DPA_LOG_STORICO   
   Begin
      Istruz_Sql := 'create index INDX_DTA_AZIONE_STORICO on DPA_LOG_STORICO(DTA_AZIONE) compress storage (buffer_pool recycle)';
      EXECUTE IMMEDIATE  istruz_sql ;
      utl_insert_log_w_autcommit (Myusername,Sysdate,Istruz_Sql       
            ,'procedura UTL_STORICIZZA_LOG, in package UTILITA'       ,'OK') ;
   EXCEPTION
      WHEN idx_esistente       THEN
         Dbms_Output.Put_Line ('indice gi esistente');
      When Idx_Esistente_Su_Colonne Then
         Dbms_Output.Put_Line ('indice gi esistente');
      
      When others then 
           utl_insert_log_w_autcommit ( Myusername,Sysdate ,Istruz_Sql       
            ,'procedura UTL_STORICIZZA_LOG, in package UTILITA'            ,'KO!'              ) ;
             Raise;
   End;
-- end of create indexes on DPA_LOG(DTA_AZIONE) and DPA_LOG_STORICO(DTA_AZIONE) if do not exist

select min(dta_azione) into min_dta_azione_log
    From Dpa_Log;  
Select Max(Dta_Azione) Into Max_Dta_Azione_log_storico
    From Dpa_Log_storico;  

If Min_Dta_Azione_Log < Nvl(Max_Dta_Azione_Log_Storico, To_Date('01-01-1970','dd-mm-yyyy') ) Then
  -- N8OT OK !
  Utl_Insert_Log_W_Autcommit (Myusername  ,Sysdate    , 'procedura interrotta '        
    ,'invocata procedura UTL_STORICIZZA_LOG, in package UTILITA'
    ,'Bonificare DPA_LOG_STORICO! min(dta_azione) non  maggiore o uguale a Max(Dta_Azione_storico) '    ) ; 

Else  -- ok, Min_Dta_Azione_Log  maggiore o uguale a nvl(Max_Dta_Azione_log_storico, to_date('01-01-1970','dd-mm-yyyy') )

-- since dpa_log_storico has STORAGE COMPRESSION enabled, append hint will now store rows in compressed form
  INSERT /*+ append */ INTO dpa_log_storico (
      SYSTEM_ID,         USERID_OPERATORE,    ID_PEOPLE_OPERATORE,
      ID_GRUPPO_OPERATORE,                    ID_AMM,
      DTA_AZIONE,    VAR_OGGETTO,    ID_OGGETTO,    VAR_DESC_OGGETTO,
      VAR_COD_AZIONE,    CHA_ESITO,    VAR_DESC_AZIONE)
  SELECT SYSTEM_ID,    USERID_OPERATORE,    ID_PEOPLE_OPERATORE,
      ID_GRUPPO_OPERATORE,                   ID_AMM,
      DTA_AZIONE,    VAR_OGGETTO,    ID_OGGETTO,    VAR_DESC_OGGETTO,
      VAR_COD_AZIONE,    CHA_ESITO,    VAR_DESC_AZIONE
    From Dpa_Log
        Where Dta_Azione between Min_Dta_Azione_Log and (Min_Dta_Azione_Log + interval_of_days);  
  
  Delete From Dpa_Log 
        Where Dta_Azione between Min_Dta_Azione_Log and (Min_Dta_Azione_Log + interval_of_days); 
  
  Commit;
End If; 
-- END IF Min_Dta_Azione_Log < Max_Dta_Azione_log_storico 


Select 'A fine esecuzione della procedura, alle '||Systimestamp||',                 la tabella '||Tabelladi||' ha min dta_azione:'
    ||to_char(Min_Dta_Azione,'dd/mm/yyyy hh24:mi:ss')||' e Max Dta_Azione: ' ||to_char(Max_Dta_Azione,'dd/mm/yyyy hh24:mi:ss')     into stringa_msg
From (Select Min(Dta_Azione) As Min_Dta_Azione From Dpa_Log_Storico)
   , (Select Max(Dta_Azione) As Max_Dta_Azione From Dpa_Log_Storico)
   , (Select 'STORICO' As Tabelladi From Dual) ; 
utl_insert_log_w_autcommit (MYUSERNAME      ,sysdate       ,'estrazione intervallo date da DPA_LOG e DPA_LOG_STORICO' 
    ,'procedura UTL_STORICIZZA_LOG, package UTILITA'             ,stringa_msg     ) ; 

Select 'A fine esecuzione della procedura, alle '||Systimestamp||',                 la tabella '||Tabelladi||' ha min dta_azione:'
    ||to_char(Min_Dta_Azione_LEnd,'dd/mm/yyyy hh24:mi:ss')||' e Max Dta_Azione: ' ||to_char(Max_Dta_Azione_LEnd,'dd/mm/yyyy hh24:mi:ss') 
    Into Stringa_Msg
From (Select Min(Dta_Azione) As Min_Dta_Azione_Lend From Dpa_Log)
   , (Select Max(Dta_Azione) As Max_Dta_Azione_LEnd From Dpa_Log)
   , (Select 'DPA_LOG' As Tabelladi From Dual) ; 
utl_insert_log_w_autcommit (MYUSERNAME      ,sysdate       ,'estrazione intervallo date da DPA_LOG e DPA_LOG_STORICO' 
    ,'procedura UTL_STORICIZZA_LOG, package UTILITA'             ,Stringa_Msg     ) ; 


EXCEPTION
    When Others  Then
    utl_insert_log_w_autcommit (
    MYUSERNAME  --nomeutente VARCHAR2,
    ,Sysdate    --data_eseguito DATE,
    ,'invocata procedura UTL_STORICIZZA_LOG; in package UTILITA'         --comando_eseguito VARCHAR2,
    ,'categoria STORICIZZA LOG'         --CATEGORIA_COMANDO VARCHAR2,
    ,'eseguito rollback. Rilevato errore: '||SQLERRM           --esito VARCHAR2
    ) ; 
    ROLLBACK;
End UTL_STORICIZZA_LOG;


procedure UTL_OPTIMIZE_CONTEXT_INDEX (log_level int 
-- 0 = disabled, 1= only error messages
--, 2 = gives total elapsed time, 3 gives elapsed per single index
, Optimize_Mode Varchar2 -- can be 'FAST' or 'FULL'  or 'FULL_PARALLEL8' or 'REBUILD_INDEX'
 -- in 'REBUILD_INDEX' mode, index is not available, but elapsed time can be much less than with FULL mode, even with 'FULL_PARALLEL8' mode
) as

Cursor C_Text Is
   Select Index_Name, Parameters As Myidxparam From User_Indexes
    Where Index_Type='DOMAIN' And Ityp_Name = 'CONTEXT' And (Lower(Parameters) Like '%replace%' and Parameters is not null)
   Union Select Index_Name, 'replace '||Parameters As Myidxparam From User_Indexes
    Where Index_Type='DOMAIN' And Ityp_Name = 'CONTEXT' And (Lower(Parameters) Not Like '%replace%' And Parameters Is Not Null)
   union Select Index_Name, NULL As Myidxparam From User_Indexes
    Where Index_Type='DOMAIN' And Ityp_Name = 'CONTEXT' And Parameters Is Null;
    
Myusername Varchar2(200); Myindex Varchar2(200); myparam Varchar2(200);
Btimestamp Timestamp; Stringa_Msg Varchar2(2000);
--minutes_elapsed number(8,1);   seconds_elapsed  number(8,1);  
privilege_granted  varchar2(20);
begin
Select Username Into Myusername From User_Users;
 Dbms_Output.Put_Line ('inizio esecuzione UTL_OPTIMIZE_CONTEXT_INDEX, linea 239');
    BEGIN
        select privilege into privilege_granted from  user_tab_privs where table_name= 'CTX_DDL' ;
        IF (nvl(privilege_granted,'null') <> 'EXECUTE' ) THEN
            RAISE_APPLICATION_ERROR(-20002,'Missing EXECUTE privilege on CTX_DDL; EXECUTE privilege must be given by DBA to '||MYUSERNAME);
        end IF;
    exception when NO_DATA_FOUND then
                    RAISE_APPLICATION_ERROR(-20002,'Missing EXECUTE privilege on CTX_DDL; EXECUTE privilege must be given by DBA to '||MYUSERNAME);
              when others then RAISE;
    END;

Select Systimestamp Into Btimestamp From Dual ;
Dbms_Output.Put_Line ('proseguo esecuzione UTL_OPTIMIZE_CONTEXT_INDEX, linea 251');

For Text In C_Text
Loop
    --Dbms_Output.Put_Line ('esecuzione loop UTL_OPTIMIZE_CONTEXT_INDEX, linea 255; Text.Myidxparam'||ltrim(Text.Myidxparam));
    Myindex := Text.Index_Name ;
    Myparam := ltrim(Text.Myidxparam) ;
    Dbms_Output.Put_Line ('esecuzione loop UTL_OPTIMIZE_CONTEXT_INDEX, linea 258');
    
--OPTIMIZE_mode can be 'FULL_PARALLEL8' or 'REBUILD_INDEX' or 'FAST' or 'FULL'  
    IF OPTIMIZE_mode = 'FULL_PARALLEL8' THEN 
    -- maxtime specify maximum optimization time, in minutes, for FULL optimize.
      -- for PAT, with optimize degree 8 elapsed 36 min approx 
    Dbms_Output.Put_Line ('esecuzione loop UTL_OPTIMIZE_CONTEXT_INDEX, linea 264');
    
      Ctx_Ddl.Optimize_Index(Myindex,'FULL', Maxtime => 59, Parallel_Degree => 8 ) ;
    Dbms_Output.Put_Line ('esecuzione loop UTL_OPTIMIZE_CONTEXT_INDEX, linea 267');
     elsif  (OPTIMIZE_mode = 'REBUILD_INDEX' AND myparam IS NULL) THEN 
        Execute Immediate  'alter index '||Myindex||' rebuild ' ; 
      Elsif  (Optimize_Mode = 'REBUILD_INDEX' And Myparam Is Not Null) Then 
        Execute Immediate  'Alter Index '||Myindex||' Rebuild Parameters('' '|| Myparam||''')' ;  
    Else  -- 'FAST' or other mode
      Ctx_Ddl.Optimize_Index(Myindex,Optimize_Mode) ;
    END IF; 
    
 
 Select  'Context index '||Myindex||' '||Optimize_Mode||' optimized. Elapsed '
 ||Extract(Minute From Systimestamp - Btimestamp) ||' minutes and '
 ||round(Extract(Second From Systimestamp - Btimestamp))|| ' seconds since start ' 
      into stringa_msg from dual ;

 -- 0 = disabled, 1= only error messages, 2 = gives total elapsed time, 3 gives elapsed per single index
    --Dbms_Output.Put_Line ('esecuzione loop UTL_OPTIMIZE_CONTEXT_INDEX, linea 279; stringa_msg:'||nvl(stringa_msg,' oho!'));
    if log_level > 2 then    -- scrive nella tabella utl_system_log
        Utl_Insert_Log_W_Autcommit (Myusername           ,Sysdate  
        ,nvl(stringa_msg,'n.d.')        ,'OPTIMIZE_CONTEXT_INDEX'               ,'ok'   ) ; 
    end if;

End Loop;
      
      
Select Optimize_Mode||' optimized all '||Myusername||
      ' context indexes via UTL_OPTIMIZE_CONTEXT_INDEX in '
      ||Extract(Minute From Systimestamp - Btimestamp) ||' minutes and '
      ||round(Extract(Second From Systimestamp - Btimestamp))|| ' seconds'       
      into stringa_msg from dual ;

 If Log_Level > 1 Then -- scrive nella tabella utl_system_log
 Utl_Insert_Log_W_Autcommit (Myusername   ,Sysdate     , stringa_msg
    ,'OPTIMIZE_CONTEXT_INDEX'       ,'ok'   ) ; 
 end if;

exception when others then
 -- 0 = disabled, 1= only error messages, 2 = gives total elapsed time, 3 gives elapsed per single index
   Dbms_Output.Put_Line ('errore generico in UTL_OPTIMIZE_CONTEXT_INDEX');
 if log_level > 0 then -- scrive nella tabella utl_system_log
 utl_insert_log_w_autcommit (
    MYUSERNAME   --VARCHAR2,
    ,sysdate  --DATE,
    ,'KO WHILE '||OPTIMIZE_mode||' OPTIMIZING '||MYINDEX||' VIA THE sp UTL_OPTIMIZE_CONTEXT_INDEX ' -- VARCHAR2,
    ,'OPTIMIZE_CONTEXT_INDEX'       --VARCHAR2,
    ,SUBSTR('ko! '||sqlerrm,1,200)
              ) ; --           VARCHAR2)
 end if ;

End;

Procedure Utl_Setsecurityruoloreg Is 
Cursor C_Ruolo Is 
  Select Id_Registro, New_Aooruoloresp    
  From Utl_Track_Aooruoloresp 
    Where Cha_Changed_Notyet_Processed = 'y'; 
Myusername Varchar2(32);
Grantotale Int := 0; 
myflag boolean := true; 

Begin
Select Username Into  Myusername   From User_Users; 
 utl_insert_log_w_autcommit (    MYUSERNAME   --VARCHAR2,
        ,Sysdate  --DATE,
        ,'Starting SP ....' -- VARCHAR2,
        ,'Utl_Setsecurityruoloreg'       --VARCHAR2,
        ,'ok'         ) ;     
        
For Ruolo In C_Ruolo Loop 
exit when C_Ruolo%NOTFOUND;

If Myflag Then
    myflag := false; 
    Lock Table Utl_Track_Aooruoloresp In Exclusive Mode; 
     utl_insert_log_w_autcommit (    MYUSERNAME   --VARCHAR2,
        ,Sysdate  --DATE,
        ,'Started SP; locked Table Utl_Track_Aooruoloresp In Exclusive Mode ....' -- VARCHAR2,
        ,'Utl_Setsecurityruoloreg'       --VARCHAR2,
        ,'ok'         ) ; 
end if;

 Insert /*+ append (Security) */ Into Security (  Thing     
                        , Personorgroup
                        , Accessrights
                        , Id_Gruppo_Trasm
                        , Cha_Tipo_Diritto )  
    Select P.System_Id        As Thing               
      ,Ruolo.New_Aooruoloresp --,Cg.Id_Gruppo           
      As Personorgroup
      ,Elr.Diritto_Ruolo_Aoo  As Accessrights
      ,Null                   As Id_Gruppo_Trasm 
      ,'A'                    As Cha_Tipo_Diritto
    From Profile P,  Dpa_El_Registri Elr --, Dpa_Corr_Globali Cg
    Where P.Id_Registro=Elr.System_Id --Elr.Id_Ruolo_Resp= Cg.System_Id And 
    And Elr.Id_Ruolo_Resp Is Not Null 
    and Elr.Cha_Rf= '0' 
    and p.num_proto is not null
    and not exists (select 'x' from SECURITY s1 
      Where S1.Thing=P.System_Id 
      And S1.Personorgroup = Ruolo.New_Aooruoloresp -- Cg.Id_Gruppo --idGruppo 
      And S1.Accessrights >= Elr.Diritto_Ruolo_Aoo --Diritto 
      );
grantotale := grantotale + SQL%ROWCOUNT; 
End Loop;      

if grantotale > 0 then
     update Utl_Track_Aooruoloresp 
      Set Cha_Changed_Notyet_Processed = 'n'        ;
     commit;   
        
 utl_insert_log_w_autcommit (    MYUSERNAME   --VARCHAR2,
    ,Sysdate  --DATE,
    ,'inseriti '||grantotale|| ' record in SECURITY a seguito della modifica del Ruolo responsabile AOO' -- VARCHAR2,
    ,'Utl_Setsecurityruoloreg'       --VARCHAR2,
    ,'ok'         ) ; 
else 
 utl_insert_log_w_autcommit (    MYUSERNAME   --VARCHAR2,
    ,Sysdate  --DATE,
    ,'trovati zero record da inserire in SECURITY ' -- VARCHAR2,
    ,'Utl_Setsecurityruoloreg'       --VARCHAR2,
    ,'ok'         ) ; 

end if; 


Exception
  When No_Data_Found Then Null; 
  When Others Then 
  rollback; 
   utl_insert_log_w_autcommit (
    MYUSERNAME   --VARCHAR2,
    ,Sysdate  --DATE,
    ,'ko and rollback while running SP Utl_Setsecurityruoloreg ' -- VARCHAR2,
    ,'Utl_Setsecurityruoloreg'       --VARCHAR2,
    ,SUBSTR('ko! '||sqlerrm,200)         ) ; 
    Raise; 
end Utl_Setsecurityruoloreg;
end UTilita;
/


              
----------- FINE -
              
---- replace_sp_modify_corr_esterno_IS.ORA.sql  marcatore per ricerca ----
create or replace PROCEDURE  @db_user.sp_modify_corr_esterno_IS (
/*
versione della SP ad hoc per "Interoperabilit semplificata", by S. Furnari:
oltre a introdurre e gestire il nuovo parametro SimpInteropUrl , 
recepisce le modifiche introdotte in questa stessa versione 3.21 by C. Ferlito 
per gestire il nuovo campo var_desc_corr_old
*/
   idcorrglobale     IN       NUMBER,
   desc_corr         IN       VARCHAR2,
   nome              IN       VARCHAR2,
   cognome           IN       VARCHAR2,
   codice_aoo        IN       VARCHAR2,
   codice_amm        IN       VARCHAR2,
   email             IN       VARCHAR2,
   indirizzo         IN       VARCHAR2,
   cap               IN       VARCHAR2,
   provincia         IN       VARCHAR2,
   nazione           IN       VARCHAR2,
   citta             IN       VARCHAR2,
   cod_fiscale       IN       VARCHAR2,
   telefono          IN       VARCHAR2,
   telefono2         IN       VARCHAR2,
   note              IN       VARCHAR2,
   fax               IN       VARCHAR2,
   var_iddoctype     IN       NUMBER,
   inrubricacomune   IN       CHAR,
   tipourp           IN       CHAR,
   localita             IN          VARCHAR2,
   luogoNascita      IN       VARCHAR2,
   dataNascita       IN       VARCHAR2,
   titolo            IN       VARCHAR2,
-- aggiunto questo parametro e la gestione relativa rispetto alla vecchia versione
   SimpInteropUrl    IN       VARCHAR2,
   newid             OUT      NUMBER,
   returnvalue       OUT      NUMBER
)
IS
BEGIN
   DECLARE
      cnt   integer;
      cod_rubrica              VARCHAR2 (128);
      id_reg                   NUMBER;
      idamm                    NUMBER;
      new_var_cod_rubrica      VARCHAR2 (128);
      cha_dettaglio            CHAR (1):= '0';
      cha_tipourp             CHAR (1);
      myprofile                NUMBER;
      new_idcorrglobale        NUMBER;
      identitydettglobali      NUMBER;
      outvalue                 NUMBER         := 1;
      rtn                      NUMBER;
      v_id_doctype             NUMBER;
      identitydpatcanalecorr   NUMBER;
      chaTipoIE                CHAR (1);
      numLivello               NUMBER          := 0;
      idParent                 NUMBER;
      idPesoOrg                NUMBER;
      idUO                     NUMBER;
      idGruppo                 NUMBER;
      idTipoRuolo              NUMBER;
      cha_tipo_corr            CHAR (1);
      chapa                    CHAR (1);
      var_desc_old             VARCHAR2(256);
       url                    varchar2(4000);
   BEGIN

      <<reperimento_dati>>
      BEGIN
         SELECT var_cod_rubrica, cha_tipo_urp, id_registro, id_amm,cha_pa, cha_tipo_ie, num_livello, id_parent, id_peso_org, id_uo, id_tipo_ruolo, id_gruppo,var_desc_corr_old, InteropUrl
           INTO cod_rubrica, cha_tipourp, id_reg, idamm, chapa, chaTipoIE, numLivello, idParent, idPesoOrg, IdUO, idTipoRuolo, idGruppo,var_desc_old         , url
           FROM dpa_corr_globali
          WHERE system_id = idcorrglobale;
          dbms_output.put_line('select effettuata') ;
      EXCEPTION
         WHEN NO_DATA_FOUND
         THEN
         dbms_output.put_line('primo blocco eccezione') ;
            outvalue := 0;
            RETURN;
      END reperimento_dati;


    if(tipourp is not null and cha_tipourp is not null and cha_tipourp!=tipourp) then
        cha_tipourp := tipourp;

    end if;


      <<dati_canale_utente>>
      if cha_tipourp = 'P'
      THEN
        BEGIN
         SELECT id_documenttype
           INTO v_id_doctype
           FROM dpa_t_canale_corr
          WHERE id_corr_globale = idcorrglobale;
        EXCEPTION
         WHEN NO_DATA_FOUND
         THEN
            dbms_output.put_line('2do blocco eccezione') ; 
            outvalue := 2;
        END dati_canale_utente;
      end if;

      IF /* 0 */ outvalue = 1
      THEN
         IF /* 1 */ cha_tipourp = 'U' OR cha_tipourp = 'P'
         THEN
            cha_dettaglio := '1';
         END IF;                                                       /* 1 */

--VERIFICO se il corrisp ?? stato utilizzato come dest/mitt di protocolli
         SELECT COUNT (id_profile)
           INTO myprofile
           FROM dpa_doc_arrivo_par
          WHERE id_mitt_dest = idcorrglobale;

-- 1) non ?? stato mai utilizzato come corrisp in un protocollo
         IF /* 2 */ (myprofile = 0)
         THEN
            BEGIN
               UPDATE dpa_corr_globali
                  SET var_codice_aoo = codice_aoo,
                      var_codice_amm = codice_amm,
                      var_email = email,
                      var_desc_corr = desc_corr,
                      var_nome = nome,
                      var_cognome = cognome,
                      cha_pa=chapa,
                      cha_tipo_urp=cha_tipourp,
                      InteropUrl = SimpInteropUrl
                WHERE system_id = idcorrglobale;
            EXCEPTION
               WHEN OTHERS
               THEN
               dbms_output.put_line('3o blocco eccezione') ;
                  outvalue := 3;
                  RETURN;
            END;

/* SE L'UPDATE SU DPA_CORR_GLOBALI ?? ANDTATA A BUON FINE
PER UTENTI E UO DEVO AGGIORNARE IL RECORD SULLA DPA_DETT_GLOBALI
*/
            IF /* 3 */ cha_tipourp = 'U' OR cha_tipourp = 'P'     OR cha_tipourp = 'F'
            THEN

               <<update_dpa_dett_globali>>
               BEGIN
                      
               
               
                     SELECT count(*) into cnt
                          FROM dpa_dett_globali
                          WHERE id_corr_globali = idcorrglobale ; 
                      
                      IF (cnt = 0)
                      THEN 
                       dbms_output.put_line('sono nella INSERT,id_corr_globali =  '||idcorrglobale) ; 
                      INSERT INTO dpa_dett_globali (
                          system_id,
                          id_corr_globali, 
                          var_indirizzo ,
                                 var_cap ,
                                 var_provincia ,
                                 var_nazione ,
                                 var_cod_fiscale ,
                                 var_telefono ,
                                 var_telefono2 ,
                                 var_note ,
                                 var_citta ,
                                 var_fax ,
                                 var_localita,
                                  var_luogo_nascita,
                                 dta_nascita,
                                 var_titolo)
                          VALUES (
                          seq.nextval,
                          idcorrglobale,
                          indirizzo,
                                 cap,
                                 provincia,
                                 nazione,
                                 cod_fiscale,
                                 telefono,
                                 telefono2,
                                 note,
                                 citta,
                                 fax,
                                 localita,
                                 luogoNascita,
                                 dataNascita,
                                 titolo);
                           END IF;
                           
                        IF (cnt = 1)
                      THEN    
                        dbms_output.put_line('sono nella UPDATE') ; 
                     UPDATE dpa_dett_globali SET
                             var_indirizzo = indirizzo,
                             var_cap = cap,
                             var_provincia = provincia,
                             var_nazione = nazione,
                             var_cod_fiscale = cod_fiscale,
                             var_telefono = telefono,
                             var_telefono2 = telefono2,
                             var_note = note,
                             var_citta = citta,
                             var_fax = fax,
                             var_localita = localita,
                          var_luogo_nascita = luogoNascita,
                         dta_nascita = dataNascita,
                         var_titolo = titolo
                          WHERE (id_corr_globali = idcorrglobale) ; 
                                              END IF;
                                               
                      /*
                      
                      MERGE INTO dpa_dett_globali
                        USING (
                          SELECT system_id as id_interno
                          FROM dpa_dett_globali
                          WHERE id_corr_globali = idcorrglobale) select_interna
                        ON (system_id = select_interna.id_interno)
                        WHEN MATCHED THEN
                          UPDATE SET
                             var_indirizzo = indirizzo,
                             var_cap = cap,
                             var_provincia = provincia,
                             var_nazione = nazione,
                             var_cod_fiscale = cod_fiscale,
                             var_telefono = telefono,
                             var_telefono2 = telefono2,
                             var_note = note,
                             var_citta = citta,
                             var_fax = fax,
                             var_localita = localita,
                          var_luogo_nascita = luogoNascita,
                         dta_nascita = dataNascita,
                         var_titolo = titolo
                         WHERE (id_corr_globali = idcorrglobale) 
                        WHEN NOT MATCHED THEN
                       INSERT (
                          system_id,
                          id_corr_globali, 
                          var_indirizzo ,
                                 var_cap ,
                                 var_provincia ,
                                 var_nazione ,
                                 var_cod_fiscale ,
                                 var_telefono ,
                                 var_telefono2 ,
                                 var_note ,
                                 var_citta ,
                                 var_fax ,
                                 var_localita,
                                  var_luogo_nascita,
                                 dta_nascita,
                                 var_titolo)
                         VALUES (
                          seq.nextval,
                          idcorrglobale,
                          indirizzo,
                                 cap,
                                 provincia,
                                 nazione,
                                 cod_fiscale,
                                 telefono,
                                 telefono2,
                                 note,
                                 citta,
                                 fax,
                                 localita,
                                 luogoNascita,
                                 dataNascita,
                                 titolo);

                          
                        */
                         commit; 
                                  dbms_output.put_line('sono nella merge') ; 
               EXCEPTION
                  WHEN OTHERS
                  THEN
                     dbms_output.put_line('4o blocco eccezione'||SQLERRM) ; 
                     outvalue := 4;
                     RETURN;
               END update_dpa_dett_globali;
            END IF;                                                    /* 3 */

--METTI QUI UPDATE SU DPA_T_CANALE_CORR
--AGGIORNO LA DPA_T_CANALE_CORR
               BEGIN
                  UPDATE dpa_t_canale_corr
                     SET id_documenttype = var_iddoctype
                   WHERE id_corr_globale = idcorrglobale;
               EXCEPTION
                  WHEN OTHERS
                  THEN
                     dbms_output.put_line('5o blocco eccezione') ; 
                     outvalue := 5;
                     RETURN;
               END;
         ELSE
-- caso 2) Il corrisp ?? stato utilizzato come corrisp in un protocollo
-- NUOVO CODICE RUBRICA
            new_var_cod_rubrica :=
                                cod_rubrica || '_' || TO_CHAR (idcorrglobale);

            <<storicizzazione_corrisp>>
            BEGIN
               UPDATE dpa_corr_globali
                  SET dta_fine = SYSDATE,
                     var_cod_rubrica = new_var_cod_rubrica,
                      var_codice = new_var_cod_rubrica,
                      id_parent = NULL
                WHERE system_id = idcorrglobale;
            EXCEPTION
               WHEN OTHERS
               THEN
                 dbms_output.put_line('6o blocco eccezione') ; 
                  outvalue := 6;
                  RETURN;
            END storicizzazione_corrisp;

            SELECT seq.NEXTVAL
              INTO newid
              FROM DUAL;

/* DOPO LA STORICIZZAZIONE DEL VECCHIO CORRISPONDENTE POSSO
INSERIRE IL NUOVO CORRISPONDENTE NELLA DPA_CORR_GLOBALI */
            <<inserimento_nuovo_corrisp>>
            BEGIN
               IF (inrubricacomune = '1')
               THEN
                  cha_tipo_corr := 'C';
               ELSE
                  cha_tipo_corr := 'S';
               END IF;

               INSERT INTO dpa_corr_globali
                           (system_id, num_livello, cha_tipo_ie, id_registro,
                            id_amm, var_desc_corr, var_nome, var_cognome,
                            id_old, dta_inizio, id_parent, var_codice,
                            cha_tipo_corr, cha_tipo_urp,
                            var_codice_aoo, var_cod_rubrica, cha_dettagli,
                            var_email, var_codice_amm, cha_pa, id_peso_org,
                            id_gruppo, id_tipo_ruolo, id_uo,var_desc_corr_old     , InteropUrl
                           )
                    VALUES (newid, numLivello, chaTipoIE, id_reg,
                            idamm, desc_corr, nome, cognome,
                            idcorrglobale, SYSDATE, idParent, cod_rubrica,
                            cha_tipo_corr, cha_tipourp,
                            codice_aoo, cod_rubrica, cha_dettaglio,
                            email, codice_amm, chapa, idPesoOrg,
                            idGruppo, idTipoRuolo, idUO, var_desc_old , SimpInteropUrl
                           );
            EXCEPTION
               WHEN OTHERS
               THEN
                  dbms_output.put_line('7o blocco eccezione') ; 
                  outvalue := 7;
                  RETURN;
            END inserimento_nuovo_corrisp;

 
/* DOPO L'INSERIMENTO DEL NUOVO CORRISPONDENTE POSSO INSERIRE IL
RELATIVO RECORD NELLA DPA_DETT_GLOBALI, MA SOLO PER CORRISPONDENTI UTENTI
E UNITA' ORGANIZZATIVE */
            IF /* 4 */ cha_tipourp = 'U' OR cha_tipourp = 'P'
            THEN
--PRENDO LA SYSTEM_ID APPENA INSERITA
               SELECT seq.NEXTVAL
                 INTO identitydettglobali
                 FROM DUAL;

               <<inserimento_dettaglio_corrisp>>
               BEGIN
                  INSERT INTO dpa_dett_globali
                              (system_id, id_corr_globali, var_indirizzo,
                               var_cap, var_provincia, var_nazione,
                               var_cod_fiscale, var_telefono, var_telefono2,
                               var_note, var_citta, var_fax, var_localita, var_luogo_nascita, dta_nascita, var_titolo
                              )
                       VALUES (identitydettglobali, newid, indirizzo,
                               cap, provincia, nazione,
                               cod_fiscale, telefono, telefono2,
                               note, citta, fax, localita, luogoNascita, dataNascita, titolo
                              );
               EXCEPTION
                  WHEN OTHERS
                  THEN
                     dbms_output.put_line('8o blocco eccezione') ; 
                     outvalue := 8;
                     RETURN;
               END inserimento_dettaglio_corrisp;
            END IF;                                                    /* 4 */

--INSERISCO IL CANALE PREFERITO DEL NUOVO CORRISP ESTERNO SIA ESSO UO, RUOLO, PERSONA
            <<inserimento_dpa_t_canale_corr>>
            BEGIN
               SELECT seq.NEXTVAL
                 INTO identitydpatcanalecorr
                 FROM DUAL;

               INSERT INTO dpa_t_canale_corr
                           (system_id, id_corr_globale, id_documenttype,
                            cha_preferito
                           )
                    VALUES (identitydpatcanalecorr, newid, var_iddoctype,
                            '1'
                           );
            EXCEPTION
               WHEN OTHERS
               THEN
                  dbms_output.put_line('9o blocco eccezione') ; 
                  outvalue := 9;
                  RETURN;
            END inserimento_dpa_t_canale_corr;
         END IF;   
         
         
--se fa parte di una lista, allora la devo aggiornare.
if newid IS NOT NULL 
THEN 
    update dpa_liste_distr d set d.ID_DPA_CORR=newid where d.ID_DPA_CORR=idcorrglobale;
end if;

         
         
                                                             /* 2 */
      END IF /* 0 */;

      returnvalue := outvalue;
   END;
END;
/

              
----------- FINE -
              
---- replace_sp_modify_corr_esterno.ORA.sql  marcatore per ricerca ----
create or replace PROCEDURE  @db_user.sp_modify_corr_esterno (
   idcorrglobale     IN       NUMBER,
   desc_corr         IN       VARCHAR2,
   nome              IN       VARCHAR2,
   cognome           IN       VARCHAR2,
   codice_aoo        IN       VARCHAR2,
   codice_amm        IN       VARCHAR2,
   email             IN       VARCHAR2,
   indirizzo         IN       VARCHAR2,
   cap               IN       VARCHAR2,
   provincia         IN       VARCHAR2,
   nazione           IN       VARCHAR2,
   citta             IN       VARCHAR2,
   cod_fiscale       IN       VARCHAR2,
   telefono          IN       VARCHAR2,
   telefono2         IN       VARCHAR2,
   note              IN       VARCHAR2,
   fax               IN       VARCHAR2,
   var_iddoctype     IN       NUMBER,
   inrubricacomune   IN       CHAR,
   tipourp           IN       CHAR,
   localita             IN          VARCHAR2,
   luogoNascita      IN       VARCHAR2,
   dataNascita       IN       VARCHAR2,
   titolo            IN       VARCHAR2,
   newid             OUT      NUMBER,  
   returnvalue       OUT      NUMBER
)
IS
BEGIN
   DECLARE
      cnt   integer; 
      cod_rubrica              VARCHAR2 (128);
      id_reg                   NUMBER;
      idamm                    NUMBER;
      new_var_cod_rubrica      VARCHAR2 (128);
      cha_dettaglio            CHAR (1):= '0';
      cha_tipourp             CHAR (1);
      myprofile                NUMBER;
      new_idcorrglobale        NUMBER;
      identitydettglobali      NUMBER;
      outvalue                 NUMBER         := 1;
      rtn                      NUMBER;
      v_id_doctype             NUMBER;
      identitydpatcanalecorr   NUMBER;
      chaTipoIE                CHAR (1);
      numLivello               NUMBER          := 0;
      idParent                 NUMBER;
      idPesoOrg                NUMBER;
      idUO                     NUMBER;
      idGruppo                 NUMBER;
      idTipoRuolo              NUMBER;
      cha_tipo_corr            CHAR (1);
      chapa                    CHAR (1);
      var_desc_old             VARCHAR2(256);
   BEGIN

      <<reperimento_dati>>
      BEGIN
         SELECT var_cod_rubrica, cha_tipo_urp, id_registro, id_amm,cha_pa, cha_tipo_ie, num_livello, id_parent, id_peso_org, id_uo, id_tipo_ruolo, id_gruppo,var_desc_corr_old
           INTO cod_rubrica, cha_tipourp, id_reg, idamm, chapa, chaTipoIE, numLivello, idParent, idPesoOrg, IdUO, idTipoRuolo, idGruppo,var_desc_old
           FROM dpa_corr_globali
          WHERE system_id = idcorrglobale;
          dbms_output.put_line('select effettuata') ; 
      EXCEPTION
         WHEN NO_DATA_FOUND
         THEN
         dbms_output.put_line('primo blocco eccezione') ; 
            outvalue := 0;
            RETURN;
      END reperimento_dati;
      
      
    if(tipourp is not null and cha_tipourp is not null and cha_tipourp!=tipourp) then
        cha_tipourp := tipourp;
      
    end if;
      
 
      <<dati_canale_utente>>
      if cha_tipourp = 'P' 
      THEN
        BEGIN
         SELECT id_documenttype
           INTO v_id_doctype
           FROM dpa_t_canale_corr
          WHERE id_corr_globale = idcorrglobale;
        EXCEPTION
         WHEN NO_DATA_FOUND
         THEN
            dbms_output.put_line('2do blocco eccezione') ; 
            outvalue := 2;
        END dati_canale_utente;
      end if;
      
      IF /* 0 */ outvalue = 1
      THEN
         IF /* 1 */ cha_tipourp = 'U' OR cha_tipourp = 'P'
         THEN
            cha_dettaglio := '1';
         END IF;                                                       /* 1 */

--VERIFICO se il corrisp ?? stato utilizzato come dest/mitt di protocolli
         SELECT COUNT (id_profile)
           INTO myprofile
           FROM dpa_doc_arrivo_par
          WHERE id_mitt_dest = idcorrglobale;
          
-- 1) non ?? stato mai utilizzato come corrisp in un protocollo
         IF /* 2 */ (myprofile = 0)
         THEN
            BEGIN
               UPDATE dpa_corr_globali
                  SET var_codice_aoo = codice_aoo,
                      var_codice_amm = codice_amm,
                      var_email = email,
                      var_desc_corr = desc_corr,
                      var_nome = nome,
                      var_cognome = cognome,
                      cha_pa=chapa,
                      cha_tipo_urp=cha_tipourp
                WHERE system_id = idcorrglobale;
            EXCEPTION
               WHEN OTHERS
               THEN
               dbms_output.put_line('3o blocco eccezione') ; 
                  outvalue := 3;
                  RETURN;
            END;

/* SE L'UPDATE SU DPA_CORR_GLOBALI ?? ANDTATA A BUON FINE
PER UTENTI E UO DEVO AGGIORNARE IL RECORD SULLA DPA_DETT_GLOBALI
*/
            IF /* 3 */ cha_tipourp = 'U' OR cha_tipourp = 'P'
            THEN

               <<update_dpa_dett_globali>>
               BEGIN
                      
               
               
                     SELECT count(*) into cnt
                          FROM dpa_dett_globali
                          WHERE id_corr_globali = idcorrglobale ; 
                      
                      IF (cnt = 0)
                      THEN 
                       dbms_output.put_line('sono nella INSERT,id_corr_globali =  '||idcorrglobale) ; 
                      INSERT INTO dpa_dett_globali (
                          system_id,
                          id_corr_globali, 
                          var_indirizzo ,
                                 var_cap ,
                                 var_provincia ,
                                 var_nazione ,
                                 var_cod_fiscale ,
                                 var_telefono ,
                                 var_telefono2 ,
                                 var_note ,
                                 var_citta ,
                                 var_fax ,
                                 var_localita,
                                  var_luogo_nascita,
                                 dta_nascita,
                                 var_titolo)
                          VALUES (
                          seq.nextval,
                          idcorrglobale,
                          indirizzo,
                                 cap,
                                 provincia,
                                 nazione,
                                 cod_fiscale,
                                 telefono,
                                 telefono2,
                                 note,
                                 citta,
                                 fax,
                                 localita,
                                 luogoNascita,
                                 dataNascita,
                                 titolo);
                           END IF;
                           
                        IF (cnt = 1)
                      THEN    
                        dbms_output.put_line('sono nella UPDATE') ; 
                     UPDATE dpa_dett_globali SET
                             var_indirizzo = indirizzo,
                             var_cap = cap,
                             var_provincia = provincia,
                             var_nazione = nazione,
                             var_cod_fiscale = cod_fiscale,
                             var_telefono = telefono,
                             var_telefono2 = telefono2,
                             var_note = note,
                             var_citta = citta,
                             var_fax = fax,
                             var_localita = localita,
                          var_luogo_nascita = luogoNascita,
                         dta_nascita = dataNascita,
                         var_titolo = titolo
                          WHERE (id_corr_globali = idcorrglobale) ; 
                                              END IF;
                                               
                      /*
                      
                      MERGE INTO dpa_dett_globali
                        USING (
                          SELECT system_id as id_interno
                          FROM dpa_dett_globali
                          WHERE id_corr_globali = idcorrglobale) select_interna
                        ON (system_id = select_interna.id_interno)
                        WHEN MATCHED THEN
                          UPDATE SET
                             var_indirizzo = indirizzo,
                             var_cap = cap,
                             var_provincia = provincia,
                             var_nazione = nazione,
                             var_cod_fiscale = cod_fiscale,
                             var_telefono = telefono,
                             var_telefono2 = telefono2,
                             var_note = note,
                             var_citta = citta,
                             var_fax = fax,
                             var_localita = localita,
                          var_luogo_nascita = luogoNascita,
                         dta_nascita = dataNascita,
                         var_titolo = titolo
                         WHERE (id_corr_globali = idcorrglobale) 
                        WHEN NOT MATCHED THEN
                       INSERT (
                          system_id,
                          id_corr_globali, 
                          var_indirizzo ,
                                 var_cap ,
                                 var_provincia ,
                                 var_nazione ,
                                 var_cod_fiscale ,
                                 var_telefono ,
                                 var_telefono2 ,
                                 var_note ,
                                 var_citta ,
                                 var_fax ,
                                 var_localita,
                                  var_luogo_nascita,
                                 dta_nascita,
                                 var_titolo)
                         VALUES (
                          seq.nextval,
                          idcorrglobale,
                          indirizzo,
                                 cap,
                                 provincia,
                                 nazione,
                                 cod_fiscale,
                                 telefono,
                                 telefono2,
                                 note,
                                 citta,
                                 fax,
                                 localita,
                                 luogoNascita,
                                 dataNascita,
                                 titolo);

                          
                        */
                         commit; 
                                  dbms_output.put_line('sono nella merge') ; 
               EXCEPTION
                  WHEN OTHERS
                  THEN
                     dbms_output.put_line('4o blocco eccezione'||SQLERRM) ; 
                     outvalue := 4;
                     RETURN;
               END update_dpa_dett_globali;
            END IF;                                                    /* 3 */

--METTI QUI UPDATE SU DPA_T_CANALE_CORR
--AGGIORNO LA DPA_T_CANALE_CORR
               BEGIN
                  UPDATE dpa_t_canale_corr
                     SET id_documenttype = var_iddoctype
                   WHERE id_corr_globale = idcorrglobale;
               EXCEPTION
                  WHEN OTHERS
                  THEN
                     dbms_output.put_line('5o blocco eccezione') ; 
                     outvalue := 5;
                     RETURN;
               END;
         ELSE
-- caso 2) Il corrisp ?? stato utilizzato come corrisp in un protocollo
-- NUOVO CODICE RUBRICA
            new_var_cod_rubrica :=
                                cod_rubrica || '_' || TO_CHAR (idcorrglobale);

            <<storicizzazione_corrisp>>
            BEGIN
               UPDATE dpa_corr_globali
                  SET dta_fine = SYSDATE,
                     var_cod_rubrica = new_var_cod_rubrica,
                      var_codice = new_var_cod_rubrica,
                      id_parent = NULL
                WHERE system_id = idcorrglobale;
            EXCEPTION
               WHEN OTHERS
               THEN
                 dbms_output.put_line('6o blocco eccezione') ; 
                  outvalue := 6;
                  RETURN;
            END storicizzazione_corrisp;

            SELECT seq.NEXTVAL
              INTO newid
              FROM DUAL;

/* DOPO LA STORICIZZAZIONE DEL VECCHIO CORRISPONDENTE POSSO
INSERIRE IL NUOVO CORRISPONDENTE NELLA DPA_CORR_GLOBALI */
            <<inserimento_nuovo_corrisp>>
            BEGIN
               IF (inrubricacomune = '1')
               THEN
                  cha_tipo_corr := 'C';
               ELSE
                  cha_tipo_corr := 'S';
               END IF;

               INSERT INTO dpa_corr_globali
                           (system_id, num_livello, cha_tipo_ie, id_registro,
                            id_amm, var_desc_corr, var_nome, var_cognome,
                            id_old, dta_inizio, id_parent, var_codice,
                            cha_tipo_corr, cha_tipo_urp,
                            var_codice_aoo, var_cod_rubrica, cha_dettagli,
                            var_email, var_codice_amm, cha_pa, id_peso_org,
                            id_gruppo, id_tipo_ruolo, id_uo,var_desc_corr_old
                           )
                    VALUES (newid, numLivello, chaTipoIE, id_reg,
                            idamm, desc_corr, nome, cognome,
                            idcorrglobale, SYSDATE, idParent, cod_rubrica,
                            cha_tipo_corr, cha_tipourp,
                            codice_aoo, cod_rubrica, cha_dettaglio,
                            email, codice_amm, chapa, idPesoOrg,
                            idGruppo, idTipoRuolo, idUO, var_desc_old
                           );
            EXCEPTION
               WHEN OTHERS
               THEN
                  dbms_output.put_line('7o blocco eccezione') ; 
                  outvalue := 7;
                  RETURN;
            END inserimento_nuovo_corrisp;

 
/* DOPO L'INSERIMENTO DEL NUOVO CORRISPONDENTE POSSO INSERIRE IL
RELATIVO RECORD NELLA DPA_DETT_GLOBALI, MA SOLO PER CORRISPONDENTI UTENTI
E UNITA' ORGANIZZATIVE */
            IF /* 4 */ cha_tipourp = 'U' OR cha_tipourp = 'P'
            THEN
--PRENDO LA SYSTEM_ID APPENA INSERITA
               SELECT seq.NEXTVAL
                 INTO identitydettglobali
                 FROM DUAL;

               <<inserimento_dettaglio_corrisp>>
               BEGIN
                  INSERT INTO dpa_dett_globali
                              (system_id, id_corr_globali, var_indirizzo,
                               var_cap, var_provincia, var_nazione,
                               var_cod_fiscale, var_telefono, var_telefono2,
                               var_note, var_citta, var_fax, var_localita, var_luogo_nascita, dta_nascita, var_titolo
                              )
                       VALUES (identitydettglobali, newid, indirizzo,
                               cap, provincia, nazione,
                               cod_fiscale, telefono, telefono2,
                               note, citta, fax, localita, luogoNascita, dataNascita, titolo
                              );
               EXCEPTION
                  WHEN OTHERS
                  THEN
                     dbms_output.put_line('8o blocco eccezione') ; 
                     outvalue := 8;
                     RETURN;
               END inserimento_dettaglio_corrisp;
            END IF;                                                    /* 4 */

--INSERISCO IL CANALE PREFERITO DEL NUOVO CORRISP ESTERNO SIA ESSO UO, RUOLO, PERSONA
            <<inserimento_dpa_t_canale_corr>>
            BEGIN
               SELECT seq.NEXTVAL
                 INTO identitydpatcanalecorr
                 FROM DUAL;

               INSERT INTO dpa_t_canale_corr
                           (system_id, id_corr_globale, id_documenttype,
                            cha_preferito
                           )
                    VALUES (identitydpatcanalecorr, newid, var_iddoctype,
                            '1'
                           );
            EXCEPTION
               WHEN OTHERS
               THEN
                  dbms_output.put_line('9o blocco eccezione') ; 
                  outvalue := 9;
                  RETURN;
            END inserimento_dpa_t_canale_corr;
         END IF;   
         
         
--se fa parte di una lista, allora la devo aggiornare.
if newid IS NOT NULL 
THEN 
    update dpa_liste_distr d set d.ID_DPA_CORR=newid where d.ID_DPA_CORR=idcorrglobale;
end if;

         
         
                                                             /* 2 */
      END IF /* 0 */;

      returnvalue := outvalue;
   END;
END;
/

              
----------- FINE -
              
 
-------------------cartella  VERSIONE -------------------
              
---- INIT_CENTRO_NOTIFICHE.ORA.sql  marcatore per ricerca ----
Begin
    -- Cursore per scorrere le amministrazioni registrate in un DB
    Declare Cursor Cur Is Select Var_Codice_Amm From Dpa_Amministra;
    -- Codice dell'amministrazione da registrare nel centro notifiche
    codAmm varchar2 (2000) ;
    -- Id dell'amministrazione cui associare un canale
    idAmm integer;
    -- Id del canale di interoperabilit semplificata
    idChannel integer;

	cntdati integer; 
    Begin
    
	select count(*) into cntdati from NOTIFICATIONCHANNEL;
	IF cntdati = 0 THEN
	    -- Inserimento canale di interoperabilit semplificata
        nc.INSERTCHANNEL('IS', 'Interoperabilita Semplificata');
				
	    -- Selezione dell'id associato al canale IS
        Select Id Into idChannel From notificationchannel Where label = 'IS';
  
        Open cur;
    
        LOOP
            FETCH cur INTO codAmm;
            EXIT WHEN cur%NOTFOUND;
                
			-- Registrazione dell'amministrazione nel centro notifiche
			Nc.Insertinstance(Codamm);
        
	        -- Recupero dell'id associato all'amministrazione
            Select id into idamm From notificationinstance where description = codamm;
                
            -- Associazione del canale all'istanza
            nc.ASSOCIATECHANNELTOINSTANCE(1, idamm);
	            
        End Loop;

		Close cur;
	END IF;
    
    End;
end;
/
              
---- insert_DPA_DOCSPA.ORA.sql  marcatore per ricerca ----
begin
-- rename precedenti versioni 3.21
update @db_user.DPA_DOCSPA 
	set ID_VERSIONS_U = '3.21'
	where  ID_VERSIONS_U ='3.21.1'
	and DTA_UPDATE < sysdate - 1 ;

Insert into @db_user.DPA_DOCSPA (SYSTEM_ID, DTA_UPDATE, ID_VERSIONS_U)
 Values    (seq.nextval, sysdate, '3.21');
end;
/              
