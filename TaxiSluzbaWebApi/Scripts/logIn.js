if (sessionStorage.getItem('korisnik') === null) {
    var korisnik = null;
} else {
    var korisnikJSON = sessionStorage.getItem('korisnik');
    korisnik = $.parseJSON(korisnikJSON);
}

function DispecerUI() {
    $('.header').append('<button id="view">Pregledaj profil</button>');
    $('.header').append('<button id="edit">Izmeni profil</button>');
    $('.header').append('<button id="change">Promeni sifru</button>');
    $('.header').append('<button id="kreirajVozaca">Kreiraj vozaca</button>');
    $('.header').append('<button id="promeniVoznje">Ucitaj sve voznje</button>');
    $('.header').append('<button id="formirajVoznju">Formiraj voznju</button>');
    $('.header').append('<button id="logout">Izloguj se</button>');
    $('.mainView').html(""); 
}

function MusterijaUI() {
    $('.header').append('<button id="view">Pregledaj profil</button>');
    $('.header').append('<button id="edit">Izmeni profil</button>');
    $('.header').append('<button id="change">Promeni sifru</button>');
    $('.header').append('<button id="request">Zahtev za voznju</button>');
    $('.header').append('<button id="sort">Sortiraj voznje</button>');
    $('.header').append('<button id="filter">Filtriraj voznje</button>');
    $('.header').append('<button id="logout">Izloguj se</button>');
    $('.mainView').html("");
}

function VozacUI() {
    $('.header').append('<button id="view">Pregledaj profil</button>');
    $('.header').append('<button id="edit">Izmeni profil</button>');
    $('.header').append('<button id="change">Promeni sifru</button>');
    $('.header').append('<button id="changeLoc">Promeni lokaciju</button>');
    $('.header').append('<button id="changeStatus">Promeni status voznje</button>');
    $('.header').append('<button id="waiting">Prikazi voznje na cekanju</button>');
    $('.header').append('<button id="logout">Izloguj se</button>');
    $('.mainView').html("");
}

function PogresanUnos() {
    $('#korImeVal').show();
    $('.logInPolje').css('border-color', 'red');
}

function LogIn() {
    var korisnik = {
        korisnickoIme: `${$('#username').val()}`,
        sifra: `${$('#password').val()}`
    };

    $.ajax({
        type: 'POST',
        url: '/api/Korisnik/LogIn',
        data: JSON.stringify(korisnik),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (data) {
            if (data !== null) {
                $.ajax({
                    url: "Scripts/account.js",
                    dataType: "script"
                });
                $('#korImeVal').hide();
                $('.Prijavljen').show();
                $('#dobrodoslica').text(`Dobrodosli, ${data.KorisnickoIme} !`);
                sessionStorage.setItem('korisnik', JSON.stringify(data));
                if (data.Uloga === 2) {
                    sessionStorage.setItem('uloga', 'dispecer');
                    DispecerUI();
                    //$.ajax({
                    //    url: "Scripts/account.js",
                    //    dataType: "script"
                    //}).always(function (data) {

                    //    });

                } else if (data.Uloga === 1) {
                    sessionStorage.setItem('uloga', 'vozac');
                    VozacUI();
                } else if (data.Uloga === 0) {
                    sessionStorage.setItem('uloga', 'musterija');
                    MusterijaUI();
                }
            } else {
                PogresanUnos();
            }
        },
        error: function (data) {
            PogresanUnos();
        }

    });
}

$(document).ready(function () {
    if (korisnik !== null) {
        if (korisnik.Uloga === 0) {
            MusterijaUI();
            $.ajax({
                url: "Scripts/account.js",
                dataType: "script"
            });
        } else if (korisnik.Uloga === 2) {
            DispecerUI();
            $.ajax({
                url: "Scripts/account.js",
                dataType: "script"
            });
        } else if (korisnik.Uloga === 1) {
            VozacUI();
            $.ajax({
                url: "Scripts/account.js",
                dataType: "script"
            });
        }
    }
   

        $('#buttonPrijava').click(function () {
            LogIn();                  
    });

});
