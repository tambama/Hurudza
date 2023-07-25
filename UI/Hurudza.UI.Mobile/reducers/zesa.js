import { zesaConstants } from '../constants/zesa';

const initialState = {
    loadingZesa: false,
    zesaCustomerData: {},
    transaction: {},
    tokens: [],
    requesting: false,
    requested: false,
    buyingZesaToken: false,
    boughtZesaToken: false
  }

export function zesa(state = initialState, action) { 
  switch (action.type) {
    case zesaConstants.GET_CUSTOMER_DETAILS_REQUEST:
      return { 
          ...state, 
          loadingZesa: true,
        };
    case zesaConstants.GET_CUSTOMER_DETAILS_SUCCESS:
      return { 
          ...state, 
          loadingZesa: false, 
          zesaCustomerData: action.data
        };
    case zesaConstants.GET_CUSTOMER_DETAILS_FAILURE:
      return { 
          ...state, 
          loadingZesa: false
        };
        case zesaConstants.BUY_ZESA_TOKEN_REQUEST:
          return { 
              ...state, 
              buyingZesaToken: true,
            };
          case zesaConstants.BUY_ZESA_TOKEN_SUCCESS:
            return { 
                ...state, 
                buyingZesaToken: false,
                boughtZesaToken: true,
                transaction: action.data
              };
            case zesaConstants.BUY_ZESA_TOKEN_FAILURE:
              return { 
                  ...state, 
                  buyingZesaToken: false
              }
        case zesaConstants.GET_TOKEN_REQUEST:
            return {
                ...state,
                requesting: true,
                requested: false
            }
            case zesaConstants.GET_TOKEN_SUCCESS:
                return {
                    ...state,
                    tokens: action.tokens,
                    requesting: false,
                    requested: true
                }
                case zesaConstants.GET_TOKEN_FAILURE:
                    return {
                        ...state,
                        requesting: false,
                    }
        case zesaConstants.GET_PREVIOUS_TOKEN_REQUEST:
            return {
                ...state,
                requesting: true,
                requested: false
            }
            case zesaConstants.GET_PREVIOUS_TOKEN_SUCCESS:
                return {
                    ...state,
                    tokens: action.tokens,
                    requesting: false,
                    requested: true
                }
                case zesaConstants.GET_PREVIOUS_TOKEN_FAILURE:
                    return {
                        ...state,
                        requesting: false,
                    }
    
    case zesaConstants.CLEAR:
      return {
        ...state,
        tokens: [],
        zesaCustomerData: {},
        loadingZesa: false,
        buyingZesaToken: false,
        boughtZesaToken: false
      }
    default:
      return state
  }
}