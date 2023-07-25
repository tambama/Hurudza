import { authHeader } from '../helpers/auth-header';
import { apiUrl, apiVersion } from '../constants/api';

export const paymentService = {
    payClass,
    pollStatus,
    walletPay
};

async function payClass(phone) {
    const requestOptions = {
        method: 'GET',
        headers: await authHeader()
    };

    return fetch(`${apiUrl}/api/payments?phone=${phone}`, requestOptions).then(handleResponse);
}

async function pollStatus(pollUrl) {
    const requestOptions = {
        method: 'GET',
        headers: await authHeader()
    };

    return fetch(`${apiUrl}/api/payments?pollUrl=${pollUrl}`, requestOptions).then(handleResponse);
}

async function walletPay(payment) {
    const requestOptions = {
        method: 'POST',
        headers: { ...(await authHeader()), 'Content-Type': 'application/json' },
        body: JSON.stringify(payment)
    };

    return fetch(`${apiUrl}/Airtime/Topup${apiVersion}`, requestOptions).then(handleResponse);
}

function handleResponse(response) {
    return response.text().then(text => {
        const data = text && JSON.parse(text);
        if (!response.ok) {
            console.log(data)
            const error = (data && (data.message || data.title)) || response.statusText;
            console.log(error)
            return Promise.reject(error);
        }
        
        return data.result;
    });
}