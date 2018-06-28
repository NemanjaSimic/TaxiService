
function displayLocation(latitude, longitude) {
    var request = new XMLHttpRequest();
    var method = 'GET';
    var url = 'http://maps.googleapis.com/maps/api/geocode/json?latlng='
        + latitude + ',' + longitude + '&sensor=true';
    var async = false;
    var address;
    request.open(method, url, async);
    request.onreadystatechange = function () {
        if (request.readyState === 4 && request.status === 200) {
            var data = JSON.parse(request.responseText);
            address = data.results[0];
            var value = address.formatted_address.split(",");
            count = value.length;
            country = value[count - 1];
            state = value[count - 2];
            city = value[count - 3];
        }
    };
    request.send();
    return address.formatted_address;
}
markers = [];
function placeMarker(map, location) {
    if (markers.length > 0) {
        markers[markers.length - 1].setMap(null);
        markers = [];
    }
    var marker = new google.maps.Marker({
        position: location,
        map: map
    });

    var fullAdresa = displayLocation(location.lat(), location.lng());
    var delovi = fullAdresa.split(",");

    var ulicaDelovi = delovi[0].split(" ");
    var countUlica = ulicaDelovi.length - 2;
    var p = 0;
    var ulica = "";
    do {
        ulica += ulicaDelovi[p++] + " ";
    }
    while (countUlica >= p);
    var broj = ulicaDelovi[ulicaDelovi.length - 1];

    var gradPozivni = delovi[1].split(" ");
    var g = 0;
    var gradCount = gradPozivni.length - 2;
    var grad = "";
    do {
        grad += gradPozivni[g++] + " ";
    } while (gradCount >= g);
    var pozivni = gradPozivni[gradPozivni.length - 1];
    var drzava = delovi[2];
    var fulAdresa = location.lat() + "," + location.lng() + "," + delovi[0] + "," + grad + "," + drzava;
    $("#ulica").val(ulica);
    $("#brojKuce").val(broj);
    $("#mesto").val(grad);
    $("#pozivniBroj").val(pozivni);
    $("#xKordinata").val(location.lat());
    $("#yKordinata").val(location.lng());
    var infowindow = new google.maps.InfoWindow({
        content: 'Ulica: ' + ulica + '</br>Broj: ' + broj + '</br>Mesto: ' + grad + '</br>Pozivni broj: ' + pozivni
    });
    infowindow.open(map, marker);
    markers.push(marker);
}

function myMap() {
    var mapCanvas = document.getElementById("map");
    var myCenter = new google.maps.LatLng(45.242630873254775, 19.842914435055945);
    var mapOptions = { center: myCenter, zoom: 15 };
    var map = new google.maps.Map(mapCanvas, mapOptions);
    google.maps.event.addListener(map, 'click', function (event) {
        $('#potvrdiLokaciju').attr('disabled', false);
        $('#kreirajVoznjuD').attr('disabled', false);
        $('#kreirajVoznju').attr('disabled', false);
        placeMarker(map, event.latLng);
    });
}