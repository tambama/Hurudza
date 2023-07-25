import { productConstants } from '../constants/product';

const initialState = {
    loadingProducts: false,
    selectedProduct: {
      id: 0,
      name: 'Select'
    },
    products: [],
  }

export function products(state = initialState, action) { 
  switch (action.type) {
    case productConstants.GET_ALL_PRODUCTS_REQUEST:
      return {
        ...state,
        loadingProducts: true
      };
    case productConstants.GET_ALL_PRODUCTS_SUCCESS:
      return {
        ...state,
        products: action.products
      };
    case productConstants.GET_ALL_PRODUCTS_FAILURE:
      return { 
        error: action.error
      };
    case productConstants.GET_PRODUCT_BY_ID_REQUEST:
      return {
        ...state,
        loadingProducts:true
      };
    case productConstants.GET_PRODUCT_BY_ID_SUCCESS:
      return {
        ...state,
        products: action.products.map((_product) => { return {..._product}}),
        loadingProducts: false
      }
    case productConstants.GET_PRODUCT_BY_ID_FAILURE:
      return {
        ...state,
        loadingProducts: false
      }
    case productConstants.SELECT_PRODUCT:
      return {
        ...state,
        selectedProduct: action._product
      }
    case productConstants.CLEAR:
      return {
        ...state,
        selectedProduct: {
          id: 0,
          name: 'Select'
        },
        loadingProducts: false
      }
    default:
      return state
  }
}