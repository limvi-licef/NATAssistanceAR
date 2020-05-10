var CAMERA = "CAMERA";

//#################################################################
//#################################################################

// js

var frameCanvas=null, frameCanvasContext;

var video = null;
var cameraRunning = false;
var cameraPlay = false;

var jpgLoss = 0.3;
var resolutions = {
    "HD": {width: 1280, height: 720},
    "FullHD": {width: 1920, height: 1080},
    "Low": {width: 640, height: 480}
};

// html

var runResetButton = document.getElementById("runReset");
var snapshotButton = document.getElementById("snapshot");
var playButton = document.getElementById("play");
var stopButton = document.getElementById("stop");
var lossSelector = document.getElementById("loss");

var resolutionSelector = document.getElementById("resolutionSelector");
var cameraFrame = document.getElementById("cameraFrame");

snapshotButton.disabled = true;
playButton.disabled = true;
stopButton.disabled = true;


//#################################################################
//#################################################################
// Called once the webcam is enbled by user
function onStart(stream)
{
	video.srcObject = stream; // videostream is sent to video object
    setTimeout(createCanvas, 1000);
}


//#################################################################
// Create a canvas that is used for getting frames
function createCanvas()
{
	frameCanvas = document.createElement("canvas"); // create the canvas
	
    // set properties
	frameCanvas.id = "frameCanvas";
	frameCanvas.width = video.videoWidth;      // width and height according
	frameCanvas.height = video.videoHeight;    // to the webcam
    frameCanvasContext = frameCanvas.getContext('2d'); // the context is used for receiving the videostream
	
	cameraFrame.appendChild(frameCanvas);
    
    // update buttons
	runResetButton.innerHTML = "Reset";
	runResetButton.disabled = false;
	snapshotButton.disabled = false;
	playButton.disabled = false;
    
    // authorize snapshot, play and getFrame
    cameraRunning = true;
}


//#################################################################
// Take and display a snapshot
function snapshot()
{
    if (cameraRunning)
    {
        frameCanvasContext.drawImage(video, 0,0); // put a frame in the canvas
        if (cameraPlay) setTimeout(snapshot, 0); // recall the snapshot for play mode
    }
}


//#################################################################
// Display the video stream in the canvas
function play()
{
    if (cameraRunning && !cameraPlay)
    {
        // update buttons
        snapshotButton.disabled = true;
        playButton.disabled = true;
        stopButton.disabled = false;

        // loop of snapshots
        cameraPlay = true;
        snapshot();
    }
}


//#################################################################
// Stop to display the video stream
function stop()
{
    if (cameraRunning && cameraPlay)
    {
        cameraPlay = false; // break the loop of snapshots
    
        // update buttons
        snapshotButton.disabled = false;
        playButton.disabled = false;
        stopButton.disabled = true;
    }
}


//#################################################################
// Take a snapshot and return the frame as jpg
function getFrame()
{
    jpgLoss = parseFloat(lossSelector.value);
    if (cameraRunning)
    {
        frameCanvasContext.drawImage(video, 0,0);
        return frameCanvas.toDataURL('image/jpeg', jpgLoss);
    }
}


//#################################################################
// For activate or reset the camera
function runReset()
{
	if (video==null) // first clic
	{
        // get resolution for webcam specs
        var res = resolutions[resolutionSelector.value]; 
		var specs = {video: {width: res.width, height: res.height}};
		
        // create the video element and start the webcam
		video = document.createElement("video");
		video.autoplay = true;
		var webcam = navigator.mediaDevices.getUserMedia(specs);
		webcam.then(onStart); // onStart called when the user enable the webcam
		
        // update buttons
		runResetButton.innerHTML = "Await";
		runResetButton.disabled = true;
	}
	else // second clic = reset the camera
	{
		location.href = ""; // reload page
	}
}