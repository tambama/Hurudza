const initialState = {

    user: {
      company: null,
      accountType:null,
      defaultCurrency:null
    },
  }
  
  export function signup(state = initialState, action){
    switch(action.type) {
      case 'UPDATEUSER':
        return {
          user:action.user,
        };
      default:
        return state;
    }
  }