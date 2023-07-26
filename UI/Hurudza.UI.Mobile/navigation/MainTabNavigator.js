import React from 'react';
import { Platform, View, StyleSheet, Text } from 'react-native';
import { createBottomTabNavigator} from '@react-navigation/bottom-tabs';
import { createMaterialTopTabNavigator } from '@react-navigation/material-top-tabs';
import { createStackNavigator } from '@react-navigation/stack';
import 'react-native-gesture-handler';
import { SimpleLineIcons, MaterialIcons, Ionicons } from '@expo/vector-icons';
import { Icon } from "react-native-elements";
import { DrawerContent, createDrawerNavigator } from '@react-navigation/drawer';
import { NavigationContainer } from '@react-navigation/native';


const Drawer = createDrawerNavigator();
// Home
import HomeScreen from '../screens/home/DashboardScreen';

// Profile
import ProfileScreen from '../screens/profile/ProfileScreen';

import Header from '../components/Header'

import colors from '../constants/Colors';

import { Routes } from './Routes';
import FarmsScreen from '../screens/farms/FarmsScreen';
import AddFarmScreen from '../screens/farms/AddFarmScreen';
import FarmDetailsScreen from '../screens/farms/FarmDetailsScreen';
import AddFieldScreen from '../screens/farms/AddFieldScreen';
import MapFieldScreen from '../screens/farms/MapFieldScreen';
import CustomDrawer from '../components/CustomDrawer';
import InventoryScreen from '../screens/fields/InventoryScreen';
import AddInventoryScreen from '../screens/fields/AddInventoryScreen';


const Home = createStackNavigator();
const ActivityTab = createMaterialTopTabNavigator();


function Activity() {
  return (
    <ActivityTab.Navigator lazy={true}>
      <ActivityTab.Screen name={Routes.All} component={AllScreen} />
      <ActivityTab.Screen name={Routes.Received} component={ReceivedScreen} />
      <ActivityTab.Screen name={Routes.Sent} component={SentScreen} />
    </ActivityTab.Navigator>
  );
}

function HomeStack(){
  return (
    <Home.Navigator headerMode='screen'>
      <Home.Screen name={Routes.Dashboard} component={HomeScreen} options={{headerShown: false}} />
    </Home.Navigator>
  )
}
const Farms = createStackNavigator();
function FarmsStack(){
  return (
    <Farms.Navigator initialRouteName={Routes.FarmDetails}>
      <Farms.Screen name={Routes.Farms} component={FarmsScreen} options={{headerShown: false}}/>
      <Farms.Screen name={Routes.AddFarmScreen} component={AddFarmScreen} options={{headerShown: false}} />
      <Farms.Screen name={Routes.FarmDetails} component={FarmDetailsScreen} options={{headerShown: false}} />
      <Farms.Screen name={Routes.AddFieldScreen} component={AddFieldScreen} options={{headerShown: false}} />
      <Farms.Screen name={Routes.MapFieldScreen} component={MapFieldScreen} options={{headerShown: false}} />
    </Farms.Navigator>
  )
}
const Inventory = createStackNavigator();
function InventoryStack(){
  return (
    <Inventory.Navigator initialRouteName={Routes.Inventory}>
      <Inventory.Screen name={Routes.Inventory} component={InventoryScreen} options={{headerShown: false}}/>
      <Inventory.Screen name={Routes.AddInventory} component={AddInventoryScreen} options={{headerShown: false}}/>
    </Inventory.Navigator>
  )
}
const Profile = createStackNavigator();

function ProfileStack(){
  return (
    <Profile.Navigator>
      <Profile.Screen name={Routes.Profile} component={ProfileScreen} options={{headerShown: false}} />
    </Profile.Navigator>
  )
}

const Tab = createBottomTabNavigator();

{/*export function MainTabNaigator(){
  return (
    <Tab.Navigator tabBarOptions={{showLabel:false,labelStyle:{fontFamily:'montserrat-semi', fontSize:15}, style: {borderTopColor: colors.lightGray1, backgroundColor:'transparent'}}}>
      <Tab.Screen name={Routes.Home} component={HomeStack}
        options={({navigation}) => ({
          tabBarIcon: ({ focused }) => (
            <View>
              <Icon
              name='dashboard'
              type='material'
              size={20}
              color={focused ? 'teal' : 'grey'}
            />
            <Text style={{color:focused ?'teal' : 'grey', fontFamily:'montserrat-regular', fontSize:18}}>Dashboard</Text>
            </View>

          )})} />
         <Tab.Screen name={Routes.Farms} component={FarmsStack} 
        options={({navigation}) => ({
          tabBarIcon: ({ focused }) => (
            <View style={{alignItems:'center'}}>
              <Icon
              name='fence'
              type='material'
              size={25}
              color = {focused ? 'teal' : 'grey'}
            />
            <Text style={{color:focused ?'teal' : 'grey', fontFamily:'montserrat-regular', fontSize:18}}>Farms</Text>
            </View>
          )
        })}/>
          
      <Tab.Screen name={Routes.Profile} component={ProfileStack} 
        options={({navigation}) => ({
          tabBarIcon: ({ focused }) => (
            <View>
              <Icon
              name='person'
              type='material'
              size={25}
              color={focused ? 'teal' : 'grey'}
            />
            <Text style={{color:focused ?'teal' : 'grey', fontFamily:'montserrat-regular', fontSize:18}}>Profile</Text>
            </View>
          )
        })}/>

    </Tab.Navigator>
    
  )
}*/}
export function MainTabNavigator(){
  return (
    <Drawer.Navigator initialRouteName={Routes.InventoryStack} drawerContent={props =><CustomDrawer {...props}/>} screenOptions={{drawerType:'back', drawerStyle:{backgroundColor:'transparent'}, drawerActiveBackgroundColor:'rgba(82,115,87,0.3)', drawerActiveTintColor:'#52734D', drawerInactiveTintColor:'rgba(0,0,0,0.8)'}}>
    <Drawer.Screen name={Routes.HomeStack} component={HomeStack} options={({navigation}) => ({
       headerShown:false,
      drawerLabel:'Home',
      drawerLabelStyle:{fontFamily:'montserrat-bold', fontSize:20},
          drawerIcon: ({ focused }) => (
              <Icon
              name='home'
              type='feather'
              size={30}
              color={focused ? '#52734D' : 'grey'}
              style={{overflow:'hidden'}}
            />
          )})} />
    <Drawer.Screen name={Routes.Fields} component={FarmsStack} options={({navigation}) => ({
      headerShown:false,
      drawerLabel:'Fields',
      drawerLabelStyle:{fontFamily:'montserrat-bold', fontSize:20},
          drawerIcon: ({ focused }) => (
              <Icon
              name='list'
              type='feather'
              size={30}
              color={focused ? '#52734D' : 'grey'}
            />
          )})} />
    <Drawer.Screen name={Routes.InventoryStack} component={InventoryStack} options={({navigation}) => ({
      headerShown:false,
      drawerLabel:'Inventory',
      drawerLabelStyle:{fontFamily:'montserrat-bold', fontSize:20},
          drawerIcon: ({ focused }) => (
              <Icon
              name='box'
              type='feather'
              size={30}
              color={focused ? '#52734D' : 'grey'}
            />
          )})} />
        <Drawer.Screen name={'Projects'} component={FarmsStack} options={({navigation}) => ({
      headerShown:false,
      drawerLabel:'Projects',
      drawerLabelStyle:{fontFamily:'montserrat-bold', fontSize:20},
          drawerIcon: ({ focused }) => (
              <Icon
              name='grid'
              type='feather'
              size={30}
              color={focused ? '#52734D' : 'grey'}
            />
          )})} />
    <Drawer.Screen name={Routes.ProfileStack} component={ProfileStack} options={({navigation}) => ({
            headerShown:false,
      drawerLabel:'Profile',
      drawerLabelStyle:{fontFamily:'montserrat-bold', fontSize:20},
          drawerIcon: ({ focused }) => (
              <Icon
              name='user'
              type='feather'
              size={30}
              color={focused ? '#52734D' : 'grey'}
            />
          )})} />
  </Drawer.Navigator>
    
  )
}

const styles = StyleSheet.create({
  headerTitle: {
    fontFamily: 'montserrat-regular'
  },
  headerLeftContainerStyle: {
    backgroundColor: colors.lightGray1,
    borderRadius: 50,
    justifyContent: "center",
    paddingLeft: 0,
    marginLeft: 10
  }
})
