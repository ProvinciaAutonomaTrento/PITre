SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [sp_CheckConsConvAndUpdate]	(
		@p_id_istanza		int,
		@p_doc_number       int,
		@p_newVersionId     int,
		@returnvalue		int  OUTPUT  
		)
AS
BEGIN

	DECLARE @estenzioneNuovaVersione    VARCHAR(5);
    DECLARE @numDocReportDaConv         INT;
    DECLARE @numDocReportConvertiti     INT;
    DECLARE @numDocReportInErrore       INT;
    	
	DECLARE @logObject VARCHAR(50) = OBJECT_NAME(@@PROCID)
	DECLARE @errorCode INT
		
    BEGIN 
		print('inizio stored')
		
		SET @returnvalue = 1
		
		select @estenzioneNuovaVersione = c.EXT 
        from components c
        where c.docnumber = @p_doc_number 
        AND c.version_id = @p_newVersionId
        		
		IF (@estenzioneNuovaVersione = 'pdf')
			BEGIN
			
					UPDATE [dpa_verifica_formati_cons]
					SET    [CONVERTITO] = '1'
					WHERE  [ID_ISTANZA] = @p_id_istanza
					AND    [DOCNUMBER] = @p_doc_number
				
					UPDATE [dpa_items_conservazione]
					SET    [VAR_TIPO_FILE] = '.pdf'
					WHERE  [ID_CONSERVAZIONE] = @p_id_istanza
					AND    [ID_PROFILE] = @p_doc_number

				print 'Aggiornamento reportConservazione esito OK! Estensione documento: '+ CAST(@estenzioneNuovaVersione as VARCHAR(5))
			END	
		ELSE
			BEGIN
			
				UPDATE [dpa_verifica_formati_cons]
				SET    [CONVERTITO] = '1',
					   [ERRORE] = '1',
					   [TIPOERRORE] = '2'
				WHERE  [ID_ISTANZA] = @p_id_istanza
				AND    [DOCNUMBER] = @p_doc_number
				
				print 'Aggiornamento reportConservazione esito KO! Estensione documento: ' + CAST(@estenzioneNuovaVersione as VARCHAR(5))
			END
			
		set @errorCode = @@ERROR

		IF @errorCode <> 0
		BEGIN
			
			UPDATE [dpa_verifica_formati_cons]
			SET    [CONVERTITO] = '1',
				   [ERRORE] = '1',
				   [TIPOERRORE] = '2'
			WHERE  [ID_ISTANZA] = @p_id_istanza
			AND    [DOCNUMBER] = @p_doc_number
            
            print 'ERRORE Controllo conversione documento in PDF'

			
			RETURN
		END
		
		BEGIN
      --result := 0;

      SELECT    @numDocReportDaConv = count(*)
        FROM    [dpa_verifica_formati_cons] rc
        WHERE   rc.[id_istanza] = @p_id_istanza
        AND     rc.[daConvertire] = 1;

      SELECT    @numDocReportConvertiti = count(*) 
        FROM    [dpa_verifica_formati_cons] rc
        WHERE   rc.[id_istanza] = @p_id_istanza
        AND     rc.[daConvertire] = '1'
        AND     rc.[convertito] = '1'
        AND     (rc.[errore] = 0 OR rc.[errore] is null);
              
      SELECT    @numDocReportInErrore = count(*) 
        FROM    [dpa_verifica_formati_cons] rc
        WHERE   rc.[id_istanza] = @p_id_istanza
        AND     rc.[daConvertire] = '1'
        AND     rc.[convertito] = '1'
        AND     rc.[errore] = '1'
        AND     rc.[tipoerrore] = '2';
      
      print 'Controllo documenti istanza '+ CAST(@p_id_istanza as VARCHAR(10)) +' ' +CAST(@numDocReportDaConv as VARCHAR(10)) +' ' +CAST(@numDocReportConvertiti as VARCHAR(10));
      
	  IF (@numDocReportDaConv > 0 AND @numDocReportDaConv = @numDocReportConvertiti)
		BEGIN
	  
          IF @numDocReportInErrore > 0 
			BEGIN
				UPDATE [dpa_area_conservazione]
				SET [cha_stato] = 'Z' --ERRORE CONVERSIONE
				WHERE [system_id] = @p_id_istanza;
				
				print 'stato in errore';
            END
			
          ELSE
		  
			BEGIN
				UPDATE [dpa_area_conservazione]
				SET [cha_stato] = 'I',
					[data_invio] = GETDATE()
				WHERE [system_id] = @p_id_istanza;
				
				print 'stato Inviata';
            END
		END
	  		
		set @errorCode = @@ERROR

		IF @errorCode <> 0
		BEGIN
			
			UPDATE [dpa_area_conservazione]
			SET [cha_stato] = 'Z' --ERRORE CONVERSIONE
			WHERE [system_id] = @p_id_istanza;
            
            print 'eccezione: stato in errore';
			
			RETURN
		END
		
		print 'fine stored';
		
END
