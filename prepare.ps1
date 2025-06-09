<#
.SYNOPSIS
Prepares the project by renaming files, cleaning up unnecessary files and removing itself.

#>

$newName = $args[0]

# 1. Run rename.ps1 with "newName" argument
if (Test-Path ".\rename.ps1") {
    Write-Host "Running rename.ps1 with newName=$newName argument..."
    & ".\rename.ps1" $newName
}

# 2. Delete specified files and directories
$itemsToDelete = @(
    "LICENSE.txt",
    "rename.ps1",
    ".git"
)

foreach ($item in $itemsToDelete) {
    if (Test-Path $item) {
        try {
            if (Test-Path $item -PathType Container) {
                Write-Host "Removing directory: $item"
                Remove-Item $item -Recurse -Force -ErrorAction Stop
            }
            else {
                Write-Host "Removing file: $item"
                Remove-Item $item -Force -ErrorAction Stop
            }
        }
        catch {
            Write-Warning "Failed to remove $item : $_"
        }
    }
}

# 3. Delete itself
Write-Host "Removing prepare.ps1..."
Start-Sleep -Seconds 1  # Small delay to ensure script completes
Remove-Item $MyInvocation.MyCommand.Path -Force