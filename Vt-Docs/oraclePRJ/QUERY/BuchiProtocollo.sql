declare 
cursor c_buco is 
select num_proto from profile 
where id_registro =86107 and num_anno_proto=2012 -- and num_proto > 102400 per trovare il secondo
order by 1 ; 
cnt int := 0 ; -- 102400; per trovare il secondo
begin
for buco in c_buco
loop
cnt := cnt + 1 ; 
if (buco.num_proto <> cnt) then dbms_output.put_line('buco: '||buco.num_proto );
end if; 
end loop; 
end; 


-- cerco il protocollo precedente a quello mancante
select * from profile where num_proto in (1349, 1348) and num_anno_proto = 2012 and id_registro = 218147  -- 2782848

-- duplico la riga nella profila modificando i campi con i valori corretti. Inserisco un finto docnumber da aggiornare in seguito
Insert into PROFILE
   (SYSTEM_ID, DOCNUMBER, DOCNAME, TYPIST, AUTHOR, DOCUMENTTYPE, CREATION_DATE, CREATION_TIME, LAST_EDIT_DATE, ID_REGISTRO, CHA_TIPO_PROTO, ID_OGGETTO, NUM_PROTO, NUM_ANNO_PROTO, ID_PARENT, DTA_PROTO, CHA_MOD_OGGETTO, CHA_MOD_MITT_DEST, CHA_MOD_MITT_INT, DTA_PROTO_IN, VAR_SEGNATURA, CHA_DA_PROTO, CHA_ASSEGNATO, CHA_IMG, CHA_FASCICOLATO, CHA_INVIO_CONFERMA, CHA_CONGELATO, CHA_PRIVATO, VAR_CHIAVE_PROTO, CHA_EVIDENZA, VAR_PROF_OGGETTO, ID_PEOPLE_PROT, ID_RUOLO_PROT, ID_UO_PROT, ID_RUOLO_CREATORE, ID_UO_CREATORE, CHA_INTEROP, CHA_PERSONALE, CHA_IN_ARCHIVIO, CHA_FIRMATO, ID_PEOPLE_DELEGATO, CHA_DOCUMENTO_DA_PEC, LAST_FORWARD, FORWARDING_SOURCE)
 Values
   (seq.nextval, 11, 'TNNET-0001349-20/02/2012-A', 221971, 221971, 9, TO_DATE('02/17/2012 17:25:12', 'MM/DD/YYYY HH24:MI:SS'), TO_DATE('02/17/2012 17:25:12', 'MM/DD/YYYY HH24:MI:SS'), TO_DATE('02/20/2012 09:42:47', 'MM/DD/YYYY HH24:MI:SS'), 218147, 'A', 2785122, 1349, 2012, 0, TO_DATE('02/20/2012 09:42:45', 'MM/DD/YYYY HH24:MI:SS'), '0', '0', '0', TO_DATE('02/16/2012 00:00:00', 'MM/DD/YYYY HH24:MI:SS'), 'TNNET-0001349-20/02/2012-A', '0', '0', '1', '1', '0', '1', '0', '1349_2012_218147', '0', 'Richiesta di emissione di ordinanza per provvedimenti sul traffico', 221965, 221618, 221567, 221618, 221567, 'P', '0', '0', '0', 0, '1', -1, -1);

   -- mi serve per sapere il valore corretto del system_id appena inserito
select * from profile where docnumber = 11

-- alla fine aggiorno il record della profile con i seguenti valori
update profile set docnumber = 3088188, cha_mod_oggetto = 1, cha_in_cestino = 1 where system_id = 3088188

-- seleziono i record della security relativi ai soli proprietari del protocollo precedente
select * from security where thing = 2782848 and cha_tipo_diritto = 'P'

-- inserisco i dati nella security modificando il thing
Insert into SECURITY
   (THING, PERSONORGROUP, ACCESSRIGHTS, CHA_TIPO_DIRITTO, TS_INSERIMENTO)
 Values
   (3088188, 221971, 0, 'P', TO_TIMESTAMP('17/02/2012 17:24:12.919684','DD/MM/YYYY HH24:MI:SS.FF'));
Insert into SECURITY
   (THING, PERSONORGROUP, ACCESSRIGHTS, ID_GRUPPO_TRASM, CHA_TIPO_DIRITTO, TS_INSERIMENTO)
 Values
   (3088188, 221617, 255, 221617, 'P', TO_TIMESTAMP('17/02/2012 17:24:12.946058','DD/MM/YYYY HH24:MI:SS.FF'));

-- seleziono il record della version per il protocollo precedente
select * from versions v where v.DOCNUMBER = 2782848

-- inserisco nella versions il record per il nuovo protocollo
Insert into VERSIONS
   (VERSION_ID, DOCNUMBER, VERSION, SUBVERSION, VERSION_LABEL, AUTHOR, TYPIST, DTA_CREAZIONE, CHA_DA_INVIARE, DTA_ARRIVO, CARTACEO, ID_PEOPLE_DELEGATO)
 Values
   (seq.nextval, 3088188, 1, 'A', '1', 221971, 221971, TO_DATE('02/17/2012 17:25:12', 'MM/DD/YYYY HH24:MI:SS'), '1', TO_DATE('02/17/2012 15:43:01', 'MM/DD/YYYY HH24:MI:SS'), 0, 0);

-- prendo l'ultimo record inserito nella versions per inserire il version_id nella components
select * from versions where docnumber = 3088188 order by version_id desc

-- seleziono il record nella components del protocollo precedente
select * from components c where c.DOCNUMBER = 2782848

-- inserisco nella components i l valore relativo al nuovo protocollo con version_id e docnumber modificati
Insert into COMPONENTS
   (PATH, VERSION_ID, DOCNUMBER, FILE_SIZE, VAR_IMPRONTA, EXT, CHA_FIRMATO)
 Values
   ('\\172.20.15.20\PITRERepository\ENTI\DocServer\TNNET\2012\DIR GEN\20120217\2782854.PDF', 3088382, 3088381, 21139, 'A0CA4C7766F2C31A3D36147A4DA0E96E03C62E27B805F7384A3B03ED890C4FE3', 'PDF', '0');

-- prendo i dati dei mittenti e destinatari del vecchio protocollo
select * from dpa_doc_arrivo_par d where d.ID_PROFILE = 2782848

-- inserisco il mittente e i destinatari del nuovo protocollo
Insert into DPA_DOC_ARRIVO_PAR
   (SYSTEM_ID, ID_MITT_DEST, ID_PROFILE, CHA_TIPO_MITT_DEST, ID_DOCUMENTTYPES)
 Values
   (seq.nextval, 461000, 3088381, 'M', 683);
Insert into DPA_DOC_ARRIVO_PAR
   (SYSTEM_ID, ID_MITT_DEST, ID_PROFILE, CHA_TIPO_MITT_DEST, ID_DOCUMENTTYPES)
 Values
   (seq.nextval, 605441, 3088381, 'MD', 683);

-- prendo l'oggetto del vecchio protocollo
select * from dpa_oggetti_sto where id_profile = 2782848

-- Inserisco l'oggetto del nuovo protocollo
Insert into DPA_OGGETTI_STO
   (SYSTEM_ID, DTA_MODIFICA, ID_PROFILE, ID_OGGETTO, ID_PEOPLE, ID_RUOLO_IN_UO)
 Values
   (seq.nextval, TO_DATE('02/20/2012 09:42:45', 'MM/DD/YYYY HH24:MI:SS'), 3088188, 2782850, 221965, 221618);