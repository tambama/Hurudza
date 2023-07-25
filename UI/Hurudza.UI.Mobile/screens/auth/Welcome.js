import React, { useEffect, useRef, useState } from 'react'
import { Image, Button, Avatar, Icon, Badge, Dialog, CheckBox, Divider, Stack, Chip } from '@rneui/themed'
import { StatusBar } from 'expo-status-bar';
import RNPickerSelect from 'react-native-picker-select';
import Illustration from '../../assets/images/illustrations/team.svg';
import { 
  StyleSheet,
  TouchableOpacity, 
  Dimensions, 
  Platform, 
  KeyboardAvoidingView, 
  ScrollView,
  SafeAreaView,
  ImageBackground,
  Touchable,
}  from 'react-native'
import Constants from 'expo-constants';
import { Text, View } from 'react-native-animatable'
import colors from '../../constants/Colors';
var {width ,height}  = Dimensions.get('window')
import { getBottomSpace, ifIphoneX } from 'react-native-iphone-x-helper'
import { connect, useDispatch, useSelector } from 'react-redux'
import { Input} from 'react-native-elements'

import { CustomButton } from '../../components/Buttons'
import DropdownAlert from 'react-native-dropdownalert';

import { userActions } from '../../actions/user';
import { Register1 } from '../../actions/signup';
import { alertActions } from '../../actions/alert';
import { Routes } from '../../navigation/Routes';
import { connectAlert } from '../../components/Alert';
import { Ionicons } from '@expo/vector-icons';
import ActionSheet from 'react-native-actions-sheet';
import { color } from 'react-native-reanimated';
import { BackgroundImage } from '@rneui/base';

function WelcomeScreen({navigation, route}, props){
  const dispatch = useDispatch();

  const buttonRef = useRef(null);
  const linkRef = useRef(null);

  useEffect(() => {
    //dispatch(userActions.logout());
  }, []);



    _loginAsync = async () => {
      dispatch(userActions.login(userName, password));
      dispatch(alertActions.clear());
    };
  //Handle Login button click
  _handleLoginClick = () => {
  navigation.navigate(Routes.LoginScreen);
  }
  //Handle Next button click
  _handleNextClick = () => {
    navigation.navigate(Routes.RegisterAdmin);
  }
    const isIphone = Platform.OS === 'ios'
    const keyboardVerticalOffset = Platform.OS === 'ios' ? 40 : 0
    return ( 
      <View style={styles.container}>
        <StatusBar style='dark' />
        <View style={{marginTop:Platform.OS==='ios'?0:0}}></View>
        <View style={{height:'100%'}}>
                <SafeAreaView></SafeAreaView>
                <Image source={require('../../assets/images/illustrations/teams.png')} style={{width:'100%', height:300, resizeMode:'contain', marginTop:'25%'}}/>
                <View style={{alignItems:'center', marginBottom:80, marginTop:30}}>
                    <Text style={{...styles.headerTitle, textAlign:'center'}}>Keep your teams seamlessly connected.</Text>
                    <Text style={{...styles.subTitle, textAlign:'center', width:'70%', marginTop:5}}>You can schedule airtime topups and more for your teams</Text>
                </View>
                <View>
                <View ref={buttonRef} animation={'bounceIn'} duration={600} delay={400} style={{alignSelf:'center'}}>
      <CustomButton
        onPress={() => _handleNextClick()}
        isEnabled={true}
        buttonStyle={{...styles.loginButton, backgroundColor: colors.dohweBlue, fontFamily: 'montserrat-regular'}}
        textStyle={
          {
            ...styles.loginButtonText, 
            color:  colors.white,
            fontFamily: 'montserrat-medium',
            fontSize:20
          }
        }
        text={'Get started'}
      />
    </View>
    <View style={styles.websiteLink} ref={linkRef}>
        <Text style={styles.termsGreyText}>Already have an account?  </Text>
          <TouchableOpacity onPress={() => _handleLoginClick()}>
            <Text
              ref={(ref) => this.linkRef = ref}
              style={styles.termsText}
              animation={'fadeIn'}
              duration={600}
              delay={400}
            >
              {'Login'}
            </Text>
            <Divider style={styles.linkDivider} />
        </TouchableOpacity>
      </View>
                </View>


        </View>

     </View>
  )
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f1f2f2',
  },
  Bottomcontainer: {
    bottom:0,
    justifyContent:'center',
    backgroundColor: '#fff',
    alignItems:'center'
  },
  termsContainer: {
    width:'95%',
    marginBottom:10
  },
  welcomeContainer: {
    height: Platform.OS === 'ios' ? '14%' : 63,
    paddingTop: Platform.OS === 'ios' ? Constants.statusBarHeight : 5,
    flexDirection:'row',
    justifyContent: 'space-between',
    alignItems: 'flex-end',
    paddingBottom: 5
  },
  headerTitle: {
    fontSize: 30,
    fontWeight: '800',
    color:'#1c5db9',
    marginBottom:5,
    fontFamily:'montserrat-bold',
    marginHorizontal: 5
  },
  subTitle: {
    fontSize: 20,
    fontWeight: '400',
    color:'#003366',
    fontFamily:'montserrat-regular',
    marginHorizontal: 10
  },
  websiteLink: {
    flexDirection:'row',
    alignSelf: "center",
    alignItems:'center',
    alignContent: 'center',
    marginTop:10,
    ...ifIphoneX({
      marginBottom: getBottomSpace()
    }, {
        marginBottom: 15
    })
  },
  termsGreyText: {
    fontWeight: '300',
    textAlign:'center',
    fontFamily:'montserrat-regular',
    fontSize:14
  },
  termsText: {
    color:'#1c5db9',
    fontFamily: 'montserrat-regular',
    fontWeight: '100',
    fontSize: 14
  },
  logoImage: {
    width: 100,
    height: 100,
    marginTop:'25%',
    resizeMode: 'contain',
    marginLeft: -10,
    alignSelf:'center'
  },
  backgroundImage:{
    width:width,
    height:height,
    position:'absolute'
  },
  logoContainer: {
    justifyContent: 'center',
    alignItems: 'center',
    backgroundColor:'purple'
  },
  heroText: {
    paddingHorizontal:40,
    fontSize: 26,
    fontWeight: '400',
    marginBottom: 20,
  },
  formContainer: {
    alignItems: 'center',
    marginBottom:20
  },
  form: {
    alignItems: 'center',
    width: '100%',
    justifyContent:'center'
  },
  inputContainerStyle: {
    borderBottomColor:Platform.OS === 'ios' || Platform.OS === 'android' ? colors.lightGray1 : colors.textInputBorderGrey,
    width: 250,
    color: Platform.OS === 'ios' || Platform.OS === 'android' ? colors.black : colors.white,
  },
  inputTextStyle:{
    fontSize: 16,
    color:colors.black,
    fontFamily:'montserrat-medium'
  },
  inputLabel:{
    color: 'gray',
    fontWeight: '300',
    fontSize: 14,
    marginBottom:-5,
    fontFamily:'montserrat-regular'
  },
  forgotPasswordContainer: {
    alignSelf: 'flex-end',
    marginTop: -10,
    marginBottom: 30,
    paddingRight: 10
  },
  forgotPasswordText: {
    color: colors.activeBlue,
    fontFamily: 'montserrat-regular'
  },
  linkDivider: {
    backgroundColor: colors.activeBlue
  },
  haveAccountContainer:{
    flexDirection: 'row',
    justifyContent: 'center',
    marginTop: 20
  },
  haveAccountGreyText: {
    fontWeight: '100'
  },
  haveAccountText: {
    color: colors.black,
    fontFamily: 'montserrat-regular'
  },
  line: {
    height: 1,
    flex: 2,
    backgroundColor: 'black'
  },
  loginButton: {
    width: Platform.OS === 'ios' ? 318 : 290,
    height:62,
    borderRadius:15
  },
  loginButtonText: {
    fontWeight: '300',
    fontSize: 14,
  },
  signupLink: {
    color: colors.black, 
    alignSelf: 'center',
    padding: 20
  },
  websiteLinkText: {
    color: Platform.OS === 'ios' || Platform.OS === 'android' ? colors.black : colors.iconsGrey, 
    fontWeight:'300', 
    fontFamily: 'montserrat-regular'
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
    fontSize: 16,
    fontWeight: '500',
    color: '#003366'
  },

  // Dialog styles

  dialogBox:{
    alignItems:'center',
    justifyContent:'center',
  },

  dialogTitle:{
    fontSize: 20,
    fontWeight: '600',
    color:'#1c5db9',
    marginBottom:5,
    textAlign:'center'
  },
  dialogSelect:{
    backgroundColor:'red',
    marginTop:5
  },
  checkBoxText:{
    color:'#003366'
  }

}) 
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


export default connectAlert(WelcomeScreen);