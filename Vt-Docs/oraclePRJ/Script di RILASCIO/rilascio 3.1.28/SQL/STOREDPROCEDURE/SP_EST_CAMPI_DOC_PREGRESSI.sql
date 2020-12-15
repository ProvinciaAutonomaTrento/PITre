ALTER PROCEDURE [DOCSADM].[SP_EST_CAMPI_DOC_PREGRESSI]
(
	@id_oggetto_in    	int,
    @id_template_in 	int,
    @anno_in           	int,
    @id_aoo_rf_in       int,
    @returnvalue 		int OUT
)
AS
BEGIN
	DECLARE @id_doc int
	DECLARE DOC_PREGRESSI CURSOR LOCAL FOR
		SELECT DISTINCT(DOC_NUMBER)
		FROM docsadm.DPA_ASSOCIAZIONE_TEMPLATES
		WHERE ID_TEMPLATE = @id_template_in
		AND DOC_NUMBER IS NOT NULL
        UNION
        SELECT DOCNUMBER FROM PROFILE a WHERE a.ID_TIPO_ATTO = @id_template_in

	BEGIN TRY  
		open DOC_PREGRESSI
		fetch next from DOC_PREGRESSI into @id_doc

		while @@fetch_status=0
		begin

    		INSERT INTO docsadm.DPA_ASSOCIAZIONE_TEMPLATES
      		(
      			ID_OGGETTO,
      			ID_TEMPLATE,
      			Doc_Number,
      			Valore_Oggetto_Db,
      			Anno,
      			ID_AOO_RF,
      			CODICE_DB,
      			MANUAL_INSERT,
      			VALORE_SC,
      			DTA_INS,
      			ANNO_ACC
      		)
			VALUES
      		(
      			@id_oggetto_in,
      			@id_template_in,
      			@id_doc,
      			'',
      			@anno_in,
      			@id_aoo_rf_in,
      			'',
      			0,
      			NULL,
      			getdate(),
      			''
      		)
            
			fetch next from DOC_PREGRESSI into @id_doc
		
		end	
		SET @returnvalue = 0
	END TRY  
	BEGIN CATCH  
		-- Execute error retrieval routine.  
		SET @returnvalue = -1
	END CATCH; 

    CLOSE DOC_PREGRESSI
	deallocate DOC_PREGRESSI
	

END