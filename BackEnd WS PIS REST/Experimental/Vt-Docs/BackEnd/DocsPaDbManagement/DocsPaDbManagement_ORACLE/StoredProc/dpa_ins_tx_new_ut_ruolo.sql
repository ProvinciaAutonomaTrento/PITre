CREATE OR REPLACE PROCEDURE dpa_ins_tx_new_ut_ruolo (
   p_idpeople      IN      integer,
   p_idcorrglob     IN    integer,
   p_returnvalue     OUT   integer
)
IS
/******************************************************************************
   NAME:       DPA_INS_TX_NEW_UT_RUOLO
   PURPOSE:

   REVISIONS:
   Ver        Date        Author           Description
   ---------  ----------  ---------------  ------------------------------------
   1.0        20/02/2008          1. Created this procedure.

   NOTES:
   
******************************************************************************/
BEGIN
   DECLARE
      CURSOR trasm (idpeople integer, idcorrglob integer)
      IS
         SELECT DISTINCT b.system_id AS ID,A.system_id as idTrasm,
		 a.id_people as idpeopletx,a.id_ruolo_in_uo as ruolo_in_uo,a.dta_invio as dtainvio 
		 ,a.var_note_generali as note_gen,b.id_ragione as idragione,a.id_project as idproj
		 ,a.id_profile as idprof,b.var_note_sing as note_sing 
		 ,b.dta_scadenza as scadenza
		 ,b.cha_tipo_dest as cha_tipodest,
		 b.id_corr_globale as idtscorrglob
		                     FROM dpa_trasmissione a,
                         dpa_trasm_singola b,
                         dpa_trasm_utente c,
                         dpa_ragione_trasm d
                   WHERE a.system_id = b.id_trasmissione
                     AND b.system_id = c.id_trasm_singola
                     AND c.cha_valida = '1'
                     AND a.dta_invio IS NOT NULL
                     AND b.id_corr_globale = idcorrglob
                     AND (a.cha_tipo_oggetto = 'D' OR a.cha_tipo_oggetto = 'F'
                         )
                     AND b.id_ragione = d.system_id
                     AND (   (    d.cha_tipo_ragione = 'W'
                              AND c.cha_accettata = '0'
                              AND c.cha_rifiutata = '0'
                              AND c.cha_valida = '1'
                             )
                          OR (    (   d.cha_tipo_ragione = 'N'
                                   OR d.cha_tipo_ragione = 'I'
                                  )
                              AND c.cha_vista = '0'
                             )
                         )
                     AND c.id_people NOT IN (idpeople);

      id_trasmutente   NUMBER;
      tmpvar           NUMBER;
      sysidtxut        NUMBER;
    --  sysidtx          NUMBER;
      dtainvio         DATE;
      idpeoplemitt     NUMBER;
      idruolomitt      NUMBER;
      idprofile        NUMBER;
      idproject        NUMBER;
      notegen          VARCHAR2 (250);
	  idreg number;
   BEGIN
      p_returnvalue := 0;

      FOR currenttrasm IN trasm (p_idpeople, p_idcorrglob)
      LOOP
         BEGIN
            --D_DPA_TRASM_UTENTE_ID_TRASM_SINGOLA
            SELECT system_id
              INTO id_trasmutente
              FROM dpa_trasm_utente
             WHERE id_trasm_singola = currenttrasm.ID
               AND id_people = p_idpeople;
			    EXCEPTION
            WHEN OTHERS
            THEN --se non c'è alcuna.
               null;
end;
begin

            DELETE FROM dpa_trasm_utente
                  WHERE id_trasm_singola = currenttrasm.ID
                    AND id_people = p_idpeople;
         EXCEPTION
            WHEN OTHERS
            THEN
               p_returnvalue := -1;
         END;

         BEGIN
		  --D_dpa_todolist
		  if(id_trasmutente=null)
		  then
		  begin
		     DELETE FROM dpa_todolist
                  WHERE id_trasm_utente = id_trasmutente;
				  
         EXCEPTION
            WHEN OTHERS
            THEN
               p_returnvalue := -2;
         END;
		 end if;
end;
         BEGIN
            SELECT seq.NEXTVAL
              INTO sysidtxut
              FROM DUAL;
-- I dpa_trasm_utente 
            INSERT INTO dpa_trasm_utente
                        (system_id, id_people, id_trasm_singola, cha_vista,
                         cha_accettata, cha_rifiutata, cha_valida,cha_in_todolist
                        )
                 VALUES (sysidtxut, p_idpeople, currenttrasm.ID, '0',
                         '0', '0', '1','1'
                        );
         EXCEPTION
            WHEN OTHERS
            THEN
               p_returnvalue := -3;
         END;
		 --commit;
		 
		  
         BEGIN
	-- I dpa_todolist
	idreg:=null;
	if(currenttrasm.idprof is not null)
	then 
	 idreg:= TO_NUMBER (vardescribe (currenttrasm.idprof, 'PROF_IDREG'));
	end if; 
            INSERT INTO dpa_todolist
                        (id_trasmissione, id_trasm_singola, id_trasm_utente,
                         dta_invio, id_people_mitt, id_ruolo_mitt,
                         id_people_dest, id_ragione_trasm, var_note_gen,
                         var_note_sing, dta_scadenza, id_profile, id_project,
                         id_ruolo_dest, id_registro, cha_tipo_trasm)
			    values
				( currenttrasm.idTrasm, currenttrasm.ID,sysidtxut,
					   currenttrasm.dtainvio,currenttrasm.idpeopletx, currenttrasm.ruolo_in_uo,
					    p_idpeople, currenttrasm.idragione, currenttrasm.note_gen,
                      currenttrasm.note_sing, currenttrasm.scadenza, currenttrasm.idprof,
                      currenttrasm.idproj,
                      TO_NUMBER (vardescribe (currenttrasm.idtscorrglob,
                                              'ID_GRUPPO')
                                )
                     ,idreg,
                      currenttrasm.cha_tipodest		);
					  	 
             
         EXCEPTION
            WHEN OTHERS
            THEN
               p_returnvalue := -4;
         END;
      END LOOP;
	  p_returnvalue :=1;
   END;
END dpa_ins_tx_new_ut_ruolo;
/
