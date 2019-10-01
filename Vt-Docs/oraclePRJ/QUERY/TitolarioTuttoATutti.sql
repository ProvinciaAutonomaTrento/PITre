--BACKUP security 
--create table security_appo_project0509 as select* from  security


/*
analyze table security compute statistics;

analyze project compute statistics;

analyze dpa_corr_globali compute statistics;

*/
--nodi tito 
insert /*+append*/ into security 
(THING, PERSONORGROUP, ACCESSRIGHTS, ID_GRUPPO_TRASM, CHA_TIPO_DIRITTO)
--select *From dpa_corr_globali  where id_gruppo in (
select  /*+index (a) index (b) */  a.system_id,
b.id_gruppo,255,null,'P'
 from  project a ,dpa_corr_globali b where a.id_titolario=@idTitolario@
and b.cha_tipo_urp='R' and cha_tipo_ie='I' and b.id_amm=@idAmministrazione@ and b.id_amm=a.ID_amm and a.CHA_TIPO_PROJ='T' and dta_fine is null
and not exists (select /*+index (s)*/ 'x' from security s where s.thing=a.system_id and s.PERSONORGROUP=b.id_gruppo 
)--)

--fascicoli generali

insert /*+append*/ into security 
(THING, PERSONORGROUP, ACCESSRIGHTS, ID_GRUPPO_TRASM, CHA_TIPO_DIRITTO)
--select *From dpa_corr_globali  where id_gruppo in (
select /*+index (a) index (b) */   a.system_id,
b.id_gruppo,
255,null,'P'
 from  project a ,dpa_corr_globali b where a.id_titolario=@idTitolario@
and b.cha_tipo_urp='R' and cha_tipo_ie='I' and b.id_amm=@idAmministrazione@ and b.id_amm=a.ID_amm and a.CHA_TIPO_fascicolo='G' and dta_fine is null
and not exists (select /*+index (s)*/  'x' from security s where s.thing=a.system_id and s.PERSONORGROUP=b.id_gruppo 
)--)

--root folder

insert /*+append*/ into security 
(THING, PERSONORGROUP, ACCESSRIGHTS, ID_GRUPPO_TRASM, CHA_TIPO_DIRITTO)
--select *From dpa_corr_globali  where id_gruppo in (
select /*+index (a) index (b) */   a.system_id,
b.id_gruppo,
255,null,'P'
 from  project a ,dpa_corr_globali b where a.id_titolario=@idTitolario@ and dta_fine is null
and b.cha_tipo_urp='R' and cha_tipo_ie='I' and b.id_amm=@idAmministrazione@ and b.id_amm=a.ID_amm and a.CHA_TIPO_proj='C' and exists 
(
select 'x' from project p where p.system_id=a.id_fascicolo and id_titolario=@idTitolario@ and cha_tipo_fascicolo='G' 
)
and not exists (select /*+index (s)*/  'x' from security s where s.thing=a.system_id and s.PERSONORGROUP=b.id_gruppo 
)

