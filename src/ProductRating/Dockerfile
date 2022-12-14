
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/ProductRating/ProductRating.csproj", "src/ProductRating/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]
COPY ["src/Application/Application.csproj", "src/Application/"]
RUN dotnet restore "src/ProductRating/ProductRating.csproj"
COPY . .
WORKDIR "/src/src/ProductRating"
RUN dotnet build "ProductRating.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProductRating.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductRating.dll"]