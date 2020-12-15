
ALTER PROCEDURE [DOCSADM].[sp_modify_corr_esterno_IS] 
  /*  
  versione della SP ad hoc per "Interoperabilit semplificata", by S. Furnari:  
  oltre a introdurre e gestire il nuovo parametro SimpInteropUrl ,   
  recepisce le modifiche introdotte in questa stessa versione 3.21 by C. Ferlito 
    per gestire il nuovo campo var_desc_corr_old  
  */ 
  
  /* Da testare */
  @idcorrglobale   INT, 
  @desc_corr       VARCHAR(4000), 
  @nome            VARCHAR(4000), 
  @cognome         VARCHAR(4000), 
  @codice_aoo      VARCHAR(4000), 
  @codice_amm      VARCHAR(4000), 
  @email           VARCHAR(4000), 
  @indirizzo       VARCHAR(4000), 
  @cap             VARCHAR(4000), 
  @provincia       VARCHAR(4000), 
  @nazione         VARCHAR(4000), 
  @citta           VARCHAR(4000), 
  @cod_fiscale     VARCHAR(4000), 
  @telefono        VARCHAR(4000), 
  @telefono2       VARCHAR(4000), 
  @note            VARCHAR(4000), 
  @fax             VARCHAR(4000), 
  @var_iddoctype   INT, 
  @inrubricacomune CHAR(2000), 
  @tipourp         CHAR(2000), 
  @localita        VARCHAR(4000), 
  @luogoNascita    VARCHAR(4000), 
  @dataNascita     VARCHAR(4000), 
  @titolo          VARCHAR(4000), 

  -- aggiunto questo parametro e la gestione relativa rispetto alla vecchia versione
   @SimpInteropUrl  VARCHAR(4000), 
  @newid           INT output 
AS 
  BEGIN 
      DECLARE @error INT 
      DECLARE @no_data_error INT 
      DECLARE @cnt INT 
      DECLARE @cod_rubrica VARCHAR(128) 
      DECLARE @id_reg INT 
      DECLARE @idamm INT 
      DECLARE @new_var_cod_rubrica VARCHAR(128) 
      DECLARE @cha_dettaglio CHAR(1) = '0' 
      DECLARE @cha_tipourp CHAR(1) 
      DECLARE @myprofile INT 
      DECLARE @new_idcorrglobale INT 
      DECLARE @identitydettglobali INT 
      DECLARE @outvalue INT = 1 
      DECLARE @rtn INT 
      DECLARE @v_id_doctype INT 
      DECLARE @identitydpatcanalecorr INT 
      DECLARE @chaTipoIE CHAR(1) 
      DECLARE @numLivello INT = 0 
      DECLARE @idParent INT 
      DECLARE @idPesoOrg INT 
      DECLARE @idUO INT 
      DECLARE @idGruppo INT 
      DECLARE @idTipoRuolo INT 
      DECLARE @cha_tipo_corr CHAR(1) 
      DECLARE @chapa CHAR(1) 
      DECLARE @var_desc_old VARCHAR(256) 
      DECLARE @url VARCHAR(4000) 

      BEGIN 
          SELECT @cod_rubrica = var_cod_rubrica, 
                 @cha_tipourp = cha_tipo_urp, 
                 @id_reg = id_registro, 
                 @idamm = id_amm, 
                 @chapa = cha_pa, 
                 @chaTipoIE = cha_tipo_ie, 
                 @numLivello = num_livello, 
                 @idParent = id_parent, 
                 @idPesoOrg = id_peso_org, 
                 @idUO = id_uo, 
                 @idTipoRuolo = id_tipo_ruolo, 
                 @idGruppo = id_gruppo, 
                 @var_desc_old = var_desc_corr_old 
          FROM   dpa_corr_globali 
          WHERE  system_id = @idcorrglobale 

          SELECT @no_data_error = @@ROWCOUNT 

          IF ( @no_data_error <> 0 ) 
            PRINT 'select effettuata' 

          IF ( @no_data_error = 0 ) 
            BEGIN 
                PRINT 'primo blocco eccezione' 

                SET @outvalue = 0 

                RETURN 
            END 
      END 

      IF( @tipourp IS NOT NULL 
          AND @cha_tipourp IS NOT NULL 
          AND @cha_tipourp != @tipourp ) 
        SET @cha_tipourp = @tipourp 

      --<< dati_canale_utente >>  
      IF @cha_tipourp = 'P' 
        BEGIN 
            SELECT @v_id_doctype = id_documenttype 
            FROM   dpa_t_canale_corr 
            WHERE  id_corr_globale = @idcorrglobale 

            SELECT @no_data_error = @@ROWCOUNT 

            IF ( @no_data_error = 0 ) 
              BEGIN 
                  PRINT '2do blocco eccezione' 

                  SET @outvalue = 2 
              END 
        END 

      IF /* 0 */ @outvalue = 1 
        BEGIN 
            IF /* 1 */ @cha_tipourp = 'U' 
                        OR @cha_tipourp = 'P' 
              SET @cha_dettaglio = '1' 

        /* 1 */ 
            --VERIFICO se il corrisp è stato utilizzato come dest/mitt di protocolli
            SELECT @myprofile = Count(id_profile) 
            FROM   dpa_doc_arrivo_par 
            WHERE  id_mitt_dest = @idcorrglobale 

            -- 1) non è stato mai utilizzato come corrisp in un protocollo  
            IF /* 2 */ ( @myprofile = 0 ) 
              BEGIN 
                  UPDATE dpa_corr_globali 
                  SET    var_codice_aoo = @codice_aoo, 
                         var_codice_amm = @codice_amm, 
                         var_email = @email, 
                         var_desc_corr = @desc_corr, 
                         var_nome = @nome, 
                         var_cognome = @cognome, 
                         cha_pa = @chapa, 
                         cha_tipo_urp = @cha_tipourp, 
                         interopurl = @SimpInteropUrl 
                  WHERE  system_id = @idcorrglobale 

                  SELECT @error = @@ERROR 

                  IF ( @error <> 0 ) 
                    BEGIN 
                        PRINT '3o blocco eccezione' 

                        SET @outvalue = 3 

                        RETURN 
                    END 

                  /* SE L'UPDATE SU DPA_CORR_GLOBALI è ANDTATA A BUON FINE  
                  PER UTENTI E UO DEVO AGGIORNARE IL RECORD SULLA DPA_DETT_GLOBALI
                    */ 
                  IF /* 3 */ @cha_tipourp = 'U' 
                              OR @cha_tipourp = 'P' 
                              OR @cha_tipourp = 'F' 
                    --<< update_dpa_dett_globali2 >>  
                    BEGIN 
                        DECLARE @PrintVar VARCHAR(4000) 

                        SELECT @cnt = Count(*) 
                        FROM   dpa_dett_globali 
                        WHERE  id_corr_globali = @idcorrglobale 

                        SELECT @error = @@ERROR 

                        IF ( @error = 0 ) 
                          BEGIN 
                              IF ( @cnt = 0 ) 
                                BEGIN 
                                    SET @PrintVar = 
                                    'sono nella INSERT,id_corr_globali =  ' 
                                    + @idcorrglobale 

                                    PRINT @PrintVar 

                                    INSERT INTO dpa_dett_globali 
                                                (id_corr_globali, 
                                                 var_indirizzo, 
                                                 var_cap, 
                                                 var_provincia, 
                                                 var_nazione, 
                                                 var_cod_fiscale, 
                                                 var_telefono, 
                                                 var_telefono2, 
                                                 var_note, 
                                                 var_citta, 
                                                 var_fax, 
                                                 var_localita, 
                                                 var_luogo_nascita, 
                                                 dta_nascita, 
                                                 var_titolo) 
                                    VALUES      ( @idcorrglobale, 
                                                  @indirizzo, 
                                                  @cap, 
                                                  @provincia, 
                                                  @nazione, 
                                                  @cod_fiscale, 
                                                  @telefono, 
                                                  @telefono2, 
                                                  @note, 
                                                  @citta, 
                                                  @fax, 
                                                  @localita, 
                                                  @luogoNascita, 
                                                  @dataNascita, 
                                                  @titolo ) 

                                    SELECT @error = @@ERROR 
                                END 

                              IF ( @error = 0 ) 
                                BEGIN 
                                    IF ( @cnt = 1 ) 
                                      BEGIN 
                                          PRINT 'sono nella UPDATE' 

                                          UPDATE dpa_dett_globali 
                                          SET    var_indirizzo = @indirizzo, 
                                                 var_cap = @cap, 
                                                 var_provincia = @provincia, 
                                                 var_nazione = @nazione, 
                                                 --var_cod_fisc         = cod_fiscale,
                                                  --var_cod_pi           = partita_iva,
                                                  var_telefono = @telefono, 
                                                 var_telefono2 = @telefono2, 
                                                 var_note = @note, 
                                                 var_citta = @citta, 
                                                 var_fax = @fax, 
                                                 var_localita = @localita, 
                                                 var_luogo_nascita = @luogoNascita, 
                                                 dta_nascita = @dataNascita, 
											     var_titolo = @titolo 
											WHERE  ( id_corr_globali = @idcorrglobale ) 

  SELECT @error = @@ERROR 
  END 

  IF ( @error = 0 ) 
  BEGIN 
  PRINT 'sono nella merge' 
  END 
  END 
  END 

  IF ( @error <> 0 ) 
  BEGIN 
  SET @PrintVar = '4o blocco eccezione' + Str(@error) 

  PRINT @PrintVar 

  SET @outvalue = 4 

  RETURN 
  END 
  END 

  /* 3 */ 
  --METTI QUI UPDATE SU DPA_T_CANALE_CORR  
  --AGGIORNO LA DPA_T_CANALE_CORR  
  UPDATE dpa_t_canale_corr 
  SET    id_documenttype = @var_iddoctype 
  WHERE  id_corr_globale = @idcorrglobale 

  SELECT @error = @@ERROR 

  IF ( @error <> 0 ) 
  BEGIN 
  PRINT '5o blocco eccezione' 

  SET @outvalue = 5 

  RETURN 
  END 
  END 
		  ELSE 
		  -- caso 2) Il corrisp è stato utilizzato come corrisp in un protocollo  
		  BEGIN 
		  -- NUOVO CODICE RUBRICA  
		  SET @new_var_cod_rubrica = @cod_rubrica + '_' + CONVERT(varchar, @idcorrglobale) 

		  --<< storicizzazione_corrisp2 >>  
		  UPDATE dpa_corr_globali 
		  SET    dta_fine = Getdate(), 
		  var_cod_rubrica = @new_var_cod_rubrica, 
		  var_codice = @new_var_cod_rubrica, 
		  id_parent = NULL 
		  WHERE  system_id = @idcorrglobale 

		  SELECT @error = @@ERROR 

		  IF ( @error <> 0 ) 
		  BEGIN 
		  PRINT '6o blocco eccezione' 

		  SET @outvalue = 6 

		  RETURN 
		  END 

		  /* DOPO LA STORICIZZAZIONE DEL VECCHIO CORRISPONDENTE POSSO  
		  INSERIRE IL NUOVO CORRISPONDENTE NELLA DPA_CORR_GLOBALI */ 
		  --<< inserimento_nuovo_corrisp2 >>  
		  BEGIN 
		  IF ( @inrubricacomune = '1' ) 
		  SET @cha_tipo_corr = 'C' 
		  ELSE 
		  SET @cha_tipo_corr = 'S' 

		  INSERT INTO dpa_corr_globali 
		  (num_livello, 
		  cha_tipo_ie, 
		  id_registro, 
		  id_amm, 
		  var_desc_corr, 
		  var_nome, 
		  var_cognome, 
		  id_old, 
		  dta_inizio, 
		  id_parent, 
		  var_codice, 
		  cha_tipo_corr, 
		  cha_tipo_urp, 
		  var_codice_aoo, 
		  var_cod_rubrica, 
		  cha_dettagli, 
		  var_email, 
		  var_codice_amm, 
		  cha_pa, 
		  id_peso_org, 
		  id_gruppo, 
		  id_tipo_ruolo, 
		  id_uo, 
		  var_desc_corr_old, 
		  interopurl) 
		  VALUES      ( @numLivello, 
		  @chaTipoIE, 
		  @id_reg, 
		  @idamm, 
		  @desc_corr, 
		  @nome, 
		  @cognome, 
		  @idcorrglobale, 
		  Getdate(), 
		  @idParent, 
		  @cod_rubrica, 
		  @cha_tipo_corr, 
		  @cha_tipourp, 
		  @codice_aoo, 
		  @cod_rubrica, 
		  @cha_dettaglio, 
		  @email, 
		  @codice_amm, 
		  @chapa, 
		  @idPesoOrg, 
		  @idGruppo, 
		  @idTipoRuolo, 
		  @idUO, 
		  @var_desc_old, 
		  @SimpInteropUrl ) 

			SELECT @newid = @@identity 

		  SELECT @error = @@ERROR 

		  IF ( @error <> 0 ) 
		  BEGIN 
		  PRINT '7o blocco eccezione' 

		  SET @outvalue = 7 

		  RETURN 
		  END 
		  END 

		  /* DOPO L'INSERIMENTO DEL NUOVO CORRISPONDENTE POSSO INSERIRE IL  
		  RELATIVO RECORD NELLA DPA_DETT_GLOBALI, MA SOLO PER CORRISPONDENTI UTENTI  
		  E UNITA' ORGANIZZATIVE */ 
		  IF /* 4 */ @cha_tipourp = 'U' 
		  OR @cha_tipourp = 'P' 
		  --PRENDO LA SYSTEM_ID APPENA INSERITA  
		  BEGIN 
		  SELECT @identitydettglobali = @@identity 

		  --<< inserimento_dettaglio_corrisp2 >>  
		  INSERT INTO dpa_dett_globali 
		  (id_corr_globali, 
		  var_indirizzo, 
		  var_cap, 
		  var_provincia, 
		  var_nazione, 
		  var_cod_fiscale, 
		  var_telefono, 
		  var_telefono2, 
		  var_note, 
		  var_citta, 
		  var_fax, 
		  var_localita, 
		  var_luogo_nascita, 
		  dta_nascita, 
		  var_titolo) 
		  VALUES      ( @newid, 
		  @indirizzo, 
		  @cap, 
		  @provincia, 
		  @nazione, 
		  @cod_fiscale, 
		  @telefono, 
		  @telefono2, 
		  @note, 
		  @citta, 
		  @fax, 
		  @localita, 
		  @luogoNascita, 
		  @dataNascita, 
		  @titolo ) 

		  SELECT @error = @@ERROR 

		  IF ( @error <> 0 ) 
		  BEGIN 
		  PRINT '8o blocco eccezione' 

		  SET @outvalue = 8 

		  RETURN 
		  END 
		  END 

		  /* 4 */ 
		  --INSERISCO IL CANALE PREFERITO DEL NUOVO CORRISP ESTERNO SIA ESSO UO, RUOLO, PERSONA
		   --<< inserimento_dpa_t_canale_corr2 >>  
		  BEGIN 
		  INSERT INTO dpa_t_canale_corr 
		  (id_corr_globale, 
		  id_documenttype, 
		  cha_preferito) 
		  VALUES      (@newid, 
		  @var_iddoctype, 
		  '1') 

		  SELECT @error = @@ERROR, 
		  @identitydpatcanalecorr = @@identity 

		  IF ( @error <> 0 ) 
		  BEGIN 
		  PRINT '9o blocco eccezione' 

		  SET @outvalue = 9 

		  RETURN 
		  END 
		  END 
		  END 

		  --se fa parte di una lista, allora la devo aggiornare.  
		  IF @newid IS NOT NULL 
		  UPDATE dpa_liste_distr 
		  SET    id_dpa_corr = @newid 
		  FROM   dpa_liste_distr d 
		  WHERE  d.id_dpa_corr = @idcorrglobale 
		END 

  /* 2 */ 
      /* 0 */ 
      RETURN @outvalue 
  END  
GO


