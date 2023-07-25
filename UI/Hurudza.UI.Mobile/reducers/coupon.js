import { couponConstants } from '../constants/coupon';

const initialState = {
    addingCoupon: false,
    addedCoupon: false,
    loading: false,
    coupons: [],
    selectedCoupon: {
      id: 0
    }
  }

export function coupons(state = initialState, action) { 
  switch (action.type) {
    case couponConstants.GET_ALL_COUPONS_REQUEST:
      return {
        ...state,
        loading: true
      };
    case couponConstants.GET_ALL_COUPONS_SUCCESS:
      return {
        ...state,
        coupons: action.coupons
      };
    case couponConstants.GET_ALL_COUPONS_FAILURE:
      return { 
        error: action.error
      };
    case couponConstants.GET_USER_COUPONS_REQUEST:
      return {
        ...state,
        loading:true
      };
    case couponConstants.GET_USER_COUPONS_SUCCESS:
      return {
        ...state,
        coupons: action.coupons.map((_coupon) => { return {..._coupon}}),
        loading: false
      }
    case couponConstants.GET_USER_COUPONS_FAILURE:
      return {
        ...state,
        loading: false
      }
    case couponConstants.SELECT_COUPON:
      return {
        ...state,
        selectedCoupon: action._coupon
      }
    case couponConstants.ADD_COUPON_REQUEST:
      return { 
          ...state, 
          addingCoupon: true 
        };
    case couponConstants.ADD_COUPON_SUCCESS:
      return { 
          ...state, 
          addingCoupon: false, 
          addedCoupon: true, 
          coupons: [...state.coupons, {...action._coupon}]
        };
    case couponConstants.ADD_COUPON_FAILURE:
      return { 
          ...state, 
          addingCoupon: false
        };
        case couponConstants.CREATE_COUPON_MOBILE_PAYMENT_REQUEST:
          return { 
              ...state, 
              addingCoupon: true 
            };
        case couponConstants.CREATE_COUPON_MOBILE_PAYMENT_SUCCESS:
          return { 
              ...state, 
              addingCoupon: false, 
              addedCoupon: true, 
              coupons: [...state.coupons, {...action._coupon}]
            };
        case couponConstants.CREATE_COUPON_MOBILE_PAYMENT_FAILURE:
          return { 
              ...state, 
              addingCoupon: false,
            };
    case couponConstants.UPDATE_COUPON_REQUEST:
      return { 
          ...state, 
          updatingCoupon: true,
          updatedCoupon:false 
        };
    case couponConstants.UPDATE_COUPON_SUCCESS:
      return { 
          ...state, 
          updatingCoupon: false, 
          updatedCoupon: true, 
          coupons: state.coupons.map(_coupon =>
            _coupon.id === action._coupon.id
              ? action._coupon
              : _coupon
            )
        };
    case couponConstants.UPDATE_COUPON_FAILURE:
      return { 
          ...state, 
          updatingCoupon: false
        };
    case couponConstants.DELETE_COUPON_REQUEST:
      // add 'deleting:true' property to _coupon being deleted
      return {
        ...state,
        coupons: state.coupons.map(_coupon =>
          _coupon.id === action.id
            ? { ..._coupon, deleting: true }
            : _coupon
        )
      };
    case couponConstants.DELETE_COUPON_SUCCESS:
      // remove deleted _coupon from state
      return {
        coupons: state.coupons.filter(_coupon => _coupon.id !== action.id)
      };
    case couponConstants.DELETE_COUPON_FAILURE:
      // remove 'deleting:true' property and add 'deleteError:[error]' property to _coupon 
      return {
        ...state,
        coupons: state.coupons.map(_coupon => {
          if (_coupon.id === action.id) {
            // make copy of _coupon without 'deleting:true' property
            const { deleting, ...couponCopy } = _coupon;
            // return copy of _coupon with 'deleteError:[error]' property
            return { ...couponCopy, deleteError: action.error };
          }

          return _coupon;
        })
      };
    case couponConstants.CLEAR:
      return {
        ...state,
        addedCoupon: false,
        addedCoupon: false,
        addingCoupon: false,
        updatingCoupon: false,
        updatedCoupon: false,
        selectedCoupon: {
          id: 0
        },
        loading: false
      }
    default:
      return state
  }
}