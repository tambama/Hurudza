import { walletConstants } from '../constants/wallet';

const initialState = {
    addingWallet: false,
    addedWallet: false,
    loading: false,
    wallets: [],
    updatingWallet:false,
    updatedWallet:false,
    selectedWallet: {},
    wallet: {},
    paying: false,
    paid: false
  }

export function wallets(state = initialState, action) { 
  switch (action.type) {
    case walletConstants.GET_ALL_WALLETS_REQUEST:
      return {
        ...state,
        loading: true
      };
    case walletConstants.GET_ALL_WALLETS_SUCCESS:
      return {
        ...state,
        wallets: action.wallets
      };
    case walletConstants.GET_ALL_WALLETS_FAILURE:
      return { 
        error: action.error
      };
    case walletConstants.GET_USER_WALLETS_REQUEST:
      return {
        ...state,
        loading:true
      };
    case walletConstants.GET_USER_WALLETS_SUCCESS:
      return {
        ...state,
        wallets: action.wallets,
        loading: false
      }
    case walletConstants.GET_USER_WALLETS_FAILURE:
      return {
        ...state,
        loading: false
      }
      case walletConstants.GET_WALLET_BY_CODE_REQUEST:
        return {
          ...state,
          loading:true
        };
      case walletConstants.GET_WALLET_BY_CODE_SUCCESS:
        return {
          ...state,
          wallet: action.wallet,
          loading: false
        }
      case walletConstants.GET_WALLET_BY_CODE_FAILURE:
        return {
          ...state,
          loading: false
        }
    case walletConstants.SELECT_WALLET:
      return {
        ...state,
        selectedWallet: action._wallet
      }
    case walletConstants.ADD_WALLET_REQUEST:
      return { 
          ...state, 
          addingWallet: true 
        };
    case walletConstants.ADD_WALLET_SUCCESS:
      return { 
          ...state, 
          addingWallet: false, 
          addedWallet: true, 
          wallets: [...state.wallets, {...action._wallet}]
        };
    case walletConstants.ADD_WALLET_FAILURE:
      return { 
          ...state, 
          addingWallet: false
        };
    case walletConstants.UPDATE_WALLET_REQUEST:
      return { 
          ...state, 
          updatingWallet: true,
          updatedWallet:false 
        };
    case walletConstants.UPDATE_WALLET_SUCCESS:
      return { 
          ...state, 
          updatingWallet: false, 
          updatedWallet: true, 
          wallets: state.wallets.map(_wallet =>
            _wallet.id === action._wallet.id
              ? action._wallet
              : _wallet
            )
        };
    case walletConstants.UPDATE_WALLET_FAILURE:
      return { 
          ...state, 
          updatingWallet: false
        };
        case walletConstants.WALLET_PAY_STATION_REQUEST:
          return { 
              ...state, 
              paying: true,
              paid: false
            };
        case walletConstants.WALLET_PAY_STATION_SUCCESS:
          return { 
              ...state, 
              paying: false, 
              paid: true,
              wallets: state.wallets.map(_wallet =>
                _wallet.id === action.wallet.id
                  ? action.wallet
                  : _wallet
                ),
              selectedWallet: action.wallet
            };
        case walletConstants.WALLET_PAY_STATION_FAILURE:
          return { 
              ...state, 
              paying: false,
              paid: false
            };
            case walletConstants.WALLET_TO_WALLET_TRANSFER_REQUEST:
              return { 
                  ...state, 
                  transfering: true,
                  transfered: false
                };
            case walletConstants.WALLET_TO_WALLET_TRANSFER_SUCCESS:
              return { 
                  ...state, 
                  transfering: false, 
                  transfered: true,
                  wallets: state.wallets.map(_wallet =>
                    _wallet.id === action.wallet.id
                      ? action.wallet
                      : _wallet
                    ),
                  selectedWallet: action.wallet,
                  wallet: {}
                };
            case walletConstants.WALLET_TO_WALLET_TRANSFER_FAILURE:
              return { 
                  ...state, 
                  transfering: false,
                  transfered: false
                };
        case walletConstants.TOPUP_WALLET_MOBILE_PAYMENT_REQUEST:
          return { 
              ...state, 
              updatingWallet: true,
              updatedWallet:false 
            };
        case walletConstants.TOPUP_WALLET_MOBILE_PAYMENT_SUCCESS:
          return { 
              ...state, 
              updatingWallet: false, 
              updatedWallet: true, 
              wallets: state.wallets.map(_wallet =>
                _wallet.id === action._wallet.id
                  ? action._wallet
                  : _wallet
                ),
              selectedWallet: action._wallet
            };
        case walletConstants.TOPUP_WALLET_MOBILE_PAYMENT_FAILURE:
          return { 
              ...state, 
              updatingWallet: false,
              updatedWallet: false
            };
    case walletConstants.DELETE_WALLET_REQUEST:
      // add 'deleting:true' property to _wallet being deleted
      return {
        ...state,
        wallets: state.wallets.map(_wallet =>
          _wallet.id === action.id
            ? { ..._wallet, deleting: true }
            : _wallet
        )
      };
    case walletConstants.DELETE_WALLET_SUCCESS:
      // remove deleted _wallet from state
      return {
        wallets: state.wallets.filter(_wallet => _wallet.id !== action.id)
      };
    case walletConstants.DELETE_WALLET_FAILURE:
      // remove 'deleting:true' property and add 'deleteError:[error]' property to _wallet 
      return {
        ...state,
        wallets: state.wallets.map(_wallet => {
          if (_wallet.id === action.id) {
            // make copy of _wallet without 'deleting:true' property
            const { deleting, ...walletCopy } = _wallet;
            // return copy of _wallet with 'deleteError:[error]' property
            return { ...walletCopy, deleteError: action.error };
          }

          return _wallet;
        })
      };
    case walletConstants.CLEAR:
      return {
        ...state,
        wallet: {},
        addedWallet: false,
        addingWallet: false,
        updatingWallet:false,
        updatedWallet:false,
        loading: false,
        paying: false,
        paid: false,
        transfered: false,
        transfering: false,
      }
    default:
      return state
  }
}