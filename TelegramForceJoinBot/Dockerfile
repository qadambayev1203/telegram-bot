# .NET 8 SDK bilan build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# .csproj faylni ko‘chirib olish va restore
COPY TelegramForceJoinBot.csproj ./
RUN dotnet restore TelegramForceJoinBot.csproj

# Boshqa fayllarni ko‘chirib olish va publish qilish
COPY . .
RUN dotnet publish -c Release -o /app

# Runtime image
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build /app .

# Botni ishga tushirish
ENTRYPOINT ["dotnet", "TelegramForceJoinBot.dll"]
