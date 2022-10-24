# URLShortener

## Running locally

To run the API, you need an instance (or multiple instances) of Cassandra. You can find instructions on how to spin up a Cassandra cluster using Docker below.

## Local Cassandra cluster
### Docker CLI

Run the following commands from the root of the repository:

```
docker network create url-shortener-network
# The following 2 commands will start 2 Cassandra nodes. It will take about a minute for them to initialize and migrations will fail until nodes are initialized
docker run --rm -d -p 9042:9042 --name cassandra-node1 --network url-shortener-network cassandra:4.0.6
docker run --rm -d --name cassandra-node2 --network url-shortener-network -e CASSANDRA_SEEDS=cassandra-node1 cassandra:4.0.6

docker run --rm -d --name zookeeper --network url-shortener-network -e "ZOOKEEPER_CLIENT_PORT=2181" -e "ZOOKEEPER_TICK_TIME=2000" confluentinc/cp-zookeeper:7.0.0
docker run --rm -d --name broker --network url-shortener-network -p 9092:9092 -e "KAFKA_BROKER_ID=1" -e "KAFKA_ZOOKEEPER_CONNECT=zookeeper:2181" -e "KAFKA_LISTENER_SECURITY_PROTOCOL_MAP=PLAINTEXT:PLAINTEXT,PLAINTEXT_INTERNAL:PLAINTEXT" -e "KAFKA_ADVERTISED_LISTENERS=PLAINTEXT://host.docker.internal:9092,PLAINTEXT_INTERNAL://broker:29092" -e "KAFKA_OFFSETS_TOPIC_REPLICATION_FACTOR=1" -e "KAFKA_TRANSACTION_STATE_LOG_MIN_ISR=1" -e "KAFKA_TRANSACTION_STATE_LOG_REPLICATION_FACTOR=1" confluentinc/cp-kafka:7.0.0

docker exec broker kafka-topics --create --topic alias_candidates --bootstrap-server host.docker.internal:9092 --replication-factor 1 --partitions 1

docker build --tag cassandra-migrate migrations
docker run --rm -it -v %cd%/migrations:/migrations --network url-shortener-network cassandra-migrate cassandra-migrate -H host.docker.internal -c migrations/config.yml migrate

docker build -f URLShortener.WebUI/Dockerfile -t url-shortener .
docker run --rm -d --network url-shortener-network -e "ASPNETCORE_ENVIRONMENT=Development" -e "ASPNETCORE_URLS=http://+:80" -p 9889:80/tcp --name url-shortener url-shortener

docker build -f URLShortener.Generator/Dockerfile -t url-shortener-generator .
docker run --rm -d --network url-shortener-network -e "DOTNET_ENVIRONMENT=Development" --name url-shortener-generator url-shortener-generator
```

If all is well, you should be able to access the API at http://localhost:9889/swagger/index.html

To clean up and exit containers, run the following commands:
```
docker kill cassandra-node1
docker kill cassandra-node2
docker kill zookeper
docker kill broker
docker kill url-shortener
docker kill url-shortener-generator
```

### Docker Compose (outdated)
https://github.com/ch200c/URLShortener/blob/fc2044be0b7bc389f1a88b915973b1f891c17509/docker-compose.yml


## TODO
- Alias expiration - cleanup expired links
- Users
- Port exposure for other Cassandra nodes on the same machine
- Folder structure
- Access modifiers

