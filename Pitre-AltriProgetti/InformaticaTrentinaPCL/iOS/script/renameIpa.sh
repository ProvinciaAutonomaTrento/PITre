#!/bin/bash   
# Script utilizzato per spostare e rinominare il file .ipa generato da Jenkins:
# Il file viene rinominato nella versione PiTre.ipa
# I parametri di input sono:
# - Il path in cui viene generato il file .ipa
# - Il path in cui è presente il file Info.plist che viene utilizzato per leggere informazioni riguardanti il progetto
# - Il path del file plist da riportare sull'FTP per la creazione della pagina web
# - Il path di destinazione in cui muovere il file rinominato 
# Modified by Davide Calò
# davide.calo@nttdata.com

echo "== RENAME THE iOS IPA FILE =="

if [ $# -ne 5 ]; then
	echo "!! The script is in the format 'renameIpa.sh {GENERATED_FILE_PATH} {INFO_PLIST_PATH} {RELEASE_PLIST_PATH} {OUTPUT_PATH} {CONFIGURATION_PLIST_PATH}"
	exit -1;
fi


GENERATED_FILE_PATH="$1" 
INFO_PLIST_PATH="$2"
RELEASE_PLIST_PATH="$3"
OUTPUT_FILE_PATH="$4"
CONFIGURATION_PLIST_PATH="$5"

if [[ $GENERATED_FILE_PATH != *.ipa ]]; then
	echo "$GENERATED_FILE_PATH! The input file must be an ipa"
	exit -1
fi

if [[ $INFO_PLIST_PATH != *.plist ]]; then
	echo "$INFO_PLIST_PATH! The input file must be a .plist file"
	exit -1
fi

if [[ ! -d $OUTPUT_FILE_PATH ]]; then
    echo "$OUTPUT_FILE_PATH! The output path doesn't exist!"
	exit -1
fi


if [[ ! -f $RELEASE_PLIST_PATH ]]; then
    echo "$RELEASE_PLIST_PATH! The info plist file doesn't exist!"
	exit -1
fi

if [[ ! -f $CONFIGURATION_PLIST_PATH ]]; then
    echo "$CONFIGURATION_PLIST_PATH! The info plist file doesn't exist!"
	exit -1
fi


BASE_FILE_NAME=`/usr/libexec/Plistbuddy -c "Print :DeploymentInformation:FileName" "$CONFIGURATION_PLIST_PATH"`

BUNDLE_IDENTIFIER=`/usr/libexec/Plistbuddy -c "Print CFBundleIdentifier" "$INFO_PLIST_PATH"`
echo "BUNDLE_IDENTIFIER:$BUNDLE_IDENTIFIER"
VERSION_NAME=`/usr/libexec/Plistbuddy -c "Print :DeploymentInformation:ProjectVersion" "$CONFIGURATION_PLIST_PATH"`
echo "VERSION_NAME:$VERSION_NAME"
BUILD=`/usr/libexec/Plistbuddy -c "Print CFBundleVersion $BUILD" "$INFO_PLIST_PATH"`
echo "BUILD:$BUILD"

PROJECT_TITLE=`/usr/libexec/Plistbuddy -c "Print :DeploymentInformation:ProjectTitle" "$CONFIGURATION_PLIST_PATH"`
echo "PROJECT_TITLE:$PROJECT_TITLE"
PROJECT_FTP_URL=`/usr/libexec/Plistbuddy -c "Print :DeploymentInformation:DestinationURL" "$CONFIGURATION_PLIST_PATH"`
echo "PROJECT_FTP_URL:$PROJECT_FTP_URL"


OUTPUT_FILE_NAME="${BASE_FILE_NAME}_${VERSION_NAME}.ipa"
echo "OUTPUT_FILE_NAME: $OUTPUT_FILE_NAME "
OUTPUT_PLIST_NAME="${BASE_FILE_NAME}_${VERSION_NAME}.plist"
echo "OUTPUT_PLIST_NAME: $OUTPUT_PLIST_NAME"

IPA_FTP_URL="${PROJECT_FTP_URL}/${OUTPUT_FILE_NAME}"
echo "IPA_FTP_URL: $IPA_FTP_URL"

/usr/libexec/Plistbuddy -c "Set :items:0:assets:0:url $IPA_FTP_URL" "$RELEASE_PLIST_PATH"
/usr/libexec/Plistbuddy -c "Set :items:0:metadata:bundle-identifier $BUNDLE_IDENTIFIER" "$RELEASE_PLIST_PATH"
/usr/libexec/Plistbuddy -c "Set :items:0:metadata:bundle-version $VERSION_NAME" "$RELEASE_PLIST_PATH"
/usr/libexec/Plistbuddy -c "Set :items:0:metadata:title $PROJECT_TITLE" "$RELEASE_PLIST_PATH"


mv "$GENERATED_FILE_PATH" "${OUTPUT_FILE_PATH}/${OUTPUT_FILE_NAME}"
mv "$RELEASE_PLIST_PATH" "${OUTPUT_FILE_PATH}/${OUTPUT_PLIST_NAME}"

echo "++ File ${OUTPUT_FILE_PATH}/${OUTPUT_FILE_NAME} created!"
echo "++ File ${OUTPUT_FILE_PATH}/${OUTPUT_PLIST_NAME} created!"
