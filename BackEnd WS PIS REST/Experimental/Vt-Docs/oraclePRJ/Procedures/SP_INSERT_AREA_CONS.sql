--------------------------------------------------------
--  DDL for Procedure SP_INSERT_AREA_CONS
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."SP_INSERT_AREA_CONS" (
   p_idamm                   NUMBER,
   p_idconservazione         NUMBER,
   p_idpeople                NUMBER,
   p_idprofile               NUMBER,
   p_idproject               NUMBER,
   p_codfasc                 VARCHAR,
   p_oggetto                 VARCHAR,
   p_tipodoc                 CHAR,
   p_idgruppo                NUMBER,
   p_idregistro              NUMBER,
   p_docnumber               NUMBER,
   p_userid                  VARCHAR,
   p_tipooggetto             CHAR,
   p_tipoatto                VARCHAR,
   p_idpolicy                NUMBER,
   p_result            OUT   NUMBER
)
IS
   idruoloinuo   NUMBER := 0;
   id_cons_1     NUMBER := 0;
   id_cons_2     NUMBER := 0;
   res           NUMBER := 0;
BEGIN
   BEGIN
      SELECT seq_conservazione.NEXTVAL
        INTO id_cons_1
        FROM DUAL;

      SELECT seq_conservazione.NEXTVAL
        INTO id_cons_2
        FROM DUAL;

      SELECT dpa_corr_globali.system_id
        INTO idruoloinuo
        FROM dpa_corr_globali
       WHERE dpa_corr_globali.id_gruppo = p_idgruppo;

      IF (p_idconservazione IS NULL)
      THEN
         BEGIN
            -- Reperimento istanza di conservazione corrente
            SELECT dpa_area_conservazione.system_id
                       INTO res
                       FROM dpa_area_conservazione
                      WHERE dpa_area_conservazione.id_people = p_idpeople
                        AND dpa_area_conservazione.id_ruolo_in_uo =
                                                                   idruoloinuo
                        AND dpa_area_conservazione.cha_stato = 'N'
                        AND dpa_area_conservazione.is_preferred ='1';
         EXCEPTION
            WHEN OTHERS
            THEN
               res := 0;
         END;
      ELSE
         BEGIN
            res := p_idconservazione;
         END;
      END IF;

      
      IF (res is null or res=0)
      THEN
         BEGIN
            -- Reperimento istanza di conservazione corrente
            SELECT sys INTO res FROM (SELECT dpa_area_conservazione.system_id as sys
                       FROM dpa_area_conservazione
                      WHERE dpa_area_conservazione.id_people = p_idpeople
                        AND dpa_area_conservazione.id_ruolo_in_uo =
                                                                   idruoloinuo
                        AND dpa_area_conservazione.cha_stato = 'N' order by dpa_area_conservazione.system_id desc) WHERE rownum=1;
         EXCEPTION
            WHEN OTHERS
            THEN
               res := 0;
         END;
      END IF;
      

      IF (res > 0)
      THEN
         INSERT INTO dpa_items_conservazione
                     (system_id, id_conservazione, id_profile, id_project,
                      cha_tipo_doc, var_oggetto, id_registro, data_ins,
                      cha_stato, var_xml_metadati, cod_fasc, docnumber,
                      cha_tipo_oggetto, var_tipo_atto 
                     )
              VALUES (id_cons_1, res, p_idprofile, p_idproject,
                      p_tipodoc, p_oggetto, p_idregistro, SYSDATE,
                      'N', EMPTY_CLOB (), p_codfasc, p_docnumber,
                      p_tipooggetto, p_tipoatto
                     );

         p_result := id_cons_1;
      ELSE
         INSERT INTO dpa_area_conservazione
                     (system_id, id_amm, id_people, id_ruolo_in_uo,
                      cha_stato, data_apertura, user_id, id_gruppo, id_policy,
                      var_file_chiusura, var_file_chiusura_firmato
                      
                     )
              VALUES (id_cons_1, p_idamm, p_idpeople, idruoloinuo,
                      'N', SYSDATE, p_userid, p_idgruppo, p_idpolicy,
                      EMPTY_CLOB(),EMPTY_CLOB()
                     );

         INSERT INTO dpa_items_conservazione
                     (system_id, id_conservazione, id_profile, id_project,
                      cha_tipo_doc, var_oggetto, id_registro, data_ins,
                      cha_stato, var_xml_metadati, cod_fasc, docnumber,
                      cha_tipo_oggetto, var_tipo_atto
                     )
              VALUES (id_cons_2, id_cons_1, p_idprofile, p_idproject,
                      p_tipodoc, p_oggetto, p_idregistro, SYSDATE,
                      'N', EMPTY_CLOB (), p_codfasc, p_docnumber,
                      p_tipooggetto, p_tipoatto
                     );

         p_result := id_cons_2;
      END IF;
   EXCEPTION
      WHEN OTHERS
      THEN
         p_result := 1;
   END;
END;

/
