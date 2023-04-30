# CIL Browser test PowerShell script

<# Multiline 
comment 'string inside comment'#>

echo ""
echo "Hello from PowerShell!"

$animals = 'cat', 'dog', 'rabbit'

foreach ($x in $animals)
{
    echo ('-'+$x)
}

Copy-Item "..\foo.txt" -Destination ".\bar\"
Rename-Item -Path ".\bar\foo.txt" -NewName "text.txt"

echo """End''"
[void][System.Console]::ReadKey($true)
