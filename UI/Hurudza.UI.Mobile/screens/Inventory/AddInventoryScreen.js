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
import * as Location from 'expo-location';
import Constants from 'expo-constants';
import { ListItem, Icon, Badge, Input } from 'react-native-elements';
import moment from 'moment';
import { LinearGradient } from 'expo-linear-gradient';
import {Dimensions} from 'react-native';
import { CustomButton } from '../../components/Buttons'

import { Pagination } from 'react-native-snap-carousel';
import RNPickerSelect from 'react-native-picker-select';
import ActionSheet from "react-native-actions-sheet";

import * as SQLite from 'expo-sqlite';


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
import { value } from 'react-native-extended-stylesheet';

function AddInventoryScreen({navigation, route}, props) {
  const db = SQLite.openDatabase('HurudzaTest.db');

    const drawerStatus = useDrawerStatus();
  const dispatch = useDispatch();
  const { type, message } = useSelector(state => state.alert);
  const windowWidth = Dimensions.get('window').width;
const windowHeight = Dimensions.get('window').height;
const height = windowWidth*0.7


  const [index, setIndex] = useState(0);
  const [nestedScrollEnabled, setNestedScrollEnabled] = useState(false);
  const [modalVisible, setModalVisible] = useState(false);
  const [equipment, setEquipment] = useState([]);
  const [itemName, setItemName] = useState(undefined);
  const [itemCount, setItemCount] = useState(undefined);
  const [itemModel, setItemModel] = useState(undefined);
  const [itemSerial, setItemSerial] = useState(undefined);
  const [isLoading, setIsLoading] = useState(true);

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
  useEffect(() => {
    db.transaction(tx => {
        tx.executeSql(
          'CREATE TABLE IF NOT EXISTS farm_equipment (' +
          'id INTEGER PRIMARY KEY AUTOINCREMENT, ' +
          'item_name TEXT NOT NULL, ' +
          'model_type TEXT, ' +
          'item_count INTEGER NOT NULL, ' +
          'serial_number TEXT NOT NULL)'
        );
      });
      db.transaction(tx => {
        tx.executeSql('SELECT * FROM farm_equipment', null,
          (txObj, resultSet) => setEquipment(resultSet.rows._array),
          (txObj, error) => console.log(error)
        );
      });
    setIsLoading(false);
  }, []);
  _handleAddItem =(event)=>{
    if(itemName === undefined || itemName === null || itemName === ''){
      dropDownAlertRef.current.alertWithType('error', 'Error', 'Please enter the item name');
      return;
    }
    if(itemCount === undefined || itemCount === null || itemCount === ''){
      dropDownAlertRef.current.alertWithType('error', 'Error', 'Please enter the item count');
      return;
    }
    if(itemSerial === undefined || itemSerial === null || itemSerial === ''){
      dropDownAlertRef.current.alertWithType('error', 'Error', 'Please enter the item serial number');
      return;
    }
    if(itemModel === undefined || itemModel === null || itemModel === ''){
      dropDownAlertRef.current.alertWithType('error', 'Error', 'Please enter the item model');
      return;
    }
    db.transaction(tx => {
      tx.executeSql(
        'INSERT INTO farm_equipment (item_name, model_type, item_count, serial_number) VALUES (?, ?, ?, ?)',
        [itemName, itemModel, itemCount, itemSerial],
        (txObj, resultSet) => {
          if (resultSet.rowsAffected > 0) {
            console.log('Item added successfully');
            dropDownAlertRef.current.alertWithType('success', 'Success', 'Item added successfully');
            setItemName(undefined);
            setItemModel(undefined);
            setItemCount(undefined);
            setItemSerial(undefined);
            db.transaction(tx => {
              tx.executeSql('SELECT * FROM farm_equipment', null,
                (txObj, resultSet) => setEquipment(resultSet.rows._array),
                (txObj, error) => console.log(error)
              );
            });
          } else {
            console.log('Item could not be added');
          }
        },
        (txObj, error) => console.log('Error', error)
      );
    });
  }
  const showEquipment = () => {
    return equipment.map((item, index) => {
      return (
        <View key={index} style={styles.row}>
          <Text>{item.item_name} {item.id}yacho</Text>
          <Button title='Delete' onPress={() =>{}} />
          <Button title='Update' onPress={() => {}} />
        </View>
      );
    });
  };
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

            <Text style={styles.headerTitle}>Enter Equipment/Machinery details</Text>
            <View style={{backgroundColor:'#ffffff', paddingVertical:20, borderRadius:15, marginTop:30, overflow:'hidden', }}>
                <View style={{paddingHorizontal:25
                }}>
                <View style={{...styles.historyTitleContainer, backgroundColor:''}}>
                  <View>
                    <Text style={styles.historyTitle}>Item</Text>
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
                <Input
              style={styles.textInput}
              inputContainerStyle={styles.inputContainer}
              containerStyle={styles.textInputContainer}
              keyboardType='default'
              returnKeyType={'done'}
              onChangeText={(value) => setItemName(value)}
              onSubmitEditing={(value) => setItemName(value)}
              
            />
                  <View style={{...styles.historyTitleContainer, backgroundColor:''}}>
                  <View>
                    <Text style={styles.historyTitle}>Items count</Text>
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
                <Input
              style={styles.textInput}
              inputContainerStyle={styles.inputContainer}
              containerStyle={styles.textInputContainer}
              keyboardType='default'
              returnKeyType={'done'}
              type='number'
              cursorColor={'#52734D'}
              onChangeText={(value) => setItemCount(value)}
              onSubmitEditing={(value) => setItemCount(value)}
              
            />
                 <View style={{...styles.historyTitleContainer, backgroundColor:''}}>
                  <View>
                    <Text style={styles.historyTitle}>Model (If applicable)</Text>
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
                <Input
              style={styles.textInput}
              inputContainerStyle={styles.inputContainer}
              containerStyle={{...styles.textInputContainer}}
              keyboardType='default'
              returnKeyType={'done'}
              onChangeText={(value) => setItemModel(value)}
              onSubmitEditing={(value) => setItemModel(value)}
              cursorColor={'#52734D'}
            />
            <View style={{...styles.historyTitleContainer, backgroundColor:''}}>
                  <View>
                    <Text style={styles.historyTitle}>Serial number (If applicable)</Text>
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
                <Input
              style={styles.textInput}
              inputContainerStyle={styles.inputContainer}
              containerStyle={{...styles.textInputContainer, marginBottom:70}}
              keyboardType='default'
              returnKeyType={'done'}
              onChangeText={(value) => setItemSerial(value)}
              onSubmitEditing={(value) => setItemSerial(value)}
              cursorColor={'#52734D'}

              
            />
            </View>
            <TouchableOpacity style={{position:'absolute', bottom:0, height:60, backgroundColor:'#52734D', width:'100%', alignItems:'center', justifyContent:'center'}} onPress={()=>{_handleAddItem()}}>
                <Text style={{color:'white', fontFamily:'montserrat-medium', fontSize:22, textAlign:'center'}}>Add item</Text>
            </TouchableOpacity>
            </View>
            {showEquipment()}


            
        </View>
        <DropdownAlert inactiveStatusBarStyle='dark-content' inactiveStatusBarBackgroundColor={Platform.OS === 'ios' ? colors.white : colors.lightGray1} ref={dropDownAlertRef} />
      </View>
  ); 
}

const styles = StyleSheet.create({
  row: {
    flexDirection: 'row',
    alignItems: 'center',
    alignSelf: 'stretch',
    justifyContent: 'space-between',
    margin: 8
  },
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
    fontSize: 16,

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
  loginButton: {
    width: Platform.OS === 'ios' ? 350 : 500
  },
  loginButtonText: {
    fontWeight: '300',
    fontSize: 14,
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

export default connectAlert(AddInventoryScreen);
