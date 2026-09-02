if ($args.Count -lt 1) { Write-Error "Usage: .\rename.ps1 NewName"; exit 1 }

$targetName = "MyMarsPlugin"
$newName = $args[0]

# Имя пакета: буквы/цифры/точки/дефис/подчёркивание (как NuGet id, без пробелов и символов путей).
# Точки допустимы: "Mars.TelegramPlugin" -> папки, файлы, PackageId, namespace, пути /_plugin/...
if ($newName -notmatch '^[A-Za-z_][A-Za-z0-9_.-]*$') {
    Write-Error "Invalid name '$newName'. Use letters, digits and dots only (e.g. Mars.TelegramPlugin)."
    exit 1
}

if (-not $newName.EndsWith("Plugin")) {
    $newName = $newName + "Plugin"
    Write-Host "Notice: Added 'Plugin' suffix. New name: $newName"
}

# C#-имя: внутри идентификаторов (классов и т.п.) точки недопустимы — для них используется имя без
# не-C#-символов: "Mars.TelegramPlugin" -> "MarsTelegramPlugin" (MainMarsTelegramPlugin и т.д.).
$csName = $newName -replace '[^A-Za-z0-9_]', ''
Write-Host "C# name: $csName"

$scriptName = $MyInvocation.MyCommand.Name

# Расширения, которые нужно обрабатывать
$allowedExtensions = @(".sln", ".slnx", ".csproj", ".cs", ".razor", ".js", ".css", ".json", ".yml", ".yaml", ".ps1")

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

$escaped = [regex]::Escape($targetName)
$options = [System.Text.RegularExpressions.RegexOptions]::IgnoreCase

# Замена содержимого файлов (только с разрешёнными расширениями, кроме текущего скрипта)
Get-ChildItem -Recurse -File |
    Where-Object { $_.Name -ne $scriptName -and $allowedExtensions -contains $_.Extension } |
    ForEach-Object {
        $content = Get-Content $_ -Raw
        if ($_.Extension -in @(".cs", ".razor")) {
            # C#: токен, приклеенный к букве/цифре/_, — часть идентификатора (MainMyMarsPlugin,
            # MyMarsPluginDbContext) -> имя без точек; иначе точка попадёт в имя класса.
            $content = [regex]::Replace($content, "(?<=\w)$escaped|$escaped(?=\w)", $csName, $options)
            # C#: токен отдельным словом (namespace/using/строки/пути /_plugin/...) -> полное имя.
            $content = [regex]::Replace($content, "(?<!\w)$escaped(?!\w)", $newName, $options)
        }
        else {
            $content = [regex]::Replace($content, $escaped, $newName, $options)
        }
        [IO.File]::WriteAllText($_.FullName, $content)
    }
