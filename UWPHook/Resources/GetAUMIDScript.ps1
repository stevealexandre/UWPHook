﻿$installedapps = get-AppxPackage
$invalidNames = '*ms-resource*', '*DisplayName*'
$aumidList = @()

foreach ($app in $installedapps)
{
    try {
        if(-not $app.IsFramework) 
        {
            foreach ($id in (Get-AppxPackageManifest $app).package.applications.application.id)
            {
                $appx = Get-AppxPackageManifest $app;
                if ($id -in ("Game","App") -or $appx.Package.Applications.Application.Executable -eq "GameLaunchHelper.exe") {
                    $name = $appx.Package.Properties.DisplayName;
                    $executable = $appx.Package.Applications.Application[0].Executable;

                    # Handle app running with microsoft launcher or which doesn't have an executable in the manifest
                    if([string]::IsNullOrWhitespace($executable) -or $executable -eq "GameLaunchHelper.exe") {
                        if(Test-Path -Path ($app.InstallLocation + "\MicrosoftGame.Config")) {
                            [xml]$msconfig = Get-Content ($app.InstallLocation + "\MicrosoftGame.Config");
                            if ($msconfig.Game.ExecutableList.Executable -is [array]) {
                                $executable = $msconfig.Game.ExecutableList.Executable[0].Name;
                            }
                            else {
                                $executable = $msconfig.Game.ExecutableList.Executable.Name;
                            }
                        }
                        else {
                            # Cannot handle app which doesn't have any configuration to read
                            continue;
                        }
                    }
                    # Convert object to ensure is the String of execuble (cf Halo Master Chief Collection example below)
                    # mcclauncher.exe
                    # MCC\Binaries\Win64\MCCWinStore-Win64-Shipping.exe
                    if($executable -is [Object[]]) { $executable = $executable[1].ToString() }

                    # Exclude apps without a name acceptable
                    if($name -like '*DisplayName*' -or $name  -like '*ms-resource*')
                    {
                        continue;
                    }

                    if ($appx.Package.Applications.Application -is [array]) {
                        $logo = $app.InstallLocation + "\" + $appx.Package.Applications.Application[0].VisualElements.Square150x150Logo;
                    }
                    else {
                        $logo = $app.InstallLocation + "\" + $appx.Package.Applications.Application.VisualElements.Square150x150Logo;
                    }
                    
                    # Check for possible duplicate game like Halo MCC which have two version (one with AC and one witohut AC)
                    $duplicate = $false;
                    foreach($item in $aumidList) {
                        #Write-host $item - $name
                        if($item.StartsWith($name)) {
                            $duplicate = $true;
                            break;
                        }
                    }

                    # Insert if not duplicated
                    if(!$duplicate) {
                        $aumidList += $name + "|" + $logo + "|" + $app.packagefamilyname + "!" + $id + "|" + $executable + ";"
                    }
                }
            }
        }
    }
    catch
    {
        $ErrorMessage = $_.Exception.Message
        $FailedItem = $_.Exception.ItemName
    }
}

$aumidList | Sort-Object;