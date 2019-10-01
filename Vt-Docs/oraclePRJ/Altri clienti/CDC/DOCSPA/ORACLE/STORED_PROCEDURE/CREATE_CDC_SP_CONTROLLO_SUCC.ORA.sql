DROP PROCEDURE CDC_SP_CONTROLLO_SUCC;

CREATE OR REPLACE PROCEDURE         cdc_sp_controllo_succ (
   p_my_int_elabora_h   VARCHAR,
   p_inizio             VARCHAR,
   p_fine               VARCHAR,
   p_my_id_amm          VARCHAR,
   p_ufficio            VARCHAR,
   p_magistrato         VARCHAR,
   p_revisore           VARCHAR)
IS
   intervallo_elabora   NUMBER;
   inizio               date;
   fine                 date;

   inizio_com               VARCHAR2 (20);
   fine_com                 VARCHAR2 (20);

   condizioni           VARCHAR2 (2000) := '';
   stringa_esecuzione   VARCHAR2 (9000) := '';
/******************************************************************************
   NAME:       sp_cdc_controllo_succ
   PURPOSE:
    Valorizza la tabella CDC_CONTROLLO_SUCC con i valori del report per Corte dei Conti
    per "il Controllo Successivo sugli atti delle Amministrazioni dello Stato"
    per gli atti di tipo 'Controllo Successivo SCCLA'

   REVISIONS:
   Ver        Date        Author           Description
   ---------  ----------  ---------------  ------------------------------------
   1.0        23/05/2012  Iacozzilli Giordano        1. Created this procedure.

Flusso logico della SP:

1. verifica data ed esegui solo se la differenza tra la data corrente e la precedente elaborazione
 superiore al parametro di intervallo passato (in ore)

2. nel caso di elaborazione costruisci le condizioni di elaborazione in base ai parametri passati
poi esegui l'interrogazione, cancella e valorizza la tabella

******************************************************************************/
BEGIN


inizio_com:=p_inizio||' 00:00:00';
fine_com:=p_fine||' 23:59:59';

-- inizio := to_date(p_inizio,'dd/mm/yyyy') ;
-- fine     := to_date(p_fine,'dd/mm/yyyy') ;  

inizio := to_date(inizio_com,'dd/mm/yyyy hh24:mi:ss') ;
fine     := to_date(fine_com,'dd/mm/yyyy hh24:mi:ss') ;  


--1. verifica data ed esegui solo se la differenza tra la data corrente e la precedente elaborazione
--  superiore al parametro di intervallo passato (in ore)
   SELECT   SYSDATE
          - NVL (MIN (data_elaborazione),
                 TO_DATE ('01/01/1970', 'mm/dd/yyyy'))
     INTO intervallo_elabora
     FROM cdc_controllo_succ;

  -- IF (intervallo_elabora * 24 > p_my_int_elabora_h)
  -- THEN
      -- elabora condizioni aggiuntive
      IF (p_ufficio != '' OR p_ufficio IS NOT NULL)
      THEN
         condizioni := condizioni || ' AND personorgroup IN ' || p_ufficio;
      END IF;

      IF (p_magistrato != '' OR p_magistrato IS NOT NULL)
      THEN
         condizioni :=
                  condizioni || ' AND Magistrato_Istruttore = ' || p_magistrato;
      END IF;

      IF (p_revisore != '' OR p_revisore IS NOT NULL)
      THEN
         condizioni :=
               condizioni
            || ' AND (Primo_Revisore = '
            || p_revisore
            || ' OR Secondo_Revisore = '
            || p_revisore
            || ' )';
      END IF;


-- inizio insert
     stringa_esecuzione :=
            'INSERT INTO cdc_controllo_succ
   SELECT *
  FROM (WITH singolerighe AS (SELECT cln0.lista,
   NVL (clna.cnt, 0) AS cnta,
   NVL (clnb.cnt, 0) AS cntb,
  NVL (clna.cnt, 0)
   + NVL (clnb.cnt, 0) AS cntc,
   NVL (clnd.cnt, 0) AS cntd,
   NVL (clne.cnt, 0) AS cnte,
   
   NVL (clnd.cnt, 0) + NVL (clne.cnt, 0)    AS cntf
   
   , NVL (clna.cnt, 0)  + NVL (clnb.cnt, 0)
   -  NVL (clnd.cnt, 0) - NVL (clne.cnt, 0)  AS cntg,
   NVL (clnh.cnt, 0) AS cnth,
   NVL (clni.cnt, 0) AS cnti,
   NVL (clnl.cnt, 0) AS cntl,
   NVL (clnm.cnt, 0) AS cntm,
   NVL (clnn.cnt, 0) AS cntn
 FROM (SELECT valore AS lista
   FROM v_cdc_ltipi_controlli_succ) cln0,
   (SELECT   Tipologia AS tipo,
 COUNT (*) AS cnt
  FROM V_CDC_CONTROLLO_SUCCESSIVO v
 WHERE  id_amm = '
         || p_my_id_amm
         || ' 
   '
         || condizioni
         || '
-- condizioni per calcolare i valori della cln A
 AND ( NVL (dta_arrivo,
   dta_protocollo
  ) < '''
         || inizio
         || '''
   AND ( Stato IN (''In esame'')
  )
 OR ( NVL (dta_arrivo, dta_protocollo) <
   '''
         || inizio
         || '''
  AND Data_Registrazione >= '''
         || inizio
         || '''
 )
 OR ( NVL (dta_arrivo, dta_protocollo) <
   ''' || inizio || '''
  AND Data_restituz_Amministrazione >=
   '''
         || inizio
         || '''
 ))
 GROUP BY Tipologia
  ) clna,
   (SELECT   Tipologia AS tipo,
 COUNT (*) AS cnt
  FROM V_CDC_CONTROLLO_SUCCESSIVO
 WHERE id_amm = '
         || p_my_id_amm
         || ' 
   '
         || condizioni
         || '
-- condizioni per calcolare i valori della cln B
   AND NVL (dta_arrivo, dta_protocollo)
 BETWEEN '''
         || inizio
         || '''
  AND '''
         || fine
         || '''
 GROUP BY Tipologia
  ) clnb,
   (SELECT   Tipologia AS tipo,
 COUNT (*) AS cnt
  FROM V_CDC_CONTROLLO_SUCCESSIVO
 WHERE id_amm = '
         || p_my_id_amm
         || '
  '
         || condizioni
         || '
-- condizioni per calcolare i valori della cln D
   AND Data_Registrazione BETWEEN '''
         || inizio
         || '''
  AND '''
         || fine
         || '''
 GROUP BY Tipologia
  ) clnd,
   (SELECT   Tipologia AS tipo,
 COUNT (*) AS cnt
  FROM V_CDC_CONTROLLO_SUCCESSIVO
 WHERE id_amm = '
         || p_my_id_amm
         || ' 
   '
         || condizioni
         || '
-- condizioni per calcolare i valori della cln E
   AND Data_restituz_Amministrazione
 BETWEEN '''
         || inizio
         || '''
  AND '''
         || fine
         || '''
 GROUP BY Tipologia
  ) clne,
   (SELECT   Tipologia AS tipo,
 COUNT (*) AS cnt
  FROM V_CDC_CONTROLLO_SUCCESSIVO
 WHERE  id_amm = '
         || p_my_id_amm
         || ' 
 '
         || condizioni
         || '
-- condizioni per calcolare i valori della cln F
 AND Data_restituz_Amministrazione
  BETWEEN '''
         || inizio
         || '''
   AND '''
         || fine
         || '''
 OR Data_Registrazione BETWEEN '''
         || inizio
         || '''
  AND '''
         || fine
         || '''
 GROUP BY Tipologia
  ) clnf,
   (SELECT   Tipologia AS tipo,
 COUNT (*) AS cnt
  FROM v_cdc_rilievi
 WHERE id_amm = '
         || p_my_id_amm
         || ' 
  '
         || condizioni
         || '
-- condizioni per calcolare i valori della cln H
   AND dta_protocollo BETWEEN '''
         || inizio
         || ''' AND '''
         || fine
         || '''
 GROUP BY Tipologia
  ) clnh,
   (SELECT   Tipologia AS tipo,
 COUNT (*) AS cnt
  FROM V_CDC_CONTROLLO_SUCCESSIVO
 WHERE id_amm = '
         || p_my_id_amm
         || ' 
   '
         || condizioni
         || '
-- condizioni per calcolare i valori della cln I
   AND (   Data_primo_rilievo BETWEEN '''
         || inizio
         || ''' AND '''
         || fine
         || '''
  OR Data_secondo_rilievo BETWEEN '''
         || inizio
         || '''  AND '''
         || fine
         || '''
  OR Data_osservazione BETWEEN '''
         || inizio
         || '''  AND '''
         || fine
         || '''
 )
 GROUP BY Tipologia
  ) clni,
   (SELECT   Tipologia AS tipo,
 COUNT (*) AS cnt
  FROM V_CDC_CONTROLLO_SUCCESSIVO
 WHERE id_amm = '
         || p_my_id_amm
         || ' 
  '
         || condizioni
         || '
-- condizioni per calcolare i valori della cln L
   AND Data_invio_deferimento BETWEEN '''
         || inizio
         || ''' AND '''
         || fine
         || '''
 GROUP BY Tipologia
  ) clnl,
   (SELECT   Tipologia AS tipo,
 COUNT (*) AS cnt
  FROM V_CDC_CONTROLLO_SUCCESSIVO
 WHERE id_amm = '
         || p_my_id_amm
         || ' 
  '
         || condizioni
         || '
-- condizioni per calcolare i valori della cln M
   AND Data_delibera BETWEEN '''
         || inizio
         || ''' AND '''
         || fine
         || '''
 GROUP BY Tipologia
  ) clnm,
   (SELECT   Tipologia AS tipo,
 COUNT (*) AS cnt
  FROM v_cdc_pareri
 WHERE id_amm = '
         || p_my_id_amm
         || '
  '
         || condizioni
         || '
-- condizioni per calcolare i valori della cln N
   AND creation_date BETWEEN '''
         || inizio
         || ''' AND '''
         || fine
         || '''
 GROUP BY Tipologia
  ) clnn
   WHERE cln0.lista = clna.tipo(+)
  AND cln0.lista = clnb.tipo(+)
  AND cln0.lista = clnd.tipo(+)
  AND cln0.lista = clne.tipo(+)
  AND cln0.lista = clnf.tipo(+)
  AND cln0.lista = clnh.tipo(+)
  AND cln0.lista = clni.tipo(+)
  AND cln0.lista = clnl.tipo(+)
  AND cln0.lista = clnm.tipo(+)
  AND cln0.lista = clnn.tipo(+))
  SELECT lista, cnta, cntb, cntc, cntd,
   cnte, cntf, cntg, cnth,
   cnti, cntl, cntm, cntn,
   SYSDATE
 FROM singolerighe
  UNION
  SELECT ''ZZTotali'', SUM (cnta), SUM (cntb),
   SUM (cntc), SUM (cntd), SUM (cnte),
   SUM (cntf), SUM (cntg), SUM (cnth),
   SUM (cnti), SUM (cntl), SUM (cntm),
   SUM (cntn), SYSDATE
 FROM singolerighe)
 order by 1 ';


--fine insert

IF (p_my_int_elabora_h = 'D') 
then
    DBMS_OUTPUT.put_line ('STRINGA ESECUZIONE: ' || stringa_esecuzione);

ELSE
     DELETE      cdc_controllo_succ;
     EXECUTE IMMEDIATE stringa_esecuzione;

END IF;

      COMMIT;
  -- END IF;
EXCEPTION
   WHEN OTHERS
   THEN
      -- Consider logging the error and then re-raise
      DBMS_OUTPUT.put_line ('Errore: ' || SQLERRM);

      RAISE;
END cdc_sp_controllo_succ;
/

