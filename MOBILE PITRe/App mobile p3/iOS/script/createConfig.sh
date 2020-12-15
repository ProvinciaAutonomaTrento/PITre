#!/bin/bash  

if [ $# -ne 1 ]; then
	echo "!! The script is in the format 'createConfig.sh {CONFIGURATION_PLIST_PATH}"
	exit -1;
fi

CONFIGURATION_PLIST_PATH="$1"
PROJECT_VERSION=`/usr/libexec/Plistbuddy -c "Print :DeploymentInformation:ProjectVersion" "$CONFIGURATION_PLIST_PATH"`
FTP_URL=`/usr/libexec/Plistbuddy -c "Print :DeploymentInformation:DestinationURL" "$CONFIGURATION_PLIST_PATH"`
PROJECT_TITLE=`/usr/libexec/Plistbuddy -c "Print :DeploymentInformation:ProjectTitle" "$CONFIGURATION_PLIST_PATH"`

JSON_STRING="{
	\"title\" : \"${PROJECT_TITLE}\",
	\"app\" : ["

 
declare -a FILTERED_FILES

for FILE in *.*; do 

if [[ $FILE == *".plist"* ]]; then
	# echo "Founded ${FILE}"
  FILTERED_FILES+=($FILE)
elif [[ $FILE == *".apk"* ]]; then
	# echo "Founded ${FILE}"
	 FILTERED_FILES+=($FILE)
fi
done

# echo "FILTERED_FILES ${FILTERED_FILES[@]}"

POS=$(( ${#FILTERED_FILES[@]} - 1 ))
LAST=${FILTERED_FILES[$POS]}
LAST_VALUE=`basename $LAST`


for FILE in "${FILTERED_FILES[@]}"
do 
	echo "Found $FILE"

	VERSION_VALUE="$PROJECT_VERSION"
	NAME_VALUE="$PROJECT_TITLE"
	PATH_VALUE="$FTP_URL/$FILE"

if [[ $FILE == *".plist"* ]]; then
	
	CURRENT_PLIST_PATH="./$FILE"

	TYPE_VALUE="ios"
 	NAME_VALUE+=" iOS"

elif [[ $FILE == *".apk"* ]]; then
	
	TYPE_VALUE="android"
	NAME_VALUE+=" Android"

fi

  if [[ $FILE == $LAST_VALUE ]]
  then
     echo "$FILE is the last" 
     JSON_STRING+="$( jq -n \
                  --arg bucketName "$TYPE_VALUE" \
                  --arg objectName "$VERSION_VALUE" \
                  --arg targetLocation "$NAME_VALUE" \
                  --arg path "$PATH_VALUE" \
                  '{type: $bucketName, version: $objectName, name: $targetLocation, path:$path }')]}"
     break
  else
  	 echo "$FILE"
  JSON_STRING+="$( jq -n \
                  --arg bucketName "$TYPE_VALUE" \
                  --arg objectName "$VERSION_VALUE" \
                  --arg targetLocation "$NAME_VALUE" \
                  --arg path "$PATH_VALUE" \
                  '{type: $bucketName, version: $objectName, name: $targetLocation, path:$path }'),"

  fi 
done 

echo $JSON_STRING

echo $JSON_STRING > config.json
 

