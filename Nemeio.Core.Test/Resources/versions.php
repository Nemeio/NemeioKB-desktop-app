<?php

    function getMD5($filename) {
        return md5_file($filename);
    }

    $stm32 = "b1994-nemeio-firmware.zip";
    $osx = "Stickies.app.zip";
    $windows = "Release.zip";

?>

{
    "cpu": {
        "version" : "2.0.0.0",
        "url" : "http://colomba.moessner.fr/<?php echo $stm32; ?>",
        "checksum": "<?php echo getMD5("./".$stm32); ?>"
    },
    "ble": {
        "version" : "2.0.0.0",
        "url" : "http://colomba.moessner.fr/fake.nrf.zip",
        "checksum": "<?php echo getMD5("./fake.nrf.zip"); ?>"
    },
    "osx": {
        "version" : "2.0.0.0",
        "url" : "http://colomba.moessner.fr/<?php echo $osx; ?>",
        "checksum": "<?php echo getMD5("./".$osx); ?>"
    },
    "windows": {
        "version" : "2.0.0.0",
        "url" : "http://colomba.moessner.fr/<?php echo $windows; ?>",
        "checksum": "<?php echo getMD5("./".$windows); ?>"
    }
}