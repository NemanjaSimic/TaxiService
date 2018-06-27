$(document).off('click', '#filtriraj').on('click', '#filtriraj', function () {
    var korisnikJSON = sessionStorage.getItem('korisnik');
    var korisnik = $.parseJSON(korisnikJSON);

    var data = {
        KorisnickoIme: korisnik.KorisnickoIme,
        Filter: `${$('#tipFiltera').val()}`
    };

    $.ajax({
        type: 'GET',
        url: 'api/Voznja/Filter',
        data: JSON.stringify(data),
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

$(document).off('click', '#sortirajDatum').on('click', '#sortirajDatum', function () {
    var korisnikJSON = sessionStorage.getItem('korisnik');
    var korisnik = $.parseJSON(korisnikJSON);

    var data = {
        KorisnickoIme: korisnik.KorisnickoIme
    };

    $.ajax({
        type: 'GET',
        url: 'api/Voznja/SortDatum',
        data: JSON.stringify(data),
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