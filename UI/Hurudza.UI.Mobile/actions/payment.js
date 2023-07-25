import { paymentConstants } from '../constants/payment';
import { paymentService } from '../services/payment';
import { alertActions } from './alert';

export const paymentActions = {
    payClass,
    pollStatus,
    walletPay,
    clear
};

function payClass(phone) {
    return dispatch => {
        dispatch(request({ phone }));

        paymentService.payClass(phone)
            .then(
                _payment => { 
                    dispatch(success(_payment));
                },
                error => {
                    dispatch(failure(error.Message));
                    dispatch(alertActions.error(error.Message));
                }
            );
    };

    function request(phone) { return { type: paymentConstants.PAYMENT_REQUEST, phone } }
    function success(_payment) { return { type: paymentConstants.PAYMENT_SUCCESS, _payment } }
    function failure(error) { return { type: paymentConstants.PAYMENT_FAILURE, error } }
}

function walletPay(payment) {
    return dispatch => {
        dispatch(request(payment));

        paymentService.walletPay(payment)
            .then(
                payment => {
                    dispatch(success());
                    dispatch(alertActions.success(payment.description));
                },
                error => {
                    dispatch(failure(error.toString()));
                    dispatch(alertActions.error(error.toString()));
                }
            );
    };

    function request() { return { type: paymentConstants.WALLET_PAY_REQUEST } }
    function success() { return { type: paymentConstants.WALLET_PAY_SUCCESS } }
    function failure(error) { return { type: paymentConstants.WALLET_PAY_FAILURE, error } }
}

function pollStatus(pollUrl) {
    return dispatch => {
        dispatch(request({ pollUrl }));

        paymentService.pollStatus(pollUrl)
            .then(
                _pollStatus => { 
                    dispatch(success(_pollStatus));
                },
                error => {
                    dispatch(failure(error.toString()));
                    dispatch(alertActions.error(error.toString()));
                }
            );
    };

    function request(pollUrl) { return { type: paymentConstants.POLL_REQUEST, pollUrl } }
    function success(_pollStatus) { return { type: paymentConstants.POLL_SUCCESS, _pollStatus } }
    function failure(error) { return { type: paymentConstants.POLL_FAILURE, error } }
}

function clear(_class) {
    return { type: paymentConstants.CLEAR, _class };
}