$exe = "..\\..\\..\\Mars\\src\\Mars.WebApp\\bin\\Debug\\net9.0\\Mars.exe"
$cfg = "..\\..\\..\\MyMarsPlugin\\src\\MyMarsPlugin\\appsettings.local.json"

$env:ASPNETCORE_ENVIRONMENT="Development"

& $exe -cfg $cfg
