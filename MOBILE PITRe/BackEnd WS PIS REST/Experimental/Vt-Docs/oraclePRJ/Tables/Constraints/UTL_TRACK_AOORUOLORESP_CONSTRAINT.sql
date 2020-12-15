--------------------------------------------------------
--  Constraints for Table UTL_TRACK_AOORUOLORESP
--------------------------------------------------------

  ALTER TABLE "ITCOLL_6GIU12"."UTL_TRACK_AOORUOLORESP" ADD CHECK (Cha_changed_notyet_processed  In ('y','n') ) ENABLE;
