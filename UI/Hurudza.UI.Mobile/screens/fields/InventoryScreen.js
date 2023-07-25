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
  Pressable,
  Alert
} from 'react-native';
import { connect, useSelector } from 'react-redux';
import { Ionicons, SimpleLineIcons, FontAwesome, MaterialCommunityIcons, FontAwesome5 } from '@expo/vector-icons'
import { getStatusBarHeight, isIphoneX, ifIphoneX } from 'react-native-iphone-x-helper'
import * as Permissions from 'expo-permissions';
import Constants from 'expo-constants';
import { ListItem, Icon, Badge, Input } from 'react-native-elements';
import moment from 'moment';
import {Dimensions} from 'react-native';

import { Pagination } from 'react-native-snap-carousel';
import RNPickerSelect from 'react-native-picker-select';
import ActionSheet from "react-native-actions-sheet";

import MapView, { Callout, Marker, Polygon } from 'react-native-maps';
import * as Location from 'expo-location';

import { Table, Row, Rows, TableWrapper, Col, Cols, Cell } from 'react-native-reanimated-table';


import colors from '../../constants/Colors';
import DropdownAlert from 'react-native-dropdownalert';
import { userActions } from '../../actions/user';
import { Routes } from '../../navigation/Routes';
import { useDispatch } from 'react-redux';
import { connectAlert } from '../../components/Alert';
import { getNetwork } from '../../helpers/mobile-numbers';
import { onlyUnique } from '../../helpers/helper-methods';
import { Divider, Skeleton, SearchBar, Button, CheckBox } from '@rneui/themed';
import { useDrawerStatus } from '@react-navigation/drawer';

function InventoryScreen({navigation, route}, props) {
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

  const [tableHead, setTableHead] = useState([['Inventory item', 'Condition', 'Quantity']]);
  const [tableData, setTableData] = useState([
    ['Hoe', 'Good', '10'],
    ['Rake', 'Good', '10'],
    ['Shovel', 'Good', '10'],
    ['Hoe', 'Bad', '3'],
    ['Rake', 'Bad', '1'],
    ['Shovel', 'Bad', '2'],
    ['Tractor', 'Good', '4'],
    ['Tipper', 'Bad', '1'],
    ['Axe', 'Good', '4'],
    ['Hoe', 'Good', '10'],
    ['Seeder v654.65', 'Good', '1'],
    ['Rain Gauge v643.53', 'Good', '10'],
    ['Harvester v654.65', 'Good', '1'],
    ['Perfo', 'Good', '10'],

  ])

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
  _alertIndex=(index)=> {
    Alert.alert(`This is row ${index + 1}`);
  }
  const [checkedAll, setCheckedAll] = useState(false);
  const element = (Celldata, index) => (
    <View style={{flexDirection:'row', alignItems:'center'}}>
        <CheckBox
           checked={checkedAll}
           iconType="material-community"
           checkedIcon="checkbox-marked"
           uncheckedIcon="checkbox-blank-outline"
           checkedColor="#52734D"
           size={30}
         />
         <Text style={{...styles.TableText, marginLeft:5, fontFamily:'montserrat-bold', color:'rgba(0,0,0,0.8)'}}>{Celldata}</Text>
    </View>

  );
  const titleElement = (Celldata, index) => (
    <View style={{flexDirection:'row', alignItems:'center'}}>
        <CheckBox
           checked={checkedAll}
           onPress={()=>setCheckedAll(!checkedAll)}
           iconType="material-community"
           checkedIcon="checkbox-marked"
           uncheckedIcon="checkbox-blank-outline"
           checkedColor="#52734D"
           size={30}
         />
         <Text style={{...styles.Tabletitle,marginLeft:5, }}>{Celldata}</Text>
    </View>

  );

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
        <Text style={styles.headerTitle}>Farm Inventory</Text>
        <View style={{...styles.historyTitleContainer, backgroundColor:'transparent'}}>
                  <View>
                    <Text style={styles.historyTitle}>Summary</Text>
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
                <View style={{width:'100%', backgroundColor:'transparent', flexDirection:'row', justifyContent:'space-between', paddingHorizontal:20, alignItems:'center'}}>
                    <View style={{}}>
                    <Text style={{fontSize:15, fontFamily:'montserrat-medium'}}>Total items registered : {tableData.length}</Text>
                    <Text style={{fontSize:15, fontFamily:'montserrat-medium'}}>Last inventory Update : {moment().calendar()}</Text>
                    </View>
                </View>
            </View>
            </View>

            <View style={{...styles.history, width:'100%', flex:1}}>
                <View style={styles.historyTitleContainer}>
                <SearchBar
                platform={ Platform.OS === 'ios' ? 'android' : 'android' }
                placeholder='Search by name, condition, etc.'
                inputStyle={{fontFamily:'montserrat-medium'}}
                onSubmitEditing={() => {}}
                spellCheck={false}
                autoCorrect={false}
                returnKeyType='search'
                containerStyle={ Platform.OS === 'ios' ? {
                  height: 50,
                  borderRadius: 15,
                } : {
                  width: '100%',
                }}
                inputContainerStyle={ 
                  Platform.OS === 'ios' ? {
                  backgroundColor: colors.white,
                  borderRadius:15
                } : null } />
                </View>
                <View style={{marginBottom:120, flex:1}}>
                        <Divider />
                        <Table borderStyle={{borderColor: 'transparent'}}>
                            <>
                            {
                                    tableHead.map((rowData, index) => (
                                    <>
                                    <TableWrapper key={index} style={styles.row}>
                                {
                                    rowData.map((cellData, cellIndex) => (
                                    <Cell key={cellIndex} flex={cellIndex === 0 ?3:1} data={cellIndex === 0 ? titleElement(cellData, index) : cellData} textStyle={styles.Tabletitle}/>
                                    ))
                                }
                                    </TableWrapper>
                                    <Divider />
                                    </>

                 
                                ))
                                }


                 

                            <Divider />
                            </>
                            <ScrollView style={{}}>

                            {
                                    tableData.map((rowData, index) => (
                                    <>
                                    <TableWrapper key={index} style={styles.row}>
                                {
                                    rowData.map((cellData, cellIndex) => (
                                    <Cell key={cellIndex} flex={cellIndex === 0 ?3:1} data={cellIndex === 0 ? element(cellData, index) : cellData} textStyle={styles.TableText}/>
                                    ))
                                }
                                    </TableWrapper>
                                    <Divider />
                                    </>

                 
                                ))
                                }
                             </ScrollView>
                        </Table>
                </View>

                <TouchableOpacity style={{position:'absolute', bottom:0, height:50, backgroundColor:'#52734D', width:'100%', alignItems:'center', justifyContent:'center', flexDirection:'row'}} onPress={()=>{navigation.navigate(Routes.AddFieldScreen)}}>
                    <Icon name='plus' type='feather' color={'#ffffff'}/>
                    <Text style={{fontFamily:'montserrat-semi', color:'#ffffff'}}> Add item</Text>
                </TouchableOpacity>
              </View>
        </View>
        <DropdownAlert inactiveStatusBarStyle='dark-content' inactiveStatusBarBackgroundColor={Platform.OS === 'ios' ? colors.white : colors.lightGray1} ref={dropDownAlertRef} />
      </View>
  ); 
}

const styles = StyleSheet.create({
    head: { height: 60, paddingLeft:15},
    TableText: { margin: 6, fontSize:16, fontFamily:'montserrat-semi', color:'rgba(0,0,0,0.8)' },
    Tabletitle: { margin: 6, fontSize:20, fontFamily:'montserrat-semi', color:'rgba(0,0,0,0.8)' },
    row: { flexDirection: 'row', height: 60, paddingLeft:15},
    btn: { width: 58, height: 18, backgroundColor: '#78B7BB',  borderRadius: 2 },
    btnText: { textAlign: 'center', color: '#fff' },
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

export default connectAlert(InventoryScreen);
