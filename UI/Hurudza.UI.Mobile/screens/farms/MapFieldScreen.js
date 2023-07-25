import React, { useEffect, useRef, useState } from 'react';
import CircularProgress from 'react-native-circular-progress-indicator';
import {
  Image,
  Platform,
  StyleSheet,
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
import {Text, View} from 'react-native-animatable'

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
        accuracy: Location.Accuracy.BestForNavigation,});
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
  const [miniMenuOpen, setMiniMenuOpen] = useState(false)

  const dropDownAlertRef = useRef(null);
  const helpActionSheetRef = useRef(null);

  _handleHelpClick =()=>{
    helpActionSheetRef.current?.show();
  }

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
    let b = locations.indexOf(position)
    console.log(b);
    return(
      <ListItem
      key={locations.indexOf(position) +1}
      containerStyle={{backgroundColor:'#f1f2f2', marginTop:5}}
style={styles.historyItem}>
  <ListItem.Content>
    <View style={styles.historyItemContent}>
      <View style={{...styles.historyItemContentLeft}}>
        <View style={{width: 50, justifyContent:'center', marginRight:5}}>
              { /*         <Icon
                  type={'material'}
                  name={'location-pin'} 
                  size={30}
                  color='rgba(00,00,00,0.7)' />
              */}
              <Image source={require(`../../assets/marker${1}.png`)} style={{width:35,height:35}}/>
        </View> 
      </View>
      <View style={{...styles.historyItemContentMiddle,flex:1, alignItems:'center', justifyContent:'center'}}>
          <View style={{...styles.historyItemTitleContainer}}>
            <Text style={{...styles.historyItemTitle, textAlign:'center'}}>Field Point {locations.indexOf(position) +1}</Text>
          </View>
          <Text style={styles.historyItemSubtitle}>longitude: {position.longitude}      latitude: {position.latitude}</Text>
        </View>
        <View style={{justifyContent:'center', flexDirection:'row', alignItems:'center'}}>
          <TouchableOpacity style={{}}>
            <Icon name='edit' type='feather' color={'#52734D'} size={21}/>
          </TouchableOpacity>
          <TouchableOpacity style={{marginLeft:15}} onPress={()=>{_handleDeleteClick(1)}}>
            <Icon name='trash-2' type='feather' color={'#8C3333'} size={23}/>
          </TouchableOpacity>
        </View>
    </View>
  </ListItem.Content>
</ListItem>
    )
  };
  return (
    <View style={styles.container} onLayout={()=>{_handleHelpClick()}}>
      <StatusBar barStyle='dark-content' hidden backgroundColor={Platform.OS === 'ios' ? colors.white : colors.lightGray1} />
      <View style={{width:'100%', height:'100%'}}>
        <View style={{...styles.welcomeContainer, position:'absolute',top:0,elevation:5,zIndex:3000}}>
            <View style={{flexDirection:'row', alignItems:'center'}}>
                <TouchableOpacity onPress={() => navigation.openDrawer()} style={{backgroundColor:'#91C788', padding:15, borderRadius:50}}>
                <Icon name='menu' size={35}/>
                </TouchableOpacity>
            </View>
        </View>
        <View style={{width:'100%', height:'45%'}}>
              <MapView style={{width:'100%', height:'100%'}}
                region={location}>
              </MapView>
              <View style={{position:'absolute',width:'90%',height:60, elevation:10,bottom:35, backgroundColor:'#ffffff', alignSelf:'center', borderRadius:15, shadowColor:'#91C788', shadowOpacity:0.7, shadowRadius:1, shadowOffset:{width:0,height:1}}}>
          <View style={{width:'100%', justifyContent:'space-between',flexDirection:'row', height:'100%', alignItems:'center', paddingHorizontal:15}}>
            <View style={{flexDirection:'row', justifyContent:'center', alignItems:'center'}}>
              <Icon name='map-marker-path' type='material-community' size={35} color={'#52734D'}/>
            </View>
            <Text style={{marginLeft:15, fontFamily:'montserrat-medium', fontSize:13}}>Represantation of markers on map only available when connected to data.</Text>
            <TouchableOpacity onPress={()=>{_handleHelpClick()}}>
            <Icon name='help-circle-outline' type='material-community' size={35} color={'#52734D'}/>
            </TouchableOpacity>
          </View>
              </View>
        </View>
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

                  {!miniMenuOpen?(
                  <TouchableOpacity onPress={()=>{setMiniMenuOpen(!miniMenuOpen)}}>
                    <View animation ={'slideInLeft'} duration={600}>
                    <Icon name='appstore-o' type='ant-design' color={'#52734D'} size={30}/>
                    </View>

                  </TouchableOpacity>):
                  <View animation ={'slideInRight'} duration={600} style={{flexDirection:'row'}}>
                  <TouchableOpacity style={{flexDirection:'row', alignItems:'center'}}>
                    <Icon name='trash' type='font-awesome-5' size={18} color={'#8C3333'}/>
                    <Text style={{marginLeft:6, fontFamily:'montserrat-regular', fontSize:14, textDecorationLine:'underline'}}>Clear markings</Text>
                  </TouchableOpacity>
                  <TouchableOpacity style={{flexDirection:'row', alignItems:'center', marginLeft:10}}>
                    <Icon name='clockcircleo' type='ant-design' size={16} color={'#EA906C'}/>
                    <Text style={{marginLeft:6, fontFamily:'montserrat-regular', fontSize:14, textDecorationLine:'underline'}}>Finish later</Text>
                  </TouchableOpacity>
                  <TouchableOpacity onPress={()=>{setMiniMenuOpen(!miniMenuOpen)}} style={{marginLeft:20}}>
                    <Icon name='close' type='ant-design' color={'#52734D'} size={25}/>
                  </TouchableOpacity>
                  </View>

                  }


                </View>
                <ScrollView style={{flex:1, marginTop:30,marginBottom:70, backgroundColor:'#fffff', marginHorizontal:20}}>
                  {locations.length > 0 ?(
                                      <>
                                      { locations.map(position => _renderPositions(position))}
  </>
                  ):null
                  }
                  <TouchableOpacity onPress={()=>{locatePress()}}
                  style={{flexDirection:'row', justifyContent:'center', backgroundColor:'#f1f2f2', paddingVertical:15, alignItems:'center', marginTop:5}}>
                    <Icon name='add-location' color={'#52734D'} size={30}/>
                    <Text style={{color:'#52734D', fontFamily:'montserrat-medium', fontSize:18, marginLeft:15}}>Add position</Text>
                  </TouchableOpacity>
                </ScrollView>
                <TouchableOpacity style={{position:'absolute', bottom:0, height:60, backgroundColor:'#52734D', width:'100%', alignItems:'center', justifyContent:'center'}} onPress={()=>{_handleNextClick()}}>
                      <Text style={{color:'white', fontFamily:'montserrat-medium', fontSize:22, textAlign:'center'}}>Save field</Text>
                </TouchableOpacity>
        </View>


      </View>
        <DropdownAlert inactiveStatusBarStyle='dark-content' inactiveStatusBarBackgroundColor={Platform.OS === 'ios' ? colors.white : colors.lightGray1} ref={dropDownAlertRef} />
        <ActionSheet ref={helpActionSheetRef}>
          <View>
            <Text>lucas</Text>
          </View>
        </ActionSheet>
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
    borderTopLeftRadius: 15,
    borderTopRightRadius: 15,
    backgroundColor: '#ffffff',
    overflow:'hidden',
    width:'100%',
    marginTop:-25,
    flex:1,
    alignSelf:'center'
  },
  historyItemContent: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    width: '100%',
    backgroundColor: '#f1f2f2',
  },
  historyItemContentLeft: {
    flexDirection: 'row'
  },
  historyItemContentMiddle: {
    
  },
  historyTitleContainer: {
    flexDirection:'row',
    justifyContent:'space-between',
    backgroundColor: '#ffffff',
    paddingTop: 5,
    marginTop: 5,
    marginHorizontal:20,
    alignItems:'center'
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
