#!/bin/bash   
# Script utilizzato per aggiornare le versioni delle applicazioni in base al valore inserito nel file di confgiruazione:
# I parametri di input sono:
# - Il path in del file di configurazione (ConfigurationRelease.plist); 
# - Il path del progetto iOS;
# - Il path del progetto Android;
# Created by Davide Calò
# davide.calo@nttdata.com

echo "== UPDATE APPLICATION VERSION =="

if [ $# -ne 3 ]; then
	echo "!! The script is in the format 'updateApplicationVersion.sh {CONFIGURATION_PLIST_PATH} {IOS_PROJECT_PATH} {ANDROID_PROJECT_PATH}"
	echo "Insert at least one between ANDROID_PROJECT_PATH and IOS_PROJECT_PATH"
	echo "If you have only one project, set \"\" path for other"
	exit -1;
fi

CONFIGURATION_PLIST_PATH="$1" 
IOS_PROJECT_PATH="$2"
ANDROID_PROJECT_PATH="$3"


APPLICATION_VERSION=`/usr/libexec/Plistbuddy -c "Print :DeploymentInformation:ProjectVersion" "$CONFIGURATION_PLIST_PATH"`
BUILD_NUMBER=`/usr/libexec/Plistbuddy -c "Print :DeploymentInformation:BuildNumber" "$CONFIGURATION_PLIST_PATH"`

echo "APPLICATION_VERSION retrieved from ConfigurationRelease.plist: $APPLICATION_VERSION"
echo "BUILD_NUMBER retrieved from ConfigurationRelease.plist: $BUILD_NUMBER"

if [ -d "$IOS_PROJECT_PATH" ]; then
	IOS_INFO_PLIST_PATH="$IOS_PROJECT_PATH/Info.plist"

	/usr/libexec/Plistbuddy -c "Set :CFBundleShortVersionString $APPLICATION_VERSION" "$IOS_INFO_PLIST_PATH"
	/usr/libexec/Plistbuddy -c "Set :CFBundleVersion $BUILD_NUMBER" "$IOS_INFO_PLIST_PATH"

	echo "update Version & BuildNumber for iOS"

fi

if [ -d "$ANDROID_PROJECT_PATH" ]; then
	ANDROID_MANIFEST_PATH="$ANDROID_PROJECT_PATH/Properties/AndroidManifest.xml"

	sed -i.bak 's/android:versionName="._"/android:versionName="${APPLICATION_VERSION}"/g' $ANDROID_MANIFEST_PATH
	sed -i.bak 's/android:versionCode="._"/android:versionCode="${BUILD_NUMBER}"/g' $ANDROID_MANIFEST_PATH

	echo "update Version & BuildNumber for Android"
fi

exit 0
