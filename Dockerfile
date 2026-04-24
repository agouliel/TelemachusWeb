# ── Stage 1: Build React SPA ─────────────────────────────────────────────
FROM node:18-alpine AS spa-builder
WORKDIR /spa
COPY Telemachus.Web/package*.json ./
RUN npm ci --silent
COPY Telemachus.Web/ ./
RUN npm run build

# ── Stage 2: Build .NET API ───────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:3.1 AS api-builder
# TARGETARCH is set automatically by Docker: "amd64" on x86_64, "arm64" on Apple Silicon.
ARG TARGETARCH
WORKDIR /src

# Copy full source (node_modules, bin, obj filtered by .dockerignore)
COPY . .

# Overlay pre-built SPA where PublishRunWebpack expects it.
# SpaRoot in .csproj = ..\..\Telemachus.Web\ relative to Telemachus.Api/Telemachus/
# which resolves to /src/Telemachus.Web/ inside this stage.
COPY --from=spa-builder /spa/dist ./Telemachus.Web/dist

# Map Docker's TARGETARCH ("amd64"/"arm64") to .NET RIDs ("linux-x64"/"linux-arm64").
# Override win-x64 RID from the .csproj; skip npm steps because dist is already present.
RUN DOTNET_RID=$([ "$TARGETARCH" = "amd64" ] && echo "linux-x64" || echo "linux-arm64") && \
    dotnet publish Telemachus.Api/Telemachus/Telemachus.csproj \
      -c Release \
      --runtime $DOTNET_RID \
      --self-contained false \
      -p:DotNetPublishArgs=--no-build \
      -o /publish

# ── Stage 3: Runtime ──────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:3.1
WORKDIR /app
COPY --from=api-builder /publish .
# appsettings.json is required (optional: false). Placeholder values here are
# overridden at runtime by the environment variables in docker-compose.yml.
COPY appsettings.json.example appsettings.json
RUN mkdir -p /app/Static
EXPOSE 3001
ENTRYPOINT ["dotnet", "Telemachus.dll"]
