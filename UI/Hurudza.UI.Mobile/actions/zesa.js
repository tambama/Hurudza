import { zesaConstants } from '../constants/zesa';
import { zesaService } from '../services/zesa';
import { alertActions } from './alert';

export const zesaActions = {
    getCustomerDetails,
    buyZesaToken,
    getToken,
    getPreviousTokens,
    clear
};

function getCustomerDetails(model) {
    return dispatch => {
        dispatch(request(model));

        zesaService.getCustomerDetails(model)
            .then(
                data => {
                    dispatch(success(data));
                },
                error => {
                    dispatch(failure(error.toString()));
                    dispatch(alertActions.error(error.toString()));
                }
            );
    };

    function request(data) { return { type: zesaConstants.GET_CUSTOMER_DETAILS_REQUEST, data } }
    function success(data) { return { type: zesaConstants.GET_CUSTOMER_DETAILS_SUCCESS, data } }
    function failure(error) { return { type: zesaConstants.GET_CUSTOMER_DETAILS_FAILURE, error } }
}

function buyZesaToken(model) {
    return dispatch => {
        dispatch(request(model));

        zesaService.buyZesaToken(model)
            .then(
                data => {
                    dispatch(success(data));
                },
                error => {
                    dispatch(failure(error.toString()));
                    dispatch(alertActions.error(error.toString()));
                }
            );
    };

    function request(data) { return { type: zesaConstants.BUY_ZESA_TOKEN_REQUEST, data } }
    function success(data) { return { type: zesaConstants.BUY_ZESA_TOKEN_SUCCESS, data } }
    function failure(error) { return { type: zesaConstants.BUY_ZESA_TOKEN_FAILURE, error } }
}

function getToken(reference) {
    return dispatch => {
        dispatch(request());

        zesaService.getToken(reference)
        .then(
            token => {
                console.log(token)
                dispatch(success(token.zesaTokens))
            },
            error => {
                 dispatch(failure(error.toString()))
                 dispatch(alertActions.error(error.toString()))
            }
        );
    };
    
    function request() { return { type: zesaConstants.GET_TOKEN_REQUEST } }
    function success(tokens) { return { type:zesaConstants.GET_TOKEN_SUCCESS, tokens } }
    function failure(error) { return { type:zesaConstants.GET_TOKEN_FAILURE, error } }
}

function getPreviousTokens(meter){
    return dispatch => {
        dispatch(request());

        zesaService.getPreviousTokens(meter)
            .then(
                tokens => {
                    dispatch(success(tokens))
                },
                error => {
                    dispatch(failure(error.toString()))
                    dispatch(alertActions.error(error.toString()))
                }
            )
    }

    function request() { return { type: zesaConstants.GET_PREVIOUS_TOKEN_REQUEST } }
    function success(tokens) { return { type:zesaConstants.GET_PREVIOUS_TOKEN_SUCCESS, tokens } }
    function failure(error) { return { type:zesaConstants.GET_PREVIOUS_TOKEN_FAILURE, error } }
}

function clear() {
    return { type: zesaConstants.CLEAR };
}