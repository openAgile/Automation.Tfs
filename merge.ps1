param(
    [alias("ws")]
    $ws = ''
)
Write-Host "Getting TFSConfig full path"
$path = (Get-ChildItem -path $ws\ -filter "ilmerge.exe" -erroraction silentlycontinue -recurse)[0].FullName
& $path /out:$ws\VersionOne.Tfs.Configure\bin\Debug\VersionOne.Tfs.Configure.all.exe $ws\VersionOne.Tfs.Configure\bin\Debug\VersionOne.Tfs.Configure.exe $ws\VersionOne.Tfs.Configure\bin\Debug\CLAP.dll $ws\VersionOne.Tfs.Configure\bin\Debug\VersionOne.Tfs.Tools.dll