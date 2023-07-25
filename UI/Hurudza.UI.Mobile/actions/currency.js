import { currencyConstants } from '../constants/currency';
import { currencyService } from '../services/currency';
import { alertActions } from './alert';

export const currencyActions = {
    add,
    select: _select,
    getAll,
    getUserCurrencies,
    getOmcCouponCurrencies,
    delete: _delete,
    clear,
    updateCurrency
};

function _select(_currency) {
    return { type: currencyConstants.SELECT_CURRENCY, _currency };
}

function add(_currency) {
    return dispatch => {
        dispatch(request(_currency));

        currencyService.add(_currency)
            .then(
                _currency => { 
                    dispatch(success(_currency));
                    dispatch(alertActions.success('Currency added successfully'));
                },
                error => {
                    dispatch(failure(error.toString()));
                    dispatch(alertActions.error(error.toString()));
                }
            );
    };

    function request(_currency) { return { type: currencyConstants.ADD_CURRENCY_REQUEST, _currency } }
    function success(_currency) { return { type: currencyConstants.ADD_CURRENCY_SUCCESS, _currency } }
    function failure(error) { return { type: currencyConstants.ADD_CURRENCY_FAILURE, error } }
}

function updateCurrency(_currency) {
    return dispatch => {
        dispatch(request(_currency));

        currencyService.update(_currency)
            .then(
                _currency => {
                    dispatch(success(_currency));
                    dispatch(alertActions.success('Currency updated successfully'));
                },
                error => {
                    dispatch(failure(error.toString()));
                    dispatch(alertActions.error(error.toString()));
                }
            );
    };

    function request(_currency) { return { type: currencyConstants.UPDATE_CURRENCY_REQUEST, _currency } }
    function success(_currency) { return { type: currencyConstants.UPDATE_CURRENCY_SUCCESS, _currency } }
    function failure(error) { return { type: currencyConstants.UPDATE_CURRENCY_FAILURE, error } }
}

function getAll() {
    return dispatch => {
        dispatch(request());

        currencyService.getAll()
            .then(
                currencies => dispatch(success(currencies)),
                error => dispatch(failure(error.toString()))
            );
    };

    function request() { return { type: currencyConstants.GET_ALL_CURRENCIES_REQUEST } }
    function success(currencies) { return { type: currencyConstants.GET_ALL_CURRENCIES_SUCCESS, currencies } }
    function failure(error) { return { type: currencyConstants.GET_ALL_CURRENCIES_FAILURE, error } }
}

function getUserCurrencies(id) {
    return dispatch => {
        dispatch(request());

        currencyService.getUserCurrencies(id)
            .then(
                currencies => dispatch(success(currencies)),
                error => dispatch(failure(error.toString()))
            );
    };

    function request() { return { type: currencyConstants.GET_USER_CURRENCIES_REQUEST } }
    function success(currencies) { return { type: currencyConstants.GET_USER_CURRENCIES_SUCCESS, currencies } }
    function failure(error) { return { type: currencyConstants.GET_USER_CURRENCIES_FAILURE, error } }
}

function getOmcCouponCurrencies(id) {
    return dispatch => {
        dispatch(request());

        currencyService.getOmcCouponCurrencies(id)
            .then(
                currencies => dispatch(success(currencies)),
                error => dispatch(failure(error.toString()))
            );
    };

    function request() { return { type: currencyConstants.GET_OMC_COUPON_CURRENCIES_REQUEST } }
    function success(currencies) { return { type: currencyConstants.GET_OMC_COUPON_CURRENCIES_SUCCESS, currencies } }
    function failure(error) { return { type: currencyConstants.GET_OMC_COUPON_CURRENCIES_FAILURE, error } }
}

// prefixed function name with underscore because delete is a reserved word in javascript
function _delete(id) {
    return dispatch => {
        dispatch(request(id));

        currencyService.delete(id)
            .then(
                () => dispatch(success(id)),
                error => dispatch(failure(id, error.toString()))
            );
    };

    function request(id) { return { type: currencyConstants.DELETE_CURRENCY_REQUEST, id } }
    function success(id) { return { type: currencyConstants.DELETE_CURRENCY_SUCCESS, id } }
    function failure(id, error) { return { type: currencyConstants.DELETE_CURRENCY_FAILURE, id, error } }
}

function clear() {
    return { type: currencyConstants.CLEAR };
}