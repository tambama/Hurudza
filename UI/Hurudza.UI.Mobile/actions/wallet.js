import { walletConstants } from '../constants/wallet';
import { walletService } from '../services/wallet';
import { alertActions } from './alert';

export const walletActions = {
    add,
    select: _select,
    getAll,
    getUserWallets,
    getUserWalletsByFuelType,
    delete: _delete,
    clear,
    updateWallet,
    topupWallet,
    topupWalletMobilePayment,
    getWalletByCode,
    walletToWalletTransfer
};

function _select(_wallet) {
    return { type: walletConstants.SELECT_WALLET, _wallet };
}

function add(_wallet) {
    return dispatch => {
        dispatch(request(_wallet));

        walletService.add(_wallet)
            .then(
                _wallet => { 
                    dispatch(success(_wallet));
                    dispatch(alertActions.success('Wallet added successfully'));
                },
                error => {
                    dispatch(failure(error.toString()));
                    dispatch(alertActions.error(error.toString()));
                }
            );
    };

    function request(_wallet) { return { type: walletConstants.ADD_WALLET_REQUEST, _wallet } }
    function success(_wallet) { return { type: walletConstants.ADD_WALLET_SUCCESS, _wallet } }
    function failure(error) { return { type: walletConstants.ADD_WALLET_FAILURE, error } }
}

function updateWallet(_wallet) {
    return dispatch => {
        dispatch(request(_wallet));

        walletService.update(_wallet)
            .then(
                _wallet => {
                    dispatch(success(_wallet));
                    dispatch(alertActions.success('Wallet updated successfully'));
                },
                error => {
                    dispatch(failure(error.toString()));
                    dispatch(alertActions.error(error.toString()));
                }
            );
    };

    function request(_wallet) { return { type: walletConstants.UPDATE_WALLET_REQUEST, _wallet } }
    function success(_wallet) { return { type: walletConstants.UPDATE_WALLET_SUCCESS, _wallet } }
    function failure(error) { return { type: walletConstants.UPDATE_WALLET_FAILURE, error } }
}

function topupWallet(_wallet) {
    return dispatch => {
        dispatch(request(_wallet));

        walletService.topup(_wallet)
            .then(
                _wallet => {
                    dispatch(success(_wallet));
                    dispatch(alertActions.success('Wallet topup successful'));
                },
                error => {
                    dispatch(failure(error.toString()));
                    dispatch(alertActions.error(error.toString()));
                }
            );
    };

    function request(_wallet) { return { type: walletConstants.UPDATE_WALLET_REQUEST, _wallet } }
    function success(_wallet) { return { type: walletConstants.UPDATE_WALLET_SUCCESS, _wallet } }
    function failure(error) { return { type: walletConstants.UPDATE_WALLET_FAILURE, error } }
}

function walletToWalletTransfer(transfer) {
    return dispatch => {
        dispatch(request(transfer));

        walletService.walletToWalletTransfer(transfer)
            .then(
                wallet => {
                    dispatch(success(wallet));
                    dispatch(alertActions.success('Transfer successful'));
                },
                error => {
                    dispatch(failure(error.toString()));
                    dispatch(alertActions.error(error.toString()));
                }
            );
    };

    function request() { return { type: walletConstants.WALLET_TO_WALLET_TRANSFER_REQUEST } }
    function success(wallet) { return { type: walletConstants.WALLET_TO_WALLET_TRANSFER_SUCCESS, wallet } }
    function failure(error) { return { type: walletConstants.WALLET_TO_WALLET_TRANSFER_FAILURE, error } }
}

function topupWalletMobilePayment(_wallet) {
    return dispatch => {
        dispatch(request(_wallet));

        walletService.topupWalletMobilePayment(_wallet)
            .then(
                _wallet => {
                    dispatch(success(_wallet));
                    dispatch(alertActions.success('Wallet topup was successful'));
                },
                error => {
                    dispatch(failure(error.toString()));
                    dispatch(alertActions.error(error.toString()));
                }
            );
    };

    function request(_wallet) { return { type: walletConstants.TOPUP_WALLET_MOBILE_PAYMENT_REQUEST, _wallet } }
    function success(_wallet) { return { type: walletConstants.TOPUP_WALLET_MOBILE_PAYMENT_SUCCESS, _wallet } }
    function failure(error) { return { type: walletConstants.TOPUP_WALLET_MOBILE_PAYMENT_FAILURE, error } }
}

function getAll() {
    return dispatch => {
        dispatch(request());

        walletService.getAll()
            .then(
                wallets => dispatch(success(wallets)),
                error => dispatch(failure(error.toString()))
            );
    };

    function request() { return { type: walletConstants.GET_ALL_WALLETS_REQUEST } }
    function success(wallets) { return { type: walletConstants.GET_ALL_WALLETS_SUCCESS, wallets } }
    function failure(error) { return { type: walletConstants.GET_ALL_WALLETS_FAILURE, error } }
}

function getUserWallets(id) {
    return dispatch => {
        dispatch(request());
        walletService.getUserWallets(id)
            .then(
                wallets => dispatch(success(wallets)),
                error => dispatch(failure(error.toString()))
            );
    };

    function request() { return { type: walletConstants.GET_USER_WALLETS_REQUEST } }
    function success(wallets) { return { type: walletConstants.GET_USER_WALLETS_SUCCESS, wallets } }
    function failure(error) { return { type: walletConstants.GET_USER_WALLETS_FAILURE, error } }
}

function getWalletByCode(code) {
    return dispatch => {
        dispatch(request());

        walletService.getWalletByCode(code)
            .then(
                wallet => dispatch(success(wallet)),
                error => dispatch(failure(error.toString()))
            );
    };

    function request() { return { type: walletConstants.GET_WALLET_BY_CODE_REQUEST } }
    function success(wallet) { return { type: walletConstants.GET_WALLET_BY_CODE_SUCCESS, wallet } }
    function failure(error) { return { type: walletConstants.GET_WALLET_BY_CODE_FAILURE, error } }
}

function getUserWalletsByFuelType(userId, productId) {
    return dispatch => {
        dispatch(request());

        walletService.getUserWalletsByFuelType(userId, productId)
            .then(
                wallets => dispatch(success(wallets)),
                error => dispatch(failure(error.toString()))
            );
    };

    function request() { return { type: walletConstants.GET_USER_WALLETS_REQUEST } }
    function success(wallets) { return { type: walletConstants.GET_USER_WALLETS_SUCCESS, wallets } }
    function failure(error) { return { type: walletConstants.GET_USER_WALLETS_FAILURE, error } }
}

// prefixed function name with underscore because delete is a reserved word in javascript
function _delete(id) {
    return dispatch => {
        dispatch(request(id));

        walletService.delete(id)
            .then(
                () => dispatch(success(id)),
                error => dispatch(failure(id, error.toString()))
            );
    };

    function request(id) { return { type: walletConstants.DELETE_WALLET_REQUEST, id } }
    function success(id) { return { type: walletConstants.DELETE_WALLET_SUCCESS, id } }
    function failure(id, error) { return { type: walletConstants.DELETE_WALLET_FAILURE, id, error } }
}

function clear() {
    return { type: walletConstants.CLEAR };
}