CREATE OR REPLACE PROCEDURE SP_CLEAN_DPA_T_CANALE_CORR

IS

sysCorrente NUMBER;
IdentityDpaTcanaleCorr NUMBER;

CURSOR sysCursor IS
		select system_id from DPA_CORR_GLOBALI 
		WHERE cha_tipo_ie = 'E' AND CHA_TIPO_URP IN ('U', 'P', 'R')
		and system_id 
		not in (select id_corr_globale from dpa_t_canale_corr);
BEGIN
	DELETE FROM dpa_t_canale_corr
      	WHERE id_corr_globale IN (
                   SELECT system_id
                     FROM dpa_corr_globali
                    WHERE cha_tipo_ie = 'I');

	 OPEN sysCursor;
	 LOOP
		 	 FETCH sysCursor INTO sysCorrente;
			 EXIT WHEN sysCursor%NOTFOUND;
			 
			 	  BEGIN
				  	   SELECT seq.NEXTVAL INTO IdentityDpaTcanaleCorr FROM dual;
					   
				  	   INSERT INTO DPA_T_CANALE_CORR
					   (
					   		  SYSTEM_ID,
							  ID_CORR_GLOBALE
					   )
					   VALUES
					   (
					   		 IdentityDpaTcanaleCorr,
							 sysCorrente
					   );
				  END;
			 
     END LOOP;
	 CLOSE sysCursor;
	
	COMMIT;	 
END
;