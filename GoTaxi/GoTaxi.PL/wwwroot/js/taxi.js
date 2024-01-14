const TaxiPage = {
    LocationManager: class {
        static sendRequest(url, data) {
            const xhr = new XMLHttpRequest();
            xhr.open('POST', url, true);
            xhr.setRequestHeader('Content-Type', 'application/json');
            xhr.send(JSON.stringify(data));
        }

        static sendLocationToServer(longitude, latitude) {
            TaxiPage.LocationManager.sendRequest('/Taxi/UpdateLocation', { longitude, latitude })
        }

        static getCurrentLocation() {
            return new Promise((resolve, reject) => {
                navigator.geolocation.getCurrentPosition(
                    position => {
                        //userPosition = [position.coords.longitude, position.coords.latitude];
                        TaxiPage.TaxiApp.userPosition = [27.42, 42.56]
                        TaxiPage.LocationManager.sendLocationToServer(TaxiPage.TaxiApp.userPosition[0], TaxiPage.TaxiApp.userPosition[1]);
                        resolve(TaxiPage.TaxiApp.userPosition);
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
    },

    MarkerManager: class {
        static addDriverMarker(driver) {
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

            newMarker.addTo(TaxiPage.TaxiApp.map);
            newMarker.driverPlateNumber = driver.plateNumber; // Store driver plate number in the marker

            TaxiPage.TaxiApp.driverMarkers.push(newMarker);
        }

        static addClientMarker(client) {
            const clientPosition = [client.user.location.longitude, client.user.location.latitude];

            let markerDiv = document.createElement('div');
            markerDiv.innerHTML =
                `   <p class="destination">${client.destination.name}</p>
            <p class="name>${client.user.fullName}</p>
            <p class="email>${client.user.email}</p>
            <p class="phone">${client.phoneNumber}</h6>
            <p class="reports">Reports: ${client.reports}</p>
            <button id="claim-button" onclick="TaxiPage.ClientManager.claimClient('${client.phoneNumber}')">Claim</button>
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

            newMarker.addTo(TaxiPage.TaxiApp.map);
            newMarker.clientPhoneNumber = client.phoneNumber; // Store client phone number in the marker
            newMarker.destination = client.destination.name;

            TaxiPage.TaxiApp.clientMarkers.push(newMarker);
        }

        static updateDriverMarker(marker, driver) {
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

        static updateClientMarker(marker, client) {
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
                    if (button) button.onclick = function () { TaxiPage.ClientManager.claimClient(client.phoneNumber); };

                }
            } else {
                console.error('Invalid marker or missing elements:', marker);
            }
        }

        static updateDriverMarkers(newDrivers) {
            const existingDriverPlateNumbers = TaxiPage.TaxiApp.driverMarkers.map(marker => marker.driverPlateNumber);
            const newDriverPlateNumbers = newDrivers.map(driver => driver.plateNumber);

            // Remove markers for drivers that are not in the new list
            const driversToRemove = TaxiPage.TaxiApp.driverMarkers.filter(marker => !newDriverPlateNumbers.includes(marker.driverPlateNumber));
            driversToRemove.forEach(marker => {
                marker.remove();
                TaxiPage.TaxiApp.driverMarkers.splice(TaxiPage.TaxiApp.driverMarkers.indexOf(marker));
            });

            // Update or add markers for the new drivers
            newDrivers.forEach(driver => {
                const index = existingDriverPlateNumbers.indexOf(driver.plateNumber);
                if (index !== -1) {
                    // Update existing marker
                    TaxiPage.MarkerManager.updateDriverMarker(TaxiPage.TaxiApp.driverMarkers[index], driver);
                } else {
                    // Add new marker
                    TaxiPage.MarkerManager.addDriverMarker(driver);
                }
            });
        }

        static updateClientMarkers(newClients) {
            const existingClientPhoneNumbers = TaxiPage.TaxiApp.clientMarkers.map(marker => marker.clientPhoneNumber);
            const newClientPhoneNumbers = newClients.map(client => client.phoneNumber);
            console.log(newClientPhoneNumbers)

            console.log(newClients);

            // Remove markers for clients that are not in the new list
            const clientsToRemove = TaxiPage.TaxiApp.clientMarkers.filter(marker => !newClientPhoneNumbers.includes(marker.clientPhoneNumber));
            clientsToRemove.forEach(marker => {
                marker.remove();
                TaxiPage.TaxiApp.clientMarkers.splice(TaxiPage.TaxiApp.clientMarkers.indexOf(marker));

            });

            // Update or add markers for the new clients
            newClients.forEach(client => {
                const index = existingClientPhoneNumbers.indexOf(client.phoneNumber);

                if (index !== -1) {
                    // Update existing marker
                    TaxiPage.MarkerManager.updateClientMarker(TaxiPage.TaxiApp.clientMarkers[index], client);
                } else {
                    // Add new marker
                    TaxiPage.MarkerManager.addClientMarker(client);
                }
            });
        }

        static clearMarkers(phoneNumber = 0) {
            TaxiPage.TaxiApp.driverMarkers.forEach(marker => {
                marker.remove();
            });
            TaxiPage.TaxiApp.driverMarkers = [];

            TaxiPage.TaxiApp.clientMarkers = TaxiPage.TaxiApp.clientMarkers.filter(marker => {
                if ((marker.clientPhoneNumber !== phoneNumber) || TaxiPage.TaxiApp.clientIsInTheCar === true) {
                    marker.remove();
                    return false;  // Exclude this marker from the new array
                }

                return true;  // Include this marker in the new array
            });
        }

        static addDestinationMarker() {

            TaxiPage.MarkerManager.clearMarkers();

            let destinationPosition = [TaxiPage.TaxiApp.claimedClient.destination.location.longitude, TaxiPage.TaxiApp.claimedClient.destination.location.latitude];

            let markerDiv = document.createElement('div');
            markerDiv.innerHTML =
                `
            <p class="destinationName">${TaxiPage.TaxiApp.claimedClient.destination.name}</p>
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

            TaxiPage.TaxiApp.destinationMarker = new tt.Marker({
                element: markerBorder
            }).setLngLat(destinationPosition).setPopup(markerPopup);

            TaxiPage.TaxiApp.destinationMarker.addTo(TaxiPage.TaxiApp.map);
        }
    },

    RouteManager: class {
        static deleteRoute() {
            // Check if the 'route' layer already exists and remove it
            if (TaxiPage.TaxiApp.map.getLayer('route')) {
                TaxiPage.TaxiApp.map.removeLayer('route');
            }

            // Check if the 'route' source already exists and remove it
            if (TaxiPage.TaxiApp.map.getSource('route')) {
                TaxiPage.TaxiApp.map.removeSource('route');
            }
        }

        static displayRoute(geoJSON) {

            if (!TaxiPage.TaxiApp.map.getLayer('route')) {

                TaxiPage.TaxiApp.map.addSource('route', {
                    'type': 'geojson',
                    'data': geoJSON
                });

                TaxiPage.TaxiApp.map.addLayer({
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
                TaxiPage.TaxiApp.map.getSource('route').setData(geoJSON);
            }
        }

        static createRouteToClient() {

            if (TaxiPage.TaxiApp.clientIsInTheCar) {
                if (!TaxiPage.TaxiApp.destinationMarker) TaxiPage.MarkerManager.addDestinationMarker();
            }
            else {
                if (!TaxiPage.TaxiApp.claimedClientMarker) {
                    TaxiPage.MarkerManager.addClientMarker(TaxiPage.TaxiApp.claimedClient);
                    TaxiPage.TaxiApp.claimedClientMarker = TaxiPage.TaxiApp.clientMarkers.find(marker => marker.clientPhoneNumber === TaxiPage.TaxiApp.claimedClient.phoneNumber);
                }
                else {
                    TaxiPage.MarkerManager.updateClientMarker(TaxiPage.TaxiApp.claimedClientMarker, TaxiPage.TaxiApp.claimedClient);
                }
            }

            if ((TaxiPage.TaxiApp.claimedClientMarker || TaxiPage.TaxiApp.destinationMarker) && TaxiPage.TaxiApp.currentMarker) {

                let longitude = 90;
                let latitude = 90;

                if (TaxiPage.TaxiApp.clientIsInTheCar === true) {
                    longitude = TaxiPage.TaxiApp.destinationMarker.getLngLat().lng;
                    latitude = TaxiPage.TaxiApp.destinationMarker.getLngLat().lat;
                }
                else {
                    longitude = TaxiPage.TaxiApp.claimedClientMarker.getLngLat().lng;
                    latitude = TaxiPage.TaxiApp.claimedClientMarker.getLngLat().lat;
                }

                let routeOptions = {
                    key: "MVjOYcUAh8yzcRi8zYnynWAhvqtASz8G",
                    locations: [
                        { lat: TaxiPage.TaxiApp.currentMarker.getLngLat().lat.toFixed(6), lon: TaxiPage.TaxiApp.currentMarker.getLngLat().lng.toFixed(6) },
                        { lat: latitude.toFixed(6), lon: longitude.toFixed(6) }
                    ]
                };

                console.log(routeOptions);

                tt.services
                    .calculateRoute(routeOptions)
                    .then(response => {

                        TaxiPage.RouteManager.displayRoute(response.toGeoJson(), TaxiPage.TaxiApp.map);
                    })
                    .catch(error => {
                        console.error('Error calculating route:', error);
                    });

            } else {
                console.error('Claimed client marker or current driver marker not found.');
            }
        }

    },

    ClientManager: class {
        static getNearestClients(currentPosition) {
            fetch(`/Taxi/GetNearestClients?currentClientLongitude=${currentPosition[0]}&currentClientLatitude=${currentPosition[1]}`)
                .then(response => response.json())
                .then(nearestClients => {
                    if (nearestClients) {

                        TaxiPage.MarkerManager.updateClientMarkers(nearestClients);
                    }
                })
                .catch(error => {
                    console.error('Error fetching nearest clients:', error);
                });
        }

        static claimClient(phoneNumber) {
            TaxiPage.LocationManager.sendRequest('/Taxi/ClaimClient', phoneNumber);

            TaxiPage.TaxiApp.claimedClientMarker = TaxiPage.TaxiApp.clientMarkers.find(marker => marker.clientPhoneNumber === phoneNumber);

            TaxiPage.ClientManager.checkClientStatus();

            TaxiPage.MarkerManager.clearMarkers(phoneNumber);
        }

        static checkClaimedClient() {
            return new Promise((resolve) => {
                fetch(`/Taxi/CheckClaimedClient`)
                    .then(response => response.json())
                    .then(client => {

                        if (client === null && TaxiPage.TaxiApp.claimedClient && TaxiPage.TaxiApp.claimedClientMarker) {
                            TaxiPage.TaxiApp.claimedClientMarker.remove();
                            TaxiPage.TaxiApp.claimedClientMarker = null;
                            TaxiPage.MarkerManager.clearMarkers();
                            TaxiPage.RouteManager.deleteRoute(TaxiPage.TaxiApp.map);
                            TaxiPage.TaxiApp.clientIsInTheCar = false;
                        }

                        TaxiPage.TaxiApp.claimedClient = client;
                        resolve(); // Resolve the promise once the asynchronous operation is complete
                    })
                    .catch(error => {
                        console.error('Error fetching claimed client:', error);
                        resolve(); // Resolve the promise even in case of an error
                    });
            });
        }

        static isInTheCar() {
            return new Promise((resolve) => {
                fetch(`/Taxi/IsInTheCar?phoneNumber=${TaxiPage.TaxiApp.claimedClient.phoneNumber}`)
                    .then(response => response.json())
                    .then(result => {
                        TaxiPage.TaxiApp.clientIsInTheCar = JSON.parse(result) && TaxiPage.TaxiApp.claimedClient.destination.location.longitude != 90 && TaxiPage.TaxiApp.claimedClient.destination.location.latitude != 90;
                        console.log("In the car:", TaxiPage.TaxiApp.clientIsInTheCar);

                        if (TaxiPage.TaxiApp.clientIsInTheCar === true && TaxiPage.TaxiApp.claimedClientMarker !== null) {
                            TaxiPage.TaxiApp.claimedClientMarker.remove();
                            TaxiPage.TaxiApp.claimedClientMarker = null;
                        }

                        TaxiPage.RouteManager.createRouteToClient();

                        resolve(); // Resolve the promise once the asynchronous operation is complete
                    })
                    .catch(error => {
                        console.error('Error checking if the client is in the car:', error);
                        resolve(); // Resolve the promise even in case of an error
                    });
            });
        }

        static async checkClientStatus() {
            await TaxiPage.ClientManager.checkClaimedClient()
                .then(() => {
                    if (TaxiPage.TaxiApp.claimedClient) {
                        TaxiPage.ClientManager.isInTheCar();
                    }
                })
        }
    },

    DriverManager: class {
        static getNearestDrivers(currentPosition) {
            fetch(`/Taxi/GetNearestDrivers?currentDriverLongitude=${currentPosition[0]}&currentDriverLatitude=${currentPosition[1]}`)
                .then(response => response.json())
                .then(nearestDrivers => {
                    if (nearestDrivers.length > 0) {
                        TaxiPage.MarkerManager.updateDriverMarkers(nearestDrivers);
                    }
                })
                .catch(error => {
                    console.error('Error fetching nearest drivers:', error);
                });
        }
    },

    MapManager: class {
        static createMap(userPosition) {
            TaxiPage.TaxiApp.map = tt.map({
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

            TaxiPage.TaxiApp.currentMarker = new tt.Marker({
                element: border
            }).setLngLat(userPosition).setPopup(popup);

            TaxiPage.TaxiApp.currentMarker.addTo(TaxiPage.TaxiApp.map);
        }

        static async initMap() {
            await TaxiPage.LocationManager.getCurrentLocation();

            TaxiPage.ClientManager.checkClientStatus();
            setInterval(function () {

                navigator.geolocation.getCurrentPosition(
                    newPosition => {
                        //const newDriverPosition = [newPosition.coords.longitude, newPosition.coords.latitude];

                        TaxiPage.TaxiApp.currentMarker.setLngLat(TaxiPage.TaxiApp.userPosition);
                        TaxiPage.LocationManager.sendLocationToServer(TaxiPage.TaxiApp.userPosition[0], TaxiPage.TaxiApp.userPosition[1]);
                    },
                    error => {
                        console.error('Error getting user location:', error);
                    },
                    {
                        enableHighAccuracy: true
                    }
                );

                console.log(TaxiPage.TaxiApp.claimedClientMarker)

                if (!TaxiPage.TaxiApp.clientIsInTheCar) {
                    if (TaxiPage.TaxiApp.destinationMarker) {
                        TaxiPage.TaxiApp.destinationMarker.remove();
                    }

                    TaxiPage.DriverManager.getNearestDrivers(TaxiPage.TaxiApp.userPosition);
                    TaxiPage.ClientManager.getNearestClients(TaxiPage.TaxiApp.userPosition);

                }

                TaxiPage.ClientManager.checkClientStatus();

                if (TaxiPage.TaxiApp.claimedClient === null) {

                    TaxiPage.RouteManager.deleteRoute();
                    if (TaxiPage.TaxiApp.claimedClient) {
                        TaxiPage.TaxiApp.claimedClientMarker.remove();
                        TaxiPage.TaxiApp.claimedClientMarker = null;
                    }

                    TaxiPage.TaxiApp.clientIsInTheCar = false;
                }

            }, 5000); // Repeat every 4 seconds
        }
    },

    TaxiApp: class {
        constructor() {
            TaxiPage.TaxiApp.map = null;
            TaxiPage.TaxiApp.currentMarker = null;
            TaxiPage.TaxiApp.userPosition = null;
            TaxiPage.TaxiApp.driverMarkers = [];
            TaxiPage.TaxiApp.clientMarkers = [];
            TaxiPage.TaxiApp.clientIsInTheCar = false;
            TaxiPage.TaxiApp.claimedClient = null;
            TaxiPage.TaxiApp.claimedClientMarker = null;
            TaxiPage.TaxiApp.destinationMarker = null;


            TaxiPage.LocationManager.getCurrentLocation().then(position => {
                TaxiPage.TaxiApp.userPosition = position;
                TaxiPage.MapManager.createMap(position);
                TaxiPage.MapManager.initMap();
            });
        }
    },
}

window.onload = function () {
    new TaxiPage.TaxiApp();
};

window.addEventListener("keydown", function (event) {

    if (event.key == "ArrowRight") {
        TaxiPage.TaxiApp.userPosition[0] += 0.001;
    }
    else if (event.key == "ArrowLeft") {
        TaxiPage.TaxiApp.userPosition[0] -= 0.001;
    }
    else if (event.key == "ArrowUp") {
        TaxiPage.TaxiApp.userPosition[1] += 0.001;
    }
    else if (event.key == "ArrowDown") {
        TaxiPage.TaxiApp.userPosition[1] -= 0.001;
    }
    else if (event.key == "d") {
        TaxiPage.TaxiApp.userPosition[0] += 0.0001;
    }
    else if (event.key == "a") {
        TaxiPage.TaxiApp.userPosition[0] -= 0.0001;
    }
    else if (event.key == "w") {
        TaxiPage.TaxiApp.userPosition[1] += 0.0001;
    }
    else if (event.key == "s") {
        TaxiPage.TaxiApp.userPosition[1] -= 0.0001;
    }
})