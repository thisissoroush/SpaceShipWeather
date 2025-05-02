## SpaceShipWeather - Returns weather for a spaceship which will not be available till 20yrs! 

KISS and High Frequnt!
SQLite being used.

[![MIT License](https://img.shields.io/badge/License-MIT-green.svg)](https://choosealicense.com/licenses/mit/)




## Routes&Defaults

| Route                | Result       |
|----------------------|--------------|
| /api/weather         | weather      |



## Docker

To Dockerize the project using multi stage build.

```bash
  docker build .


  docker run -d \
  -p 8080:80 \
  --name spaceship-weather-api \
  -v $(pwd)/DatabaseFiles:/app/DataBase/Data \
  spaceship-weather-image

```


## Run Locally

Clone the project

```bash
  git clone https://github.com/thisissoroush/`SpaceShipWeather`
```

Go to the project directory

```bash
  cd https://github.com/thisissoroush/SpaceShipWeather
```

Install dependencies

```bash
  dotnet restore
```

Build the project

```bash
  dotnet build
```

Start the server

```bash
  dotnet run
```


## Author

- [@Soroush Nasiri](https://www.github.com/Thisissoroush)