// Import and configure the Firebase SDK
// These scripts are made available when the app is served or deployed on Firebase Hosting
// If you do not serve/host your project using Firebase Hosting see https://firebase.google.com/docs/web/setup

importScripts('https://www.gstatic.com/firebasejs/8.3.2/firebase-app.js');
importScripts('https://www.gstatic.com/firebasejs/8.3.2/firebase-messaging.js');


firebase.initializeApp({
    apiKey: "AIzaSyAF3lsyJBHDwpdd2u9D0qW-m3c2TJftQvE",
    authDomain: "bachelor-it-97124.firebaseapp.com",
    databaseURL: "https://bachelor-it-97124-default-rtdb.europe-west1.firebasedatabase.app",
    projectId: "bachelor-it-97124",
    storageBucket: "bachelor-it-97124.appspot.com",
    messagingSenderId: "56089551498",
    appId: "1:56089551498:web:e88b4bd832bf7cd49a8373",
    measurementId: "G-73VFZE16YG"
});
//const messaging = firebase.messaging();



/**
 * Here is is the code snippet to initialize Firebase Messaging in the Service
 * Worker when your app is not hosted on Firebase Hosting.

 // [START initialize_firebase_in_sw]
 // Give the service worker access to Firebase Messaging.
 // Note that you can only use Firebase Messaging here, other Firebase libraries
 // are not available in the service worker.

 // Initialize the Firebase app in the service worker by passing in the
 // messagingSenderId.
 

 // Retrieve an instance of Firebase Messaging so that it can handle background
 // messages.
 const messaging = firebase.messaging();
 // [END initialize_firebase_in_sw]
 **/

class CustomPushEvent extends Event {
    constructor(data) {
        super('push')

        Object.assign(this, data)
        this.custom = true
    }
}

/*
 * Overrides push notification data, to avoid having 'notification' key and firebase blocking
 * the message handler from being called
 */
self.addEventListener('push', (e) => {
    // Skip if event is our own custom event
    if (e.custom) return;

    // Kep old event data to override
    let oldData = e.data

    // Create a new event to dispatch, pull values from notification key and put it in data key, 
    // and then remove notification key
    let newEvent = new CustomPushEvent({
        data: {
            ehheh: oldData.json(),
            json() {
                let newData = oldData.json()
                newData.data = {
                    ...newData.data,
                    ...newData.notification
                }
                delete newData.notification
                return newData
            },
        },
        waitUntil: e.waitUntil.bind(e),
    })

    // Stop event propagation
    e.stopImmediatePropagation()

    // Dispatch the new wrapped event
    dispatchEvent(newEvent)
})
const messaging = firebase.messaging();
messaging.onBackgroundMessage(function (payload) {
    const notificationTitle = payload.data.title;


    console.log("Inni background 1111: " + payload.body + " " + payload.title)
    const notificationOptions = {
        body: payload.data.body,
        icon: payload.data.icon
    };

    return self.registration.showNotification(notificationTitle,
        notificationOptions);
});

self.addEventListener('notificationclick', function (event) {
    self.clients.openWindow('https://github.com/Suketu-Patel')
    //close notification after click
    event.notification.close()
})

// If you would like to customize notifications that are received in the
// background (Web app is closed or not in browser focus) then you should
// implement this optional method.
// [START background_handler]
messaging.setBackgroundMessageHandler(function (payload) {
    console.log('[firebase-messaging-sw.js] Received background message her? LOLOL', payload);
    // Customize notification here

    /*
    var antallNot = document.getElementById("badge_counter").innerHTML;

    if (antallNot == "") {
        document.getElementById("badge_counter").innerHTML = 1;
    } else {
        var nyNot = parseInt(antallNot) + 1;
        document.getElementById("badge_counter").innerHTML = nyNot;
    };
    */
    console.log("inni background: " + payload.data.body + " " + payload.data.title)
    const notificationTitle = 'Background Message Title';
    const notificationOptions = {
        body: 'Background Message body.',
        icon: '/resources/Portfollowers.png'
    };

    return self.registration.showNotification(notificationTitle,
        notificationOptions);

    $.ajax({
        type: "GET",
        url: '@Url.Action("Test", "Home")',
    });
}
// [END background_handler]
