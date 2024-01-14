let interval;
let map;
let marker;
let driverMarker;
let destinationMarker = null;
let sharingLocation;

const button = document.getElementById('locationButton');
const input = document.getElementById('destination');
const destinationForm = document.getElementById("destination-form");

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
            const newClientPosition = [newPosition.coords.longitude, newPosition.coords.latitude];
            marker.setLngLat(newClientPosition);
            sendLocationToServer(newClientPosition[0], newClientPosition[1]);
        },
        error => {
            console.error('Error getting client location:', error);
        },
        {
            enableHighAccuracy: true
        }
    );
}

function updateDestination(destination, longitude, latitude, visibility) {
    const data = {
        newDestination: destination,
        newLongitude: longitude,
        newLatitude: latitude,
        newVisibility: visibility,
    };

    const xhr = new XMLHttpRequest();
    xhr.open('POST', '/Client/UpdateDestination', true);
    xhr.setRequestHeader('Content-Type', 'application/json');

    const jsonData = JSON.stringify(data);
    xhr.send(jsonData);
}

function removeDestinationMarker() {
    if (destinationMarker) {
        destinationMarker.remove();
        destinationMarker = null;
    }
}

function toggleLocationSharing() {

    let longitude = 90;
    let latitude = 90;

    if (!sharingLocation) {
        sharingLocation = true;
        startSharingLocation();
        destinationForm.style.visibility = "hidden";

        console.log("selectedPlace: ", selectedPlace);

        if (selectedPlace) {
            longitude = selectedPlace.lon;
            latitude = selectedPlace.lat;
            addDestinationMarker();
            map.flyTo({ center: destinationMarker.getLngLat(), zoom: 13 });
        }

        updateDestination(input.value, longitude, latitude, true);
    }
    else {
        // Stop sharing location
        destinationForm.style.visibility = "visible";

        flyToUser();
        removeDestinationMarker();
        sharingLocation = false;
        button.innerHTML = 'Request Taxi';
        clearInterval(interval);
        updateDestination("", 90, 90, false);

        if (driverMarker) driverMarker.remove();
    }
}

function startSharingLocation() {
    button.innerHTML = 'Cancel Request';
    interval = setInterval(function () {
        clientClaimedBy().then(driver => {
            if (driver) {
                updateDriverMarker(driver);
            }
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

    const driverPosition = [driver.user.location.longitude, driver.user.location.latitude];

    if (driverMarker && driverMarker.getElement() && driverMarker.getPopup()) {
        driverMarker.setLngLat(driverPosition);

        const driverMarkerDiv = driverMarker.getPopup().getElement();

        // Check if the required elements inside the popup exist
        if (driverMarkerDiv) {
            const plateNumber = driverMarkerDiv.querySelector('p.plate');
            const name = driverMarkerDiv.querySelector('p.name');

            // Update the elements if they exist
            if (plateNumber) plateNumber.innerText = driver.plateNumber;
            if (name) name.innerText = driver.user.fullName;

        }
    }
    else {
        let driverMarkerDiv = document.createElement('div');
        driverMarkerDiv.innerHTML =
            `
                    <p class="plate">${driver.plateNumber}</>
                    <p class="name">${driver.user.fullName}</p>
                `;

        let driverMarkerPopup = new tt.Popup({
            closeButton: false,
            offset: 25,
            anchor: 'bottom'
        }).setDOMContent(driverMarkerDiv);

        let driverMarkerBorder = document.createElement('div');
        driverMarkerBorder.className = 'marker-border driver-marker';

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
                else {
                    destinationForm.style.visibility = "visible";
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
            if (destination.location.longitude !== 90) {
                input.value = destination.name;
                selectedPlace = { display_name: destination.name, lon: destination.location.longitude, lat: destination.location.latitude };
                console.log(destination)
            }
        })
        .catch(error => {
            console.error('Error fetching client:', error);
        });
}

function addDestinationMarker() {
    let destinationPosition = [selectedPlace.lon, selectedPlace.lat];

    let markerDiv = document.createElement('div');
    markerDiv.innerHTML =
        `
        <p class="destinationName">${selectedPlace.display_name}</p>
    `;

    let markerPopup = new tt.Popup({
        closeButton: false,
        offset: 25,
        anchor: 'bottom'
    }).setDOMContent(markerDiv);

    let markerBorder = document.createElement('div');
    markerBorder.className = ' destination-marker marker-border';

    let markerIcon = document.createElement('div');
    markerIcon.className = 'marker-icon';
    markerIcon.style.backgroundImage = 'url(/images/destination.png)';
    markerBorder.appendChild(markerIcon);

    destinationMarker = new tt.Marker({
        element: markerBorder
    }).setLngLat(destinationPosition).setPopup(markerPopup);

    destinationMarker.addTo(map);
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
                pitch: 60
            });
            let div = document.createElement('div');
            div.innerHTML = '<p>You</p>';

            let popup = new tt.Popup({
                closeButton: false,
                offset: 25,
                anchor: 'bottom'
            }).setDOMContent(div);

            let border = document.createElement('div');
            border.className = 'marker-border client-marker';

            let icon = document.createElement('div');
            icon.className = 'marker-icon';
            icon.style.backgroundImage = 'url(/images/client-icon.png)';
            border.appendChild(icon);

            marker = new tt.Marker({
                element: border
            }).setLngLat(userPosition).setPopup(popup);

            marker.addTo(map);
            map.on('load', function () {
                requestAnimationFrame(rotateCamera);
            });

        },
        error => {
            console.error('Error getting user location:', error);
        },
        {
            enableHighAccuracy: true
        }

    );

    isClientVisible();
    getClientDestination();
}

function rotateCamera(timestamp) {

    if (sharingLocation) {
        if (selectedPlace) {
            console.log("yes");

            addDestinationMarker();
            map.flyTo({ center: destinationMarker.getLngLat(), zoom: 13 });
        }

        return;
    }

    var rotationDegree = timestamp / 100 % 360;
    map.rotateTo(rotationDegree, { duration: 0 });
    requestAnimationFrame(rotateCamera);
}

function flyToUser() {
    map.flyTo({ center: marker.getLngLat(), zoom: 13 });
}


window.onload = initMap;

// Close autocomplete results if user clicks outside the input and results
window.addEventListener("click", (event) => {
    if (!event.target.matches("#destination") && !event.target.matches(".autocomplete-item")) {
        document.getElementById("autocomplete-results").style.display = "none";
    }
});