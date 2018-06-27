$(document).on('click', '#view', function () {  
    var korisnikJSON = sessionStorage.getItem('korisnik');
    var korisnik = $.parseJSON(korisnikJSON);

    $.ajax({
        type: 'GET',
        url: 'api/Korisnik/Profil',
        data: korisnikJSON,
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

$(document).off('click', '#home').on('click','#home',function () {
    window.location.href = "Index.html";
});

$(document).on('click', '#edit', function () {
    var korisnikJSON = sessionStorage.getItem('korisnik');
    var korisnik = $.parseJSON(korisnikJSON);

    $.ajax({
        type: 'GET',
        url: 'api/Korisnik/ProfilEdit',
        data: korisnikJSON,
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

$(document).on('click', '#change', function () {
    $.ajax({
        type: 'GET',
        url: 'api/Korisnik/Sifra',
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

$(document).on('click', '#logout', function () { 
    var korisnikJSON = sessionStorage.getItem('korisnik');
    var korisnik = $.parseJSON(korisnikJSON);

    $.ajax({
        type: 'DELETE',
        url: 'api/Korisnik/LogOut',
        data: JSON.stringify(korisnik),
        contentType: 'application/json; charset=utf-8',
        dataType: 'html',
        complete: function (data) {
            if (data.status === 200) {
                sessionStorage.setItem('korisnik', null);
                window.location.href = "Index.html";
            } else {
                $('.mainView').html("");
                $('.mainView').append("<h1>GRESKA NA SERVERU!</h1>");
            }
        }
    });
});

$(document).off('click', '#buttonEdit').on('click', '#buttonEdit', function () {

    var retVal = true;
    $('#regValEdit').html("");

    if ($('#imeEdit').val() === "") {
        retVal = false;
        $('#regValEdit').append('<label>Polje ime je obavezno!</label><br/>');
    }

    if ($('#prezimeEdit').val() === "") {
        retVal = false;
        $('#regValEdit').append('<label>Polje prezime je obavezno!</label><br/>');
    }

    if ($('#jmbgEdit').val() === "") {
        retVal = false;
        $('#regValEdit').append('<label>Polje jmbg je obavezno!</label><br/>');
    } else {
        let jmbginput = $('#jmbgEdit').val();
        let pattern = /^\b\d{13}\b$/i;

        if (!pattern.test(jmbginput)) {
            retVal = false;
            $('#regValEdit').append('<label>Polje jmbg nije u ispravnom formatu!</label><br/>');
        }
    }

    if ($('#korisnickoImeEdit').val() === "") {
        retVal = false;
        $('#regValEdit').append('<label>Polje korisnicko ime je obavezno!</label><br/>');
    }

    if ($('#emailEdit').val() === "") {
        retVal = false;
        $('#regValEdit').append('<label>Polje email je obavezno!</label><br/>');
    } else {
        let emailinput = $('#emailEdit').val();
        let pattern = /^\b\[a-zA-Z0-9._%-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}\b$/i;
        //if (!pattern.test(emailinput)) {
        //    retVal = false;
        //    $('#regVal').append('<label>Forma email-a nije validna !</label><br/>');
        //}
    }
    if ($('#brojTelefonaEdit').val() === "") {
        retVal = false;
        $('#regValEdit').append('<label>Polje broj telefona je obavezno!</label><br/>');
    } else {
        let brinput = $('#brojTelefonaEdit').val();
        let pattern = /^\b\d{8,10}\b$/i;
        if (!pattern.test(brinput)) {
            retVal = false;
            $('#regValEdit').append('<label>Polje broj telefona mora imati od 8 do 10 cifara!</label><br/>');
        }
    }
/////////////////////////////////////////////////////////////////
    if (retVal) {
        var korisnikJSON = sessionStorage.getItem('korisnik');
        var korisnik = $.parseJSON(korisnikJSON);

        let musterija = {
            Ime: `${$('#imeEdit').val()}`,
            Prezime: `${$('#prezimeEdit').val()}`,
            JMBG: `${$('#jmbgEdit').val()}`,
            KorisnickoIme: `${$('#korisnickoImeEdit').val()}`,           
            Email: `${$('#emailEdit').val()}`,
            KontaktTelefon: `${$('#brojTelefonaEdit').val()}`,
            Sifra: `${korisnik.KorisnickoIme}`
        };

        $.ajax({
            type: 'PUT',
            url: '/api/Korisnik/Edit',
            data: JSON.stringify(musterija),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (data) {            
                    $('#regValEdit').append('<label>Uspesno sacuvane promene!</label><br/>');
                    sessionStorage.setItem('korisnik', JSON.stringify(data));
            },
            error: function (data) {
                if (data.status === 409) {
                    $('#regValEdit').append('<label>Korisnicko ime je zauzeto!</label><br/>');
                }
            }
        });
    }
});

$(document).off('click','#promeniSifru').on('click', '#promeniSifru', function () {
    $('#regValEditS').html("");

    var korisnikJSON = sessionStorage.getItem('korisnik');
    korisnik = $.parseJSON(korisnikJSON);
    var retVal = true;

    if ($('#sifraAut').val() === "") {
        retVal = false;
        $('#regValEditS').append('<label>Polje za sifru ne sme biti prazno!</label><br/>');
    } else if ($('#sifraAut').val() !== korisnik.Sifra) {
            retVal = false;
            $('#regValEditS').append('<label>Sifra nije ispravna!</label><br/>');
    }
    

    if ($('#sifraNova').val() === "") {
        retVal = false;
        $('#regValEditS').append('<label>Polje za novu sifru ne sme biti prazno!</label><br/>');
    }

    if ($('#sifraNova').val() !== $('#sifraNovaP').val()) {
        retVal = false;
        $('#regValEditS').append('<label>Nova sifra se ne poklapa sa ponovljenom!</label><br/>');
    }

    if ($('#sifraNova').val() === $('#sifraAut').val()) {
        retVal = false;
        $('#regValEditS').append('<label>Nova sifra se poklapa sa starom!</label><br/>');
    }
    

    if (retVal) {
        korisnik.Sifra = `${$('#sifraNova').val()}`;
        sessionStorage.setItem('korisnik', JSON.stringify(korisnik));
        $.ajax({
            type: 'PUT',
            url: '/api/Korisnik/ChangePass',
            data: JSON.stringify(korisnik),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (data) {
                $('#regValEditS').append('<label>Sifra uspesno promenjena!</label><br/>');
                sessionStorage.setItem('korisnik', JSON.stringify(data));
            },
            error: function (data) {            
                    $('#regValEditS').append('<label>Neko je cackao skriptu!</label><br/>');              
            }
        });
    }

});
