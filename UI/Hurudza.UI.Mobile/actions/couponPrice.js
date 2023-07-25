import { couponPriceConstants } from '../constants/couponPrice';
import { couponPriceService } from '../services/couponPrice';
import { alertActions } from './alert';

export const couponPriceActions = {
    add,
    select: _select,
    getAll,
    getUserCouponPrices,
    getCouponPricesByOmc,
    delete: _delete,
    clear,
    updateCouponPrice
};

function _select(_couponPrice) {
    return { type: couponPriceConstants.SELECT_COUPONPRICE, _couponPrice };
}

function add(_couponPrice) {
    return dispatch => {
        dispatch(request(_couponPrice));

        couponPriceService.add(_couponPrice)
            .then(
                _couponPrice => { 
                    dispatch(success(_couponPrice));
                    dispatch(alertActions.success('CouponPrice added successfully'));
                },
                error => {
                    dispatch(failure(error.toString()));
                    dispatch(alertActions.error(error.toString()));
                }
            );
    };

    function request(_couponPrice) { return { type: couponPriceConstants.ADD_COUPONPRICE_REQUEST, _couponPrice } }
    function success(_couponPrice) { return { type: couponPriceConstants.ADD_COUPONPRICE_SUCCESS, _couponPrice } }
    function failure(error) { return { type: couponPriceConstants.ADD_COUPONPRICE_FAILURE, error } }
}

function updateCouponPrice(_couponPrice) {
    return dispatch => {
        dispatch(request(_couponPrice));

        couponPriceService.update(_couponPrice)
            .then(
                _couponPrice => {
                    dispatch(success(_couponPrice));
                    dispatch(alertActions.success('CouponPrice updated successfully'));
                },
                error => {
                    dispatch(failure(error.toString()));
                    dispatch(alertActions.error(error.toString()));
                }
            );
    };

    function request(_couponPrice) { return { type: couponPriceConstants.UPDATE_COUPONPRICE_REQUEST, _couponPrice } }
    function success(_couponPrice) { return { type: couponPriceConstants.UPDATE_COUPONPRICE_SUCCESS, _couponPrice } }
    function failure(error) { return { type: couponPriceConstants.UPDATE_COUPONPRICE_FAILURE, error } }
}

function getAll() {
    return dispatch => {
        dispatch(request());

        couponPriceService.getAll()
            .then(
                couponPrices => dispatch(success(couponPrices)),
                error => dispatch(failure(error.toString()))
            );
    };

    function request() { return { type: couponPriceConstants.GET_ALL_COUPONPRICES_REQUEST } }
    function success(couponPrices) { return { type: couponPriceConstants.GET_ALL_COUPONPRICES_SUCCESS, couponPrices } }
    function failure(error) { return { type: couponPriceConstants.GET_ALL_COUPONPRICES_FAILURE, error } }
}

function getUserCouponPrices(id) {
    return dispatch => {
        dispatch(request());

        couponPriceService.getUserCouponPrices(id)
            .then(
                couponPrices => dispatch(success(couponPrices)),
                error => dispatch(failure(error.toString()))
            );
    };

    function request() { return { type: couponPriceConstants.GET_USER_COUPONPRICES_REQUEST } }
    function success(couponPrices) { return { type: couponPriceConstants.GET_USER_COUPONPRICES_SUCCESS, couponPrices } }
    function failure(error) { return { type: couponPriceConstants.GET_USER_COUPONPRICES_FAILURE, error } }
}

function getCouponPricesByOmc(id) {
    return dispatch => {
        dispatch(request());

        couponPriceService.getCouponPricesByOmc(id)
            .then(
                couponPrices => dispatch(success(couponPrices)),
                error => dispatch(failure(error.toString()))
            );
    };

    function request() { return { type: couponPriceConstants.GET_OMC_COUPONPRICES_REQUEST } }
    function success(couponPrices) { return { type: couponPriceConstants.GET_OMC_COUPONPRICES_SUCCESS, couponPrices } }
    function failure(error) { return { type: couponPriceConstants.GET_OMC_COUPONPRICES_FAILURE, error } }
}

// prefixed function name with underscore because delete is a reserved word in javascript
function _delete(id) {
    return dispatch => {
        dispatch(request(id));

        couponPriceService.delete(id)
            .then(
                () => dispatch(success(id)),
                error => dispatch(failure(id, error.toString()))
            );
    };

    function request(id) { return { type: couponPriceConstants.DELETE_COUPONPRICE_REQUEST, id } }
    function success(id) { return { type: couponPriceConstants.DELETE_COUPONPRICE_SUCCESS, id } }
    function failure(id, error) { return { type: couponPriceConstants.DELETE_COUPONPRICE_FAILURE, id, error } }
}

function clear() {
    return { type: couponPriceConstants.CLEAR };
}