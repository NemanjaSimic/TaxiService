$(document).on('click', '#changeLoc', function () {
    var korisnikJSON = sessionStorage.getItem('korisnik');
    korisnik = $.parseJSON(korisnikJSON);

    $.ajax({
        type: 'GET',
        url: 'api/Vozac/Lokacija',
        dataType: 'html',
        complete: function (data) {
            if (data.status === 200) {
                $('.mainView').html("");
                $('.mainView').append(data.responseText);
            } else {
                $('.mainView').html("");
                $('.mainView').append("<h1>GRESKA NA SERVERU!</h1>");
            }
        }
    });
});

let ocistiFormuL = function () {
    $('#ulica').val("");
    $('#brojKuce').val("");
    $('#pozivniBroj').val("");
    $('#mesto').val("");
};

$(document).off('click','#promeniLokaciju').on('click','#promeniLokaciju',function () {

    var retVal = true;
    $('#regVal').html("");
    if ($('#ulica').val() === "") {
        retVal = false;
        $('#regVal').append('<label>Polje ulica je obavezno!</label><br/>');
    }

    if ($('#brojKuce').val() === "") {
        retVal = false;
        $('#regVal').append('<label>Polje ID vozila automobila je obavezno!</label><br/>');
    } else {
        let input = $('#brojKuce').val();
        let pattern = /^\b\d{1,4}\b$/i;

        if (!pattern.test(input)) {
            retVal = false;
            $('#regVal').append('<label>Polje broj kuce nije u validnom formatu!</label><br/>');
        }
    }

    if ($('#pozivniBroj').val() === "") {
        retVal = false;
        $('#regVal').append('<label>Polje pozivni broj je obavezno!</label><br/>');
    } else {
        let input = $('#pozivniBroj').val();
        let pattern = /^\b\d{4,8}\b$/i;

        if (!pattern.test(input)) {
            retVal = false;
            $('#regVal').append('<label>Polje pozivni broj nije u validnom formatu!</label><br/>');
        }
    }

    if ($('#mesto').val() === "") {
        retVal = false;
        $('#regVal').append('<label>Polje naseljeno mesto je obavezno!</label><br/>');
    }

    if (retVal) {
        var korisnikJSON = sessionStorage.getItem('korisnik');
        korisnik = $.parseJSON(korisnikJSON);

        let lokacija = {
            Mesto: `${$('#mesto').val()}`,
            PozivniBroj: `${$('#pozivniBroj').val()}`,
            BrojKuce: `${$('#brojKuce').val()}`,
            Ulica: `${$('#ulica').val()}`,
            KorisnickoIme: korisnik.KorisnickoIme
        };

        $.ajax({
            type: 'PUT',
            url: '/api/Vozac/PromenaLokacije',
            data: JSON.stringify(lokacija),
            contentType: 'application/json; charset=utf-8',
            dataType: 'html',
            complete: function (data) {
                $('#regVal').html("");
                if (data.status === 200) {
                    $('#regVal').append(data.responseText);
                    ocistiFormuL();
                } else {                
                    $('#regVal').append(`Status: ${data.status}`);
                    $('#regVal').append(data.responseText);
                }
            }
        });
    }
});