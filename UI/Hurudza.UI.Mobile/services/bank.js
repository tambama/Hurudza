import { AsyncStorage } from 'react-native';
import { authHeader } from '../helpers/auth-header';

import { apiUrl } from '../constants/api'

export const bankService = {
    add,
    getAll,
    getById,
    getUserBank,
    getBanksWithCouponPrices,
    update,
    delete: _delete,
};

async function getAll() {
    const requestOptions = {
        method: 'GET',
        headers: await authHeader()
    };

    return fetch(`${apiUrl}/Banks/GetBanks`, requestOptions).then(handleResponse);
}

async function getBanksWithCouponPrices() {
    const requestOptions = {
        method: 'GET',
        headers: await authHeader()
    };

    return fetch(`${apiUrl}/Banks/GetBanksWithCouponPrices`, requestOptions).then(handleResponse);
}

async function getById(id) {
    const requestOptions = {
        method: 'GET',
        headers: await authHeader()
    };

    return fetch(`${apiUrl}/Banks/${id}`, requestOptions).then(handleResponse);
}

async function getUserBank(id) {
    const requestOptions = {
        method: 'GET',
        headers: await authHeader()
    };

    return fetch(`${apiUrl}/Banks/getUserBank/${id}`, requestOptions).then(handleResponse);
}

async function add(_Wallet) {
    const requestOptions = {
        method: 'POST',
        headers: { ...(await authHeader()), 'Content-Type': 'application/json' },
        body: JSON.stringify(_Wallet)
    };

    return fetch(`${apiUrl}/Banks/create`, requestOptions).then(handleResponse);
}

async function update(_Wallet) {
    const requestOptions = {
        method: 'PUT',
        headers: { ...(await authHeader()), 'Content-Type': 'application/json' },
        body: JSON.stringify(_Wallet)
    };

    return fetch(`${apiUrl}/Banks/${_Wallet.id}`, requestOptions).then(handleResponse);;
}

// prefixed function name with underscore because delete is a reserved word in javascript
async function _delete(id) {
    const requestOptions = {
        method: 'DELETE',
        headers: await authHeader()
    };

    return fetch(`${apiUrl}/Banks/${id}`, requestOptions).then(handleResponse);
}

function handleResponse(response) {
    return response.text().then(text => {
        const data = text && JSON.parse(text);
        if (!response.ok) {
            const error = (data && data.message) || response.statusText;
            return Promise.reject(error);
        }
        
        return data.result;
    });
}