--------------------------------------------------------
--  DDL for Procedure SP_DPA_COUNT_TODOLIST
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."SP_DPA_COUNT_TODOLIST" (
id_people_p   IN   NUMBER,
id_gruppo     IN   NUMBER,
ts            IN   VARCHAR
)
IS
trasmdoctot             NUMBER;
trasmdocnonletti        NUMBER;
trasmdocnonaccettati    NUMBER;
trasmfasctot            NUMBER;
trasmfascnonletti       NUMBER;
trasmfascnonaccettati   NUMBER;
docpredisposti          NUMBER;
ts_stampa_p             DATE;
BEGIN
--numero documenti presenti in todolist
SELECT COUNT (DISTINCT (id_trasmissione))
INTO trasmdoctot
FROM dpa_todolist
WHERE (    id_profile > 0
AND (   id_registro IN (
SELECT id_registro
FROM dpa_l_ruolo_reg g, dpa_el_registri r
WHERE r.system_id = g.id_registro
AND r.cha_rf = '0'
AND id_ruolo_in_uo IN (SELECT system_id
FROM dpa_corr_globali
WHERE id_gruppo = id_gruppo))
OR id_registro = 0
)
)
AND (   (id_people_dest = id_people_p AND id_ruolo_dest = id_gruppo)
OR (id_people_dest = id_people_p AND id_ruolo_dest = 0)
);

--numero documenti non letti in todolist
SELECT COUNT (DISTINCT (id_trasmissione))
INTO trasmdocnonletti
FROM dpa_todolist
WHERE (    id_profile > 0
AND (   id_registro IN (
SELECT id_registro
FROM dpa_l_ruolo_reg g, dpa_el_registri r
WHERE r.system_id = g.id_registro
AND r.cha_rf = '0'
AND id_ruolo_in_uo IN (SELECT system_id
FROM dpa_corr_globali
WHERE id_gruppo = id_gruppo))
OR id_registro = 0
)
)
AND (   (id_people_dest = id_people_p AND id_ruolo_dest = id_gruppo)
OR (id_people_dest = id_people_p AND id_ruolo_dest = 0)
)
AND dta_vista = TO_DATE ('01/01/1753', 'dd/mm/yyyy');

--numero documenti non ancora accettati in todolist
SELECT COUNT (DISTINCT (id_trasmissione))
INTO trasmdocnonaccettati
FROM dpa_todolist
WHERE (    id_profile > 0
AND (   id_registro IN (
SELECT id_registro
FROM dpa_l_ruolo_reg g, dpa_el_registri r
WHERE r.system_id = g.id_registro
AND r.cha_rf = '0'
AND id_ruolo_in_uo IN (SELECT system_id
FROM dpa_corr_globali
WHERE id_gruppo = id_gruppo))
OR id_registro = 0
)
)
AND (   (id_people_dest = id_people_p AND id_ruolo_dest = id_gruppo)
OR (id_people_dest = id_people_p AND id_ruolo_dest = 0)
)
and exists (select dpa_ragione_trasm.system_id from dpa_ragione_trasm
where id_ragione_trasm = dpa_ragione_trasm.system_id
and dpa_ragione_trasm.CHA_TIPO_RAGIONE = 'W' )
AND dta_vista =  to_date('01/01/1753','dd/mm/yyyy HH24:mi:ss');

--numero fascicoli presenti in todolist
SELECT COUNT (DISTINCT (id_trasmissione))
INTO trasmfasctot
FROM dpa_todolist
WHERE id_project > 0
AND (   (id_people_dest = id_people_p AND id_ruolo_dest = id_gruppo)
OR (id_people_dest = id_people_p AND id_ruolo_dest = 0)
);

--numero fascicoli non letti in todolist
SELECT COUNT (DISTINCT (id_trasmissione))
INTO trasmfascnonletti
FROM dpa_todolist
WHERE id_project > 0
AND (   (id_people_dest = id_people_p AND id_ruolo_dest = id_gruppo)
OR (id_people_dest = id_people_p AND id_ruolo_dest = 0)
)
AND dta_vista = TO_DATE ('01/01/1753', 'dd/mm/yyyy');

--numero fascicoli non ancora accettati in todolist
SELECT COUNT (DISTINCT (id_trasmissione))
INTO trasmfascnonaccettati
FROM dpa_todolist
WHERE id_project > 0
AND (   (id_people_dest = id_people_p AND id_ruolo_dest = id_gruppo)
OR (id_people_dest = id_people_p AND id_ruolo_dest = 0)
)
and exists (select dpa_ragione_trasm.system_id from dpa_ragione_trasm
where id_ragione_trasm = dpa_ragione_trasm.system_id
and dpa_ragione_trasm.CHA_TIPO_RAGIONE = 'W' )
AND dta_vista =  to_date('01/01/1753','dd/mm/yyyy HH24:mi:ss');

--numero documenti predisposti
SELECT COUNT (DISTINCT (id_trasmissione))
INTO docpredisposti
FROM dpa_todolist
WHERE (    id_profile > 0
AND (   id_registro IN (
SELECT id_registro
FROM dpa_l_ruolo_reg g, dpa_el_registri r
WHERE r.system_id = g.id_registro
AND r.cha_rf = '0'
AND id_ruolo_in_uo IN (SELECT system_id
FROM dpa_corr_globali
WHERE id_gruppo = id_gruppo))
OR id_registro = 0
)
)
AND id_profile IN (SELECT system_id
FROM PROFILE
WHERE cha_da_proto = '1')
AND (   (id_people_dest = id_people_p AND id_ruolo_dest = id_gruppo)
OR (id_people_dest = id_people_p AND id_ruolo_dest = 0)
);

BEGIN                                                               -- MAIN
ts_stampa_p := TO_DATE (ts, 'dd/mm/yyyy hh24:mi:ss');

-- SVUOTO LA TABELLA DEI DATI
DELETE      dpa_count_todolist
WHERE (id_people = id_people_p);

INSERT INTO dpa_count_todolist
(id_people, ts_stampa, tot_doc, tot_doc_no_letti,
tot_doc_no_accettati, tot_fasc, tot_fasc_no_letti,
tot_fasc_no_accettati, tot_doc_predisposti
)
VALUES (id_people_p, ts_stampa_p, trasmdoctot, trasmdocnonletti,
trasmdocnonaccettati, trasmfasctot, trasmfascnonletti,
trasmfascnonaccettati, docpredisposti
);
END;                                                                -- MAIN
EXCEPTION
WHEN OTHERS
THEN
RETURN;
END;

/
