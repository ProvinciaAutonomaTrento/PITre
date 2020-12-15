--------------------------------------------------------
--  DDL for Function ESISTENOTAVISIBILE
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."ESISTENOTAVISIBILE" (
   p_tipooggettoassociato   VARCHAR,
   p_idoggettoassociato     INT,
   p_id_ruolo_in_uo         INT,
   p_idutentecreatore       INT,
   p_idruolocreatore        INT
)
   RETURN VARCHAR
IS
   ultimanota   VARCHAR (2000);
BEGIN
   IF (p_tipooggettoassociato <> 'F' AND p_tipooggettoassociato <> 'D')
   THEN
      ultimanota := '-1';
      RETURN ultimanota;
   END IF;

   IF p_tipooggettoassociato = 'F'
   THEN
      SELECT testo
        INTO ultimanota
        FROM (SELECT   /*+ FIRST_ROWS(1) */
                       -- HINT PRECEDENTE serve per ottimizzare tempi risposta, da oracle 10 in poi
                       -- N.SYSTEM_ID,
                       NVL (n.testo, 'null') AS testo
                  FROM dpa_note n LEFT JOIN people p
                       ON n.idutentecreatore = p.system_id
                       LEFT JOIN GROUPS g ON n.idruolocreatore = g.system_id
                       LEFT JOIN project pr
                       ON n.idoggettoassociato = pr.system_id
                       LEFT JOIN dpa_corr_globali dp
                       ON n.idruolocreatore = dp.id_gruppo
                 WHERE n.tipooggettoassociato = p_tipooggettoassociato
                   AND n.idoggettoassociato = p_idoggettoassociato
                   AND (   n.tipovisibilita = 'T'
                        OR (    n.tipovisibilita = 'F'
                            AND n.idrfassociato IN (
                                         SELECT id_registro
                                           FROM dpa_l_ruolo_reg rr
                                          WHERE dp.id_gruppo =
                                                              p_id_ruolo_in_uo)
                           )
                        OR (    n.tipovisibilita = 'P'
                            AND n.idutentecreatore = p_idutentecreatore
                           )
                        OR (    n.tipovisibilita = 'R'
                            AND n.idruolocreatore = p_idruolocreatore
                           )
                       )
              ORDER BY n.datacreazione DESC)
       WHERE ROWNUM = 1;
   END IF;

   IF p_tipooggettoassociato = 'D'
   THEN                             --join con la profile invece della project
      SELECT testo
        INTO ultimanota
        FROM (SELECT   /*+ FIRST_ROWS(1) */
                       -- HINT PRECEDENTE serve per ottimizzare tempi risposta, da oracle 10 in poi
                       -- N.SYSTEM_ID,
                       NVL (n.testo, 'null') AS testo
                  FROM dpa_note n LEFT JOIN people p
                       ON n.idutentecreatore = p.system_id
                       LEFT JOIN GROUPS g ON n.idruolocreatore = g.system_id
                       LEFT JOIN PROFILE pr
                       ON n.idoggettoassociato = pr.system_id
                       LEFT JOIN dpa_corr_globali dp
                       ON n.idruolocreatore = dp.id_gruppo
                 WHERE n.tipooggettoassociato = p_tipooggettoassociato
                   AND n.idoggettoassociato = p_idoggettoassociato
                   AND (   n.tipovisibilita = 'T'
                        OR (    n.tipovisibilita = 'F'
                            AND n.idrfassociato IN (
                                         SELECT id_registro
                                           FROM dpa_l_ruolo_reg rr
                                          WHERE dp.id_gruppo =
                                                              p_id_ruolo_in_uo)
                           )
                        OR (    n.tipovisibilita = 'P'
                            AND n.idutentecreatore = p_idutentecreatore
                           )
                        OR (    n.tipovisibilita = 'R'
                            AND n.idruolocreatore = p_idruolocreatore
                           )
                       )
              ORDER BY n.datacreazione DESC)
       WHERE ROWNUM = 1;
   END IF;

   IF (ultimanota IS NOT NULL)
   THEN
      ultimanota := 'Si';
   ELSE
      ultimanota := 'No';
   END IF;

   RETURN ultimanota;
EXCEPTION
   WHEN NO_DATA_FOUND
   THEN
      ultimanota := 'No';
      RETURN ultimanota;
   WHEN OTHERS
   THEN
      ultimanota := '-1';
      RETURN ultimanota;
END; 

/
