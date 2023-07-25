import AsyncStorage from '@react-native-async-storage/async-storage';
export async function authHeader() {
    // return authorization header with jwt token
    let user = JSON.parse(await AsyncStorage.getItem('user'));

    if (user && user.token) {
        return { 'Authorization': 'Bearer ' + user.token };
    } else {
        return {};
    }
}