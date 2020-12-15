#!/bin/bash  

# Script da utilizzare per configurare il mobile provisioning iOS sui nodi Jenkins.
# Questo script va avviato nella pipeline di build iOS, prima di lanciare la build.
# Lo script va a controllare se esiste nel nodo il mobileprovisioning utilizzato nel progetto. 
# Attenzione: Il .mobileprovisioning deve essere inserito all'interno del repository GIT, in modo da consentire
# l'installazione e il controllo sulla versione.
# Infatti se il file non esiste oppure è diverso da quello presente nel nodo Jenkins, lo script
# effettua la copia. Altrimenti non fa nulla.
#
# Attenzione 2: Il mobileprovisioning deve essere collegato ad un certificato presente 
# nel nodo Jenkins. 
# Per Informatica Trentina stiamo utilizzando il certificato di distribuzione 
# NTT DATA Italia scad. 13 Jun 2021 
#
# Created by Francesco Florio
# francesco.florio@nttdata.com
echo "== Configure mobile provisioning == "

#Input to config
PROVISIONING_FILE=Informatica_Trentina_Distribution.mobileprovision

SCRIPT_FOLDER=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )
PRJ_PROVISIONING_FOLDER=$SCRIPT_FOLDER

PROVISIONING_FOLDER=~/Library/MobileDevice/Provisioning\ Profiles
PRJ_MOBILE_PROVISIONING="$PRJ_PROVISIONING_FOLDER/$PROVISIONING_FILE"
SYS_MOBILE_PROVISIONING="$PROVISIONING_FOLDER/$PROVISIONING_FILE"

echo "++ Mobile Provisioning to use is '$PROVISIONING_FILE'"

if [[ -f $SYS_MOBILE_PROVISIONING ]]; then
   	echo "++ File exists, check if they are the same."
   	diff "$SYS_MOBILE_PROVISIONING" "$PRJ_MOBILE_PROVISIONING"
    
    DIFF_VALUE=$(echo $?)
   	if [ $DIFF_VALUE -ne 0 ]; then
   		cp "$PRJ_MOBILE_PROVISIONING" "$PROVISIONING_FOLDER"
   		echo "!! Provisionings replaced!"
    else
    	echo "++ Provisionings are the same, nothing to do"
    fi
else
	echo "!! Provisioning not found, I'm copying it into the system folder"
    cp "$PRJ_MOBILE_PROVISIONING" "$PROVISIONING_FOLDER" 
fi
