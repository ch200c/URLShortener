# URLShortener

## Running locally

To run the API, you need an instance (or multiple instances) of Cassandra. You can find instructions on how to spin up a Cassandra cluster using Docker below.

## Local Cassandra cluster
### Docker CLI

Run the following commands from the root of the repository:

```
docker network create cassandra-network
# The following 2 commands will start 2 Cassandra nodes. It will take about a minute for them to initialize and migrations will fail until nodes are initialized
docker run --rm -d -p 9042:9042 --name cassandra-node1 --network cassandra-network cassandra:4.0.6
docker run --rm -d --name cassandra-node2 --network cassandra-network -e CASSANDRA_SEEDS=cassandra-node1 cassandra:4.0.6

docker build --tag cassandra-migrate migrations
docker run --rm -it -v %cd%/migrations:/migrations --network cassandra-network cassandra-migrate cassandra-migrate -H host.docker.internal -c migrations/config.yml migrate

docker build -f URLShortener.WebUI/Dockerfile -t url-shortener .
docker run --rm -d -e "ASPNETCORE_ENVIRONMENT=Development" -e "ASPNETCORE_URLS=http://+:80" -p 9889:80/tcp --name url-shortener url-shortener
```

If all is well, you should be able to access the API at http://localhost:9889/swagger/index.html

To clean up and exit containers, run the following commands:
```
docker kill cassandra-node1
docker kill cassandra-node2
docker kill url-shortener
```

### Docker Compose (outdated)
https://github.com/ch200c/URLShortener/blob/fc2044be0b7bc389f1a88b915973b1f891c17509/docker-compose.yml


## TODO
- Alias expiration - cleanup expired links
- Users
- Port exposure for other Cassandra nodes on the same machine
- Folder structure
- Access modifiers