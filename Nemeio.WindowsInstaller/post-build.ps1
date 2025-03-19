# This script need some parameters
#   -MsiName
#   -Configuration
#   -Platform

# It represents the current name of MSI (e.g. Nemeio-x64-X.X.X.X.msi)

# Declare need constantes and variables
$torchPath = $args[5];

$refLang = "en-us";
$supportedLanguagesCode = @(1036);
$supportedLanguagesName = @("fr-fr");
$setLanguages = "1033,"; # English is always added by default

$configuration = $args[1];
$platform = $args[3];

Set-Location $refLang;
$msiName = "$(@(gci *.msi)[0].BaseName).msi";
Set-Location "..";

$msiPath = "\bin\$($platform)\$($configuration)";
$refMsiPath = "$($refLang)\$($msiName)";

Write-Output "Parameters :";
Write-Output "Torche Path = $($torchPath)";
Write-Output "MSI Path = $($msiPath)";
Write-Output "MSI Filename = $($msiName)";
Write-Output "Selected plateform = $($platform)";
Write-Output "Selected configuration = $($configuration)";
Write-Output "";

# For each supportedLanguages, we create transform file (.mst) and merge it on
# en-us MSI

For ($i=0; $i -le $supportedLanguagesCode.Count - 1; $i++) 
{
    $currentCode = $supportedLanguagesCode[$i];
    $currentName = $supportedLanguagesName[$i];

    Write-Output "Start process for $($currentName) [$($currentCode)]";

    # Invoke torch.exe to get transform file
    $mainMsi = ".\$($refMsiPath)";
    $targetMsi = ".\$($currentName)\$($msiName)";
    $outputMst = ".\$($refLang)\$($currentName).mst";

    $torchArguments = "-t Language $($mainMsi) $($targetMsi) -out $($outputMst)";

    Write-Output $torchPath;
    Write-Output $torchArguments;

    $proc = Start-Process -FilePath $torchPath -ArgumentList $torchArguments -PassThru;
    $handle = $proc.Handle # cache proc.Handle
    $proc.WaitForExit();

    if ($proc.ExitCode -ne 0) {
        Write-Warning "$_ exited with status code $($proc.ExitCode)";
        throw "torch exist with code $($proc.ExitCode)";
    }
   
    # Merge MST file with en-us MSI
    cscript "..\..\..\wisubstg.vbs" ".\$($refMsiPath)" ".\$($refLang)\$($currentName).mst" "$($currentCode)";

    $setLanguages += $currentCode;
    if($i -ne $supportedLanguagesCode.Count - 1)
    {
        $setLanguages += ",";
    }
}

# Update final MSI to support each languages
cscript "..\..\..\wilangid.vbs" ".\$($refMsiPath)" "Package" "$($setLanguages)";