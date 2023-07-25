import { omcConstants } from '../constants/omc';
import { omcService } from '../services/omc';
import { alertActions } from './alert';

export const omcActions = {
    add,
    select: _select,
    getAll,
    getUserOmcs,
    getOmcsWithCouponPrices,
    getOmcFuelPrice,
    delete: _delete,
    clear,
    updateOmc
};

function _select(_omc) {
    return { type: omcConstants.SELECT_OMC, _omc };
}

function add(_omc) {
    return dispatch => {
        dispatch(request(_omc));

        omcService.add(_omc)
            .then(
                _omc => { 
                    dispatch(success(_omc));
                    dispatch(alertActions.success('Omc added successfully'));
                },
                error => {
                    dispatch(failure(error.toString()));
                    dispatch(alertActions.error(error.toString()));
                }
            );
    };

    function request(_omc) { return { type: omcConstants.ADD_OMC_REQUEST, _omc } }
    function success(_omc) { return { type: omcConstants.ADD_OMC_SUCCESS, _omc } }
    function failure(error) { return { type: omcConstants.ADD_OMC_FAILURE, error } }
}

function updateOmc(_omc) {
    return dispatch => {
        dispatch(request(_omc));

        omcService.update(_omc)
            .then(
                _omc => {
                    dispatch(success(_omc));
                    dispatch(alertActions.success('Omc updated successfully'));
                },
                error => {
                    dispatch(failure(error.toString()));
                    dispatch(alertActions.error(error.toString()));
                }
            );
    };

    function request(_omc) { return { type: omcConstants.UPDATE_OMC_REQUEST, _omc } }
    function success(_omc) { return { type: omcConstants.UPDATE_OMC_SUCCESS, _omc } }
    function failure(error) { return { type: omcConstants.UPDATE_OMC_FAILURE, error } }
}

function getAll() {
    return dispatch => {
        dispatch(request());

        omcService.getAll()
            .then(
                omcs => dispatch(success(omcs)),
                error => dispatch(failure(error.toString()))
            );
    };

    function request() { return { type: omcConstants.GET_ALL_OMCS_REQUEST } }
    function success(omcs) { return { type: omcConstants.GET_ALL_OMCS_SUCCESS, omcs } }
    function failure(error) { return { type: omcConstants.GET_ALL_OMCS_FAILURE, error } }
}

function getOmcsWithCouponPrices() {
    return dispatch => {
        dispatch(request());

        omcService.getCompaniesWithCouponPrices()
            .then(
                omcs => dispatch(success(omcs)),
                error => dispatch(failure(error.toString()))
            );
    };

    function request() { return { type: omcConstants.GET_OMCS_WITH_COUPON_PRICES_REQUEST } }
    function success(omcs) { return { type: omcConstants.GET_OMCS_WITH_COUPON_PRICES_SUCCESS, omcs } }
    function failure(error) { return { type: omcConstants.GET_OMCS_WITH_COUPON_PRICES_FAILURE, error } }
}

function getOmcFuelPrice(omcId, productId, currencyId) {
    return dispatch => {
        dispatch(request());

        omcService.getOmcFuelPrice(omcId, productId, currencyId)
            .then(
                price => dispatch(success(price)),
                error => dispatch(failure(error.toString()))
            );
    };

    function request() { return { type: omcConstants.GET_OMC_FUEL_PRICE_REQUEST } }
    function success(price) { return { type: omcConstants.GET_OMC_FUEL_PRICE_SUCCESS, price} }
    function failure(error) { return { type: omcConstants.GET_OMC_FUEL_PRICE_FAILURE, error } }
}

function getUserOmcs(id) {
    return dispatch => {
        dispatch(request());

        omcService.getUserCompanies(id)
            .then(
                omcs => dispatch(success(omcs)),
                error => dispatch(failure(error.toString()))
            );
    };

    function request() { return { type: omcConstants.GET_USER_OMCS_REQUEST } }
    function success(omcs) { return { type: omcConstants.GET_USER_OMCS_SUCCESS, omcs } }
    function failure(error) { return { type: omcConstants.GET_USER_OMCS_FAILURE, error } }
}

// prefixed function name with underscore because delete is a reserved word in javascript
function _delete(id) {
    return dispatch => {
        dispatch(request(id));

        omcService.delete(id)
            .then(
                () => dispatch(success(id)),
                error => dispatch(failure(id, error.toString()))
            );
    };

    function request(id) { return { type: omcConstants.DELETE_OMC_REQUEST, id } }
    function success(id) { return { type: omcConstants.DELETE_OMC_SUCCESS, id } }
    function failure(id, error) { return { type: omcConstants.DELETE_OMC_FAILURE, id, error } }
}

function clear() {
    return { type: omcConstants.CLEAR };
}