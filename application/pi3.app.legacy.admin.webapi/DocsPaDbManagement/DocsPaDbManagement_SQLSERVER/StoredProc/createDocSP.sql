if exists (select * from dbo.sysobjects where id = object_id(N'[docsadm].[createDocSP]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [docsadm].[createDocSP]
GO

CREATE PROCEDURE [docsadm].[createDocSP]
@idpeople int,
@doctype varchar (128), 
@systemId int out

 AS

DECLARE @Docnum  int
DECLARE @Verid int
DECLARE @IDDocType int

BEGIN

SET @systemId= 0

set @IDDocType =  ( SELECT SYSTEM_ID  FROM
	 DOCUMENTTYPES
	 WHERE TYPE_ID = @doctype)

IF (NOT @IDDocType IS NULL)
	BEGIN
		INSERT INTO Profile 
			(
				
				TYPIST,  
				AUTHOR,
				DOCUMENTTYPE, 
				CREATION_DATE, 
				CREATION_TIME
				
			) 
			VALUES 
			(
				
				@idpeople, 
				@idpeople, 
				@IDDocType,
				 GETDATE(),
				 GETDATE()
				
			)	
				
			-- Reperimento identity appena immessa
			SET @systemId=scope_identity()
			SET @Docnum = @systemId

			--AGGIORNO LA PROFILE CON DOCNUMBER=SYSTEMID APPENA INSERITA
			UPDATE PROFILE 
			SET DOCNUMBER = @Docnum 
			WHERE SYSTEM_ID = @systemId
			
		
			IF (@@ROWCOUNT = 0)
				BEGIN
					SET @systemId= 0
					RETURN @systemId 
				END
			ELSE
			 
				BEGIN

				INSERT INTO VERSIONS 
				(
				  DOCNUMBER,
			               VERSION, 
			               SUBVERSION, 
			               VERSION_LABEL, 
			              AUTHOR, TYPIST, 
			              DTA_CREAZIONE
				) VALUES (
				 @Docnum, 1, '!', '1',@idpeople, @idpeople,  GETDATE()
				)
				
				SET @Verid = scope_identity()


				INSERT INTO COMPONENTS 
					(
					VERSION_ID, 
					DOCNUMBER, 
					FILE_SIZE
					) VALUES (
						@Verid, @Docnum, 0
					)
				

				INSERT INTO SECURITY 
					(
					THING,
					PERSONORGROUP,
					ACCESSRIGHTS,
					ID_GRUPPO_TRASM,
					CHA_TIPO_DIRITTO
					) VALUES (
						@systemId, @idpeople, 0, NULL, NULL			
					)		
					
				END	
		
	END
ELSE
	
	SET @systemId = 0
END
GO
