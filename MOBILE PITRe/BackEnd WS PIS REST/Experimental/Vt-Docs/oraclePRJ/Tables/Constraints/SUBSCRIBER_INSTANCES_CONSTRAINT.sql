--------------------------------------------------------
--  Constraints for Table SUBSCRIBER_INSTANCES
--------------------------------------------------------

  ALTER TABLE "ITCOLL_6GIU12"."SUBSCRIBER_INSTANCES" ADD CONSTRAINT "PUBLISH_INSTANCES_PK" PRIMARY KEY ("ID") ENABLE;
 
  ALTER TABLE "ITCOLL_6GIU12"."SUBSCRIBER_INSTANCES" MODIFY ("NAME" NOT NULL ENABLE);
