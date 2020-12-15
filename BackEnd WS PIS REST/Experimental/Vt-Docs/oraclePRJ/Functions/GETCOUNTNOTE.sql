--------------------------------------------------------
--  DDL for Function GETCOUNTNOTE
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETCOUNTNOTE" (tipoOggetto CHAR, idOggetto NUMBER, note NVARCHAR2, idUtente NUMBER, idGruppo NUMBER, tipoRic char, idRegistro NUMBER)
RETURN NUMBER IS retValue NUMBER;

BEGIN

IF tipoRic = 'Q' THEN
SELECT COUNT(SYSTEM_ID) INTO retValue
FROM   DPA_NOTE N
WHERE  N.TIPOOGGETTOASSOCIATO = tipoOggetto AND
N.IDOGGETTOASSOCIATO = idOggetto AND
upper(N.TESTO) LIKE upper('%'||note||'%') AND
(N.TIPOVISIBILITA = 'T' OR
(N.TIPOVISIBILITA = 'P' AND N.IDUTENTECREATORE = idUtente) OR
(N.TIPOVISIBILITA = 'R' AND N.IDRUOLOCREATORE = idGruppo)OR
(n.tipovisibilita = 'F' AND n.idrfassociato in
(select id_registro from dpa_l_ruolo_Reg r,dpa_el_registri lr where lr.CHA_RF='1' and r.ID_REGISTRO=lr.SYSTEM_ID
and r.ID_RUOLO_IN_UO in (select system_id from dpa_corr_globali where id_gruppo=idGruppo ))
)

);

ELSIF tipoRic = 'T' THEN
SELECT COUNT(SYSTEM_ID) INTO retValue
FROM   DPA_NOTE N
WHERE  N.TIPOOGGETTOASSOCIATO = tipoOggetto AND
N.IDOGGETTOASSOCIATO = idOggetto AND
upper(N.TESTO) LIKE upper('%'||note||'%') AND
N.TIPOVISIBILITA = 'T';

ELSIF tipoRic = 'P' THEN
SELECT COUNT(SYSTEM_ID) INTO retValue
FROM   DPA_NOTE N
WHERE  N.TIPOOGGETTOASSOCIATO = tipoOggetto AND
N.IDOGGETTOASSOCIATO = idOggetto AND
upper(N.TESTO) LIKE upper('%'||note||'%') AND
(N.TIPOVISIBILITA = 'P' AND N.IDUTENTECREATORE = idUtente);

ELSIF tipoRic = 'R' THEN
SELECT COUNT(SYSTEM_ID) INTO retValue
FROM   DPA_NOTE N
WHERE  N.TIPOOGGETTOASSOCIATO = tipoOggetto AND
N.IDOGGETTOASSOCIATO = idOggetto AND
upper(N.TESTO) LIKE upper('%'||note||'%') AND
(N.TIPOVISIBILITA = 'R' AND N.IDRUOLOCREATORE = idGruppo);

ELSIF tipoRic = 'F' THEN
SELECT COUNT(SYSTEM_ID) INTO retValue
FROM   DPA_NOTE N
WHERE  N.TIPOOGGETTOASSOCIATO = tipoOggetto AND
N.IDOGGETTOASSOCIATO = idOggetto AND
upper(N.TESTO) LIKE upper('%'||note||'%') AND
(N.TIPOVISIBILITA = 'F' and N.IDRFASSOCIATO = idRegistro);


END IF;

RETURN retValue;
END GetCountNote;

/
