import React from 'react';
import { createStackNavigator } from '@react-navigation/stack';

import { Routes } from './Routes'

import Welcome from '../screens/auth/Welcome';

const Auth = createStackNavigator();

export function AuthNavigator(){
  return (
    <Auth.Navigator initialRouteName={Routes.WelcomeScreen}>
      <Auth.Screen name={Routes.WelcomeScreen} component={Welcome} options={{headerShown: false}} />

    </Auth.Navigator>
  )
}