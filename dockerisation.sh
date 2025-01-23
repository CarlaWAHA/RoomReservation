mkdir -p docker
cd ./docker
touch Dockerfile
cat <<EQL > Dockerfile
FROM alpine
CMD ["echo", "Hello, Docker!"]
EQL
docker build -t myimage .
docker images
docker run myimage
mkdir -p data
cat <<EQL > data/index.html
<html>
<head>
	<title> Docker</title>
</head>
<body>
	<h1>Hello, Docker!</h1>
</body>
</html>
EQL
docker run -dit --name cdata -v "$(pwd)/../data:/app" myimage
docker exec cdata ls /app
docker exec cdata cat /app/index.html
docker exec -it cdata pwd
docker exec -it cdata ls /app
docker exec -it cdata cat /app/index.html
docker stop cdata
docker rm cdata
docker image prune -f
docker container prune -f

