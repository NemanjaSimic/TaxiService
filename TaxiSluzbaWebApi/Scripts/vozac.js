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
        $('#regVal').append('<label>Polje broj kuce je obavezno!</label><br/>');
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

$(document).on('click', '#changeStatus', function () {
    var korisnikJSON = sessionStorage.getItem('korisnik');
    var korisnik = $.parseJSON(korisnikJSON);

    $.ajax({
        type: 'GET',
        url: 'api/Vozac/Status',
        data: JSON.stringify(korisnik),
        contentType: 'application/json; charset=utf-8',
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

$(document).on('click', '#promeniStatus', function () {
    
    var retVal = true;
    if ($('#statusV').val() === "6") {
        $('#regVal').html("");
        if ($('#ulicaO').val() === "") {
            retVal = false;
            $('#regVal').append('<label>Polje ulica je obavezno!</label><br/>');
        }

        if ($('#brojO').val() === "") {
            retVal = false;
            $('#regVal').append('<label>Polje broj kuce je obavezno!</label><br/>');
        } else {
            let input = $('#brojO').val();
            let pattern = /^\b\d{1,4}\b$/i;

            if (!pattern.test(input)) {
                retVal = false;
                $('#regVal').append('<label>Polje broj kuce nije u validnom formatu!</label><br/>');
            }
        }

        if ($('#iznos').val() === "") {
            retVal = false;
            $('#regVal').append('<label>Polje iznos je obavezno!</label><br/>');
        } else {
            let input = $('#iznos').val();
            let pattern = /^\b\d{1,6}\b$/i;

            if (!pattern.test(input)) {
                retVal = false;
                $('#regVal').append('<label>Polje iznos nije u validnom formatu!</label><br/>');
            }
        }

        if ($('#pozivniBrojO').val() === "") {
            retVal = false;
            $('#regVal').append('<label>Polje pozivni broj je obavezno!</label><br/>');
        } else {
            let input = $('#pozivniBrojO').val();
            let pattern = /^\b\d{4,8}\b$/i;

            if (!pattern.test(input)) {
                retVal = false;
                $('#regVal').append('<label>Polje pozivni broj nije u validnom formatu!</label><br/>');
            }
        }

        if ($('#mestoO').val() === "") {
            retVal = false;
            $('#regVal').append('<label>Polje naseljeno mesto je obavezno!</label><br/>');
        }
    } else {
        if ($('#ulicaO').val() !== "") {
            retVal = false;
            $('#regVal').html('<label>Polje moraju biti prazna! Status voznje je neuspesna!</label><br/>');
        }

        if ($('#brojO').val() !== "") {
            retVal = false;
            $('#regVal').html('<label>Polje moraju biti prazna! Status voznje je neuspesna!</label><br/>');
        } 


        if ($('#pozivniBrojO').val() !== "") {
            retVal = false;
            $('#regVal').html('<label>Polje moraju biti prazna! Status voznje je neuspesna!</label><br/>');
        } 

        if ($('#mestoO').val() !== "") {
            retVal = false;
            $('#regVal').html('<label>Polje moraju biti prazna! Status voznje je neuspesna!</label><br/>');
        }

        if ($('#pozivniBrojO').val() !== "") {
            retVal = false;
            $('#regVal').append('<label>Polje pozivni broj je obavezno!</label><br/>');
        }

    }
    if (retVal) {
        var korisnikJSON = sessionStorage.getItem('korisnik');
        var korisnik = $.parseJSON(korisnikJSON);

        let data = {
            Mesto: `${$('#mestoO').val()}`,
            PozivniBroj: `${$('#pozivniBrojO').val()}`,
            BrojKuce: `${$('#brojO').val()}`,
            Ulica: `${$('#ulicaO').val()}`,
            Iznos: `${$('#iznos').val()}`,
            KorisnickoIme: korisnik.KorisnickoIme,
            Status: `${$('#statusV').val()}`,
            ID: `${$('#promeniStatus').val()}`
        };

        $.ajax({
            type: 'PUT',
            url: 'api/Vozac/ZavrsiVoznju',
            data: JSON.stringify(data),
            contentType: 'application/json; charset=utf-8',
            dataType: 'html',
            complete: function (data) {
                if (data.status === 302) {
                    $(':button').not('#komentarisi :button').attr('disabled', true);
                }
                $('.mainView').html("");
                $('.mainView').append(data.responseText);
            }
        });
    }
});

$(document).on('click', '#waiting', function () {
    var korisnikJSON = sessionStorage.getItem('korisnik');
    var korisnik = $.parseJSON(korisnikJSON);

    $.ajax({
        type: 'GET',
        url: 'api/Vozac/Obradi',
        data: JSON.stringify(korisnik),
        contentType: 'application/json; charset=utf-8',
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

$(document).off('click', '.obradiV').on('click', '.obradiV', function () {
    var id = `${$(this).val()}`;
    var korisnikJSON = sessionStorage.getItem('korisnik');
    korisnik = $.parseJSON(korisnikJSON);

    let data = {
        ID: id,
        KorisnickoIme: korisnik.KorisnickoIme
    };

    $.ajax({
        type: 'PUT',
        url: '/api/Vozac/Obradi',
        data: JSON.stringify(data),
        contentType: 'application/json; charset=utf-8',
        dataType: 'html',
        complete: function (data) {
            if (data.status === 200) {
                $('.mainView').html(data.responseText);
            } else {
                $('.mainView').html(data.responseText);
            }
        }
    });
});

$(document).off('click', '#komentarisi').on('click', '#komentarisi', function () {
    var id = `${$(this).val()}`;
    var korisnikJSON = sessionStorage.getItem('korisnik');
    korisnik = $.parseJSON(korisnikJSON);


    let data = {
        ID: id,
        KorisnickoIme: korisnik.KorisnickoIme,
        Komentar: `${$('textarea').val()}`,
        Ocena: `${$('#ocena').val()}`
    };

    $.ajax({
        type: 'POST',
        url: '/api/Vozac/Komentarisi',
        data: JSON.stringify(data),
        contentType: 'application/json; charset=utf-8',
        dataType: 'html',
        complete: function (data) {
            if (data.status === 200) {
                $('.mainView').html(data.responseText);
                $(':button').not('#komentarisi :button').attr('disabled', false);
            } else {
                $('.mainView').html(data.responseText);
            }
        }
    });
});