create or replace
FUNCTION GETOBJECTMAINDOCCHECKSECURITY(PROFILEID NUMBER, PEOPLEID NUMBER, GROUPID NUMBER, ruoloPubblicoId NUMBER)
    RETURN VARCHAR  IS TMPVAR VARCHAR (2000);
    CHECKSECURITY INT;
    tipoObjParam VARCHAR(200) := 'D';
  BEGIN
    BEGIN
      SELECT var_prof_oggetto
      INTO tmpvar
      FROM PROFILE A
      WHERE A.SYSTEM_ID = profileID and  ROWNUM = 1;
   EXCEPTION
      WHEN NO_DATA_FOUND
      THEN
         tmpvar := '';
   END;
   BEGIN
	   checkSecurity := checkSecurityDocumento(profileID, peopleID, groupID, ruoloPubblicoId, tipoObjParam);
	   IF checkSecurity = 0
	   THEN
			tmpvar := tmpvar || '#0';
	   ELSE
			tmpvar := tmpvar || '#1';
	   END IF;
   END;

   RETURN TMPVAR;
  END GETOBJECTMAINDOCCHECKSECURITY;