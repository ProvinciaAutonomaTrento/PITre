-- Restituisce 1 se sia il documento principale che eventuali allegati utente sono cartacei
-- Restituisce 0 se almeno uno tra documento principale o allegati utente non è cartaceo, o se il documento proviene da spedizione
CREATE FUNCTION [DOCSADM].[IsDocCartaceo]
(
	@docnum INTEGER
)
RETURNS VARCHAR(16)
AS
BEGIN
	DECLARE @isCartaceo			VARCHAR(16)
	DECLARE @vmaxidgenerica 	INTEGER
	
	SELECT @vmaxidgenerica = MAX (v1.version_id)
	FROM VERSIONS v1, components c
	WHERE v1.docnumber = @docnum AND v1.version_id = c.version_id

	SELECT @isCartaceo = ISNULL(cartaceo,'0')
	FROM VERSIONS
	WHERE docnumber = @docnum AND version_id = @vmaxidgenerica
	
	IF(@isCartaceo = '1')
		BEGIN
			DECLARE @item VARCHAR(255)
			DECLARE @maxVersion INTEGER
			DECLARE @curAllegato CURSOR
			DECLARE @typeAtt VARCHAR(16)
			SET @curAllegato = CURSOR FOR SELECT system_id FROM profile WHERE id_documento_principale = @docnum
			OPEN @curAllegato
			
			WHILE 1 = 1
			BEGIN
				FETCH @curAllegato INTO @item
				IF (@@FETCH_STATUS <> 0 OR @isCartaceo = '0')
				BREAK
				
				SELECT @maxVersion = MAX (v1.version_id)
               FROM VERSIONS v1, components c
               WHERE v1.docnumber = @item AND v1.version_id = c.version_id
			   
			   SELECT @typeAtt = CHA_ALLEGATI_ESTERNO
               FROM VERSIONS WHERE version_id = @maxVersion
			   
			   IF(@typeAtt = '0')
					SELECT @isCartaceo = ISNULL(cartaceo,'0')
					FROM VERSIONS
					WHERE docnumber = @item AND version_id = @maxVersion
				 
			END
			CLOSE @curAllegato
		END
		
	IF(@isCartaceo = '1')
		BEGIN
			DECLARE @checkMailInt VARCHAR(16)
			
			SELECT @checkMailInt = COUNT(A.ID_PROFILE)
			FROM DPA_DOC_ARRIVO_PAR A, DOCUMENTTYPES B, PROFILE C
			WHERE A.ID_DOCUMENTTYPES = B.SYSTEM_ID AND A.ID_PROFILE = C.SYSTEM_ID AND A.ID_PROFILE = @docnum
			AND (B.TYPE_ID = 'MAIL' OR B.TYPE_ID = 'INTEROPERABILITA' OR B.TYPE_ID = 'SIMPLIFIEDINTEROPERABILITY' OR B.TYPE_ID = 'SERVIZI ONLINE') 
			AND C.CHA_TIPO_PROTO = 'A'
			
			IF(@checkMailInt = '0')
				SET @isCartaceo = '1'
			ELSE
  				SET @isCartaceo = '0'
			
		END


	RETURN @isCartaceo

END
 