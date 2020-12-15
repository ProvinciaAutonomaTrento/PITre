--------------------------------------------------------
--  DDL for Function ATLEASTONECARTACEO
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."ATLEASTONECARTACEO" (docnum number)
RETURN VARCHAR2
IS
isCartaceo   VARCHAR2 (16);
BEGIN

DECLARE
vmaxidgenerica   NUMBER;

BEGIN
   SELECT MAX (v1.version_id)
   INTO vmaxidgenerica
   FROM VERSIONS v1, components c
   WHERE v1.docnumber = docnum AND v1.version_id = c.version_id;
  
   SELECT cartaceo
   INTO isCartaceo
   FROM VERSIONS
   WHERE docnumber = docnum AND version_id = vmaxidgenerica;
end;

   if(isCartaceo = '1') then
    begin
        declare item varchar(255);
        maxVersion number;
        CURSOR curAllegato IS select system_id from profile where id_documento_principale = docnum; 
        BEGIN
            OPEN curAllegato;
            LOOP
            FETCH curAllegato INTO item;
            EXIT WHEN (curAllegato%NOTFOUND or isCartaceo='0');
               SELECT MAX (v1.version_id)
               INTO maxVersion
               FROM VERSIONS v1, components c
               WHERE v1.docnumber = item AND v1.version_id = c.version_id;
               
               SELECT cartaceo
               INTO isCartaceo
               FROM VERSIONS
               WHERE docnumber = item AND version_id = maxVersion;
            END LOOP;
            CLOSE curAllegato;
        END;
    END;    
    end if;
   
RETURN isCartaceo;

exception
when no_data_found
then
isCartaceo :=  '0';
RETURN isCartaceo;
when others
then
isCartaceo := '0'; 
RETURN isCartaceo;

End AtLeastOneCartaceo; 

/
