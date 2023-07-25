import { bankConstants } from '../constants/bank';

const initialState = {
    addingBank: false,
    addedBank: false,
    loading: false,
    selectedBank: {
      sortCode: 0,
      bankName: 'Select'
    },
    banks: [],
    bankBranches: []
  }

export function banks(state = initialState, action) { 
  switch (action.type) {
    case bankConstants.GET_ALL_BANKS_REQUEST:
      return {
        ...state,
        loading: true
      };
    case bankConstants.GET_ALL_BANKS_SUCCESS:
      return {
        ...state,
        banks: action.banks
      };
    case bankConstants.GET_ALL_BANKS_FAILURE:
      return { 
        error: action.error
      };
      case bankConstants.GET_BANKS_WITH_COUPON_PRICES_REQUEST:
        return {
          ...state,
          loading: true
        };
      case bankConstants.GET_BANKS_WITH_COUPON_PRICES_SUCCESS:
        return {
          ...state,
          banks: action.banks
        };
      case bankConstants.GET_BANKS_WITH_COUPON_PRICES_FAILURE:
        return { 
          error: action.error
        };
    case bankConstants.GET_USER_BANKS_REQUEST:
      return {
        ...state,
        loading:true
      };
    case bankConstants.GET_USER_BANKS_SUCCESS:
      return {
        ...state,
        banks: action.banks.map((_bank) => { return {..._bank}}),
        loading: false
      }
    case bankConstants.GET_USER_BANKS_FAILURE:
      return {
        ...state,
        loading: false
      }
    case bankConstants.SELECT_BANK:
      return {
        ...state,
        selectedBank: action._bank,
        bankBranches: action._bank.bankBranches
      }
    case bankConstants.ADD_BANK_REQUEST:
      return { 
          ...state, 
          addingBank: true 
        };
    case bankConstants.ADD_BANK_SUCCESS:
      return { 
          ...state, 
          addingBank: false, 
          addedBank: true, 
          banks: [...state.banks, {...action._bank}]
        };
    case bankConstants.ADD_BANK_FAILURE:
      return { 
          ...state, 
          addingBank: false
        };
    case bankConstants.UPDATE_BANK_REQUEST:
      return { 
          ...state, 
          updatingBank: true,
          updatedBank:false 
        };
    case bankConstants.UPDATE_BANK_SUCCESS:
      return { 
          ...state, 
          updatingBank: false, 
          updatedBank: true, 
          banks: state.banks.map(_bank =>
            _bank.id === action._bank.id
              ? action._bank
              : _bank
            )
        };
    case bankConstants.UPDATE_BANK_FAILURE:
      return { 
          ...state, 
          updatingBank: false
        };
    case bankConstants.DELETE_BANK_REQUEST:
      // add 'deleting:true' property to _bank being deleted
      return {
        ...state,
        banks: state.banks.map(_bank =>
          _bank.id === action.id
            ? { ..._bank, deleting: true }
            : _bank
        )
      };
    case bankConstants.DELETE_BANK_SUCCESS:
      // remove deleted _bank from state
      return {
        banks: state.banks.filter(_bank => _bank.id !== action.id)
      };
    case bankConstants.DELETE_BANK_FAILURE:
      // remove 'deleting:true' property and add 'deleteError:[error]' property to _bank 
      return {
        ...state,
        banks: state.banks.map(_bank => {
          if (_bank.id === action.id) {
            // make copy of _bank without 'deleting:true' property
            const { deleting, ...bankCopy } = _bank;
            // return copy of _bank with 'deleteError:[error]' property
            return { ...bankCopy, deleteError: action.error };
          }

          return _bank;
        })
      };
    case bankConstants.CLEAR:
      return {
        ...state,
        addedBank: false,
        addedBank: false,
        addingBank: false,
        loading: false
      }
    default:
      return state
  }
}