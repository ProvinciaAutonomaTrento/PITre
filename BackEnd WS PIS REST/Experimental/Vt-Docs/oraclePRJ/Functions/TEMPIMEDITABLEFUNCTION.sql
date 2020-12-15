--------------------------------------------------------
--  DDL for Function TEMPIMEDITABLEFUNCTION
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."TEMPIMEDITABLEFUNCTION" (
   p_id_amm        NUMBER,
   p_id_registro   NUMBER,
   anno            NUMBER,
   tit             INT
)
   RETURN tempimeditablerow PIPELINED
IS
--dichiarazione
   out_rec          tempimeditabletype
                                     := tempimeditabletype (NULL, NULL, NULL);
--DICHIARAZIONI VARIABILI
   codclass         VARCHAR (255);
   descclass        VARCHAR (255);
   contafasc        NUMBER;
   valore           FLOAT;
   tempomedio       FLOAT;
-- variabili ausiliarie per il cursore che recupera le voci di titolario
   system_id_vt     NUMBER;
   description_vt   VARCHAR (255);
   var_codice_vt    VARCHAR (255);
--variabili ausuliarie per il cursore dei fascicoli
   dta_creazione    DATE;
   dta_chiusura     DATE;
   intervallo       INT;
   tmp              FLOAT;
   tmp_a            NUMBER;
   tmp_c            NUMBER;

--Dichiarazione dei cursori
--Cursore per le voci di titolario
   CURSOR c_vocitit (amm NUMBER, reg NUMBER)
   IS
      SELECT   project.system_id, project.description, project.var_codice
          FROM project
         WHERE project.cha_tipo_proj = 'T'
           AND project.var_codice IS NOT NULL
           AND project.id_amm = amm
           AND (project.id_registro = reg OR project.id_registro IS NULL)
           AND project.id_titolario = tit
           and var_codice != 'T'
      ORDER BY project.var_cod_liv1;

-- contiene tutti i fascicoli (TIPO "F")
   CURSOR c_fascicoli (amm NUMBER, parentid NUMBER)
   IS
      SELECT project.dta_creazione, project.dta_chiusura
        FROM project
       WHERE project.cha_tipo_proj = 'F'
         AND project.id_amm = amm
         AND project.id_parent = parentid
         AND project.id_titolario = tit
         and var_codice != 'T';

   c1               c_fascicoli%ROWTYPE;

   CURSOR c_vocitit_notit (amm NUMBER, reg NUMBER)
   IS
      SELECT   project.system_id, project.description, project.var_codice
          FROM project
         WHERE project.cha_tipo_proj = 'T'
           AND project.var_codice IS NOT NULL
           AND project.id_amm = amm
           AND (project.id_registro = reg OR project.id_registro IS NULL)
           and var_codice != 'T'
      ORDER BY project.var_cod_liv1;

-- contiene tutti i fascicoli (TIPO "F")
   CURSOR c_fascicoli_notit (amm NUMBER, parentid NUMBER)
   IS
      SELECT project.dta_creazione, project.dta_chiusura
        FROM project
       WHERE project.cha_tipo_proj = 'F'
         AND project.id_amm = amm
         AND project.id_parent = parentid
         and var_codice != 'T';

   c1_notit         c_fascicoli_notit%ROWTYPE;
BEGIN
--SETTAGGIO INIZIALE VARIABILI
   contafasc := 0;
   tempomedio := 0;
   valore := 0;
   intervallo := 0;
   tmp := 0;

--1 QUERY- elenco voci di titolario  -- (input : @id_amm)
   IF tit <> 0
   THEN
      OPEN c_vocitit (p_id_amm, p_id_registro);
   ELSE
      OPEN c_vocitit_notit (p_id_amm, p_id_registro);
   END IF;

   LOOP
      IF tit <> 0
      THEN
         FETCH c_vocitit
          INTO system_id_vt, description_vt, var_codice_vt;

         EXIT WHEN c_vocitit%NOTFOUND;
      ELSE
         FETCH c_vocitit_notit
          INTO system_id_vt, description_vt, var_codice_vt;

         EXIT WHEN c_vocitit_notit%NOTFOUND;
      END IF;

-- IF tit <> 0 THEN   ELSE END IF ;
      IF tit <> 0
      THEN
--2 QUERY- selezione dei fascicoli dellla relativa voce di titolario- (input @id_amm)
         OPEN c_fascicoli (p_id_amm, system_id_vt);
      ELSE
         OPEN c_fascicoli_notit (p_id_amm, system_id_vt);
      END IF;

      LOOP
         IF tit <> 0
         THEN
            FETCH c_fascicoli             INTO c1;

            EXIT WHEN c_fascicoli%NOTFOUND;

            IF (    (c1.dta_creazione IS NOT NULL)
                AND (c1.dta_chiusura IS NOT NULL)
               )
            THEN
               contafasc := contafasc + 1;
               DBMS_OUTPUT.put_line (contafasc);
               tmp := c1.dta_chiusura - c1.dta_creazione;
               tmp := tmp + 1;
               tmp := ROUND (tmp);
               intervallo := intervallo + tmp;
            END IF;
         ELSE
            FETCH c_fascicoli_notit             INTO c1_notit;

            EXIT WHEN c_fascicoli_notit%NOTFOUND;

            IF (    (c1_notit.dta_creazione IS NOT NULL)
                AND (c1_notit.dta_chiusura IS NOT NULL)
               )
            THEN
               contafasc := contafasc + 1;
               DBMS_OUTPUT.put_line (contafasc);
               tmp := c1_notit.dta_chiusura - c1_notit.dta_creazione;
               tmp := tmp + 1;
               tmp := ROUND (tmp);
               intervallo := intervallo + tmp;
            END IF;
         END IF;

--conto le differenze parziali di tutti i fascicoli contenuti nella voce di titolario selezionata
----------------- end
      END LOOP;

--(FINE 2 ciclo)
      IF tit <> 0
      THEN
         CLOSE c_fascicoli;
      ELSE
         CLOSE c_fascicoli_notit;
      END IF;

--converto i valori trovati e calcolo il tempo di lavorazione medio di tutti i fascicoli della voce di titolario prescelta
      IF ((intervallo <> 0) AND (contafasc <> 0))
      THEN
         tempomedio := ROUND (intervallo / contafasc, 2);

         IF (tempomedio < 0)
         THEN
            tempomedio := 0;
         END IF;
      END IF;

-- INSERISCO I VALORI TROVATI NELLA TABELLA TEMPORANEA
      out_rec.cod_class := var_codice_vt;
      out_rec.desc_class := description_vt;
      out_rec.t_medio_lav := tempomedio;
      PIPE ROW (out_rec);
-- reset delle variabili di conteggio
      contafasc := 0;
      intervallo := 0;
      tempomedio := 0;
   END LOOP;

--(FINE 1 ciclo)
   IF tit <> 0
   THEN
      CLOSE c_vocitit;
   ELSE
      CLOSE c_vocitit_notit;
   END IF;

   RETURN;
EXCEPTION
   WHEN OTHERS
   THEN
     out_rec.cod_class := var_codice_vt;
      out_rec.desc_class := SQLERRM;
      out_rec.t_medio_lav := tempomedio;
      PIPE ROW (out_rec);
      RETURN;
END tempimeditablefunction; 

/
