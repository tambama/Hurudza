import { authHeader } from '../helpers/auth-header';

import { apiUrl, utilitiesApiUrl, apiVersion } from '../constants/api';

export const zesaService = {
    getCustomerDetails,
    getToken,
    getPreviousTokens,
    buyZesaToken
};

async function getCustomerDetails(model) {
    const requestOptions = {
        method: 'POST',
        headers: { ...(await authHeader()), 'Content-Type': 'application/json' },
        body: JSON.stringify(model)
    };

    return fetch(`${utilitiesApiUrl}/ZesaAccounts/GetZesaCustomerDetails${apiVersion}`, requestOptions).then(handleResponse);
}

async function getToken(reference){
    const requestOptions = {
        method: 'GET',
        headers: { 'Content-Type': 'application/json' }
    };

    return fetch(`${utilitiesApiUrl}/Esolutions/PollZesaTransaction/${reference}${apiVersion}`, requestOptions).then(handleResponse);
}

async function getPreviousTokens(meter){
    const requestOptions = {
        method: 'GET',
        headers: { 'Content-Type': 'application/json' }
    };

    return fetch(`${baseUrl}/api/Payments/GetPreviousTokens/${meter}`, requestOptions).then(handleResponse);
}

async function buyZesaToken(model) {
    const requestOptions = {
        method: 'POST',
        headers: { ...(await authHeader()), 'Content-Type': 'application/json' },
        body: JSON.stringify(model)
    };

    return fetch(`${utilitiesApiUrl}/Esolutions/TopupZesa${apiVersion}`, requestOptions).then(handleResponse);;
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