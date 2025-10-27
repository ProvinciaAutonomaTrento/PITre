CREATE PROCEDURE SP_CLEAN_DPA_T_CANALE_CORR

AS

DECLARE @sysCorrente INT

DECLARE sysCursor CURSOR FOR	
	select system_id from DPA_CORR_GLOBALI 
	WHERE cha_tipo_ie = 'E' AND CHA_TIPO_URP IN ('U', 'P', 'R')
	and system_id 
	not in (select id_corr_globale from dpa_t_canale_corr)
BEGIN
	DELETE FROM dpa_t_canale_corr
      	WHERE id_corr_globale IN (
                   SELECT system_id
                     FROM dpa_corr_globali
                    WHERE cha_tipo_ie = 'I')
	OPEN  sysCursor
		 	 FETCH NEXT FROM sysCursor INTO @sysCorrente
			 WHILE @@FETCH_STATUS = 0
			 
			 	  BEGIN
					   
				  	   INSERT INTO DPA_T_CANALE_CORR
					   (	 
							  ID_CORR_GLOBALE
					   )
					   VALUES
					   (
							 @sysCorrente
					   )
 				
				   	   FETCH NEXT FROM sysCursor INTO @sysCorrente
				END
			 
    
		CLOSE sysCursor
		DEALLOCATE sysCursor
	
	
END
GO
