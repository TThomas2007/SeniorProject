//var apiKey = "46493732";
//var sessionId = "2_MX40NjQ5MzczMn5-MTU3OTgxNzc4ODkzOH4xUitvcDZmNGRxcW5ySWxHNGt5UWlPa3J-fg";
//var token = "T1==cGFydG5lcl9pZD00NjQ5MzczMiZzaWc9YzU2Y2QwZGZlM2E3NjJjMTVmZTYxYjUyNWIxNDU3MDIyYjI0NzE0NjpzZXNzaW9uX2lkPTJfTVg0ME5qUTVNemN6TW41LU1UVTNPVGd4TnpjNE9Ea3pPSDR4VWl0dmNEWm1OR1J4Y1c1eVNXeEhOR3Q1VVdsUGEzSi1mZyZjcmVhdGVfdGltZT0xNTc5ODE3ODAxJm5vbmNlPTAuODUwMzcwNDc2MzU4NzczOCZyb2xlPXB1Ymxpc2hlciZleHBpcmVfdGltZT0xNTc5ODIxNDAzJmluaXRpYWxfbGF5b3V0X2NsYXNzX2xpc3Q9";

var apiKey = "";
var sessionId = "";
var session = "";
var token = "";

// Handling all of our errors here by alerting them
function handleError(error) {
    if (error) {
        alert(error.message);
    }
}

//THIS CODE BRINGS IN NEW SESSION
var SERVER_BASE_URL = 'https://interviewvideochat.herokuapp.com/';
fetch(SERVER_BASE_URL + '/session').then(function (res) {
    return res.json()
}).then(function (res) {
    apiKey = res.apiKey;
    sessionId = res.sessionId;
    token = res.token;
    initializeSession();
    }).catch(handleError);



function initializeSession() {
   
    session = OT.initSession(apiKey, sessionId);
    // Subscribe to a newly created stream
    session.on('streamCreated', function (event) {
        session.subscribe(event.stream, 'subscriber', {
            insertMode: 'append',
            width: '100%',
            height: '100%'
        }, handleError);
    });

    // Create a publisher
    var publisher = OT.initPublisher('publisher', {
        insertMode: 'append',
        width: '100%',
        height: '100%'
    }, handleError);

    // Connect to the session
    session.connect(token, function (error) {
        // If the connection is successful, initialize a publisher and publish to the session
        if (error) {
            handleError(error);
        } else {
            session.publish(publisher, handleError);
        }
    });
    // Receive a message and append it to the history
    var msgHistory = document.querySelector('#history');
    session.on('signal:msg', function signalCallback(event) {
        var msg = document.createElement('p');
        msg.textContent = event.data;
        msg.className = event.from.connectionId === session.connection.connectionId ? 'mine' : 'theirs';
        msgHistory.appendChild(msg);
        msg.scrollIntoView();
    });
}


// Text chat
var form = document.querySelector('form');
var msgTxt = document.querySelector('#msgTxt');

// Send a signal once the user enters data in the form
form.addEventListener('submit', function submit(event) {
    event.preventDefault();

    session.signal({
        type: 'msg',
        data: msgTxt.value
    }, function signalCallback(error) {
        if (error) {
            console.error('Error sending signal:', error.name, error.message);
        } else {
            msgTxt.value = '';
        }
    });
});