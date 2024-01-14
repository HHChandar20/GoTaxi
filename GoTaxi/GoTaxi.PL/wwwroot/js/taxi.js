﻿let map;

let driverMarkers = [];
let clientMarkers = [];

let claimedClient;
let claimedClientMarker;
let currentMarker;

function addDriverMarker(driver) {
    const driverPosition = [driver.longitude, driver.latitude];

    let markerDiv = document.createElement('div');
    markerDiv.innerHTML =
        `
        <p class="driverName">${driver.fullName}</p>
        <p class="plateNumber">${driver.plateNumber}</p>
    `;


    let markerPopup = new tt.Popup({
        closeButton: false,
        offset: 25,
        anchor: 'bottom'
    }).setDOMContent(markerDiv);

    let markerBorder = document.createElement('div');
    markerBorder.className = 'marker-border';
    markerBorder.style.background = 'green';

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
    const clientPosition = [client.longitude, client.latitude];

    let markerDiv = document.createElement('div');
    markerDiv.innerHTML =
        `   <h6 class="destination">${client.destination}</h3>
        <p class="name>${client.fullName}</p>
        <p class="email>${client.email}</p>
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
    markerBorder.className = 'marker-border';
    markerBorder.style.background = 'red';

    let markerIcon = document.createElement('div');
    markerIcon.className = 'marker-icon';
    markerIcon.style.backgroundImage = 'url(/images/client-icon.png)';
    markerBorder.appendChild(markerIcon);

    const newMarker = new tt.Marker({
        element: markerBorder
    }).setLngLat(clientPosition).setPopup(markerPopup);

    newMarker.addTo(map);
    newMarker.clientPhoneNumber = client.phoneNumber; // Store client phone number in the marker
    newMarker.destination = client.destination;

    clientMarkers.push(newMarker);
}

function updateDriverMarker(marker, driver) {
    const driverPosition = [driver.longitude, driver.latitude];

    if (marker && marker.getElement() && marker.getPopup()) {
        marker.setLngLat(driverPosition);

        const markerDiv = marker.getPopup().getElement();

        if (markerDiv) {
            const name = markerDiv.querySelector('p.driverName');
            const plateNumber = markerDiv.querySelector('p.plateNumber');

            if (name) name.innerText = driver.fullName;
            if (plateNumber) plateNumber.innerText = driver.plateNumber;
        }
    } else {
        console.error('Invalid marker or missing elements:', marker);
    }
}

function updateClientMarker(marker, client) {
    const clientPosition = [client.longitude, client.latitude];

    // Check if the marker is defined and has an element and popup
    if (marker && marker.getElement() && marker.getPopup()) {
        marker.setLngLat(clientPosition);

        const markerDiv = marker.getPopup().getElement();
        // Check if the required elements inside the popup exist
        if (markerDiv) {
            const destination = markerDiv.querySelector('h3');
            const name = markerDiv.querySelector('p.name');
            const email = markerDiv.querySelector('p.email');
            const phoneNumber = markerDiv.querySelector('p.phone');
            const reports = markerDiv.querySelector('p.reports');
            const button = markerDiv.querySelector('button');

            // Update the elements if they exist
            if (destination) destination.innerText = client.destination;
            if (name) name.innerText = client.fullName;
            if (email) email.innerText = client.email;
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
            if (Array.isArray(nearestDrivers)) {
                console.log("drivers: ", nearestDrivers);
                updateDriverMarkers(nearestDrivers);
            } else {
                console.error('Invalid response format:', nearestDrivers);
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
            if (Array.isArray(nearestClients)) {
                updateClientMarkers(nearestClients);
            }
            else {
                console.error('Invalid response format:', nearestClients);
            }
        })
        .catch(error => {
            console.error('Error fetching nearest clients:', error);
        });
}

function clearMarkers(phoneNumber) {
    driverMarkers.forEach(marker => {
        marker.remove();
        driverMarkers.shift();
    });

    clientMarkers = clientMarkers.filter(marker => {
        if (marker.clientPhoneNumber !== phoneNumber) {
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

    deleteRoute();

    // Add the new 'route' source
    map.addSource('route', {
        'type': 'geojson',
        'data': geoJSON
    });

    // Add the new 'route' layer
    map.addLayer({
        'id': 'route',
        'type': 'line',
        'source': 'route',  // Use the 'route' source
        'paint': {
            'line-color': 'red',
            'line-width': 5
        }
    });
}

function claimClient(phoneNumber) {
    const xhr = new XMLHttpRequest();
    xhr.open('POST', '/Taxi/ClaimClient', true);
    xhr.setRequestHeader('Content-Type', 'application/json');

    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4) {
            if (xhr.status === 200) {
                console.log('Location update successful:', xhr.responseText);
            } else {
                console.error('Error updating location. Status code:', xhr.status);
            }
        }
    };

    const jsonData = JSON.stringify(phoneNumber);
    xhr.send(jsonData);

    // Store the claimed client marker
    claimedClientMarker = clientMarkers.find(marker => marker.clientPhoneNumber === phoneNumber);

    createRouteToClient();
    clearMarkers(phoneNumber);
    isActive = true;
}

function createRouteToClient() {
    console.log("claimed", claimedClient);

    if (claimedClientMarker && currentMarker) {
        let routeOptions = {
            key: "MVjOYcUAh8yzcRi8zYnynWAhvqtASz8G",
            locations: [
                { lat: currentMarker.getLngLat().lat, lon: currentMarker.getLngLat().lng },
                { lat: claimedClientMarker.getLngLat().lat, lon: claimedClientMarker.getLngLat().lng }
            ]
        };

        tt.services
            .calculateRoute(routeOptions)
            .then(response => {
                // Add the new route layer
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
    fetch(`/Taxi/CheckClaimedClient`)
        .then(response => response.json())
        .then(client => {

            console.log("client:", client);
            claimedClient = client;

            if (claimedClient != "0") {
                addClientMarker(claimedClient);
                claimClient(claimedClient.phoneNumber);
            }
        })
        .catch(error => {
            console.error('Error fetching claimed client:', error);
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
                console.log('Location update successful:', xhr.responseText);
            } else {
                console.error('Error updating location. Status code:', xhr.status);
            }
        }
    };

    const jsonData = JSON.stringify(locationData);
    xhr.send(jsonData);
}

function isInTheCar() {
    fetch(`/Taxi/IsInTheCar`)
        .then(response => response.json())
        .then(result => {
            console.log("Result: ", result);
            return result;
        })
        .catch(error => {
            console.error('Error checking if the client is in the car:', error);
        });
}

function initMap() {
    navigator.geolocation.getCurrentPosition(
        position => {
            userPosition = [27.4021016, 42.4574976]//[position.coords.longitude, position.coords.latitude];
            sendLocationToServer(userPosition[0], userPosition[1]);

            map = tt.map({
                key: "MVjOYcUAh8yzcRi8zYnynWAhvqtASz8G",
                container: 'map',
                center: userPosition,
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
            border.className = 'marker-border';

            let icon = document.createElement('div');
            icon.className = 'marker-icon';
            icon.style.backgroundImage = 'url(/images/taxi-icon.png)';
            border.appendChild(icon);

            currentMarker = new tt.Marker({
                element: border
            }).setLngLat(userPosition).setPopup(popup);

            currentMarker.addTo(map);

            checkClaimedClient();

            getNearestDrivers(userPosition);
            getNearestClients(userPosition);

            setInterval(function () {
                const currentPosition = [userPosition[0] += 0.0001, userPosition[1]];

                navigator.geolocation.getCurrentPosition(
                    newPosition => {
                        const newDriverPosition = [userPosition[0] += 0.0001, userPosition[1]];

                        checkClaimedClient();

                        console.log(driverMarkers);
                        console.log("hey", claimedClient);

                        if (claimedClient != "0") {
                            createRouteToClient();
                        }

                        deleteRoute();

                        isInTheCar();

                        currentMarker.setLngLat(currentPosition);
                        sendLocationToServer(currentPosition[0], currentPosition[1]);
                    },
                    error => {
                        console.error('Error getting user location:', error);
                    },
                    {
                        enableHighAccuracy: true
                    }
                );

                if (claimedClient == "0") {
                    getNearestDrivers(userPosition);
                    getNearestClients(userPosition);
                }

            }, 6000); // Repeat every 6 seconds
        },
        error => {
            console.error('Error getting user location:', error);
        },
        {
            enableHighAccuracy: true
        }
    );
}

window.onload = initMap;