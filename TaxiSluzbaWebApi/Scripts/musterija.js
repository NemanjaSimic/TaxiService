korisnikJSON = sessionStorage.getItem('korisnik');
korisnik = $.parseJSON(korisnikJSON);

$(document).on('click', '#request', function () {
    $('.mainView').html("");
    let informations = '<table><tr><td>Ulica:</td><td><input type="text" id="ulica" placeholder="npr.Bulevar Oslobodjenja" autocomplete="off" /></td></tr>';
    informations += '<tr><td>Broj kuce/zgrade:</td><td><input type="text" id="brojKuce" placeholder="npr.147" autocomplete="off" /></td></tr>';
    informations += '<tr><td>Naseljeno mesto:</td><td><input type="text" id="mesto" placeholder="npr.Novi Sad" autocomplete="off" /></td></tr>';
    informations += '<tr><td>Postanski broj:</td><td><input type="text" id="pozivniBroj" placeholder="npr.21000" autocomplete="off" /></td></tr>';
    informations += '<tr><td>Zeljeni tip vozila:</td><td><select id="tip"><option value="0">-</option><option value="1">Putnicki</option><option value="2">Kombi</option></select></td></tr>';
    informations += '<tr><td></td><td><button id="kreirajVoznju">Posalji zahtev za voznju</ button></td></tr></table>';
    informations += '<div id="regVal"></div>';
    $('.mainView').html(informations);
});

let IsprazniFormuMV = function () {
    $('#ulica').val("");
    $('#brojKuce').val("");
    $('#mesto').val("");
    $('#pozivniBroj').val("");
};
$(document).off('click', '#kreirajVoznju').on('click', '#kreirajVoznju', function () {
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

        let voznja = {
            Ulica: `${$('#ulica').val()}`,
            Broj: `${$('#brojKuce').val()}`,
            PozivniBroj: `${$('#pozivniBroj').val()}`,
            Mesto: `${$('#mesto').val()}`,
            TipVozila: `${$('#tip').val()}`,
            KorisnickoIme: korisnik.KorisnickoIme
        };

        $.ajax({
            type: 'POST',
            url: '/api/Musterija/KreirajVoznju',
            data: JSON.stringify(voznja),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            complete: function (data) {
                if (data.status === 200) {
                    $('#regVal').append('<label>Voznja uspeno kreirana</label><br/>');
                    IsprazniFormuMV();
                } else {
                    $('#regVal').append('<label>Neko je nesto cackao !</label><br/>');
                }
            }
        });

    }
});

$(document).on('click', '#otkaziVoznju', function () {
    
    let voznja = {
        Id: `${$(this).val()}`,
        KorisnickoIme: korisnik.KorisnickoIme
    };
    $.ajax({
        type: 'DELETE',
        url: '/api/Musterija/OtkaziVoznju',
        data: JSON.stringify(voznja),
        contentType: 'application/json; charset=utf-8',
        dataType: 'html',
        complete: function (data) {
            if (data.status === 200) {
                $('.mainView').html(data.responseText);
                $(':button').not('#komentarisiVoznju :button').attr('disabled', true);
            } else if (data.status === 409) {
                $('.mainView').html('<label>Voznja je prihvacena u medjuvremenu!</label><br/>');  
            } else if (data.status === 401) {
                $('.mainView').html('<label>Niste autorizovani za ovu radnju!</label><br/>');
            } else if (data.status === 403) {
                $('.mainView').html('<label>Niste ulogovani!</label><br/>');
            }
            else {
                $('#regVal').append('<label>Greska na serveru!</label><br/>');
            }
        }
    });

});

$(document).on('click', '#izmeniVoznju', function () {
    var retVal = true;
    $('#regVal').html("");

    if ($('#ulicaM').val() === "") {
        retVal = false;
        $('#regVal').append('<label>Polje ulica je obavezno!</label><br/>');
    }

    if ($('#brojKuceM').val() === "") {
        retVal = false;
        $('#regVal').append('<label>Polje ID vozila automobila je obavezno!</label><br/>');
    } else {
        let input = $('#brojKuceM').val();
        let pattern = /^\b\d{1,4}\b$/i;

        if (!pattern.test(input)) {
            retVal = false;
            $('#regVal').append('<label>Polje broj kuce nije u validnom formatu!</label><br/>');
        }
    }

    if ($('#pozivniBrojM').val() === "") {
        retVal = false;
        $('#regVal').append('<label>Polje pozivni broj je obavezno!</label><br/>');
    } else {
        let input = $('#pozivniBrojM').val();
        let pattern = /^\b\d{4,8}\b$/i;

        if (!pattern.test(input)) {
            retVal = false;
            $('#regVal').append('<label>Polje pozivni broj nije u validnom formatu!</label><br/>');
        }
    }

    if ($('#mestoM').val() === "") {
        retVal = false;
        $('#regVal').append('<label>Polje naseljeno mesto je obavezno!</label><br/>');
    }

    if (retVal) {
        let voznja = {
            Id: `${$(this).val()}`,
            Ulica: `${$('#ulicaM').val()}`,
            Broj: `${$('#brojKuceM').val()}`,
            PozivniBroj: `${$('#pozivniBrojM').val()}`,
            Mesto: `${$('#mestoM').val()}`,
            KorisnickoIme: korisnik.KorisnickoIme
        };
        $.ajax({
            type: 'PUT',
            url: '/api/Musterija/IzmeniVoznju',
            data: JSON.stringify(voznja),
            contentType: 'application/json; charset=utf-8',
            dataType: 'html',
            complete: function (data) {
                if (data.status === 200) {
                    $('.mainView').html('<label>Voznja uspesno izmenjena</label><br/>');
                } else if (data.status === 409) {
                    $('.mainView').html('<label>Voznja je prihvacena u medjuvremenu!</label><br/>');
                } else if (data.status === 401) {
                    $('.mainView').html('<label>Niste autorizovani za ovu radnju!</label><br/>');
                } else if (data.status === 403) {
                    $('.mainView').html('<label>Niste ulogovani!</label><br/>');
                }
                else {
                    $('#regVal').append('<label>Greska na serveru!</label><br/>');
                }
            }
        });
    }
});

$(document).off('click', '#komentarisiVoznju').on('click', '#komentarisiVoznju', function () {
    var id = `${$(this).val()}`;
    var korisnikJSON = sessionStorage.getItem('korisnik');
    korisnik = $.parseJSON(korisnikJSON);

    var retVal = true;

    if ($('textarea').val() === "") {
        retVal = false;
        $('textarea').prop('border-color', 'red');
    }

    let data = {
        ID: id,
        KorisnickoIme: korisnik.KorisnickoIme,
        Komentar: `${$('textarea').val()}`,
        Ocena: `${$('#ocena').val()}`
    };

    $.ajax({
        type: 'POST',
        url: '/api/Musterija/Komentarisi',
        data: JSON.stringify(data),
        contentType: 'application/json; charset=utf-8',
        dataType: 'html',
        complete: function (data) {
            if (data.status === 200) {
                window.location.href = "Index.html";
                $(':button').not('#komentarisiVoznju :button').attr('disabled', false);
            } else {
                $('.mainView').html(data.responseText);
            }
        }
    });
});
