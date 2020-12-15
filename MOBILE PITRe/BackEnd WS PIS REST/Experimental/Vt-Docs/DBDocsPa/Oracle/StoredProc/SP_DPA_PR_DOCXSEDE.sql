CREATE OR REPLACE PROCEDURE @db_user.SP_DPA_PR_DOCXSEDE (
ID_AMM_P  			  IN NUMBER,
ID_REGISTRO_P          IN NUMBER,
ANNO_P                 IN NUMBER,
ID_PEOPLE_P	 		  IN NUMBER,
TS_STAMPA  	 		  IN VARCHAR
)
IS
BEGIN --GENERALE
DECLARE
--variabile del cursore
v_var_sede         VARCHAR (255);
v_cha_tipo_proto   VARCHAR (1);
v_proto_count      NUMBER;
v_flag			 NUMBER;
--variabili ausiliarie
existSede NUMBER;
protoA NUMBER;
protoP NUMBER;
protoI NUMBER;
totProto NUMBER;
protoAnn NUMBER;
TS_STAMPA_P DATE;


CURSOR c_grigi
IS
SELECT  /*+  INDEX (profile INDX_PROFILE9) */ COUNT (system_id) AS protocount, var_sede
FROM PROFILE
WHERE
Getidamm(PROFILE.author) = ID_AMM_P
AND num_proto IS NULL
AND cha_tipo_proto = 'G'
AND TO_NUMBER (TO_CHAR (creation_date, 'YYYY')) = ANNO_P
AND var_sede >=CHR(0)
GROUP BY ROLLUP (var_sede);
-- CURSORE PER I PROTOCOLLI A,I,P,ANNULLATI
CURSOR c_protocolli
IS
SELECT /*+  INDEX (profile INDX_PROFILE41) */
COUNT (system_id) AS protocount, var_sede,cha_tipo_proto,DECODE(ID_ANNULLATORE,NULL,0,1) AS FLAG
FROM PROFILE
WHERE '0' = cha_da_proto
AND 1e10 >= -num_proto
AND ANNO_P = num_anno_proto
AND PROFILE.var_sede >= CHR (0)
AND ID_REGISTRO_P = id_registro
AND ('R' > cha_tipo_proto OR 'R' < cha_tipo_proto)
GROUP BY ROLLUP (var_sede, cha_tipo_proto, DECODE(ID_ANNULLATORE,NULL,0,1));

-- CURSORE PER I TOTALI Creati
CURSOR c_totali_creati
IS
SELECT SUM (grigi+prot),sede FROM DPA_PR_DOCXSEDE
WHERE ANNO = 'Creati' AND ID_PEOPLE = ID_PEOPLE_P AND TS_STAMPA = TS_STAMPA_P GROUP BY ROLLUP (sede);

-- CURSORE PER I DOC GRIGI CLASSIFICATI
CURSOR c_grigiClass
IS
SELECT /*+  INDEX (profile INDX_PROFILE9) */ COUNT (p.system_id) AS protocount, p.var_sede
FROM PROFILE p, PROJECT_COMPONENTS  pc
WHERE Getidamm(p.author) = ID_AMM_P
AND p.num_proto IS NULL
AND p.cha_tipo_proto = 'G'
AND TO_NUMBER (TO_CHAR (p.creation_date, 'YYYY')) = ANNO_P
AND p.var_sede >=CHR(0)
AND p.system_id IN pc.LINK
GROUP BY ROLLUP (var_sede);

--CURSORE PER PROTOCOLLI A,I,P,ANNULL CLASSIFICATI
CURSOR c_protocolli_class
IS
SELECT /*+  INDEX (profile INDX_PROFILE41) */
COUNT (system_id) AS protocount, var_sede,cha_tipo_proto,DECODE(ID_ANNULLATORE,NULL,0,1) AS FLAG
FROM PROFILE,PROJECT_COMPONENTS  pc
WHERE '0' = cha_da_proto
AND 1e10 >= -num_proto
AND ANNO_P = num_anno_proto
AND PROFILE.var_sede >= CHR (0)
AND ID_REGISTRO_P = id_registro
AND ('R' > cha_tipo_proto OR 'R' < cha_tipo_proto)
AND system_id IN pc.LINK
GROUP BY ROLLUP (var_sede, cha_tipo_proto, DECODE(ID_ANNULLATORE,NULL,0,1));

-- CURSORE PER I TOTALI classificati
CURSOR c_totali_classificati
IS
SELECT SUM (grigi+prot),sede FROM DPA_PR_DOCXSEDE WHERE
anno = 'Classificati' AND ID_PEOPLE = ID_PEOPLE_P AND TS_STAMPA = TS_STAMPA_P GROUP BY ROLLUP (sede);


--CURSORE PER I DOC GRIGI SENZA IMG
CURSOR c_grigi_prof
IS
SELECT  /*+  INDEX (profile INDX_PROFILE9) */ COUNT (system_id) AS protocount, var_sede
FROM PROFILE
WHERE
Getidamm(PROFILE.author) = ID_AMM_P
AND num_proto IS NULL
AND CHA_IMG = '0'
AND cha_tipo_proto = 'G'
AND TO_NUMBER (TO_CHAR (creation_date, 'YYYY')) = ANNO_P
AND var_sede >=CHR(0)
GROUP BY ROLLUP (var_sede);

-- CURSORE PER I PROTOCOLLI A,I,P,ANNULLATI SENZA IMG
CURSOR c_protocolli_prof
IS
SELECT /*+  INDEX (profile INDX_PROFILE41) */
COUNT (system_id) AS protocount, var_sede,cha_tipo_proto,DECODE(ID_ANNULLATORE,NULL,0,1) AS FLAG
FROM PROFILE
WHERE '0' = cha_da_proto
AND 1e10 >= -num_proto
AND CHA_IMG = '0'
AND ANNO_P = num_anno_proto
AND PROFILE.var_sede >= CHR (0)
AND ID_REGISTRO_P = id_registro
AND ('R' > cha_tipo_proto OR 'R' < cha_tipo_proto)
GROUP BY ROLLUP (var_sede, cha_tipo_proto, DECODE(ID_ANNULLATORE,NULL,0,1));

-- CURSORE PER I TOTALI Creati senza IMG
CURSOR c_totali_creati_prof
IS
SELECT SUM (grigi+prot),sede FROM DPA_PR_DOCXSEDE WHERE anno = 'Senza Img.'
AND ID_PEOPLE = ID_PEOPLE_P AND TS_STAMPA = TS_STAMPA_P GROUP BY ROLLUP (sede);

BEGIN -- MAIN
TS_STAMPA_P := TO_DATE (TS_STAMPA,'dd/mm/yyyy hh24:mi:ss');
-- SVUOTO LA TABELLA DEI DATI
DELETE DPA_PR_DOCXSEDE WHERE (ID_PEOPLE = ID_PEOPLE_P);

BEGIN ----------------------------------- CURSORE PER I DOC GRIGI --------------------------------------
OPEN c_grigi;
LOOP
FETCH c_grigi INTO v_proto_count, v_var_sede;
EXIT WHEN c_grigi%NOTFOUND;
-- CODICE DI LOOP
IF ((v_proto_count IS NOT NULL) AND (v_var_sede IS NOT NULL))THEN
INSERT INTO DPA_PR_DOCXSEDE (id_people,ts_stampa,anno,sede,grigi) VALUES (ID_PEOPLE_P,TS_STAMPA_P,'Creati',v_var_sede,v_proto_count);
END IF;
-- END CODICE DI LOOP
END LOOP;
CLOSE c_grigi;
-- RESET DELLE VARIABILI LOCALI
v_var_sede := '';
v_cha_tipo_proto := '';
v_proto_count := 0;
v_flag := 0;
existSede := 0;
END; ----------------------------------- CURSORE PER I DOC GRIGI --------------------------------------

BEGIN -------------------------------- CURSORE PER PROTOCOLLI A.I.P.ANNULLATI -------------------------
OPEN c_protocolli;
LOOP
FETCH c_protocolli INTO v_proto_count, v_var_sede, v_cha_tipo_proto,v_flag;
EXIT WHEN c_protocolli%NOTFOUND;


SELECT COUNT(*) INTO existSede FROM DPA_PR_DOCXSEDE WHERE SEDE=v_var_sede AND ID_PEOPLE=ID_PEOPLE_P AND TS_STAMPA = TS_STAMPA_P;
IF (existSede = 0) THEN
IF ((v_var_sede IS NOT NULL) AND (v_cha_tipo_proto IS NOT NULL) AND (v_proto_count IS NOT NULL) AND (v_flag = 0))
THEN
INSERT INTO DPA_PR_DOCXSEDE (id_people,ts_stampa,anno,sede) VALUES (ID_PEOPLE_P,TS_STAMPA_P,'Creati',v_var_sede);
END IF;
END IF;

-- PROTO ARRIVO
IF ((v_cha_tipo_proto = 'A') AND (v_proto_count > 0) AND (v_flag = 0))
THEN
UPDATE DPA_PR_DOCXSEDE
SET arrivo = v_proto_count
WHERE sede = v_var_sede AND anno = 'Creati'AND ID_PEOPLE=ID_PEOPLE_P AND TS_STAMPA = TS_STAMPA_P ;
protoA := v_proto_count;

END IF;

-- PROTO PARTENZA
IF ((v_cha_tipo_proto = 'P') AND (v_proto_count > 0) AND (v_flag = 0))
THEN
UPDATE DPA_PR_DOCXSEDE
SET partenza = v_proto_count
WHERE sede = v_var_sede AND anno = 'Creati' AND ID_PEOPLE=ID_PEOPLE_P AND TS_STAMPA = TS_STAMPA_P ;
protoP := v_proto_count;
END IF;

-- PROTO INTERNI
IF ((v_cha_tipo_proto = 'I') AND (v_proto_count > 0) AND (v_flag = 0))
THEN
UPDATE DPA_PR_DOCXSEDE
SET interni = v_proto_count
WHERE sede = v_var_sede AND anno = 'Creati' AND ID_PEOPLE=ID_PEOPLE_P AND TS_STAMPA = TS_STAMPA_P ;
protoI := v_proto_count;
END IF;

-- INSERISCO IL TOTALE PROTOCOLLI
IF ((v_proto_count > 0 ) AND (v_var_sede IS NOT NULL) AND (v_cha_tipo_proto IS NULL) AND (v_flag IS NULL)) THEN
UPDATE DPA_PR_DOCXSEDE SET prot = v_proto_count WHERE sede = v_var_sede AND anno = 'Creati' AND ID_PEOPLE=ID_PEOPLE_P AND TS_STAMPA = TS_STAMPA_P;
totProto := v_proto_count;
END IF;

--PROTO ANNULLATI
IF ((v_proto_count > 0) AND (v_var_sede IS NOT NULL) AND (v_flag = 1)) THEN
UPDATE DPA_PR_DOCXSEDE SET annull = v_proto_count WHERE sede = v_var_sede AND anno = 'Creati'  AND ID_PEOPLE=ID_PEOPLE_P AND TS_STAMPA = TS_STAMPA_P ;
protoAnn := NULL;
totProto := NULL;
protoA := NULL;
protoP := NULL;
protoI := NULL;
END IF;

-- PROTO ANNULLATI
--IF ((protoA IS NOT NULL) AND (protoP IS NOT NULL) AND (protoI IS NOT NULL) AND (totProto IS NOT NULL)) THEN
--protoAnn := totProto - (protoA+protoP+protoI);
--UPDATE DPA_PR_DOCXSEDE SET annull = protoAnn WHERE sede = v_var_sede AND anno = 'Creati'  AND ID_PEOPLE=ID_PEOPLE_P AND TS_STAMPA = TS_STAMPA_P ;
--protoAnn := NULL;
--totProto := NULL;
--protoA := NULL;
--protoP := NULL;
--protoI := NULL;
-- END IF;
--end inserimento
END LOOP;
CLOSE c_protocolli;
--reset variabili:
protoAnn := NULL;
totProto := NULL;
protoA := NULL;
protoP := NULL;
protoI := NULL;
v_var_sede := '';
v_cha_tipo_proto := '';
v_proto_count := 0;
v_flag := 0;
existSede := 0;

END;  -------------------------------- CURSORE PER PROTOCOLLI A.I.P.ANNULLATI--------------------------

BEGIN -------------------------------- CURSORE PER I TOTALI creati -------------------------
OPEN c_totali_creati;
LOOP
FETCH c_totali_creati INTO v_proto_count, v_var_sede;
UPDATE DPA_PR_DOCXSEDE SET TOT_DOC = v_proto_count WHERE sede = v_var_sede AND anno = 'Creati'  AND ID_PEOPLE=ID_PEOPLE_P AND TS_STAMPA = TS_STAMPA_P ;
EXIT WHEN c_totali_creati%NOTFOUND;
END LOOP;
CLOSE c_totali_creati;
END; -------------------------------- CURSORE PER I TOTALI creati -------------------------

BEGIN -------------------------------START CONTA DOCUMENTI CLASSIFICATI ------------
-- GRIGI CLASSIFICATI
OPEN c_grigiClass;
LOOP
FETCH c_grigiClass INTO v_proto_count, v_var_sede;
EXIT WHEN c_grigiClass%NOTFOUND;
IF ((v_proto_count IS NOT NULL) AND (v_var_sede IS NOT NULL))THEN
INSERT INTO DPA_PR_DOCXSEDE (anno,sede,grigi,id_people,ts_stampa) VALUES ('Classificati',v_var_sede,v_proto_count,ID_PEOPLE_P,TS_STAMPA_P );
END IF;
END LOOP;
CLOSE c_grigiClass;
--- PROTOCOLLI CLASSIFICATI
OPEN c_protocolli_class;
LOOP
FETCH c_protocolli_class INTO v_proto_count, v_var_sede, v_cha_tipo_proto,v_flag;
EXIT WHEN c_protocolli_class%NOTFOUND;
SELECT COUNT(*) INTO existSede FROM DPA_PR_DOCXSEDE WHERE SEDE=v_var_sede AND id_people= ID_PEOPLE_P AND ts_stampa=TS_STAMPA_P ;
IF (existSede = 0) THEN
IF ((v_var_sede IS NOT NULL) AND (v_cha_tipo_proto IS NOT NULL) AND (v_proto_count IS NOT NULL) AND (v_flag = 0))
THEN
INSERT INTO DPA_PR_DOCXSEDE (anno,sede,id_people,ts_stampa) VALUES ('Classificati',v_var_sede,ID_PEOPLE_P,TS_STAMPA_P );
END IF;
END IF;

-- PROTO ARRIVO
IF ((v_cha_tipo_proto = 'A') AND (v_proto_count > 0) AND (v_flag = 0))
THEN
UPDATE DPA_PR_DOCXSEDE
SET arrivo = v_proto_count
WHERE sede = v_var_sede AND anno = 'Classificati' AND ID_PEOPLE=ID_PEOPLE_P AND TS_STAMPA = TS_STAMPA_P ;
protoA := v_proto_count;
END IF;

-- PROTO PARTENZA
IF ((v_cha_tipo_proto = 'P') AND (v_proto_count > 0) AND (v_flag = 0))
THEN
UPDATE DPA_PR_DOCXSEDE
SET partenza = v_proto_count
WHERE sede = v_var_sede AND anno = 'Classificati' AND ID_PEOPLE=ID_PEOPLE_P AND TS_STAMPA = TS_STAMPA_P ;
protoP := v_proto_count;
END IF;

-- PROTO INTERNI
IF ((v_cha_tipo_proto = 'I') AND (v_proto_count > 0) AND (v_flag = 0))
THEN
UPDATE DPA_PR_DOCXSEDE
SET interni = v_proto_count
WHERE sede = v_var_sede AND anno = 'Classificati' AND ID_PEOPLE=ID_PEOPLE_P AND TS_STAMPA = TS_STAMPA_P ;
protoI := v_proto_count;
END IF;

-- INSERISCO IL TOTALE PROTOCOLLI
IF ((v_proto_count > 0 ) AND (v_var_sede IS NOT NULL) AND (v_cha_tipo_proto IS NULL) AND (v_flag IS NULL)) THEN
UPDATE DPA_PR_DOCXSEDE SET prot = v_proto_count WHERE sede = v_var_sede AND anno = 'Classificati' AND ID_PEOPLE=ID_PEOPLE_P AND TS_STAMPA = TS_STAMPA_P ;
totProto := v_proto_count;
END IF;

-- PROTO ANNULLATI
--IF ((protoA IS NOT NULL) AND (protoP IS NOT NULL) AND (protoI IS NOT NULL) AND (totProto IS NOT NULL)) THEN
--protoAnn := totProto - (protoA+protoP+protoI);
--UPDATE DPA_PR_DOCXSEDE SET annull = protoAnn WHERE sede = v_var_sede AND anno = 'Classificati' AND ID_PEOPLE=ID_PEOPLE_P AND TS_STAMPA = TS_STAMPA_P ;
--protoAnn := NULL;
--totProto := NULL;
--protoA := NULL;
--protoP := NULL;
--protoI := NULL;
-- END IF;
--PROTO ANNULLATI
IF ((v_proto_count > 0) AND (v_var_sede IS NOT NULL) AND (v_flag = 1)) THEN
UPDATE DPA_PR_DOCXSEDE SET annull = v_proto_count WHERE sede = v_var_sede AND anno = 'Classificati'  AND ID_PEOPLE=ID_PEOPLE_P AND TS_STAMPA = TS_STAMPA_P ;
protoAnn := NULL;
totProto := NULL;
protoA := NULL;
protoP := NULL;
protoI := NULL;
END IF;
END LOOP;
CLOSE c_protocolli_class;
--reset variabili:
protoAnn := NULL;
totProto := NULL;
protoA := NULL;
protoP := NULL;
protoI := NULL;
v_var_sede := '';
v_cha_tipo_proto := '';
v_proto_count := 0;
v_flag := 0;
existSede := 0;
BEGIN -------------------------------- CURSORE PER I TOTALI Classificati -------------------------
OPEN c_totali_classificati;
LOOP
FETCH c_totali_classificati INTO v_proto_count, v_var_sede;
UPDATE DPA_PR_DOCXSEDE SET TOT_DOC = v_proto_count WHERE sede = v_var_sede AND anno = 'Classificati'  AND ID_PEOPLE=ID_PEOPLE_P AND TS_STAMPA = TS_STAMPA_P ;
EXIT WHEN c_totali_classificati%NOTFOUND;
END LOOP;
CLOSE c_totali_classificati;
END; -------------------------------- CURSORE PER I TOTALI Classificati -------------------------
END;-------------------------------END CONTA DOCUMENTI CLASSIFICATI -----------------------------
BEGIN ------- START CONTA DOC SENZA IMMAGINI -----------------------
BEGIN ----------------------------------- DOC GRIGI SENZA IMG--------------------------------------
OPEN c_grigi_prof;
LOOP
FETCH c_grigi_prof INTO v_proto_count, v_var_sede;
EXIT WHEN c_grigi_prof%NOTFOUND;
-- CODICE DI LOOP
IF ((v_proto_count IS NOT NULL) AND (v_var_sede IS NOT NULL))THEN
INSERT INTO DPA_PR_DOCXSEDE (anno,sede,grigi,id_people,ts_stampa) VALUES ('Senza Img.',v_var_sede,v_proto_count,ID_PEOPLE_P,TS_STAMPA_P );
END IF;
-- END CODICE DI LOOP
END LOOP;
CLOSE c_grigi_prof;
-- RESET DELLE VARIABILI LOCALI
v_var_sede := '';
v_cha_tipo_proto := '';
v_proto_count := 0;
v_flag := 0;
existSede := 0;
END; ----------------------------------- DOC GRIGI SENZA IMG --------------------------------------
BEGIN -------------------------------- CURSORE PER PROTOCOLLI A.I.P.ANNULLATI senza IMG-------------------------
OPEN c_protocolli_prof;
LOOP
FETCH c_protocolli_prof INTO v_proto_count, v_var_sede, v_cha_tipo_proto,v_flag;
EXIT WHEN c_protocolli_prof%NOTFOUND;


SELECT COUNT(*) INTO existSede FROM DPA_PR_DOCXSEDE WHERE SEDE=v_var_sede AND ID_PEOPLE=ID_PEOPLE_P AND TS_STAMPA = TS_STAMPA_P ;
IF (existSede = 0) THEN
IF ((v_var_sede IS NOT NULL) AND (v_cha_tipo_proto IS NOT NULL) AND (v_proto_count IS NOT NULL) AND (v_flag = 0))
THEN
INSERT INTO DPA_PR_DOCXSEDE (anno,sede,id_people,ts_stampa) VALUES ('Senza Img.',v_var_sede,ID_PEOPLE_P,TS_STAMPA_P );
END IF;
END IF;

-- PROTO ARRIVO
IF ((v_cha_tipo_proto = 'A') AND (v_proto_count > 0) AND (v_flag = 0))
THEN
UPDATE DPA_PR_DOCXSEDE
SET arrivo = v_proto_count
WHERE sede = v_var_sede AND anno = 'Senza Img.' AND ID_PEOPLE=ID_PEOPLE_P AND TS_STAMPA = TS_STAMPA_P ;
protoA := v_proto_count;
END IF;

-- PROTO PARTENZA
IF ((v_cha_tipo_proto = 'P') AND (v_proto_count > 0) AND (v_flag = 0))
THEN
UPDATE DPA_PR_DOCXSEDE
SET partenza = v_proto_count
WHERE sede = v_var_sede AND anno = 'Senza Img.' AND ID_PEOPLE=ID_PEOPLE_P AND TS_STAMPA = TS_STAMPA_P ;
protoP := v_proto_count;
END IF;

-- PROTO INTERNI
IF ((v_cha_tipo_proto = 'I') AND (v_proto_count > 0) AND (v_flag = 0))
THEN
UPDATE DPA_PR_DOCXSEDE
SET interni = v_proto_count
WHERE sede = v_var_sede AND anno = 'Senza Img.' AND ID_PEOPLE=ID_PEOPLE_P AND TS_STAMPA = TS_STAMPA_P ;
protoI := v_proto_count;
END IF;

-- INSERISCO IL TOTALE PROTOCOLLI
IF ((v_proto_count > 0 ) AND (v_var_sede IS NOT NULL) AND (v_cha_tipo_proto IS NULL) AND (v_flag IS NULL)) THEN
UPDATE DPA_PR_DOCXSEDE SET prot = v_proto_count WHERE sede = v_var_sede AND anno = 'Senza Img.' AND ID_PEOPLE=ID_PEOPLE_P AND TS_STAMPA = TS_STAMPA_P ;
totProto := v_proto_count;
END IF;

-- PROTO ANNULLATI
--IF ((protoA IS NOT NULL) AND (protoP IS NOT NULL) AND (protoI IS NOT NULL) AND (totProto IS NOT NULL)) THEN
--protoAnn := totProto - (protoA+protoP+protoI);
--UPDATE DPA_PR_DOCXSEDE SET annull = protoAnn WHERE sede = v_var_sede AND anno = 'Senza Img.' AND ID_PEOPLE=ID_PEOPLE_P AND TS_STAMPA = TS_STAMPA_P ;
--protoAnn := NULL;
--totProto := NULL;
--protoA := NULL;
--protoP := NULL;
--protoI := NULL;
--END IF;
IF ((v_proto_count > 0) AND (v_var_sede IS NOT NULL) AND (v_flag = 1)) THEN
UPDATE DPA_PR_DOCXSEDE SET annull = v_proto_count WHERE sede = v_var_sede AND anno = 'Senza Img.'  AND ID_PEOPLE=ID_PEOPLE_P AND TS_STAMPA = TS_STAMPA_P ;
protoAnn := NULL;
totProto := NULL;
protoA := NULL;
protoP := NULL;
protoI := NULL;
END IF;
--end inserimento
END LOOP;
CLOSE c_protocolli_prof;
--reset variabili:
protoAnn := NULL;
totProto := NULL;
protoA := NULL;
protoP := NULL;
protoI := NULL;
v_var_sede := '';
v_cha_tipo_proto := '';
v_proto_count := 0;
v_flag := 0;
existSede := 0;

END;  -------------------------------- CURSORE PER PROTOCOLLI A.I.P.ANNULLATI--------------------------

BEGIN -------------------------------- CURSORE PER I TOTALI creati -------------------------
OPEN c_totali_creati_prof;
LOOP
FETCH c_totali_creati_prof INTO v_proto_count, v_var_sede;
UPDATE DPA_PR_DOCXSEDE SET TOT_DOC = v_proto_count WHERE sede = v_var_sede AND anno = 'Senza Img.' AND ID_PEOPLE=ID_PEOPLE_P AND TS_STAMPA = TS_STAMPA_P ;
EXIT WHEN c_totali_creati_prof%NOTFOUND;
END LOOP;
CLOSE c_totali_creati_prof;
END; -------------------------------- CURSORE PER I TOTALI creati -------------------------
END; ------- end CONTA DOC SENZA IMMAGINI -----------------------

END; -- MAIN
EXCEPTION
WHEN OTHERS
THEN
RETURN;

END; --GENERALE;
/