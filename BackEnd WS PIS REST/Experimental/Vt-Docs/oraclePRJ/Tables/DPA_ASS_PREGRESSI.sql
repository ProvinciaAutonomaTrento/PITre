CREATE TABLE @db_user.DPA_ASS_PREGRESSI 
            ( 
			"SYSTEM_ID"			INTEGER , 
            "ID_PREGRESSO" 		INTEGER , 
            "ID_REGISTRO" 		INTEGER , 
			"ID_DOCUMENTO" 		INTEGER , 
			"ID_UTENTE" 			INTEGER , 
			"ID_RUOLO" 			INTEGER , 
			"TIPO_OPERAZIONE" 	CHAR(1 BYTE), 
			"DATA" 				DATE, 
			"ERRORE" 			VARCHAR2(1000 BYTE), 
			"ESITO" 				CHAR(1 BYTE), 
			"ID_NUM_PROTO_EXCEL" VARCHAR2(255 BYTE)     
            );
			/