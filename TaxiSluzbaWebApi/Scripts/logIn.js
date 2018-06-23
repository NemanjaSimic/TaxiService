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

function LogIn(user) {
    var korisnik = user;
    if (korisnik === null) {
        korisnik = {
            KorisnickoIme: `${$('#username').val()}`,
            Sifra: `${$('#password').val()}`
        };
    }

    $.ajax({
        type: 'GET',
        url: 'api/Korisnik/LogIn',
        data: JSON.stringify(korisnik),
        contentType: 'application/json; charset=utf-8',
        dataType: 'html',
        complete: function (data) {
            if (data.status === 200) {
                $('.mainView').html("");
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
            } else {
                DobraSifra();
                PogresanUsername();
            }
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
        LogIn(korisnik);
    }

        $('#buttonPrijava').click(function () {
            LogIn(korisnik);                  
    });

});
