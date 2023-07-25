import React, { useEffect, useRef, useState } from 'react';
import CircularProgress from 'react-native-circular-progress-indicator';
import {
  Image,
  Platform,
  StyleSheet,
  Text,
  View,
  TouchableHighlight,
  TouchableOpacity,
  StatusBar,
  SafeAreaView,
  ActivityIndicator,
  TextInput,
  ScrollView,    
  Modal,
  Pressable
} from 'react-native';
import { connect, useSelector } from 'react-redux';
import { Ionicons, SimpleLineIcons, FontAwesome, MaterialCommunityIcons, FontAwesome5 } from '@expo/vector-icons'
import { getStatusBarHeight, isIphoneX, ifIphoneX } from 'react-native-iphone-x-helper'
import * as Permissions from 'expo-permissions';
import Constants from 'expo-constants';
import { ListItem, Icon, Badge, Input } from 'react-native-elements';
import moment from 'moment';
import { LinearGradient } from 'expo-linear-gradient';
import {Dimensions} from 'react-native';

import { Pagination } from 'react-native-snap-carousel';
import RNPickerSelect from 'react-native-picker-select';
import ActionSheet from "react-native-actions-sheet";

import MapView, { Callout, Marker, Polygon } from 'react-native-maps';
import * as Location from 'expo-location';


import colors from '../../constants/Colors';
import DropdownAlert from 'react-native-dropdownalert';
import { userActions } from '../../actions/user';
import { Routes } from '../../navigation/Routes';
import { useDispatch } from 'react-redux';
import { connectAlert } from '../../components/Alert';
import { getNetwork } from '../../helpers/mobile-numbers';
import { onlyUnique } from '../../helpers/helper-methods';
import { Divider, Skeleton, SearchBar, Button } from '@rneui/themed';
import { useDrawerStatus } from '@react-navigation/drawer';

function FarmDetailsScreen({navigation, route}, props) {
    const drawerStatus = useDrawerStatus();
  const dispatch = useDispatch();
  const { type, message } = useSelector(state => state.alert);
  const windowWidth = Dimensions.get('window').width;
const windowHeight = Dimensions.get('window').height;
const height = windowWidth*0.7


  const [index, setIndex] = useState(0);
  const [nestedScrollEnabled, setNestedScrollEnabled] = useState(false);
  const [farmAvailable, setFarmAvailable]= useState(false);
  const [modalVisible, setModalVisible] = useState(false);

  const dropDownAlertRef = useRef(null);


  useEffect(() => {
    if(type === 'alert-danger'){
      dropDownAlertRef.current.alertWithType('error', 'Error', message);
      dispatch(alertActions.clear());
    }
    
    if(type === 'alert-success'){
      dropDownAlertRef.current.alertWithType('success', 'Success', message); 
      dispatch(alertActions.clear());
    }
  }, [type, message]);

  return (
    <View style={styles.container}>
      <StatusBar barStyle='dark-content' backgroundColor={Platform.OS === 'ios' ? colors.white : colors.lightGray1} />
        <View style={styles.welcomeContainer}>
            <View style={{flexDirection:'row', alignItems:'center'}}>
                <TouchableOpacity onPress={() => navigation.openDrawer()}>
                <Icon name='menu' size={35}/>
                </TouchableOpacity>

            </View>
        </View>
        <View style={styles.contentContainer}>
        <Text style={styles.headerTitle}>Nyazura Adventist High School</Text>
        <View style={{...styles.historyTitleContainer, backgroundColor:'transparent'}}>
                  <View>
                    <Text style={styles.historyTitle}>Farm Description</Text>
                    <View style={{
                      width: 20,
                      height: 3,
                      backgroundColor: '#52734D',
                      marginHorizontal: 4,
                      marginTop: 3
                    }}></View>
                  </View>
                  
                    <Text style={styles.historyTitleRight}></Text>
                  
                </View>
            <View style={{alignItems:'center', width:'100%'}}>
                
            <View style={{flexDirection:'row', justifyContent:'space-around', marginBottom:30}}>
                <View style={{width:'70%', backgroundColor:'transparent', flexDirection:'row', justifyContent:'space-between', paddingHorizontal:20, borderRightWidth:0.5, alignItems:'center'}}>
                    <View style={{}}>
                    <Text style={{fontSize:15, fontFamily:'montserrat-medium'}}>Very arable land with a dynamic structure, placed in a hilly area with the right soil type around for the available farms and people can till the land kusvikira vaneta, uye vasingachade kuita izvozvo.</Text>
                    </View>

                </View>
                <View style={{width:'30%', backgroundColor:'transparent', flexDirection:'row', justifyContent:'space-between', paddingHorizontal:25}}>
                    <View style={{}}>
                    <Text style={{fontSize:40, fontFamily:'montserrat-medium'}}>30,54</Text>
                    <Text style={{fontSize:19, fontFamily:'montserrat-regular'}}><Text style={{color:'#52734D'}}>hacteres</Text></Text>

                    </View>


                </View>
            </View>
            </View>
            <View style={{flexDirection:'row', justifyContent:'space-between'}}>
            <View style={{...styles.history, width:'55%'}}>
                <View style={styles.historyTitleContainer}>
                  <View>
                    <Text style={styles.historyTitle}>Fields</Text>
                    <View style={{
                      width: 20,
                      height: 3,
                      backgroundColor: '#52734D',
                      marginHorizontal: 4,
                      marginTop: 3
                    }}></View>
                  </View>
                  <TouchableOpacity onPress={() => goToTransactions()}>
                    <Text style={styles.historyTitleRight}>View All</Text>
                  </TouchableOpacity>
                </View>
                <View style={{alignItems:'center', justifyContent:'center', height:windowWidth*0.5, marginTop:30}}><Text style={{fontFamily:'montserrat-medium', fontSize:16, marginTop:-100, textAlign:'center', marginHorizontal:30}}>No fields</Text></View>
                <TouchableOpacity style={{position:'absolute', bottom:0, height:50, backgroundColor:'#52734D', width:'100%', alignItems:'center', justifyContent:'center', flexDirection:'row'}} onPress={()=>{navigation.navigate(Routes.AddFieldScreen)}}>
                    <Icon name='plus' type='feather' color={'#ffffff'}/>
                    <Text style={{fontFamily:'montserrat-semi', color:'#ffffff'}}> Add field</Text>
                </TouchableOpacity>
              </View>
              <View style={{width:'40%', backgroundColor:'#f1f2f2'}}>
                <View style={{flexDirection:'row', justifyContent:'space-between'}}>
                <View style={{width:'45%', height:150, backgroundColor:'#52734D', alignItems:'center', justifyContent:'space-between', borderRadius:10, paddingVertical:20}}>
                    <Text style={{fontSize:50, color:'white'}}>35</Text>
                    <Text style={{fontSize:16, color:'white'}}>Projects</Text>
                </View>
                <View style={{width:'45%', height:150, backgroundColor:'#91C788', alignItems:'center', justifyContent:'space-between', borderRadius:10, paddingVertical:20}}>
                    <Text style={{fontSize:50, color:'white'}}>3</Text>
                    <Text style={{fontSize:16, color:'white', textAlign:'center'}}> completed</Text>
                </View>
                </View>
                <View style={{height:20, backgroundColor:'transparent'}}/>
                <View style={{...styles.history, flex:1, width:'100%', backgroundColor:'#DDFFBC'}}>
                <View style={{...styles.historyTitleContainer, backgroundColor:'#DDFFBC'}}>
                  <View>
                    <Text style={styles.historyTitle}>This quarter</Text>
                    <View style={{
                      width: 20,
                      height: 3,
                      backgroundColor: '#52734D',
                      marginHorizontal: 4,
                      marginTop: 3
                    }}></View>
                  </View>
                    <Text style={styles.historyTitleRight}></Text>

                </View>
                <View style={{marginTop:15, paddingHorizontal:15, justifyContent:'space-between', flex:1, paddingBottom:15, backgroundColor:'#DDFFBC'}}>
                    <View/>
                    <Text style={{fontSize:30, fontFamily:'montserrat-medium', width:200}}>Made 3 requests for farming inputs</Text>
                    <View style={{flexDirection:'row', alignItems:'center'}}>
                    <CircularProgress
                    value={33.33}
                    activeStrokeWidth={8}
                    progressValueColor={'transparent'}
                    radius={15}
                    activeStrokeColor={'green'}
                    />
<Text style={{fontFamily:'montserrat-regular', fontSize:15}}>   1/3 Completed</Text>
                    </View>

                </View>

              </View>

              </View>
            </View>
            <View style={{flexDirection:'row', justifyContent:'space-between', marginTop:30}}>
            <View style={styles.history}>
                <View style={styles.historyTitleContainer}>
                  <View>
                    <Text style={styles.historyTitle}>Inventory</Text>
                    <View style={{
                      width: 20,
                      height: 3,
                      backgroundColor: '#52734D',
                      marginHorizontal: 4,
                      marginTop: 3
                    }}></View>
                  </View>
                  <TouchableOpacity onPress={() => goToTransactions()}>
                    <Text style={styles.historyTitleRight}>View All</Text>
                  </TouchableOpacity>
                </View>
                <View style={{alignItems:'center', justifyContent:'center', height:250, marginTop:30}}><Text style={{fontFamily:'montserrat-medium', fontSize:16, marginTop:-100, textAlign:'center', marginHorizontal:30}}>No inventory registered</Text></View>
                <TouchableOpacity style={{position:'absolute', bottom:0, height:50, backgroundColor:'#52734D', width:'100%', alignItems:'center', justifyContent:'center', flexDirection:'row'}}>
                    <Icon name='plus' type='feather' color={'#ffffff'}/>
                    <Text style={{fontFamily:'montserrat-semi', color:'#ffffff'}}> Add inventory</Text>
                </TouchableOpacity>
              </View>
              
              <View style={styles.history}>
                <View style={styles.historyTitleContainer}>
                  <View>
                    <Text style={styles.historyTitle}>Farm managers</Text>
                    <View style={{
                      width: 20,
                      height: 3,
                      backgroundColor: '#52734D',
                      marginHorizontal: 4,
                      marginTop: 3
                    }}></View>
                  </View>
                  <TouchableOpacity onPress={() => goToTransactions()}>
                    <Text style={styles.historyTitleRight}>View All</Text>
                  </TouchableOpacity>
                </View>
                <View style={{alignItems:'center', justifyContent:'center', height:250, marginTop:30}}><Text style={{fontFamily:'montserrat-medium', fontSize:16, marginTop:-100, textAlign:'center', marginHorizontal:30}}>No farm managers registered</Text></View>
                <TouchableOpacity style={{position:'absolute', bottom:0, height:50, backgroundColor:'#52734D', width:'100%', alignItems:'center', justifyContent:'center', flexDirection:'row'}}>
                    <Icon name='plus' type='feather' color={'#ffffff'}/>
                    <Text style={{fontFamily:'montserrat-semi', color:'#ffffff'}}> Add manager</Text>
                </TouchableOpacity>
              </View>
            </View>

            
        </View>
        <DropdownAlert inactiveStatusBarStyle='dark-content' inactiveStatusBarBackgroundColor={Platform.OS === 'ios' ? colors.white : colors.lightGray1} ref={dropDownAlertRef} />
      </View>
  ); 
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: colors.lightGray1,
  },
  welcomeContainer: {
    paddingTop: Platform.OS === 'ios' ? Constants.statusBarHeight +20 : 30,
    flexDirection:'row',
    justifyContent: 'space-between',
    alignItems: 'flex-end',
    paddingBottom: 5,
    marginHorizontal:30,
  },
  headerTitle: {
    fontSize: 30,
    fontWeight: '500',
    fontFamily:'montserrat-semi',
  },
  notificationsContainer:{
    width:50, 
    height:50, 
    borderRadius:50, 
    marginRight:15, 
    marginBottom: -10,
    alignItems:'center', 
  },
  notificationsIconContainer:{
    flex:1,
    width:23,
    height:23
  },
  notificationsIcon: {
    width:23,
    height:23,
    flex:1,
    resizeMode:"contain"
  },
  notificationBadge: {
    backgroundColor: colors.deepRed,
    width:25,
    height:25,
    borderRadius:25,
    justifyContent: 'center',
    alignItems: 'center',
    position: 'absolute',
    top:-3,
    left:-3
  },
  contentContainer: {
    marginHorizontal:30,
    marginTop: Platform.OS === 'ios' ? 25 : 30,
    flex:1
  },
  walletsCarouselContainer: {
    marginBottom:0,
    alignItems: 'center',
    height: '38%'
  },
  quickOperations:{
    height: '25%',
    paddingTop: 10,
    marginBottom: 10,
    backgroundColor: colors.white,
    borderRadius: 15
  },
  quickOperationsTitle:{
    paddingHorizontal: 15,
    marginTop: 5,
    fontSize:18,
    fontWeight: '500',
    fontFamily:'montserrat-semi'
  },
  operations:{
    height: 100,
    flexDirection:"row",
    // justifyContent:'space-between'
  },
  quickOperation:{
    height:'100%',
    marginBottom:10,
    justifyContent:'space-between',
    alignItems: 'center',
    padding:15
  },
  modalView: {
    margin: 20,
    backgroundColor: 'white',
    borderRadius: 20,
    padding: 35,
    alignItems: 'center',
    shadowColor: '#000',
    shadowOffset: {
      width: 0,
      height: 2,
    },
    shadowOpacity: 0.25,
    shadowRadius: 4,
    elevation: 5,
  },
  button: {
    borderRadius: 20,
    padding: 10,
    elevation: 2,
  },
  buttonOpen: {
    backgroundColor: '#F194FF',
  },
  buttonClose: {
    backgroundColor: '#2196F3',
  },
  textStyle: {
    color: 'white',
    fontWeight: 'bold',
    textAlign: 'center',
  },
  modalText: {
    marginBottom: 15,
    textAlign: 'center',
  },
  quickOperationIcon:{
    height: 70,
    width: 70,
    borderRadius:5,
    backgroundColor: colors.dohweBlue,
    flexDirection: 'row',
    justifyContent: 'center',
    alignItems: 'center'
  },
  quickOperationTextContainer:{
    marginTop: 5,
    flexDirection: 'row',
    justifyContent: 'center'
  },
  quickOperationText:{ 
    color:colors.black,
    fontSize: 12,
    fontFamily:'montserrat-regular'
  },
  history:{
    borderRadius: 15,
    backgroundColor:'white',
    overflow:'hidden',
    width:'47%',
  },
  historyItemContent: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    width: '100%',
    backgroundColor:'white'
  },
  historyItemContentLeft: {
    flexDirection: 'row'
  },
  historyItemContentMiddle: {
    marginLeft: 5
  },
  historyTitleContainer: {
    flexDirection:'row',
    justifyContent:'space-between',
    paddingHorizontal: 15,
    backgroundColor: 'white',
    paddingTop: 5,
    marginTop: 5,
  },
  historyTitle:{
    fontSize:18,
    fontWeight: Platform.OS === 'ios' ? '500' : '500',
    fontFamily:'montserrat-semi'
  },
  historyTitleRight: {
    fontWeight: Platform.OS === 'ios' ? '200' : '100',
    fontFamily:'montserrat-medium',
    color:'#52734D',
    marginRight:2
  },
  historyItem: {
    height: Platform.OS === 'ios' ? 65 : 50,
    
  },
  historyItemTitleContainer:{
    flexDirection: 'row',
    marginBottom: 4
  },
  historyItemTitlePrefix: {
    fontWeight: '100'
  },
  historyItemTitle:{
    fontSize: 18,
    fontWeight: Platform.OS === 'ios' ? '500' : '300',
    fontFamily:'montserrat-medium'
  },
  historyItemSubtitle: {
    fontSize: Platform.OS === 'ios' ? 14 : 14,
    color: Platform.OS === 'ios' ? colors.secondaryTextGrey : colors.secondaryTextGrey,
    fontWeight: Platform.OS === 'ios' ? '200' : '100',
    fontFamily:'montserrat-regular'
  },
  historyItemRightText:{
    fontSize: 16,
    fontWeight: Platform.OS === 'ios' ? '500' : '300',
    alignSelf: 'center',
    fontFamily:'montserrat-medium'
  },
  actionSheetContainer: {
    width: '100%',
    padding: 12,
    height: 300,
  },
  actionsContainer: {
    flexDirection: 'row',
    justifyContent: 'space-evenly',
    alignItems: 'center',
    marginBottom: 15,
  },
  action: {
    width: 60,
    height: 60,
    borderRadius: 10,
    backgroundColor: colors.lightGray1,
    flexDirection: 'column',
    justifyContent: 'center'
  },
  actionTitle: {
    alignSelf: 'center', 
    marginTop: 5, 
    fontSize: 14,
    fontWeight: '500',
    color: colors.primaryBlue
  },
  inputContainer: {
    borderColor:'transparent',
  },
  textInputContainer: {
    width: '100%',
    height: 50,
    borderRadius: 5,
    borderWidth: 1,
    borderColor: colors.lightGray2,
    marginVertical: 15,
    paddingHorizontal: 10,
    fontSize: 16,
  },
  textInput: {
    fontSize: 16
  },
  pickerContainer: {
    borderWidth: 1,
    borderColor: colors.lightGray1,
    borderRadius: 5
  },
  picker: {
    width: '100%',
    height: 50,
    paddingHorizontal: 10,
    fontSize: 16,
  },
  amountContainer: {
    flexDirection:'row',
    justifyContent: 'flex-end',
    marginBottom: 15,
  },
  actionTitle: {
    fontSize: 14,
    fontWeight: '500',
    color: colors.black
  },
  actionDetail: { 
    fontSize: 14,
    fontWeight: '300',
    color: colors.black,
    marginBottom: 5
  },
});

const pickerSelectStyles = StyleSheet.create({
  inputIOS: {
    fontSize: 16,
    paddingVertical: 12,
    paddingHorizontal: 10,
    borderWidth: 1,
    borderColor: colors.lightGray1,
    borderRadius: 4,
    color: 'black',
    paddingRight: 30, // to ensure the text is never behind the icon
  },
  inputAndroid: {
    fontSize: 16,
    paddingHorizontal: 10,
    paddingVertical: 8,
    borderWidth: 0.5,
    borderColor: colors.lightGray1,
    borderRadius: 8,
    color: 'black',
    paddingRight: 30, // to ensure the text is never behind the icon
  },
  iconContainer: {
    top: 15,
    right: 10,
  },
  itemContainer: {
    width: '100%',
    height: '80%',
    marginLeft: 0,
    marginRight: 0,
    marginTop: 20,
    paddingHorizontal: 0,
    borderRadius:16,
    alignSelf: 'flex-start',
    justifyContent: 'space-between',
    backgroundColor: colors.white,
    shadowColor: colors.shadowBlue,
    shadowOffset: {
      width: 0,
      height: 5,
    },
    shadowOpacity: 0.25,
    shadowRadius: 3.84,
    elevation: 5,
  },
});

export default connectAlert(FarmDetailsScreen);
