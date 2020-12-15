
ALTER FUNCTION [DOCSADM].[GETSEGNATURAREPERTORIO]
(
	@docId INT, 
	@idAmm INT
)
RETURNS VARCHAR(2000)
AS
BEGIN
	
	DECLARE @formato_cont VARCHAR(255)
	DECLARE @valore_ogg_db VARCHAR(100)
	DECLARE @anno_rep INT
	DECLARE @cod_db VARCHAR(50)
	DECLARE @dta_inserimento DATETIME
	DECLARE @idaoorf INT
	DECLARE @cod_reg VARCHAR(16)
	DECLARE @cod_amm VARCHAR(16)
	DECLARE @segnatura VARCHAR(2000)
	DECLARE @dta_annullato DATETIME
	DECLARE cur CURSOR FOR
		SELECT 
			oc.formato_contatore, 
			t.valore_oggetto_db, 
			t.anno, 
			t.codice_db,
			t.dta_ins,
			t.id_aoo_rf,
			t.dta_annullamento
		FROM dpa_associazione_templates t
		JOIN dpa_oggetti_custom oc
		ON t.doc_number  = @docId
		AND t.id_oggetto =oc.system_id
		AND oc.repertorio='1';
		
	--BEGIN
	SET @segnatura = @formato_cont;
	
	--OPEN cur;
	--LOOP
	--FETCH cur INTO formato_cont, valore_ogg_db, anno_rep, cod_db, dta_inserimento, idaoorf;
	OPEN cur
	FETCH NEXT FROM cur
	INTO @formato_cont, @valore_ogg_db, @anno_rep, @cod_db, @dta_inserimento, @idaoorf, @dta_annullato
	
	WHILE @@FETCH_STATUS = 0
	BEGIN 
	
		
		IF (@valore_ogg_db is not null)
		BEGIN		 
			SET @formato_cont = replace(upper(@formato_cont), 'ANNO', @anno_rep + '');
			SET @formato_cont = replace(upper(@formato_cont), 'CONTATORE', @valore_ogg_db + '');
			SET @formato_cont = replace(upper(@formato_cont), 'COD_UO', @cod_db + '');
			SET @formato_cont = replace(upper(@formato_cont),'GG/MM/AAAA HH:MM', CONVERT(varchar, GETDATE(), 103) + ' ' + CONVERT(varchar(5), GETDATE(), 108) + '');
			SET @formato_cont = replace(upper(@formato_cont),'GG/MM/AAAA', CONVERT(varchar, @dta_inserimento, 103) + '');

			IF (@idaoorf is not null and @idaoorf != 0)
			BEGIN
				select @cod_reg = var_codice from dpa_el_registri where system_id = @idaoorf
			END

			IF (@cod_reg is not null)
			BEGIN
				SET @formato_cont = replace(upper(@formato_cont), 'RF', @cod_reg + '');
				SET @formato_cont = replace(upper(@formato_cont), 'AOO', @cod_reg + '');
			END


			IF (@idAmm is not null)
			BEGIN
				select @cod_amm = var_codice_amm from dpa_amministra where system_id = @idAmm
				SET @formato_cont = replace(upper(@formato_cont), 'COD_AMM', @cod_amm + '');
			END
			
			IF (@dta_annullato IS NOT NULL)
			BEGIN
			  SET @formato_cont = @formato_cont + '#1';
			END
			ELSE
			BEGIN
			  SET @formato_cont = @formato_cont + '#0';
			END
					
			SET @segnatura = @formato_cont
		END
		
		ELSE
		
		BEGIN
			SET @segnatura = ''
			RETURN @segnatura
		END
		
		FETCH NEXT FROM cur
		INTO @formato_cont, @valore_ogg_db, @anno_rep, @cod_db, @dta_inserimento, @idaoorf, @dta_annullato
						
	END
	CLOSE cur
	DEALLOCATE cur
	
	
	RETURN @segnatura
		
END

