let map;
let currentMarker;
let userPosition;

let driverMarkers = [];
let clientMarkers = [];

let clientIsInTheCar = false;

let claimedClient = null;
let claimedClientMarker = null;
let destinationMarker = null;


function addDriverMarker(driver) {
    const driverPosition = [driver.user.location.longitude, driver.user.location.latitude];

    let markerDiv = document.createElement('div');
    markerDiv.innerHTML =
        `
        <p class="driverName">${driver.user.fullName}</p>
        <p class="plateNumber">${driver.plateNumber}</p>
    `;


    let markerPopup = new tt.Popup({
        closeButton: false,
        offset: 25,
        anchor: 'bottom'
    }).setDOMContent(markerDiv);

    let markerBorder = document.createElement('div');
    markerBorder.className = 'marker-border secondary-driver-marker';

    let markerIcon = document.createElement('div');
    markerIcon.className = 'marker-icon';
    markerIcon.style.backgroundImage = 'url(/images/taxi-icon.png)';
    markerBorder.appendChild(markerIcon);

    const newMarker = new tt.Marker({
        element: markerBorder
    }).setLngLat(driverPosition).setPopup(markerPopup);

    newMarker.addTo(map);
    newMarker.driverPlateNumber = driver.plateNumber; // Store driver plate number in the marker

    driverMarkers.push(newMarker);
}

function addClientMarker(client) {
    const clientPosition = [client.user.location.longitude, client.user.location.latitude];

    let markerDiv = document.createElement('div');
    markerDiv.innerHTML =
        `   <p class="destination">${client.destination.name}</p>
        <p class="name>${client.user.fullName}</p>
        <p class="email>${client.user.email}</p>
        <p class="phone">${client.phoneNumber}</h6>
        <p class="reports">Reports: ${client.reports}</p>
        <button id="claim-button" onclick="claimClient('${client.phoneNumber}')">Claim</button>
    `;

    markerDiv.className = 'marker-info';

    let markerPopup = new tt.Popup({
        closeButton: false,
        offset: 25,
        anchor: 'bottom'
    }).setDOMContent(markerDiv);
    markerPopup.className = 'marker-popup';

    let markerBorder = document.createElement('div');
    markerBorder.className = 'marker-border client-marker';

    let markerIcon = document.createElement('div');
    markerIcon.className = 'marker-icon';
    markerIcon.style.backgroundImage = 'url(/images/client-icon.png)';
    markerBorder.appendChild(markerIcon);

    const newMarker = new tt.Marker({
        element: markerBorder
    }).setLngLat(clientPosition).setPopup(markerPopup);

    newMarker.addTo(map);
    newMarker.clientPhoneNumber = client.phoneNumber; // Store client phone number in the marker
    newMarker.destination = client.destination.name;

    clientMarkers.push(newMarker);
}

function updateDriverMarker(marker, driver) {
    const driverPosition = [driver.user.location.longitude, driver.user.location.latitude];

    if (marker && marker.getElement() && marker.getPopup()) {
        marker.setLngLat(driverPosition);

        const markerDiv = marker.getPopup().getElement();

        if (markerDiv) {
            const name = markerDiv.querySelector('p.driverName');
            const plateNumber = markerDiv.querySelector('p.plateNumber');

            if (name) name.innerText = driver.user.fullName;
            if (plateNumber) plateNumber.innerText = driver.plateNumber;
        }
    } else {
        console.error('Invalid marker or missing elements:', marker);
    }
}

function updateClientMarker(marker, client) {
    const clientPosition = [client.user.location.longitude, client.user.location.latitude];

    // Check if the marker is defined and has an element and popup
    if (marker && marker.getElement() && marker.getPopup()) {
        marker.setLngLat(clientPosition);

        const markerDiv = marker.getPopup().getElement();
        // Check if the required elements inside the popup exist
        if (markerDiv) {
            const destination = markerDiv.querySelector('p.destination');
            const name = markerDiv.querySelector('p.name');
            const email = markerDiv.querySelector('p.email');
            const phoneNumber = markerDiv.querySelector('p.phone');
            const reports = markerDiv.querySelector('p.reports');
            const button = markerDiv.querySelector('button');

            // Update the elements if they exist
            if (destination) destination.innerText = client.destination.name;
            if (name) name.innerText = client.user.fullName;
            if (email) email.innerText = client.user.email;
            if (phoneNumber) phoneNumber.innerText = client.phoneNumber;
            if (reports) reports.innerText = `Reports: ${client.reports}`;
            if (button) button.onclick = function () { claimClient(client.phoneNumber); };

        }
    } else {
        console.error('Invalid marker or missing elements:', marker);
    }
}

function updateDriverMarkers(newDrivers) {
    const existingDriverPlateNumbers = driverMarkers.map(marker => marker.driverPlateNumber);
    const newDriverPlateNumbers = newDrivers.map(driver => driver.plateNumber);

    // Remove markers for drivers that are not in the new list
    const driversToRemove = driverMarkers.filter(marker => !newDriverPlateNumbers.includes(marker.driverPlateNumber));
    driversToRemove.forEach(marker => {
        marker.remove();
        driverMarkers.splice(driverMarkers.indexOf(marker));
    });

    // Update or add markers for the new drivers
    newDrivers.forEach(driver => {
        const index = existingDriverPlateNumbers.indexOf(driver.plateNumber);
        if (index !== -1) {
            // Update existing marker
            updateDriverMarker(driverMarkers[index], driver);
        } else {
            // Add new marker
            addDriverMarker(driver);
        }
    });
}

function updateClientMarkers(newClients) {
    const existingClientPhoneNumbers = clientMarkers.map(marker => marker.clientPhoneNumber);
    const newClientPhoneNumbers = newClients.map(client => client.phoneNumber);

    console.log(newClients);

    // Remove markers for clients that are not in the new list
    const clientsToRemove = clientMarkers.filter(marker => !newClientPhoneNumbers.includes(marker.clientPhoneNumber));
    clientsToRemove.forEach(marker => {
        marker.remove();
        clientMarkers.splice(clientMarkers.indexOf(marker));

    });

    // Update or add markers for the new clients
    newClients.forEach(client => {
        const index = existingClientPhoneNumbers.indexOf(client.phoneNumber);

        if (index !== -1) {
            // Update existing marker
            updateClientMarker(clientMarkers[index], client);
        } else {
            // Add new marker
            addClientMarker(client);
        }
    });
}

function getNearestDrivers(currentPosition) {
    fetch(`/Taxi/GetNearestDrivers?currentDriverLongitude=${currentPosition[0]}&currentDriverLatitude=${currentPosition[1]}`)
        .then(response => response.json())
        .then(nearestDrivers => {
            if (nearestDrivers.length > 0) {
                updateDriverMarkers(nearestDrivers);
            }
        })
        .catch(error => {
            console.error('Error fetching nearest drivers:', error);
        });
}

function getNearestClients(currentPosition) {
    fetch(`/Taxi/GetNearestClients?currentClientLongitude=${currentPosition[0]}&currentClientLatitude=${currentPosition[1]}`)
        .then(response => response.json())
        .then(nearestClients => {
            if (nearestClients.length > 0) {

                updateClientMarkers(nearestClients);
            }
        })
        .catch(error => {
            console.error('Error fetching nearest clients:', error);
        });
}

function addDestinationMarker() {
    clearMarkers();


    let destinationPosition = [claimedClient.destination.location.longitude, claimedClient.destination.location.latitude];

    let markerDiv = document.createElement('div');
    markerDiv.innerHTML =
        `
        <p class="destinationName">${claimedClient.destination.name}</p>
    `;

    let markerPopup = new tt.Popup({
        closeButton: false,
        offset: 25,
        anchor: 'bottom'
    }).setDOMContent(markerDiv);

    let markerBorder = document.createElement('div');
    markerBorder.className = 'marker-border destination-marker';

    let markerIcon = document.createElement('div');
    markerIcon.className = 'marker-icon';
    markerIcon.style.backgroundImage = 'url(/images/destination.png)';
    markerIcon.style.backgroundSize = 'contain';
    markerIcon.style.backgroundRepeat = 'no-repeat';
    markerBorder.appendChild(markerIcon);

    destinationMarker = new tt.Marker({
        element: markerBorder
    }).setLngLat(destinationPosition).setPopup(markerPopup);

    destinationMarker.addTo(map);
}

function clearMarkers(phoneNumber = 0) {
    function clearMarkers(phoneNumber = 0) {
        driverMarkers.forEach(marker => {
            marker.remove();
        });
        driverMarkers = [];

    clientMarkers = clientMarkers.filter(marker => {
        if ((marker.clientPhoneNumber !== phoneNumber) || clientIsInTheCar === true) {
            console.log("no", phoneNumber)
            marker.remove();
            return false;  // Exclude this marker from the new array
        }

        return true;  // Include this marker in the new array
    });
}

function deleteRoute() {
    // Check if the 'route' layer already exists and remove it
    if (map.getLayer('route')) {
        map.removeLayer('route');
    }

    // Check if the 'route' source already exists and remove it
    if (map.getSource('route')) {
        map.removeSource('route');
    }
}

function displayRoute(geoJSON) {

    if (!map.getLayer('route')) {

        map.addSource('route', {
            'type': 'geojson',
            'data': geoJSON
        });

        map.addLayer({
            'id': 'route',
            'type': 'line',
            'source': 'route',
            'paint': {
                'line-color': '#FFC100',
                'line-blur': 0,
                'line-width': 5,
            }
        });

    } else {
        map.getSource('route').setData(geoJSON);
    }
}

function claimClient(phoneNumber) {
    const xhr = new XMLHttpRequest();
    xhr.open('POST', '/Taxi/ClaimClient', true);
    xhr.setRequestHeader('Content-Type', 'application/json');

    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4) {
            if (xhr.status === 200) {
            } else {
                console.error('Error updating location. Status code:', xhr.status);
            }
        }
    };

    const jsonData = JSON.stringify(phoneNumber);
    xhr.send(jsonData);

    claimedClientMarker = clientMarkers.find(marker => marker.clientPhoneNumber === phoneNumber);

    checkMarkers();

    console.log(phoneNumber)
    clearMarkers(phoneNumber);
    isActive = true;
}

function createRouteToClient() {

    if (clientIsInTheCar) {
        if (!destinationMarker)
            addDestinationMarker();
    }
    else {
        if (!claimedClientMarker) {
            addClientMarker(claimedClient);
            claimedClientMarker = clientMarkers.find(marker => marker.clientPhoneNumber === claimedClient.phoneNumber);
        }
        else {
            updateClientMarker(claimedClientMarker, claimedClient);
        }
    }

    if ((claimedClientMarker || destinationMarker) && currentMarker) {

        let longitude = 90;
        let latitude = 90;

        if (clientIsInTheCar === true) {
            longitude = destinationMarker.getLngLat().lng;
            latitude = destinationMarker.getLngLat().lat;
        }
        else {
            longitude = claimedClientMarker.getLngLat().lng;
            latitude = claimedClientMarker.getLngLat().lat;
        }

        let routeOptions = {
            key: "MVjOYcUAh8yzcRi8zYnynWAhvqtASz8G",
            locations: [
                { lat: currentMarker.getLngLat().lat.toFixed(6), lon: currentMarker.getLngLat().lng.toFixed(6) },
                { lat: latitude.toFixed(6), lon: longitude.toFixed(6) }
            ]
        };

        console.log(routeOptions);

        tt.services
            .calculateRoute(routeOptions)
            .then(response => {

                displayRoute(response.toGeoJson());
            })
            .catch(error => {
                console.error('Error calculating route:', error);
            });

    } else {
        console.error('Claimed client marker or current driver marker not found.');
    }
}

function checkClaimedClient() {
    return new Promise((resolve) => {
        fetch(`/Taxi/CheckClaimedClient`)
            .then(response => response.json())
            .then(client => {

                if (client === null && claimedClient && claimedClientMarker) {
                    claimedClientMarker.remove();
                    claimedClientMarker = null;
                    clearMarkers();
                    deleteRoute();
                    clientIsInTheCar = false;
                }

                claimedClient = client;
                resolve(); // Resolve the promise once the asynchronous operation is complete
            })
            .catch(error => {
                console.error('Error fetching claimed client:', error);
                resolve(); // Resolve the promise even in case of an error
            });
    });
}

function isInTheCar() {
    return new Promise((resolve) => {
        fetch(`/Taxi/IsInTheCar?phoneNumber=${claimedClient.phoneNumber}`)
            .then(response => response.json())
            .then(result => {
                clientIsInTheCar = JSON.parse(result) && claimedClient.destination.location.longitude != 90 && claimedClient.destination.location.latitude != 90;
                console.log("In the car:", clientIsInTheCar);

                if (clientIsInTheCar === true && claimedClientMarker !== null) {
                    claimedClientMarker.remove();
                    claimedClientMarker = null;
                }
                createRouteToClient();

                resolve(); // Resolve the promise once the asynchronous operation is complete
            })
            .catch(error => {
                console.error('Error checking if the client is in the car:', error);
                resolve(); // Resolve the promise even in case of an error
            });
    });
}

function sendLocationToServer(longitude, latitude) {
    const locationData = {
        longitude: longitude,
        latitude: latitude
    };

    const xhr = new XMLHttpRequest();
    xhr.open('POST', '/Taxi/UpdateLocation', true);
    xhr.setRequestHeader('Content-Type', 'application/json');

    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4) {
            if (xhr.status === 200) {
            } else {
                console.error('Error updating location. Status code:', xhr.status);
            }
        }
    };

    const jsonData = JSON.stringify(locationData);
    xhr.send(jsonData);

}

async function checkMarkers() {
    await checkClaimedClient()
        .then(() => {
            if (claimedClient) {
                isInTheCar();
            }
        })
}

function getCurrentLocation() {
    return new Promise((resolve, reject) => {
        navigator.geolocation.getCurrentPosition(
            position => {
                //userPosition = [position.coords.longitude, position.coords.latitude];
                userPosition = [27.42, 42.56]
                sendLocationToServer(userPosition[0], userPosition[1]);
                resolve(userPosition);
            },
            error => {
                console.error('Error getting user location:', error);
                reject(error);
            },
            {
                enableHighAccuracy: true
            }
        );
    });
}

function createMap(userPosition) {
    map = tt.map({
        key: "MVjOYcUAh8yzcRi8zYnynWAhvqtASz8G",
        container: 'map',
        center: userPosition,
        style: 'https://api.tomtom.com/style/2/custom/style/dG9tdG9tQEBAWk5Zc08wRVFuZGJ5NjhZUjtjNDlhMDc0OS05M2M4LTRjNTQtODQyYS1hZTg0N2UwMTlmZmQ=/drafts/0.json?key=MVjOYcUAh8yzcRi8zYnynWAhvqtASz8G',
        zoom: 13
    });
    let div = document.createElement('div');
    div.innerHTML = '<p>You</p>';

    let popup = new tt.Popup({
        closeButton: false,
        offset: 25,
        anchor: 'bottom'
    }).setDOMContent(div);

    let border = document.createElement('div');
    border.className = 'marker-border driver-marker';

    let icon = document.createElement('div');
    icon.className = 'marker-icon';
    icon.style.backgroundImage = 'url(/images/taxi-icon.png)';
    border.appendChild(icon);

    currentMarker = new tt.Marker({
        element: border
    }).setLngLat(userPosition).setPopup(popup);

    currentMarker.addTo(map);
}

async function initMap() {
    await getCurrentLocation();

    checkMarkers();
    setInterval(function () {

        navigator.geolocation.getCurrentPosition(
            newPosition => {
                const newDriverPosition = [userPosition[0], userPosition[1]];

                currentMarker.setLngLat(userPosition);
                sendLocationToServer(userPosition[0], userPosition[1]);
            },
            error => {
                console.error('Error getting user location:', error);
            },
            {
                enableHighAccuracy: true
            }
        );

        console.log(claimedClientMarker)

        if (!clientIsInTheCar) {
            if (destinationMarker) {
                destinationMarker.remove();
            }

            getNearestDrivers(userPosition);
            getNearestClients(userPosition);

        }

        checkMarkers();

        if (claimedClient === null) {

            deleteRoute();
            if (claimedClient) {
                claimedClientMarker.remove();
                claimedClientMarker = null;
            }

            clientIsInTheCar = false;
        }

    }, 2000); // Repeat every 4 seconds
}

window.onload = function () {
    getCurrentLocation().then(position => {
        userPosition = position;
        createMap(position);
        initMap();
    });
};

window.addEventListener("keydown", function (event) {

    if (event.key == "ArrowRight") {
        userPosition[0] += 0.001;
    }
    else if (event.key == "ArrowLeft") {
        userPosition[0] -= 0.001;
    }
    else if (event.key == "ArrowUp") {
        userPosition[1] += 0.001;
    }
    else if (event.key == "ArrowDown") {
        userPosition[1] -= 0.001;
    }
    else if (event.key == "d") {
        userPosition[0] += 0.0001;
    }
    else if (event.key == "a") {
        userPosition[0] -= 0.0001;
    }
    else if (event.key == "w") {
        userPosition[1] += 0.0001;
    }
    else if (event.key == "s") {
        userPosition[1] -= 0.0001;
    }
})