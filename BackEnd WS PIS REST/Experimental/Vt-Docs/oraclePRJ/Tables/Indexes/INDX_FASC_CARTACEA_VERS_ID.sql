--------------------------------------------------------
--  DDL for Index INDX_FASC_CARTACEA_VERS_ID
--------------------------------------------------------

  CREATE INDEX "ITCOLL_6GIU12"."INDX_FASC_CARTACEA_VERS_ID" ON "ITCOLL_6GIU12"."DPA_FASCICOLAZIONE_CARTACEA" ("VERSION_ID") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS NOCOMPRESS LOGGING
  STORAGE( INITIAL 65536
  FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "PITRE_INFOTN_DATA_COLL" ;