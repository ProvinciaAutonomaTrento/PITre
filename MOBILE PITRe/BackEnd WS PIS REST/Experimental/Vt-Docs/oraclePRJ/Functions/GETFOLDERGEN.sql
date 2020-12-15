--------------------------------------------------------
--  DDL for Function GETFOLDERGEN
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETFOLDERGEN" (projectId INT)
RETURN number IS risultato number;
BEGIN
select * into risultato from (SELECT A.system_id FROM PROJECT A WHERE A.id_fascicolo=projectId and a.ID_FASCICOLO=a.ID_PARENT and a.CHA_TIPO_PROJ='C'  )
where rownum=1 ;
RETURN risultato;
END getFolderGEn; 

/
