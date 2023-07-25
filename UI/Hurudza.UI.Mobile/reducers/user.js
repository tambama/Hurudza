import { userConstants } from '../constants/user';

const initialState = {
  user: {
    fullName: '',
    lastName: '',
    username: '',
    password: '',
  },
  selectedUser: {},
  gettingUser: false,
  updatingUser: false,
  updated: false,
  userError: '',
  hasUserError: false
}

export function user(state = initialState, action) {
  switch (action.type) {
    case userConstants.GETALL_REQUEST:
      return {
        loading: true
      };
    case userConstants.GETALL_SUCCESS:
      return {
        items: action.users
      };
    case userConstants.GETALL_FAILURE:
      return { 
        error: action.error
      };
      case userConstants.GET_USER_BY_USERNAME_REQUEST:
        return {
          gettingUser: true,
          hasUserError: false,
          userError: ''
        };
      case userConstants.GET_USER_BY_USERNAME_SUCCESS:
        return {
          selectedUser: action.user,
          gettingUser: false
        };
      case userConstants.GET_USER_BY_USERNAME_FAILURE:
        return { 
          userError: action.error,
          gettingUser: false,
          hasUserError: true
        };
    case userConstants.UPDATE_USER_REQUEST:
      return { 
        ...state, 
        updatingUser: true 
      };
    case userConstants.UPDATE_USER_SUCCESS:
      return { 
        ...state, 
        updatingUser: false, 
        updated: true,
      };
    case userConstants.UPDATE_USER_FAILURE:
      return { 
        ...state, 
        updatingUser: false,
        updated: false
      };
    case userConstants.DELETE_REQUEST:
      // add 'deleting:true' property to user being deleted
      return {
        ...state,
        items: state.items.map(user =>
          user.id === action.id
            ? { ...user, deleting: true }
            : user
        )
      };
    case userConstants.DELETE_SUCCESS:
      // remove deleted user from state
      return {
        items: state.items.filter(user => user.id !== action.id)
      };
    case userConstants.DELETE_FAILURE:
      // remove 'deleting:true' property and add 'deleteError:[error]' property to user 
      return {
        ...state,
        items: state.items.map(user => {
          if (user.id === action.id) {
            // make copy of user without 'deleting:true' property
            const { deleting, ...userCopy } = user;
            // return copy of user with 'deleteError:[error]' property
            return { ...userCopy, deleteError: action.error };
          }

          return user;
        })
      };
      case userConstants.CLEAR:
        return {
          ...state,
          updatingUser: false,
          updated: false,
          requestedForgot: false,
          registered: false,
          hasUserError: false,
          selectedUser: {},
          userError: ''
        }
    default:
      return state
  }
}