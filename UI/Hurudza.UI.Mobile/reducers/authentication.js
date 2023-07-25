import { userConstants } from '../constants/user';

const initialState = { loggedIn: false, loggingIn: false, user: {} };

export function authentication(state = initialState, action) {
  switch (action.type) {
    case userConstants.RESTORE_TOKEN_REQUEST:
      return {
        ...state,
        isLoading: true
      }
    case userConstants.RESTORE_TOKEN_SUCCESS:
      return {
        ...state,
        isLoading: false,
        user: action.user
      }
    case userConstants.RESTORE_TOKEN_FAILURE:
      return {
        ...state,
        isLoading: false,
      }
    case userConstants.LOGIN_REQUEST:
      return {
        ...state,
        loggingIn: true,
      };
    case userConstants.LOGIN_SUCCESS:
      return {
        loggedIn: true,
        loggingIn:false,
        user: action.user
      };
    case userConstants.LOGIN_FAILURE:
      return {
        ...state, 
        loggingIn:false
      };
      case userConstants.PASSWORD_RESET_REQUEST:
        return {
          ...state,
          otp: {},
          requestingReset: true,
          requestedReset: false,
        };
      case userConstants.PASSWORD_RESET_SUCCESS:
        return {
          ...state,
          requestedReset: true,
          requestingReset:false,
          otp: action.otp
        };
      case userConstants.PASSWORD_RESET_FAILURE:
        return {
          ...state, 
          requestingReset:false
        };
        case userConstants.RESET_PASSWORD_REQUEST:
        return {
          ...state,
          passwordChanged: false,
          resetRequesting: true,
          requestedReset: false,
          requestingReset:false,
        };
      case userConstants.RESET_PASSWORD_SUCCESS:
        return {
          ...state,
          passwordChanged: action.changed,
          resetRequesting:false
        };
      case userConstants.RESET_PASSWORD_FAILURE:
        return {
          ...state, 
          passwordChanged: false,
          resetRequesting:false
        };
        case userConstants.FORGOT_PASSWORD_REQUEST:
          return {
            ...state,
            passwordChanged: false,
            forgotRequesting: true,
            requestedForgot: false,
            requestingForgot:true,
          };
        case userConstants.FORGOT_PASSWORD_SUCCESS:
          return {
            ...state,
            otp: action.otp,
            requestedForgot: true,
            requestingForgot:false,
          };
        case userConstants.FORGOT_PASSWORD_FAILURE:
          return {
            ...state, 
            requestedForgot: false,
            requestingForgot:false,
          };
    case userConstants.LOGOUT:
      return {
        ...state,
        loggedIn:false,
        loggingIn:false, 
        user: {}
      };
      case userConstants.UPDATE_USER_SUCCESS:
        return { 
          ...state,
          user: action.user,
        };
        case userConstants.CLEAR: 
          return {
            ...state,
            requestedForgot: false,
            requestingForgot: false

        }
    default:
      return state
  }
}