import { userConstants } from '../constants/user';
import { userService } from '../services/user';
import { alertActions } from './alert';

export const userActions = {
    login,
    logout,
    register,
    updateRegistrationDetails,
    createOTP,
    passwordResetRequest,
    resetPassword,
    forgotPassword,
    getOTP,
    getAll,
    delete: _delete,
    update,
    clear,
    getUserByUserName,

};

function login(username, password) {
    return dispatch => {
        dispatch(request({ username }));

        userService.login(username, password)
            .then(
                user => {
                    dispatch(success(user));
                },
                error => {
                    dispatch(failure(error.toString()));
                    dispatch(alertActions.error(error.toString()));
                }
            );
    };

    function request(user) { return { type: userConstants.LOGIN_REQUEST, user } }
    function success(user) { return { type: userConstants.LOGIN_SUCCESS, user } }
    function failure(error) { return { type: userConstants.LOGIN_FAILURE, error } }
}

function logout() {
    userService.logout();
    return { type: userConstants.LOGOUT };
}
function updateRegistrationDetails(user){
    return async dispatch => {
        dispatch({
            type: userConstants.UPDATE_REGISTRATION_SUCCESS,
            user:user
          })
    };
    function success(user) { return { type: userConstants.UPDATE_REGISTRATION_SUCCESS, user } }
  }
function register(user) {
    return dispatch => {
        dispatch(request(user));

        userService.register(user)
            .then(
                user => { 
                    dispatch(success(user));
                    dispatch(alertActions.success('Registration successful'));
                },
                error => {
                    dispatch(failure(error.toString()));
                    dispatch(alertActions.error(error.toString()));
                }
            );
    };

    function request(user) { return { type: userConstants.REGISTER_REQUEST, user } }
    function success(user) { return { type: userConstants.REGISTER_SUCCESS, user } }
    function failure(error) { return { type: userConstants.REGISTER_FAILURE, error } }
}

function update(user) {
    return dispatch => {
        dispatch(request(user));

        userService.update(user)
            .then(
                user => {
                    dispatch(success(user));
                    dispatch(alertActions.success('Update successful'));
                },
                error => {
                    dispatch(failure(error.toString()));
                    dispatch(alertActions.error(error.toString()));
                }
            );
    };

    function request(user) { return { type: userConstants.UPDATE_USER_REQUEST, user } }
    function success(user) { return { type: userConstants.UPDATE_USER_SUCCESS, user } }
    function failure(error) { return { type: userConstants.UPDATE_USER_FAILURE, error } }
}

function createOTP(user) {
    return dispatch => {
        dispatch(request(user));

        userService.createOTP(user)
            .then(
                user => { 
                    dispatch(success(user));
                    dispatch(alertActions.success('Confirmation code sent'));
                },
                error => {
                    dispatch(failure(error.toString()));
                    dispatch(alertActions.error(error.toString()));
                }
            );
    };

    function request(user) { return { type: userConstants.CREATE_OTP_REQUEST, user } }
    function success(otp) { return { type: userConstants.CREATE_OTP_SUCCESS, otp } }
    function failure(error) { return { type: userConstants.CREATE_OTP_FAILURE, error } }
}

function passwordResetRequest(user) {
    return dispatch => {
        dispatch(request(user));

        userService.passwordResetRequest(user)
            .then(
                otp => { 
                    dispatch(success(otp));
                    dispatch(alertActions.success('Confirmation code sent'));
                },
                error => {
                    dispatch(failure(error.toString()));
                    dispatch(alertActions.error(error.toString()));
                }
            );
    };

    function request(user) { return { type: userConstants.PASSWORD_RESET_REQUEST, user } }
    function success(otp) { return { type: userConstants.PASSWORD_RESET_SUCCESS, otp } }
    function failure(error) { return { type: userConstants.PASSWORD_RESET_FAILURE, error } }
}

function resetPassword(password) {
    return dispatch => {
        dispatch(request(password));

        userService.resetPassword(password)
            .then(
                changed => { 
                    dispatch(success(changed));
                    dispatch(alertActions.success('Password changed succesfully!'));
                },
                error => {
                    dispatch(failure(error.toString()));
                    dispatch(alertActions.error(error.toString()));
                }
            );
    };

    function request(password) { return { type: userConstants.RESET_PASSWORD_REQUEST, password } }
    function success(changed) { return { type: userConstants.RESET_PASSWORD_SUCCESS, changed } }
    function failure(error) { return { type: userConstants.RESET_PASSWORD_FAILURE, error } }
}

function forgotPassword(password) {
    return dispatch => {
        dispatch(request(password));

        userService.forgotPassword(password)
            .then(
                otp => { 
                    dispatch(success(otp));
                    dispatch(alertActions.success('Code sent succesfully!'));
                },
                error => {
                    dispatch(failure(error.toString()));
                    dispatch(alertActions.error(error.toString()));
                }
            );
    };

    function request() { return { type: userConstants.FORGOT_PASSWORD_REQUEST } }
    function success(otp) { return { type: userConstants.FORGOT_PASSWORD_SUCCESS, otp } }
    function failure(error) { return { type: userConstants.FORGOT_PASSWORD_FAILURE, error } }
}

function getOTP(phone, otp) {
    return dispatch => {
        dispatch(request(phone, otp));

        userService.getOTP(phone, otp)
            .then(
                otp => dispatch(success(otp)),
                error => dispatch(failure(error.toString()))
            );
    };

    function request(phone, otp) { return { type: userConstants.GETALL_REQUEST, phone, otp } }
    function success(otp) { return { type: userConstants.GETALL_SUCCESS, otp } }
    function failure(error) { return { type: userConstants.GETALL_FAILURE, error } }
}

function getAll() {
    return dispatch => {
        dispatch(request());

        userService.getAll()
            .then(
                users => dispatch(success(users)),
                error => dispatch(failure(error.toString()))
            );
    };

    function request() { return { type: userConstants.GETALL_REQUEST } }
    function success(users) { return { type: userConstants.GETALL_SUCCESS, users } }
    function failure(error) { return { type: userConstants.GETALL_FAILURE, error } }
}

function getUserByUserName(username) {
    return dispatch => {
        dispatch(request());

        userService.getUserByUserName(username)
            .then(
                user => dispatch(success(user)),
                error => dispatch(failure(error.toString()))
            );
    };

    function request() { return { type: userConstants.GET_USER_BY_USERNAME_REQUEST } }
    function success(user) { return { type: userConstants.GET_USER_BY_USERNAME_SUCCESS, user } }
    function failure(error) { return { type: userConstants.GET_USER_BY_USERNAME_FAILURE, error } }
}

// prefixed function name with underscore because delete is a reserved word in javascript
function _delete(id) {
    return dispatch => {
        dispatch(request(id));

        userService.delete(id)
            .then(
                user => dispatch(success(id)),
                error => dispatch(failure(id, error.toString()))
            );
    };

    function request(id) { return { type: userConstants.DELETE_REQUEST, id } }
    function success(id) { return { type: userConstants.DELETE_SUCCESS, id } }
    function failure(id, error) { return { type: userConstants.DELETE_FAILURE, id, error } }
}

function clear(user) {
    return { type: userConstants.CLEAR, user };
}

function clearOtp(){
    return { type: userConstants.CLEAR_OTP }
}