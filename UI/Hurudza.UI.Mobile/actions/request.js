import { requestConstants } from '../constants/request';
import { requestService } from '../services/request';
import { alertActions } from './alert';

export const requestActions = {
    add,
    select: _select,
    getAll,
    getUserRequests,
    getUnreadUserRequests,
    grantCouponRequest,
    grantWalletRequest,
    delete: _delete,
    clear,
    updateRequest
};

function _select(_request) {
    return dispatch => {
        dispatch(request(_request));
        requestService.update(_request)
            .then(
                _request => { 
                    dispatch(success(_request));
                },
                error => {
                    dispatch(failure(error.toString()));
                    dispatch(alertActions.error(error.toString()));
                }
            );
    };

    function request(_request) { return { type: requestConstants.SELECT_REQUEST_REQUEST, _request } }
    function success(_request) { return { type: requestConstants.SELECT_REQUEST_SUCCESS, _request } }
    function failure(error) { return { type: requestConstants.SELECT_REQUEST_FAILURE, error } }
}

function add(_request) {
    return dispatch => {
        dispatch(request(_request));
        requestService.add(_request)
            .then(
                _request => { 
                    dispatch(success(_request));
                    dispatch(alertActions.success('Fuel request sent successfully'));
                },
                error => {
                    dispatch(failure(error.toString()));
                    dispatch(alertActions.error(error.toString()));
                }
            );
    };

    function request(_request) { return { type: requestConstants.ADD_REQUEST_REQUEST, _request } }
    function success(_request) { return { type: requestConstants.ADD_REQUEST_SUCCESS, _request } }
    function failure(error) { return { type: requestConstants.ADD_REQUEST_FAILURE, error } }
}

function updateRequest(_request) {
    return dispatch => {
        dispatch(request(_request));

        requestService.update(_request)
            .then(
                _request => {
                    dispatch(success(_request));
                    dispatch(alertActions.success('Request updated successfully'));
                },
                error => {
                    dispatch(failure(error.toString()));
                    dispatch(alertActions.error(error.toString()));
                }
            );
    };

    function request(_request) { return { type: requestConstants.UPDATE_REQUEST_REQUEST, _request } }
    function success(_request) { return { type: requestConstants.UPDATE_REQUEST_SUCCESS, _request } }
    function failure(error) { return { type: requestConstants.UPDATE_REQUEST_FAILURE, error } }
}

function grantWalletRequest(_request) {
    return dispatch => {
        dispatch(request(_request));

        requestService.grantWalletRequest(_request)
            .then(
                _request => {
                    dispatch(success(_request));
                    dispatch(alertActions.success('Fuel sent succesfully'));
                },
                error => {
                    dispatch(failure(error.toString()));
                    dispatch(alertActions.error(error.toString()));
                }
            );
    };

    function request(_request) { return { type: requestConstants.UPDATE_REQUEST_REQUEST, _request } }
    function success(_request) { return { type: requestConstants.UPDATE_REQUEST_SUCCESS, _request } }
    function failure(error) { return { type: requestConstants.UPDATE_REQUEST_FAILURE, error } }
}

function grantCouponRequest(_request) {
    return dispatch => {
        dispatch(request(_request));

        requestService.grantCouponRequest(_request)
            .then(
                _request => {
                    dispatch(success(_request));
                    dispatch(alertActions.success('Fuel sent succesfully'));
                },
                error => {
                    dispatch(failure(error.toString()));
                    dispatch(alertActions.error(error.toString()));
                }
            );
    };

    function request(_request) { return { type: requestConstants.UPDATE_REQUEST_REQUEST, _request } }
    function success(_request) { return { type: requestConstants.UPDATE_REQUEST_SUCCESS, _request } }
    function failure(error) { return { type: requestConstants.UPDATE_REQUEST_FAILURE, error } }
}

function getAll() {
    return dispatch => {
        dispatch(request());

        requestService.getAll()
            .then(
                requests => dispatch(success(requests)),
                error => dispatch(failure(error.toString()))
            );
    };

    function request() { return { type: requestConstants.GET_ALL_REQUESTS_REQUEST } }
    function success(requests) { return { type: requestConstants.GET_ALL_REQUESTS_SUCCESS, requests } }
    function failure(error) { return { type: requestConstants.GET_ALL_REQUESTS_FAILURE, error } }
}

function getUserRequests(id) {
    return dispatch => {
        dispatch(request());

        requestService.getUserRequests(id)
            .then(
                requests => dispatch(success(requests)),
                error => dispatch(failure(error.toString()))
            );
    };

    function request() { return { type: requestConstants.GET_USER_REQUESTS_REQUEST } }
    function success(requests) { return { type: requestConstants.GET_USER_REQUESTS_SUCCESS, requests } }
    function failure(error) { return { type: requestConstants.GET_USER_REQUESTS_FAILURE, error } }
}

function getUnreadUserRequests(id) {
    return dispatch => {
        dispatch(request());

        requestService.getUnreadUserRequests(id)
            .then(
                unreadCount => dispatch(success(unreadCount)),
                error => dispatch(failure(error.toString()))
            );
    };

    function request() { return { type: requestConstants.GET_UNREAD_USER_REQUESTS_REQUEST } }
    function success(unreadCount) { return { type: requestConstants.GET_UNREAD_USER_REQUESTS_SUCCESS, unreadCount } }
    function failure(error) { return { type: requestConstants.GET_UNREAD_USER_REQUESTS_FAILURE, error } }
}

// prefixed function name with underscore because delete is a reserved word in javascript
function _delete(id) {
    return dispatch => {
        dispatch(request(id));

        requestService.delete(id)
            .then(
                () => dispatch(success(id)),
                error => dispatch(failure(id, error.toString()))
            );
    };

    function request(id) { return { type: requestConstants.DELETE_REQUEST_REQUEST, id } }
    function success(id) { return { type: requestConstants.DELETE_REQUEST_SUCCESS, id } }
    function failure(id, error) { return { type: requestConstants.DELETE_REQUEST_FAILURE, id, error } }
}

function clear() {
    return { type: requestConstants.CLEAR };
}