function PogresnaSifra() {
    $('#password').css('border-color', 'red');
    $('#password').focus();
    $('#korImeVal').append("Pogresna sifra!");
}

function DobraSifra() {
    $('#password').css('border-color', '#6587AD');
    $('#korImeVal').html("");
}

function DobarUsername() {
    $('#username').css('border-color', '#6587AD');
    $('#korImeVal').html("");
}

function PogresanUsername() {
    $('#usrname').css('border-color', 'red');
    $('#usrname').focus();
    $('#korImeVal').append("Pogresan username!");

}

function LogIn(user,refresh) {
    var korisnik = user;
    if (korisnik === null) {
        korisnik = {
            KorisnickoIme: `${$('#username').val()}`,
            Sifra: `${$('#password').val()}`,        
            Refresh: refresh
        };
    } else {
        korisnik.Refresh = refresh;
    }

    $.ajax({
        type: 'GET',
        url: 'api/Korisnik/LogIn',
        data: JSON.stringify(korisnik),
        contentType: 'application/json; charset=utf-8',
        dataType: 'html',
        complete: function (data) {
            if (data.status === 200) {
                GetRides(korisnik);
                $('.header').append(data.responseText);
                sessionStorage.setItem('korisnik', JSON.stringify(korisnik));
                $('#korImeVal').html("");
                //$.ajax({
                //    url: "Scripts/account.js",
                //    dataType: "script"
                //});        
            } else if (data.status === 401) {
                DobarUsername();
                PogresnaSifra();
            } else if (data.status === 409) {
                DobraSifra();
                PogresanUsername();
            } else if (data.status === 403) {
                $('.mainView').html('<h1>Izlogujte se sa prijavljenog naloga!</h1>');
            } else {
                $('.mainView').html('<h1>Greska na serveru!</h1>');
            }
        }
    });
}

function GetRides(korisnik) {
    $.ajax({
        type: 'GET',
        url: 'api/Korisnik/Voznje',
        data: JSON.stringify(korisnik),
        contentType: 'application/json; charset=utf-8',
        dataType: 'html',
        complete: function (data) {
            $('.mainView').html(data.responseText);
        }
    });
}

$(document).ready(function () {
    $('#korImeVal').html("");
    if (sessionStorage.getItem('korisnik') === null || sessionStorage.getItem('korisnik') === "null") {
        korisnik = null;
    } else {
        var korisnikJSON = sessionStorage.getItem('korisnik');
        korisnik = $.parseJSON(korisnikJSON);
        LogIn(korisnik,true);
    }

        $('#buttonPrijava').click(function () {
            LogIn(korisnik,false);                  
    });


});

//$(document).on('click', '#ucitaj', function () {
//    var korisnikJSON = sessionStorage.getItem('korisnik');
//    var korisnik = $.parseJSON(korisnikJSON);
//    $.ajax({
//        type: 'GET',
//        url: 'api/Korisnik/Voznje',
//        data: JSON.stringify(korisnik),
//        contentType: 'application/json; charset=utf-8',
//        dataType: 'html',
//        complete: function (data) {
//            $('.voznje').append(data.responseText);
//        }
//    });
//});