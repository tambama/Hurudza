import { AsyncStorage } from 'react-native';
import { authHeader } from '../helpers/auth-header';

import { apiUrl } from '../constants/api'

export const requestService = {
    add,
    getAll,
    getById,
    getUserRequests,
    getUnreadUserRequests,
    grantCouponRequest,
    grantWalletRequest,
    update,
    delete: _delete,
};

async function getAll() {
    const requestOptions = {
        method: 'GET',
        headers: await authHeader()
    };

    return fetch(`${apiUrl}/Requests`, requestOptions).then(handleResponse);
}

async function getById(id) {
    const requestOptions = {
        method: 'GET',
        headers: await authHeader()
    };

    return fetch(`${apiUrl}/Requests/${id}`, requestOptions).then(handleResponse);
}

async function getUserRequests(id) {
    const requestOptions = {
        method: 'GET',
        headers: await authHeader()
    };

    return fetch(`${apiUrl}/Requests/GetUserRequests/${id}`, requestOptions).then(handleResponse);
}

async function getUnreadUserRequests(id) {
    const requestOptions = {
        method: 'GET',
        headers: await authHeader()
    };

    return fetch(`${apiUrl}/Requests/GetUnreadUserRequests/${id}`, requestOptions).then(handleResponse);
}

async function add(_Request) { 
    const requestOptions = {
        method: 'POST',
        headers: { ...(await authHeader()), 'Content-Type': 'application/json' },
        body: JSON.stringify(_Request)
    };

    return fetch(`${apiUrl}/Requests/createRequest`, requestOptions).then(handleResponse);
}

async function update(_Request) {
    const requestOptions = {
        method: 'PUT',
        headers: { ...(await authHeader()), 'Content-Type': 'application/json' },
        body: JSON.stringify(_Request)
    };

    return fetch(`${apiUrl}/Requests/UpdateRequest`, requestOptions).then(handleResponse);;
}

async function grantWalletRequest(_Request) {
    const requestOptions = {
        method: 'PUT',
        headers: { ...(await authHeader()), 'Content-Type': 'application/json' },
        body: JSON.stringify(_Request)
    };

    return fetch(`${apiUrl}/Requests/GrantWalletRequest`, requestOptions).then(handleResponse);;
}

async function grantCouponRequest(_Request) {
    const requestOptions = {
        method: 'PUT',
        headers: { ...(await authHeader()), 'Content-Type': 'application/json' },
        body: JSON.stringify(_Request)
    };

    return fetch(`${apiUrl}/Requests/GrantCouponRequest`, requestOptions).then(handleResponse);;
}

// prefixed function name with underscore because delete is a reserved word in javascript
async function _delete(id) {
    const requestOptions = {
        method: 'DELETE',
        headers: await authHeader()
    };

    return fetch(`${apiUrl}/Requests/DeleteRequest/${id}`, requestOptions).then(handleResponse);
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