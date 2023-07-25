import React, { useEffect, useState, useCallback } from 'react';
import { StyleSheet } from 'react-native';
import * as SplashScreen from 'expo-splash-screen';
import { Asset } from 'expo-asset';
import * as Font from 'expo-font';
import { Ionicons } from '@expo/vector-icons';
import { Provider } from 'react-redux';
import store from './config/store';
import { NavigationContainer } from '@react-navigation/native';
import Navigator from './navigation/Navigator';
import EStyleSheet from 'react-native-extended-stylesheet';
import { SafeAreaProvider} from 'react-native-safe-area-context';

import { userConstants } from './constants/user';

EStyleSheet.build({
  $primaryBlue: '#4F6D7A',
  $primaryOrange: '#D57A66',
  $primaryGreen: '#00BD9D',
  $primaryPurple: '#9E768F',
  $white: '#FFFFFF',
  $border: '#E2E2E2',
  $inputText: '#fff', 
  $lightGray: '#F0F0F0',
  $darkText: '#343434',
});

// Keep the splash screen visible while we fetch resources
SplashScreen.preventAutoHideAsync();

export default function App() {
  const [isLoadingComplete, setIsLoadingComplete] = useState(false);

  const _loadResourcesAsync = async () => {

    return Promise.all([
      Asset.loadAsync([
        require('./assets/images/robot-dev.png'),
        require('./assets/images/robot-prod.png'),
      ]),
      Font.loadAsync({
        // This is the font that we are using for our tab bar
        ...Ionicons.font,
        // We include SpaceMono because we use it in HomeScreen.js. Feel free
        // to remove this if you are not using it in your app,
        'montserrat-black': require('./assets/fonts/Montserrat-Black.ttf'),
        'montserrat-regular': require('./assets/fonts/Open_Sans/static/OpenSans-Regular.ttf'),
        'montserrat-medium': require('./assets/fonts/Open_Sans/static/OpenSans-Medium.ttf'),
        'montserrat-semi': require('./assets/fonts/Open_Sans/static/OpenSans-SemiBold.ttf'),
        'montserrat-bold': require('./assets/fonts/Open_Sans/static/OpenSans-Bold.ttf'),
        'space-mono': require('./assets/fonts/SpaceMono-Regular.ttf'),
        'opensans-bold': require('./assets/fonts/Open_Sans/static/OpenSans-Bold.ttf'),
        'Poppins-bold': require('./assets/fonts/Poppins/Poppins-SemiBold.ttf'),
      }),
      bootstrapAsync()
    ]);
  };

  const _handleLoadingError = error => {
    // In this case, you might want to report the error to your error
    // reporting service, for example Sentry
    console.warn(error);
  };

  const _handleFinishLoading = async () => {
    setIsLoadingComplete(true);
    await SplashScreen.hideAsync();
  };

  const bootstrapAsync = async () => {
    let user;
    try {
      user = JSON.parse(await AsyncStorage.getItem('user'));
    } catch (error) {
      // Restoring token failed
    }
    // After restoring token, we may need to validate it in production apps
    // This will switch to the App screen or Auth screen and this loading
    // screen will be unmounted and thrown away.
    if(user && user.token) store.dispatch({type:userConstants.RESTORE_TOKEN_SUCCESS, user})
  };

  useEffect(() => {
    async function prepare() {
      try {
        // Pre-load fonts, make any API calls you need to do here
        await _loadResourcesAsync()
        // Artificially delay for two seconds to simulate a slow loading
        // experience. Please remove this if you copy and paste the code!
        await new Promise(resolve => setTimeout(resolve, 2000));
      } catch (e) {
        _handleLoadingError(e)
      } finally {
        // Tell the application to render
        await _handleFinishLoading();
      }
    }

    prepare();
  }, []);

  const onLayoutRootView = useCallback(async () => {
    if (isLoadingComplete) {
      // This tells the splash screen to hide immediately! If we call this after
      // `setAppIsReady`, then we may see a blank screen while the app is
      // loading its initial state and rendering its first pixels. So instead,
      // we hide the splash screen once we know the root view has already
      // performed layout.
      await SplashScreen.hideAsync();
    }
  }, [isLoadingComplete]);

  if (!isLoadingComplete) {
    return null;
  }

  return (
    <Provider store={store} style={styles.container}>
      <SafeAreaProvider>
        <NavigationContainer>
          <Navigator/>
        </NavigationContainer>
      </SafeAreaProvider>
    </Provider>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#fff',
  },
});