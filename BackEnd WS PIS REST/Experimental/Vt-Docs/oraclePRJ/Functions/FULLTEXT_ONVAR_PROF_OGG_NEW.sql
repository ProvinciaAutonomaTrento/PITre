--------------------------------------------------------
--  DDL for Function FULLTEXT_ONVAR_PROF_OGG_NEW
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."FULLTEXT_ONVAR_PROF_OGG_NEW" (
   stringa               VARCHAR,
   --myregistro            INTEGER,
   start_creation_date   VARCHAR, -- fmt dd/mm/yyyy
   end_creation_date     VARCHAR, -- fmt dd/mm/yyyy
   start_date            VARCHAR, -- fmt dd/mm/yyyy
   end_date              VARCHAR  -- fmt dd/mm/yyyy
)
   RETURN fulltext_objrow PIPELINED
IS
   out_record   fulltext_obj
                             /*
                             := fulltext_obj (NULL,                       NULL,                       NULL,                       NULL,
                                              NULL,                       NULL,                       NULL,                       NULL,
                                              NULL,                       NULL,                       NULL,                       NULL,
                                              NULL,                       NULL,                       NULL,                       NULL,
                                              NULL,                       NULL,                       NULL,                       NULL,
                                              NULL,                       NULL,                       NULL,                       NULL
                                             )
                                             */
   := fulltext_obj (NULL, NULL, NULL);

   CURSOR c_sysid
   IS
      SELECT a.system_id, a.docnumber,                      -- a.dta_annulla,
             a.var_prof_oggetto AS var_desc_oggetto
        /*, a.id_registro,
        a.cha_tipo_proto, a.cha_evidenza, a.num_anno_proto,
        TO_CHAR (NVL (a.dta_proto, a.creation_time),
                 'dd/mm/yyyy'
                ) AS DATA,
        TO_CHAR (a.creation_time, 'dd/mm/yyyy') AS creation_date,
        a.num_proto, a.var_segnatura,
        TO_CHAR (a.dta_proto, 'dd/mm/yyyy') AS dta_proto, a.cha_privato,
        a.cha_personale, a.id_documento_principale, a.cha_in_archivio,
        a.id_tipo_atto AS id_tipo_atto,
        ta.var_desc_atto AS desc_tipo_atto, a.prot_tit,
        a.archive_date AS dta_archiviazione, a.id_ruolo_creatore,
        a.author, a.cha_cod_t_a */
      FROM   PROFILE a                                   --, dpa_tipo_atto ta
       WHERE catsearch (a.var_prof_oggetto, stringa, ' 
           creation_time >= NVL (TO_DATE ('''||start_creation_date||''', ''dd/mm/yyyy''), TO_DATE (''01/01/2000 00:00:00'', ''dd/mm/yyyy HH24:mi:ss''))
       AND creation_time <= NVL (TO_DATE ('''||  end_creation_date||' 23:59:59'',''dd/mm/yyyy HH24:mi:ss''), SYSDATE + 1) ') > 0
         --AND a.id_tipo_atto = ta.system_id(+)
         AND (    a.cha_tipo_proto IN ('G', 'P', 'A', 'I')
              AND a.cha_da_proto = '0'
              AND a.id_documento_principale IS NULL
             )
         AND NVL (a.cha_in_cestino, '0') = '0'
         AND NVL (a.dta_proto, a.creation_time)
                BETWEEN NVL (TO_DATE (start_date, 'dd/mm/yyyy'),
                             TO_DATE ('01/01/2000 00:00:00',
                                      'dd/mm/yyyy HH24:mi:ss'
                                     )
                            )
                    AND NVL (TO_DATE (end_date||' 23:59:59','dd/mm/yyyy HH24:mi:ss'), SYSDATE + 1)
      UNION
      SELECT 1 AS system_id, 1 as docnumber,
             'la ricerca non ha prodotto risultati perche il termine cercato e presente nella lista dei termini comuni!'
                                                          AS var_desc_oggetto
        FROM ctx_stopwords
       WHERE spw_word = stringa;


-- in caso si tolga condizione su anno
   CURSOR c_sysid_nodata
   IS
      SELECT a.system_id, a.docnumber,             
             a.var_prof_oggetto AS var_desc_oggetto
      FROM   PROFILE a                                   
       WHERE catsearch (a.var_prof_oggetto, stringa, '') > 0
         AND (    a.cha_tipo_proto IN ('G', 'P', 'A', 'I')
              AND a.cha_da_proto = '0'
              AND a.id_documento_principale IS NULL
             )
         AND NVL (a.cha_in_cestino, '0') = '0'
      UNION
      SELECT 1 AS system_id, 1 as docnumber,
             'la ricerca non ha prodotto risultati perche il termine cercato e presente nella lista dei termini comuni!'
                                                          AS var_desc_oggetto
        FROM ctx_stopwords
       WHERE spw_word = stringa;


BEGIN

-- ramo in cui entra se si toglie l'anno, modificare opportunamente condizione in base ai parametri passati 
IF (  end_creation_date = '01/01/2005' and end_date  =  '01/01/2005' ) THEN
   FOR c1 IN c_sysid_nodata
   LOOP
      EXIT WHEN c_sysid%NOTFOUND;
      out_record.system_id := c1.system_id;
      out_record.docnumber := c1.docnumber;
      out_record.var_desc_oggetto := c1.var_desc_oggetto;
      PIPE ROW (out_record);
end loop;
ELSE -- tutti gli altri casi
   FOR c1 IN c_sysid
   LOOP
      EXIT WHEN c_sysid%NOTFOUND;
      out_record.system_id := c1.system_id;
      out_record.docnumber := c1.docnumber;
      --out_record.dta_annulla := c1.dta_annulla;
      out_record.var_desc_oggetto := c1.var_desc_oggetto;
      /*out_record.id_registro := c1.id_registro;
      out_record.cha_tipo_proto := c1.cha_tipo_proto;
      out_record.cha_evidenza := c1.cha_evidenza;
      out_record.num_anno_proto := c1.num_anno_proto;
      out_record.DATA := c1.DATA;
      out_record.creation_date := c1.creation_date;
      out_record.num_proto := c1.num_proto;
      out_record.var_segnatura := c1.var_segnatura;
      out_record.dta_proto := c1.dta_proto;
      out_record.cha_privato := c1.cha_privato;
      out_record.cha_personale := c1.cha_personale;
      out_record.id_documento_principale := c1.id_documento_principale;
      out_record.cha_in_archivio := c1.cha_in_archivio;
      out_record.id_tipo_atto := c1.id_tipo_atto;
      out_record.desc_tipo_atto := c1.desc_tipo_atto;
      out_record.prot_tit := c1.prot_tit;
      out_record.dta_archiviazione := c1.dta_archiviazione;
      out_record.id_ruolo_creatore := c1.id_ruolo_creatore;
      out_record.author := c1.author;
      out_record.cha_cod_t_a := c1.cha_cod_t_a; */
      PIPE ROW (out_record);
   END LOOP;
END IF;
   
EXCEPTION
   WHEN OTHERS
   THEN
      --RAISE;
      out_record.var_desc_oggetto := SUBSTR (SQLERRM, 1, 2000);      PIPE ROW (out_record);
      PIPE ROW (out_record);
END; 

/
