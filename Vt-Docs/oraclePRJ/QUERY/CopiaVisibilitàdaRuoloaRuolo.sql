INSERT INTO SECURITY (THING,PERSONORGROUP,ACCESSRIGHTS,ID_GRUPPO_TRASM,CHA_TIPO_DIRITTO)
--INSERT INTO sec_s144svr_u413vol (THING,PERSONORGROUP,ACCESSRIGHTS,ID_GRUPPO_TRASM,CHA_TIPO_DIRITTO)
--create table sec_s144svr_u413vol(thing, personorgroup, accessrights, id_gruppo_trasm, cha_tipo_diritto) as
SELECT THING,@idNuovoRuolo@, -- nuovo ruolo
63,ID_GRUPPO_TRASM,'A' from
SECURITY s where s.PERSONORGROUP = @idVecchioRuolo@ --vecchio ruolo 
and not exists (select 'x' from SECURITY s1 where s1.thing=s.thing and s1.PERSONORGROUP=@idNuovoRuolo@) -- nuovo ruolo