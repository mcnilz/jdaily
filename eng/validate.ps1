[CmdletBinding(SupportsShouldProcess = $true)]
param([Parameter(Mandatory = $true)][ValidateSet('Domain','UI','Jira','Dependency','Docs')][string]$Profile)
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$commands = @{ Domain=@('dotnet build JiraBoard.slnx -c Release','dotnet test JiraBoard.slnx -c Release --no-build'); UI=@('dotnet build JiraBoard.slnx -c Release','dotnet test JiraBoard.slnx -c Release --no-build'); Jira=@('dotnet build JiraBoard.slnx -c Release','dotnet test JiraBoard.slnx -c Release --no-build'); Dependency=@('dotnet restore JiraBoard.slnx --locked-mode','dotnet build JiraBoard.slnx -c Release','dotnet test JiraBoard.slnx -c Release --no-build','pwsh -NoProfile -File eng/check-dependency-policy.ps1'); Docs=@('git diff --check') }[$Profile]
foreach($command in $commands){ if($PSCmdlet.ShouldProcess($command,"validate $Profile")){ $watch=[Diagnostics.Stopwatch]::StartNew(); Invoke-Expression $command; $code=$LASTEXITCODE; $watch.Stop(); if($code -ne 0){ exit $code }; [Console]::WriteLine("PASS: $command | exit=0 | duration=$([math]::Round($watch.Elapsed.TotalSeconds,2))s") } }
