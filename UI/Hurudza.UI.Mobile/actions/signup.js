import { userConstants } from '../constants/user';
import { userService } from '../services/user';
import { alertActions } from './alert';
import AsyncStorage from "@react-native-async-storage/async-storage";

{//export const Init = () => {
  //return async dispatch => {
   // let token = await AsyncStorage.getItem('token');
   // if (token !== null) {
  //    console.log('token fetched');
   //   dispatch({
  //      type: 'LOGIN',
  //      payload: token,
  //    })
  //  }
 // }
//}
}

export const Register1 = (userDetails) => {
  return async dispatch => {
    dispatch({
      type: 'UPDATEUSER',
      user:userDetails
    })
  }
}
export const Logout = () => {
  return async dispatch => {
    await AsyncStorage.clear();
    dispatch({
      type: 'LOGOUT'
    })
  }
}