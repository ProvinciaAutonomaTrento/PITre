--------------------------------------------------------
--  DDL for Procedure SETDATAREG
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."SETDATAREG" IS BEGIN
 DECLARE
      CURSOR c
      IS
         SELECT a.dta_open, b.num_rif, a.system_id AS sysid
           FROM dpa_el_registri a, dpa_reg_proto b
          WHERE a.system_id = b.id_registro
            AND a.cha_automatico = '1'
            AND cha_stato = 'A';

      c1   c%ROWTYPE;
   BEGIN
      OPEN c;

      LOOP
         FETCH c
          INTO c1;

         EXIT WHEN c%NOTFOUND;

         BEGIN
--INSERT INTO DPA_REGISTRI_STO
            INSERT INTO dpa_registro_sto
                        (system_id, dta_open, dta_close, num_rif,
                         id_registro, id_people, id_ruolo_in_uo)
               SELECT (seq.NEXTVAL), a.dta_open, SYSDATE, b.num_rif,
                      a.system_id, 1, 1
                 FROM dpa_el_registri a, dpa_reg_proto b
                WHERE a.system_id = b.id_registro AND a.system_id = c1.sysid;

            UPDATE dpa_el_registri
               SET dta_open = SYSDATE,
                   cha_stato = 'A',
                   dta_close = NULL
             WHERE system_id = c1.sysid;

            UPDATE dpa_reg_proto
               SET num_rif = 1
             WHERE TO_CHAR (SYSDATE, 'dd/mm') = '01/01'
             -- agisce solo per quei registri che hanno autopertura impostata, cha_automatico = '1'
             and id_registro = c1.sysid ;
         EXCEPTION
            WHEN OTHERS
            THEN
               RAISE ;
         END;
      END LOOP;

      CLOSE c;

      COMMIT;
   END;
END setdatareg; 

/
