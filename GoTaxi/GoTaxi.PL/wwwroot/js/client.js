// ClientPage object encapsulates the client-side functionality of the application.
const ClientPage = {

    // Manager for handling client's location-related functionality
    LocationManager: class {

        // Sends an HTTP POST request to update client's location on the server
        static sendRequest(url, data) {
            const xhr = new XMLHttpRequest();
            xhr.open('POST', url, true);
            xhr.setRequestHeader('Content-Type', 'application/json');
            xhr.send(JSON.stringify(data));
        }

        static sendLocationToServer(longitude, latitude) {
            this.sendRequest('/Client/UpdateLocation', { longitude, latitude });
        }

        static updateDestination(destination, longitude, latitude, visibility) {
            this.sendRequest('/Client/UpdateDestination', { destination, longitude, latitude, visibility });
        }

        // Retrieves the client's current location and updates the marker
        static shareLocation() {
            navigator.geolocation.getCurrentPosition(
                newPosition => {
                    const newClientPosition = [newPosition.coords.longitude, newPosition.coords.latitude];
                    ClientPage.ClientApp.marker.setLngLat(newClientPosition);
                    this.sendLocationToServer(newClientPosition[0], newClientPosition[1]); // ...newClientPosition
                },
                error => {
                    console.error('Error getting client location:', error);
                },
                {
                    enableHighAccuracy: true
                }
            );
        }

    },

    // Manager for handling map markers, destinations, and driver information
    MarkerManager: class {

        static removeDestinationMarker() {
            let marker = ClientPage.ClientApp.destinationMarker;
            if (marker) {
                marker.remove();
            }
            return null;
        }

        static addDestinationMarker() {
            const selectedPlace = ClientPage.ClientApp.selectedPlace;

            const destinationPosition = [selectedPlace.lon, selectedPlace.lat];

            const markerDiv = document.createElement('div');
            markerDiv.innerHTML = `<p class="destinationName">${selectedPlace.display_name}</p>`;

            const markerPopup = new tt.Popup({
                closeButton: false,
                offset: 25,
                anchor: 'bottom',
            }).setDOMContent(markerDiv);

            const markerBorder = document.createElement('div');
            markerBorder.className = ' destination-marker marker-border';

            const markerIcon = document.createElement('div');
            markerIcon.className = 'marker-icon';
            markerIcon.style.backgroundImage = 'url(/images/destination.png)';
            markerBorder.appendChild(markerIcon);

            ClientPage.ClientApp.destinationMarker = new tt.Marker({
                element: markerBorder,
            }).setLngLat(destinationPosition).setPopup(markerPopup);

            ClientPage.ClientApp.destinationMarker.addTo(ClientPage.ClientApp.map);
        }

        static updateDriverMarker(driver) {
            const driverPosition = [driver.user.location.longitude, driver.user.location.latitude];

            if (ClientPage.ClientApp.driverMarker && ClientPage.ClientApp.driverMarker.getElement() && ClientPage.ClientApp.driverMarker.getPopup()) {

                ClientPage.ClientApp.driverMarker.setLngLat(driverPosition);

                const driverMarkerDiv = ClientPage.ClientApp.driverMarker.getPopup().getElement();

                if (driverMarkerDiv) {
                    const plateNumber = driverMarkerDiv.querySelector('p.plate');
                    const name = driverMarkerDiv.querySelector('p.name');

                    if (plateNumber) plateNumber.innerText = driver.plateNumber;
                    if (name) name.innerText = driver.user.fullName;
                }
            }
            else {
                const driverMarkerDiv = document.createElement('div');
                driverMarkerDiv.innerHTML = `
                    <p class="plate">${driver.plateNumber}</p>
                    <p class="name">${driver.user.fullName}</p>
                `;

                const driverMarkerPopup = new tt.Popup({
                    closeButton: false,
                    offset: 25,
                    anchor: 'bottom',
                }).setDOMContent(driverMarkerDiv);

                const driverMarkerBorder = document.createElement('div');
                driverMarkerBorder.className = 'marker-border driver-marker';

                const driverMarkerIcon = document.createElement('div');
                driverMarkerIcon.className = 'marker-icon';
                driverMarkerIcon.style.backgroundImage = 'url(/images/taxi-icon.png)';
                driverMarkerBorder.appendChild(driverMarkerIcon);

                ClientPage.ClientApp.driverMarker = new tt.Marker({
                    element: driverMarkerBorder,
                }).setLngLat(driverPosition).setPopup(driverMarkerPopup);

                ClientPage.ClientApp.driverMarker.addTo(ClientPage.ClientApp.map);

            }
        }

    },


    // Manager for handling client-specific actions and data fetching
    ClientManager: class {
        static clientClaimedBy() {
            return fetch(`/Client/ClientClaimedBy`)
                .then(response => response.json())
                .catch(error => {
                    console.error('Error fetching client:', error);
                });
        }

        static startSharingLocation() {
            ClientPage.ClientApp.button.innerHTML = 'Cancel Request';
            ClientPage.ClientApp.interval = setInterval(function () {
                ClientPage.ClientManager.clientClaimedBy().then(driver => {
                    if (driver) {
                        ClientPage.MarkerManager.updateDriverMarker(driver, ClientPage.ClientApp.driverMarker);
                    }
                    ClientPage.LocationManager.shareLocation(ClientPage.ClientApp.marker);
                });
            }, 6000); // Repeat every 6 seconds
        }

        static stopSharingLocation() {
            ClientPage.ClientApp.destinationForm.style.visibility = "visible";

            ClientPage.MapManager.flyToUser();

            ClientPage.ClientApp.sharingLocation = false;
            ClientPage.ClientApp.button.innerHTML = 'Request Taxi';
            clearInterval(ClientPage.ClientApp.interval);
            ClientPage.LocationManager.updateDestination("", 90, 90, false);

            if (ClientPage.ClientApp.driverMarker) ClientPage.ClientApp.driverMarker.remove();
        }

        static toggleLocationSharing() {

            let longitude = 90;
            let latitude = 90;

            const selectedPlace = ClientPage.ClientApp.selectedPlace;

            ClientPage.ClientApp.destinationMarker = ClientPage.MarkerManager.removeDestinationMarker(ClientPage.ClientApp.destinationMarker);

            if (!ClientPage.ClientApp.sharingLocation) {
                ClientPage.ClientApp.sharingLocation = true;
                this.startSharingLocation();
                ClientPage.ClientApp.destinationForm.style.visibility = "hidden";

                if (selectedPlace) {
                    longitude = selectedPlace.lon;
                    latitude = selectedPlace.lat;
                    ClientPage.MarkerManager.addDestinationMarker();
                    ClientPage.ClientApp.map.flyTo({ center: ClientPage.ClientApp.destinationMarker.getLngLat(), zoom: 13 });
                }

                ClientPage.LocationManager.updateDestination(ClientPage.ClientApp.input.value, longitude, latitude, true);
            }
            else {
                this.stopSharingLocation();
            }

        }

        static isClientVisible() {
            fetch(`/Client/IsClientVisible`)
                .then(response => response.json())
                .then(visibility => {

                    ClientPage.ClientApp.sharingLocation = visibility;

                    if (ClientPage.ClientApp.sharingLocation) {
                        this.startSharingLocation();
                    } else {
                        ClientPage.ClientApp.destinationForm.style.visibility = 'visible';
                    }
                })
                .catch(error => {
                    console.error('Error fetching client:', error);
                });
        }

        static getClientDestination() {
            fetch(`/Client/GetClientDestination`)
                .then(response => response.json())
                .then(destination => {

                    if (destination && destination.location.longitude !== 90) {
                        ClientPage.ClientApp.input.value = destination.name;
                        ClientPage.ClientApp.selectedPlace = {
                            display_name: destination.name,
                            lon: destination.location.longitude,
                            lat: destination.location.latitude,
                        };
                    }
                })
                .catch(error => {
                    console.error('Error fetching client:', error);
                });
        }
    },


    // Manager for handling map-related actions and initialization
    MapManager: class {

        // Rotates the camera for a dynamic map experience
        static rotateCamera(timestamp) {
            if (ClientPage.ClientApp.sharingLocation) {
                if (ClientPage.ClientApp.selectedPlace) {
                    ClientPage.MarkerManager.addDestinationMarker();
                    ClientPage.ClientApp.map.flyTo({ center: ClientPage.ClientApp.destinationMarker.getLngLat(), zoom: 13 });
                }

                return;
            }

            let rotationDegree = timestamp / 100 % 360;
            ClientPage.ClientApp.map.rotateTo(rotationDegree, { duration: 0 });
            requestAnimationFrame((timestamp) => this.rotateCamera(timestamp));
        }

        // Moves the map view to the client's location
        static flyToUser() {
            ClientPage.ClientApp.map.flyTo({ center: ClientPage.ClientApp.marker.getLngLat(), zoom: 13 });
        }

        // Initializes the map with user's location
        static initMap() {
            navigator.geolocation.getCurrentPosition(
                position => {
                    const userPosition = [position.coords.longitude, position.coords.latitude];
                    ClientPage.LocationManager.sendLocationToServer(userPosition[0], userPosition[1]);

                    ClientPage.ClientApp.map = tt.map({
                        key: 'MVjOYcUAh8yzcRi8zYnynWAhvqtASz8G',
                        container: 'map',
                        center: userPosition,
                        style: 'https://api.tomtom.com/style/1/style/22.2.1-*?map=2/basic_street-dark&poi=2/poi_dark',
                        zoom: 13,
                        pitch: 60,
                    });

                    let div = document.createElement('div');
                    div.innerHTML = '<p>You</p>';

                    let popup = new tt.Popup({
                        closeButton: false,
                        offset: 25,
                        anchor: 'bottom',
                    }).setDOMContent(div);

                    let border = document.createElement('div');
                    border.className = 'marker-border client-marker';

                    let icon = document.createElement('div');
                    icon.className = 'marker-icon';
                    icon.style.backgroundImage = 'url(/images/client-icon.png)';
                    border.appendChild(icon);

                    ClientPage.ClientApp.marker = new tt.Marker({
                        element: border,
                    }).setLngLat(userPosition).setPopup(popup);

                    ClientPage.ClientApp.marker.addTo(ClientPage.ClientApp.map);

                    ClientPage.ClientApp.map.on('load', () => {
                        requestAnimationFrame((timestamp) => ClientPage.MapManager.rotateCamera(timestamp));
                    });
                },
                error => {
                    console.error('Error getting user location:', error);
                },
                {
                    enableHighAccuracy: true,
                }
            );

            ClientPage.ClientManager.isClientVisible();
            ClientPage.ClientManager.getClientDestination();
        }
    },

    // Main application class with static properties
    ClientApp: class {
        static button = document.getElementById("locationButton");
        static input = document.getElementById('destination');
        static destinationForm = document.getElementById('destination-form');

        static interval;
        static sharingLocation;

        static map;
        static marker;
        static driverMarker;
        static destinationMarker = null;

        static selectedPlace;

        static updateSelectedPlace(newPlace) {
            this.selectedPlace = newPlace;
        }

    }
};

// Initialize the map when the window is loaded
window.onload = ClientPage.MapManager.initMap;

// Close autocomplete results if user clicks outside the input and results
window.addEventListener("click", (event) => {
    if (!event.target.matches("#destination") && !event.target.matches(".autocomplete-item")) {
        document.getElementById("autocomplete-results").style.display = "none";
    }
});