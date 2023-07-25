import { AsyncStorage } from 'react-native';
import { authHeader } from '../helpers/auth-header';

import { apiUrl } from '../constants/api'

export const couponService = {
    add,
    share,
    getAll,
    getById,
    getUserCoupons,
    getUserCouponsByFuelType,
    update,
    delete: _delete,
    createCouponMobilePayment
};

async function getAll() {
    const requestOptions = {
        method: 'GET',
        headers: await authHeader()
    };

    return fetch(`${apiUrl}/Coupons`, requestOptions).then(handleResponse);
}

async function getById(id) {
    const requestOptions = {
        method: 'GET',
        headers: await authHeader()
    };

    return fetch(`${apiUrl}/Coupons/${id}`, requestOptions).then(handleResponse);
}

async function getUserCoupons(id) {
    const requestOptions = {
        method: 'GET',
        headers: await authHeader()
    };

    return fetch(`${apiUrl}/Coupons/GetCouponsByOwner/${id}`, requestOptions).then(handleResponse);
}

async function getUserCouponsByFuelType(userId, productId) {
    const requestOptions = {
        method: 'GET',
        headers: await authHeader()
    };

    return fetch(`${apiUrl}/Coupons/GetUserCouponsByFuelType/${userId}/${productId}`, requestOptions).then(handleResponse);
}

async function add(_Coupon) {
    const requestOptions = {
        method: 'POST',
        headers: { ...(await authHeader()), 'Content-Type': 'application/json' },
        body: JSON.stringify(_Coupon)
    };

    return fetch(`${apiUrl}/Coupons/createCoupon`, requestOptions).then(handleResponse);
}

async function createCouponMobilePayment(_Coupon) {
    const requestOptions = {
        method: 'POST',
        headers: { ...(await authHeader()), 'Content-Type': 'application/json' },
        body: JSON.stringify(_Coupon)
    };

    return fetch(`${apiUrl}/Payments/createCouponMobilePayment`, requestOptions).then(handleResponse);
}

async function share(_Coupon) {
    const requestOptions = {
        method: 'POST',
        headers: { ...(await authHeader()), 'Content-Type': 'application/json' },
        body: JSON.stringify(_Coupon)
    };

    return fetch(`${apiUrl}/Coupons/ShareCoupon`, requestOptions).then(handleResponse);
}

async function update(_Coupon) {
    const requestOptions = {
        method: 'PUT',
        headers: { ...(await authHeader()), 'Content-Type': 'application/json' },
        body: JSON.stringify(_Coupon)
    };

    return fetch(`${apiUrl}/Coupons/${_Coupon.id}`, requestOptions).then(handleResponse);;
}

// prefixed function name with underscore because delete is a reserved word in javascript
async function _delete(id) {
    const requestOptions = {
        method: 'DELETE',
        headers: await authHeader()
    };

    return fetch(`${apiUrl}/Coupons/${id}`, requestOptions).then(handleResponse);
}

function handleResponse(response) {
    return response.text().then(text => {
        const data = text && JSON.parse(text);
        if (!response.ok) {
            const error = (data && (data.error || data.message || data.title)) || response.statusText;

            console.log(error);
            return Promise.reject(error);
        }
        return data.result;
    });
}