--------------------------------------------------------
--  DDL for Procedure SP_MODIFY_UO_INTERNA
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."SP_MODIFY_UO_INTERNA" (
idcorrglobale     IN       NUMBER,
desc_corr         IN       VARCHAR2,
codice_aoo        IN       VARCHAR2,
codice_amm        IN       VARCHAR2,
indirizzo         IN       VARCHAR2,
cap               IN       VARCHAR2,
provincia         IN       VARCHAR2,
nazione           IN       VARCHAR2,
citta             IN       VARCHAR2,
telefono          IN       VARCHAR2,
telefono2         IN       VARCHAR2,
fax               IN       VARCHAR2,
newid             OUT      NUMBER,
returnvalue       OUT      NUMBER
)
IS
BEGIN
DECLARE
cod_rubrica              VARCHAR2 (128);
id_reg                   NUMBER;
idamm                    NUMBER;
new_var_cod_rubrica      VARCHAR2 (128);
cha_dettaglio            CHAR (1)       := '0';
cha_tipourp              CHAR (1);
myprofile                NUMBER;
new_idcorrglobale        NUMBER;
identitydettglobali      NUMBER;
outvalue                 NUMBER         := 1;
rtn                      NUMBER;
v_id_doctype             NUMBER;
identitydpatcanalecorr   NUMBER;
chaTipoIE                CHAR (1);
numLivello               NUMBER          := 0;
idParent                 NUMBER;
idPesoOrg                NUMBER;
idUO                     NUMBER;
idGruppo                 NUMBER;
idTipoRuolo              NUMBER;
cha_tipocorr             CHAR (1);
chapa            CHAR (1);
varcodaoo                VARCHAR2(16);
varcodamm                VARCHAR2(32);
varemail                 VARCHAR2(128);
BEGIN

<<reperimento_dati>>
BEGIN
SELECT var_cod_rubrica, cha_tipo_urp, id_registro, id_amm, cha_pa, cha_tipo_ie,
num_livello, id_parent, id_peso_org, id_uo, id_tipo_ruolo, id_gruppo, cha_tipo_corr, cha_dettagli,
var_codice_aoo, var_codice_amm, var_email
INTO cod_rubrica, cha_tipourp, id_reg, idamm, chapa, chaTipoIE,
numLivello, idParent, idPesoOrg, IdUO, idTipoRuolo, idGruppo, cha_tipocorr, cha_dettaglio,
varcodaoo, varcodamm, varemail
FROM dpa_corr_globali
WHERE system_id = idcorrglobale;
EXCEPTION
WHEN NO_DATA_FOUND
THEN
outvalue := 0;
RETURN;
END reperimento_dati;

IF /* 0 */ outvalue = 1
THEN
-- 1) update del record vecchio nella dpa_corr_globali
new_var_cod_rubrica := cod_rubrica || '_' || TO_CHAR (idcorrglobale);

<<storicizzazione_corrisp>>
BEGIN
UPDATE dpa_corr_globali
SET dta_fine = SYSDATE,
var_cod_rubrica = new_var_cod_rubrica,
var_codice = new_var_cod_rubrica,
id_parent = NULL
WHERE system_id = idcorrglobale;
EXCEPTION
WHEN OTHERS
THEN
outvalue := 0;
RETURN;
END storicizzazione_corrisp;


-- 3) inserisco il nuovo record nella dpa_corr_globali
IF /* 2 */ outvalue = 1
THEN
SELECT seq.NEXTVAL
INTO newid
FROM DUAL;

<<inserimento_nuovo_corrisp>>
BEGIN
INSERT INTO dpa_corr_globali
(system_id, num_livello, cha_tipo_ie, id_registro,
id_amm, var_desc_corr,
id_old, dta_inizio, id_parent, var_codice,
cha_tipo_corr, cha_tipo_urp,
var_codice_aoo, var_cod_rubrica, cha_dettagli,
var_codice_amm, cha_pa, id_peso_org,
id_gruppo, id_tipo_ruolo, id_uo, var_email
)
VALUES (newid, numLivello, chaTipoIE, id_reg,
idamm, desc_corr,
idcorrglobale, SYSDATE, idParent, cod_rubrica,
cha_tipocorr, cha_tipourp,
varcodaoo, cod_rubrica, cha_dettaglio,
varcodamm, chapa, idPesoOrg,
idGruppo, idTipoRuolo, idUO, varemail
);
EXCEPTION
WHEN OTHERS
THEN
outvalue := 0;
RETURN;
END inserimento_nuovo_corrisp;

/* DOPO L'INSERIMENTO DEL NUOVO CORRISPONDENTE POSSO INSERIRE IL
RELATIVO RECORD NELLA DPA_DETT_GLOBALI,*/
IF /* 3 */ outvalue = 1
THEN
--PRENDO LA SYSTEM_ID APPENA INSERITA
SELECT seq.NEXTVAL
INTO identitydettglobali
FROM DUAL;

<<inserimento_dettaglio_corrisp>>
BEGIN
INSERT INTO dpa_dett_globali
(system_id, id_corr_globali, var_indirizzo,
var_cap, var_provincia, var_nazione,
var_telefono, var_telefono2,
var_citta, var_fax
)
VALUES (identitydettglobali, newid, indirizzo,
cap, provincia, nazione,
telefono, telefono2,
citta, fax
);
EXCEPTION
WHEN OTHERS
THEN
outvalue := 0;
RETURN;
END inserimento_dettaglio_corrisp;
END IF;  /* 3 */

END IF; /* 2 */

-- 4) update di tutti i ruoli che avevano ID_UO = ID_UO_OLD con l'ID del nuovo record inserito
IF /* 4 */ outvalue = 1
THEN
BEGIN
UPDATE DPA_CORR_GLOBALI
SET ID_UO = newid
WHERE ID_UO = idcorrglobale;
EXCEPTION
WHEN OTHERS
THEN
outvalue := 0;
RETURN;
END;
END IF; /* 4 */

-- 5) update di tutte le UO che avevano ID_PARENT = ID_UO_OLD con l'ID del nuovo record inserito
IF /* 5 */ outvalue = 1
THEN
BEGIN
UPDATE DPA_CORR_GLOBALI
SET ID_PARENT = newid
WHERE ID_PARENT = idcorrglobale;
EXCEPTION
WHEN OTHERS
THEN
outvalue := 0;
RETURN;
END;
END IF; /* 5 */

IF /* 6 */ outvalue = 1
THEN
BEGIN
INSERT INTO DPA_UO_REG(SYSTEM_ID, ID_UO, ID_REGISTRO)
SELECT seq.NEXTVAL, newid, ID_REGISTRO FROM DPA_UO_REG WHERE ID_UO = idcorrglobale;
EXCEPTION
WHEN OTHERS
THEN
outvalue := 0;
RETURN;
END;
END IF;  /* 6 */

--se fa parte di una lista, allora la devo aggiornare.
update dpa_liste_distr d set d.ID_DPA_CORR=newid where d.ID_DPA_CORR=idcorrglobale;


END IF /* 0 */;

returnvalue := outvalue;
END;
END; 

/
