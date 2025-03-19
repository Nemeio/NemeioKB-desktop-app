namespace Nemeio.Core.Errors
{
    public enum ErrorCode
    {
        //  === Default status

        [ErrorDescription("Success")]
        Success = 0,

        //  === Core

        [ErrorDescription("Open application failed")]
        CoreOpenApplicationFailed = 0x02010101,

        [ErrorDescription("Open url failed")]
        CoreOpenUrlFailed = 0x02010102,

        [ErrorDescription("Synchronisation failed")]
        CoreSynchronizationFailed = 0x02010201,

        [ErrorDescription("Layout watcher failed")]
        CoreLayoutWatcherFailed = 0x02010202,

        [ErrorDescription("Retrieve blacklist failed")]
        CoreFindExceptionFromBlacklistFailed = 0x02010301,

        [ErrorDescription("Invalid updater")]
        CoreUpdateUpdaterInvalid = 0x02010401,

        [ErrorDescription("Delete installer failed")]
        CoreUpdateDeleteInstallerFailed = 0x02010402,

        [ErrorDescription("Installer file not found")]
        CoreUpdateInstallerFileNotFound = 0x02010403,

        [ErrorDescription("Apply update failed")]
        CoreUpdateApplyFailed = 0x02010404,

        [ErrorDescription("Manifest.json file not found")]
        CoreUpdateNrfManifestFileNotFound = 0x02010405,

        [ErrorDescription("Desktop application update unexpected failure")]
        CoreUpdateDesktopApplicationUnexpectedFailure = 0x02010406,

        [ErrorDescription("Update download failed")]
        CoreUpdateDownloadFailed = 0x02010407,

        [ErrorDescription("Zip file not found")]
        CoreUpdateZipFileNotFound = 0x02010408,

        [ErrorDescription("Read temp update file failed")]
        CoreUpdateReadTempUpdateFileFailed = 0x02010409,

        [ErrorDescription("Nrf update missing part")]
        CoreUpdateNrfPartMissing = 0x0201040A,

        [ErrorDescription("Checksum invalid")]
        CoreUpdateChecksumInvalid = 0x0201040B,

        [ErrorDescription("Layout synchronization from database failed")]
        CoreLayoutSynchronizationFromDatabaseFailed = 0x02010501,

        [ErrorDescription("Layout validity check before save failed")]
        CoreLayoutValidationBeforeSaveFailed = 0x02010502,

        [ErrorDescription("Layout validity check at load failed")]
        CoreLayoutValidationAfterLoadFailed = 0x02010503,

        [ErrorDescription("Layout definitive validation failed")]
        CoreLayoutDefinitiveValidationFailed = 0x02010504,

        [ErrorDescription("Invalid layout id")]
        CoreExportLayoutInvalidId = 0x02010510,

        [ErrorDescription("Layout is not found")]
        CoreExportLayoutIsNotFound = 0x02010511,

        [ErrorDescription("Specified layout is HID : export is forbidden")]
        CoreExportLayoutHidForbidden = 0x02010512,

        [ErrorDescription("Layout Export version is too old")]
        CoreImportLayoutInvalidExportVersion = 0x02010521,

        [ErrorDescription("Layout import title is empty")]
        CoreImportLayoutTitleEmpty = 0x02010522,

        [ErrorDescription("Layout import title already used")]
        CoreImportLayoutTitleAlreadyUsed = 0x02010523,

        [ErrorDescription("Layout import invalid associated layout id")]
        CoreImportLayoutInvalidAssociatedLayoutId = 0x02010524,

        [ErrorDescription("Layout import associated layout not found")]
        CoreImportLayoutMissingAssociatedLayout = 0x02010525,

        [ErrorDescription("Load development settings failed")]
        CoreLoadDevelopmentSettingsFailed = 0x02010601,

        [ErrorDescription("Load required fonts failed")]
        CoreLoadRequiredFontFailed = 0x02010701,

        [ErrorDescription("Set specific layout failed")]
        CoreSetSpecificLayoutFailed = 0x02010526,

        //  === Database

        [ErrorDescription("Can't retrieve protected key")]
        DatabaseRetrieveProtectedKeyFailed = 0x02020101,

        [ErrorDescription("Save protected key failed")]
        DatabaseSaveProtectedKeyFailed = 0x02020102,

        [ErrorDescription("Keychain access denied")]
        DatabaseKeychainAccessDenied = 0x02020103,

        [ErrorDescription("Database's file not found")]
        DatabaseFileNotFound = 0x02020201,

        [ErrorDescription("Delete database file failed")]
        DatabaseFileDeletionFailed = 0x02020202,

        [ErrorDescription("Data migration failed")]
        DatabaseMigrationFailed = 0x02020301,

        [ErrorDescription("Can't load database")]
        DatabaseLoadFailed = 0x02020401,

        //  === Api

        [ErrorDescription("Invalid parameters")]
        ApiInvalidParameters = 0x02030001,

        //  Blacklist
        [ErrorDescription("Not Found")]
        ApiPostBlacklistNotFound = 0x02030101,

        [ErrorDescription("Blacklist id not found")]
        ApiDeleteBlacklistIdNotFound = 0x02030102,

        [ErrorDescription("Try to delete system blacklist : forbidden")]
        ApiDeleteBlacklistSystemForbidden = 0x02030103,

        //  Category
        [ErrorDescription("Category not found")]
        ApiGetCategoryIdNotFound = 0x02030201,

        [ErrorDescription("Category not found")]
        ApiPutCategoryNotFound = 0x02030202,

        [ErrorDescription("Category not found")]
        ApiDeleteCategoryIdNotFound = 0x02030203,

        [ErrorDescription("Specified category is not found : forbidden")]
        ApiDeleteDefaultCategoryForbidden = 0x02030204,

        //  Layout
        [ErrorDescription("Option are invalid")]
        ApiGetLayoutOptionInvalid = 0x02030301,

        [ErrorDescription("Layout is not found")]
        ApiGetLayoutIdNotFound = 0x02030302,

        [ErrorDescription("Layout not found")]
        ApiPutLayoutIdNotFound = 0x02030303,

        [ErrorDescription("Category id specified is incorrect")]
        ApiPutLayoutCategoryIdInvalid = 0x02030304,

        [ErrorDescription("Data format / content are invalid")]
        ApiPutLayoutDataContentInvalid = 0x02030305,

        [ErrorDescription("Layout name is empty")]
        ApiPutLayoutNameEmpty = 0x02030306,

        [ErrorDescription("Layout name already used")]
        ApiPutLayoutNameAlreadyUsed = 0x02030307,

        [ErrorDescription("Invalid selected app file")]
        ApiPutLayoutNotExecutableFile = 0x02030308,

        [ErrorDescription("Invalid path")]
        ApiPutLayoutInvalidPath = 0x02030309,

        [ErrorDescription("Layout is not found")]
        ApiDeleteLayoutIdNotFound = 0x0203030A,

        [ErrorDescription("Specified layout is HID : deletion forbidden")]
        ApiDeleteHidLayoutForbidden = 0x0203030B,

        [ErrorDescription("Layout not found (HID)")]
        ApiSetDefaultLayoutIdNotFound = 0x0203030C,

        [ErrorDescription("Specified template not found")]
        ApiPostLayoutTemplateIdNotFound = 0x0203030E,

        [ErrorDescription("Image name are invalid")]
        ApiGetHidImageNameInvalid = 0x0203030F,

        [ErrorDescription("Image not found")]
        ApiGetHidImageNotFound = 0x02030310,

        [ErrorDescription("Id is invalid")]
        ApiUpdateLayoutKeyIdInvalid = 0x02030311,

        [ErrorDescription("Key index is out of range")]
        ApiUpdateLayoutKeyIndexInvalid = 0x02030312,

        [ErrorDescription("Specified layout not found")]
        ApiUpdateLayoutKeyLayoutNotFound = 0x02030313,

        [ErrorDescription("Specified layout is HID : update is forbidden")]
        ApiUpdateLayoutKeyHidLayoutForbidden = 0x02030314,

        [ErrorDescription("Specified key is locked : update forbidden")]
        ApiUpdateLayoutKeyUpdateKeyFailed = 0x02030315,

        [ErrorDescription("Specified key is not found")]
        ApiUpdateLayoutKeyNotFound = 0x0203031D,

        [ErrorDescription("Selected file isn't an executable")]
        ApiUpdateLayoutKeyApplicationInvalid = 0x02030322,

        [ErrorDescription("Invalid path selected : doesn't exists on the current computer")]
        ApiUpdateLayoutKeyPathInvalid = 0x02030323,

        [ErrorDescription("Invalid Http Url")]
        ApiUpdateLayoutKeyUrlInvalid = 0x02030324,

        [ErrorDescription("Specified id is invalid")]
        ApiCommitUiIdInvalid = 0x02030325,

        [ErrorDescription("Specified layout not found")]
        ApiCommitUiLayoutNotFound = 0x02030326,

        [ErrorDescription("Specified layout is HID : recreate image is forbidden")]
        ApiCommitUiHidLayoutForbidden = 0x02030327,

        [ErrorDescription("Invalid font")]
        ApiUpdateLayoutKeyFontInvalid = 0x02030328,

        [ErrorDescription("Modifier is locked")]
        ApiUpdateLayoutKeyModifierIsLocked = 0x02030329,

        [ErrorDescription("Layout is not found")]
        ApiDuplicateLayoutIsNotFound = 0x0203032A,

        [ErrorDescription("Specified layout is HID : duplication is forbidden")]
        ApiDuplicateLayoutHidForbidden = 0x0203032B,

        [ErrorDescription("Duplication failed")]
        ApiDuplicationLayoutFailed = 0x0203032C,

        [ErrorDescription("Layout name is already used")]
        ApiDuplicationLayoutNameAlreadyUsed = 0x0203032D,

        [ErrorDescription("Invalid layout id")]
        ApiExportLayoutInvalidId = 0x02030330,

        [ErrorDescription("Layout is not found")]
        ApiExportLayoutIsNotFound = 0x02030331,

        [ErrorDescription("Specified layout is HID : export is forbidden")]
        ApiExportLayoutHidForbidden = 0x02030332,

        [ErrorDescription("Layout Export version is too old")]
        ApiImportLayoutInvalidExportVersion = 0x02030333,

        [ErrorDescription("Layout import title is empty")]
        ApiImportLayoutTitleEmpty = 0x02030334,

        [ErrorDescription("Layout import title already used")]
        ApiImportLayoutTitleAlreadyUsed = 0x02030335,

        [ErrorDescription("Layout import invalid associated layout id")]
        ApiImportLayoutInvalidAssociatedLayoutId = 0x02030336,

        [ErrorDescription("Layout import associated layout not found")]
        ApiImportLayoutMissingAssociatedLayout = 0x02030337,

        [ErrorDescription("Specified AssociatedId is invalid")]
        ApiPutAssociatedIdInvalid = 0x02030304,

        //  Keyboard
        [ErrorDescription("Invalid parameters")]
        ApiPostKeyboardParametersInvalidParameters = 0x02030401,

        [ErrorDescription("Invalid format (e.g. event type is higher than expected)")]
        ApiPostKeyboardParametersInvalidFormat = 0x02030402,

        [ErrorDescription("Keyboard is not plugged")]
        ApiKeyboardNotPlugged = 0x02030403,

        //  === Communication

        [ErrorDescription("Http request failed")]
        AclHttpRequestFailed = 0x02040101,

        [ErrorDescription("Unexpected")]
        AclKeyboardResponseUnexpected = 0x02040201,

        [ErrorDescription("Data malformed")]
        AclKeyboardResponseDataMalformed = 0x02040202,

        [ErrorDescription("File system failure")]
        AclKeyboardResponseFileSystemFailure = 0x02040203,

        [ErrorDescription("State")]
        AclKeyboardResponseStateInvalid = 0x02040204,

        [ErrorDescription("Invalid content")]
        AclKeyboardResponseContentInvalid = 0x02040205,

        [ErrorDescription("Not found")]
        AclKeyboardResponseNotFound = 0x02040206,

        [ErrorDescription("Draw configuration")]
        AclKeyboardResponseDrawConfiguration = 0x02040207,

        [ErrorDescription("Protected configuration")]
        AclKeyboardResponseConfigurationProtected = 0x02040208,

        [ErrorDescription("Stream data protocol")]
        AclKeyboardResponseStreamDataProtocolFailed = 0x2040209,

        [ErrorDescription("Send data nomenclature")]
        AclKeyboardResponseSendDataNomenclatureFailed = 0x0204020A,

        [ErrorDescription("Firmware update failed")]
        AclKeyboardResponseFirmwareUpdateFailed = 0x0204020B,

        [ErrorDescription("Firmware update missing flash")]
        AclKeyboardResponseFirmwareUpdateMissingFlash = 0x0204020C,

        [ErrorDescription("The new firmware version is not compatible with the current keyboard components.")]
        AclKeyboardResponseFirmwareUpdateVersionInvalid = 0x0204020D,

        [ErrorDescription("Firmware update write failed")]
        AclKeyboardResponseFirmwareUpdateWriteFailed = 0x0204020E,

        [ErrorDescription("Ble chip communication error")]
        AclKeyboardResponseBleChipCommunicationError = 0x0204020F,

        [ErrorDescription("Empty or invalid payload")]
        AclKeyboardResponsePayloadEmptyOrInvalid = 0x02040210,

        [ErrorDescription("Connection command(s) failed")]
        AclKeyboardConnectionCommandFailed = 0x02040301,

        [ErrorDescription("Connection failed")]
        AclKeyboardConnectionFailedUnknown = 0x02040302,

        [ErrorDescription("Battery status information is not yet available.")]
        AclKeyboardResponseBatteryIsNoReady = 0x02040303,

        [ErrorDescription("Error when retrieving battery status information from the Fuel Gauge")]
        AclKeyboardResponseBatteryFuelGaugeFailed = 0x02040304,

        [ErrorDescription("The screen is not ready for order processing.")]
        AclKeyboardResponseDisplayNotReady = 0x02040305,

        //  === Windows

        [ErrorDescription("Get process id failed")]
        WindowsGetProcessIdFailed = 0x02060101,

        [ErrorDescription("Read file from Isolated Storage failed")]
        WindowsReadFileFromIsolatedStorageFailed = 0x02060201,

        [ErrorDescription("Write file from Isolated Storage failed")]
        WindowsWriteFileFromIsolatedStorageFailed = 0x02060202,

        [ErrorDescription("Release NUS Failed")]
        WindowsBluetoothReleaseNusFailed = 0x02060301,

        [ErrorDescription("No bluetooth capability found")]
        WindowsBluetoothCapabilityNotFound = 0x02060302,

        [ErrorDescription("Check nemeio device failed")]
        WindowsBluetoothCheckNemeioDeviceFailed = 0x02060303,

        //  === Mac

        [ErrorDescription("Read key from Keychain failed")]
        MacReadKeyFromKeychainFailed = 0x02070201,

        [ErrorDescription("Write key from Keychain failed")]
        MacWriteKeyFromKeychainFailed = 0x02070202,

        [ErrorDescription("No bluetooth capability found")]
        MacBluetoothCapabilityNotFound = 0x02070301,
    }
}
