-- STORED PROCEDURE NON TESTATA, SOLO TRADOTTA DA ORACLE
USE [PCM_330]
GO
/****** Object:  StoredProcedure [DOCSADM].[SP_INS_AREA_CONS_VINCOLI]    Script Date: 12/06/2013 17:42:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [DOCSADM].[SP_INS_AREA_CONS_VINCOLI]
@idAmm int,
@idPeople int,
@idProfile int,
@idProject int,
@codFasc varchar(64),
@oggetto varchar(64),
@tipoDoc char,
@idGruppo int,
@idRegistro int,
@docNumber int,
@userId varchar(32),
@tipoOggetto char,
@tipoAtto   VARCHAR,
@idpolicy   int,
@dimIstanzaViolato int,
@numDocIstanzaViolato int,
@vincoloDim int,    -- valore massimo BYTE dimensione istanza comprensivo di tolleranza
@vincoloNumDoc int,    -- numero di documenti massimi all'interno di un'istanza
@sizeItem int,
@result int  OUT
AS
BEGIN

DECLARE @idRuoloInUo int
DECLARE @id_cons_1   int
DECLARE @id_cons_2   int
DECLARE @res         int
DECLARE @dimIstCorr  int 
DECLARE @dimIst      int
DECLARE @dimMaxByte  int
DECLARE @numDocIstCorr int 
DECLARE @numDocIst   int
DECLARE @VdimIstanzaViolato bit --0/1
DECLARE @VnumDocIstanzaViolato bit --0/1
DECLARE @isIstanzaPrefered	char



SET @result = -1
SET @idRuoloInUo = 0
SET @id_cons_1 = 0
SET @res = 0
SET @id_cons_2 = 0
SET @dimIstCorr = 0
SET @dimIst = 0
SET @dimMaxByte = 0
SET @numDocIstCorr = 0
SET @numDocIst = 0
SET @VdimIstanzaViolato = 0 -- false
SET @VnumDocIstanzaViolato = 0 --false
SET @isIstanzaPrefered = '0'


IF(@dimIstanzaViolato = 1)
    SET @VdimIstanzaViolato = 1
ELSE
    SET @VdimIstanzaViolato = 0

IF(@numDocIstanzaViolato = 1)
    SET @VnumDocIstanzaViolato = 1
ELSE
    SET @VnumDocIstanzaViolato = 0
        
SELECT @idRuoloInUo = DPA_CORR_GLOBALI.SYSTEM_ID FROM DPA_CORR_GLOBALI
WHERE DPA_CORR_GLOBALI.ID_GRUPPO = @idGruppo

IF (@idconservazione IS NULL)
    BEGIN
		-- Reperimento istanza di conservazione Preferita
		SELECT @res = dpa_area_conservazione.system_id
		FROM dpa_area_conservazione
		WHERE dpa_area_conservazione.id_people = @idpeople
		AND dpa_area_conservazione.id_ruolo_in_uo = @idruoloinuo
		AND dpa_area_conservazione.cha_stato = 'N'
		AND dpa_area_conservazione.is_preferred ='1'
		order by dpa_area_conservazione.system_id desc
		-- Condizione per determinare le policy Manuali
		-- AND id_policy is null
	END
ELSE
	BEGIN
		SET @res = @idconservazione
	END

IF (@res is null or @res = 0)
	BEGIN
		SELECT DISTINCT @res = DPA_AREA_CONSERVAZIONE.SYSTEM_ID
		FROM DPA_AREA_CONSERVAZIONE
		WHERE
		DPA_AREA_CONSERVAZIONE.ID_PEOPLE = @idPeople AND
		DPA_AREA_CONSERVAZIONE.ID_RUOLO_IN_UO = @idRuoloInUo AND
		DPA_AREA_CONSERVAZIONE.CHA_STATO = 'N'
		order by dpa_area_conservazione.system_id desc
	END

IF (@res > 0)
	BEGIN
		-- Calcolo DIMENSIONE CORRENTE istanza di conservazione
		SELECT @dimIst = SUM(dpa_items_conservazione.size_item)
        FROM dpa_items_conservazione 
        WHERE dpa_items_conservazione.id_conservazione = @res
		
		IF(@dimIst is null)
			BEGIN
                SET @dimIst = 0
            END
		
		SET @dimIstCorr = @dimIst + @sizeItem
		
		-- Variabile per dimensione massima istanza in byte
		-- @dimMaxByte = @vincoloDim * 1024 * 1024; -- Era in MB, è stata passata direttamente in Byte
		@dimMaxByte = @vincoloDim
		
		-- Verifico la validità del vincolo
        IF(@dimIstCorr > @dimMaxByte)
            BEGIN
                SET @VdimIstanzaViolato = 1
			END
        ELSE
			BEGIN
                SET @VdimIstanzaViolato = 0
            END
		
		-- Calcolo NUMERO DI DOCUMENTI corrente istanza di conservazione
        SELECT @numDocIst = COUNT(dpa_items_conservazione.system_id)
        FROM dpa_items_conservazione 
        WHERE dpa_items_conservazione.id_conservazione = @res
		
		SET @numDocIstCorr = @numDocIst + 1
		
		-- Verifico la validità del vincolo
        IF(@numDocIstCorr > @vincoloNumDoc)
            BEGIN 
                SET @VnumDocIstanzaViolato = 1
			END
        ELSE
			BEGIN
                SET @VnumDocIstanzaViolato = 0
            END
		
		-- Verifico se istanza è preferred
		SELECT @isIstanzaPrefered = dpa_area_conservazione.is_preferred 
        FROM dpa_area_conservazione
        WHERE dpa_area_conservazione.system_id = @res
	
	------------------------	
	
		-- VINCOLO SULLA DIMENSIONE ISTANZE VIOLATO
		IF(@VdimIstanzaViolato = 1)
			BEGIN
				-- VINCOLO NUMERO DOCUMENTI VIOLATO
                IF (@VnumDocIstanzaViolato = 1)
					BEGIN
						-- INSERIMENTO IN UNA NUOVA ISTANZA DI CONSERVAZIONE
						
						-- Controllo se istanza è predefinita
                        IF(@isIstanzaPrefered = '1')
							BEGIN
								--Aggiorno la vecchia a non preferred
								UPDATE DPA_AREA_CONSERVAZIONE
								SET DPA_AREA_CONSERVAZIONE.IS_PREFERRED = NULL
								WHERE DPA_AREA_CONSERVAZIONE.SYSTEM_ID = @res
								
								-- è un'istanza predefinita
								INSERT INTO DPA_AREA_CONSERVAZIONE
								 (ID_AMM, ID_PEOPLE, ID_RUOLO_IN_UO,
								  CHA_STATO, DATA_APERTURA, USER_ID, ID_GRUPPO, ID_POLICY,
								  VAR_FILE_CHIUSURA, VAR_FILE_CHIUSURA_FIRMATO, IS_PREFERRED
								 )
								VALUES (@idAmm, @idPeople, @idRuoloInUo,
								  'N', getdate(), @userId, @idGruppo, @idpolicy,
								  NULL,NULL,'1'
								)
								
								SET @id_cons_1=SCOPE_IDENTITY()
								
								INSERT INTO DPA_ITEMS_CONSERVAZIONE
								 (ID_CONSERVAZIONE, ID_PROFILE, ID_PROJECT,
								  CHA_TIPO_DOC, VAR_OGGETTO, ID_REGISTRO, DATA_INS,
								  CHA_STATO, VAR_XML_METADATI, COD_FASC, DOCNUMBER,
								  CHA_TIPO_OGGETTO, VAR_TIPO_ATTO
								 )
								VALUES (@id_cons_1, @idProfile, @idProject,
								  @tipoDoc, @oggetto, @idRegistro, getdate(),
								  'N', NULL, @codFasc, @docNumber,
								  @tipoOggetto, @tipoAtto
								)

								--p_result := id_cons_2;
								SET @result = SCOPE_IDENTITY()
							END
						ELSE
							BEGIN
								-- non è predefinita
								INSERT INTO DPA_AREA_CONSERVAZIONE
								 (ID_AMM, ID_PEOPLE, ID_RUOLO_IN_UO,
								  CHA_STATO, DATA_APERTURA, USER_ID, ID_GRUPPO, ID_POLICY,
								  VAR_FILE_CHIUSURA, VAR_FILE_CHIUSURA_FIRMATO
								 )
								VALUES (@idAmm, @idPeople, @idRuoloInUo,
								  'N', getdate(), @userId, @idGruppo, @idpolicy,
								  NULL,NULL
								)
								
								SET @id_cons_1=SCOPE_IDENTITY()
								
								INSERT INTO DPA_ITEMS_CONSERVAZIONE
								 (ID_CONSERVAZIONE, ID_PROFILE, ID_PROJECT,
								  CHA_TIPO_DOC, VAR_OGGETTO, ID_REGISTRO, DATA_INS,
								  CHA_STATO, VAR_XML_METADATI, COD_FASC, DOCNUMBER,
								  CHA_TIPO_OGGETTO, VAR_TIPO_ATTO
								 )
								VALUES (@id_cons_1, @idProfile, @idProject,
								  @tipoDoc, @oggetto, @idRegistro, getdate(),
								  'N', NULL, @codFasc, @docNumber,
								  @tipoOggetto, @tipoAtto
								)

								--p_result := id_cons_2;
								SET @result = SCOPE_IDENTITY()
							END
					END
				ELSE
					-- VINCOLO NUMERO DOCUMENTI NON VIOLATO e DIMENSIONE VIOLATO
					BEGIN
						-- INSERIMENTO IN UNA NUOVA ISTANZA DI CONSERVAZIONE
                            
						-- Controllo se istanza è predefinita
                        IF(@isIstanzaPrefered = '1')
							BEGIN 
								--Aggiorno la vecchia a non preferred
								UPDATE DPA_AREA_CONSERVAZIONE
								SET DPA_AREA_CONSERVAZIONE.IS_PREFERRED = NULL
								WHERE DPA_AREA_CONSERVAZIONE.SYSTEM_ID = @res
									
								-- è un'istanza predefinita
								INSERT INTO DPA_AREA_CONSERVAZIONE
								 (ID_AMM, ID_PEOPLE, ID_RUOLO_IN_UO,
								  CHA_STATO, DATA_APERTURA, USER_ID, ID_GRUPPO, ID_POLICY,
								  VAR_FILE_CHIUSURA, VAR_FILE_CHIUSURA_FIRMATO, IS_PREFERRED
								 )
								VALUES (@idAmm, @idPeople, @idRuoloInUo,
								  'N', getdate(), @userId, @idGruppo, @idpolicy,
								  NULL,NULL,'1'
								)
								
								SET @id_cons_1=SCOPE_IDENTITY()
								
								INSERT INTO DPA_ITEMS_CONSERVAZIONE
								 (ID_CONSERVAZIONE, ID_PROFILE, ID_PROJECT,
								  CHA_TIPO_DOC, VAR_OGGETTO, ID_REGISTRO, DATA_INS,
								  CHA_STATO, VAR_XML_METADATI, COD_FASC, DOCNUMBER,
								  CHA_TIPO_OGGETTO, VAR_TIPO_ATTO
								 )
								VALUES (@id_cons_1, @idProfile, @idProject,
								  @tipoDoc, @oggetto, @idRegistro, getdate(),
								  'N', NULL, @codFasc, @docNumber,
								  @tipoOggetto, @tipoAtto
								)

								--p_result := id_cons_2;
								SET @result = SCOPE_IDENTITY()
							END
						ELSE
							BEGIN
								-- non è predefinita
								INSERT INTO DPA_AREA_CONSERVAZIONE
								(SYSTEM_ID, ID_AMM, ID_PEOPLE, ID_RUOLO_IN_UO,
								  CHA_STATO, DATA_APERTURA, USER_ID, ID_GRUPPO, ID_POLICY,
								  VAR_FILE_CHIUSURA, VAR_FILE_CHIUSURA_FIRMATO
								)
								VALUES (@idAmm, @idPeople, @idRuoloInUo,
								  'N', getdate(), @userId, @idGruppo, @idpolicy,
								  NULL,NULL
								)
								
								SET @id_cons_1=SCOPE_IDENTITY()
								
								INSERT INTO DPA_ITEMS_CONSERVAZIONE
								 (ID_CONSERVAZIONE, ID_PROFILE, ID_PROJECT,
								  CHA_TIPO_DOC, VAR_OGGETTO, ID_REGISTRO, DATA_INS,
								  CHA_STATO, VAR_XML_METADATI, COD_FASC, DOCNUMBER,
								  CHA_TIPO_OGGETTO, VAR_TIPO_ATTO
								 )
								VALUES (@id_cons_1, @idProfile, @idProject,
								  @tipoDoc, @oggetto, @idRegistro, getdate(),
								  'N', NULL, @codFasc, @docNumber,
								  @tipoOggetto, @tipoAtto
								)

								--p_result := id_cons_2;
								SET @result = SCOPE_IDENTITY()
							END
					END
			END
		ELSE
			-- VINCOLO DIMENSIONE ISTANZE NON VIOLATO
			BEGIN
				-- VINCOLO NUMERO DOCUMENTI VIOLATO
				IF (numDocIstanzaViolato)
					BEGIN
						-- INSERIMENTO IN UNA NUOVA ISTANZA DI CONSERVAZIONE
						
						-- Controllo se istanza è predefinita
						IF(@isIstanzaPrefered = '1')
							BEGIN 
								--Aggiorno la vecchia a non preferred
								UPDATE DPA_AREA_CONSERVAZIONE
								SET DPA_AREA_CONSERVAZIONE.IS_PREFERRED = NULL
								WHERE DPA_AREA_CONSERVAZIONE.SYSTEM_ID = @res
								
								-- è un'istanza predefinita
								INSERT INTO DPA_AREA_CONSERVAZIONE
								 (ID_AMM, ID_PEOPLE, ID_RUOLO_IN_UO,
								  CHA_STATO, DATA_APERTURA, USER_ID, ID_GRUPPO, ID_POLICY,
								  VAR_FILE_CHIUSURA, VAR_FILE_CHIUSURA_FIRMATO, IS_PREFERRED
								 )
								VALUES (@idAmm, @idPeople, @idRuoloInUo,
								  'N', getdate(), @userId, @idGruppo, @idpolicy,
								  NULL,NULL,'1'
								)
								
								SET @id_cons_1=SCOPE_IDENTITY()
								
								INSERT INTO DPA_ITEMS_CONSERVAZIONE
								 (ID_CONSERVAZIONE, ID_PROFILE, ID_PROJECT,
								  CHA_TIPO_DOC, VAR_OGGETTO, ID_REGISTRO, DATA_INS,
								  CHA_STATO, VAR_XML_METADATI, COD_FASC, DOCNUMBER,
								  CHA_TIPO_OGGETTO, VAR_TIPO_ATTO
								 )
								VALUES (@id_cons_1, @idProfile, @idProject,
								  @tipoDoc, @oggetto, @idRegistro, getdate(),
								  'N', NULL, @codFasc, @docNumber,
								  @tipoOggetto, @tipoAtto
								)

								--p_result := id_cons_2;
								SET @result = SCOPE_IDENTITY()
							END
						ELSE
							BEGIN
								INSERT INTO DPA_AREA_CONSERVAZIONE
								 (ID_AMM, ID_PEOPLE, ID_RUOLO_IN_UO,
								  CHA_STATO, DATA_APERTURA, USER_ID, ID_GRUPPO, ID_POLICY,
								  VAR_FILE_CHIUSURA, VAR_FILE_CHIUSURA_FIRMATO
								 )
								VALUES (@idAmm, @idPeople, @idRuoloInUo,
								  'N', getdate(), @userId, @idGruppo, @idpolicy,
								  NULL,NULL
								)
								
								SET @id_cons_1=SCOPE_IDENTITY()
								
								INSERT INTO DPA_ITEMS_CONSERVAZIONE
								 (ID_CONSERVAZIONE, ID_PROFILE, ID_PROJECT,
								  CHA_TIPO_DOC, VAR_OGGETTO, ID_REGISTRO, DATA_INS,
								  CHA_STATO, VAR_XML_METADATI, COD_FASC, DOCNUMBER,
								  CHA_TIPO_OGGETTO, VAR_TIPO_ATTO
								 )
								VALUES (@id_cons_1, @idProfile, @idProject,
								  @tipoDoc, @oggetto, @idRegistro, getdate(),
								  'N', NULL, @codFasc, @docNumber,
								  @tipoOggetto, @tipoAtto
								)

								--p_result := id_cons_2;
								SET @result = SCOPE_IDENTITY()
							END
					END
				ELSE
					-- VINCOLO NUMERO DOCUMENTI NON VIOLATO E DIMENSIONE ISTANZE NON VIOLATO
					BEGIN
						-- INSERIMENTO NELLA VECCHIA ISTANZA DI CONSERVAZIONE
						INSERT INTO DPA_ITEMS_CONSERVAZIONE
						 (ID_CONSERVAZIONE, ID_PROFILE, ID_PROJECT,
						 CHA_TIPO_DOC, VAR_OGGETTO, ID_REGISTRO, DATA_INS,
						 CHA_STATO, VAR_XML_METADATI, COD_FASC, DOCNUMBER,
						 CHA_TIPO_OGGETTO, VAR_TIPO_ATTO 
						 )
						VALUES (@res, @idProfile, @idProject,
						  @tipoDoc, @oggetto, @idRegistro, getdate(),
						  'N', NULL, @codFasc, @docNumber,
						  @tipoOggetto, @tipoAtto
						)

						--p_result := id_cons_2;
						SET @result = SCOPE_IDENTITY()
					END
			END
	END
ELSE
	-- CORPO ELSE - ID ISTANZA NON RECUPERABILE - (non c'è una istanza manuale predefinita o corrente)
	BEGIN
		IF(NOT numDocIstanzaViolato AND NOT dimIstanzaViolato)
			BEGIN
				INSERT INTO DPA_AREA_CONSERVAZIONE
                    (ID_AMM, ID_PEOPLE, ID_RUOLO_IN_UO,
                    CHA_STATO, DATA_APERTURA, USER_ID, ID_GRUPPO, ID_POLICY,
                    VAR_FILE_CHIUSURA, VAR_FILE_CHIUSURA_FIRMATO
                    )
                    VALUES (@idAmm, @idPeople, @idRuoloInUo,
                    'N', getdate(), @userId, @idGruppo, @idpolicy,
                    NULL,NULL
                    )
							 
				SET @id_cons_1=SCOPE_IDENTITY()

                INSERT INTO DPA_ITEMS_CONSERVAZIONE
                    (ID_CONSERVAZIONE, ID_PROFILE, ID_PROJECT,
                    CHA_TIPO_DOC, VAR_OGGETTO, ID_REGISTRO, DATA_INS,
                    CHA_STATO, VAR_XML_METADATI, COD_FASC, DOCNUMBER,
                    CHA_TIPO_OGGETTO, VAR_TIPO_ATTO
                    )
                    VALUES (@id_cons_1, @idProfile, @idProject,
					@tipoDoc, @oggetto, @idRegistro, getdate(),
					'N', NULL, @codFasc, @docNumber,
					@tipoOggetto, @tipoAtto
                    )

                 --p_result := id_cons_2;
                SET @result = SCOPE_IDENTITY()
			END
	END
END


