CREATE FUNCTION getDiagrammiProcessiFirma ( @processoid   INT ) RETURNS VARCHAR(8000) AS
BEGIN
	DECLARE @risultato   VARCHAR(8000)
    DECLARE @item        VARCHAR(200)
    
	SET @risultato = ''
	
	DECLARE cur CURSOR
        FOR SELECT DISTINCT( d.var_descrizione )
        FROM dpa_diagrammi_stato d JOIN dpa_stati s ON d.system_id = s.id_diagramma
        WHERE s.id_processo_firma = @processoid
		OPEN cur
		
		fetch next from cur into @item
			while(@@fetch_status=0)
			BEGIN
			IF (@risultato IS NOT NULL)
				SET @risultato = @risultato + '; ' + @item
			ELSE
				SET @risultato = @risultato + @item
		END
		RETURN @risultato
		
END

