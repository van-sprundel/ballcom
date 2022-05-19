$path = $args[0]
$msg = "Building " + $path

echo $msg
$out_path = $path+'/out'

if (-not(Test-Path -Path $out_path)) {
    mkdir $out_path
} else {
    echo "Emptying /out"
    Remove-Item -path $out_path/* -r
}

.\build-dotnet.ps1 $path