CREATE OR REPLACE FUNCTION @db_user.fascicolivttablefunction (
id_amm        NUMBER,
id_registro   NUMBER,
anno          NUMBER,
mese          NUMBER
)
RETURN fascicolivttablerow PIPELINED
IS

out_rec                 fascicolivttabletype
:= fascicolivttabletype (NULL,
NULL,
NULL,
NULL,
NULL,
NULL,
NULL,
NULL,
NULL,
NULL,
NULL,
NULL,
NULL,
NULL,
NULL,
NULL,
NULL
);

system_id_vt            NUMBER;
description_vt          VARCHAR (255);
var_codice_vt           VARCHAR (255);
m_fasc_creati           NUMBER;
fasc_chiusi             NUMBER;
fasc_creati             NUMBER;
m_fasc_chiusi           NUMBER;
i                       NUMBER;
mese_vc                 NUMBER;
mese_vc_c               NUMBER;
gennaio                 NUMBER;
febbraio                NUMBER;
marzo                   NUMBER;
aprile                  NUMBER;
maggio                  NUMBER;
giugno                  NUMBER;
luglio                  NUMBER;
agosto                  NUMBER;
settembre               NUMBER;
ottobre                 NUMBER;
novembre                NUMBER;
dicembre                NUMBER;
vt_fasc_creati          NUMBER;
vt_fasc_chiusi          NUMBER;
num_livello1            VARCHAR (255);
var_codice_livello1     VARCHAR (255);
description__livello1   VARCHAR (255);
system_id_fasc          NUMBER;
gennaio_c               NUMBER;
febbraio_c              NUMBER;
marzo_c                 NUMBER;
aprile_c                NUMBER;
maggio_c                NUMBER;
giugno_c                NUMBER;
luglio_c                NUMBER;
agosto_c                NUMBER;
settembre_c             NUMBER;
ottobre_c               NUMBER;
novembre_c              NUMBER;
dicembre_c              NUMBER;
tot_gennaio             VARCHAR (255);
tot_febbraio            VARCHAR (255);
tot_marzo               VARCHAR (255);
tot_aprile              VARCHAR (255);
tot_maggio              VARCHAR (255);
tot_giugno              VARCHAR (255);
tot_luglio              VARCHAR (255);
tot_agosto              VARCHAR (255);
tot_settembre           VARCHAR (255);
tot_ottobre             VARCHAR (255);
tot_novembre            VARCHAR (255);
tot_dicembre            VARCHAR (255);
tot_vt_t                VARCHAR (255);
tot_vt                  NUMBER;
tot_vt_c                NUMBER;
t_gennaio               NUMBER;
t_febbraio              NUMBER;
t_marzo                 NUMBER;
t_aprile                NUMBER;
t_maggio                NUMBER;
t_giugno                NUMBER;
t_luglio                NUMBER;
t_agosto                NUMBER;
t_settembre             NUMBER;
t_ottobre               NUMBER;
t_novembre              NUMBER;
t_dicembre              NUMBER;
t_gennaio_c             NUMBER;
t_febbraio_c            NUMBER;
t_marzo_c               NUMBER;
t_aprile_c              NUMBER;
t_maggio_c              NUMBER;
t_giugno_c              NUMBER;
t_luglio_c              NUMBER;
t_agosto_c              NUMBER;
t_settembre_c           NUMBER;
t_ottobre_c             NUMBER;
t_novembre_c            NUMBER;
t_dicembre_c            NUMBER;

CURSOR c_vocitit (amm NUMBER, idreg NUMBER)
IS
SELECT   project.system_id, project.description, project.var_codice,
project.num_livello
FROM project
WHERE project.var_codice IS NOT NULL
AND project.id_titolario = (select system_id from project where cha_stato= 'A' and var_codice = 'T')
AND project.id_amm = amm
AND project.cha_tipo_proj = 'T'
AND (project.id_registro = idreg OR project.id_registro IS NULL)
ORDER BY project.var_cod_liv1;
BEGIN
mese_vc := 0;
mese_vc_c := 0;
i := 1;
gennaio := 0;
febbraio := 0;
marzo := 0;
aprile := 0;
maggio := 0;
giugno := 0;
luglio := 0;
agosto := 0;
settembre := 0;
ottobre := 0;
novembre := 0;
dicembre := 0;
gennaio_c := 0;
febbraio_c := 0;
marzo_c := 0;
aprile_c := 0;
maggio_c := 0;
giugno_c := 0;
luglio_c := 0;
agosto_c := 0;
settembre_c := 0;
ottobre_c := 0;
novembre_c := 0;
dicembre_c := 0;
t_gennaio := 0;
t_febbraio := 0;
t_marzo := 0;
t_aprile := 0;
t_maggio := 0;
t_giugno := 0;
t_luglio := 0;
t_agosto := 0;
t_settembre := 0;
t_ottobre := 0;
t_novembre := 0;
t_dicembre := 0;
t_gennaio_c := 0;
t_febbraio_c := 0;
t_marzo_c := 0;
t_aprile_c := 0;
t_maggio_c := 0;
t_giugno_c := 0;
t_luglio_c := 0;
t_agosto_c := 0;
t_settembre_c := 0;
t_ottobre_c := 0;
t_novembre_c := 0;
t_dicembre_c := 0;
tot_vt_t := '0/0';
tot_vt := 0;
tot_vt_c := 0;

/*******************************************************************/

/*******************************RECUPERO DEI DATI GENERALI************************************************************************************
FASC_CREATI:  Tutti i fascicoli con:
- anno di creazione pari all'anno di interesse,
- id_registro pari a qullo selezionato
- id_amm pari a quello selezionato

FASC_GEN_CREATI: Tutti i fascicoli generali:
- id_registro pari a null
- id_amm pari a quello selezionato

FASC_CHIUSI: Tutti i fascicoli chiusi:
- id_registro pari a qullo selezionato
- id_amm pari a quello selezionato
- anno della dati di chiusura pari a quello selezionato
*/
/*QUERY PER IL RECUPERO DEI DATI GENERALI*/
SELECT COUNT (project.system_id)
INTO fasc_creati
FROM project
WHERE project.cha_tipo_proj = 'F'
AND TO_NUMBER (TO_CHAR (project.dta_creazione, 'YYYY')) = anno
AND (project.id_registro = id_registro OR project.id_registro IS NULL)
AND project.id_amm = id_amm
AND project.cha_stato = 'A';

SELECT COUNT (project.system_id)
INTO fasc_chiusi
FROM project
WHERE project.cha_tipo_proj = 'F'
AND (project.id_registro = id_registro OR project.id_registro = NULL)
AND project.id_amm = id_amm
AND TO_NUMBER (TO_CHAR (project.dta_chiusura, 'YYYY')) = anno
AND project.cha_stato = 'C';

/*****************************************************************************************************************************************************/

/*1 QUERY- Recupera l'elenco delle voci di titolario  (input : @id_amm) */
/*-- contiene tutte le voci di titolario (TIPO "T")*/

/*Apertura del cursore*/
OPEN c_vocitit (id_amm, id_registro);

LOOP
FETCH c_vocitit
INTO system_id_vt, description_vt, var_codice_vt, num_livello1;

EXIT WHEN c_vocitit%NOTFOUND;

IF (num_livello1 = 1)
THEN
var_codice_livello1 := var_codice_vt;
description__livello1 := description_vt;
END IF;

/*2 QUERY- selezione dei fascicoli dellla relativa voce di titolario- (input @id_amm)*/
WHILE (i <= mese)
LOOP
SELECT COUNT (project.system_id)
INTO vt_fasc_creati
FROM project
WHERE TO_NUMBER (TO_CHAR (project.dta_creazione, 'YYYY')) = anno
AND project.cha_tipo_proj = 'F'
AND project.id_amm = id_amm
AND project.id_parent = system_id_vt
AND (   project.id_registro = id_registro
OR project.id_registro IS NULL
)
AND project.cha_stato = 'A';

SELECT COUNT (project.system_id)
INTO vt_fasc_chiusi
FROM project
WHERE project.cha_tipo_proj = 'F'
AND project.id_amm = id_amm
AND project.id_parent = system_id_vt
AND (   project.id_registro = id_registro
OR project.id_registro IS NULL
)
AND TO_NUMBER (TO_CHAR (project.dta_chiusura, 'YYYY')) = anno
AND project.cha_stato = 'C';

SELECT COUNT (project.system_id)
INTO m_fasc_creati
FROM project
WHERE TO_NUMBER (TO_CHAR (project.dta_creazione, 'YYYY')) = anno
AND project.cha_tipo_proj = 'F'
AND project.id_amm = id_amm
AND project.id_parent = system_id_vt
AND TO_NUMBER (TO_CHAR (project.dta_apertura, 'MM')) = i
AND (   project.id_registro = id_registro
OR project.id_registro IS NULL
)
AND project.cha_stato = 'A';

SELECT COUNT (project.system_id)
INTO m_fasc_chiusi
FROM project
WHERE TO_NUMBER (TO_CHAR (project.dta_chiusura, 'YYYY')) = anno
AND project.cha_tipo_proj = 'F'
AND project.cha_stato = 'C'
AND project.id_amm = id_amm
AND project.id_parent = system_id_vt
AND TO_NUMBER (TO_CHAR (project.dta_chiusura, 'MM')) = i
AND (   project.id_registro = id_registro
OR project.id_registro IS NULL
);

/*INSERIMENTO DEL VALORE MENSILE NELLA TABELLA TEMPORANEA*/
/*A seconda del mese avremo una query di inserimento diversa, per popolare il giusto campo della tabella*/
mese_vc := vt_fasc_creati;
mese_vc_c := vt_fasc_chiusi;

IF (i = 1)
THEN
/*Gennaio*/
IF (m_fasc_creati > 0 OR m_fasc_chiusi > 0)
THEN
gennaio := m_fasc_creati;
gennaio_c := m_fasc_chiusi;
ELSE
gennaio := 0;
gennaio_c := 0;
END IF;
END IF;

IF (i = 2)
THEN
/*Febbraio*/
IF (m_fasc_creati > 0 OR m_fasc_chiusi > 0)
THEN
febbraio := m_fasc_creati;
febbraio_c := m_fasc_chiusi;
ELSE
febbraio := 0;
febbraio_c := 0;
END IF;
END IF;

IF (i = 3)
THEN
/*Marzo*/
IF (m_fasc_creati > 0 OR m_fasc_chiusi > 0)
THEN
marzo := m_fasc_creati;
marzo_c := m_fasc_chiusi;
ELSE
marzo := 0;
marzo_c := 0;
END IF;
END IF;

IF (i = 4)
THEN
/*Aprile*/
IF (m_fasc_creati > 0 OR m_fasc_chiusi > 0)
THEN
aprile := m_fasc_creati;
aprile_c := m_fasc_chiusi;
ELSE
aprile := 0;
aprile_c := 0;
END IF;
END IF;

IF (i = 5)
THEN
/*MAggio*/
IF (m_fasc_creati > 0 OR m_fasc_chiusi > 0)
THEN
maggio := m_fasc_creati;
maggio_c := m_fasc_chiusi;
ELSE
maggio := 0;
maggio_c := 0;
END IF;
END IF;

IF (i = 6)
THEN
/*Giugno*/
IF (m_fasc_creati > 0 OR m_fasc_chiusi > 0)
THEN
giugno := m_fasc_creati;
giugno_c := m_fasc_chiusi;
ELSE
giugno := 0;
giugno_c := 0;
END IF;
END IF;

IF (i = 7)
THEN
/*Luglio*/
IF (m_fasc_creati > 0 OR m_fasc_chiusi > 0)
THEN
luglio := m_fasc_creati;
luglio_c := m_fasc_chiusi;
ELSE
luglio := 0;
luglio_c := 0;
END IF;
END IF;

IF (i = 8)
THEN
/*Agosto*/
IF (m_fasc_creati > 0 OR m_fasc_chiusi > 0)
THEN
agosto := m_fasc_creati;
agosto_c := m_fasc_chiusi;
ELSE
agosto := 0;
agosto_c := 0;
END IF;
END IF;

IF (i = 9)
THEN
/*Settembre*/
IF (m_fasc_creati > 0 OR m_fasc_chiusi > 0)
THEN
settembre := m_fasc_creati;
settembre_c := m_fasc_chiusi;
ELSE
settembre := 0;
settembre_c := 0;
END IF;
END IF;

IF (i = 10)
THEN
/*Ottobre*/
IF (m_fasc_creati > 0 OR m_fasc_chiusi > 0)
THEN
ottobre := m_fasc_creati;
ottobre_c := m_fasc_chiusi;
ELSE
ottobre := 0;
ottobre_c := 0;
END IF;
END IF;

IF (i = 11)
THEN
/*Novembre*/
IF (m_fasc_creati > 0 OR m_fasc_chiusi > 0)
THEN
novembre := m_fasc_creati;
novembre_c := m_fasc_chiusi;
ELSE
novembre := 0;
novembre_c := 0;
END IF;
END IF;

IF (i = 12)
THEN
/*Dicembre*/
IF (m_fasc_creati > 0 OR m_fasc_chiusi > 0)
THEN
dicembre := m_fasc_creati;
dicembre_c := m_fasc_chiusi;
ELSE
dicembre := 0;
dicembre_c := 0;
END IF;
END IF;

/*AGGIORNAMENTO DEL CONTATORE*/
i := i + 1;
END LOOP;


t_gennaio := t_gennaio + gennaio;
t_febbraio := t_febbraio + febbraio;
t_marzo := t_marzo + marzo;
t_aprile := t_aprile + aprile;
t_maggio := t_maggio + maggio;
t_giugno := t_giugno + giugno;
t_luglio := t_luglio + luglio;
t_agosto := t_agosto + agosto;
t_settembre := t_settembre + settembre;
t_ottobre := t_ottobre + ottobre;
t_novembre := t_novembre + novembre;
t_dicembre := t_dicembre + dicembre;
t_gennaio_c := t_gennaio_c + gennaio_c;
t_febbraio_c := t_febbraio_c + febbraio_c;
t_marzo_c := t_marzo_c + marzo_c;
t_aprile_c := t_aprile_c + aprile_c;
t_maggio_c := t_maggio_c + maggio_c;
t_giugno_c := t_giugno_c + giugno_c;
t_luglio_c := t_luglio_c + luglio_c;
t_agosto_c := t_agosto_c + agosto_c;
t_settembre_c := t_settembre_c + settembre_c;
t_ottobre_c := t_ottobre_c + ottobre_c;
t_novembre_c := t_novembre_c + novembre_c;
t_dicembre_c := t_dicembre_c + dicembre_c;
tot_vt := tot_vt + mese_vc;
tot_vt_c :=
t_gennaio_c
+ t_febbraio_c
+ t_marzo_c
+ t_aprile_c
+ t_maggio_c
+ t_giugno_c
+ t_luglio_c
+ t_agosto_c
+ t_settembre_c
+ t_ottobre_c
+ t_novembre_c
+ t_dicembre_c;
mese_vc_c := 0;
mese_vc := 0;

IF ((tot_vt = 0) AND (tot_vt_c = 0))
THEN
tot_vt_t := '-';
ELSE
tot_vt_t := TO_CHAR (tot_vt) || '/' || TO_CHAR (tot_vt_c);
END IF;

IF ((t_gennaio = 0) AND (t_gennaio_c = 0))
THEN
tot_gennaio := '-';
ELSE
tot_gennaio := TO_CHAR (t_gennaio) || '/' || TO_CHAR (t_gennaio_c);
END IF;

IF ((t_febbraio = 0) AND (t_febbraio_c = 0))
THEN
tot_febbraio := '-';
ELSE
tot_febbraio := TO_CHAR (t_febbraio) || '/'
|| TO_CHAR (t_febbraio_c);
END IF;

IF ((t_marzo = 0) AND (t_marzo_c = 0))
THEN
tot_marzo := '-';
ELSE
tot_marzo := TO_CHAR (t_marzo) || '/' || TO_CHAR (t_marzo_c);
END IF;

IF ((t_aprile = 0) AND (t_aprile_c = 0))
THEN
tot_aprile := '-';
ELSE
tot_aprile := TO_CHAR (t_aprile) || '/' || TO_CHAR (t_aprile_c);
END IF;

IF ((t_maggio = 0) AND (t_maggio_c = 0))
THEN
tot_maggio := '-';
ELSE
tot_maggio := TO_CHAR (t_maggio) || '/' || TO_CHAR (t_maggio_c);
END IF;

IF ((t_giugno = 0) AND (t_giugno_c = 0))
THEN
tot_giugno := '-';
ELSE
tot_giugno := TO_CHAR (t_giugno) || '/' || TO_CHAR (t_giugno_c);
END IF;

IF ((t_luglio = 0) AND (t_luglio_c = 0))
THEN
tot_luglio := '-';
ELSE
tot_luglio := TO_CHAR (t_luglio) || '/' || TO_CHAR (t_luglio_c);
END IF;

IF ((t_agosto = 0) AND (t_agosto_c = 0))
THEN
tot_agosto := '-';
ELSE
tot_agosto := TO_CHAR (t_agosto) || '/' || TO_CHAR (t_agosto_c);
END IF;

IF ((t_settembre = 0) AND (t_settembre_c = 0))
THEN
tot_settembre := '-';
ELSE
tot_settembre :=
TO_CHAR (t_settembre) || '/'
|| TO_CHAR (t_settembre_c);
END IF;

IF ((t_ottobre = 0) AND (t_ottobre_c = 0))
THEN
tot_ottobre := '-';
ELSE
tot_ottobre := TO_CHAR (t_ottobre) || '/' || TO_CHAR (t_ottobre_c);
END IF;

IF ((t_novembre = 0) AND (t_novembre_c = 0))
THEN
tot_novembre := '-';
ELSE
tot_novembre := TO_CHAR (t_novembre) || '/'
|| TO_CHAR (t_novembre_c);
END IF;

IF ((t_dicembre = 0) AND (t_dicembre_c = 0))
THEN
tot_dicembre := '-';
ELSE
tot_dicembre := TO_CHAR (t_dicembre) || '/'
|| TO_CHAR (t_dicembre_c);
END IF;

/*Aggiorniamo il cursore delle voci di Titolario*/
i := 1;

IF (num_livello1 = 1)
THEN
out_rec.fasc_creati := fasc_creati;
out_rec.fasc_chiusi := fasc_chiusi;
out_rec.var_cod := var_codice_livello1;
out_rec.var_descr := description__livello1;
out_rec.gennaio := tot_gennaio;
out_rec.febbraio := tot_febbraio;
out_rec.marzo := tot_marzo;
out_rec.aprile := tot_aprile;
out_rec.maggio := tot_maggio;
out_rec.giugno := tot_giugno;
out_rec.luglio := tot_luglio;
out_rec.agosto := tot_agosto;
out_rec.settembre := tot_settembre;
out_rec.ottobre := tot_ottobre;
out_rec.novembre := tot_novembre;
out_rec.dicembre := tot_dicembre;
out_rec.vt_fac_creati := tot_vt_t;
PIPE ROW (out_rec);
END IF;


tot_vt := 0;
tot_vt_c := 0;
gennaio := 0;
febbraio := 0;
marzo := 0;
aprile := 0;
maggio := 0;
giugno := 0;
luglio := 0;
agosto := 0;
settembre := 0;
ottobre := 0;
novembre := 0;
dicembre := 0;
t_gennaio := 0;
t_febbraio := 0;
t_marzo := 0;
t_aprile := 0;
t_maggio := 0;
t_giugno := 0;
t_luglio := 0;
t_agosto := 0;
t_settembre := 0;
t_ottobre := 0;
t_novembre := 0;
t_dicembre := 0;
t_gennaio_c := 0;
t_febbraio_c := 0;
t_marzo_c := 0;
t_aprile_c := 0;
t_maggio_c := 0;
t_giugno_c := 0;
t_luglio_c := 0;
t_agosto_c := 0;
t_settembre_c := 0;
t_ottobre_c := 0;
t_novembre_c := 0;
t_dicembre_c := 0;
END LOOP;

CLOSE c_vocitit;

RETURN;
EXCEPTION
WHEN OTHERS
THEN
RETURN;
END fascicolivttablefunction;
/