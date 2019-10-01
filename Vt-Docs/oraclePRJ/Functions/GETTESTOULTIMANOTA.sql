--------------------------------------------------------
--  DDL for Function GETTESTOULTIMANOTA
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETTESTOULTIMANOTA" 
-- =============================================
-- Author:        P. De Luca
-- Create date: 15 giu 2011
-- Description:    ritorna il testo della nota piu recente associata al documento
-- =============================================
(
p_TIPOOGGETTOASSOCIATO varchar 
, p_IDOGGETTOASSOCIATO int
, p_ID_RUOLO_IN_UO int
, p_IDUTENTECREATORE int
, p_IDRUOLOCREATORE int
) 
RETURN varchar IS ultimanota varchar(2000);

BEGIN 

IF (p_TIPOOGGETTOASSOCIATO  <> 'F' AND p_TIPOOGGETTOASSOCIATO  <> 'D') THEN 
            ultimanota := '-1'; 
    RETURN ultimanota ;

END IF;          


IF p_TIPOOGGETTOASSOCIATO = 'F' THEN 
SELECT TESTO into ultimanota 
      FROM
      (
      SELECT /*+ FIRST_ROWS(1) */ 
      -- HINT PRECEDENTE serve per ottimizzare tempi risposta, da oracle 10 in poi
      -- N.SYSTEM_ID,
      nvl(N.TESTO,'null') as testo
     FROM    DPA_NOTE N
      LEFT JOIN People P ON N.IDUTENTECREATORE = P.SYSTEM_ID
      LEFT JOIN Groups G ON N.IDRUOLOCREATORE = G.SYSTEM_ID
      LEFT JOIN PROJECT PR  ON N.IDOGGETTOASSOCIATO =  PR.SYSTEM_ID 
      WHERE   
      N.TIPOOGGETTOASSOCIATO = p_TIPOOGGETTOASSOCIATO AND
      N.IDOGGETTOASSOCIATO = p_IDOGGETTOASSOCIATO AND
      (N.TIPOVISIBILITA = 'T' OR
      (N.TIPOVISIBILITA = 'F' and N.IDRFASSOCIATO in 
      (select id_registro from dpa_l_ruolo_reg rr where  rr.ID_RUOLO_IN_UO = p_ID_RUOLO_IN_UO)) OR
      (N.TIPOVISIBILITA = 'P' AND N.IDUTENTECREATORE = p_IDUTENTECREATORE) OR
      (N.TIPOVISIBILITA = 'R' AND N.IDRUOLOCREATORE = p_IDRUOLOCREATORE))
      ORDER BY N.DATACREAZIONE DESC
      )
      WHERE   ROWNUM = 1 ; 
END IF; 

IF p_TIPOOGGETTOASSOCIATO = 'D' THEN  --join con la profile invece della project
SELECT TESTO into ultimanota 
      FROM
      (
      SELECT /*+ FIRST_ROWS(1) */ 
      -- HINT PRECEDENTE serve per ottimizzare tempi risposta, da oracle 10 in poi
      -- N.SYSTEM_ID,
      nvl(N.TESTO,'null') as testo
     FROM    DPA_NOTE N
      LEFT JOIN People P ON N.IDUTENTECREATORE = P.SYSTEM_ID
      LEFT JOIN Groups G ON N.IDRUOLOCREATORE = G.SYSTEM_ID
      LEFT JOIN profile PR  ON N.IDOGGETTOASSOCIATO =  PR.SYSTEM_ID 
      WHERE   
      N.TIPOOGGETTOASSOCIATO = p_TIPOOGGETTOASSOCIATO AND
      N.IDOGGETTOASSOCIATO = p_IDOGGETTOASSOCIATO AND
      (N.TIPOVISIBILITA = 'T' OR
      (N.TIPOVISIBILITA = 'F' and N.IDRFASSOCIATO in 
      (select id_registro from dpa_l_ruolo_reg rr where  rr.ID_RUOLO_IN_UO = p_ID_RUOLO_IN_UO)) OR
      (N.TIPOVISIBILITA = 'P' AND N.IDUTENTECREATORE = p_IDUTENTECREATORE) OR
      (N.TIPOVISIBILITA = 'R' AND N.IDRUOLOCREATORE = p_IDRUOLOCREATORE))
      ORDER BY N.DATACREAZIONE DESC
      )
      WHERE   ROWNUM = 1 ; 
      
END IF; 
return ultimanota;


EXCEPTION
when no_data_found  then
ultimanota := ''; 
 return ultimanota;

when others  then
ultimanota := '-1'; 
 return ultimanota;

 END; 

/
