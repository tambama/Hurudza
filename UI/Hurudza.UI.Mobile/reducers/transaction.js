import { transactionConstants } from '../constants/transaction';

const initialState = {
    loading: false,
    transactions: [],
    selectedTransaction: {},
    homeTransactions: [],
    walletTransactions: []
  }

export function transactions(state = initialState, action) { 
  switch (action.type) {
    case transactionConstants.GET_USER_TRANSACTIONS_REQUEST:
      return {
        ...state,
        loading:true
      };
    case transactionConstants.GET_USER_TRANSACTIONS_SUCCESS:
      return {
        ...state,
        transactions: action.transactions,
        homeTransactions: action.transactions.filter((a, i) => i < 3),
        loading: false
      }
    case transactionConstants.GET_USER_TRANSACTIONS_FAILURE:
      return {
        ...state,
        loading: false
      }
      case transactionConstants.GET_TRANSACTIONS_BY_WALLET_REQUEST:
        return {
          ...state,
          loading:true
        };
      case transactionConstants.GET_TRANSACTIONS_BY_WALLET_SUCCESS:
        return {
          ...state,
          walletTransactions: action.transactions,
          loading: false
        }
      case transactionConstants.GET_TRANSACTIONS_BY_WALLET_FAILURE:
        return {
          ...state,
          loading: false
        }
    case transactionConstants.SELECT_TRANSACTION:
      return {
        ...state,
        selectedTransaction: action._transaction
      }
    case transactionConstants.CLEAR:
      return {
        ...state,
        loading: false,
      }
    default:
      return state
  }
}