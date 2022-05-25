$path = $args[0]
$out_path = $path+'/out'

$msg = "Building " + $out_path
Write-Host $msg -ForegroundColor Magenta

if (-not(Test-Path -Path $out_path)) {
    mkdir $out_path
} else {
    Write-Host "Emptying /out..." -ForegroundColor Red
    Remove-Item -path $out_path/* -r
}

dotnet publish $path -c Release -r linux-musl-x64 --self-contained true -o $out_path

Write-Host "Success!" -ForegroundColor Green