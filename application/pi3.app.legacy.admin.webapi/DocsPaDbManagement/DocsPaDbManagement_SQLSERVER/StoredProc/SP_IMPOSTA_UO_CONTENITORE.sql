if exists (select * from dbo.sysobjects where id = object_id(N'[docsadm].[SP_IMPOSTA_UO_CONTENITORE]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [docsadm].[SP_IMPOSTA_UO_CONTENITORE]
GO

CREATE PROCEDURE docsadm.SP_IMPOSTA_UO_CONTENITORE  @id_amm int as

DECLARE @num_livello int
DECLARE @system_id varchar(32)
DECLARE @var_desc_amm varchar(32)
DECLARE @var_codice_amm varchar(32)
DECLARE @identity int


	BEGIN 
		
		 SELECT *
		 FROM DPA_CORR_GLOBALI  
		 WHERE cha_tipo_urp = 'U' AND CHA_TIPO_IE = 'I' 
		 AND ID_AMM = @id_amm and  NUM_LIVELLO = 0 and id_parent = 0
 		
		IF @@ROWCOUNT > 1

			BEGIN -- inizio 1° begin
						
				SELECT  @var_desc_amm=VAR_DESC_AMM,  @var_codice_amm=VAR_CODICE_AMM
				FROM DPA_AMMINISTRA WHERE SYSTEM_ID = @id_amm
		
				SELECT * FROM  DPA_CORR_GLOBALI WHERE VAR_CODICE = @var_codice_amm
					 OR VAR_COD_RUBRICA = @var_codice_amm  AND ID_AMM = @id_amm
				
				IF @@ROWCOUNT = 0 
			
					BEGIN -- inizio 2° begin
						
						UPDATE DPA_CORR_GLOBALI 
						SET NUM_LIVELLO = NUM_LIVELLO+1 
						WHERE cha_tipo_urp = 'U' AND CHA_TIPO_IE = 'I' AND ID_AMM = @id_amm
						
						 --INSERISCO IL CORRISPONDENTE
						INSERT INTO DPA_CORR_GLOBALI
						(
						ID_AMM,
						VAR_COD_RUBRICA,
						VAR_DESC_CORR,
						ID_PARENT, 
						NUM_LIVELLO, 
						VAR_CODICE, 
						CHA_TIPO_IE, 
						CHA_TIPO_CORR, 
						CHA_TIPO_URP
						) 
						VALUES (	
						@id_amm,
						@var_codice_amm,
						@var_desc_amm,
						0,
						0,
						@var_codice_amm,
						'I',
						'S',
						'U'
						)	
						
						SET @identity = SCOPE_IDENTITY()
					
						UPDATE DPA_CORR_GLOBALI 
						SET ID_PARENT = @identity
						WHERE cha_tipo_urp = 'U' AND CHA_TIPO_IE = 'I' AND ID_AMM = @id_amm
						AND NUM_LIVELLO = 1
								
					END	-- fine 2° begin	
				ELSE
					
					print 'ATTENZIONE:  VAR_CODICE O CODICE RUBRICA PRESENTI - LA PROCEDURA TERMINA QUI'
											
			END 

		END

GO
