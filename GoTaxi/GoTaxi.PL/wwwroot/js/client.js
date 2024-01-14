let interval;
let map;
let marker;
let driverMarker;
let sharingLocation;

isClientVisible();


const button = document.getElementById('locationButton');
const input = document.getElementById('destination');

function sendLocationToServer(longitude, latitude) {
    const locationData = {
        longitude: longitude,
        latitude: latitude
    };

    const xhr = new XMLHttpRequest();
    xhr.open('POST', '/Client/UpdateLocation', true);
    xhr.setRequestHeader('Content-Type', 'application/json');

    const jsonData = JSON.stringify(locationData);
    xhr.send(jsonData);
}

function shareLocation() {
    navigator.geolocation.getCurrentPosition(
        newPosition => {
            const newDriverPosition = [newPosition.coords.longitude, newPosition.coords.latitude];
            marker.setLngLat(newDriverPosition);
            sendLocationToServer(newDriverPosition[0], newDriverPosition[1]);
        },
        error => {
            console.error('Error getting user location:', error);
        },
        {
            enableHighAccuracy: true
        }
    );
}

function updateDestination(destination, visibility) {
    const data = {
        newDestination: destination,
        newVisibility: visibility,
    };

    const xhr = new XMLHttpRequest();
    xhr.open('POST', '/Client/UpdateDestination', true);
    xhr.setRequestHeader('Content-Type', 'application/json');

    const jsonData = JSON.stringify(data);
    xhr.send(jsonData);

    getResults(destination);
}

function toggleLocationSharing() {

    if (!sharingLocation) {
        // Start sharing location
        sharingLocation = true;
        startSharingLocation();
        updateDestination(input.value, true);

    }
    else {
        // Stop sharing location
        sharingLocation = false;
        button.innerHTML = 'Request Taxi';
        clearInterval(interval);
        sendLocationToServer(90, 90);
        updateDestination("", false);

        if (driverMarker) driverMarker.remove();
    }
}

function startSharingLocation() {
    button.innerHTML = 'Cancel Request';
    interval = setInterval(function () {
        clientClaimedBy().then(driver => {
            if (driver != null) updateDriverMarker(driver);
            shareLocation();
        });
    }, 6000); // Repeat every 6 seconds
}

function clientClaimedBy() {
    return fetch(`/Client/ClientClaimedBy`)
        .then(response => response.json())
        .catch(error => {
            console.error('Error fetching client:', error);
        });
}

function updateDriverMarker(driver) {

    const driverPosition = [driver.longitude, driver.latitude];

    if (driverMarker && driverMarker.getElement() && driverMarker.getPopup()) {
        driverMarker.setLngLat(driverPosition);

        const driverMarkerDiv = driverMarker.getPopup().getElement();

        // Check if the required elements inside the popup exist
        if (driverMarkerDiv) {
            const plateNumber = driverMarkerDiv.querySelector('p.plate');
            const name = driverMarkerDiv.querySelector('p.name');

            // Update the elements if they exist
            if (plateNumber) plateNumber.innerText = driver.plateNumber;
            if (name) name.innerText = driver.fullName;

        }
    }
    else {
        let driverMarkerDiv = document.createElement('div');
        driverMarkerDiv.innerHTML =
            `
                    <p class="plate">${driver.plateNumber}</>
                    <p class="name">${driver.fullName}</p>
                `;

        let driverMarkerPopup = new tt.Popup({
            closeButton: false,
            offset: 25,
            anchor: 'bottom'
        }).setDOMContent(driverMarkerDiv);

        let driverMarkerBorder = document.createElement('div');
        driverMarkerBorder.className = 'marker-border';
        driverMarkerBorder.style.background = 'yellow';

        let driverMarkerIcon = document.createElement('div');
        driverMarkerIcon.className = 'marker-icon';
        driverMarkerIcon.style.backgroundImage = 'url(/images/taxi-icon.png)';
        driverMarkerBorder.appendChild(driverMarkerIcon);

        driverMarker = new tt.Marker({
            element: driverMarkerBorder
        }).setLngLat(driverPosition).setPopup(driverMarkerPopup);


        driverMarker.addTo(map);
    }
}

function isClientVisible() {
    fetch(`/Client/IsClientVisible`)
        .then(response => response.json())
        .then(visibility => {
            if (visibility != null) {
                sharingLocation = visibility;

                if (sharingLocation) {
                    startSharingLocation();
                }
            }
        })
        .catch(error => {
            console.error('Error fetching client:', error);
        });
}

function getClientDestination() {
    fetch(`/Client/GetClientDestination`)
        .then(response => response.json())
        .then(destination => {
            if (destination != null) {
                input.value = destination;
            }
        })
        .catch(error => {
            console.error('Error fetching client:', error);
        });
}

function initMap() {

    navigator.geolocation.getCurrentPosition(
        position => {
            const userPosition = [position.coords.longitude, position.coords.latitude];
            sendLocationToServer(userPosition[0], userPosition[1]);

            map = tt.map({
                key: "MVjOYcUAh8yzcRi8zYnynWAhvqtASz8G",
                container: 'map',
                center: userPosition,
                style: 'https://api.tomtom.com/style/1/style/22.2.1-*?map=2/basic_street-dark&poi=2/poi_dark',
                zoom: 13,
            });
            let div = document.createElement('div');
            div.innerHTML = '<p>You</p>';

            let popup = new tt.Popup({
                closeButton: false,
                offset: 25,
                anchor: 'bottom'
            }).setDOMContent(div);

            let border = document.createElement('div');
            border.className = 'marker-border';

            let icon = document.createElement('div');
            icon.className = 'marker-icon';
            icon.style.backgroundImage = 'url(/images/client-icon.png)';
            border.appendChild(icon);

            marker = new tt.Marker({
                element: border
            }).setLngLat(userPosition).setPopup(popup);

            marker.addTo(map);
        },
        error => {
            console.error('Error getting user location:', error);
        },
        {
            enableHighAccuracy: true
        }

    );

    getClientDestination();
}

window.onload = initMap;