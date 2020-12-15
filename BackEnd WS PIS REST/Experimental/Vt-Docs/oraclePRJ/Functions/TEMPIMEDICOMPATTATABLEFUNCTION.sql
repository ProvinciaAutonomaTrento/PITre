--------------------------------------------------------
--  DDL for Function TEMPIMEDICOMPATTATABLEFUNCTION
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."TEMPIMEDICOMPATTATABLEFUNCTION" (
   id_amm        NUMBER,
   id_registro   NUMBER,
   anno          NUMBER,
   tit           INT
)
   RETURN tempimeditablerow PIPELINED
IS
--dichiarazione
   out_rec                 tempimeditabletype
                                     := tempimeditabletype (NULL, NULL, NULL);
--DICHIARAZIONI VARIABILI
   codclass                VARCHAR (255);
   descclass               VARCHAR (255);
   contafasc               NUMBER;
   valore                  FLOAT;
   tempomedio              FLOAT;
-- variabili ausiliarie per il cursore che recupera le voci di titolario
   system_id_vt            NUMBER;
   description_vt          VARCHAR (255);
   var_codice_vt           VARCHAR (255);
--variabili ausuliarie per il cursore dei fascicoli
   dta_creazione           DATE;
   dta_chiusura            DATE;
   intervallo              INT;
   num_livello1            VARCHAR (255);
   var_codice_livello1     VARCHAR (255);
   description__livello1   VARCHAR (255);
   tot_vt                  NUMBER;
   contatore               NUMBER;
   tmp                     FLOAT;

--Dichiarazione dei cursori
--Cursore per le voci di titolario
   CURSOR c_vocitit (amm NUMBER, reg NUMBER)
   IS
      SELECT   project.system_id, project.description, project.var_codice,
               project.num_livello
          FROM project
         WHERE project.cha_tipo_proj = 'T'
           AND project.var_codice IS NOT NULL
           AND project.id_amm = amm
           AND (project.id_registro = reg OR project.id_registro IS NULL)
           AND project.id_titolario = tit
      ORDER BY project.var_cod_liv1;

-- contiene tutti i fascicoli (TIPO "F")
   CURSOR c_fascicoli (amm NUMBER, parentid NUMBER)
   IS
      SELECT project.dta_creazione, project.dta_chiusura
        FROM project
       WHERE project.cha_tipo_proj = 'F'
         AND project.id_amm = amm
         AND project.id_parent = parentid;

   c1                      c_fascicoli%ROWTYPE;

--Cursore per le voci di titolario
   CURSOR c_vocitit_notit (amm NUMBER, reg NUMBER)
   IS
      SELECT   project.system_id, project.description, project.var_codice,
               project.num_livello
          FROM project
         WHERE project.cha_tipo_proj = 'T'
           AND project.var_codice IS NOT NULL
           AND project.id_amm = amm
           AND (project.id_registro = reg OR project.id_registro IS NULL)
      ORDER BY project.var_cod_liv1;

-- contiene tutti i fascicoli (TIPO "F")
   CURSOR c_fascicoli_notit (amm NUMBER, parentid NUMBER)
   IS
      SELECT project.dta_creazione, project.dta_chiusura
        FROM project
       WHERE project.cha_tipo_proj = 'F'
         AND project.id_amm = amm
         AND project.id_parent = parentid;

   c1_notit                c_fascicoli_notit%ROWTYPE;
BEGIN
--SETTAGGIO INIZIALE VARIABILI
   contafasc := 0;
   tempomedio := 0;
   valore := 0;
   intervallo := 0;
   contatore := 0;
   tot_vt := 0;
   tmp := 0;

--1 QUERY- elenco voci di titolario  -- (input : @id_amm)
   IF tit <> 0
   THEN
      OPEN c_vocitit (id_amm, id_registro);
   ELSE
      OPEN c_vocitit_notit (id_amm, id_registro);
   END IF;

   LOOP
      IF tit <> 0
      THEN
         FETCH c_vocitit
          INTO system_id_vt, description_vt, var_codice_vt, num_livello1;

         EXIT WHEN c_vocitit%NOTFOUND;
      ELSE
         FETCH c_vocitit_notit
          INTO system_id_vt, description_vt, var_codice_vt, num_livello1;

         EXIT WHEN c_vocitit_notit%NOTFOUND;
      END IF;

      IF (num_livello1 = 1)
      THEN
         var_codice_livello1 := var_codice_vt;
         description__livello1 := description_vt;
         contatore := 0;
      END IF;

      contatore := contatore + 1;

--2 QUERY- selezione dei fascicoli dellla relativa voce di titolario- (input @id_amm)
 IF tit <> 0
   THEN
      OPEN c_fascicoli (id_amm, system_id_vt);
      ELSE 
      OPEN c_fascicoli_notit (id_amm, system_id_vt);
      END IF;

      LOOP
 IF tit <> 0
   THEN
         FETCH c_fascicoli
          INTO c1;

         EXIT WHEN c_fascicoli%NOTFOUND;
         IF ((c1.dta_creazione IS NOT NULL) AND (c1.dta_chiusura IS NOT NULL)
            )
         THEN
            contafasc := contafasc + 1;
            tmp := c1.dta_chiusura - c1.dta_creazione;
            tmp := tmp + 1;
            tmp := ROUND (tmp);
            intervallo := intervallo + tmp;
         END IF;
         ELSE 
FETCH c_fascicoli_notit
          INTO c1_notit;

         EXIT WHEN c_fascicoli_notit%NOTFOUND;
--conto le differenze parziali di tutti i fascicoli contenuti nella voce di titolario selezionata
         IF ((c1_notit.dta_creazione IS NOT NULL) AND (c1_notit.dta_chiusura IS NOT NULL)
            )
         THEN
            contafasc := contafasc + 1;
            tmp := c1_notit.dta_chiusura - c1_notit.dta_creazione;
            tmp := tmp + 1;
            tmp := ROUND (tmp);
            intervallo := intervallo + tmp;
         END IF;
         END IF;
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
         tot_vt := tot_vt + tempomedio;

         IF (tempomedio < 0)
         THEN
            tempomedio := 0;
            tot_vt := 0;
         END IF;
      END IF;

-- INSERISCO I VALORI TROVATI NELLA TABELLA TEMPORANEA
      IF (num_livello1 = 1)
      THEN
         out_rec.cod_class := var_codice_livello1;
         out_rec.desc_class := description__livello1;
         out_rec.t_medio_lav := tot_vt / contatore;
         PIPE ROW (out_rec);
      END IF;

-- reset delle variabili di conteggio
      contafasc := 0;
      intervallo := 0;
      tempomedio := 0;
      tot_vt := 0;
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
      RETURN;
END tempimedicompattatablefunction; 

/
