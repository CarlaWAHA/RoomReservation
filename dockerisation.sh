cd .\src
cd .\Docker
docker build -t hello-docker .
docker images
docker run hello-docker
cd .\data
echo. > index.html
echo ^<html^>^<head^>^<title^>Hello^</title^>^</head^>^<body^>^<h1^>Hello, World!^</h1^>^</body^>^</html^> > index.html
docker run -dit -- name mycontainer -v "C:\Users\PC\OneDrive\Documents\RoomReservation\src\Data:/app" alpine
docker exec -it mycontainer sh
ls
cd ./app
pwd
ls
cat index.html
exit
docker stop mycontainer
docker rm mycontainer
docker image prune -a
docker container prune


