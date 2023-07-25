import { couponConstants } from '../constants/coupon';
import { couponService } from '../services/coupon';
import { alertActions } from './alert';

export const couponActions = {
    add,
    shareCoupon,
    select: _select,
    getAll,
    getUserCoupons,
    getUserCouponsByFuelType,
    delete: _delete,
    clear,
    updateCoupon,
    createCouponMobilePayment
};

function _select(_coupon) {
    return { type: couponConstants.SELECT_COUPON, _coupon };
}

function add(_coupon) {
    return dispatch => {
        dispatch(request(_coupon));
        couponService.add(_coupon)
            .then(
                _coupon => { 
                    dispatch(success(_coupon));
                    dispatch(alertActions.success('Coupon added successfully'));
                },
                error => {
                    dispatch(failure(error.toString()));
                    dispatch(alertActions.error(error.toString()));
                }
            );
    };

    function request(_coupon) { return { type: couponConstants.ADD_COUPON_REQUEST, _coupon } }
    function success(_coupon) { return { type: couponConstants.ADD_COUPON_SUCCESS, _coupon } }
    function failure(error) { return { type: couponConstants.ADD_COUPON_FAILURE, error } }
}

function createCouponMobilePayment(_coupon) {
    return dispatch => {
        dispatch(request(_coupon));
        couponService.createCouponMobilePayment(_coupon)
            .then(
                _coupon => { 
                    dispatch(success(_coupon));
                    dispatch(alertActions.success('Coupon added successfully'));
                },
                error => {
                    dispatch(failure(error.toString()));
                    dispatch(alertActions.error(error.toString()));
                }
            );
    };

    function request(_coupon) { return { type: couponConstants.CREATE_COUPON_MOBILE_PAYMENT_REQUEST, _coupon } }
    function success(_coupon) { return { type: couponConstants.CREATE_COUPON_MOBILE_PAYMENT_SUCCESS, _coupon } }
    function failure(error) { return { type: couponConstants.CREATE_COUPON_MOBILE_PAYMENT_FAILURE, error } }
}

function updateCoupon(_coupon) {
    return dispatch => {
        dispatch(request(_coupon));

        couponService.update(_coupon)
            .then(
                _coupon => {
                    dispatch(success(_coupon));
                    dispatch(alertActions.success('Coupon updated successfully'));
                },
                error => {
                    dispatch(failure(error.toString()));
                    dispatch(alertActions.error(error.toString()));
                }
            );
    };

    function request(_coupon) { return { type: couponConstants.UPDATE_COUPON_REQUEST, _coupon } }
    function success(_coupon) { return { type: couponConstants.UPDATE_COUPON_SUCCESS, _coupon } }
    function failure(error) { return { type: couponConstants.UPDATE_COUPON_FAILURE, error } }
}

function shareCoupon(_coupon) {
    return dispatch => {
        dispatch(request(_coupon));

        couponService.share(_coupon)
            .then(
                _coupon => {
                    dispatch(success(_coupon));
                    dispatch(alertActions.success('Coupon shared successfully'));
                },
                error => {
                    dispatch(failure(error.toString()));
                    dispatch(alertActions.error(error.toString()));
                }
            );
    };

    function request(_coupon) { return { type: couponConstants.UPDATE_COUPON_REQUEST, _coupon } }
    function success(_coupon) { return { type: couponConstants.UPDATE_COUPON_SUCCESS, _coupon } }
    function failure(error) { return { type: couponConstants.UPDATE_COUPON_FAILURE, error } }
}

function getAll() {
    return dispatch => {
        dispatch(request());

        couponService.getAll()
            .then(
                coupons => dispatch(success(coupons)),
                error => dispatch(failure(error.toString()))
            );
    };

    function request() { return { type: couponConstants.GET_ALL_COUPONS_REQUEST } }
    function success(coupons) { return { type: couponConstants.GET_ALL_COUPONS_SUCCESS, coupons } }
    function failure(error) { return { type: couponConstants.GET_ALL_COUPONS_FAILURE, error } }
}

function getUserCoupons(id) {
    return dispatch => {
        dispatch(request());

        couponService.getUserCoupons(id)
            .then(
                coupons => dispatch(success(coupons)),
                error => dispatch(failure(error.toString()))
            );
    };

    function request() { return { type: couponConstants.GET_USER_COUPONS_REQUEST } }
    function success(coupons) { return { type: couponConstants.GET_USER_COUPONS_SUCCESS, coupons } }
    function failure(error) { return { type: couponConstants.GET_USER_COUPONS_FAILURE, error } }
}

function getUserCouponsByFuelType(userId, productId) {
    return dispatch => {
        dispatch(request());

        couponService.getUserCouponsByFuelType(userId, productId)
            .then(
                coupons => dispatch(success(coupons)),
                error => dispatch(failure(error.toString()))
            );
    };

    function request() { return { type: couponConstants.GET_USER_COUPONS_REQUEST } }
    function success(coupons) { return { type: couponConstants.GET_USER_COUPONS_SUCCESS, coupons } }
    function failure(error) { return { type: couponConstants.GET_USER_COUPONS_FAILURE, error } }
}

// prefixed function name with underscore because delete is a reserved word in javascript
function _delete(id) {
    return dispatch => {
        dispatch(request(id));

        couponService.delete(id)
            .then(
                () => dispatch(success(id)),
                error => dispatch(failure(id, error.toString()))
            );
    };

    function request(id) { return { type: couponConstants.DELETE_COUPON_REQUEST, id } }
    function success(id) { return { type: couponConstants.DELETE_COUPON_SUCCESS, id } }
    function failure(id, error) { return { type: couponConstants.DELETE_COUPON_FAILURE, id, error } }
}

function clear() {
    return { type: couponConstants.CLEAR };
}