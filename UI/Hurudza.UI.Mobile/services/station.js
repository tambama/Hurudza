import { AsyncStorage } from 'react-native';
import { authHeader } from '../helpers/auth-header';

import { apiUrl } from '../constants/api'

export const stationService = {
    add,
    getAll,
    getNearestServiceStations,
    getById,
    getUserStation,
    update,
    delete: _delete,
    getFuelPriceByStationCode,
    getStationByCode,
    searchNearestServiceStations
};

async function getAll() {
    const requestOptions = {
        method: 'GET',
        headers: await authHeader()
    };

    return fetch(`${apiUrl}/ServiceStations/GetServiceStations`, requestOptions).then(handleResponse);
}

async function getNearestServiceStations(lat, long, howMany) {
    const requestOptions = {
        method: 'GET',
        headers: await authHeader()
    };

    return fetch(`${apiUrl}/ServiceStations/GetNearestServiceStations/${lat}/${long}/${howMany}`, requestOptions).then(handleResponse);
}

async function searchNearestServiceStations(lat, long, search, howMany) {
    const requestOptions = {
        method: 'GET',
        headers: await authHeader()
    };

    return fetch(`${apiUrl}/ServiceStations/SearchNearestServiceStations/${lat}/${long}/${search}/${howMany}`, requestOptions).then(handleResponse);
}

async function getById(id) {
    const requestOptions = {
        method: 'GET',
        headers: await authHeader()
    };

    return fetch(`${apiUrl}/Station/${id}`, requestOptions).then(handleResponse);
}

async function getUserStation(id) {
    const requestOptions = {
        method: 'GET',
        headers: await authHeader()
    };

    return fetch(`${apiUrl}/Station/getUserStation/${id}`, requestOptions).then(handleResponse);
}

async function getStationByCode(stationCode) {
    const requestOptions = {
        method: 'GET',
        headers: await authHeader()
    };

    return fetch(`${apiUrl}/Companies/getStationByCode/${stationCode}`, requestOptions).then(handleResponse);
}

async function getFuelPriceByStationCode(currencyId, productId, stationCode){
    const requestOptions = {
        method: 'GET',
        headers: await authHeader()
    };

    return fetch(`${apiUrl}/fuelPrices/getFuelPriceByStationCode/${currencyId}/${productId}/${stationCode}`, requestOptions).then(handleResponse);
}

async function add(_Wallet) {
    const requestOptions = {
        method: 'POST',
        headers: { ...(await authHeader()), 'Content-Type': 'application/json' },
        body: JSON.stringify(_Wallet)
    };

    return fetch(`${apiUrl}/Station/create`, requestOptions).then(handleResponse);
}

async function update(_Wallet) {
    const requestOptions = {
        method: 'PUT',
        headers: { ...(await authHeader()), 'Content-Type': 'application/json' },
        body: JSON.stringify(_Wallet)
    };

    return fetch(`${apiUrl}/Station/${_Wallet.id}`, requestOptions).then(handleResponse);;
}

// prefixed function name with underscore because delete is a reserved word in javascript
async function _delete(id) {
    const requestOptions = {
        method: 'DELETE',
        headers: await authHeader()
    };

    return fetch(`${apiUrl}/Station/${id}`, requestOptions).then(handleResponse);
}

function handleResponse(response) {
    return response.text().then(text => {
        const data = text && JSON.parse(text);
        if (!response.ok) {
            const error = (data && (data.error || data.message || data.title)) || response.statusText;
            return Promise.reject(error);
        }
        
        return data.result;
    });
}