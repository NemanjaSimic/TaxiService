$(document).on('click', '#kreirajVozaca', function () {
    //let form = '<div class="dispecer>';
    let form = '<h3>Registracija novog vozaca</h3><br/>';
    form += '<input type="text" id="ime" placeholder="Ime" autocomplete="off" /><br />';
    form += '<input type="text" id="prezime" placeholder="Prezime" autocomplete="off" /><br />';
    form += '<input type="text" id="jmbg" placeholder="JMBG" autocomplete="off" /><br />';
    form += '<select id="pol"><option value="0">Muski</option><option value="1"> Zenski</option></select><br />';
    form += '<input type="text" id="korisnickoIme" placeholder="Korisnicko ime" autocomplete="off" /> <br />';
    form += '<input type="password" id="sifra" placeholder="Sifra" autocomplete="off" /> <br />';
    form += '<input type="password" id="sifraPotvrda" placeholder="Potvrda sifre" autocomplete="off" /> <br />';
    form += '<input type="email" id="email" placeholder="Email adresa" autocomplete="off" /> <br />';
    form += '<input type="text" id="brojTelefona" placeholder="Kontakt telefon" autocomplete="off" /> <br />';
    form += '<input type="text" id="godisteAutomobila" placeholder="Godiste vozila" autocomplete="off" /> <br />';
    form += '<input type="text" id="registracijaAutomobila" placeholder="Registarska oznaka vozila" autocomplete="off" /> <br />';
    form += '<input type="text" id="idTaxi" placeholder="ID taxi vozila" autocomplete="off" /> <br />';
    form += '<select id="tip"><option value="1">Putnicki</option><option value="2">Kombi</option></select><br />';
    //form += '<input type="text" id="ulica" placeholder="Ulica" autocomplete="off" /> <br />';
    //form += '<input type="text" id="brojKuce" placeholder="Broj kuce" autocomplete="off" /> <br />';
    //form += '<input type="text" id="mesto" placeholder="Naseljeno mesto" autocomplete="off" /> <br />';
    //form += '<input type="text" id="pozivniBroj" placeholder="Postasnki broj" autocomplete="off" /> <br />';
    form += '<button id="buttonRegistracijaVozaca">Registruj se</button>';
    form += '<div id="regVal"></div></div>';
    $('.mainView').html(form);
});

let IsprazniFormu = function () {
    $('#ime').val("");
    $('#prezime').val("");
    $('#jmbg').val("");
    $('#korisnickoIme').val("");
    $('#sifra').val("");
    $('#sifraPotvrda').val("");
    $('#email').val("");
    $('#brojTelefona').val("");
    $('#godisteAutomobila').val("");
    $('#registracijaAutomobila').val("");
    $('#idTaxi').val("");
};

$(document).off('click', '#buttonRegistracijaVozaca').on('click', '#buttonRegistracijaVozaca', function () {
    var retVal = true;
    $('#regVal').html("");

    if ($('#ime').val() === "") {
        retVal = false;
        $('#regVal').append('<label>Polje ime je obavezno!</label><br/>');
    }

    if ($('#prezime').val() === "") {
        retVal = false;
        $('#regVal').append('<label>Polje prezime je obavezno!</label><br/>');
    }

    if ($('#jmbg').val() === "") {
        retVal = false;
        $('#regVal').append('<label>Polje jmbg je obavezno!</label><br/>');
    } else {
        let jmbginput = $('#jmbg').val();
        let pattern = /^\b\d{13}\b$/i;

        if (!pattern.test(jmbginput)) {
            retVal = false;
            $('#regVal').append('<label>Polje jmbg nije u ispravnom formatu!</label><br/>');
        }
    }

    if ($('#korisnickoIme').val() === "") {
        retVal = false;
        $('#regVal').append('<label>Polje korisnicko ime je obavezno!</label><br/>');
    }

    if ($('#sifra').val() === "") {
        retVal = false;
        $('#regVal').append('<label>Polje sifra je obavezno!</label><br/>');
    }

    if ($('#sifraPotvrda').val() === "") {
        retVal = false;
        $('#regVal').append('<label>Polje potvrda sifre je obavezno!</label><br/>');
    }

    if ($('#sifra').val() !== $('#sifraPotvrda').val()) {
        retVal = false;
        $('#regVal').append('<label>Lozinke se ne poklapaju!</label><br/>');
    }

    if ($('#email').val() === "") {
        retVal = false;
        $('#regVal').append('<label>Polje email je obavezno!</label><br/>');
    } else {
        let emailinput = $('#email').val();
        let pattern = /^\b\[a-zA-Z0-9._%-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}\b$/i;
        //if (!pattern.test(emailinput)) {
        //    retVal = false;
        //    $('#regVal').append('<label>Forma email-a nije validna !</label><br/>');
        //}
    }
    if ($('#brojTelefona').val() === "") {
        retVal = false;
        $('#regVal').append('<label>Polje broj telefona je obavezno!</label><br/>');
    } else {
        let brinput = $('#brojTelefona').val();
        let pattern = /^\b\d{8,10}\b$/i;
        if (!pattern.test(brinput)) {
            retVal = false;
            $('#regVal').append('<label>Polje broj telefona mora imati od 8 do 10 cifara!</label><br/>');
        }
    }

    if ($('#godisteAutomobila').val() === "") {
        retVal = false;
        $('#regVal').append('<label>Polje godiste automobila je obavezno!</label><br/>');
    } else if ($('#godisteAutomobila').val() > 2018 || $('#godisteAutomobila').val() < 1950) {
        retVal = false;
        $('#regVal').append('<label>Polje godiste automobila mora biti u opsegu izmedju 1950 i 2018!</label><br/>');
    } else {
        let godisteinput = $('#godisteAutomobila').val();
        let pattern = /^\b\d{4}\b$/i;

        if (!pattern.test(godisteinput)) {
            retVal = false;
            $('#regVal').append('<label>Polje godiste nije u ispravnom formatu!</label><br/>');
        }
    }

    if ($('#registracijaAutomobila').val() === "") {
        retVal = false;
        $('#regVal').append('<label>Polje registracija automobila je obavezno!</label><br/>');
    } else {
        let input = $('#registracijaAutomobila').val();
        let pattern = /^\b\d{6,8}\b$/i;

        if (!pattern.test(input)) {
            retVal = false;
            $('#regVal').append('<label>Polje registracija mora sadrzati od 6 do 8 brojeva!</label><br/>');
        }
    }

    if ($('#idTaxi').val() === "") {
        retVal = false;
        $('#regVal').append('<label>Polje ID vozila automobila je obavezno!</label><br/>');
    } else {
        let input = $('#idTaxi').val();
        let pattern = /^\b\d{4}\b$/i;

        if (!pattern.test(input)) {
            retVal = false;
            $('#regVal').append('<label>Polje ID vozila mora sadrzati 4 brojeva!</label><br/>');
        }
    }

   
    if (retVal) {
        let musterija = {
            Ime: `${$('#ime').val()}`,
            Prezime: `${$('#prezime').val()}`,
            JMBG: `${$('#jmbg').val()}`,
            KorisnickoIme: `${$('#korisnickoIme').val()}`,
            Sifra: `${$('#sifra').val()}`,
            Email: `${$('#email').val()}`,
            KontaktTelefon: `${$('#brojTelefona').val()}`,
            Pol: `${$('#pol').val()}`,
            GodisteAutomobila: `${$('#godisteAutomobila').val()}`,
            Id: `${$('#idTaxi').val()}`,
            Registracija: `${$('#registracijaAutomobila').val()}`,
            Tip: `${$('#tip').val()}`    
        };

        $.ajax({
            type: 'POST',
            url: '/api/Dispecer/Registracija',
            data: JSON.stringify(musterija),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json'
        }).always(function (data) {
            if (data.status === 409) {
                $('#regVal').append('<label>Korisnicko ime je zauzeto!</label><br/>');
            } else if (data.status === 200) {
                $('#regVal').append('<label>Vozac je uspesno registrovan!</label><br/>');
                IsprazniFormu();
            } else {
                $('#regVal').append('<label>Greska na serverskoj strani!</label><br/>');
            }
        });
    }
});

$(document).on('click', '#formirajVoznju', function () {
    var korisnikJSON = sessionStorage.getItem('korisnik');
    var korisnik = $.parseJSON(korisnikJSON);

    $.ajax({
        type: 'GET',
        url: 'api/Dispecer/Voznja/' + korisnik.KorisnickoIme,
        //data: JSON.stringify(korisnik),
        //contentType: 'application/json; charset=utf-8',
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

$(document).off('click', '#kreirajVoznjuD').on('click', '#kreirajVoznjuD', function () {
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

    if ($('#korisnickoImeV').val() === "") {
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
            KorisnickoImeV: `${$('#korisnickoImeV').val()}`,
            TipVozila: `${$('#tip').val()}`,
            Vozac: `${$('#vozac').val()}`,
            KorisnickoIme: korisnik.KorisnickoIme
        };

        $.ajax({
            type: 'POST',
            url: '/api/Dispecer/KreirajVoznju',
            data: JSON.stringify(voznja),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            complete: function (data) {
               if (data.status === 200) {
                    $('#regVal').append('<label>Voznja uspeno kreirana</label><br/>');
                    IsprazniFormu();
                } else {
                    $('#regVal').append('<label>Los zahtev!(Proverite vozaca)</label><br/>');
                }
            }
        });
        
    }
});

$(document).on('click', '#obradiVoznju', function () {
    $.ajax({
        type: 'GET',
        url: '/api/Dispecer/Obradi',            
        dataType: 'html',
        complete: function (data) {
            if (data.status === 200) {
                $('.mainView').html(data.responseText);
            } else {
                $('.mainView').html("<h1>GRESKA NA SERVERU!</h1>");
            }
        }
    });

});

$(document).off('click', '.obradi').on('click', '.obradi', function () {
    var id = `${$(this).val()}`;
    var korisnikJSON = sessionStorage.getItem('korisnik');
    korisnik = $.parseJSON(korisnikJSON);

    let data = {
        ID: id,
        Vozac: `${$('#idVozaca').val()}`,
        KorisnickoIme: korisnik.KorisnickoIme
    };

    $.ajax({
        type: 'PUT',
        url: '/api/Dispecer/Obradi',
        data: JSON.stringify(data),
        contentType: 'application/json; charset=utf-8',
        dataType: 'html',
        complete: function (data) {
            if (data.status === 200) {
                $('.mainView').html(data.responseText);
            } else {
                $('.mainView').html("<h1>GRESKA NA SERVERU!</h1>");
            }
        }
    });
});