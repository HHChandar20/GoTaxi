﻿@{
    ViewData["Title"] = "Clients page";
}

< !DOCTYPE html >
    <html>

        <head>

            <link rel="stylesheet" href="https://api.tomtom.com/maps-sdk-for-web/cdn/6.x/6.25.0/maps/maps.css" type="text/css" />

            <script type="text/javascript" src="https://api.tomtom.com/maps-sdk-for-web/cdn/6.x/6.25.0/maps/maps-web.min.js"></script>
            <script type="text/javascript" src="~/js/autocomplete.js"></script>

            <style>

                #client-page {
                    margin: 0;
                display: flex;
                align-items: center;
                justify-content: center;
                flex-direction: column;
                height: 90vh;
                width: 100%;
        }

                #request-form {
                    margin - bottom: 50px;
        }

                #map {
                    width: 100%;
                height: 60%;
        }

                .marker-border {
                    background: orange;
                border-radius: 50%;
                height: 40px;
                width: 40px;
        }

                .marker-icon {
                    background - position: center;
                background-size: 25px 20px;
                position: absolute;
                left: 7.5px;
                top: 10px;
                height: 20px;
                width: 25px;
        }

                .marker-info {
                    display: flex;
                flex-direction: column;
                align-items: center;
                justify-content: center;
        }

                .mapboxgl-popup-content {
                    border - radius: 10px;
        }

                #destination-search {
                    display: flex;
                gap: 5%;
        }

                #destination-button {
                    background - color: yellow;
                width: 15%;
        }

                #destination-form {
                    visibility: hidden;
        }

                #autocomplete-results {
                    background - color: #fff;
                border: 1px solid #ccc;
                max-height: 200px;
                width: 12.2em;
                overflow-y: auto;
                display: none;
                position: absolute;
                z-index: 1000;
        }

                .autocomplete-item {
                    padding: 10px;
                cursor: pointer;
                border: 1px solid #ccc;
        }

                .autocomplete-item:hover {
                    background - color: lightgoldenrodyellow;
        }

                #autocomplete-results::-webkit-scrollbar {
                    width: 10px;
        }

                #autocomplete-results::-webkit-scrollbar-track {
                    background - color: darkgrey;
        }

                #autocomplete-results::-webkit-scrollbar-thumb {
                    background: yellow;
                border-radius: 5em;
        }

            </style>

        </head>

        <body>

            <div id="client-page">

                <div id="request-form">
                    <div class="form-group" id="destination-form">
                        <label for="destination" id="destination-label">Where are you going?</label>

                        <div id="destination-search">
                            <input type="text" autocomplete="off" class="form-control" id="destination" name="destination" placeholder="Search..." onchange="clearSelectedPlace()" />
                            <button type="button" id="destination-button" class="btn btn-secondary" onclick="handleAutocomplete()">P</button>
                        </div>
                        <div id="autocomplete-results"></div>
                    </div>

                    <div class="form-group text-center pt-1 mt-4">
                        <button type="button" id="locationButton" class="btn btn-primary" onclick="toggleLocationSharing()">Request Taxi</button>
                    </div>
                </div>


                <div id="map"></div>

            </div>

            <script type="text/javascript" src="~/js/client.js"></script>

        </body>

    </html>