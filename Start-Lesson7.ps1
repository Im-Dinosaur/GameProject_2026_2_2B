param([switch]$SqlConsole)
$ErrorActionPreference = 'Stop'
$lessonTools = Join-Path $PSScriptRoot 'Lesson7Tools'
$lessonMysql = Join-Path $lessonTools 'mysql-8.4.13-winx64\bin'
$lessonServerConfig = Join-Path $lessonTools 'my.ini'
$lessonClientConfig = Join-Path $lessonTools 'mysql-client.ini'
& (Join-Path $lessonMysql 'mysqladmin.exe') "--defaults-extra-file=$lessonClientConfig" ping 2>$null | Out-Null
if ($LASTEXITCODE -ne 0) {
    Start-Process -FilePath (Join-Path $lessonMysql 'mysqld.exe') -ArgumentList "--defaults-file=`"$lessonServerConfig`"" -WindowStyle Hidden | Out-Null
    $lessonReady = $false
    for ($attempt = 0; $attempt -lt 40; $attempt++) {
        Start-Sleep -Milliseconds 500
        & (Join-Path $lessonMysql 'mysqladmin.exe') "--defaults-extra-file=$lessonClientConfig" ping 2>$null | Out-Null
        if ($LASTEXITCODE -eq 0) { $lessonReady = $true; break }
    }
    if (!$lessonReady) { throw 'MySQL startup failed. Check Lesson7Tools/mysql-error.log.' }
}
if (!(Get-NetTCPConnection -LocalPort 3000 -State Listen -ErrorAction SilentlyContinue)) {
    $lessonNode = (Get-Command node.exe -ErrorAction Stop).Source
    $lessonServer = Join-Path $PSScriptRoot 'Node\DungeonServer.js'
    Start-Process -FilePath $lessonNode -ArgumentList "`"$lessonServer`"" -WorkingDirectory (Join-Path $PSScriptRoot 'Node') -WindowStyle Hidden -RedirectStandardOutput (Join-Path $lessonTools 'node.stdout.log') -RedirectStandardError (Join-Path $lessonTools 'node.stderr.log') | Out-Null
    Start-Sleep -Milliseconds 700
}
$lessonState = Invoke-RestMethod -Uri 'http://127.0.0.1:3000/api/game/state'
if (!$lessonState.success) { throw 'Dungeon server verification failed.' }
Write-Output 'Ready: Node http://127.0.0.1:3000 | MySQL 127.0.0.1:3306 | database gametest'
if ($SqlConsole) {
    & (Join-Path $lessonMysql 'mysql.exe') "--defaults-extra-file=$lessonClientConfig" gametest
}
