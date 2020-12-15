begin
execute immediate 
'CREATE OR REPLACE FORCE VIEW RDE_AUTORIZZAZIONE (IDREGISTROREMOTO, IDUTENTEREMOTO) ' ||
'AS  ' ||
'SELECT DISTINCT  DPA_EL_REGISTRI.SYSTEM_ID AS IdRegistroRemoto,  PEOPLE.SYSTEM_ID AS IdUtenteRemoto ' ||
'FROM    DPA_TIPO_F_RUOLO INNER JOIN ' ||
'DPA_TIPO_FUNZIONE ON  DPA_TIPO_F_RUOLO.ID_TIPO_FUNZ =  DPA_TIPO_FUNZIONE.SYSTEM_ID INNER JOIN ' ||
'DPA_EL_REGISTRI INNER JOIN ' ||
'DPA_L_RUOLO_REG ON  DPA_EL_REGISTRI.SYSTEM_ID =  DPA_L_RUOLO_REG.ID_REGISTRO INNER JOIN ' ||
'DPA_CORR_GLOBALI ON  DPA_L_RUOLO_REG.ID_RUOLO_IN_UO =  DPA_CORR_GLOBALI.SYSTEM_ID INNER JOIN ' ||
'PEOPLEGROUPS ON  DPA_CORR_GLOBALI.ID_GRUPPO =  PEOPLEGROUPS.GROUPS_SYSTEM_ID INNER JOIN ' ||
'PEOPLE ON  PEOPLEGROUPS.PEOPLE_SYSTEM_ID =  PEOPLE.SYSTEM_ID ON ' ||
'DPA_TIPO_F_RUOLO.ID_RUOLO_IN_UO =  DPA_L_RUOLO_REG.ID_RUOLO_IN_UO ' ||
'WHERE  ( DPA_TIPO_FUNZIONE.SYSTEM_ID IN (select id_tipo_funzione from DPA_FUNZIONI where cod_funzione = ''PROTO_EME'')) AND ( DPA_CORR_GLOBALI.DTA_FINE IS NULL) AND ' ||
'( DPA_CORR_GLOBALI.CHA_TIPO_URP = ''R'') ';
end;
/



