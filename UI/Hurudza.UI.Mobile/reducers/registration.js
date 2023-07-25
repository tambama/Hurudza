import { userConstants } from '../constants/user';

const initialState = {
  user: {
    name:'',
    accountType:null,
    acceptTermsAndConditions:true
  },
  registering: false,
  registered: false,
}

export function registration(state = initialState, action) {
  switch (action.type) {
    case userConstants.UPDATE_REGISTRATION_SUCCESS: 
    return {
      ...state,
      user: action.user
  }
    case userConstants.REGISTER_REQUEST:
      return { ...state, registering: true };
    case userConstants.REGISTER_SUCCESS:
      return { ...state, registering: false, registered: true, user: action.user};
    case userConstants.REGISTER_FAILURE:
      return { ...state, registering: false};
    case userConstants.CLEAR:
      return {
        ...state,
        user: {
          companyName:'',
          accountType:'',
          defaultCurrency: '',
          adminName: '',
          adminNumber: '',
          adminEmail: '',
          adminPassword: '',
        },
        registering: false,
        registered: false,
      }
    default:
      return state
  }
}