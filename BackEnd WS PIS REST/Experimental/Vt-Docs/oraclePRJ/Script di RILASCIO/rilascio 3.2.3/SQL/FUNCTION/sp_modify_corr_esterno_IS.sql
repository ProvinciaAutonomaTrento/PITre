
ALTER PROCEDURE  [DOCSADM].[sp_modify_corr_esterno_IS] 
/*
versione della SP ad hoc per "Interoperabilit semplificata", by S. Furnari:
oltre a introdurre e gestire il nuovo parametro SimpInteropUrl , 
recepisce le modifiche introdotte in questa stessa versione 3.21 by C. Ferlito 
per gestire il nuovo campo var_desc_corr_old
*/
@idcorrglobale     INT                        ,
@desc_corr         VARCHAR(4000)    ,
@nome              VARCHAR(4000)    ,
@cognome           VARCHAR(4000)    ,
@codice_aoo        VARCHAR(4000)    ,
@codice_amm        VARCHAR(4000)    ,
@email             VARCHAR(4000)    ,
@indirizzo         VARCHAR(4000)    ,
@cap               VARCHAR(4000)    ,
@provincia         VARCHAR(4000)    ,
@nazione           VARCHAR(4000)    ,
@citta             VARCHAR(4000)    ,
@cod_fiscale       VARCHAR(4000)    ,
@partita_iva       VARCHAR(4000)    ,
--@codice_ipa              VARCHAR(4000)    , -- attualmente non usato (adeguamento normativa in sviluppo)
@telefono          VARCHAR(4000)    ,
@telefono2         VARCHAR(4000)    ,
@note              VARCHAR(4000)    ,
@fax               VARCHAR(4000)    ,
@var_iddoctype     INT                        ,
@inrubricacomune   CHAR(2000)          ,
@tipourp           CHAR(2000)          ,
@localita          VARCHAR(4000)    ,
@luogoNascita      VARCHAR(4000)    ,
@dataNascita       VARCHAR(4000)    ,
@titolo            VARCHAR(4000)    ,
@SimpInteropUrl    VARCHAR(4000)    ,
@IdPeople          INT,
@IdGruppoPeople    INT,
@IdRegistro        INT,
@newid             INT OUTPUT      
--@returnvalue       INT OUTPUT 
AS
BEGIN
-- << REPERIMENTO_DATI >>
       DECLARE @cnt                                  INT
       DECLARE @cod_rubrica                          VARCHAR(128)
       DECLARE @id_reg                                     INT
       DECLARE @idamm                                      INT
       DECLARE @new_var_cod_rubrica           VARCHAR(128)
       DECLARE @new_var_cod_rubrica1          VARCHAR(128)
       DECLARE @cha_dettaglio                        CHAR(1) = '0'
       DECLARE @cha_tipourp                          CHAR(1)
       DECLARE @myprofile                            INT
       DECLARE @new_idcorrglobale             INT
       DECLARE @identitydettglobali           INT
       DECLARE @outvalue                             INT       = 0
       DECLARE @rtn                                  INT
       DECLARE @v_id_doctype                         INT
       DECLARE @identitydpatcanalecorr        INT
       DECLARE @chaTipoIE                            CHAR(1)
       DECLARE @numLivello                           INT       = 0
       DECLARE @idParent                             INT
       DECLARE @idPesoOrg                            INT
       DECLARE @idUO                                       INT
       DECLARE @idGruppoOld                          INT
       DECLARE @idTipoRuolo                          INT
       DECLARE @cha_tipo_corr                        CHAR(1)
       DECLARE @chapa                                      CHAR(1)
       DECLARE @var_desc_old                         VARCHAR(256)
       DECLARE @v_curdoc_docnumber       INT
       DECLARE @v_curdoc_idtemplate      INT
    DECLARE @v_curdoc_idoggetto       INT
    DECLARE @v_curfasc_idproject      INT
    DECLARE @v_curfasc_idtemplate     INT
    DECLARE @V_Curfasc_Idoggetto      INT
       DECLARE @Countprofilato                INT     = 0
    DECLARE @Countprofdoc             INT     = 0
    DECLARE @countProfFasc            INT     = 0
    DECLARE @IdRuolo                  INT
       DECLARE @url                                  VARCHAR(4000)
       
       DECLARE @error                                      INT
       DECLARE @no_data_error                        INT

       DECLARE cursor_doc CURSOR
    FOR select at.doc_number, at.id_template, at.id_oggetto 
      from dpa_associazione_templates at, dpa_oggetti_custom oc, dpa_tipo_oggetto dto 
      where at.valore_oggetto_db = CAST(@idcorrglobale AS varchar)
      and at.ID_OGGETTO = oc.SYSTEM_ID
      and oc.ID_TIPO_OGGETTO = dto.SYSTEM_ID
         and upper(dto.DESCRIZIONE) = 'CORRISPONDENTE';
       

       DECLARE cursor_fasc CURSOR
             FOR select atf.id_project, atf.id_template, atf.id_oggetto 
               from dpa_ass_templates_fasc atf, dpa_oggetti_custom_fasc ocf, dpa_tipo_oggetto_fasc tof
               where atf.valore_oggetto_db = CAST(@idcorrglobale AS varchar)
               and atf.ID_OGGETTO = ocf.SYSTEM_ID
               and ocf.ID_TIPO_OGGETTO = tof.SYSTEM_ID
               and upper(tof.DESCRIZIONE) = 'CORRISPONDENTE';

       BEGIN
       
             SELECT   @cod_rubrica = var_cod_rubrica
                    , @cha_tipourp = cha_tipo_urp
                    , @id_reg = id_registro
                    , @idamm = id_amm
                    , @chapa = cha_pa
                    , @chaTipoIE = cha_tipo_ie
                    , @numLivello = num_livello
                    , @idParent = id_parent
                    , @idPesoOrg = id_peso_org
                    , @idUO = id_uo
                    , @idTipoRuolo = id_tipo_ruolo
                    , @idGruppoOld = id_gruppo
                    , @var_desc_old = var_desc_corr_old
                    --, @url = InteropUrl
             FROM DOCSADM.DPA_CORR_GLOBALI
             WHERE system_id = @idcorrglobale
             
             Select @Idruolo = System_Id  
          from dpa_corr_globali where id_gruppo = @IdGruppoPeople;
                    
             IF(@tipourp IS NOT NULL AND @cha_tipourp IS NOT NULL AND @cha_tipourp != @tipourp)
                    SET @cha_tipourp = @tipourp
          
          
             IF @cha_tipourp = 'U' OR @Cha_Tipourp = 'P' OR @Cha_Tipourp = 'F' 
                    SET @cha_dettaglio = '1'
             
             SELECT @no_data_error = @@ROWCOUNT
                    
                    IF (@no_data_error <> 0)
                          PRINT 'select effettuata' 
                    IF (@no_data_error = 0)
                    BEGIN
                          PRINT 'Primo blocco eccezione' 
                          SET @outvalue = 0
                          RETURN
                    END
             END -- << REPERIMENTO_DATI >>
          
             -- << DATI_CANALE_UTENTE >>
             if @cha_tipourp = 'P'
             BEGIN
                    SELECT   @v_id_doctype = id_documenttype
                    FROM DOCSADM.DPA_T_CANALE_CORR
                    WHERE id_corr_globale = @idcorrglobale

                    SELECT @no_data_error = @@ROWCOUNT
             
                    IF (@no_data_error = 0)
                    BEGIN
                          PRINT 'Secondo blocco eccezione' 
                          SET @outvalue = 2
                    END
             END  

--controlli preliminari
       --VERIFICO SE IL CORRISP è STATO UTILIZZATO COME DEST/MITT DI PROTOCOLLI
             SELECT @myprofile = COUNT(ID_PROFILE)
             FROM DOCSADM.DPA_DOC_ARRIVO_PAR
             WHERE ID_MITT_DEST = @idcorrglobale
             
             -- verifico se il corrispondente é stato usato o meno nei campi profilati
             SELECT @Countprofdoc = Count(System_Id) 
             From Dpa_Associazione_Templates 
             where valore_oggetto_db = cast(@IDCorrGlobale as varchar)
                      
             Select @Countproffasc = Count(System_Id) 
             from dpa_ass_templates_fasc
             where valore_oggetto_db = cast(@IdCorrGlobale as varchar)
          
    SET @Countprofilato = @Countprofdoc + @Countproffasc;
       
       PRINT '@myprofile'  + cast(@myprofile as varchar)
       --FINE controlli preliminari
       
-- 1) non ?? stato mai utilizzato come corrisp in un protocollo         /* 2 */ 
             IF(@myprofile = 0 AND @Countprofilato = 0)
             BEGIN
                    
                    UPDATE DOCSADM.dpa_t_canale_corr
                          SET id_documenttype = @var_iddoctype
                    WHERE id_corr_globale = @idcorrglobale
             
                    SELECT   @error = @@ERROR
            
                    IF (@error <> 0)
                    BEGIN
                          PRINT '5o blocco eccezione' 
                          SET @outvalue = 3
                          RETURN
                    END
       
                    If(@IdRegistro Is Not Null)
                    BEGIN
                          PRINT 'idRegistro is not null'
                          
                          UPDATE DOCSADM.DPA_CORR_GLOBALI 
                          SET var_codice_aoo = @codice_aoo
                                 ,var_codice_amm = @codice_amm
                                 ,var_email = @email
                                 ,var_desc_corr = @desc_corr
                                 ,var_nome = @nome
                                 ,var_cognome = @cognome
                                 ,cha_pa = @chapa
                                 ,cha_tipo_urp = @cha_tipourp
                                 ,InteropUrl = @SimpInteropUrl
                                 ,id_registro = @IdRegistro
                          WHERE system_id = @idcorrglobale
                          
                          SELECT   @error = @@ERROR
                          IF(@error <> 0)
                          BEGIN
                                 PRINT '3o blocco eccezione' 
                                 SET @outvalue = 3
                                 RETURN
                          END
                    END
                    ELSE
                    BEGIN
                          PRINT 'idRegistro null'
                          
                          UPDATE dpa_corr_globali
                          SET var_codice_aoo = @codice_aoo,
                          var_codice_amm = @codice_amm,
                          var_email = @email,
                          var_desc_corr = @desc_corr,
                          var_nome = @nome,
                          var_cognome = @cognome,
                          cha_pa = @chapa,
                          cha_tipo_urp = @cha_tipourp,
                          InteropUrl = @SimpInteropUrl
                          WHERE system_id = @idcorrglobale
                          SELECT   @error = @@ERROR
                          IF(@error <> 0)
                          BEGIN
                                 PRINT '4o blocco eccezione' 
                                 SET @outvalue = 4
                                 RETURN
                          END
                    END
                    
                    /* SE L'UPDATE SU DPA_CORR_GLOBALI è ANDTATA A BUON FINE PER UTENTI E UO DEVO AGGIORNARE IL RECORD SULLA DPA_DETT_GLOBALI */
                    PRINT @cha_tipourp
                    IF @cha_tipourp = 'U' OR @cha_tipourp = 'P' OR @cha_tipourp = 'F'
                    BEGIN -- << UPDATE_DPA_DETT_GLOBALI2 >>
                          DECLARE @PrintVar VARCHAR(4000)
                          SELECT   @cnt = count(*)
                          FROM DOCSADM.DPA_DETT_GLOBALI
                          WHERE ID_CORR_GLOBALI = @idcorrglobale 
                          
                          SELECT   @error = @@ERROR
                          
                          IF (@cnt = 0)
                          BEGIN
                                 SET @PrintVar = 'SONO NELLA INSERT,ID_CORR_GLOBALI =  '+cast(@idcorrglobale as varchar)
                                 PRINT @PrintVar
                                 INSERT INTO DOCSADM.DPA_DETT_GLOBALI
                                 (
                                       id_corr_globali
                                       , var_indirizzo
                                       , var_cap
                                       , var_provincia
                                       , var_nazione
                                       , var_cod_fiscale
                                       , VAR_COD_PI
                                       --, VAR_COD_IPA
                                       , var_telefono
                                       , var_telefono2
                                       , var_note
                                       , var_citta
                                       , var_fax
                                       , var_localita
                                       , var_luogo_nascita
                                       , dta_nascita
                                       , var_titolo
                                 )
                                 VALUES
                                 (@idcorrglobale, @indirizzo, @cap, @provincia
                                       , @nazione, @cod_fiscale, @partita_iva 
                                       --, @codice_ipa
                                       , @telefono, @telefono2
                                       , @note, @citta, @fax, @localita
                                       , @luogoNascita, @dataNascita, @titolo)
                          END
               
                        SELECT   @error = @@ERROR
                             
                          IF (@error = 0)
                          BEGIN
                                 IF (@cnt = 1)
                                 BEGIN
                                       PRINT 'SONO NELLA UPDATE di dpa_dett_globali'
                                       UPDATE DOCSADM.DPA_DETT_GLOBALI SET 
                                              var_indirizzo = @indirizzo
                                              ,var_cap = @cap
                                              ,var_provincia = @provincia
                                              ,var_nazione = @nazione
                                              ,var_cod_fiscale = @cod_fiscale
                                              ,VAR_COD_PI = @partita_iva
                                              --,VAR_COD_IPA = @codice_ipa
                                              ,var_telefono = @telefono
                                              ,var_telefono2 = @telefono2
                                              ,var_note = @note
                                              ,var_citta = @citta
                                              ,var_fax = @fax
                                              ,var_localita = @localita
                                              ,var_luogo_nascita = @luogoNascita
                                              ,dta_nascita = @dataNascita
                                              ,var_titolo = @titolo  
                                       WHERE (id_corr_globali = @idcorrglobale)
                                       
                                       SELECT   @error = @@ERROR
                                              
                                       IF (@error = 0)
                                       BEGIN
                                              PRINT 'SONO NELLA MERGE' 
                                       END
                                 END
                          END
                          
                          IF (@error <> 0)
                          BEGIN
                                 SET @PrintVar = '6o blocco eccezione'+ str(@error)
                                 PRINT @PrintVar
                                 SET @outvalue = 6
                                 RETURN
                          END
                    END --Update_Dpa_Dett_Globali 2
             END
ELSE
-- occorre storicizzare il corrispondente 
-- ramo else /* 1 */ (myprofile = 0 AND Countprofilato = 0)  
-- perché il corrisp è stato utilizzato in un protocollo
-- storicizzo il corrispondente
-- Ricavo il codice rubrica del corrispondente      
       BEGIN
             Select @new_var_cod_rubrica1 = Var_Cod_Rubrica                    
             FROM dpa_corr_globali
             WHERE system_id = @IDCorrGlobale
       
             -- Costruisco il codice rubrica da attribuire la corrispondente storicizzato
             SET @new_var_cod_rubrica1 = @cod_rubrica + '_' + cast(@idcorrglobale as varchar)
             UPDATE DPA_CORR_GLOBALI
               SET DTA_FINE = GetDate(),
                    var_cod_rubrica = @new_var_cod_rubrica1,
                    var_codice      = @new_var_cod_rubrica1,
                    Id_Parent = Null
               WHERE SYSTEM_ID = @IDCorrGlobale

                          BEGIN --<<inserimento_nuovo_corrisp>>
                                 If (@Inrubricacomune = '1')                        
                                       SET @cha_tipo_corr = 'C';
                                 ELSE
                                       SET @cha_tipo_corr = 'S';

                                 If(@IdRegistro Is Not Null) 
                        BEGIN
                            INSERT INTO dpa_corr_globali
                            (num_livello, cha_tipo_ie, id_registro,
                            id_amm, var_desc_corr, var_nome, var_cognome,
                            id_old, dta_inizio, id_parent, var_codice,
                            cha_tipo_corr, cha_tipo_urp,
                            var_codice_aoo, var_cod_rubrica, cha_dettagli,
                            var_email, var_codice_amm, cha_pa, id_peso_org,
                            id_gruppo, id_tipo_ruolo, id_uo, var_desc_corr_old, InteropUrl
                           )
                            VALUES (@numLivello, @chaTipoIE, @IdRegistro,
                            @idamm, @desc_corr, @nome, @cognome,
                            @idcorrglobale, GetDate(), @idParent, @cod_rubrica,
                            @cha_tipo_corr, @cha_tipourp,
                            @codice_aoo, @cod_rubrica, @cha_dettaglio,
                            @email, @codice_amm, @chapa, @idPesoOrg,
                            @idGruppoOld, @idTipoRuolo, @idUO, @var_desc_old , @SimpInteropUrl
                           )
                            SELECT   @error = @@ERROR
                                              IF (@error <> 0)
                                              BEGIN
                                                    SET @PrintVar = '7o blocco eccezione'
                                                    SET @outvalue = 7
                                                    RETURN
                            END
                                       END
                                 ELSE
                        BEGIN
                            INSERT INTO dpa_corr_globali
                            (num_livello, cha_tipo_ie, id_registro,
                            id_amm, var_desc_corr, var_nome, var_cognome,
                            id_old, dta_inizio, id_parent, var_codice,
                            cha_tipo_corr, cha_tipo_urp,
                            var_codice_aoo, var_cod_rubrica, cha_dettagli,
                            var_email, var_codice_amm, cha_pa, id_peso_org,
                            id_gruppo, id_tipo_ruolo, id_uo, var_desc_corr_old, InteropUrl
                            )
                            VALUES (@numLivello, @chaTipoIE, @id_reg,
                            @idamm, @desc_corr, @nome, @cognome,
                            @idcorrglobale, GetDate(), @idParent, @cod_rubrica,
                            @cha_tipo_corr, @cha_tipourp,
                            @codice_aoo, @cod_rubrica, @cha_dettaglio,
                            @email, @codice_amm, @chapa, @idPesoOrg,
                            @idGruppoOld, @idTipoRuolo, @idUO, @var_desc_old , @SimpInteropUrl
                            )
                            SELECT   @error = @@ERROR
                                              IF (@error <> 0)
                                              BEGIN
                                                    SET @PrintVar = '7o blocco eccezione'
                                                    SET @outvalue = 7
                                                    RETURN
                            END
                        END
                          END --<<Fine inserimento_nuovo_corrisp>>
                          
             --INSERISCO IL CANALE PREFERITO DEL NUOVO CORRISP ESTERNO SIA ESSO UO, RUOLO, PERSONA
             BEGIN --<<inserimento_dpa_t_canale_corr>>
                    SET @newid = @@IDENTITY
                    Insert Into Dpa_T_Canale_Corr                           
                    (id_corr_globale, id_documenttype,cha_preferito)
                    Values (@Newid, @Var_Iddoctype,'1')
                    
                    SELECT   @error = @@ERROR

                    IF (@error <> 0)
                    BEGIN
                          PRINT '9o blocco eccezione' 
                          SET @outvalue = 9
                          RETURN
                    END
             END --inserimento_dpa_t_canale_corr           
             
       END
       /* DOPO L'INSERIMENTO DEL NUOVO CORRISPONDENTE POSSO INSERIRE IL
    RELATIVO RECORD NELLA DPA_DETT_GLOBALI, MA SOLO PER CORRISPONDENTI UTENTI
    E UNITA' ORGANIZZATIVE */
    IF @cha_tipourp = 'U' OR @cha_tipourp = 'P' Or @Cha_Tipourp = 'F'
       BEGIN
             -- Modifica 06/11/2014
             --SELECT   @identitydettglobali = @@IDENTITY
             PRINT '@identitydettglobali-->' + cast(@identitydettglobali as varchar)
             --<< inserimento_dettaglio_corrisp2 >>
             BEGIN
            
            -- Modifica 06/11/2014     
                    --SET IDENTITY_INSERT DOCSADM.dpa_dett_globali ON
                    
                    -- Modifica 06/11/2014
                    INSERT INTO DOCSADM.DPA_DETT_GLOBALI
                    (
                          --system_id, 
                           id_corr_globali
                          , var_indirizzo
                          , var_cap
                          , var_provincia
                          , var_nazione
                          , var_cod_fiscale
                          , VAR_COD_PI
                          --, VAR_COD_IPA
                          , var_telefono
                          , var_telefono2
                          , var_note
                          , var_citta
                          , var_fax
                          , var_localita
                          , var_luogo_nascita
                          , dta_nascita
                          , var_titolo
                    )
                    VALUES
                    (
                          --@identitydettglobali,
                          @newid
                          , @indirizzo
                          , @cap
                          , @provincia
                          , @nazione
                          , @cod_fiscale
                          , @partita_iva
                          --, @codice_ipa
                          , @telefono
                          , @telefono2
                          , @note
                          , @citta
                          , @fax
                          , @localita
                          , @luogoNascita
                          , @dataNascita
                          , @titolo
                    )
                    
                    -- Modifica 06/11/2014
                    --SET IDENTITY_INSERT dpa_dett_globali OFF
             
                    SELECT @error = @@ERROR
           
                    IF (@error <> 0)
                    BEGIN
                          PRINT '8o blocco eccezione' 
                          SET @outvalue = 8
                          RETURN
                    END
             END 
       END
       
       IF(@countProfDoc > 0) 
    BEGIN
             OPEN cursor_doc
             FETCH cursor_doc INTO @v_curdoc_docnumber,@v_curdoc_idtemplate,@v_curdoc_idoggetto
             WHILE (@@FETCH_STATUS = 0)
             BEGIN
                    INSERT INTO dpa_profil_sto
                          (id_template,
                          dta_modifica, id_profile, id_ogg_custom,
                          id_people, id_ruolo_in_uo, var_desc_modifica
                          )
                    VALUES (@v_curdoc_idtemplate,
                          GetDate(), @v_curdoc_docnumber, @v_curdoc_idoggetto,
                          @IdPeople, @IdRuolo, 'Corrispondente storicizzato per modifica da rubrica')
             
                    FETCH cursor_doc INTO @v_curdoc_docnumber,@v_curdoc_idtemplate,@v_curdoc_idoggetto
             END
             CLOSE cursor_doc
             DEALLOCATE cursor_doc
     END
     
    IF(@countProfFasc > 0) 
       BEGIN
             OPEN cursor_fasc
             FETCH cursor_fasc INTO @v_curfasc_idtemplate,@v_curfasc_idproject,@v_curfasc_idoggetto
             WHILE (@@FETCH_STATUS = 0)
             BEGIN
                    INSERT INTO dpa_profil_fasc_sto
                                 (id_template,
                                 dta_modifica, id_project, id_ogg_custom,
                                 id_people, id_ruolo_in_uo, var_desc_modifica
                                 )
                          VALUES (@v_curfasc_idtemplate,
                                 GetDate(), @v_curfasc_idproject, @v_curfasc_idoggetto,
                                 @IdPeople, @IdRuolo, 'Corrispondente storicizzato per modifica da rubrica'
                                 )
                    FETCH cursor_fasc INTO @v_curfasc_idtemplate,@v_curfasc_idproject,@v_curfasc_idoggetto
             END
             CLOSE cursor_fasc
             DEALLOCATE cursor_fasc
       END
       
       IF (NOT @newid IS NULL )
       BEGIN 
             Update DOCSADM.Dpa_Liste_Distr 
             Set Id_Dpa_Corr=@newid 
             where ID_DPA_CORR=@idcorrglobale
       END
       
       SET @outvalue = @newid;
    RETURN @outvalue 
    
END




