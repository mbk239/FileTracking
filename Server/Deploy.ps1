cls

$projectpath = "$base_directory\FileTracking.Services\FileTracking.Services.csproj"
$publishProfile = "FolderProfile"

$msBuildPath = "C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\msbuild.exe"
$devEnvPath = "C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\IDE\devenv.exe”
$aspnetMergePath = "C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6.2 Tools"
$visualStudioVersion = "15.0"

$service_name = "FileTrackingService"
$service_displayname = "File Tracking Service"
$deployFolder = "C:\FileTrackingService"
Function Get-PSScriptRoot
{
    $ScriptRoot = ""

    Try
    {
        $ScriptRoot = Get-Variable -Name PSScriptRoot -ValueOnly -ErrorAction Stop
    }
    Catch
    {
        $ScriptRoot = Split-Path $script:MyInvocation.MyCommand.Path
    }
    return $ScriptRoot 
}

$base_directory = Get-PSScriptRoot
$service_binfile = "$deployFolder\FileTracking.Services.exe"

Function ServiceExists([string] $ServiceName) {
    [bool] $Return = $False
    # If you use just "Get-Service $ServiceName", it will return an error if 
    # the service didn't exist.  Trick Get-Service to return an array of 
    # Services, but only if the name exactly matches the $ServiceName.  
    # This way you can test if the array is emply.
    if ( Get-Service "$ServiceName*" -Include $ServiceName ) {
        $Return = $True
    }
    Return $Return
}

Function DeleteService([string] $ServiceName) {
    [bool] $Return = $False
    $Service = Get-WmiObject -Class Win32_Service -Filter "Name='$ServiceName'" 
    if ( $Service ) {
        Stop-Service $ServiceName
        $Service.Delete()
        if ( -Not ( ServiceExists $ServiceName ) ) {
            $Return = $True
        }
    } else {
        $Return = $True
    }
    Return $Return
}


function PublishProject([string]$ProjectFilePath, [string]$ProjectPublishProfile) 
{
    $parameters = " ""$ProjectFilePath"" /flp:logfile=$ProjectPublishProfile.txt;errorsonly /p:DeployOnBuild=true /p:PublishProfile=$ProjectPublishProfile /p:VisualStudioVersion=$visualStudioVersion"
    write-host "     Start publish ""$ProjectPublishProfile"""
    $process = [System.Diagnostics.Process]::Start( """$msBuildPath""", $parameters )
    $process.WaitForExit()
    write-host "     Publish $ProjectPublishProfile done..........."
}



if (DeleteService $service_name)
{
    Write-Host "Older service deleted"

    PublishProject $projectpath $publishProfile

	xcopy "$base_directory\PublishFileTrackingService\*" "$deployFolder" /Y /S /I /Q
    New-Service -Name $service_name -BinaryPathName $service_binfile -DisplayName $service_displayname -StartupType Manual -Description $service_name
    Start-Service $service_name
    Write-Host "$service_displayname : Started"
}
else
{
    Write-Host "Cannot delete older service. Please verify"
}

