import React from 'react';
import { createStackNavigator } from '@react-navigation/stack';
import { MainTabNavigator } from './MainTabNavigator';
import { AuthNavigator } from './AuthNavigator';
import { useSelector } from 'react-redux';
import { Routes } from './Routes';

const Stack = createStackNavigator();

const Navigator = () => {
    const user = 2

    return (
        <>
        <Stack.Navigator>
                <>
                <Stack.Screen name={Routes.Auth} component={AuthNavigator} options={{headerShown: false}} />
                </>
            
            
        </Stack.Navigator>
        </>
    )
}

export default Navigator;
