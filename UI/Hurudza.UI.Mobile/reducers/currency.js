import { currencyConstants } from '../constants/currency';

const initialState = {
    addingCurrency: false,
    addedCurrency: false,
    loading: false,
    selectedCurrency: {
      id: 0,
      name: 'Select'
    },
    currencies: [],
  }

export function currencies(state = initialState, action) { 
  switch (action.type) {
    case currencyConstants.GET_ALL_CURRENCIES_REQUEST:
      return {
        ...state,
        loading: true
      };
    case currencyConstants.GET_ALL_CURRENCIES_SUCCESS:
      return {
        ...state,
        currencies: action.currencies
      };
    case currencyConstants.GET_ALL_CURRENCIES_FAILURE:
      return { 
        error: action.error
      };
    case currencyConstants.GET_USER_CURRENCIES_REQUEST:
      return {
        ...state,
        loading:true
      };
    case currencyConstants.GET_USER_CURRENCIES_SUCCESS:
      return {
        ...state,
        currencies: action.currencies.map((_currency) => { return {..._currency}}),
        loading: false
      }
    case currencyConstants.GET_USER_CURRENCIES_FAILURE:
      return {
        ...state,
        loading: false
      }
      case currencyConstants.GET_OMC_COUPON_CURRENCIES_REQUEST:
        return {
          ...state,
          loading:true
        };
      case currencyConstants.GET_OMC_COUPON_CURRENCIES_SUCCESS:
        return {
          ...state,
          currencies: action.currencies.map((_currency) => { return {..._currency}}),
          loading: false
        }
      case currencyConstants.GET_OMC_COUPON_CURRENCIES_FAILURE:
        return {
          ...state,
          loading: false
        }
    case currencyConstants.SELECT_CURRENCY:
      return {
        ...state,
        selectedCurrency: action._currency
      }
    case currencyConstants.ADD_CURRENCY_REQUEST:
      return { 
          ...state, 
          addingCurrency: true 
        };
    case currencyConstants.ADD_CURRENCY_SUCCESS:
      return { 
          ...state, 
          addingCurrency: false, 
          addedCurrency: true, 
          currencies: [...state.currencies, {...action._currency}]
        };
    case currencyConstants.ADD_CURRENCY_FAILURE:
      return { 
          ...state, 
          addingCurrency: false
        };
    case currencyConstants.UPDATE_CURRENCY_REQUEST:
      return { 
          ...state, 
          updatingCurrency: true,
          updatedCurrency:false 
        };
    case currencyConstants.UPDATE_CURRENCY_SUCCESS:
      return { 
          ...state, 
          updatingCurrency: false, 
          updatedCurrency: true, 
          currencies: state.currencies.map(_currency =>
            _currency.id === action._currency.id
              ? action._currency
              : _currency
            )
        };
    case currencyConstants.UPDATE_CURRENCY_FAILURE:
      return { 
          ...state, 
          updatingCurrency: false
        };
    case currencyConstants.DELETE_CURRENCY_REQUEST:
      // add 'deleting:true' property to _currency being deleted
      return {
        ...state,
        currencies: state.currencies.map(_currency =>
          _currency.id === action.id
            ? { ..._currency, deleting: true }
            : _currency
        )
      };
    case currencyConstants.DELETE_CURRENCY_SUCCESS:
      // remove deleted _currency from state
      return {
        currencies: state.currencies.filter(_currency => _currency.id !== action.id)
      };
    case currencyConstants.DELETE_CURRENCY_FAILURE:
      // remove 'deleting:true' property and add 'deleteError:[error]' property to _currency 
      return {
        ...state,
        currencies: state.currencies.map(_currency => {
          if (_currency.id === action.id) {
            // make copy of _currency without 'deleting:true' property
            const { deleting, ...currencyCopy } = _currency;
            // return copy of _currency with 'deleteError:[error]' property
            return { ...currencyCopy, deleteError: action.error };
          }

          return _currency;
        })
      };
    case currencyConstants.CLEAR:
      return {
        ...state,
        addedCurrency: false,
        addedCurrency: false,
        addingCurrency: false,
        loading: false,
        selectedCurrency: {
          id: 0,
          name: 'Select'
        },
      }
    default:
      return state
  }
}