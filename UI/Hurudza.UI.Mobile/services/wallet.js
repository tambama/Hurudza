import { authHeader } from '../helpers/auth-header';

import { apiUrl, apiVersion } from '../constants/api';

export const walletService = {
    add,
    getAll,
    getById,
    getUserWallets,
    getUserWalletsByFuelType,
    update,
    topup,
    topupWalletMobilePayment,
    delete: _delete,
    walletPay,
    getWalletByCode,
    walletToWalletTransfer
};

async function getAll() {
    const requestOptions = {
        method: 'GET',
        headers: await authHeader()
    };

    return fetch(`${apiUrl}/Wallets${apiVersion}`, requestOptions).then(handleResponse);
}

async function getById(id) {
    const requestOptions = {
        method: 'GET',
        headers: await authHeader()
    };

    return fetch(`${apiUrl}/Wallets/GetWallet/${id}`, requestOptions).then(handleResponse);
}

async function getWalletByCode(code) {
    const requestOptions = {
        method: 'GET',
        headers: await authHeader()
    };

    return fetch(`${apiUrl}/Wallets/GetWalletByCode/${code}${apiVersion}`, requestOptions).then(handleResponse);
}

async function getUserWallets(id) {
    const requestOptions = {
        method: 'GET',
        headers: await authHeader()
    };
    
    return fetch(`${apiUrl}/Wallets/getCompanyWallets/${id}${apiVersion}`, requestOptions).then(handleResponse);
}

async function getUserWalletsByFuelType(userId, productId) {
    const requestOptions = {
        method: 'GET',
        headers: await authHeader()
    };

    return fetch(`${apiUrl}/Wallets/getUserWalletsByFuelType/${userId}/${productId}`, requestOptions).then(handleResponse);
}

async function add(_Wallet) {
    const requestOptions = {
        method: 'POST',
        headers: { ...(await authHeader()), 'Content-Type': 'application/json' },
        body: JSON.stringify(_Wallet)
    };

    return fetch(`${apiUrl}/Wallets/createwallet`, requestOptions).then(handleResponse);
}

async function update(_Wallet) {
    const requestOptions = {
        method: 'PUT',
        headers: { ...(await authHeader()), 'Content-Type': 'application/json' },
        body: JSON.stringify(_Wallet)
    };

    return fetch(`${apiUrl}/Wallets/${_Wallet.id}`, requestOptions).then(handleResponse);;
}

async function topup(_Wallet) {
    const requestOptions = {
        method: 'PUT',
        headers: { ...(await authHeader()), 'Content-Type': 'application/json' },
        body: JSON.stringify(_Wallet)
    };

    return fetch(`${apiUrl}/Wallets/TopupWallet/`, requestOptions).then(handleResponse);;
}

async function walletPay(payment) {
    const requestOptions = {
        method: 'POST',
        headers: { ...(await authHeader()), 'Content-Type': 'application/json' },
        body: JSON.stringify(payment)
    };

    return fetch(`${apiUrl}/Airtime/Topup${apiVersion}`, requestOptions).then(handleResponse);;
}

async function walletToWalletTransfer(transfer) {
    const requestOptions = {
        method: 'POST',
        headers: { ...(await authHeader()), 'Content-Type': 'application/json' },
        body: JSON.stringify(transfer)
    };

    return fetch(`${apiUrl}/Wallets/Transfer${apiVersion}`, requestOptions).then(handlePostResponse);;
}

async function topupWalletMobilePayment(_Wallet) {
    const requestOptions = {
        method: 'POST',
        headers: { ...(await authHeader()), 'Content-Type': 'application/json' },
        body: JSON.stringify(_Wallet)
    };

    return fetch(`${apiUrl}/Payments/TopupWalletMobilePayment/`, requestOptions).then(handleResponse);;
}

// prefixed function name with underscore because delete is a reserved word in javascript
async function _delete(id) {
    const requestOptions = {
        method: 'DELETE',
        headers: await authHeader()
    };

    return fetch(`${apiUrl}/Wallets/${id}`, requestOptions).then(handleResponse);
}

function handleResponse(response) {
    return response.text().then(text => {
        const data = text && JSON.parse(text);
        if (!response.ok) {
            const error = (data && (data.error || data.message || data.title)) || response.statusText;
            return Promise.reject(error);
        }

        return data;
    });
}

function handlePostResponse(response) {
    return response.text().then(text => {
        const data = text && JSON.parse(text);
        if (!response.ok) {
            const error = (data && (data.error || data.message || data.title)) || response.statusText;
            return Promise.reject(error);
        }

        return data.result;
    });
}