import { bankCardConstants } from '../constants/bankCard';
import { bankCardService } from '../services/bankCard';
import { alertActions } from './alert';

export const bankCardActions = {
    add,
    select: _select,
    getAll,
    getUserBankCards,
    delete: _delete,
    clear,
    updateBankCard
};

function _select(_bankCard) {
    return { type: bankCardConstants.SELECT_BANK_CARD, _bankCard };
}

function add(_bankCard) {
    return dispatch => {
        dispatch(request(_bankCard));

        bankCardService.add(_bankCard)
            .then(
                _bankCard => { 
                    dispatch(success(_bankCard));
                    dispatch(alertActions.success('BankCard added successfully'));
                },
                error => {
                    dispatch(failure(error.toString()));
                    dispatch(alertActions.error(error.toString()));
                }
            );
    };

    function request(_bankCard) { return { type: bankCardConstants.ADD_BANK_CARD_REQUEST, _bankCard } }
    function success(_bankCard) { return { type: bankCardConstants.ADD_BANK_CARD_SUCCESS, _bankCard } }
    function failure(error) { return { type: bankCardConstants.ADD_BANK_CARD_FAILURE, error } }
}

function updateBankCard(_bankCard) {
    return dispatch => {
        dispatch(request(_bankCard));

        bankCardService.update(_bankCard)
            .then(
                _bankCard => {
                    dispatch(success(_bankCard));
                    dispatch(alertActions.success('BankCard updated successfully'));
                },
                error => {
                    dispatch(failure(error.toString()));
                    dispatch(alertActions.error(error.toString()));
                }
            );
    };

    function request(_bankCard) { return { type: bankCardConstants.UPDATE_BANK_CARD_REQUEST, _bankCard } }
    function success(_bankCard) { return { type: bankCardConstants.UPDATE_BANK_CARD_SUCCESS, _bankCard } }
    function failure(error) { return { type: bankCardConstants.UPDATE_BANK_CARD_FAILURE, error } }
}

function getAll() {
    return dispatch => {
        dispatch(request());

        bankCardService.getAll()
            .then(
                bankCards => dispatch(success(bankCards)),
                error => dispatch(failure(error.toString()))
            );
    };

    function request() { return { type: bankCardConstants.GET_ALL_BANK_CARDS_REQUEST } }
    function success(bankCards) { return { type: bankCardConstants.GET_ALL_BANK_CARDS_SUCCESS, bankCards } }
    function failure(error) { return { type: bankCardConstants.GET_ALL_BANK_CARDS_FAILURE, error } }
}

function getUserBankCards(id) {
    return dispatch => {
        dispatch(request());

        bankCardService.getUserBankCards(id)
            .then(
                bankCards => dispatch(success(bankCards)),
                error => dispatch(failure(error.toString()))
            );
    };

    function request() { return { type: bankCardConstants.GET_USER_BANK_CARDS_REQUEST } }
    function success(bankCards) { return { type: bankCardConstants.GET_USER_BANK_CARDS_SUCCESS, bankCards } }
    function failure(error) { return { type: bankCardConstants.GET_USER_BANK_CARDS_FAILURE, error } }
}

// prefixed function name with underscore because delete is a reserved word in javascript
function _delete(id) {
    return dispatch => {
        dispatch(request(id));

        bankCardService.delete(id)
            .then(
                () => dispatch(success(id)),
                error => dispatch(failure(id, error.toString()))
            );
    };

    function request(id) { return { type: bankCardConstants.DELETE_BANK_CARD_REQUEST, id } }
    function success(id) { return { type: bankCardConstants.DELETE_BANK_CARD_SUCCESS, id } }
    function failure(id, error) { return { type: bankCardConstants.DELETE_BANK_CARD_FAILURE, id, error } }
}

function clear() {
    return { type: bankCardConstants.CLEAR };
}