function validatePalletBalance(source, eventArgs) {
    try {
        var currentValue = parseInt(eventArgs.Value, 10);
        eventArgs.IsValid = currentValue >= 0 ? true : false;
    }
    catch (err) {
        eventArgs.IsValid = false;
    }
}