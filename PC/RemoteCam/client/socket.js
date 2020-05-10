var SOCKET = "SOCKET";

//#################################################################

var socket = null;
var socketConnected = false;

var connectionButton = document.getElementById("connectionButton");
var closeButton = document.getElementById("closeButton");

closeButton.disabled = true;

//#################################################################
// Create a socket and try to connect
function openSocket()
{
	if (!socketConnected)
	{
        // build url from html input
		var host = document.getElementById("host").value;
		var port = document.getElementById("port").value;
		var url = "ws://"+host+":"+port;
		
		try
		{
			socket = new WebSocket(url); // open socket
			
            // callbacks
			socket.onerror = onError;
			socket.onopen = onOpen;
		}
		catch (exception)
		{
			console.error(exception);
		}
	}
}


//#################################################################
// Close the websocket connection
function closeSocket()
{
	if (socketConnected)
	{
		socket.close();
		socket = null;
	}
}

	
//#################################################################
// Called when the socket is connected
function onOpen(event)
{
    
	socket.onmessage = onFrameMessage;
    socket.onclose = onClose;
	socket.ready = true;
    socketConnected = true;
    
    console.log("Connected.");
    
    // update buttons
	connectionButton.disabled = true;
	closeButton.disabled = false;
}


//#################################################################
// Default action when receive a message
function messageToConsole(event)
{
	console.log("Recv: ", event.data);
}


//#################################################################
// Called when an error occurs on the socket
function onError(error)
{
	console.log("Connection failed.");
    console.error(error);
    
    socketConnected = false;
    
    // update buttons
	connectionButton.disabled = false;
	closeButton.disabled = true;
}


//#################################################################
// Called when the connection is closing
function onClose(event)
{
	console.log("Connection closed.");
    
    socketConnected = false;
    
    // update buttons
	connectionButton.disabled = false;
	closeButton.disabled = true;
}


//#################################################################
// Called when the connection is closing
function onFrameMessage(event)
{
	socket.send(getFrame());
}