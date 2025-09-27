FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY TelegramForceJoinBot/*.csproj ./
RUN dotnet restore ./TelegramForceJoinBot.csproj
COPY TelegramForceJoinBot/ ./
RUN dotnet publish TelegramForceJoinBot.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "TelegramForceJoinBot.dll"]
