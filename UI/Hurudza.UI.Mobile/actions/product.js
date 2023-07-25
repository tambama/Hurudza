import { productConstants } from '../constants/product';
import { productService } from '../services/product';

export const productActions = {
    select: _select,
    getAll,
    getProductById,
    clear
};

function _select(_product) {
    return { type: productConstants.SELECT_PRODUCT, _product };
}

function getAll() {
    return dispatch => {
        dispatch(request());

        productService.getAll()
            .then(
                products => dispatch(success(products)),
                error => dispatch(failure(error.toString()))
            );
    };

    function request() { return { type: productConstants.GET_ALL_PRODUCTS_REQUEST } }
    function success(products) { return { type: productConstants.GET_ALL_PRODUCTS_SUCCESS, products } }
    function failure(error) { return { type: productConstants.GET_ALL_PRODUCTS_FAILURE, error } }
}

function getProductById(id) {
    return dispatch => {
        dispatch(request());

        productService.getProductById(id)
            .then(
                product => dispatch(success(product)),
                error => dispatch(failure(error.toString()))
            );
    };

    function request() { return { type: productConstants.GET_PRODUCT_BY_ID_REQUEST } }
    function success(product) { return { type: productConstants.GET_PRODUCT_BY_ID_SUCCESS, product } }
    function failure(error) { return { type: productConstants.GET_PRODUCT_BY_ID_FAILURE, error } }
}

function clear() {
    return { type: productConstants.CLEAR };
}