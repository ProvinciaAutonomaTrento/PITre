--------------------------------------------------------
--  Ref Constraints for Table SUBSCRIBER_HISTORY
--------------------------------------------------------

  ALTER TABLE "ITCOLL_6GIU12"."SUBSCRIBER_HISTORY" ADD CONSTRAINT "FK_PUBLISH_RULEID" FOREIGN KEY ("RULEID")
	  REFERENCES "ITCOLL_6GIU12"."SUBSCRIBER_RULES" ("ID") ON DELETE CASCADE ENABLE NOVALIDATE;
