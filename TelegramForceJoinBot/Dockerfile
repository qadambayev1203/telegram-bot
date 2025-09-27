# 1. Build bosqichi
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Loyiha fayllarini ko‘chirish
COPY *.csproj ./
RUN dotnet restore

# Hamma kodni ko‘chirish va build qilish
COPY . ./
RUN dotnet publish -c Release -o /app

# 2. Run bosqichi
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "TelegramForceJoinBot.dll"]
