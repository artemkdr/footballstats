# footballstats
This project is a coding challenge.
It's a simple service for storing football game results.

The service consists of 2 subservices inside: [API](API/README.md) and [client](client/README.md).

## API
.NET 6.0 web api application

## Client
React (Typescript) application on ChakraUI as a base for the components.

## How to build
The project contains [Dockerfile.client](Dockerfile.client) (client), [Dockerfile.api](Dockerfile.api) (API), [Dockerfile.psql](Dockerfile.psql) (database) to build 3 containers.<br/>

The client container is an nginx ([nginx.conf](nginx.conf)) based image for serving the react build and for serving the API web app under `/api/...`.

The API container is an aspnet 6.0 runtime container with a .NET core application. The application is running on 5000 port, but it's served via nginx reverse proxy via /api/... anyway.

The database container is a postgre 15 server with 'footballstats' database inside with a minumum set of the tables needed. The server is exposed on 5442 port via docker-compose.

You can use [docker-compose.yml](docker-compose.yml) to run all 3 containers locally using [Docker Desktop](https://www.docker.com/products/docker-desktop/) for example.

## Deploy
coming soon...