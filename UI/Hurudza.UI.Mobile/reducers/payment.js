import { paymentConstants } from '../constants/payment';

const initialState = {
    paying: false,
    paid: false
  }

export function payments(state = initialState, action) { 
  switch (action.type) {
    case paymentConstants.WALLET_PAY_REQUEST:
      return {
        ...state,
        paying: true
      };
    case paymentConstants.WALLET_PAY_SUCCESS:
      return {
        ...state,
        paying: false,
        paid: true
      };
    case paymentConstants.WALLET_PAY_FAILURE:
      return { 
        ...state,
        paying: false,
        paid: false,
        error: action.error
      };
    case paymentConstants.CLEAR:
      return {
        ...state,
        paying: false,
        paid: false,
      }
    default:
      return state
  }
}