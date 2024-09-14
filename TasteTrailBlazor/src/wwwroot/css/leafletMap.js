// var map;
// var marker;
// var geocoder = L.Control.Geocoder.nominatim();

// function initializeMap(dotNetHelper) {
//     map = L.map('map').setView([51.505, -0.09], 13);

//     L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
//         maxZoom: 19,
//     }).addTo(map);

//     geocoder = L.Control.geocoder({
//         defaultMarkGeocode: false
//     }).on('markgeocode', function (e) {
//         var latlng = e.geocode.center;
//         if (marker) {
//             map.removeLayer(marker);
//         }
//         marker = L.marker(latlng).addTo(map);
//         map.setView(latlng, 13);

//         // Логирование для отладки
//         console.log("Geocoded address:", e.geocode.name, latlng);

//         // Вызов метода C# для обновления адреса и координат
//         dotNetHelper.invokeMethodAsync('UpdateLocation1', e.geocode.name, latlng.lat, latlng.lng);
//     }).addTo(map);

//     map.on('click', async function (e) {
//         if (marker) {
//             map.removeLayer(marker);
//         }
//         marker = L.marker(e.latlng).addTo(map);

//         // Получение адреса по координатам с помощью OpenStreetMap API
//         var response = await fetch(`https://nominatim.openstreetmap.org/reverse?format=json&lat=${e.latlng.lat}&lon=${e.latlng.lng}`);
//         var data = await response.json();

//         var address = data.display_name;

//         // Логирование для отладки
//         console.log("Clicked location:", e.latlng);
//         console.log("Clicked location:", typeof(e.latlng));
//         console.log("Clicked location:", e.latlng.lat);
//         console.log("Clicked location:", typeof(e.latlng.lat));
//         console.log("Clicked location:", e.latlng.lng);
//         console.log("Clicked location:", typeof(e.latlng.lng));
//         console.log("Reverse geocoded address:", address);

//         // Вызов метода C# для обновления адреса и координат
//         dotNetHelper.invokeMethodAsync('UpdateLocation1', address, e.latlng.lat, e.latlng.lng);
//     });
// }


// function searchAddress(address) {
//     geocoder.geocode(address, function(results) {
//         if (results.length > 0) {
//             var latlng = results[0].center;
//             map.setView(latlng, 13);
//             L.marker(latlng).addTo(map)
//               .bindPopup(results[0].name)
//               .openPopup();
//         }
//     });
// }
