import { branchConstants } from '../constants/branch';
import { branchService } from '../services/branch';
import { alertActions } from './alert';

export const branchActions = {
    add,
    select: _select,
    getAll,
    getUserBranches,
    getBankBranches,
    delete: _delete,
    clear,
    updateBranch
};

function _select(_branch) {
    return { type: branchConstants.SELECT_BRANCH, _branch };
}

function add(_branch) {
    return dispatch => {
        dispatch(request(_branch));

        branchService.add(_branch)
            .then(
                _branch => { 
                    dispatch(success(_branch));
                    dispatch(alertActions.success('Branch added successfully'));
                },
                error => {
                    dispatch(failure(error.toString()));
                    dispatch(alertActions.error(error.toString()));
                }
            );
    };

    function request(_branch) { return { type: branchConstants.ADD_BRANCH_REQUEST, _branch } }
    function success(_branch) { return { type: branchConstants.ADD_BRANCH_SUCCESS, _branch } }
    function failure(error) { return { type: branchConstants.ADD_BRANCH_FAILURE, error } }
}

function updateBranch(_branch) {
    return dispatch => {
        dispatch(request(_branch));

        branchService.update(_branch)
            .then(
                _branch => {
                    dispatch(success(_branch));
                    dispatch(alertActions.success('Branch updated successfully'));
                },
                error => {
                    dispatch(failure(error.toString()));
                    dispatch(alertActions.error(error.toString()));
                }
            );
    };

    function request(_branch) { return { type: branchConstants.UPDATE_BRANCH_REQUEST, _branch } }
    function success(_branch) { return { type: branchConstants.UPDATE_BRANCH_SUCCESS, _branch } }
    function failure(error) { return { type: branchConstants.UPDATE_BRANCH_FAILURE, error } }
}

function getAll() {
    return dispatch => {
        dispatch(request());

        branchService.getAll()
            .then(
                branches => dispatch(success(branches)),
                error => dispatch(failure(error.toString()))
            );
    };

    function request() { return { type: branchConstants.GET_ALL_BRANCHES_REQUEST } }
    function success(branches) { return { type: branchConstants.GET_ALL_BRANCHES_SUCCESS, branches } }
    function failure(error) { return { type: branchConstants.GET_ALL_BRANCHES_FAILURE, error } }
}

function getBankBranches(sortCode) {
    return dispatch => {
        dispatch(request());

        branchService.getBankBranches(sortCode)
            .then(
                branches => dispatch(success(branches)),
                error => dispatch(failure(error.toString()))
            );
    };

    function request() { return { type: branchConstants.GET_BANK_BRANCHES_REQUEST } }
    function success(branches) { return { type: branchConstants.GET_BANK_BRANCHES_SUCCESS, branches } }
    function failure(error) { return { type: branchConstants.GET_BANK_BRANCHES_FAILURE, error } }
}

function getUserBranches(id) {
    return dispatch => {
        dispatch(request());

        branchService.getUserBranches(id)
            .then(
                branches => dispatch(success(branches)),
                error => dispatch(failure(error.toString()))
            );
    };

    function request() { return { type: branchConstants.GET_USER_BRANCHES_REQUEST } }
    function success(branches) { return { type: branchConstants.GET_USER_BRANCHES_SUCCESS, branches } }
    function failure(error) { return { type: branchConstants.GET_USER_BRANCHES_FAILURE, error } }
}

// prefixed function name with underscore because delete is a reserved word in javascript
function _delete(id) {
    return dispatch => {
        dispatch(request(id));

        branchService.delete(id)
            .then(
                () => dispatch(success(id)),
                error => dispatch(failure(id, error.toString()))
            );
    };

    function request(id) { return { type: branchConstants.DELETE_BRANCH_REQUEST, id } }
    function success(id) { return { type: branchConstants.DELETE_BRANCH_SUCCESS, id } }
    function failure(id, error) { return { type: branchConstants.DELETE_BRANCH_FAILURE, id, error } }
}

function clear() {
    return { type: branchConstants.CLEAR };
}