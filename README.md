# URLShortener

## Overview

This repository contains an alias generator and an API for URL shortening. Alias generator is a [worker service](https://learn.microsoft.com/en-us/dotnet/core/extensions/workers) that generates aliases on demand. The API provides a way to shorten URLs with either a user provided or generated alias. It also provides redirects to the original URL by alias.

Project structure is organized by [Clean Architecture Solution Template](https://github.com/jasontaylordev/CleanArchitecture).

## Infrastructure  

- Database: [Apache Cassandra](https://cassandra.apache.org/_/index.html) (NoSQL)
- Messaging: [Apache Kafka](https://kafka.apache.org/)

## Running locally

The setup for running the solution locally is intended to be flexible and aimed for a real-world application. To run the API and/or the alias generator, you need to have your infrastructure running. Listed below are the steps on how to do it:
- [Create a docker bridge network](#create-a-docker-bridge-network)
- [Start Cassandra cluster](#start-cassandra-cluster)
- [Run database migrations](#run-database-migrations)
- [Start Kafka ZooKeeper and Broker](#start-kafka-zookeeper-and-broker)
- [Create Kafka topic](#create-kafka-topic)

To run the generator and the API:
- [Run alias generator](#run-alias-generator)
- [Run API](#run-api)

Finally, to clean up:
- [Cleaning up](#cleaning-up)

### Docker CLI

Run the following commands **from the root of the repository**:

#### Create a docker bridge network
Creates a docker [bridge network](https://docs.docker.com/network/bridge/) for our application.
```
docker network create url-shortener-network
```

#### Start Cassandra cluster
Please note that it takes a while for Cassandra nodes to initialize.
```
docker run --rm -d -p 9042:9042 --name cassandra-node1 --network url-shortener-network cassandra:4.0.6
docker run --rm -d --name cassandra-node2 --network url-shortener-network -e CASSANDRA_SEEDS=cassandra-node1 cassandra:4.0.6
```

#### Run database migrations
Please note that migrations will fail if Cassandra nodes are not initialized.
```
docker build --tag cassandra-migrate migrations
docker run --rm -it -v %cd%/migrations:/migrations --network url-shortener-network cassandra-migrate cassandra-migrate -H host.docker.internal -c migrations/config.yml migrate
```

#### Start Kafka ZooKeeper and Broker
```
docker run --rm -d --name zookeeper --network url-shortener-network -e "ZOOKEEPER_CLIENT_PORT=2181" -e "ZOOKEEPER_TICK_TIME=2000" confluentinc/cp-zookeeper:7.0.0
docker run --rm -d --name broker --network url-shortener-network -p 9092:9092 -e "KAFKA_BROKER_ID=1" -e "KAFKA_ZOOKEEPER_CONNECT=zookeeper:2181" -e "KAFKA_LISTENER_SECURITY_PROTOCOL_MAP=PLAINTEXT:PLAINTEXT,PLAINTEXT_INTERNAL:PLAINTEXT" -e "KAFKA_ADVERTISED_LISTENERS=PLAINTEXT://host.docker.internal:9092,PLAINTEXT_INTERNAL://broker:29092" -e "KAFKA_OFFSETS_TOPIC_REPLICATION_FACTOR=1" -e "KAFKA_TRANSACTION_STATE_LOG_MIN_ISR=1" -e "KAFKA_TRANSACTION_STATE_LOG_REPLICATION_FACTOR=1" confluentinc/cp-kafka:7.0.0
```

#### Create Kafka topic
Creates alias_candidates topic. Alias generator will publish messages to it and API will consume them.
```
docker exec broker kafka-topics --create --topic alias_candidates --bootstrap-server host.docker.internal:9092 --replication-factor 1 --partitions 1
```

#### Run alias generator
```
docker build -f URLShortener.Generator/Dockerfile -t url-shortener-generator .
docker run --rm -d --network url-shortener-network -e "DOTNET_ENVIRONMENT=Development" --name url-shortener-generator url-shortener-generator
```

#### Run API
```
docker build -f URLShortener.API/Dockerfile -t url-shortener-api .
docker run --rm -d --network url-shortener-network -e "ASPNETCORE_ENVIRONMENT=Development" -e "ASPNETCORE_URLS=http://+:80" -p 9889:80/tcp --name url-shortener-api url-shortener-api
```

If all is well, you should be able to access the API at http://localhost:9889/swagger/index.html

#### Cleaning up
```
docker kill cassandra-node1
docker kill cassandra-node2
docker kill zookeeper
docker kill broker
docker kill url-shortener-api
docker kill url-shortener-generator
```

### Docker Compose (outdated)
https://github.com/ch200c/URLShortener/blob/fc2044be0b7bc389f1a88b915973b1f891c17509/docker-compose.yml


## TODO
- Resilience policies
- Authentication
- Optimize alias generation in batches, etc
- Cache
- CORS check
- API versioning, validation
- More application services in controllers (?)
- Alias expiration - cleanup expired links
- Users
- Port exposure for other Cassandra nodes on the same machine
- Option<T> usage instead of nullable types
- Access modifiers

