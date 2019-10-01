
function initDate() {
    var allTags = document.getElementsByTagName("*");
    for (var i = 0; i < allTags.length; i++) {
        //Se cambia il classname nel css cambiare anche la stringa sotto nell'index of
        if (allTags[i].className.indexOf("lg") == 0) {
            showTheTime(allTags[i].className.substring(2, 4), allTags[i].className.substring(7));
        }
    }

    setTimeout("initDate()", 1000);
}

function showTheTime(idLanguage) {
    var dayName;
    var monthname;

    switch (idLanguage) {
        case "1": //Italiano
            dayName = new Array("Domenica", "Lunedì", "Martedì", "Mercoledì", "Giovedì", "Venerdì", "Sabato");
            monthname = new Array("Gennaio", "Febbraio", "Marzo", "Aprile", "Maggio", "Giugno", "Luglio", "Agosto",
                    "Settembre", "Ottobre", "Novembre", "Dicembre");
            break;
        case "2": //Inglese
            dayName = new Array("Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday");
            monthname = new Array("Jenuary", "February", "March", "April", "May", "June", "July", "August",
                    "Semptember", "October", "November", "December");
            break;
        //            case "3": //Arabo  
        //                 
        //                break;  
    }

    var currdate = new Date();
    var dateStr = currdate.toString();

    dateStr = dateStr.substr(0, dateStr.length - 3);

    var li = document.getElementById('spanris');

    if (idLanguage == 1)
        li.innerHTML = dayName[currdate.getDay()] + ' ' + currdate.getDate() + ' ' + monthname[currdate.getMonth()] + ' ' +
            currdate.getFullYear() + '  ' + showTheHours(currdate.getHours()) + showZeroFilled(currdate.getMinutes()) +
            showZeroFilled(currdate.getSeconds());
    else
        li.innerHTML = dayName[currdate.getDay()] + ' ' + currdate.getDate() + ' ' + monthname[currdate.getMonth()] + ' ' +
            currdate.getFullYear() + '  ' + showTheHoursEng(currdate.getHours()) + showZeroFilled(currdate.getMinutes()) +
            showZeroFilled(currdate.getSeconds()) + ' ' + showAmPm(currdate.getHours());

    function showTheHours(theHour) {
        if (theHour == 0) {
            return (24);
        }
        if (theHour < 10)
            return ('0' + theHour);

        return theHour;
    }

    function showZeroFilled(inValue) {
        if (inValue > 9) {
            return ":" + inValue;
        }
        return ":0" + inValue;
    }

    function showTheHoursEng(theHour) {
        if (theHour == 0) {
            return (12);
        }
        if (theHour < 10)
            return ('0' + theHour);
        if (theHour < 13) {
            return (theHour);
        }
        return (theHour - 12);
        return theHour;
    }

    function showAmPm(thatTime) {
        if (thatTime < 12) {
            return (" AM ");
        }
        return (" PM ");
    }
}