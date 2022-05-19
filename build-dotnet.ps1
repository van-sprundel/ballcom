$path = $args[0]
$out_path = $path+'/out'

dotnet publish $path -c Release -r linux-musl-x64 --self-contained true -o $out_path