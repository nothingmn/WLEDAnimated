#https://learn.microsoft.com/en-us/dotnet/core/docker/build-container?tabs=linux&pivots=dotnet-8-0

dotnet publish -c Release WLEDAnimated.API

docker build -t wledanimateapi -f Dockerfile .

docker images

docker create --name wledanimateapi wledanimateapi

rem docker run --rm -d -p 8080:8080/tcp --name wledanimateapi wledanimateapi

docker run  --restart unless-stopped  -d -p 8080:8080/tcp -v %cd%/Schedule.json:/app/Schedule.json -v %cd%/Animations:/app/Animations  --name wledanimateapi wledanimateapi

rem docker run --rm -d -p 8080:8080/tcp -v %cd%/Schedule.json:/app/Schedule.json -v %cd%/Animations:/app/Animations  --name wledanimateapi robchartier/wledanimateapi

rem docker login --username robchartier
rem docker tag wledanimateapi:latest robchartier/wledanimateapi:latest
rem docker push robchartier/wledanimateapi:latest



