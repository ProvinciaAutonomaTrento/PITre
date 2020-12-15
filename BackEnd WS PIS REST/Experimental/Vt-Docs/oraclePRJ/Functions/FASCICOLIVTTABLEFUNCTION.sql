--------------------------------------------------------
--  DDL for Function FASCICOLIVTTABLEFUNCTION
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."FASCICOLIVTTABLEFUNCTION" (
   id_amm        NUMBER,
   id_registro   NUMBER,
   anno          NUMBER,
   mese          NUMBER,
   tit           NUMBER
)
   RETURN fascicolivttablerow PIPELINED
IS
   p_id_amm                NUMBER               := id_amm;
   p_id_registro           NUMBER               := id_registro;
   p_anno                  NUMBER               := anno;
   p_mese                  NUMBER               := mese;
   p_tit                   NUMBER               := tit;
   out_rec                 fascicolivttabletype
      := fascicolivttabletype (NULL,
                               NULL,
                               NULL,
                               NULL,
                               NULL,
                               NULL,
                               NULL,
                               NULL,
                               NULL,
                               NULL,
                               NULL,
                               NULL,
                               NULL,
                               NULL,
                               NULL,
                               NULL,
                               NULL
                              );
   system_id_vt            NUMBER;
   description_vt          VARCHAR (255);
   var_codice_vt           VARCHAR (255);
   m_fasc_creati           NUMBER;
   fasc_chiusi             NUMBER;
   fasc_creati             NUMBER;
   m_fasc_chiusi           NUMBER;
   i                       NUMBER;
   mese_vc                 NUMBER;
   mese_vc_c               NUMBER;
   gennaio                 NUMBER;
   febbraio                NUMBER;
   marzo                   NUMBER;
   aprile                  NUMBER;
   maggio                  NUMBER;
   giugno                  NUMBER;
   luglio                  NUMBER;
   agosto                  NUMBER;
   settembre               NUMBER;
   ottobre                 NUMBER;
   novembre                NUMBER;
   dicembre                NUMBER;
   vt_fasc_creati          NUMBER;
   vt_fasc_chiusi          NUMBER;
   num_livello1            VARCHAR (255);
   var_codice_livello1     VARCHAR (255);
   description__livello1   VARCHAR (255);
   system_id_fasc          NUMBER;
   gennaio_c               NUMBER;
   febbraio_c              NUMBER;
   marzo_c                 NUMBER;
   aprile_c                NUMBER;
   maggio_c                NUMBER;
   giugno_c                NUMBER;
   luglio_c                NUMBER;
   agosto_c                NUMBER;
   settembre_c             NUMBER;
   ottobre_c               NUMBER;
   novembre_c              NUMBER;
   dicembre_c              NUMBER;
   tot_gennaio             VARCHAR (255);
   tot_febbraio            VARCHAR (255);
   tot_marzo               VARCHAR (255);
   tot_aprile              VARCHAR (255);
   tot_maggio              VARCHAR (255);
   tot_giugno              VARCHAR (255);
   tot_luglio              VARCHAR (255);
   tot_agosto              VARCHAR (255);
   tot_settembre           VARCHAR (255);
   tot_ottobre             VARCHAR (255);
   tot_novembre            VARCHAR (255);
   tot_dicembre            VARCHAR (255);
   tot_vt_t                VARCHAR (255);
   tot_vt                  NUMBER;
   tot_vt_c                NUMBER;
   t_gennaio               NUMBER;
   t_febbraio              NUMBER;
   t_marzo                 NUMBER;
   t_aprile                NUMBER;
   t_maggio                NUMBER;
   t_giugno                NUMBER;
   t_luglio                NUMBER;
   t_agosto                NUMBER;
   t_settembre             NUMBER;
   t_ottobre               NUMBER;
   t_novembre              NUMBER;
   t_dicembre              NUMBER;
   t_gennaio_c             NUMBER;
   t_febbraio_c            NUMBER;
   t_marzo_c               NUMBER;
   t_aprile_c              NUMBER;
   t_maggio_c              NUMBER;
   t_giugno_c              NUMBER;
   t_luglio_c              NUMBER;
   t_agosto_c              NUMBER;
   t_settembre_c           NUMBER;
   t_ottobre_c             NUMBER;
   t_novembre_c            NUMBER;
   t_dicembre_c            NUMBER;

   CURSOR c_vocitit
   IS
--solo VOCI di TITOLARIO di PRIMO LIVELLO, come richiesto dal report COMPATTO che richiede il conteggio aggregato sulle voci di 1? livello
      SELECT project.system_id, project.description, project.var_codice,
             project.num_livello
        FROM project
       WHERE project.var_codice IS NOT NULL
         AND project.cha_tipo_proj = 'T'
         AND project.num_livello = 1
         AND project.id_amm = p_id_amm
         AND (   project.id_registro = p_id_registro
              OR project.id_registro IS NULL
             )
         AND project.id_titolario = p_tit
                                         /* prima era calcolato, filtrando su titolario attivo
                                                (SELECT system_id
                                                   FROM project
                                                  WHERE cha_stato = 'A'
                                                    AND var_codice = 'T'
                                                    AND id_amm = p_id_amm
                                                    AND id_titolario = 0)
                                           */
    order by var_cod_liv1;

   CURSOR c_vocitit_notit
   IS
--solo VOCI di TITOLARIO di PRIMO LIVELLO, come richiesto dal report COMPATTO che richiede il conteggio aggregato sulle voci di 1? livello
      SELECT project.system_id, project.description, project.var_codice,
             project.num_livello
        FROM project
       WHERE project.var_codice IS NOT NULL
         AND project.cha_tipo_proj = 'T'
         AND project.num_livello = 1
         AND project.id_amm = p_id_amm
         AND (   project.id_registro = p_id_registro
              OR project.id_registro IS NULL
             )
             order by var_cod_liv1;
BEGIN
   mese_vc := 0;
   mese_vc_c := 0;
   i := 1;
   gennaio := 0;
   febbraio := 0;
   marzo := 0;
   aprile := 0;
   maggio := 0;
   giugno := 0;
   luglio := 0;
   agosto := 0;
   settembre := 0;
   ottobre := 0;
   novembre := 0;
   dicembre := 0;
   gennaio_c := 0;
   febbraio_c := 0;
   marzo_c := 0;
   aprile_c := 0;
   maggio_c := 0;
   giugno_c := 0;
   luglio_c := 0;
   agosto_c := 0;
   settembre_c := 0;
   ottobre_c := 0;
   novembre_c := 0;
   dicembre_c := 0;
   t_gennaio := 0;
   t_febbraio := 0;
   t_marzo := 0;
   t_aprile := 0;
   t_maggio := 0;
   t_giugno := 0;
   t_luglio := 0;
   t_agosto := 0;
   t_settembre := 0;
   t_ottobre := 0;
   t_novembre := 0;
   t_dicembre := 0;
   t_gennaio_c := 0;
   t_febbraio_c := 0;
   t_marzo_c := 0;
   t_aprile_c := 0;
   t_maggio_c := 0;
   t_giugno_c := 0;
   t_luglio_c := 0;
   t_agosto_c := 0;
   t_settembre_c := 0;
   t_ottobre_c := 0;
   t_novembre_c := 0;
   t_dicembre_c := 0;
   tot_vt_t := '0/0';
   tot_vt := 0;
   tot_vt_c := 0;

/*******************************************************************/

   /*******************************RECUPERO DEI DATI GENERALI************************************************************************************
FASC_CREATI:  Tutti i fascicoli con:
- anno di creazione pari all'anno di interesse,
- id_registro pari a qullo selezionato
- id_amm pari a quello selezionato

FASC_GEN_CREATI: Tutti i fascicoli generali:
- id_registro pari a null
- id_amm pari a quello selezionato

FASC_CHIUSI: Tutti i fascicoli chiusi:
- id_registro pari a qullo selezionato
- id_amm pari a quello selezionato
- anno della dati di chiusura pari a quello selezionato
*/
/*QUERY PER IL RECUPERO DEI DATI GENERALI */
   IF tit <> 0
   THEN
      SELECT COUNT (project.system_id)
        INTO fasc_creati
        FROM project
       WHERE project.cha_tipo_proj = 'F'
         AND TO_NUMBER (TO_CHAR (project.dta_creazione, 'YYYY')) = p_anno
         AND (   project.id_registro = p_id_registro
              OR project.id_registro IS NULL
             )
         AND project.id_amm = p_id_amm
-- FILTRO SU TITOLARIO
         AND project.id_titolario = p_tit
         AND project.cha_stato = 'A';

      SELECT COUNT (project.system_id)
        INTO fasc_chiusi
        FROM project
       WHERE project.cha_tipo_proj = 'F'
         AND (project.id_registro = p_id_registro
              OR project.id_registro = NULL
             )
         AND project.id_amm = p_id_amm
         AND TO_NUMBER (TO_CHAR (project.dta_creazione, 'YYYY')) = p_anno
-- commentata condizione su dta_chiusura perche vale la condizione sullo stato
         --AND TO_NUMBER (TO_CHAR (project.dta_chiusura, 'YYYY')) = p_anno
-- FILTRO SU TITOLARIO
         AND project.id_titolario = tit
         AND project.cha_stato = 'C';
   ELSE
      SELECT COUNT (project.system_id)
        INTO fasc_creati
        FROM project
       WHERE project.cha_tipo_proj = 'F'
         AND TO_NUMBER (TO_CHAR (project.dta_creazione, 'YYYY')) = p_anno
         AND (   project.id_registro = p_id_registro
              OR project.id_registro IS NULL
             )
         AND project.id_amm = p_id_amm
         AND project.cha_stato = 'A';

      SELECT COUNT (project.system_id)
        INTO fasc_chiusi
        FROM project
       WHERE project.cha_tipo_proj = 'F'
         AND (project.id_registro = p_id_registro
              OR project.id_registro = NULL
             )
         AND project.id_amm = p_id_amm
         AND TO_NUMBER (TO_CHAR (project.dta_creazione, 'YYYY')) = p_anno
-- commentata condizione su dta_chiusura perche vale la condizione sullo stato
         --AND TO_NUMBER (TO_CHAR (project.dta_chiusura, 'YYYY')) = p_anno
         AND project.cha_stato = 'C';
   END IF;

/*****************************************************************************************************************************************************/

   /*1 QUERY- Recupera l'elenco delle voci di titolario  (input : @id_amm) */
/*-- contiene tutte le voci di titolario (TIPO "T")*/

   /*Apertura del cursore*/
   
IF tit <> 0 
then    
   OPEN c_vocitit;                                     --(id_amm, id_registro)
   ELSE 
   OPEN c_vocitit_notit;                                     --(id_amm, id_registro)
   END IF; 

   LOOP
IF tit <> 0 
then    
      FETCH c_vocitit
       INTO system_id_vt, description_vt, var_codice_vt, num_livello1;

      EXIT WHEN c_vocitit%NOTFOUND;
ELSE 
      FETCH c_vocitit_notit
       INTO system_id_vt, description_vt, var_codice_vt, num_livello1;

      EXIT WHEN c_vocitit_notit%NOTFOUND;
END IF;
      var_codice_livello1 := var_codice_vt;
      
      description__livello1 := description_vt;

IF tit <> 0 
then    
-- conteggio fascicoli creati sulla voce titolario corrente
      SELECT     COUNT (project.system_id)
            INTO vt_fasc_creati
            FROM project
           WHERE TO_NUMBER (TO_CHAR (project.dta_creazione, 'YYYY')) = p_anno
-- FILTRO SU TITOLARIO
             AND project.id_titolario = tit
             AND project.cha_tipo_proj = 'F'
             AND project.id_amm = p_id_amm
-- condizione su ID_PARENT, che va bene per il REPORT ESTESO, qui e stato sostituito da CONNECT BY
--            AND project.id_parent = system_id_vt
             AND (   project.id_registro = p_id_registro
                  OR project.id_registro IS NULL
                 )
             AND project.cha_stato = 'A'
      CONNECT BY PRIOR project.system_id = project.id_parent
      -- FILTRO SU UNA VOCE DI PRIMO LIVELLO
      START WITH project.system_id = system_id_vt;

-- conteggio fascicoli chiusi sulla voce titolario corrente
      SELECT COUNT (project.system_id)
        INTO vt_fasc_chiusi
        FROM project
       WHERE project.cha_tipo_proj = 'F'
-- NUOVO FILTRO SU TITOLARIO
         AND project.id_titolario = tit
         AND project.id_amm = p_id_amm
         AND (   project.id_registro = p_id_registro
              OR project.id_registro IS NULL
             )
         AND TO_NUMBER (TO_CHAR (project.dta_creazione, 'YYYY')) = p_anno
-- commentata condizione su dta_chiusura perche vale la condizione sullo stato
         --AND TO_NUMBER (TO_CHAR (project.dta_chiusura, 'YYYY')) = p_anno
         AND project.cha_stato = 'C'
      CONNECT BY PRIOR project.system_id = project.id_parent
      -- FILTRO SU UNA VOCE DI PRIMO LIVELLO
      START WITH project.system_id = system_id_vt;
ELSE 
-- conteggio fascicoli creati sulla voce titolario corrente
      SELECT     COUNT (project.system_id)
            INTO vt_fasc_creati
            FROM project
           WHERE TO_NUMBER (TO_CHAR (project.dta_creazione, 'YYYY')) = p_anno
             AND project.cha_tipo_proj = 'F'
             AND project.id_amm = p_id_amm
-- condizione su ID_PARENT, che va bene per il REPORT ESTESO, qui e stato sostituito da CONNECT BY
--            AND project.id_parent = system_id_vt
             AND (   project.id_registro = p_id_registro
                  OR project.id_registro IS NULL
                 )
             AND project.cha_stato = 'A'
      CONNECT BY PRIOR project.system_id = project.id_parent
      -- FILTRO SU UNA VOCE DI PRIMO LIVELLO
      START WITH project.system_id = system_id_vt;

-- conteggio fascicoli chiusi sulla voce titolario corrente
      SELECT COUNT (project.system_id)
        INTO vt_fasc_chiusi
        FROM project
       WHERE project.cha_tipo_proj = 'F'
         AND project.id_amm = p_id_amm
         AND (   project.id_registro = p_id_registro
              OR project.id_registro IS NULL
             )
         AND TO_NUMBER (TO_CHAR (project.dta_creazione, 'YYYY')) = p_anno
-- commentata condizione su dta_chiusura perche vale la condizione sullo stato
         --AND TO_NUMBER (TO_CHAR (project.dta_chiusura, 'YYYY')) = p_anno
         AND project.cha_stato = 'C'
      CONNECT BY PRIOR project.system_id = project.id_parent
      -- FILTRO SU UNA VOCE DI PRIMO LIVELLO
      START WITH project.system_id = system_id_vt;
END IF;


      mese_vc := vt_fasc_creati;
      mese_vc_c := vt_fasc_chiusi;

/*2 QUERY- selezione dei fascicoli dellla relativa voce di titolario- (input @id_amm)*/
      WHILE (i <= mese)
      LOOP
         -- eseguo lo stesso conteggio di prima, solo disaggregato per mese
         SELECT     COUNT (project.system_id)
               INTO m_fasc_creati
               FROM project
              WHERE project.dta_creazione BETWEEN TO_DATE ('01/01/' || p_anno,
                                                           'DD/MM/YYYY'
                                                          )
                                              AND TO_DATE ('31/12/' || p_anno,
                                                           'DD/MM/YYYY'
                                                          )
                -- FILTRO SU TITOLARIO
                AND project.id_titolario = tit
                AND project.cha_tipo_proj = 'F'
                AND project.id_amm = p_id_amm
                AND (   project.id_registro = p_id_registro
                     OR project.id_registro IS NULL
                    )
                AND project.cha_stato = 'A'
-- modifica per ticket SchonAut # 1399733, incongruenza su disaggregazione sul mese, prima il campo era dta_apertura
                AND TO_NUMBER (TO_CHAR (project.dta_creazione, 'MM')) = i
         CONNECT BY PRIOR project.system_id = project.id_parent
         -- FILTRO SU UNA VOCE DI PRIMO LIVELLO
         START WITH project.system_id = system_id_vt;

         SELECT     COUNT (project.system_id)
               INTO m_fasc_chiusi
               FROM project
              WHERE TO_NUMBER (TO_CHAR (project.dta_creazione, 'YYYY')) = p_anno
-- commentata condizione su dta_chiusura perche vale la condizione sullo stato
         --AND TO_NUMBER (TO_CHAR (project.dta_chiusura, 'YYYY')) = p_anno

           -- FILTRO SU TITOLARIO
                AND project.id_titolario = tit
                AND project.cha_tipo_proj = 'F'
                AND project.cha_stato = 'C'
                AND project.id_amm = p_id_amm
                AND (   project.id_registro = p_id_registro
                     OR project.id_registro IS NULL
                    )
-- modifica per ticket SchonAut # 1399733, incongruenza su disaggregazione sul mese, prima il campo era dta_apertura
                AND TO_NUMBER (TO_CHAR (project.dta_creazione, 'MM')) = i
         CONNECT BY PRIOR project.system_id = project.id_parent
         -- FILTRO SU UNA VOCE DI PRIMO LIVELLO
         START WITH project.system_id = system_id_vt;

/*INSERIMENTO DEL VALORE MENSILE NELLA TABELLA TEMPORANEA*/
/*A seconda del mese avremo una query di inserimento diversa, per popolare il giusto campo della tabella*/
         IF (i = 1)
         THEN
/*Gennaio*/
            IF (NVL (m_fasc_creati, 0) > 0 OR NVL (m_fasc_chiusi, 0) > 0)
            THEN
               gennaio := m_fasc_creati;
               gennaio_c := m_fasc_chiusi;
            ELSE
               gennaio := 0;
               gennaio_c := 0;
            END IF;
         END IF;

         IF (i = 2)
         THEN
/*Febbraio*/
            IF (NVL (m_fasc_creati, 0) > 0 OR NVL (m_fasc_chiusi, 0) > 0)
            THEN
               febbraio := m_fasc_creati;
               febbraio_c := m_fasc_chiusi;
            ELSE
               febbraio := 0;
               febbraio_c := 0;
            END IF;
         END IF;

         IF (i = 3)
         THEN
/*Marzo*/
            IF (NVL (m_fasc_creati, 0) > 0 OR NVL (m_fasc_chiusi, 0) > 0)
            THEN
               marzo := m_fasc_creati;
               marzo_c := m_fasc_chiusi;
            ELSE
               marzo := 0;
               marzo_c := 0;
            END IF;
         END IF;

         IF (i = 4)
         THEN
/*Aprile*/
            IF (NVL (m_fasc_creati, 0) > 0 OR NVL (m_fasc_chiusi, 0) > 0)
            THEN
               aprile := m_fasc_creati;
               aprile_c := m_fasc_chiusi;
            ELSE
               aprile := 0;
               aprile_c := 0;
            END IF;
         END IF;

         IF (i = 5)
         THEN
/*MAggio*/
            IF (NVL (m_fasc_creati, 0) > 0 OR NVL (m_fasc_chiusi, 0) > 0)
            THEN
               maggio := m_fasc_creati;
               maggio_c := m_fasc_chiusi;
            ELSE
               maggio := 0;
               maggio_c := 0;
            END IF;
         END IF;

         IF (i = 6)
         THEN
/*Giugno*/
            IF (NVL (m_fasc_creati, 0) > 0 OR NVL (m_fasc_chiusi, 0) > 0)
            THEN
               giugno := m_fasc_creati;
               giugno_c := m_fasc_chiusi;
            ELSE
               giugno := 0;
               giugno_c := 0;
            END IF;
         END IF;

         IF (i = 7)
         THEN
/*Luglio*/
            IF (NVL (m_fasc_creati, 0) > 0 OR NVL (m_fasc_chiusi, 0) > 0)
            THEN
               luglio := m_fasc_creati;
               luglio_c := m_fasc_chiusi;
            ELSE
               luglio := 0;
               luglio_c := 0;
            END IF;
         END IF;

         IF (i = 8)
         THEN
/*Agosto*/
            IF (NVL (m_fasc_creati, 0) > 0 OR NVL (m_fasc_chiusi, 0) > 0)
            THEN
               agosto := m_fasc_creati;
               agosto_c := m_fasc_chiusi;
            ELSE
               agosto := 0;
               agosto_c := 0;
            END IF;
         END IF;

         IF (i = 9)
         THEN
/*Settembre*/
            IF (NVL (m_fasc_creati, 0) > 0 OR NVL (m_fasc_chiusi, 0) > 0)
            THEN
               settembre := m_fasc_creati;
               settembre_c := m_fasc_chiusi;
            ELSE
               settembre := 0;
               settembre_c := 0;
            END IF;
         END IF;

         IF (i = 10)
         THEN
/*Ottobre*/
            IF (NVL (m_fasc_creati, 0) > 0 OR NVL (m_fasc_chiusi, 0) > 0)
            THEN
               ottobre := m_fasc_creati;
               ottobre_c := m_fasc_chiusi;
            ELSE
               ottobre := 0;
               ottobre_c := 0;
            END IF;
         END IF;

         IF (i = 11)
         THEN
/*Novembre*/
            IF (NVL (m_fasc_creati, 0) > 0 OR NVL (m_fasc_chiusi, 0) > 0)
            THEN
               novembre := m_fasc_creati;
               novembre_c := m_fasc_chiusi;
            ELSE
               novembre := 0;
               novembre_c := 0;
            END IF;
         END IF;

         IF (i = 12)
         THEN
/*Dicembre*/
            IF (NVL (m_fasc_creati, 0) > 0 OR NVL (m_fasc_chiusi, 0) > 0)
            THEN
               dicembre := m_fasc_creati;
               dicembre_c := m_fasc_chiusi;
            ELSE
               dicembre := 0;
               dicembre_c := 0;
            END IF;
         END IF;

/*AGGIORNAMENTO DEL CONTATORE*/
         i := i + 1;
      END LOOP;

-- e il totale cumulativo sulle voci di titolario
      t_gennaio := gennaio;
      --t_gennaio := t_gennaio + gennaio;
      t_febbraio := febbraio;
      --t_febbraio := t_febbraio + febbraio;

      --t_marzo := t_marzo + marzo;
      t_marzo := marzo;
      --t_aprile := t_aprile + aprile;
      t_aprile := aprile;
      t_maggio := t_maggio + maggio;
      --t_giugno := t_giugno + giugno;
      t_giugno := giugno;
      --t_luglio := t_luglio + luglio;
      t_luglio := luglio;
      --t_agosto := t_agosto + agosto;
      t_agosto := agosto;
      --t_settembre := t_settembre + settembre;
      t_settembre := settembre;
      --t_ottobre := t_ottobre + ottobre;
      t_ottobre := ottobre;
      --t_novembre := t_novembre + novembre;
      t_novembre := novembre;
      --t_dicembre := t_dicembre + dicembre;
      t_dicembre := dicembre;
      t_gennaio_c := t_gennaio_c + gennaio_c;
      t_febbraio_c := t_febbraio_c + febbraio_c;
      t_marzo_c := t_marzo_c + marzo_c;
      t_aprile_c := t_aprile_c + aprile_c;
      t_maggio_c := t_maggio_c + maggio_c;
      t_giugno_c := t_giugno_c + giugno_c;
      t_luglio_c := t_luglio_c + luglio_c;
      t_agosto_c := t_agosto_c + agosto_c;
      t_settembre_c := t_settembre_c + settembre_c;
      t_ottobre_c := t_ottobre_c + ottobre_c;
      t_novembre_c := t_novembre_c + novembre_c;
      t_dicembre_c := t_dicembre_c + dicembre_c;
      --tot_vt := tot_vt + mese_vc;
      tot_vt := mese_vc;
      --tot_vt_c := mese_vc_c ;
      tot_vt_c :=
           t_gennaio_c
         + t_febbraio_c
         + t_marzo_c
         + t_aprile_c
         + t_maggio_c
         + t_giugno_c
         + t_luglio_c
         + t_agosto_c
         + t_settembre_c
         + t_ottobre_c
         + t_novembre_c
         + t_dicembre_c;
      mese_vc_c := 0;
      mese_vc := 0;

      IF ((tot_vt = 0) AND (tot_vt_c = 0))
      THEN
         tot_vt_t := '-';
      ELSE
         tot_vt_t := tot_vt || '/' || tot_vt_c;
      END IF;

      IF ((t_gennaio = 0) AND (t_gennaio_c = 0))
      THEN
         tot_gennaio := '-';
      ELSE
         tot_gennaio := t_gennaio || '/' || t_gennaio_c;
      END IF;

      IF ((t_febbraio = 0) AND (t_febbraio_c = 0))
      THEN
         tot_febbraio := '-';
      ELSE
         tot_febbraio := t_febbraio || '/' || t_febbraio_c;
      END IF;

      IF ((t_marzo = 0) AND (t_marzo_c = 0))
      THEN
         tot_marzo := '-';
      ELSE
         tot_marzo := t_marzo || '/' || t_marzo_c;
      END IF;

      IF ((t_aprile = 0) AND (t_aprile_c = 0))
      THEN
         tot_aprile := '-';
      ELSE
         tot_aprile := t_aprile || '/' || t_aprile_c;
      END IF;

      IF ((t_maggio = 0) AND (t_maggio_c = 0))
      THEN
         tot_maggio := '-';
      ELSE
         tot_maggio := t_maggio || '/' || t_maggio_c;
      END IF;

      IF ((t_giugno = 0) AND (t_giugno_c = 0))
      THEN
         tot_giugno := '-';
      ELSE
         tot_giugno := t_giugno || '/' || t_giugno_c;
      END IF;

      IF ((t_luglio = 0) AND (t_luglio_c = 0))
      THEN
         tot_luglio := '-';
      ELSE
         tot_luglio := t_luglio || '/' || t_luglio_c;
      END IF;

      IF ((t_agosto = 0) AND (t_agosto_c = 0))
      THEN
         tot_agosto := '-';
      ELSE
         tot_agosto := t_agosto || '/' || t_agosto_c;
      END IF;

      IF ((t_settembre = 0) AND (t_settembre_c = 0))
      THEN
         tot_settembre := '-';
      ELSE
         tot_settembre := t_settembre || '/' || t_settembre_c;
      END IF;

      IF ((t_ottobre = 0) AND (t_ottobre_c = 0))
      THEN
         tot_ottobre := '-';
      ELSE
         tot_ottobre := t_ottobre || '/' || t_ottobre_c;
      END IF;

      IF ((t_novembre = 0) AND (t_novembre_c = 0))
      THEN
         tot_novembre := '-';
      ELSE
         tot_novembre := t_novembre || '/' || t_novembre_c;
      END IF;

      IF ((t_dicembre = 0) AND (t_dicembre_c = 0))
      THEN
         tot_dicembre := '-';
      ELSE
         tot_dicembre := t_dicembre || '/' || t_dicembre_c;
      END IF;

/*Aggiorniamo il cursore delle voci di Titolario*/
      i := 1;
      --IF (num_livello1 = 1) THEN
      out_rec.fasc_creati := fasc_creati;
      out_rec.fasc_chiusi := fasc_chiusi;
      out_rec.var_cod := var_codice_livello1;
      out_rec.var_descr := description__livello1;
      out_rec.gennaio := tot_gennaio;
      out_rec.febbraio := tot_febbraio;
      out_rec.marzo := tot_marzo;
      out_rec.aprile := tot_aprile;
      out_rec.maggio := tot_maggio;
      out_rec.giugno := tot_giugno;
      out_rec.luglio := tot_luglio;
      out_rec.agosto := tot_agosto;
      out_rec.settembre := tot_settembre;
      out_rec.ottobre := tot_ottobre;
      out_rec.novembre := tot_novembre;
      out_rec.dicembre := tot_dicembre;
      out_rec.vt_fac_creati := tot_vt_t;
      PIPE ROW (out_rec);
--END IF;
      tot_vt := 0;
      tot_vt_c := 0;
      gennaio := 0;
      febbraio := 0;
      marzo := 0;
      aprile := 0;
      maggio := 0;
      giugno := 0;
      luglio := 0;
      agosto := 0;
      settembre := 0;
      ottobre := 0;
      novembre := 0;
      dicembre := 0;
      t_gennaio := 0;
      t_febbraio := 0;
      t_marzo := 0;
      t_aprile := 0;
      t_maggio := 0;
      t_giugno := 0;
      t_luglio := 0;
      t_agosto := 0;
      t_settembre := 0;
      t_ottobre := 0;
      t_novembre := 0;
      t_dicembre := 0;
      t_gennaio_c := 0;
      t_febbraio_c := 0;
      t_marzo_c := 0;
      t_aprile_c := 0;
      t_maggio_c := 0;
      t_giugno_c := 0;
      t_luglio_c := 0;
      t_agosto_c := 0;
      t_settembre_c := 0;
      t_ottobre_c := 0;
      t_novembre_c := 0;
      t_dicembre_c := 0;
   END LOOP;

   CLOSE c_vocitit;

   RETURN;
EXCEPTION
   WHEN OTHERS
   THEN
      DBMS_OUTPUT.put_line (SQLERRM);
      RAISE;
--RETURN ;
END fascicolivttablefunction; 

/
