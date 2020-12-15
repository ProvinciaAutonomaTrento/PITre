--------------------------------------------------------
--  DDL for Table UTL_TRACK_AOORUOLORESP
--------------------------------------------------------

  CREATE TABLE "ITCOLL_6GIU12"."UTL_TRACK_AOORUOLORESP" 
   (	"ID_REGISTRO" NUMBER(*,0), 
	"OLD_AOORUOLORESP" NUMBER(*,0), 
	"NEW_AOORUOLORESP" NUMBER(*,0), 
	"CHA_CHANGED_NOTYET_PROCESSED" CHAR(1 BYTE) DEFAULT 'y', 
	"DTA_CHANGED" DATE
   ) ;
