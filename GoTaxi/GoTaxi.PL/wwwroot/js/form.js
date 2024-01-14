function selectOption(option) {
    const taxiForm = document.getElementById('taxi-form');
    const clientForm = document.getElementById('client-form');
    const clientButton = document.querySelector('.registration-option:nth-child(1)');
    const taxiButton = document.querySelector('.registration-option:nth-child(2)');

    if (option === 'client') {
        clientForm.classList.remove('hidden');
        taxiForm.classList.add('hidden');
        clientButton.classList.add('selected');
        taxiButton.classList.remove('selected');
    }
    else if (option === 'taxi') {
        taxiForm.classList.remove('hidden');
        clientForm.classList.add('hidden');
        clientButton.classList.remove('selected');
        taxiButton.classList.add('selected');
    }
}

function clearFormInputs() {
    taxiForm.reset();
    clientForm.reset();
}