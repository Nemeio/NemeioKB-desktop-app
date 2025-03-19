#!/bin/bash

# The main goal of this script is to create OSX package to install Nemeio

# THIS SCRIPT CAN ONLY BE EXECUTED ON MACOS

# Arguments
# REQUIRED --certificat : Developer ID Installer certificat name
# REQUIRED --version : current Nemeio's version
# REQUIRED --outputName : package output name
# OPTIONAL --applicationPath : path to the ".app" file
# OPTIONAL --appBundleId : bundle id of the package's application
# OPTIONAL --installerBundleId : package's bundle id

# Exit code
# 0 : Success
# 1 : Required certificat parameter
# 2 : Required version parameter
# 3 : Required outputName parameter

# Example
#bash pkg-maker.sh \
#--certificat "Developer ID Installer: ADETEL EMBEDDED (Q7R7F3ZZDC)" \
#--version "0.2.43.40528" \
#--outputName "Nemeio"

# Constants
CURDIR="$(dirname "$0")"
NB_STEPS=4
ERROR_CODE_CERTIFICATE_PARAMETER=1
ERROR_CODE_VERSION_PARAMETER=2
ERROR_CODE_OUTPUT_NAME_PARAMETER=3

# Parameters
CERTIFICAT=-1
VERSION=-1
OUTPUT_NAME=-1
APP_PATH="${CURDIR}/../Nemeio.Mac/bin/Release/Nemeio.app"
APPLICATION_BUNDLE_ID="com.witekio.karmeliet"
INSTALLER_BUNDLE_ID="com.witekio.karmeliet.installer"
CURRENT_STEP=1

# Declare reuseable functions
function log() {
    echo ""
    echo "[${1}] ${2}"
}

function logInfo() {
    log "INFO" "${1}"
}

function logError() {
    log "ERROR" "${1}"
}

function logStepInfo() {
    logInfo "[${CURRENT_STEP}/${NB_STEPS}] ${1}"
}

function nextStep() {
    CURRENT_STEP=$(($CURRENT_STEP+1))
}

function realpath() {
  OURPWD=$PWD
  cd "$(dirname "$1")"
  LINK=$(readlink "$(basename "$1")")
  while [ "$LINK" ]; do
    cd "$(dirname "$LINK")"
    LINK=$(readlink "$(basename "$1")")
  done
  REALPATH="$PWD/$(basename "$1")"
  cd "$OURPWD"
  echo "$REALPATH"
}

# Manage script's parameter
POSITIONAL=()
while [[ $# -gt 0 ]]
do
key="$1"

case $key in
    --certificat)
    CERTIFICAT="$2"
    shift # past argument
    shift # past value
    ;;
    --version)
    VERSION="$2"
    shift # past argument
    shift # past value
    ;;
    --outputName)
    OUTPUT_NAME="$2"
    shift # past argument
    shift # past value
    ;;
    --applicationPath)
    APP_PATH="$2"
    shift # past argument
    shift # past value
    ;;
    --appBundleId)
    APPLICATION_BUNDLE_ID="$2"
    shift # past argument
    shift # past value
    ;;
    --installerBundleId)
    INSTALLER_BUNDLE_ID="$2"
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

logInfo "Start script 'pkg-maker'"

# Check parameters value
if [ $CERTIFICAT == -1 ] 
then
    logError "--certificat argument is required"
    exit $ERROR_CODE_CERTIFICATE_PARAMETER
fi

if [ $VERSION == -1 ] 
then
    logError "--version argument is required"
    exit $ERROR_CODE_VERSION_PARAMETER
fi

if [ $OUTPUT_NAME == -1 ] 
then
    logError "--outputName argument is required"
    exit $ERROR_CODE_OUTPUT_NAME_PARAMETER
fi

function addExecutableAttributes() {
    chmod u+x "${CURDIR}/Pkg/Scripts/preinstall"
    chmod u+x "${CURDIR}/Pkg/Scripts/postinstall"
}

function createDistributionFile() {
    distributionFilePath=$(realpath "${CURDIR}/Pkg/distribution.template.xml")

    echo "distribution.xml path '${distributionFilePath}'"

    export APP_VERSION="${VERSION}"
    export APP_TITLE="${OUTPUT_NAME}"
    export APP_BUNDLE_ID="${APPLICATION_BUNDLE_ID}"
    export INSTALLER_BUNDLE_ID="${INSTALLER_BUNDLE_ID}"

    envsubst < "${distributionFilePath}" > "${CURDIR}/Pkg/distribution.xml"
}

function createApplicationPackage() {
    pkgbuild \
    --root "${APP_PATH}" \
    --install-location "/Applications/${OUTPUT_NAME}.app" \
    --identifier "com.witekio.karmeliet.installer" \
    --scripts "${CURDIR}/Pkg/Scripts/" \
    --version "${VERSION}" \
    --sign "${CERTIFICAT}" \
    "${CURDIR}/Nemeio.pkg"
}

function createProductPackage() {
    productbuild \
    --distribution "${CURDIR}/Pkg/distribution.xml" \
    --resources "${CURDIR}/Pkg/Resources/" \
    --package-path "${CURDIR}" \
    --sign "${CERTIFICAT}" \
    "${CURDIR}/${OUTPUT_NAME}-${VERSION}.pkg"
}

# Start script
# --------------------------------------------------

logInfo "Selected application file : ${APP_PATH}"

# Enable execution on each package scripts
logStepInfo "Add executable to scripts"
addExecutableAttributes
nextStep

# Replace template values on distribution.xml
logStepInfo "Replace variables in distribution.xml"
createDistributionFile
nextStep

# Create package for app
logStepInfo "Start build package"
createApplicationPackage
nextStep

# Build final package with disribution
logStepInfo "Start build installer"
createProductPackage

logInfo "Build PKG finished"

exit 0