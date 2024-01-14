class FormManager {

    // Static properties to reference DOM elements
    static clientForm = document.getElementById('client-form');
    static taxiForm = document.getElementById('taxi-form');
    static clientButton = document.querySelector('.registration-option:nth-child(1)');
    static taxiButton = document.querySelector('.registration-option:nth-child(2)');

    // Method to select a registration option (client or taxi)
    static selectOption(option) {

        // Display client form and hide taxi form for 'client' option
        if (option === 'client') {
            this.clientForm.classList.remove('hidden');
            this.taxiForm.classList.add('hidden');
            this.clientButton.classList.add('selected');
            this.taxiButton.classList.remove('selected');
        }
        // Display taxi form and hide client form for 'taxi' option
        else if (option === 'taxi') {
            this.taxiForm.classList.remove('hidden');
            this.clientForm.classList.add('hidden');
            this.clientButton.classList.remove('selected');
            this.taxiButton.classList.add('selected');
        }
    }

    // Method to clear form inputs of both client and taxi forms
    static clearFormInputs() {

        this.taxiForm.reset();   // Reset taxi form inputs
        this.clientForm.reset(); // Reset client form inputs
    }
}