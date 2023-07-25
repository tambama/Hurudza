import { locationConstants } from '../constants/location';
import { locationService } from '../services/location';
import { alertActions } from './alert';

export const locationActions = {
    login,
    add,
    select: _select,
    getAll,
    getUserLocations,
    delete: _delete,
    clear,
    pick: _pickLocation,
    getCurrentLocation
};

function login(locationname, password) {
    return dispatch => {
        dispatch(request({ locationname }));

        locationService.login(locationname, password)
            .then(
                location => { 
                    dispatch(success(location));
                },
                error => {
                    dispatch(failure(error.toString()));
                    dispatch(alertActions.error(error.toString()));
                }
            );
    };

    function request(location) { return { type: locationConstants.LOGIN_REQUEST, location } }
    function success(location) { return { type: locationConstants.LOGIN_SUCCESS, location } }
    function failure(error) { return { type: locationConstants.LOGIN_FAILURE, error } }
}

function _select(location) {
    return { type: locationConstants.SELECT_LOCATION, location };
}

function _pickLocation(location) {
    return { type: locationConstants.PICK_LOCATION, location };
}

function getCurrentLocation(location) {
    return { type: locationConstants.GET_CURRENT_LOCATION, location };
}

function add(location) {
    return dispatch => {
        dispatch(request(location));

        locationService.add(location)
            .then(
                location => { 
                    dispatch(success(location));
                    dispatch(alertActions.success('Location added successfully'));
                },
                error => {
                    dispatch(failure(error.toString()));
                    dispatch(alertActions.error(error.toString()));
                }
            );
    };

    function request(location) { return { type: locationConstants.ADD_REQUEST, location } }
    function success(location) { return { type: locationConstants.ADD_SUCCESS, location } }
    function failure(error) { return { type: locationConstants.ADD_FAILURE, error } }
}

function getAll() {
    return dispatch => {
        dispatch(request());

        locationService.getAll()
            .then(
                locations => dispatch(success(locations)),
                error => dispatch(failure(error.toString()))
            );
    };

    function request() { return { type: locationConstants.GETALL_REQUEST } }
    function success(locations) { return { type: locationConstants.GETALL_SUCCESS, locations } }
    function failure(error) { return { type: locationConstants.GETALL_FAILURE, error } }
}

function getUserLocations(id) {
    return dispatch => {
        dispatch(request());

        locationService.getUserLocations(id)
            .then(
                locations => dispatch(success(locations)),
                error => dispatch(failure(error.toString()))
            );
    };

    function request() { return { type: locationConstants.GETUSERLOCATION_REQUEST } }
    function success(locations) { return { type: locationConstants.GETUSERLOCATION_SUCCESS, locations } }
    function failure(error) { return { type: locationConstants.GETUSERLOCATION_FAILURE, error } }
}

// prefixed function name with underscore because delete is a reserved word in javascript
function _delete(id) {
    return dispatch => {
        dispatch(request(id));

        locationService.delete(id)
            .then(
                () => dispatch(success(id)),
                error => dispatch(failure(id, error.toString()))
            );
    };

    function request(id) { return { type: locationConstants.DELETE_REQUEST, id } }
    function success(id) { return { type: locationConstants.DELETE_SUCCESS, id } }
    function failure(id, error) { return { type: locationConstants.DELETE_FAILURE, id, error } }
}

function clear() {
    return { type: locationConstants.CLEAR };
}