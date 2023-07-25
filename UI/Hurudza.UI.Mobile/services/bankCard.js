import { AsyncStorage } from 'react-native';
import { authHeader } from '../helpers/auth-header';

import { apiUrl } from '../constants/api'

export const bankCardService = {
    add,
    getAll,
    getById,
    getUserBankCards,
    update,
    delete: _delete,
};

async function getAll() {
    const requestOptions = {
        method: 'GET',
        headers: await authHeader()
    };

    return fetch(`${apiUrl}/BankCards`, requestOptions).then(handleResponse);
}

async function getById(id) {
    const requestOptions = {
        method: 'GET',
        headers: await authHeader()
    };

    return fetch(`${apiUrl}/BankCards/${id}`, requestOptions).then(handleResponse);
}

async function getUserBankCards(id) {
    const requestOptions = {
        method: 'GET',
        headers: await authHeader()
    };

    return fetch(`${apiUrl}/Cards/getUserCards/${id}`, requestOptions).then(handleResponse);
}

async function add(_bankCard) {
    const requestOptions = {
        method: 'POST',
        headers: { ...(await authHeader()), 'Content-Type': 'application/json' },
        body: JSON.stringify(_bankCard)
    };

    return fetch(`${apiUrl}/Cards/createCard`, requestOptions).then(handleResponse);
}

async function update(_BankCard) {
    const requestOptions = {
        method: 'PUT',
        headers: { ...(await authHeader()), 'Content-Type': 'application/json' },
        body: JSON.stringify(_BankCard)
    };

    return fetch(`${apiUrl}/BankCards/${_BankCard.id}`, requestOptions).then(handleResponse);;
}

// prefixed function name with underscore because delete is a reserved word in javascript
async function _delete(id) {
    const requestOptions = {
        method: 'DELETE',
        headers: await authHeader()
    };

    return fetch(`${apiUrl}/BankCards/${id}`, requestOptions).then(handleResponse);
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