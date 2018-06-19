$(document).ready(function () {

    let IsprazniFormu = function () {
        $('#ime').val("");
        $('#prezime').val("");
        $('#jmbg').val("");
        $('#korisnickoIme').val("");
        $('#sifra').val("");
        $('#sifraPotvrda').val("");
        $('#email').val("");
        $('#brojTelefona').val("");
    };

    $('#buttonSingUp').click(function () {
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

        if (retVal) {
            let musterija = {
                Ime: `${$('#ime').val()}`,
                Prezime: `${$('#prezime').val()}`,
                JMBG: `${$('#jmbg').val()}`,
                KorisnickoIme: `${$('#korisnickoIme').val()}`,
                Sifra: `${$('#sifra').val()}`,
                Email: `${$('#email').val()}`,
                KontaktTelefon: `${$('#brojTelefona').val()}`,
                Pol: `${$('#pol').val()}`
            };

            $.ajax({
                type: 'POST',
                url: '/api/Korisnik/Registracija',
                data: JSON.stringify(musterija),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json'
            }).always(function (data) {
                if (data.status === 409) {
                    $('#regVal').append('<label>Korisnicko ime je zauzeto!</label><br/>');
                } else if (data.status === 200) {
                    $('#regVal').append('<label>Korisnik uspesno registrovan!</label><br/>');
                    IsprazniFormu();
                }
            });
        }

    });
});