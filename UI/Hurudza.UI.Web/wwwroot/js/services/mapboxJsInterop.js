export function loadMap(center){
    mapboxgl.accessToken = 'pk.eyJ1IjoicGVuaWVsdCIsImEiOiJjbGt3Y2gxM3YweWtrM3FwbW9jaWNkMWVyIn0.cDAgTWNXN-TVJROjgoWQiw';
    console.log(message);
    const map = new mapboxgl.Map({
        container: 'map',
        style: 'mapbox://styles/mapbox/streets-v12',
        center: center,
        zoom: 9
    });
    
    return message;
}