import { AsyncStorage } from 'react-native';
import { authHeader } from '../helpers/auth-header';

import { apiUrl } from '../constants/api'

export const currencyService = {
    add,
    getAll,
    getById,
    getUserCurrency,
    getOmcCouponCurrencies,
    update,
    delete: _delete,
};

async function getAll() {
    const requestOptions = {
        method: 'GET',
        headers: await authHeader()
    };

    return fetch(`${apiUrl}/Currencies/GetCurrencies`, requestOptions).then(handleResponse);
}

async function getById(id) {
    const requestOptions = {
        method: 'GET',
        headers: await authHeader()
    };

    return fetch(`${apiUrl}/Currencies/${id}`, requestOptions).then(handleResponse);
}

async function getUserCurrency(id) {
    const requestOptions = {
        method: 'GET',
        headers: await authHeader()
    };

    return fetch(`${apiUrl}/Currencies/getUserCurrency/${id}`, requestOptions).then(handleResponse);
}

async function getOmcCouponCurrencies(id) {
    const requestOptions = {
        method: 'GET',
        headers: await authHeader()
    };

    return fetch(`${apiUrl}/Currencies/getOmcCouponCurrencies/${id}`, requestOptions).then(handleResponse);
}

async function add(_Wallet) {
    const requestOptions = {
        method: 'POST',
        headers: { ...(await authHeader()), 'Content-Type': 'application/json' },
        body: JSON.stringify(_Wallet)
    };

    return fetch(`${apiUrl}/Currencies/create`, requestOptions).then(handleResponse);
}

async function update(_Wallet) {
    const requestOptions = {
        method: 'PUT',
        headers: { ...(await authHeader()), 'Content-Type': 'application/json' },
        body: JSON.stringify(_Wallet)
    };

    return fetch(`${apiUrl}/Currencies/${_Wallet.id}`, requestOptions).then(handleResponse);;
}

// prefixed function name with underscore because delete is a reserved word in javascript
async function _delete(id) {
    const requestOptions = {
        method: 'DELETE',
        headers: await authHeader()
    };

    return fetch(`${apiUrl}/Currencies/${id}`, requestOptions).then(handleResponse);
}

function handleResponse(response) {
    return response.text().then(text => {
        const data = text && JSON.parse(text);
        if (!response.ok) {
            const error = (data && (data.error || data.message || data.title)) || response.statusText;
            console.log(error)
            return Promise.reject(error);
        }
        
        return data.result;
    });
}