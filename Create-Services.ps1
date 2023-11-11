####################################################################################################
# Function - create Windows Service from scratch (deletes if exists and creates a new one)
####################################################################################################

function Create-Service {
    param (
        [string] $ServiceName,
        [string] $BinPath,
        [string] $StartupType,
        [bool] $StartNow
    )

    if ((sc.exe query $ServiceName) -match "SERVICE_NAME: $ServiceName") {
        Write-Host " > Service already exists: ""$ServiceName""."
        Write-Host " > Deleting ""$ServiceName""."
        sc.exe stop $ServiceName | Out-Null
        sc.exe delete $ServiceName | Out-Null
        Write-Host " > Deleted ""$ServiceName""."
    }
    
    Write-Host " > Creating ""$ServiceName""."
    sc.exe create $ServiceName binPath=$BinPath start=$StartupType | Out-Null
    Write-Host " > Created ""$ServiceName""."

    if ($StartNow) {
        Write-Host " > Starting ""$ServiceName""."
        sc.exe start $ServiceName | Out-Null
        Write-Host " > Started ""$ServiceName""."
    }

    Write-Host " > ..."
}

####################################################################################################
# Calls
####################################################################################################

Write-Host " > STARTED script execution.`n > ..."

Create-Service `
    -ServiceName "Workstation Monitoring Client Service" `
    -BinPath "$PSScriptRoot\WorkstationMonitoring\ClientModule\bin\Release\net6.0-windows\win-x64\publish\ClientModule.exe" `
    -StartupType "auto" `
    -StartNow $true

Create-Service `
    -ServiceName "Workstation Monitoring Server Service" `
    -BinPath "$PSScriptRoot\WorkstationMonitoring\ServerModule\bin\Release\net6.0-windows\win-x64\publish\ServerModule.exe" `
    -StartupType "auto" `
    -StartNow $true 

Write-Host " > FINISHED script execution."
