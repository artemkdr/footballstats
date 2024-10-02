# footballstats
This project is a coding challenge.
It's a simple service for storing football game results.

The service consists of 2 subservices inside: [API](API/README.md) and [client](client/README.md).

## API
.NET 6.0 web api application

## Client
React (Typescript) application on ChakraUI as a base for the components.

## How to build
The project contains [Dockerfile.client](Dockerfile.client) and [Dockerfile.api](Dockerfile.api) to build 2 containers.<br/>
The client container is an nginx ([nginx.conf](nginx.conf)) based image for serving the react build and for serving the API web app under `/api/...`.

You can use [docker-compose.yml](docker-compose.yml) to run 2 containers locally using [Docker Desktop](https://www.docker.com/products/docker-desktop/) for example.

## Deploy
coming soon...