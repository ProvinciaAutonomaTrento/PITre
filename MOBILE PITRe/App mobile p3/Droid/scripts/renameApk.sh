#!/bin/bash 

# Script utilizzato per rinominare il file APK nel formato:
# > filename_VERSIONNAME.apk
# I parametri di input sono:
# - L'apk di input
# - La cartella di output
#
# Created by Francesco Florio
# francesco.florio@nttdata.com


echo "== RENAME THE ANDROID APK FILE =="

if [ $# -ne 3 ]; then
	echo "!! The script is in the format 'renameApk.sh {APK_FILE} {OUTPUT_PATH} {CONFIGURATION_PLIST_PATH}'"
	exit -1;
fi

CONFIGURATION_PLIST_PATH=$3

#Ottiene la cartella con la versione più nuova dei build-tools
buildToolsList=(`ls $ANDROID_HOME/build-tools | sort -rV`)
BUILD_TOOLS_VERSION=${buildToolsList[0]}
PROJECT_VERSION=`/usr/libexec/Plistbuddy -c "Print :DeploymentInformation:ProjectVersion" "$CONFIGURATION_PLIST_PATH"`


inputFile="$1"
outputPath="$2"
AAPT_PATH="$ANDROID_HOME/build-tools/$BUILD_TOOLS_VERSION"
fileName="PiTre"

if [[ $inputFile != *.apk ]]; then
	echo "!! The input file must be an APK"
	exit -1
fi

if [[ ! -d $outputPath ]]; then
    echo "!! The output path doesn't exist!"
	exit -1
fi

AAPT_EXEC="$AAPT_PATH/aapt"
if [[ ! -f $AAPT_EXEC ]]; then
    echo "!! aapt command not found!"
	exit -1
fi

# Retrieve version from application; If enabled check creation config.json 
# versionName=$($AAPT_EXEC dump badging "$inputFile" | awk '/package/{gsub("versionName=|'"'"'","");  print $4}')

mv "$inputFile" "${outputPath}/${fileName}_${PROJECT_VERSION}.apk"

echo "++ File ${outputPath}/${fileName}_${versionName}.apk created!"