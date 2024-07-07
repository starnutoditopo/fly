# See https://stackoverflow.com/a/5924807/1288109
Get-ChildItem .\ -include .vs -Hidden -Recurse | ForEach-Object ($_) { Remove-Item $_.FullName -Force -Recurse }
