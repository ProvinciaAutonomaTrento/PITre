BEGIN

/*
Create Index Indx_Cod_Fiscale_Text On Dpa_Dett_Globali(Var_Cod_Fisc)
Indextype Is Ctxsys.Context 			PARAMETERS ('sync (on commit) stoplist ctxsys.empty_stoplist'); 
*/
   DECLARE
      nomeindice  Varchar2 (32) :=  'Indx_Cod_Fiscale_Text' ;
	  nometabella Varchar2 (32) :=  'Dpa_Dett_Globali';
	  nomecolonna Varchar2 (32) :=	'Var_Cod_Fisc';
	  
	  Istruzione Varchar2 (2000) ;
      
      obj_esistente   EXCEPTION;
      PRAGMA EXCEPTION_INIT (obj_esistente   , -955);

	  index_esiste_altroNome  EXCEPTION;
      PRAGMA EXCEPTION_INIT (index_esiste_altroNome , -29879);
	  
	  mygranted_role VARCHAR2(20); 
	   
   BEGIN
      Istruzione   := 'CREATE INDEX '||nomeindice||' ON '||nometabella||'('|| nomecolonna ||')
			INDEXTYPE IS CTXSYS.CONTEXT 
			PARAMETERS (''sync (on commit) stoplist ctxsys.empty_stoplist'') ';

	  BEGIN -- check if role CTXAPP was granted
	  Select granted_role into mygranted_role 
			From User_role_Privs  where granted_role= 'CTXAPP' ; 
	  IF (nvl(mygranted_role,'null') <> 'CTXAPP' ) THEN  
            RAISE_APPLICATION_ERROR(-20001,'Missing CTXAPP role; CTXAPP role must be granted by DBA'); 
        end IF;
	  exception when NO_DATA_FOUND then 
                    RAISE_APPLICATION_ERROR(-20001,'Missing CTXAPP role; CTXAPP role must be granted by DBA'); 
              when others then RAISE; 
	  END; 
	  

	  EXECUTE IMMEDIATE istruzione;

   EXCEPTION
      WHEN obj_esistente   
      THEN
         DBMS_OUTPUT.put_line ('tabella già esistente');
      WHEN index_esiste_altroNome  
      THEN
         DBMS_OUTPUT.put_line ('indice già esistente con altro nome');
	  WHEN OTHERS
      THEN
         RAISE;
         DBMS_OUTPUT.put_line ('KO');
   END;
End;
/