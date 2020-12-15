

-- no need to backup, in caso uncomment these lines

--Begin Utl_Backup_Plsql_Code    ( 'PROCEDURE', 'SP_MODIFY_UO_FATTURAZIONE');
--end;
--/


create or replace
PROCEDURE SP_MODIFY_UO_FATTURAZIONE (
	oldCodiceUO	IN	VARCHAR2,
	newCodiceUO	IN	VARCHAR2,
	idAmm		IN	NUMBER,
	codiceAoo	IN	VARCHAR2,
  returnValue 	OUT	NUMBER
)

Is
	codiceAmmIpa 	VARCHAR2(128);
	codiceAooIpa	VARCHAR2(128);
	codiceUac 	VARCHAR2(128);
	codiceClassificazione 	VARCHAR2(128);
  isFatturazione VARCHAR2(128) := NULL;
		
 
begin 

	-- selezione il codice ipa amministrazione tramite l'id
	SELECT var_codice_amm_ipa
	INTO codiceAmmIpa
	FROM dpa_amministra 
	WHERE system_id = idAmm;

	--selezione del codice ipa aoo tramite il codice AOO
	SELECT var_codice_aoo_ipa
	INTO codiceAooIpa
	FROM dpa_el_registri
	WHERE var_codice = codiceAoo AND CHA_RF=0;
  
  
  SELECT var_desc_atto
  INTO isFatturazione
  FROM DPA_TIPO_ATTO
  WHERE ID_AMM= idAmm AND var_desc_atto = 'Fattura elettronica';

	-- caso di inserimento della UO in PITRE: mi accorgo che si tratta di una nuova UO perchè oldCodiceUO è null
	if (oldCodiceUO IS NULL)
  THEN

    BEGIN
        SELECT codice_uac, codice_classificazione
        INTO codiceUac, codiceClassificazione
        FROM dpa_el_registri
        WHERE Id_Amm= idAmm AND var_codice = codiceAoo;

      end;

    if(isFatturazione is not NULL)
    then
    BEGIN
        INSERT INTO DPA_DATI_FATTURAZIONE 
        (system_id, codice_amm, codice_aoo, codice_uo, codice_uac, codice_classificazione, var_utente_proprietario, var_tipologia_documento, var_ragione_trasmissione)
        VALUES(SEQ_DATI_FATTURAZIONE.Nextval, codiceAmmIpa, codiceAooIpa, newCodiceUO, codiceUac, codiceClassificazione, 'PROVA_TIBCO', 'Fattura elettronica', 'Ricevimento fattura');
        returnValue:=SQL%ROWCOUNT;
         EXCEPTION
         WHEN OTHERS  THEN
        dbms_output.put_line('5o blocco eccezione') ; 
        returnvalue := 5;
        RETURN;
    END;
   
    END IF;


      -- se outValue è 0, allora la UO non è presente nella tabella TIBCO; in tal caso si avvia il job
      -- che si occupa di ritentare l'aggiornamento nella tabella
	
	ELSE
	-- caso di modifica del codice UO in PITRE
	BEGIN
		UPDATE DPA_DATI_FATTURAZIONE
		SET codice_uo = newCodiceUO
		WHERE UPPER(codice_amm) = UPPER(codiceAmmIpa) AND UPPER(codice_aoo) = UPPER(codiceAooIpa) AND UPPER(codice_uo) = UPPER(oldCodiceUO);
		returnValue:=SQL%ROWCOUNT;
     EXCEPTION
    WHEN OTHERS  THEN
      dbms_output.put_line('6o blocco eccezione') ; 
      returnvalue := 6;
      RETURN;
	END;
	END IF;

end;
/

