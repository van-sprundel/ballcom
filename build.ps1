$path = $args[0]
$msg = "Building " + $path

echo $msg
$out_path = $path+'/out'

if (-not(Test-Path -Path $out_path)) {
    mkdir out
} else {
    echo "Emptying out path"
    Remove-Item -path $out_path/* -r
}

dotnet publish $path -c Release -r linux-musl-x64 --self-contained true -o $out_path