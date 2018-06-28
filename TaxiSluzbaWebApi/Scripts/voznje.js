$(document).off('click', '#filtriraj').on('click', '#filtriraj', function () {
    var korisnikJSON = sessionStorage.getItem('korisnik');
    var korisnik = $.parseJSON(korisnikJSON);
    var datum = false;
    var ocena = false;
    var odOcena = $('#od').val();
    var doOcena = $('#do').val();
    var odCena = $('#odCena').val();
    var doCena = $('#doCena').val();
    var odDatum = $('#odDatum').val();
    var doDatum = $('#doDatum').val();
    var retVal = true;
    $('#filterError').html("");
    if (doOcena < odOcena) {
        retVal = false;
        $('#filterError').append("Kriterijum za ocenu je los!('Od' mora biti manji od 'Do')</br>");
    }

    if (doDatum !== "" && odDatum !=="") {
        if (doDatum < odDatum) {
            retVal = false;
            $('#filterError').append("Kriterijum za datum je los!('Od' mora biti manji od 'Do')</br>");
        }
    }
    

    if (doCena < odCena) {
        retVal = false;
        $('#filterError').append("Kriterijum za cenu je los!('Od' mora biti manji od 'Do')</br>");
    }

    if ($('#sortirajDatum').is(':checked')) {
         datum = true;
    } 

    if ($('#sortirajOcenu').is(':checked')) {
        ocena = true;
    }
    if (retVal) {
        var musterijaIme = `${$('#musterijaIme').val()}`;
        var musterijaPrezime = `${$('#musterijaPrezime').val()}`;
        var vozacIme = `${$('#vozacIme').val()}`;
        var vozacPrezime = `${$('#vozacPrezime').val()}`;
        var flag = `${$(this).val()}`;
        if (musterijaIme === "undefined") {
            musterijaIme = "";
        }
        if (musterijaPrezime === "undefined") {
            musterijaPrezime = "";
        }
        if (vozacIme === "undefined") {
            vozacIme = "";
        }
        if (vozacPrezime === "undefined") {
            vozacPrezime = "";
        }
        

        var data = {
            KorisnickoIme: korisnik.KorisnickoIme,
            Filter: `${$('#tipFiltera').val()}`,
            Datum: datum,
            Ocena: ocena,
            OdOcena: odOcena,
            DoOcena: doOcena,
            OdCena: odCena,
            DoCena: doCena,
            DoDatum: doDatum,
            OdDatum: odDatum,
            Flag: flag,
            MusterijaIme: musterijaIme,
            MusterijaPrezime: musterijaPrezime,
            VozacIme: vozacIme,
            VozacPrezime: vozacPrezime
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
    }
});
