
Begin 
	Utl_Backup_Plsql_Code    ( 'FUNCTION', 'INTEROPTABLEFUNCTION');
end;
/


CREATE OR REPLACE FUNCTION INTEROPTABLEFUNCTION (
   anno          NUMBER,
   id_registro   NUMBER,
   idAmm         NUMBER
)
   RETURN interoptablerow PIPELINED
IS
--inizializzazione
   out_rec_amm      interoptabletype
      := interoptabletype (NULL,
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
   out_rec_anno     interoptabletype
      := interoptabletype (NULL,
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
-- variabili del cursore
   var_codice_amm   VARCHAR (255);
   var_codice_aoo   VARCHAR (255);
   doc_spediti      NUMBER;
   mese             NUMBER;

--Dichiarazione del cursore
   CURSOR c_data (a NUMBER, reg NUMBER)
   Is
      /* Select Distinct (Count (*)), To_Number (To_Char (Dta_Spedizione, 'MM')),
                      A.Var_Desc_Amm As Var_Codice_Amm, Var_Codice_Aoo
                 From Profile P,  Dpa_Stato_Invio Si , Dpa_Amministra A  
                 Where Si.Var_Codice_Amm = A.Var_Codice_Amm 
                 and cha_tipo_proto = 'P'
                  AND id_registro = reg
                  and nvl(p.CHA_IN_CESTINO,'0') = '0'
                  AND p.system_id = si.id_profile
                  And To_Number (To_Char (Dta_Spedizione, 'YYYY')) = A
                  AND si.var_codice_amm IS NOT NULL
                  And Upper (Var_Codice_Aoo) <> Upper (Getregdescr (Reg))
             GROUP BY ROLLUP ( A.Var_Desc_Amm, var_codice_aoo,
                              TO_NUMBER (TO_CHAR (dta_spedizione, 'MM')))
             ORDER BY  A.Var_Desc_Amm,
                      Var_Codice_Aoo,
                      To_Number (To_Char (Dta_Spedizione, 'MM'));*/

    Select /*+ PARALLEL (P,8)*/
Distinct  Count (*) As Conteggio, To_Number (To_Char (Dta_Spedizione, 'MM')) --As Mese_Spedizione --,D.Description
    , A.Var_Desc_Amm As Var_Codice_Amm, cg.VAR_DESC_CORR
           From Profile P
           , Dpa_Stato_Invio Si
           , Dpa_Amministra A
           , Documenttypes D
           , dpa_t_canale_corr tc
           , dpa_corr_globali cg
          WHERE cha_tipo_proto = 'P'
            AND tc.id_documenttype = d.system_id
            And Tc.Id_Corr_Globale = Si.Id_Corr_Globale
            And P.Id_Registro = reg
            And To_Number (To_Char (Dta_Spedizione, 'YYYY')) = A
            and d.type_id in ('INTEROPERABILITA','SIMPLIFIEDINTEROPERABILITY','MAIL')
            And Nvl (P.Cha_In_Cestino, '0') = '0'
            AND p.system_id = si.id_profile
            And Dta_Spedizione Is Not Null
            and cg.SYSTEM_ID = Si.ID_CORR_GLOBALE
            and A.SYSTEM_ID = idAmm
       GROUP BY ROLLUP ( A.Var_Desc_Amm, cg.VAR_DESC_CORR,
                              TO_NUMBER (TO_CHAR (dta_spedizione, 'MM')))
             ORDER BY  A.Var_Desc_Amm,
                      cg.VAR_DESC_CORR,
                      To_Number (To_Char (Dta_Spedizione, 'MM'))         ;             


BEGIN
   OPEN c_data (anno, id_registro);

--set iniziale variabili
   out_rec_amm.gennaio := 0;
   out_rec_amm.febbraio := 0;
   out_rec_amm.marzo := 0;
   out_rec_amm.aprile := 0;
   out_rec_amm.maggio := 0;
   out_rec_amm.giugno := 0;
   out_rec_amm.luglio := 0;
   out_rec_amm.agosto := 0;
   out_rec_amm.settembre := 0;
   out_rec_amm.ottobre := 0;
   out_rec_amm.novembre := 0;
   out_rec_amm.dicembre := 0;

   LOOP
      FETCH c_data
       INTO doc_spediti, mese, var_codice_amm, var_codice_aoo;

      EXIT WHEN c_data%NOTFOUND;

      IF (    (doc_spediti <> 0)
          AND (mese <> 0)
          AND (var_codice_amm IS NOT NULL)
          AND (var_codice_aoo IS NOT NULL)
         )
      THEN
         out_rec_amm.var_cod_amm := var_codice_amm;
         out_rec_amm.var_cod_aoo := var_codice_aoo;

         IF (mese = 1)
         THEN
            out_rec_amm.gennaio := doc_spediti;
         END IF;

         IF (mese = 2)
         THEN
            out_rec_amm.febbraio := doc_spediti;
         END IF;

         IF (mese = 3)
         THEN
            out_rec_amm.marzo := doc_spediti;
         END IF;

         IF (mese = 4)
         THEN
            out_rec_amm.aprile := doc_spediti;
         END IF;

         IF (mese = 5)
         THEN
            out_rec_amm.maggio := doc_spediti;
         END IF;

         IF (mese = 6)
         THEN
            out_rec_amm.giugno := doc_spediti;
         END IF;

         IF (mese = 7)
         THEN
            out_rec_amm.luglio := doc_spediti;
         END IF;

         IF (mese = 8)
         THEN
            out_rec_amm.agosto := doc_spediti;
         END IF;

         IF (mese = 9)
         THEN
            out_rec_amm.settembre := doc_spediti;
         END IF;

         IF (mese = 10)
         THEN
            out_rec_amm.ottobre := doc_spediti;
         END IF;

         IF (mese = 11)
         THEN
            out_rec_amm.novembre := doc_spediti;
         END IF;

         IF (mese = 12)
         THEN
            out_rec_amm.dicembre := doc_spediti;
         END IF;
      END IF;

-- TOTALE PARZIALE
      IF (    (doc_spediti <> 0)
          AND (mese IS NULL)
          AND (var_codice_amm IS NOT NULL)
          AND (var_codice_aoo IS NOT NULL)
         )
      THEN
         out_rec_amm.tot_m_sped := doc_spediti;
         out_rec_amm.tot_sped := 0;
         PIPE ROW (out_rec_amm);
         out_rec_amm.gennaio := 0;
         out_rec_amm.febbraio := 0;
         out_rec_amm.marzo := 0;
         out_rec_amm.aprile := 0;
         out_rec_amm.maggio := 0;
         out_rec_amm.giugno := 0;
         out_rec_amm.luglio := 0;
         out_rec_amm.agosto := 0;
         out_rec_amm.settembre := 0;
         out_rec_amm.ottobre := 0;
         out_rec_amm.novembre := 0;
         out_rec_amm.dicembre := 0;
      END IF;

-- TOTALE
      IF (    (doc_spediti <> 0)
          AND (mese IS NULL)
          AND (var_codice_amm IS NULL)
          AND (var_codice_aoo IS NULL)
         )
      THEN
-- INSERIMENTO DEL TOTALE FINALE
         out_rec_anno.var_cod_amm := '0';
         out_rec_anno.var_cod_aoo := '0';
         out_rec_anno.gennaio := '0';
         out_rec_anno.febbraio := '0';
         out_rec_anno.marzo := '0';
         out_rec_anno.aprile := '0';
         out_rec_anno.maggio := '0';
         out_rec_anno.giugno := '0';
         out_rec_anno.luglio := '0';
         out_rec_anno.agosto := '0';
         out_rec_anno.settembre := '0';
         out_rec_anno.ottobre := '0';
         out_rec_anno.novembre := '0';
         out_rec_anno.dicembre := '0';
         out_rec_anno.tot_m_sped := '0';
         out_rec_anno.tot_sped := doc_spediti;
         PIPE ROW (out_rec_anno);
      END IF;
   END LOOP;                                                --fine del cursore

   CLOSE c_data;

   RETURN;
EXCEPTION
   WHEN OTHERS
   THEN
      BEGIN
         RETURN;
      END;
END interoptablefunction;
/

