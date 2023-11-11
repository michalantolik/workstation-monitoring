############################################################################
# Publish the app
############################################################################

dotnet publish "WorkstationMonitoring\ClientModule\ClientModule.csproj" `
    -c Release `
    -p:PublishProfile=FolderProfile

dotnet publish "WorkstationMonitoring\ServerModule\ServerModule.csproj" `
    -c Release `
    -p:PublishProfile=FolderProfile
