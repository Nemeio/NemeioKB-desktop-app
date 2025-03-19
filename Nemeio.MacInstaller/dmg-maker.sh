#!/bin/bash

# Arguments
# OPTIONAL --configuration : configuration to use. By default set to "Release"
# OPTIONAL --version : version to use. By default set to "0.0.0.0"

configuration="Release"
version="0.0.0.0"

POSITIONAL=()
while [[ $# -gt 0 ]]
do
key="$1"

case $key in
    --configuration)
    configuration="$2"
    shift # past argument
    shift # past value
    ;;
    --version)
    version="$2"
    shift # past argument
    shift # past value
    ;;
    *)
    POSITIONAL+=("$1")
    shift
    ;;
esac
done

set -- "${POSITIONAL[@]}" # restore positional parameters

echo "Started with configuration: <${configuration}>"

#   Create / Copy all required files
#   ===========================================

#   Create symlink to Applications folder
ln -s /Applications "./Artefacts/${configuration}/Applications"

#   Copy DS_Store from Resources
cp "./Nemeio.MacInstaller/Resources/dmg_ds_store" "./Artefacts/${configuration}/.DS_Store"

#   Create background directory
mkdir "./Artefacts/${configuration}/.background"

#   Copy backgrounds from Resources
cp "./Nemeio.MacInstaller/Resources/dmg-background.png" "./Artefacts/${configuration}/.background/dmg-background.png"
cp "./Nemeio.MacInstaller/Resources/dmg-background@2x.png" "./Artefacts/${configuration}/.background/dmg-background@2x.png"

#   Create DMG file
#   ===========================================
hdiutil create "./Artefacts/${configuration}/Nemeio-${version}.dmg" -volname "Nemeio" -fs HFS+ -srcfolder "./Artefacts/${configuration}/"

#   Clean tmp files
#   ===========================================
rm "./Artefacts/${configuration}/Applications"
rm "./Artefacts/${configuration}/.DS_Store"
rm -rf "./Artefacts/${configuration}/.background"
