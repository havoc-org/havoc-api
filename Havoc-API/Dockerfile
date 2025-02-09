# syntax=docker/dockerfile:1

# Comments are provided throughout this file to help you get started.
# If you need more help, visit the Dockerfile reference guide at
# https://docs.docker.com/go/dockerfile-reference/

# Want to help us make this template better? Share your feedback here: https://forms.gle/ybq9Krt8jtBL3iCk7

################################################################################

# Learn about building .NET container images:
# https://github.com/dotnet/dotnet-docker/blob/main/samples/README.md

# Create a stage for building the application.
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build

COPY . /source

# This is the architecture you’re building for, which is passed in by the builder.
# Placing it here allows the previous steps to be cached across architectures.
ARG TARGETARCH

RUN --mount=type=secret,id=jwt_key \
    --mount=type=secret,id=db_connection \
    --mount=type=secret,id=api_url \
    --mount=type=secret,id=spa_url 
    # JWT__Key=$(cat /run/secrets/jwt_key) \
    # ConnectionStrings__Default=$(cat /run/secrets/db_connection) \
    # JWT__Issuer=$(cat /run/secrets/api_url) \
    # JWT__Audience=$(cat /run/secrets/spa_url)

# RUN apk add --no-cache bash \
#     && sed -i "s|#{ConnectionStrings__Default}#|$(cat run/secrets/db_connection)|g" /source/Havoc-API/appsettings.Production.json \
#     && sed -i "s|#{JWT__Key}#|$(RUN --mount=type=secret,id=jwt_key, cat run/secrets/jwt_key)|g" /source/Havoc-API/appsettings.Production.json \
#     && sed -i "s|#{JWT__Audience}#|$(cat run/secrets/spa_url)|g" /source/Havoc-API/appsettings.Production.json \
#     && sed -i "s|#{JWT__Issuer}#|$(cat run/secrets/api_url)|g" /source/Havoc-API/appsettings.Production.json

# Build the application.
# Leverage a cache mount to /root/.nuget/packages so that subsequent builds don't have to re-download packages.
# If TARGETARCH is "amd64", replace it with "x64" - "x64" is .NET's canonical name for this and "amd64" doesn't
#   work in .NET 6.0.

WORKDIR /source/Havoc-API

RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet publish -a ${TARGETARCH/amd64/x64} --use-current-runtime --self-contained false -o /app
# If you need to enable globalization and time zones:
# https://github.com/dotnet/dotnet-docker/blob/main/samples/enable-globalization.md
################################################################################
# Create a new stage for running the application that contains the minimal
# runtime dependencies for the application. This often uses a different base
# image from the build stage where the necessary files are copied from the build
# stage.
#
# The example below uses an aspnet alpine image as the foundation for running the app.
# It will also use whatever happens to be the most recent version of that tag when you
# build your Dockerfile. If reproducibility is important, consider using a more specific
# version (e.g., aspnet:7.0.10-alpine-3.18),
# or SHA (e.g., mcr.microsoft.com/dotnet/aspnet@sha256:f3d99f54d504a21d38e4cc2f13ff47d67235efeeb85c109d3d1ff1808b38d034).
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
WORKDIR /app

# Copy everything needed to run the app from the "build" stage.
COPY ./appsettings.Production.json /app/appsettings.Production.json
COPY --from=build /app .

# Install libicu-dev
ENV \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    LC_ALL=en_US.UTF-8 \
    LANG=en_US.UTF-8

RUN apk add --no-cache \
    icu-data-full \
    icu-libs

# Switch to a non-privileged user (defined in the base image) that the app will run under.
# See https://docs.docker.com/go/dockerfile-user-best-practices/
# and https://github.com/dotnet/dotnet-docker/discussions/4764
USER $APP_UID

EXPOSE 8080
ENTRYPOINT ["dotnet", "Havoc-API.dll"]
