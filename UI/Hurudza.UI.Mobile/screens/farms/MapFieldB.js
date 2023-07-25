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
import { CustomButton } from '../../components/Buttons'

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
import { Divider, Skeleton, SearchBar, Button, Avatar } from '@rneui/themed';
import { useDrawerStatus } from '@react-navigation/drawer';

function MapFieldScreen({navigation, route}, props) {
    const drawerStatus = useDrawerStatus();
  const dispatch = useDispatch();
  const { type, message } = useSelector(state => state.alert);
  const windowWidth = Dimensions.get('window').width;
const windowHeight = Dimensions.get('window').height;
const height = windowWidth*0.7
const [isSaved, setIsSaved]= useState(false)

const [locations, setLocations] = useState([])
const [location, setLocation] = useState({
  longitude:31.143623,
  latitude:-17.768583,
  latitudeDelta: 0.0922,
  longitudeDelta: 0.0421
});
const [isp, setIsp]= useState(false)
const [errorMsg, setErrorMsg] = useState(null);
const locatePress = async () => {
      let { status } = await Location.requestForegroundPermissionsAsync();
      if (status !== 'granted') {
        setErrorMsg('Permission to access location was denied');
        return;
      }

      let location = await Location.getCurrentPositionAsync({        enableHighAccuracy: true,
        accuracy: Location.Accuracy.High,});
      locations.push({latitude:location.coords.latitude,longitude:location.coords.longitude, accuracy:location.coords.accuracy})
      setLocations(locations);
      console.warn(location)
      if(locations.length > 2)
      setIsp(true);
      setIsp(false);
      setIsp(true);

    }


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

  _handleNextClick =()=>{

  }
  _handleDeleteClick =(index)=>{
    locations.splice(0,1)
    if(locations.length > 2){
      console.log(locations)
    }

  }
  _renderPositionPolygon = ()=>{
    return(
      <Polygon coordinates={locations} title='the Maize field' fillColor={'rgba(100, 100, 200, 0.3)'}>
      </Polygon>
      
    )
  };
  _renderPositionMarkers = (position)=>{
    return(
      <Marker coordinate={position} title='the point'
      key={locations.indexOf(position)}>
      </Marker>
    )
  };
  _renderPositions = (position)=>{
    return(
      <ListItem
      key={locations.indexOf(position) +1}
      containerStyle={{backgroundColor:'#f1f2f2'}}
style={styles.historyItem}>
  <ListItem.Content>
    <View style={styles.historyItemContent}>
      <View style={styles.historyItemContentLeft}>
        <View style={{width: 50, justifyContent:'center', marginRight:5}}>
          <Icon
            type={'font-awesome-5'}
            name={'map-pin'} 
            size={30}
            color='rgba(00,00,00,0.7)' />


        </View> 
      </View>
      <View style={styles.historyItemContentMiddle}>
          <View style={styles.historyItemTitleContainer}>
            <Text style={styles.historyItemTitle}>Field Point {locations.indexOf(position) +1}</Text>
          </View>
          <Text style={styles.historyItemSubtitle}>longitude: {position.longitude}  latitude: {position.latitude}</Text>
        </View>
        <View style={{backgroundColor:'', justifyContent:'center', flexDirection:'row', alignItems:'center'}}>
          <TouchableOpacity style={{}}>
            <Icon name='edit' type='feather' color={'#52734D'} size={21}/>
          </TouchableOpacity>
          <TouchableOpacity style={{marginLeft:15}} onPress={()=>{_handleDeleteClick(1)}}>
            <Icon name='trash-2' type='feather' color={'red'} size={23}/>
          </TouchableOpacity>
        </View>
    </View>
    <Divider width={1} style={{width:'100%', alignSelf:'center', marginTop:20}} color='#52734D'/>
  </ListItem.Content>
</ListItem>
    )
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

            <Text style={styles.headerTitle}>Map Field Whatnot</Text>
            <View style={{ marginHorizontal:10, marginTop:30, alignItems:'center'}}>

              <View style={{width:windowWidth*0.95, height:windowWidth*0.4, backgroundColor:colors.lightGray2, borderRadius:15, overflow:'hidden'}}>
                <MapView style={{width:'100%', height:'100%'}}
                region={location}
                >
                {isp ?(
                <>
                  { locations.map(position => _renderPositionMarkers(position))}
                  </>
                  ):null
                  }
                {isSaved ?(
                <>
                  <Polygon coordinates={locations}  fillColor={'rgba(100, 100, 200, 0.3)'}></Polygon>
                  </>
                  ):null
                  }

                </MapView>
              </View>
              <Text style={{fontFamily:'montserrat-medium', fontSize:15}}>Representation of the field markers on the map</Text>
              <View>
              <Text style={{fontFamily:'montserrat-regular', fontSize:12, color:'grey'}}>Only available if connected to data.</Text>
              </View>

            </View>
            <View style={{flex:1}}>
            <View style={styles.history}>
                <View style={styles.historyTitleContainer}>
                  <View>
                    <Text style={styles.historyTitle}>Maize Field Points</Text>
                    <View style={{
                      width: 35,
                      height: 5,
                      backgroundColor: '#52734D',
                      marginHorizontal: 4,
                      marginTop: 3
                    }}></View>
                  </View>
                  <TouchableOpacity onPress={() => goToTransactions()}>
                    <Icon name='map-marked-alt' type='font-awesome-5' color={'#52734D'}/>
                  </TouchableOpacity>
                </View>
                <ScrollView style={{flex:1, marginTop:30, backgroundColor:colors.lightGray1}}>
                  {isp ?(
                                      <>
                                      { locations.map(position => _renderPositions(position))}
  </>
                  ):null


                  }

                  <TouchableOpacity onPress={()=>{locatePress()}}
                  style={{flexDirection:'row', justifyContent:'center', backgroundColor:'#f1f2f2', paddingVertical:15, borderRadius:5, alignItems:'center', marginTop:20}}>
                    <Icon name='add-location' color={'#52734D'} size={30}/>
                    <Text style={{color:'#52734D', fontFamily:'montserrat-medium', fontSize:20, marginLeft:15}}>Add position</Text>
                  </TouchableOpacity>
                </ScrollView>
              </View>
              </View>        
        </View>
        <TouchableOpacity
        style={{position:'absolute', bottom:20, right:10, backgroundColor:'green', paddingHorizontal:15, paddingVertical:10, borderRadius:10}} onPress={()=>setIsSaved(true)}>
          <Text style={{fontSize:20, fontFamily:'montserrat-semi', color:'#ffffff'}}>Save Field</Text>
        </TouchableOpacity>
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
    backgroundColor: colors.lightGray1,
    overflow:'hidden',
    width:'100%',
    paddingHorizontal:20,
    marginTop:30,
    flex:1
  },
  historyItemContent: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    width: '100%',
    backgroundColor: colors.lightGray1,
  },
  historyItemContentLeft: {
    flexDirection: 'row'
  },
  historyItemContentMiddle: {
  },
  historyTitleContainer: {
    flexDirection:'row',
    justifyContent:'space-between',
    backgroundColor: colors.lightGray1,
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
    marginRight:2,

  },
  historyItem: {
    marginBottom: 1,
  },
  historyItemTitleContainer:{
    flexDirection: 'row',
  },
  historyItemTitlePrefix: {
    fontWeight: '100'
  },
  historyItemTitle:{
    fontSize: 15,
    fontWeight: Platform.OS === 'ios' ? '500' : '600',
    fontFamily:'montserrat-medium'
    
  },
  historyItemSubtitle: {
    fontSize: Platform.OS === 'ios' ? 12 : 11,
    color: Platform.OS === 'ios' ? colors.secondaryTextGrey : colors.secondaryTextGrey,
    fontWeight: Platform.OS === 'ios' ? '200' : '100',
    fontFamily:'montserrat-regular'
  },
  historyItemRightText:{
    fontSize: 13,
    fontWeight: Platform.OS === 'ios' ? '500' : '600',
    alignSelf: 'center',
    fontFamily:'montserrat-medium',
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

export default connectAlert(MapFieldScreen);
