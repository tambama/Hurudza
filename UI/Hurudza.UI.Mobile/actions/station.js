import { stationConstants } from '../constants/station';
import { stationService } from '../services/station';
import { alertActions } from './alert';

export const stationActions = {
    add,
    select: _select,
    getAll,
    getUserStations,
    getNearestServiceStations,
    delete: _delete,
    clear,
    updateStation,
    getFuelPriceByStationCode,
    getStationByCode,
    searchNearestServiceStations
};

function _select(_station) {
    return { type: stationConstants.SELECT_STATION, _station };
}

function add(_station) {
    return dispatch => {
        dispatch(request(_station));

        stationService.add(_station)
            .then(
                _station => { 
                    dispatch(success(_station));
                    dispatch(alertActions.success('Station added successfully'));
                },
                error => {
                    dispatch(failure(error.toString()));
                    dispatch(alertActions.error(error.toString()));
                }
            );
    };

    function request(_station) { return { type: stationConstants.ADD_STATION_REQUEST, _station } }
    function success(_station) { return { type: stationConstants.ADD_STATION_SUCCESS, _station } }
    function failure(error) { return { type: stationConstants.ADD_STATION_FAILURE, error } }
}

function updateStation(_station) {
    return dispatch => {
        dispatch(request(_station));

        stationService.update(_station)
            .then(
                _station => {
                    dispatch(success(_station));
                    dispatch(alertActions.success('Station updated successfully'));
                },
                error => {
                    dispatch(failure(error.toString()));
                    dispatch(alertActions.error(error.toString()));
                }
            );
    };

    function request(_station) { return { type: stationConstants.UPDATE_STATION_REQUEST, _station } }
    function success(_station) { return { type: stationConstants.UPDATE_STATION_SUCCESS, _station } }
    function failure(error) { return { type: stationConstants.UPDATE_STATION_FAILURE, error } }
}

function getAll() {
    return dispatch => {
        dispatch(request());

        stationService.getAll()
            .then(
                stations => dispatch(success(stations)),
                error => dispatch(failure(error.toString()))
            );
    };

    function request() { return { type: stationConstants.GET_ALL_STATIONS_REQUEST } }
    function success(stations) { return { type: stationConstants.GET_ALL_STATIONS_SUCCESS, stations } }
    function failure(error) { return { type: stationConstants.GET_ALL_STATIONS_FAILURE, error } }
}

function getNearestServiceStations(lat, long, howMany) {
    return dispatch => {
        dispatch(request());

        stationService.getNearestServiceStations(lat, long, howMany)
            .then(
                stations => dispatch(success(stations)),
                error => dispatch(failure(error.toString()))
            );
    };

    function request() { return { type: stationConstants.GET_NEAREST_SERVICE_STATIONS_REQUEST } }
    function success(stations) { return { type: stationConstants.GET_NEAREST_SERVICE_STATIONS_SUCCESS, stations } }
    function failure(error) { return { type: stationConstants.GET_NEAREST_SERVICE_STATIONS_FAILURE, error } }
}

function searchNearestServiceStations(lat, long, search, howMany) {
    return dispatch => {
        dispatch(request());

        stationService.searchNearestServiceStations(lat, long, search, howMany)
            .then(
                stations => dispatch(success(stations)),
                error => dispatch(failure(error.toString()))
            );
    };

    function request() { return { type: stationConstants.SEARCH_NEAREST_SERVICE_STATIONS_REQUEST } }
    function success(stations) { return { type: stationConstants.SEARCH_NEAREST_SERVICE_STATIONS_SUCCESS, stations } }
    function failure(error) { return { type: stationConstants.SEARCH_NEAREST_SERVICE_STATIONS_FAILURE, error } }
}

function getUserStations(id) {
    return dispatch => {
        dispatch(request());

        stationService.getUserStations(id)
            .then(
                stations => dispatch(success(stations)),
                error => dispatch(failure(error.toString()))
            );
    };

    function request() { return { type: stationConstants.GET_USER_STATIONS_REQUEST } }
    function success(stations) { return { type: stationConstants.GET_USER_STATIONS_SUCCESS, stations } }
    function failure(error) { return { type: stationConstants.GET_USER_STATIONS_FAILURE, error } }
}

function getStationByCode(stationCode) {
    return dispatch => {
        dispatch(request());

        stationService.getStationByCode(stationCode)
            .then(
                station => dispatch(success(station)),
                error => dispatch(failure(error.toString()))
            );
    };

    function request() { return { type: stationConstants.GET_STATION_BY_CODE_REQUEST } }
    function success(station) { return { type: stationConstants.GET_STATION_BY_CODE_SUCCESS, station } }
    function failure(error) { return { type: stationConstants.GET_STATION_BY_CODE_FAILURE, error } }
}

function getFuelPriceByStationCode(currencyId, productId, stationCode) {
    return dispatch => {
        dispatch(request());

        stationService.getFuelPriceByStationCode(currencyId, productId, stationCode)
            .then(
                fuelPrice => dispatch(success(fuelPrice)),
                error => dispatch(failure(error.toString()))
            );
    };

    function request() { return { type: stationConstants.GET_FUEL_PRICE_BY_STATION_CODE_REQUEST } }
    function success(fuelPrice) { return { type: stationConstants.GET_FUEL_PRICE_BY_STATION_CODE_SUCCESS, fuelPrice } }
    function failure(error) { return { type: stationConstants.GET_FUEL_PRICE_BY_STATION_CODE_FAILURE, error } }
}

// prefixed function name with underscore because delete is a reserved word in javascript
function _delete(id) {
    return dispatch => {
        dispatch(request(id));

        stationService.delete(id)
            .then(
                () => dispatch(success(id)),
                error => dispatch(failure(id, error.toString()))
            );
    };

    function request(id) { return { type: stationConstants.DELETE_STATION_REQUEST, id } }
    function success(id) { return { type: stationConstants.DELETE_STATION_SUCCESS, id } }
    function failure(id, error) { return { type: stationConstants.DELETE_STATION_FAILURE, id, error } }
}

function clear() {
    return { type: stationConstants.CLEAR };
}