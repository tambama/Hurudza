import { bankCardConstants } from '../constants/bankCard';

const initialState = {
    addingBankCard: false,
    addedBankCard: false,
    loading: false,
    selectedBankCard: {
      id: 0,
    },
    bankCards: [],
  }

export function bankCards(state = initialState, action) { 
  switch (action.type) {
    case bankCardConstants.GET_ALL_BANK_CARDS_REQUEST:
      return {
        ...state,
        loading: true
      };
    case bankCardConstants.GET_ALL_BANK_CARDS_SUCCESS:
      return {
        ...state,
        bankCards: action.bankCards
      };
    case bankCardConstants.GET_ALL_BANK_CARDS_FAILURE:
      return { 
        error: action.error
      };
    case bankCardConstants.GET_USER_BANK_CARDS_REQUEST:
      return {
        ...state,
        loading:true
      };
    case bankCardConstants.GET_USER_BANK_CARDS_SUCCESS:
      return {
        ...state,
        bankCards: action.bankCards.map((_bankCard) => { return {..._bankCard}}),
        loading: false
      }
    case bankCardConstants.GET_USER_BANK_CARDS_FAILURE:
      return {
        ...state,
        loading: false
      }
    case bankCardConstants.SELECT_BANK_CARD:
      return {
        ...state,
        selectedBankCard: action._bankCard
      }
    case bankCardConstants.ADD_BANK_CARD_REQUEST:
      return { 
          ...state, 
          addingBankCard: true 
        };
    case bankCardConstants.ADD_BANK_CARD_SUCCESS:
      return { 
          ...state, 
          addingBankCard: false, 
          addedBankCard: true, 
          bankCards: [...state.bankCards, {...action._bankCard}]
        };
    case bankCardConstants.ADD_BANK_CARD_FAILURE:
      return { 
          ...state, 
          addingBankCard: false
        };
    case bankCardConstants.UPDATE_BANK_CARD_REQUEST:
      return { 
          ...state, 
          updatingBankCard: true,
          updatedBankCard:false 
        };
    case bankCardConstants.UPDATE_BANK_CARD_SUCCESS:
      return { 
          ...state, 
          updatingBankCard: false, 
          updatedBankCard: true, 
          bankCards: state.bankCards.map(_bankCard =>
            _bankCard.id === action._bankCard.id
              ? action._bankCard
              : _bankCard
            )
        };
    case bankCardConstants.UPDATE_BANK_CARD_FAILURE:
      return { 
          ...state, 
          updatingBankCard: false
        };
    case bankCardConstants.DELETE_BANK_CARD_REQUEST:
      // add 'deleting:true' property to _bankCard being deleted
      return {
        ...state,
        bankCards: state.bankCards.map(_bankCard =>
          _bankCard.id === action.id
            ? { ..._bankCard, deleting: true }
            : _bankCard
        )
      };
    case bankCardConstants.DELETE_BANK_CARD_SUCCESS:
      // remove deleted _bankCard from state
      return {
        bankCards: state.bankCards.filter(_bankCard => _bankCard.id !== action.id)
      };
    case bankCardConstants.DELETE_BANK_CARD_FAILURE:
      // remove 'deleting:true' property and add 'deleteError:[error]' property to _bankCard 
      return {
        ...state,
        bankCards: state.bankCards.map(_bankCard => {
          if (_bankCard.id === action.id) {
            // make copy of _bankCard without 'deleting:true' property
            const { deleting, ...bankCardCopy } = _bankCard;
            // return copy of _bankCard with 'deleteError:[error]' property
            return { ...bankCardCopy, deleteError: action.error };
          }

          return _bankCard;
        })
      };
    case bankCardConstants.CLEAR:
      return {
        ...state,
        addedBankCard: false,
        addedBankCard: false,
        addingBankCard: false,
        selectedBankCard: {
          id: 0
        },
        loading: false
      }
    default:
      return state
  }
}