﻿$(document).on('click', '#view', function () {
    $('.mainView').html("");
    var korisnikJSON = sessionStorage.getItem('korisnik');
    korisnik = $.parseJSON(korisnikJSON);
    let informations = `<table><tr><td>Ime:</td><td>${korisnik.Ime}</td></tr>`;
    informations += `<tr><td>Prezime:</td><td>${korisnik.Prezime}</td></tr>`;
    informations += `<tr><td>JMBG:</td><td>${korisnik.JMBG}</td></tr>`;
    informations += `<tr><td>Korisnicko ime:</td><td>${korisnik.KorisnickoIme}</td></tr>`;
    informations += `<tr><td>Email:</td><td>${korisnik.Email}</td></tr>`;
    informations += `<tr><td>Broj telefona:</td><td>${korisnik.KontaktTelefon}</td></tr></table>`;
    $('.mainView').html(informations);
});

$(document).on('click', '#edit', function () {
    $('.mainView').html("");
    var korisnikJSON = sessionStorage.getItem('korisnik');
    korisnik = $.parseJSON(korisnikJSON);
    let informations = `<table><tr><td>Ime:</td><td><input type="text" value="${korisnik.Ime}" id="imeEdit" /></td></tr>`;
    informations += `<tr><td>Prezime:</td><td><input type="text" value="${korisnik.Prezime}" id="prezimeEdit" /></td></tr>`;
    informations += `<tr><td>JMBG:</td><td><input type="text" value="${korisnik.JMBG}" id="jmbgEdit" /></td></tr>`;
    informations += `<tr><td>Korisnicko ime:</td><td><input type="text" value="${korisnik.KorisnickoIme}" id="korisnickoImeEdit" /></td></tr>`;
    informations += `<tr><td>Email:</td><td><input type="email" value="${korisnik.Email}" id="emailEdit" /></td></tr>`;
    informations += `<tr><td>Broj telefona:</td><td><input type="text" value="${korisnik.KontaktTelefon}" id="brojTelefonaEdit" /></td></tr>`;
    informations += '<tr><td></td><td><button id="buttonEdit">Sacuvaj izmene</button></td></tr></table>';
    informations += '<div id="regValEdit"></div>';
    $('.mainView').html(informations);
});

$(document).on('click', '#logout', function () { 
        sessionStorage.setItem('korisnik', null);
        window.location.href = "Index.html";

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
        korisnik = $.parseJSON(korisnikJSON);

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
            type: 'POST',
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
        return false;
    }
});

$(document).on('click', '#change', function () {
    $('.mainView').html("");
    let informations = '<table><tr><td>Unesite sifru:</td><td><input type="password" id="sifraAut" /></td></tr>';
    informations += '<tr><td>Unesite novu sifru:</td><td><input type="password" id="sifraNova" /></td></tr>';
    informations += '<tr><td>Unesite ponovite novu sifru:</td><td><input type="password" id="sifraNovaP" /></td></tr>';
    informations += '<tr><td></td><td><button id="promeniSifru">Promeni sifru</ button></td></tr></table>';
    informations += '<div id="regValEditS"></div>';
    $('.mainView').html(informations);
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
            type: 'POST',
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
        return true;
    }

});
