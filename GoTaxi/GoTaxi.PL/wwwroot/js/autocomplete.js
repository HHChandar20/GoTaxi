class AutocompleteManager {
    static handleAutocomplete() {
        const destination = document.getElementById("destination");
        const autocompleteResults = document.getElementById("autocomplete-results");

        fetch(`https://nominatim.openstreetmap.org/search?q=${destination.value}&format=json&limit=5&countrycodes=BG`)
            .then(response => response.json())
            .then(data => {
                AutocompleteManager.displayAutocompleteResults(data, destination, autocompleteResults);
            })
            .catch(error => {
                console.error('Error fetching data:', error);
            });
    }

    static clearSelectedPlace() {
        console.log("cleared");
        ClientPage.ClientApp.selectedPlace = null;
    }

    static displayAutocompleteResults(results, destination, autocompleteResults) {
        autocompleteResults.innerHTML = "";

        results.forEach(place => {
            let autocompleteItem = document.createElement("div");

            autocompleteItem.className = "autocomplete-item";
            autocompleteItem.textContent = place.display_name;

            autocompleteItem.addEventListener("click", () => {
                destination.value = place.display_name;
                ClientPage.ClientApp.selectedPlace = place;
                autocompleteResults.style.display = "none";
            });

            autocompleteResults.appendChild(autocompleteItem);
        });

        if (results.length > 0) {
            autocompleteResults.style.display = "block";
        } else {
            autocompleteResults.style.display = "none";
        }
    }
}