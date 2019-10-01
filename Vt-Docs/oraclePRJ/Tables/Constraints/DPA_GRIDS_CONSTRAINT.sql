--------------------------------------------------------
--  Constraints for Table DPA_GRIDS
--------------------------------------------------------

  ALTER TABLE "ITCOLL_6GIU12"."DPA_GRIDS" ADD CONSTRAINT "PK_DPA_GRIDS" PRIMARY KEY ("SYSTEM_ID") ENABLE;
 
  ALTER TABLE "ITCOLL_6GIU12"."DPA_GRIDS" MODIFY ("SYSTEM_ID" NOT NULL ENABLE);
 
  ALTER TABLE "ITCOLL_6GIU12"."DPA_GRIDS" ADD CHECK (IS_SEARCH_GRID in ('Y','N')) ENABLE;
