$path = $args[0]
$out_path = $path+'/out'

$msg = "Building " + $out_path
echo $msg

if (-not(Test-Path -Path $out_path)) {
    mkdir $out_path
} else {
    echo "Emptying /out"
    Remove-Item -path $out_path/* -r
}

.\build-dotnet.ps1 $path