--------------------------------------------------------
--  DDL for Function FASCICOLITABLEFUNCTION
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."FASCICOLITABLEFUNCTION" (
   p_id_amm        NUMBER,
   p_id_registro   NUMBER,
   anno            NUMBER,
   mese            NUMBER,
   p_tit           INT
)
   RETURN fascicolitablerow PIPELINED
IS
--dichiarazione
   out_rec        fascicolitabletype
      := fascicolitabletype (NULL,
                             NULL,
                             NULL,
                             NULL,
                             NULL,
                             NULL,
                             NULL,
                             NULL,
                             NULL
                            );
-- variabili globali
   totfasc        FLOAT;
   totfasca       FLOAT;
   totfascc       FLOAT;
   mese_vc        VARCHAR (255);
--variabili mensili
   contamese      NUMBER;
   totfascm       FLOAT;
   totfascma      FLOAT;
   totfascmc      FLOAT;
   totpercfasca   FLOAT;
   totpercfascc   FLOAT;
BEGIN
--settaggio variabili
   totfasc := 0;
   totfasca := 0;
   totfascc := 0;
   contamese := 1;
   totfascm := 0;
   totfascma := 0;
   totfascmc := 0;
   totpercfasca := 0;
   totpercfascc := 0;

--conta valori globali
-- CONTA FASCICOLI totali nella amministrazione (@id_amm) e/o registro (@id_registro) se presente
   IF p_tit <> 0
   THEN
      SELECT COUNT (project.system_id)
        INTO totfasc
        FROM project
       WHERE project.cha_tipo_proj = 'F'
         AND project.id_amm = p_id_amm
         AND TO_NUMBER (TO_CHAR (project.dta_creazione, 'YYYY')) = anno
         AND (   project.id_registro = p_id_registro
              OR project.id_registro IS NULL
             )
-- MODIFICA aprile 2011 richiesta da Schonaut -- filtra solo su titolario attivo
-- MODIFICA luglio 2011 richiesta da Schonaut
         AND project.id_titolario = p_tit
/*             (SELECT system_id
                FROM project
               WHERE cha_stato = 'A' AND var_codice = 'T'
                     AND id_amm = p_id_amm)
                     */
      ;

-- CONTA FASCICOLI CREATI NELL'ANNO  nella amministrazione (@id_amm) e/o registro (@id_registro) se presente
      SELECT COUNT (project.system_id)
        INTO totfasca
        FROM project
       WHERE project.cha_tipo_proj = 'F'
         AND TO_NUMBER (TO_CHAR (project.dta_creazione, 'YYYY')) = anno
         AND project.cha_stato = 'A'
         AND project.id_amm = p_id_amm
         AND (   project.id_registro = p_id_registro
              OR project.id_registro IS NULL
             )
-- MODIFICA aprile 2011 richiesta da Schonaut -- filtra solo su titolario attivo
-- MODIFICA luglio 2011 richiesta da Schonaut
         AND project.id_titolario = p_tit
/*             (SELECT system_id
                FROM project
               WHERE cha_stato = 'A' AND var_codice = 'T'
                     AND id_amm = p_id_amm)
                     */
      ;

-- CONTA FASCICOLI CHIUSI NELL'ANNO nella amministrazione (@id_amm) e/o registro (@id_registro) se presente
      SELECT COUNT (project.system_id)
        INTO totfascc
        FROM project
       WHERE project.cha_tipo_proj = 'F'
         AND project.cha_stato = 'C'
         AND TO_NUMBER (TO_CHAR (project.dta_creazione, 'YYYY')) = anno
-- commentata condizione su dta_chiusura perche vale la condizione sullo stato
--and  to_number(to_char(project.dta_chiusura,'YYYY')) = anno
         AND project.id_amm = p_id_amm
         AND (   project.id_registro = p_id_registro
              OR project.id_registro IS NULL
             )
-- MODIFICA aprile 2011 richiesta da Schonaut -- filtra solo su titolario attivo
-- MODIFICA luglio 2011 richiesta da Schonaut
         AND project.id_titolario = p_tit
/*             (SELECT system_id
                FROM project
               WHERE cha_stato = 'A' AND var_codice = 'T'
                     AND id_amm = p_id_amm)
                     */
      ;
   ELSE                                             -- non filtro per titolari
      SELECT COUNT (project.system_id)
        INTO totfasc
        FROM project
       WHERE project.cha_tipo_proj = 'F'
         AND project.id_amm = p_id_amm
         AND TO_NUMBER (TO_CHAR (project.dta_creazione, 'YYYY')) = anno
         AND (   project.id_registro = p_id_registro
              OR project.id_registro IS NULL
             );

-- CONTA FASCICOLI CREATI NELL'ANNO  nella amministrazione (@id_amm) e/o registro (@id_registro) se presente
      SELECT COUNT (project.system_id)
        INTO totfasca
        FROM project
       WHERE project.cha_tipo_proj = 'F'
         AND TO_NUMBER (TO_CHAR (project.dta_creazione, 'YYYY')) = anno
         AND project.cha_stato = 'A'
         AND project.id_amm = p_id_amm
         AND (   project.id_registro = p_id_registro
              OR project.id_registro IS NULL
             );

-- CONTA FASCICOLI CHIUSI NELL'ANNO nella amministrazione (@id_amm) e/o registro (@id_registro) se presente
      SELECT COUNT (project.system_id)
        INTO totfascc
        FROM project
       WHERE project.cha_tipo_proj = 'F'
         AND project.cha_stato = 'C'
         AND TO_NUMBER (TO_CHAR (project.dta_creazione, 'YYYY')) = anno
-- commentata condizione su dta_chiusura perche vale la condizione sullo stato
--and  to_number(to_char(project.dta_chiusura,'YYYY')) = anno
         AND project.id_amm = p_id_amm
         AND (   project.id_registro = p_id_registro
              OR project.id_registro IS NULL
             );
   END IF;

--fine conta

   --ciclo scansione mensile
   WHILE (mese >= contamese)
   LOOP
--conto  i fascicoli creati (nel mese) della amministrazione (@id_amm) e/o registro (@id_registro) se presente
      IF p_tit <> 0
      THEN
         SELECT COUNT (project.system_id)
           INTO totfascma
           FROM project
          WHERE project.cha_tipo_proj = 'F'
            AND project.cha_stato = 'A'
            AND TO_NUMBER (TO_CHAR (project.dta_creazione, 'MM')) = contamese
            AND TO_NUMBER (TO_CHAR (project.dta_creazione, 'YYYY')) = anno
            AND project.id_amm = p_id_amm
            AND (   project.id_registro = p_id_registro
                 OR project.id_registro IS NULL
                )
-- MODIFICA aprile 2011 richiesta da Schonaut -- filtra solo su titolario attivo
-- MODIFICA luglio 2011 richiesta da Schonaut
            AND project.id_titolario = p_tit
/*             (SELECT system_id
                FROM project
               WHERE cha_stato = 'A' AND var_codice = 'T'
                     AND id_amm = p_id_amm)
                     */
         ;

--conto  i fascicoli chiusi (nel mese) della amministrazione (@id_amm) e/o registro (@id_registro) se presente
         SELECT COUNT (project.system_id)
           INTO totfascmc
           FROM project
          WHERE project.cha_tipo_proj = 'F'
            AND project.cha_stato = 'C'
            AND TO_NUMBER (TO_CHAR (project.dta_creazione, 'MM')) = contamese
            AND TO_NUMBER (TO_CHAR (project.dta_creazione, 'YYYY')) = anno
--and  to_number(to_char(project.dta_chiusura,'MM')) = contaMese
--and to_number(to_char(project.dta_chiusura,'YYYY')) = anno
            AND project.id_amm = p_id_amm
            AND (   project.id_registro = p_id_registro
                 OR project.id_registro IS NULL
                )
-- MODIFICA aprile 2011 richiesta da Schonaut -- filtra solo su titolario attivo
-- MODIFICA luglio 2011 richiesta da Schonaut
            AND project.id_titolario = p_tit
/*             (SELECT system_id
                FROM project
               WHERE cha_stato = 'A' AND var_codice = 'T'
                     AND id_amm = p_id_amm)
                     */
         ;
      ELSE   -- non filtro per titolario 
         SELECT COUNT (project.system_id)
           INTO totfascma
           FROM project
          WHERE project.cha_tipo_proj = 'F'
            AND project.cha_stato = 'A'
            AND TO_NUMBER (TO_CHAR (project.dta_creazione, 'MM')) = contamese
            AND TO_NUMBER (TO_CHAR (project.dta_creazione, 'YYYY')) = anno
            AND project.id_amm = p_id_amm
            AND (   project.id_registro = p_id_registro
                 OR project.id_registro IS NULL
                )         ;

--conto  i fascicoli chiusi (nel mese) della amministrazione (@id_amm) e/o registro (@id_registro) se presente
         SELECT COUNT (project.system_id)
           INTO totfascmc
           FROM project
          WHERE project.cha_tipo_proj = 'F'
            AND project.cha_stato = 'C'
            AND TO_NUMBER (TO_CHAR (project.dta_creazione, 'MM')) = contamese
            AND TO_NUMBER (TO_CHAR (project.dta_creazione, 'YYYY')) = anno
--and  to_number(to_char(project.dta_chiusura,'MM')) = contaMese
--and to_number(to_char(project.dta_chiusura,'YYYY')) = anno
            AND project.id_amm = p_id_amm
            AND (   project.id_registro = p_id_registro
                 OR project.id_registro IS NULL
                )         ;
      END IF;

      totfascm := totfascma + totfascmc;

--calcolo percentuali
      IF (totfascm <> 0)
      THEN
         totpercfasca := ROUND (((totfascma / totfascm) * 100), 2);
         totpercfascc := ROUND (((totfascmc / totfascm) * 100), 2);
      END IF;

-- parsing valore mese
      mese_vc :=
         CASE contamese
            WHEN 1
               THEN 'Gennaio'
            WHEN 2
               THEN 'Febbraio'
            WHEN 3
               THEN 'Marzo'
            WHEN 4
               THEN 'Aprile'
            WHEN 5
               THEN 'Maggio'
            WHEN 6
               THEN 'Giugno'
            WHEN 7
               THEN 'Luglio'
            WHEN 8
               THEN 'Agosto'
            WHEN 9
               THEN 'Settembre'
            WHEN 10
               THEN 'Ottobre'
            WHEN 11
               THEN 'Novembre'
            WHEN 12
               THEN 'Dicembre'
         END;
--
-- inserimento dati nella tabella temporanea
      out_rec.totfasc := totfasc;
      out_rec.totfasca := totfasca;
      out_rec.totfascc := totfascc;
      out_rec.mese := mese_vc;
      out_rec.totfascm := totfascm;
      out_rec.totfascma := totfascma;
      out_rec.totfascmc := totfascmc;
      out_rec.totpercfasca := totpercfasca;
      out_rec.totpercfascc := totpercfascc;
      PIPE ROW (out_rec);
--INSERT INTO [docsadm].[#REPORT_ANNUALE_FASCICOLI](TOTFASC,TOTFASCA,TOTFASCC,MESE,TOTFASCM,TOTFASCMA,TOTFASCMC,TOTPERCFASCA,TOTPERCFASCC)
--    VALUES (totFasc, totFascA, totFascC, MESE_VC, totFascM, totFascMA, totFascMC, totPercFascA, totPercFascC)

      --reset dei contatori
      contamese := contamese + 1;
      totfascm := 0;
      totfascma := 0;
      totfascmc := 0;
      totpercfasca := 0;
      totpercfascc := 0;
   END LOOP;

--fine ciclo
   RETURN;
EXCEPTION
   WHEN OTHERS
   THEN
      RETURN;
END fascicolitablefunction; 

/
