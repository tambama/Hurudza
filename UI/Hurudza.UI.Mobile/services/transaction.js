import { authHeader } from '../helpers/auth-header';

import { apiUrl, apiVersion } from '../constants/api'

export const transactionService = {
    getUserTransactions,
    getTransactionsByWallet
};

async function getUserTransactions(model) {
    const requestOptions = {
        method: 'POST',
        headers: { ...(await authHeader()), 'Content-Type': 'application/json' },
        body: JSON.stringify(model)
    };

    return fetch(`${apiUrl}/Reports/FilterMobileTransactions${apiVersion}`, requestOptions).then(handleResponse);
}

async function getTransactionsByWallet(walletCode) {
    const requestOptions = {
        method: 'GET',
        headers: await authHeader()
    };

    return fetch(`${apiUrl}/WalletTransactions/GetWalletTransactions/${walletCode}${apiVersion}`, requestOptions).then(handleBasicResponse);
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

function handleBasicResponse(response) {
    return response.text().then(text => {
        const data = text && JSON.parse(text);
        if (!response.ok) {
            const error = (data && (data.error || data.message || data.title)) || response.statusText;
            return Promise.reject(error);
        } 

        return data;
    });
}