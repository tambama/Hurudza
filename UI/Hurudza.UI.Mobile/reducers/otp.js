import { userConstants } from '../constants/user';

const initialState = {
  otp: {
    otp: '',
    fullName: '',
    lastName: '',
    email: '',
    mobileNumber: '',
    password: '',
  },
  registering: false,
  registered: false,
  loading: false
}

export function otp(state = initialState, action) {
  switch (action.type) {
    case userConstants.CREATE_OTP_REQUEST:
      return { ...state, registering: true };
    case userConstants.CREATE_OTP_SUCCESS:
      return { ...state, registering: false, registered: true, otp: action.otp};
    case userConstants.CREATE_OTP_FAILURE:
      return { ...state, registering: false };
      case userConstants.GET_OTP_REQUEST:
        return {
          loading: true
        };
      case userConstants.GET_OTP_SUCCESS:
        return {
          otp: action.otp,
          loading: false
        };
      case userConstants.GET_OTP_FAILURE:
        return { 
          error: action.error,
          loading: false
        };
    case userConstants.CLEAR:
      return {
        ...state,
        registering: false,
        registered: false,
        loading: false
      }
      case userConstants.CLEAR_OTP:
        return {
          ...state,
          otp: {
            otp: '',
            fullName: '',
            lastName: '',
            email: '',
            mobileNumber: '',
            password: '',
          },
        }
    default:
      return state
  }
}