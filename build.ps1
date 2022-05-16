$path = $args[0]
$msg = "Building " + $path

echo $msg

if (-not(Test-Path -Path $path/'out')) {
    mkdir $path/'out'
}
Remove-Item -r $path/'out/*'
dotnet publish -c Release -r linux-musl-x64 --self-contained true -o $path/'out'