--------------------------------------------------------
--  DDL for Table SUBSCRIBER_RULES
--------------------------------------------------------

  CREATE TABLE "ITCOLL_6GIU12"."SUBSCRIBER_RULES" 
   (	"ID" NUMBER(10,0), 
	"INSTANCEID" NUMBER(10,0), 
	"NAME" NVARCHAR2(255), 
	"DESCRIPTION" NVARCHAR2(2000), 
	"ENABLED" CHAR(1 BYTE), 
	"ORDINAL" NUMBER(*,0), 
	"OPTIONS" NVARCHAR2(2000), 
	"PARENTRULEID" NUMBER(10,0), 
	"CLASS_ID" NVARCHAR2(2000), 
	"SUBNAME" NVARCHAR2(255)
   ) ;
