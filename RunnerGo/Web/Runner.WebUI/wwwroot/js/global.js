
class GlobalJS {
    
    getStorage(key) {
        return localStorage.getItem(key);
    }

    setStorage(key, value) {
        localStorage.setItem(key, value);
    }

    removeStorage(key, value) {
        localStorage.removeItem(key);
    }

    attachCalendar(el, elRef, requestStr, type, displayMode) {

        const calendar = bulmaCalendar.attach(el, {
            type,
            displayMode,
            color: 'primary',
            isRange: false,
            allowSameDayRange: true,
            lang: 'pt-BR',
            startDate: undefined,
            endDate: undefined,
            minDate: null,
            maxDate: null,
            disabledDates: [],
            disabledWeekDays: undefined,
            highlightedDates: [],
            weekStart: 0,
            dateFormat: 'dd/MM/yyyy',
            enableMonthSwitch: true,
            enableYearSwitch: true,
            displayYearsCount: 50,
            editTimeManually: false,
            minuteSteps: 5
        })[0];

        const request = JSON.parse(requestStr);
        const date = new Date(Date.parse(request.value));

        calendar.value(date);

        calendar.on('save', function (datepicker) {
            const response = {
                value: datepicker.data.value()
            }
            const responseStr = JSON.stringify(response);

            elRef.invokeMethodAsync('OnSave', responseStr);
        });
    }

    updateCalendar(el, requestStr) {

        const request = JSON.parse(requestStr);
        const date = new Date(Date.parse(request.value));

        el.bulmaCalendar.value(date);
        el.bulmaCalendar.refresh();
    }
}

window.globalJS = new GlobalJS();