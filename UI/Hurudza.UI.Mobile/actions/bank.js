import { bankConstants } from '../constants/bank';
import { bankService } from '../services/bank';
import { alertActions } from './alert';

export const bankActions = {
    add,
    select: _select,
    getAll,
    getUserBanks,
    getBanksWithCouponPrices,
    delete: _delete,
    clear,
    updateBank
};

function _select(_bank) {
    return { type: bankConstants.SELECT_BANK, _bank };
}

function add(_bank) {
    return dispatch => {
        dispatch(request(_bank));

        bankService.add(_bank)
            .then(
                _bank => { 
                    dispatch(success(_bank));
                    dispatch(alertActions.success('Bank added successfully'));
                },
                error => {
                    dispatch(failure(error.toString()));
                    dispatch(alertActions.error(error.toString()));
                }
            );
    };

    function request(_bank) { return { type: bankConstants.ADD_BANK_REQUEST, _bank } }
    function success(_bank) { return { type: bankConstants.ADD_BANK_SUCCESS, _bank } }
    function failure(error) { return { type: bankConstants.ADD_BANK_FAILURE, error } }
}

function updateBank(_bank) {
    return dispatch => {
        dispatch(request(_bank));

        bankService.update(_bank)
            .then(
                _bank => {
                    dispatch(success(_bank));
                    dispatch(alertActions.success('Bank updated successfully'));
                },
                error => {
                    dispatch(failure(error.toString()));
                    dispatch(alertActions.error(error.toString()));
                }
            );
    };

    function request(_bank) { return { type: bankConstants.UPDATE_BANK_REQUEST, _bank } }
    function success(_bank) { return { type: bankConstants.UPDATE_BANK_SUCCESS, _bank } }
    function failure(error) { return { type: bankConstants.UPDATE_BANK_FAILURE, error } }
}

function getAll() {
    return dispatch => {
        dispatch(request());

        bankService.getAll()
            .then(
                banks => dispatch(success(banks)),
                error => dispatch(failure(error.toString()))
            );
    };

    function request() { return { type: bankConstants.GET_ALL_BANKS_REQUEST } }
    function success(banks) { return { type: bankConstants.GET_ALL_BANKS_SUCCESS, banks } }
    function failure(error) { return { type: bankConstants.GET_ALL_BANKS_FAILURE, error } }
}

function getBanksWithCouponPrices() {
    return dispatch => {
        dispatch(request());

        bankService.getBanksWithCouponPrices()
            .then(
                banks => dispatch(success(banks)),
                error => dispatch(failure(error.toString()))
            );
    };

    function request() { return { type: bankConstants.GET_BANKS_WITH_COUPON_PRICES_REQUEST } }
    function success(banks) { return { type: bankConstants.GET_BANKS_WITH_COUPON_PRICES_SUCCESS, banks } }
    function failure(error) { return { type: bankConstants.GET_BANKS_WITH_COUPON_PRICES_FAILURE, error } }
}

function getUserBanks(id) {
    return dispatch => {
        dispatch(request());

        bankService.getUserBanks(id)
            .then(
                banks => dispatch(success(banks)),
                error => dispatch(failure(error.toString()))
            );
    };

    function request() { return { type: bankConstants.GET_USER_BANKS_REQUEST } }
    function success(banks) { return { type: bankConstants.GET_USER_BANKS_SUCCESS, banks } }
    function failure(error) { return { type: bankConstants.GET_USER_BANKS_FAILURE, error } }
}

// prefixed function name with underscore because delete is a reserved word in javascript
function _delete(id) {
    return dispatch => {
        dispatch(request(id));

        bankService.delete(id)
            .then(
                () => dispatch(success(id)),
                error => dispatch(failure(id, error.toString()))
            );
    };

    function request(id) { return { type: bankConstants.DELETE_BANK_REQUEST, id } }
    function success(id) { return { type: bankConstants.DELETE_BANK_SUCCESS, id } }
    function failure(id, error) { return { type: bankConstants.DELETE_BANK_FAILURE, id, error } }
}

function clear() {
    return { type: bankConstants.CLEAR };
}