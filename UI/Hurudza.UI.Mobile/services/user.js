import AsyncStorage from '@react-native-async-storage/async-storage';
import { authHeader } from '../helpers/auth-header';

import { apiUrl, apiVersion } from '../constants/api'

export const userService = {
    login,
    logout,
    register,
    createOTP,
    passwordResetRequest,
    resetPassword,
    forgotPassword,
    getOTP,
    getAll,
    getById,
    update,
    delete: _delete,
    getUserByUserName
};

async function login(username, password) {
    const requestOptions = {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ username, password })
    };

    return fetch(`${apiUrl}/authentication/login${apiVersion}`, requestOptions)
        .then(handleResponse)
        .then(async user => {
            console.log(user);
            // login successful if there's a jwt token in the response
            if (user.token) {
                // store user details and jwt token in local storage to keep user logged in between page refreshes
                await AsyncStorage.setItem('user', JSON.stringify(user));
            }
            return user;
        });
}

function register(user) {
    const requestOptions = {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(user)
    };

    return fetch(`${apiUrl}/companies/createCompany${apiVersion}`, requestOptions).then(handleResponse);
}

async function logout() {
    // remove user from local storage to log user out
    await AsyncStorage.removeItem('user');
}

async function getAll() {
    const requestOptions = {
        method: 'GET',
        headers: await authHeader()
    };

    return fetch(`${apiUrl}/users`, requestOptions).then(handleResponse);
}

async function getById(id) {
    const requestOptions = {
        method: 'GET',
        headers: await authHeader()
    };

    return fetch(`${apiUrl}/users/${id}`, requestOptions).then(handleResponse);
}

async function getUserByUserName(username) {
    const requestOptions = {
        method: 'GET',
        headers: await authHeader()
    };

    return fetch(`${apiUrl}/usermanagement/getUserByUserName/${username}`, requestOptions).then(handleResponse);
}

function getOTP(phone, otp) {
    const requestOptions = {
        method: 'GET'
    };

    return fetch(`${apiUrl}/otps/getotp/${phone}/${otp}`, requestOptions).then(handleResponse);
}



function createOTP(user) {
    const requestOptions = {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(user)
    };
    
    return fetch(`${apiUrl}/otps/createotp`, requestOptions).then(handleResponse);
}

function passwordResetRequest(user) {
    const requestOptions = {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(user)
    };
    
    return fetch(`${apiUrl}/UserManagement/PasswordResetRequest`, requestOptions).then(handleResponse);
}

function resetPassword(password) {
    const requestOptions = {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(password)
    };
    
    return fetch(`${apiUrl}/UserManagement/PasswordReset`, requestOptions).then(handleResponse);
}

function forgotPassword(password) {
    const requestOptions = {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(password)
    };
    
    return fetch(`${apiUrl}/UserManagement/ForgotPassword`, requestOptions).then(handleResponse);
}

async function update(user) {
    let header = await authHeader();

    const requestOptions = {
        method: 'PUT',
        headers: { ...header, 'Content-Type': 'application/json' },
        body: JSON.stringify(user)
    };

    return fetch(`${apiUrl}/companies/updateCompany/${user.id}${apiVersion}`, requestOptions).then(handleResponse);;
}

// prefixed function name with underscore because delete is a reserved word in javascript
async function _delete(id) {
    const requestOptions = {
        method: 'DELETE',
        headers: await authHeader()
    };

    return fetch(`${apiUrl}/users/${id}`, requestOptions).then(handleResponse);
}

function handleResponse(response) {
    return response.text().then(text => {
        const data = text && JSON.parse(text);
        if (!response.ok) {

            console.log(data);

            if(data.status == 400){
                var errors = [
                    data.title
                ].concat(Object.values(data.errors).flat())

                return Promise.reject(errors);
            }
            
            const error = (data && (data.error || data.message)) || response.statusText || data.title || 'Contact Customer Care';

            return Promise.reject(error);
        }

        return data.result;
    });
}