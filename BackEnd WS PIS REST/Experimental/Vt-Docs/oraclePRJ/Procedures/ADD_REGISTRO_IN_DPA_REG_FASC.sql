--------------------------------------------------------
--  DDL for Procedure ADD_REGISTRO_IN_DPA_REG_FASC
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."ADD_REGISTRO_IN_DPA_REG_FASC" (
   p_newidregistro         NUMBER,
   p_id_amm                NUMBER,
   p_result          OUT   INTEGER
)
IS
BEGIN
   DECLARE
      CURSOR currtit
      IS
         SELECT system_id
           FROM project
          WHERE id_amm = p_id_amm
            AND cha_tipo_proj = 'T'
            AND id_registro IS NULL;
   BEGIN
-- SE IL NODO HA REGISTRO NULL ALLORA DEVONO ESSERE CREATI TANTI RECORD NELLA
-- DPA_REG_FASC QUANTI SONO I REGISTRI INTERNI ALL'AMMINISTRAZIONE
      p_result := 0;

      FOR currenttit IN currtit
      LOOP
         BEGIN
            INSERT INTO dpa_reg_fasc
                        (system_id, id_titolario, num_rif,
                         id_registro
                        )
                 VALUES (seq.NEXTVAL, currenttit.system_id, 1,
                         p_newidregistro
                        );
         EXCEPTION
            WHEN OTHERS
            THEN
               p_result := 1;
               RETURN;
         END;
      END LOOP;
   END;
END add_registro_in_dpa_reg_fasc; 

/
