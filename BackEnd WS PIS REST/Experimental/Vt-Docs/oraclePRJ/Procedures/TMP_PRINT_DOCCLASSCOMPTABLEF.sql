--------------------------------------------------------
--  DDL for Procedure TMP_PRINT_DOCCLASSCOMPTABLEF
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."TMP_PRINT_DOCCLASSCOMPTABLEF" (
   id_amm       NUMBER,
   id_registr   NUMBER,
   id_anno      NUMBER,
   sede         VARCHAR,
   titolario    NUMBER
)
IS
 


-- modifica Aprile 2011 - uso di EXECUTE IMMEDIATE per migliorare la leggibilita del codice
istruzioneSQLbase  varchar2(20000);
istruzioneSQL      varchar2(20000); 

  outrec                  docclasscomptabletype
                := docclasscomptabletype (NULL, NULL, NULL, NULL, NULL, NULL);
   totdocclass             FLOAT;
   codclass                VARCHAR (255);
   descclass               VARCHAR (25500);
   totdocclassvt           NUMBER;
   percdocclassvt          FLOAT;
   v_var_sede              VARCHAR (100);
   num_livello1            VARCHAR (255);
   tot_primo_livello       NUMBER;
   var_codice_livello1     VARCHAR (255);
   description__livello1   VARCHAR (255);
   system_id_vt            NUMBER;
   description_vt          VARCHAR (255);
   var_codice_vt           VARCHAR (255);
   dataa                   DATE;
   datada                  DATE;
   sdataa                  VARCHAR (19);
   sdatada                 VARCHAR (19);

   CURSOR vocititolario_primoliv
   IS
      SELECT   pr.system_id AS sysid, pr.description AS descrizione,
               pr.var_codice AS var_codice, pr.num_livello
          FROM project pr
         WHERE pr.var_codice IS NOT NULL
           AND pr.id_titolario = titolario
           AND pr.id_amm = id_amm
           AND pr.cha_tipo_proj = 'T'
           AND (pr.id_registro = id_registr OR pr.id_registro IS NULL)
           AND pr.num_livello = 1
      ORDER BY pr.var_cod_liv1;
BEGIN
   sdatada := '01/01/' || TO_CHAR (id_anno) || ' 00:00:00';
   sdataa := '31/12/' || TO_CHAR (id_anno) || ' 23:59:59';
   dataa := TO_DATE (sdataa, 'dd/mm/yyyy HH24:mi:ss');
   datada := TO_DATE (sdatada, 'dd/mm/yyyy HH24:mi:ss');
   percdocclassvt := 0;
   totdocclass := 0;
   tot_primo_livello := 0;

   IF (sede = '')
   THEN
      v_var_sede := NULL;
   ELSE
      v_var_sede := sede;
   END IF;

   /*IF ((v_var_sede <> '') AND (v_var_sede IS NOT NULL))
   THEN
-- il conteggio e indistinto, non si usa la clausola DISTINCT
      SELECT COUNT ((PROFILE.system_id))
        INTO totdocclass
        FROM project_components prc, PROFILE, project pr
       WHERE prc.LINK = PROFILE.system_id
         AND prc.project_id = pr.system_id
         AND (   (PROFILE.id_registro = id_registr)
              OR (PROFILE.id_registro IS NULL)
             )
-- regole sui protocolli
         AND (   
                 -- partenza
                 (    PROFILE.cha_tipo_proto = 'P'
                  AND PROFILE.dta_proto IS NOT NULL
                  AND PROFILE.dta_annulla IS NULL
                  AND num_proto IS NOT NULL
                 )
              -- arrivo
              OR (    PROFILE.cha_tipo_proto = 'A'
                  AND PROFILE.dta_proto IS NOT NULL
                  AND PROFILE.dta_annulla IS NULL
                  AND num_proto IS NOT NULL
                 )
              -- interni
              OR (    PROFILE.cha_tipo_proto = 'I'
                  AND PROFILE.dta_proto IS NOT NULL
                  AND PROFILE.dta_annulla IS NULL
                  AND num_proto IS NOT NULL
                 )
              -- grigi
              OR (    PROFILE.cha_tipo_proto = 'G'
                  AND PROFILE.id_documento_principale IS NULL
                 )
              -- annullati
              OR (                             --profile.dta_proto is null and
                  PROFILE.dta_annulla IS NOT NULL AND num_proto IS NOT NULL
                 )
             )
         --AND TO_CHAR (PROFILE.creation_date, 'yyyy') = id_anno
         AND PROFILE.creation_date BETWEEN datada AND dataa
         AND (   (v_var_sede IS NULL AND PROFILE.var_sede IS NULL)
              OR (var_sede IS NOT NULL AND PROFILE.var_sede = v_var_sede)
             )
         AND prc.project_id IN (
-- tutte le voci di tipo C                                   --= system_id_fold
                SELECT system_id
                  FROM project
                 WHERE cha_tipo_proj = 'C'
                   AND id_fascicolo IN (
-- tutte le voci di tipo F                                   ----= parentid
                          SELECT system_id
                            FROM project
                           WHERE cha_tipo_proj = 'F'
                             AND id_parent IN (
-- recupero le voci di titolario
                                    SELECT pr.system_id
                                      --, description, var_codice, num_livello
                                    FROM   project pr
                                     WHERE pr.var_codice IS NOT NULL
                                       AND pr.id_titolario = titolario
                                       AND pr.id_amm = id_amm
                                       AND pr.cha_tipo_proj = 'T'
                                       AND (   pr.id_registro = id_registr
                                            OR pr.id_registro IS NULL
                                           )
-- in questo caso non serve ricostruire la gerarchia tra i nodi
       --CONNECT BY PRIOR pr.system_id = pr.id_parent START WITH num_livello = 1
                                 )));
   ELSE
     */ 
     
 istruzioneSQLbase := '     
     SELECT COUNT ((PROFILE.system_id))
        --INTO totdocclass
        FROM project_components prc, PROFILE, project pr     --, primo_livello
       WHERE prc.LINK = PROFILE.system_id
         AND prc.project_id = pr.system_id
         --and profile.dta_proto is not null
         AND PROFILE.creation_date BETWEEN 
         TO_DATE ('''||sdatada||''', ''dd/mm/yyyy HH24:mi:ss'')
         AND 
         
         TO_DATE ('''||sdataa||''', ''dd/mm/yyyy HH24:mi:ss'')
         AND (   (PROFILE.id_registro = '||id_registr||')
              OR (PROFILE.id_registro IS NULL)
             )
-- regole sui protocolli
         AND (   
                 -- partenza
                 (    PROFILE.cha_tipo_proto = ''P''
                  AND PROFILE.dta_proto IS NOT NULL
                  AND PROFILE.dta_annulla IS NULL
                  AND num_proto IS NOT NULL
                 )
              -- arrivo
              OR (    PROFILE.cha_tipo_proto = ''A''
                  AND PROFILE.dta_proto IS NOT NULL
                  AND PROFILE.dta_annulla IS NULL
                  AND num_proto IS NOT NULL
                 )
              -- interni
              OR (    PROFILE.cha_tipo_proto = ''I''
                  AND PROFILE.dta_proto IS NOT NULL
                  AND PROFILE.dta_annulla IS NULL
                  AND num_proto IS NOT NULL
                 )
              -- grigi
              OR (    PROFILE.cha_tipo_proto = ''G''
                  AND PROFILE.id_documento_principale IS NULL
                 )
              -- annullati
              OR (                             --profile.dta_proto is null and
                  PROFILE.dta_annulla IS NOT NULL AND num_proto IS NOT NULL
                 )
             )
         AND (   ('||  NVL(v_var_sede,'var_sede')   ||' IS NULL AND PROFILE.var_sede IS NULL)
              OR (var_sede IS NOT NULL AND PROFILE.var_sede = '|| NVL(v_var_sede,'var_sede') ||')
             )
         AND prc.project_id IN (                            --= system_id_fold
                SELECT system_id
                  FROM project
                 WHERE cha_tipo_proj = ''C''
                   AND id_fascicolo IN (                       -----= parentid
                          SELECT system_id
                            FROM project
                           WHERE cha_tipo_proj = ''F''
                             AND id_parent IN (
-- recupero le voci di titolario non occorre ricostruire il titolario a partire dal primo livello con la sua gerarchia
                                    SELECT system_id
                                      --, description, var_codice, num_livello
                                    FROM   project pr
                                     WHERE pr.var_codice IS NOT NULL
                                       AND pr.id_titolario = '||titolario||'
                                       AND pr.id_amm = '||id_amm||'
                                       AND pr.cha_tipo_proj = ''T''
                                       AND (   pr.id_registro = '||id_registr||'
                                            OR pr.id_registro IS NULL
                                           )
                                       AND pr.id_titolario = '||titolario ; 

istruzioneSQL     := istruzioneSQLbase ||'
 -- in questo caso non serve ricostruire la gerarchia tra i nodi
 --CONNECT BY PRIOR pr.system_id = pr.id_parent START WITH pr.num_livello = 1
                                 )))    ';
   --END IF;
--execute immediate istruzioneSQL into totdocclass;
 dbms_output.put_line ('v_var_sede: '||v_var_sede||';');
 
 dbms_output.put_line (istruzioneSQL);
  



   FOR cursore IN vocititolario_primoliv
   LOOP
      var_codice_livello1 := cursore.var_codice;
      description__livello1 := cursore.descrizione;

      /*IF ((v_var_sede <> '') AND (v_var_sede IS NOT NULL))       THEN
         SELECT COUNT ((PROFILE.system_id))
           INTO tot_primo_livello                                  --contatore
           FROM project_components prc, PROFILE, project pr  --, primo_livello
          WHERE prc.LINK = PROFILE.system_id
            AND prc.project_id = pr.system_id
            AND PROFILE.creation_date BETWEEN datada AND dataa
            AND (   (PROFILE.id_registro = id_registr)
                 OR (PROFILE.id_registro IS NULL)
                )
            AND (   (v_var_sede IS NULL AND PROFILE.var_sede IS NULL)
                 OR (var_sede IS NOT NULL AND PROFILE.var_sede = v_var_sede)
                )
-- regole sui protocolli
            AND (   
                    -- partenza
                    (    PROFILE.cha_tipo_proto = 'P'
                     AND PROFILE.dta_proto IS NOT NULL
                     AND PROFILE.dta_annulla IS NULL
                     AND num_proto IS NOT NULL
                    )
                 -- arrivo
                 OR (    PROFILE.cha_tipo_proto = 'A'
                     AND PROFILE.dta_proto IS NOT NULL
                     AND PROFILE.dta_annulla IS NULL
                     AND num_proto IS NOT NULL
                    )
                 -- interni
                 OR (    PROFILE.cha_tipo_proto = 'I'
                     AND PROFILE.dta_proto IS NOT NULL
                     AND PROFILE.dta_annulla IS NULL
                     AND num_proto IS NOT NULL
                    )
                 -- grigi
                 OR (    PROFILE.cha_tipo_proto = 'G'
                     AND PROFILE.id_documento_principale IS NULL
                    )
                 -- annullati
                 OR (                          --profile.dta_proto is null and
                     PROFILE.dta_annulla IS NOT NULL AND num_proto IS NOT NULL
                    )
                )
            AND prc.project_id IN (                         --= system_id_fold
                   SELECT system_id
                     FROM project
                    WHERE cha_tipo_proj = 'C'
                      AND id_fascicolo IN (                    -----= parentid
                             SELECT system_id
                               FROM project
                              WHERE cha_tipo_proj = 'F'
                                AND id_parent IN (
-- ricostruisce il titolario a partire dal primo livello con la sua gerarchia
                                       SELECT     system_id
                                             --, description, var_codice, num_livello
                                       FROM       project pr
                                            WHERE pr.var_codice IS NOT NULL
                                              AND pr.id_titolario = titolario
                                              AND pr.id_amm = id_amm
                                              AND pr.cha_tipo_proj = 'T'
                                              AND (   pr.id_registro =
                                                                    id_registr
                                                   OR pr.id_registro IS NULL
                                                  )
                                              AND pr.id_titolario = titolario
                                       CONNECT BY PRIOR pr.system_id =
                                                                  pr.id_parent
-- cursore
                                       START WITH pr.system_id = cursore.sysid)));
      ELSE
        */ 
        
        SELECT COUNT ((PROFILE.system_id))
           INTO tot_primo_livello                                  --contatore
           FROM project_components prc, PROFILE, project pr  --, primo_livello
          WHERE prc.LINK = PROFILE.system_id
  
     AND (   (v_var_sede IS NULL AND PROFILE.var_sede IS NULL)
              OR (var_sede IS NOT NULL AND PROFILE.var_sede = v_var_sede)
             )
    
          AND prc.project_id = pr.system_id
            AND PROFILE.creation_date BETWEEN datada AND dataa
            AND (   (PROFILE.id_registro = id_registr)
                 OR (PROFILE.id_registro IS NULL)
                )
-- regole sui protocolli
            AND (   
                    -- partenza
                    (    PROFILE.cha_tipo_proto = 'P'
                     AND PROFILE.dta_proto IS NOT NULL
                     AND PROFILE.dta_annulla IS NULL
                     AND num_proto IS NOT NULL
                    )
                 -- arrivo
                 OR (    PROFILE.cha_tipo_proto = 'A'
                     AND PROFILE.dta_proto IS NOT NULL
                     AND PROFILE.dta_annulla IS NULL
                     AND num_proto IS NOT NULL
                    )
                 -- interni
                 OR (    PROFILE.cha_tipo_proto = 'I'
                     AND PROFILE.dta_proto IS NOT NULL
                     AND PROFILE.dta_annulla IS NULL
                     AND num_proto IS NOT NULL
                    )
                 -- grigi
                 OR (    PROFILE.cha_tipo_proto = 'G'
                     AND PROFILE.id_documento_principale IS NULL
                    )
                 -- annullati
                 OR (                          --profile.dta_proto is null and
                     PROFILE.dta_annulla IS NOT NULL AND num_proto IS NOT NULL
                    )
                )
            AND prc.project_id IN (                         --= system_id_fold
                   SELECT system_id
                     FROM project
                    WHERE cha_tipo_proj = 'C'
                      AND id_fascicolo IN (                    -----= parentid
                             SELECT system_id
                               FROM project
                              WHERE cha_tipo_proj = 'F'
                                AND id_parent IN (
-- ricostruisce il titolario a partire dal primo livello con la sua gerarchia
                                       SELECT     system_id
                                             --, description, var_codice, num_livello
                                       FROM       project pr
                                            WHERE pr.var_codice IS NOT NULL
                                              AND pr.id_titolario = titolario
                                              AND pr.id_amm = id_amm
                                              AND pr.cha_tipo_proj = 'T'
                                              AND (   pr.id_registro =
                                                                    id_registr
                                                   OR pr.id_registro IS NULL
                                                  )
                                              AND pr.id_titolario = titolario
                                       CONNECT BY PRIOR pr.system_id =
                                                                  pr.id_parent
-- cursore
                                       START WITH pr.system_id = cursore.sysid)));
      --END IF;

/*
istruzioneSQL     := istruzioneSQLbase ||'
CONNECT BY PRIOR pr.system_id =pr.id_parent
-- cursore
START WITH pr.system_id = '||cursore.sysid||'))) ';
   --END IF;
execute immediate istruzioneSQL into tot_primo_livello  ; 
*/


      percdocclassvt := 0;

      IF totdocclass <> 0
      THEN
         percdocclassvt :=
                        ROUND (((tot_primo_livello / totdocclass) * 100), 2);
      END IF;

      
      tot_primo_livello := 0;
      percdocclassvt := 0;
   END LOOP;

   RETURN;
EXCEPTION
   WHEN OTHERS
   THEN
      --RAISE;
      
      RETURN;
END tmp_print_docclasscomptablef; 

/
