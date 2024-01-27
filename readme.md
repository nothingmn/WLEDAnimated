# WLED Animated API

A simple API to provide more features on top of WLEDs APIs themselves.

A useful tool to self host via docker, or the executable (windows, linux or mac).  


## Core Features:

1. Upload and Play Animations
	a. Animations are zip files
	b. Optional animated or not animate images
	c. Animation.json file to instruct the app as to what to do
	d. Samples in the "Animations" folder
	e. WLED HTTP API 
	f. across multiple devices
	g. Scrolling text options (see below)

2. Scan the local network for other WLED devices and provide a UI to view the list

3. Lots of options for scrolling text
	a. Basic text
	b. Plugin system
		i. Things to do when Bored 
		ii. Crypto Markets trading price
		iii. Customizable formatted date time
		iv. Quotes
		v. Weather
		vi. Write your own!

4. Upload or provide a URL an image
	a. we will auto scale the image to your provide size and display it on the target devices

5. Canned images for weather (work in progress)
	a. Mostly we need a set of nice animated images for all types of weather

6. A built in scheduler which can run animations
	a. for example, create a simple animation which just updates the WLED device with the current time
	b. run that animation every minute
	c. or weather every hour
	

A good example is you can upload your custom animations and trigger them from, say home assistant.


## Support
We fully support the current WLED HTTP JSON Api, along with two UDP Protocols: DNRGB and TPM2NET.  It has been tested on a 8x32, 16x32 WLED devices running 14.x. Ideally more folks would test on a variety of matrices.  


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

