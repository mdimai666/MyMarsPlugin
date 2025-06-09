if ($args.Count -lt 1) { Write-Error "Usage: .\rename.ps1 NewName"; exit 1 }

$targetName = "MyMarsPlugin"
$newName = $args[0]
if (-not $newName.EndsWith("Plugin")) {
    $newName = $newName + "Plugin"
    Write-Host "Notice: Added 'Plugin' suffix. New name: $newName"
}
$scriptName = $MyInvocation.MyCommand.Name

# Расширения, которые нужно обрабатывать
$allowedExtensions = @(".sln", ".csproj", ".cs", ".razor", ".js", ".css", ".json")

# Функция для рекурсивного переименования папок
function Rename-Folders {
    param($path)
    $folders = Get-ChildItem -Path $path -Directory -Recurse | Sort-Object -Property FullName -Descending
    foreach ($folder in $folders) {
        if ($folder.Name -match $targetName) {
            $newFolderName = $folder.Name -replace $targetName, $newName
            Rename-Item -Path $folder.FullName -NewName $newFolderName -ErrorAction SilentlyContinue
        }
    }
}

# Переименование папок (сначала самые вложенные)
Rename-Folders -path .

# Переименование файлов (только с разрешёнными расширениями, кроме текущего скрипта)
Get-ChildItem -Recurse -File |
    Where-Object { $_.Name -ne $scriptName -and $allowedExtensions -contains $_.Extension } |
    Rename-Item -NewName { $_.Name -replace $targetName, $newName } -ErrorAction SilentlyContinue

# Замена содержимого файлов (только с разрешёнными расширениями, кроме текущего скрипта)
Get-ChildItem -Recurse -File |
    Where-Object { $_.Name -ne $scriptName -and $allowedExtensions -contains $_.Extension } |
    ForEach-Object {
        $content = (Get-Content $_ -Raw) -replace $targetName, $newName
        [IO.File]::WriteAllText($_.FullName, $content)
    }
