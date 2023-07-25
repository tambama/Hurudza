import { couponPriceConstants } from '../constants/couponPrice';

const initialState = {
    addingCouponPrice: false,
    addedCouponPrice: false,
    couponPriceChanged: false,
    loading: false,
    selectedCouponPrice: {
      id: 0,
    },
    couponPrices: [],
  }

export function couponPrices(state = initialState, action) { 
  switch (action.type) {
    case couponPriceConstants.GET_ALL_COUPONPRICES_REQUEST:
      return {
        ...state,
        loading: true
      };
    case couponPriceConstants.GET_ALL_COUPONPRICES_SUCCESS:
      return {
        ...state,
        couponPrices: action.couponPrices
      };
    case couponPriceConstants.GET_ALL_COUPONPRICES_FAILURE:
      return { 
        error: action.error
      };
    case couponPriceConstants.GET_USER_COUPONPRICES_REQUEST:
      return {
        ...state,
        loading:true
      };
    case couponPriceConstants.GET_USER_COUPONPRICES_SUCCESS:
      return {
        ...state,
        couponPrices: action.couponPrices.map((_couponPrice) => { return {..._couponPrice}}),
        loading: false
      }
    case couponPriceConstants.GET_USER_COUPONPRICES_FAILURE:
      return {
        ...state,
        loading: false
      }
      case couponPriceConstants.GET_OMC_COUPONPRICES_REQUEST:
        return {
          ...state,
          loading:true
        };
      case couponPriceConstants.GET_OMC_COUPONPRICES_SUCCESS:
        return {
          ...state,
          couponPrices: action.couponPrices.map((_couponPrice) => { return {..._couponPrice}}),
          loading: false,
          couponPriceChanged: true
        }
      case couponPriceConstants.GET_OMC_COUPONPRICES_FAILURE:
        return {
          ...state,
          loading: false
        }
    case couponPriceConstants.SELECT_COUPONPRICE:
      return {
        ...state,
        selectedCouponPrice: action._couponPrice
      }
    case couponPriceConstants.ADD_COUPONPRICE_REQUEST:
      return { 
          ...state, 
          addingCouponPrice: true 
        };
    case couponPriceConstants.ADD_COUPONPRICE_SUCCESS:
      return { 
          ...state, 
          addingCouponPrice: false, 
          addedCouponPrice: true, 
          couponPrices: [...state.couponPrices, {...action._couponPrice}]
        };
    case couponPriceConstants.ADD_COUPONPRICE_FAILURE:
      return { 
          ...state, 
          addingCouponPrice: false
        };
    case couponPriceConstants.UPDATE_COUPONPRICE_REQUEST:
      return { 
          ...state, 
          updatingCouponPrice: true,
          updatedCouponPrice:false 
        };
    case couponPriceConstants.UPDATE_COUPONPRICE_SUCCESS:
      return { 
          ...state, 
          updatingCouponPrice: false, 
          updatedCouponPrice: true, 
          couponPrices: state.couponPrices.map(_couponPrice =>
            _couponPrice.id === action._couponPrice.id
              ? action._couponPrice
              : _couponPrice
            )
        };
    case couponPriceConstants.UPDATE_COUPONPRICE_FAILURE:
      return { 
          ...state, 
          updatingCouponPrice: false
        };
    case couponPriceConstants.DELETE_COUPONPRICE_REQUEST:
      // add 'deleting:true' property to _couponPrice being deleted
      return {
        ...state,
        couponPrices: state.couponPrices.map(_couponPrice =>
          _couponPrice.id === action.id
            ? { ..._couponPrice, deleting: true }
            : _couponPrice
        )
      };
    case couponPriceConstants.DELETE_COUPONPRICE_SUCCESS:
      // remove deleted _couponPrice from state
      return {
        couponPrices: state.couponPrices.filter(_couponPrice => _couponPrice.id !== action.id)
      };
    case couponPriceConstants.DELETE_COUPONPRICE_FAILURE:
      // remove 'deleting:true' property and add 'deleteError:[error]' property to _couponPrice 
      return {
        ...state,
        couponPrices: state.couponPrices.map(_couponPrice => {
          if (_couponPrice.id === action.id) {
            // make copy of _couponPrice without 'deleting:true' property
            const { deleting, ...couponpriceCopy } = _couponPrice;
            // return copy of _couponPrice with 'deleteError:[error]' property
            return { ...couponpriceCopy, deleteError: action.error };
          }

          return _couponPrice;
        })
      };
    case couponPriceConstants.CLEAR:
      return {
        ...state,
        addedCouponPrice: false,
        addedCouponPrice: false,
        addingCouponPrice: false,
        loading: false
      }
    default:
      return state
  }
}