import React, { useEffect, useRef, useState } from 'react';
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
  ScrollView    
} from 'react-native';
import { connect, useSelector } from 'react-redux';
import { Ionicons, SimpleLineIcons, FontAwesome, MaterialCommunityIcons, FontAwesome5 } from '@expo/vector-icons'
import { getStatusBarHeight, isIphoneX, ifIphoneX } from 'react-native-iphone-x-helper'
import * as Permissions from 'expo-permissions';
import * as Location from 'expo-location';
import Constants from 'expo-constants';
import { ListItem, Icon, Button, Badge, Input } from 'react-native-elements';
import moment from 'moment';
import { LinearGradient } from 'expo-linear-gradient';
import {Dimensions} from 'react-native';

import { Pagination } from 'react-native-snap-carousel';
import RNPickerSelect from 'react-native-picker-select';
import ActionSheet from "react-native-actions-sheet";


import colors from '../../constants/Colors';
import DropdownAlert from 'react-native-dropdownalert';
import { userActions } from '../../actions/user';
import { Routes } from '../../navigation/Routes';
import { useDispatch } from 'react-redux';
import { connectAlert } from '../../components/Alert';
import { getNetwork } from '../../helpers/mobile-numbers';
import { onlyUnique } from '../../helpers/helper-methods';
import { Divider, Skeleton, SearchBar } from '@rneui/themed';

function FarmsScreen({navigation, route}, props) {

  const dispatch = useDispatch();
  const { type, message } = useSelector(state => state.alert);
  const windowWidth = Dimensions.get('window').width;
const windowHeight = Dimensions.get('window').height;
const height = windowWidth*0.7


  const [index, setIndex] = useState(0);
  const [nestedScrollEnabled, setNestedScrollEnabled] = useState(false);
  const [farmAvailable, setFarmAvailable]= useState(false);

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
            <Icon name='arrow-back-ios' type='material' size={30} style={{}}/>

          <View></View>
        </View>
        <View style={styles.contentContainer}>
        <Text style={styles.headerTitle}>New Farm</Text>
            <View style={{borderWidth:1, marginTop:30, borderRadius:15, overflow:'hidden', borderColor:colors.lightGray2}}>
                <View style={{backgroundColor:'transparent', paddingVertical:15, flexDirection:'row',alignItems:'center', paddingHorizontal:15, justifyContent:'center'}}>
                    <Text style={{fontFamily:'montserrat-medium', fontSize:18, textAlign:'center', alignSelf:'center', }}>Farm details</Text>
                    </View>
                    <View style={{backgroundColor:'white', paddingVertical:15, paddingHorizontal:15, flexDirection:'row', alignItems:'center', justifyContent:'space-between'}}>
            <View style={{alignItems:'center', width:'35%'}}>
            <Text style={{fontFamily:'montserrat-medium', fontSize:18}}>Farm name</Text>
            <Text style={{fontFamily:'montserrat-medium', fontSize:14, textAlign:'center', marginTop:5, color:'grey'}}>Enter the farm's name or the name of the owning institution.</Text>

            </View>
            <Input
              style={styles.textInput}
              inputContainerStyle={styles.inputContainer}
              containerStyle={styles.textInputContainer}
              placeholder="Enter Farm Name"
              placeholderTextColor={colors.placeholderTextColor}
              keyboardType='default'
              returnKeyType={'done'}
              rightIcon={<></> }
            />
            </View>
            <View style={{backgroundColor:'white', paddingVertical:15, paddingHorizontal:15, flexDirection:'row', alignItems:'center', justifyContent:'space-between'}}>
            <View style={{alignItems:'center', width:'35%'}}>
            <Text style={{fontFamily:'montserrat-medium', fontSize:18}}>Farm size</Text>
            <Text style={{fontFamily:'montserrat-medium', fontSize:14, textAlign:'center', marginTop:5, color:'grey'}}>Enter the farm's area in hacteres.</Text>

            </View>
            <Input
              style={styles.textInput}
              inputContainerStyle={styles.inputContainer}
              containerStyle={styles.textInputContainer}
              placeholder="Enter Farm Name"
              placeholderTextColor={colors.placeholderTextColor}
              keyboardType='default'
              returnKeyType={'done'}
              rightIcon={<></> }
            />
            </View>
            <View style={{backgroundColor:'white', paddingVertical:15, paddingHorizontal:15, flexDirection:'row', alignItems:'center', justifyContent:'space-between'}}>
            <View style={{alignItems:'center', width:'35%'}}>
            <Text style={{fontFamily:'montserrat-medium', fontSize:18}}>Farm description</Text>
            <Text style={{fontFamily:'montserrat-medium', fontSize:14, textAlign:'center', marginTop:5, color:'grey'}}>Give a short description of the information of the farm.</Text>

            </View>
            <Input
              style={styles.textInput}
              inputContainerStyle={{...styles.inputContainer}}
              containerStyle={{...styles.textInputContainer,height:100}}
              placeholder="Description"
              multiline
              numberOfLines={5}
              placeholderTextColor={colors.placeholderTextColor}
              keyboardType='default'
              returnKeyType={'done'}
              rightIcon={<></> }
            />
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
    paddingTop: Platform.OS === 'ios' ? Constants.statusBarHeight +10 : 5,
    flexDirection:'row',
    justifyContent: 'space-between',
    alignItems: 'flex-end',
    paddingBottom: 5,
    marginHorizontal:30,
  },
  headerTitle: {
    fontSize: 25,
    fontWeight: '500',
    paddingTop: 7,
    fontFamily:'montserrat-semi'
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
    marginTop: Platform.OS === 'ios' ? 25 : 20,
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
    height: Platform.OS === 'ios' ? '34%' : 100,
    borderRadius: 15,
    backgroundColor:'white',
    overflow:'hidden'
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
    fontFamily:'montserrat-regular'
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
    width: '60%',
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

export default connectAlert(FarmsScreen);
