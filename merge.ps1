param(
    [alias("ws")]
    $workspace = ''
)
Write-Host "Getting TFSConfig full path"
$path = (Get-ChildItem -path $workspace\ -filter "ilmerge.exe" -erroraction silentlycontinue -recurse).FullName
& $path /out:$workspace\Automation.Tfs.exe $workspace\Automation.Tfs.Console\bin\Debug\Automation.Tfs.exe $workspace\VersionOne.Tfs.Configure\bin\Debug\CLAP.dll $workspace\Automation.Tfs.Tools\bin\Debug\Automation.Tfs.Tools.dll