param(
    [alias("ws")]
    $workspace = (Get-Location).Path
)
Write-Host "Getting ilmerge full path"

$path = (Get-ChildItem -path $workspace\ -filter "ilmerge.exe" -erroraction silentlycontinue -recurse).FullName
& $path /out:$workspace\Automation.Tfs.exe $workspace\Automation.Tfs.Console\bin\Debug\Automation.Tfs.exe $workspace\Automation.Tfs.Console\bin\Debug\CLAP.dll $workspace\Automation.Tfs.Console\bin\Debug\Automation.Tfs.Tools.dll