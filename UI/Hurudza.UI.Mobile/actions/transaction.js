import { transactionConstants } from '../constants/transaction';
import { transactionService } from '../services/transaction';
import { alertActions } from './alert';

export const transactionActions = {
    select: _select,
    getUserTransactions,
    getTransactionsByWallet,
    clear,
};

function _select(_transaction) {
    return { type: transactionConstants.SELECT_TRANSACTION, _transaction };
}

function getUserTransactions(id) {
    return dispatch => {
        dispatch(request());

        transactionService.getUserTransactions(id)
            .then(
                transactions => dispatch(success(transactions)),
                error => dispatch(failure(error.toString()))
            );
    };

    function request() { return { type: transactionConstants.GET_USER_TRANSACTIONS_REQUEST } }
    function success(transactions) { return { type: transactionConstants.GET_USER_TRANSACTIONS_SUCCESS, transactions } }
    function failure(error) { return { type: transactionConstants.GET_USER_TRANSACTIONS_FAILURE, error } }
}

function getTransactionsByWallet(walletCode) {
    return dispatch => {
        dispatch(request());

        transactionService.getTransactionsByWallet(walletCode)
            .then(
                transactions => dispatch(success(transactions)),
                error => dispatch(failure(error.toString()))
            );
    };

    function request() { return { type: transactionConstants.GET_TRANSACTIONS_BY_WALLET_REQUEST } }
    function success(transactions) { return { type: transactionConstants.GET_TRANSACTIONS_BY_WALLET_SUCCESS, transactions } }
    function failure(error) { return { type: transactionConstants.GET_TRANSACTIONS_BY_WALLET_FAILURE, error } }
}

function clear() {
    return { type: transactionConstants.CLEAR };
}