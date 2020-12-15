/*MICRO FUNZIONE PER IL CESTINO DELLE NOTIFICHE*/

BEGIN
UTL_INSERT_CHIAVE_MICROFUNZ('DO_DELETE_NOTIFY'
,'Abilita il pulsante di rimozione notifiche'
,NULL,'N',NULL,'3.1.14',NULL);
End;


INSERT INTO dpa_funzioni (SYSTEM_ID,
                      ID_AMM,
                      COD_FUNZIONE,
                      VAR_DESC_FUNZIONE,
                      ID_PARENT,
                      CHA_TIPO_FUNZ,
                      ID_PESO,
                      CHA_FLAG_PARENT,
                      ID_TIPO_FUNZIONE)
                 SELECT SEQ.NEXTVAL,
                         a.system_id,
                         'DO_DELETE_NOTIFY',
                         'Abilita il pulsante di rimozione notifiche',
                         NULL,
                         NULL,
                         NULL,
                         NULL,
                         F.SYSTEM_ID
                         FROM DPA_AMMINISTRA A JOIN DPA_TIPO_FUNZIONE F ON A.SYSTEM_ID  = F.ID_AMM
                         where F.VAR_COD_TIPO = 'NUOVE_MICRO_DA_NI'


/**FIRMA HSM MASSIVA */

BEGIN
UTL_INSERT_CHIAVE_MICROFUNZ('FIRMA_HSM_MASSIVA'
,'Abilita la firma HSM massiva'
,NULL,'N',NULL,'3.1.14',NULL);
End;


INSERT INTO DPA_TIPO_FUNZIONE
	(SELECT seq.NEXTVAL,
              'FIRMA_HSM_MASSIVA',
              'Abilita la firma HSM massiva',
              '1',
              a.system_id
         FROM dpa_amministra a);

INSERT INTO DPA_FUNZIONI
  (SELECT seq.NEXTVAL,
		  a.system_id,
		  'FIRMA_HSM_MASSIVA',
		  'FIRMA_HSM_MASSIVA',
		  null,
		  null,
		  null,
		  null,
		  f.system_id
	 FROM DPA_AMMINISTRA A, DPA_TIPO_FUNZIONE F
	 WHERE f.VAR_COD_TIPO ='FIRMA_HSM_MASSIVA' and f.id_amm = a.system_id);

/** CREAZIONE MICROFUNZIONE PER FILTRO NO_LAVORATE DA ME NELLA MASCHERA DI RICERCA TRASMISSIONI */
BEGIN
UTL_INSERT_CHIAVE_MICROFUNZ('DO_SEARCH_TRASM_NO_LAVORATE'
,'Abilita in ricerca trasmissioni il filtro No lavorate'
,NULL,'N',NULL,'3.1.14',NULL);
End;
