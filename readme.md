# WLED Animated API

A simple API to provide more features on top of WLEDs APIs themselves.

A useful tool to self host via docker, or the executable (windows, linux or mac).  

## Demos
https://youtube.com/playlist?list=PLdmz103HwzOQKeGUdDre6nMHIgjRyUPAD&si=xWRNNLp4sn7vH5wU



## Core Features:

1. Upload and Play Animations
	1. Animations are zip files
	1. Optional animated or not animate images
	1. Animation.json file to instruct the app as to what to do
	1. Samples in the "Animations" folder
	1. WLED HTTP API 
	1. Across multiple devices
	1. Scrolling text options (see below)

2. Scan the local network for other WLED devices and provide a UI to view the list

3. Lots of options for scrolling text
	1. Basic text
	1. Plugin system
		1. [Things to do when Bored]()
		1. [Crypto Markets trading price](https://youtube.com/shorts/akgyh8ZLbqE?si=7CibZ1xyk1kBUBAW)
		1. [Customizable formatted date time](https://youtube.com/shorts/ngPygW_XpbI?si=xkq6Gq4V81VrTEXD)
		1. [Quotes](https://youtube.com/shorts/x73_-OVXjjI?si=KiuSWRdG9IEos_Xo)
   		1. [Weather](https://youtube.com/shorts/K3KJeIrk0qM?si=uLeiZKgxX6nGDImm)
		1. Prusa Printer Support
			1. Based on the printer state changes, run an animation
			1. Run animations to update your WLED device based on the current state of your printer, templated of course!
		1. Write your own - we implemented a plugin system.  Just drop your DLL in!

4. Upload or provide a URL an image
	1. we will auto scale the image to your provide size and display it on the target devices

5. Canned images for weather (work in progress)
	1. Mostly we need a set of nice animated images for all types of weather

6. A built in scheduler which can run animations
	1. for example, create a simple animation which just updates the WLED device with the current time
	1. run that animation every minute
	1. or weather every hour
	1. pick any WLED preset or effect and combine that with animated gifs or scrolling text targeting one or more WLED devices on your network!

A good example is you can upload your custom animations and trigger them from, say home assistant.


## Support
We fully support the current WLED HTTP JSON Api, along with two UDP Protocols: DNRGB and TPM2NET.  It has been tested on a 8x32, 16x32, 32x32, 16x16 WLED devices running 14.x. Ideally more folks would test on a variety of matrices.  


## Get it!

You can download one of the releases on the release tab, or via docker:

## Docker

### Windows:


Run with your own custom scheduler configuration

```cmd
docker run  --restart unless-stopped -d -p 8080:8080/tcp -v %cd%/Schedule.json:/app/Schedule.json --name wledanimateapi robchartier/wledanimateapi
```

Run with your own custom scheduler configuration and canned animations

```cmd
docker run  --restart unless-stopped  -d -p 8080:8080/tcp -v %cd%/Schedule.json:/app/Schedule.json -v %cd%/Animations:/app/Animations  --name wledanimateapi robchartier/wledanimateapi
````

### Non-Windows (linux/mac):

Run with your own custom scheduler configuration

```bash
docker run  --restart unless-stopped -d -p 8080:8080/tcp -v $(pwd)/Schedule.json:/app/Schedule.json --name wledanimateapi robchartier/wledanimateapi
```

Run with your own custom scheduler configuration and canned animations
```bash
docker run  --restart unless-stopped -d -p 8080:8080/tcp -v $(pwd)/Schedule.json:/app/Schedule.json -v $(pwd)/Animations:/app/Animations --name wledanimateapi robchartier/wledanimateapi
```

### TimeZone:
You will most likey need to add to the above arguments your timezone, especially if you want to generate the date/time scrolling text, this is done by adding

```
-e TZ=Europe/London

```

Find your timezone here: https://en.wikipedia.org/wiki/List_of_tz_database_time_zones

For example:
```bash
docker run -e TZ=Europe/London  --restart unless-stopped -d -p 8080:8080/tcp -v $(pwd)/Schedule.json:/app/Schedule.json -v $(pwd)/Animations:/app/Animations --name wledanimateapi robchartier/wledanimateapi
```

We are defaulting to America/Vancouver in the image.

