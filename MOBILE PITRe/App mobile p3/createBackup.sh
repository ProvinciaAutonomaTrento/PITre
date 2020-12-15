#!/bin/bash   

echo "CREATE BACKUP ARCHIVE"


CURRENT_DATE=`date '+%Y%m%d%H%M'`

cd "$WORKSPACE/Releases"

ARCHIVE_FILE_NAME="PiTre_${CURRENT_DATE}"

zip -9 $ARCHIVE_FILE_NAME.zip * .*

ls -l
mv ./$ARCHIVE_FILE_NAME.zip ./Backup