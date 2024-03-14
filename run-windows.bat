cp ..\appsettings.json .
docker pull robchartier/wledanimateapi
docker run -d --restart unless-stopped -p 8080:8080/tcp -v %cd%/Schedule.json:/app/Schedule.json -v %cd%/Animations:/app/Animations -v %cd%/appsettings.json:/app/appsettings.json  --name wledanimateapi robchartier/wledanimateapi
docker ps -a