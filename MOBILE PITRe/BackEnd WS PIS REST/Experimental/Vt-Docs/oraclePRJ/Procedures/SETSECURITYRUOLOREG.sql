--------------------------------------------------------
--  DDL for Procedure SETSECURITYRUOLOREG
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."SETSECURITYRUOLOREG" 
(idCorrGlobali IN NUMBER, idProfile IN NUMBER,diritto IN NUMBER,Idreg in NUMBER, ReturnValue OUT NUMBER) IS

idGruppo dpa_corr_globali.id_gruppo%TYPE;

BEGIN

SELECT ID_GRUPPO INTO idGruppo FROM DPA_CORR_GLOBALI WHERE SYSTEM_ID =
idCorrGlobali;

IF (idGruppo IS NOT NULL) THEN
BEGIN
SELECT MAX(accessrights) INTO ReturnValue from security  where thing =
idProfile and personorgroup = idGruppo;
END;
--
IF (ReturnValue < diritto ) THEN
BEGIN
update security set accessrights = diritto where thing = idProfile and
personorgroup = idGruppo;
END;
END IF;

IF (ReturnValue IS NULL) THEN
BEGIN
insert into security  (thing   ,personorgroup, ACCESSRIGHTS, CHA_TIPO_DIRITTO)  
values(idProfile,idGruppo,diritto,'A');

END;
END IF;

END IF;



insert into security ( THING, PERSONORGROUP, ACCESSRIGHTS, ID_GRUPPO_TRASM, CHA_TIPO_DIRITTO )
select SYSTEM_ID,idGruppo,diritto,null,'A' from PROFILE p where ID_REGISTRO=Idreg
and num_proto is not null
and not exists (select 'x' from SECURITY s1 where s1.THING=p.system_id and
s1.PERSONORGROUP=idGruppo and s1.ACCESSRIGHTS=diritto );

ReturnValue := diritto;
END setsecurityRuoloReg; 

/
