fx_version 'bodacious'
game 'gta5'

author 'xMrProjectZ'
version '1.0.0'

restart_on_change 'yes'

fxdk_watch_command 'dotnet' {'watch', '--project', 'Client/Menu.Client.csproj', 'publish', '--configuration', 'Release'}
fxdk_watch_command 'dotnet' {'watch', '--project', 'Server/Menu.Server.csproj', 'publish', '--configuration', 'Release'}

file 'Client/libs/LemonUI.FiveM.dll'

client_script 'Client/bin/Release/net452/*.dll'
server_script 'Server/bin/Release/netstandard2.0/*.dll'

