class FormManager {

    static clientForm = document.getElementById('client-form');
    static taxiForm = document.getElementById('taxi-form');
    static clientButton = document.querySelector('.registration-option:nth-child(1)');
    static taxiButton = document.querySelector('.registration-option:nth-child(2)');

    static selectOption(option) {

        if (option === 'client') {
            this.clientForm.classList.remove('hidden');
            this.taxiForm.classList.add('hidden');
            this.clientButton.classList.add('selected');
            this.taxiButton.classList.remove('selected');
        }
        else if (option === 'taxi') {
            this.taxiForm.classList.remove('hidden');
            this.clientForm.classList.add('hidden');
            this.clientButton.classList.remove('selected');
            this.taxiButton.classList.add('selected');
        }
    }

    static clearFormInputs() {

        this.taxiForm.reset();
        this.clientForm.reset();
    }
}