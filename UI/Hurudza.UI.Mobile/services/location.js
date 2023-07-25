import { AsyncStorage } from 'react-native';
import { authHeader } from '../helpers/auth-header';

import { apiUrl } from '../constants/api'

export const locationService = {
    login,
    add,
    getAll,
    getById,
    getUserLocations,
    update,
    delete: _delete
};

function login(locationname, password) {
    const requestOptions = {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ locationname, password })
    };

    return fetch(`${apiUrl}/locations/authenticate`, requestOptions)
        .then(handleResponse)
        .then(location => {
            // login successful if there's a jwt token in the response
            if (location.token) {
                // store location details and jwt token in local storage to keep location logged in between page refreshes
                AsyncStorage.setItem('location', JSON.stringify(location));
            }

            return location;
        });
}

async function getAll() {
    const requestOptions = {
        method: 'GET',
        headers: await authHeader()
    };

    return fetch(`${apiUrl}/locations`, requestOptions).then(handleResponse);
}

async function getById(id) {
    const requestOptions = {
        method: 'GET',
        headers: await authHeader()
    };

    return fetch(`${apiUrl}/locations/${id}`, requestOptions).then(handleResponse);
}

async function getUserLocations(id) {
    const requestOptions = {
        method: 'GET',
        headers: await authHeader()
    };

    return fetch(`${apiUrl}/locations/getUserLocations/${id}`, requestOptions).then(handleResponse);
}

async function add(location) {
    const requestOptions = {
        method: 'POST',
        headers: { ...(await authHeader()), 'Content-Type': 'application/json' },
        body: JSON.stringify(location)
    };

    return fetch(`${apiUrl}/locations/create`, requestOptions).then(handleResponse);
}

async function update(location) {
    const requestOptions = {
        method: 'PUT',
        headers: { ...(await authHeader()), 'Content-Type': 'application/json' },
        body: JSON.stringify(location)
    };

    return fetch(`${apiUrl}/locations/${location.id}`, requestOptions).then(handleResponse);;
}

// prefixed function name with underscore because delete is a reserved word in javascript
async function _delete(id) {
    const requestOptions = {
        method: 'DELETE',
        headers: await authHeader()
    };

    return fetch(`${apiUrl}/locations/${id}`, requestOptions).then(handleResponse);
}

function handleResponse(response) {
    return response.text().then(text => {
        const data = text && JSON.parse(text);
        if (!response.ok) {
            const error = (data && data.message) || response.statusText;
            return Promise.reject(error);
        }

        return data.result
        ;
    });
}