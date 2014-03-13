param(
    [alias("ws")]
    $workspace = ''
)
Write-Host "Getting TFSConfig full path"
$path = (Get-ChildItem -path $workspace\ -filter "ilmerge.exe" -erroraction silentlycontinue -recurse).FullName
& $path /out:$workspace\VersionOne.Tfs.Configure.exe $workspace\VersionOne.Tfs.Configure\bin\Debug\VersionOne.Tfs.Configure.exe $workspace\VersionOne.Tfs.Configure\bin\Debug\CLAP.dll $workspace\VersionOne.Tfs.Configure\bin\Debug\VersionOne.Tfs.Tools.dll