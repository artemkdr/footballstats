# Football (baby-foot) Stats Service

This project is a coding challenge that implements a service for storing and displaying football game results. It includes a .NET API for managing data and a React client application for viewing and interacting with the data. The client application consumes the API to display data and allow users to interact with the service.

The service uses a PostgreSQL database to store information about teams, players, and games. [See the database creation script here](psqlinit/init.sql).

The default **dev** connection string is set up for a test database Docker container (see below):
```
Host=localhost;Port=5442;Database=footballstats;Username=postgres;Password=postgres;
```

### Populating the Database

When you first run the service, the database will be empty. You have two ways to populate it:

**1. Manually via the UI:**

* **Create Players:** This automatically creates a team with the same name, containing the newly created player.
* **Create Games:** Once you have teams, you can create games between them.

**2. Automatic Generation:**

* **Players Page:** Use the "Generate Players" button to create 10 random players.
* **Games Page:** Use the "Simulate Games" button to automatically generate two matches for every possible team pairing, one with each team playing as the host.


## API

A .NET 6.0 Web API application.

Interactive API documentation, powered by Swagger UI, can be found on `/api/swagger/index.html` after starting the service using `docker-compose up -d --build`. This allows you to explore and test the API endpoints. 
If you run the service on your local machine, then you can access the Swagger UI at http://localhost/api/swagger/index.html.

## Client

A React (TypeScript) application built with [Chakra UI](https://github.com/chakra-ui/chakra-ui/).

## How to build

The project uses Docker for containerization and provides a [docker-compose.yml](docker-compose.yml) file to simplify building and running the service. 

### Running with Docker Compose

To build and run the whole service locally with Docker Desktop (from the project directory):

```bash
docker-compose up -d --build
```

This command will:

* Build three Docker containers:
    * **Client:**  An nginx-based container ([nginx.conf](nginx.conf)) that serves the React frontend application and acts as a reverse proxy for the API.
    * **API:**  An ASP.NET 6.0 container ([Dockerfile.api](Dockerfile.api)) that hosts the .NET API application. The API runs on port 5000 within the container and is accessible through the client container's nginx server on `/api/...`.
    * **Database:** A PostgreSQL 15 container ([Dockerfile.psql](Dockerfile.psql)) with the `footballstats` database. The database server is exposed on port 5432: `
Host=db;Port=5432;Database=footballstats;Username=postgres;Password=postgres;
`
* Start the containers in detached mode.

**Accessing the application:**

* The client application will be available at http://localhost.
* The API documentation will be available at http://localhost/api/swagger/index.html.

**Docker Compose Network:**

This application uses a Docker Compose defined network to facilitate communication between its components. Here's a breakdown: 
* **Network Type**: `internal` - This isolates the application containers from the host machine's network and external networks, enhancing security.
* **Service Discovery**: Containers can reach each other using their service names as hostnames within this internal network.
* **Hostnames**:
    * frontend - For the frontend service.
    * backend - For the backend service.
    * db - For the database service.

### Stopping the service

To stop the containers:
```bash
docker-compose down
```

To stop the containers and remove the associated volumes (the database data):

```
docker-compose down --volumes
```

### Running individual components
***Database only:***
#### to run the container
```
docker-compose -f .\docker-compose-db.yml up -d --build
```
#### to stop the container
```
docker-compose -f .\docker-compose-db.yml down
```

***API only:***
#### to run from Visual Studio Code project folder
```
cd api
dotnet run
```
#### to run the container
```
docker -f Dockerfile.api build -t footballstats-client .
docker run -p 5000:5000 footballstats-api 
```
#### to stop the container
```
# to stop the container
docker stop footballstats-api
```

***Client only:***

(Assuming you have the API running separately or want to connect to an external API)
#### to run from Visual Studio Code project folder
The first time: install the packages:
```
cd client
npm ci
```
If all the packages are installed:
```
cd client
npm run start
```
#### to run as a container
```
docker -f Dockerfile.client build -t footballstats-client .
docker run -p 80:80 footballstats-client
```
#### to stop the container
```
docker stop footballstats-client
```


## Github Actions
The project uses Github Actions for continuous integration. The workflows are defined in the .github/workflows directory:

### [build.yml](.github/workflows/build.yml)
Builds API .net application and runs the tests.
Builds client React application and runs the tests.

### [build-auto.yml](.github/workflows/build-auto.yml)
A wrapper to [build.yml](.github/workflows/build.yml) in order to run it automatically on every push or pull request in the 'main' branch.

### [build-net6.yml](.github/workflows/build-net6.yml)
Reusable workflow to build API .net application and runs its tests.

### [build-react.yml](.github/workflows/build-react.yml)
Reusable workflow to build React application and runs its tests.