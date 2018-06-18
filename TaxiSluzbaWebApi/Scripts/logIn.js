function DispecerUI() {
    $('.header').append('<button id="view">Pregledaj profil</button>');
    $('.header').append('<button id="edit">Izmeni profil</button>');
    $('.header').append('<button id="change">Promeni sifru</button>');
    $('.header').append('<button id="kreirajVozaca">Kreiraj vozaca</button>');
    $('.header').append('<button id="promeniVoznje">Ucitaj sve voznje</button>');
    $('.header').append('<button id="formirajVoznju">Formiraj voznju</button>');
    $('.header').append('<button id="logout">Izloguj se</button>');
    $('.mainView').hide();
}



$(document).ready(function () {
    $('#buttonPrijava').click(function () {
        var korisnik = {
            korisnickoIme: `${$('#username').val()}`,
            sifra: `${$('#password').val()}`
        };

        $.ajax({
            type: 'POST',
            url: '/api/Korisnik/LogIn',
            data: JSON.stringify(korisnik),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json'
        }).done(function (data) {
            if (data !== null) {
                $('#korImeVal').hide();
                $('.Prijavljen').show();
                $('#dobrodoslica').text(`Dobrodosli, ${data.KorisnickoIme} !`);
                sessionStorage.setItem('korisnik', JSON.stringify(data));
                if (data.Uloga == 2) {
                    DispecerUI();
                } else if (data.Uloga == 1) {

                } else if (data.Uloga == 0) {

                }
            } else {
                $('#korImeVal').show();
                $('.Prijavljen').hide();


            }
        });
    });
});
