import React, { useEffect, useRef, useState } from 'react'
import { Image, Button, Avatar, Icon, Badge, Dialog, CheckBox, Divider } from '@rneui/themed'
import { StatusBar } from 'expo-status-bar';

import RNPickerSelect from 'react-native-picker-select';
import { 
  StyleSheet,
  TouchableOpacity, 
  Dimensions, 
  Platform, 
  KeyboardAvoidingView, 
  ScrollView,
}  from 'react-native'
import Constants from 'expo-constants';
import { Text, View } from 'react-native-animatable'
import colors from '../../constants/Colors';
var {width ,height}  = Dimensions.get('window')
import { getBottomSpace, ifIphoneX } from 'react-native-iphone-x-helper'
import { connect, useDispatch, useSelector } from 'react-redux'
import { Input} from 'react-native-elements'

import DropdownAlert from 'react-native-dropdownalert';

import { Routes } from '../../navigation/Routes';
import { connectAlert } from '../../components/Alert';
import { Ionicons } from '@expo/vector-icons';
import ActionSheet from 'react-native-actions-sheet';
import { ActionSheetRef } from "react-native-actions-sheet";

export default function Refund() {
    const definitionsViewRef = useRef(null);
    const [shown, setShown] = useState(true);
    const [shown2, setShown2] = useState(true);
    const [shown3, setShown3] = useState(true);
    const [shown4, setShown4] = useState(true);
    const [shown5, setShown5] = useState(false);
    const [shown6, setShown6] = useState(false);
    const [shown7, setShown7] = useState(false);
    const [shown8, setShown8] = useState(false);
    const [shown9, setShown9] = useState(false);
    const [shown10, setShown10] = useState(false);
    const [shown11, setShown11] = useState(false);
    const [shown12, setShown12] = useState(false);
    const [shown13, setShown13] = useState(false);
    const [shown14, setShown14] = useState(false);
    const [shown15, setShown15] = useState(false);
    const [shown16, setShown16] = useState(false);
    const [shown17, setShown17] = useState(false);
  return (
    <View>
    <ScrollView style={actionSheetStyles.actionScrollView}>
      <Text style={{...actionSheetStyles.actionTitle}}>Refund Policy</Text>
      <View style={{...actionSheetStyles.actionBorderView}}>
      <Text style={actionSheetStyles.termsBlackText}>
      Products and services sold and transmitted by Dohwe on www.dohwe.com consist credit Top-Up, to mobile phones, fixed data lines, electricity vouchers, Satellite TV, fuel accounts and related services such as SMS text messaging. These services are provided on behalf of the telecoms carriers and the various service providers available on the site www.dohwe.com , which are subject to change and availability. The relevant Service Provider is responsible for the provision of Top-Up services and is solely liable to you/your organisation and the Top-Up recipient.
        {`\n \n`}
        Once the transaction has been confirmed and payment has been successfully made, the Top-Up recharge is within an hour sent and transmitted by Dohwe to the input mobile number or account number .There may be an occasional short time lapse before the Top-Up is credited by the relevant mobile carrier. A confirmation email with Top-Up details will be sent once the transaction is complete.
        {`\n \n`}
        Because of the immediate use nature of a Top-Up, once a transaction has been completed and the Top-Up sent to the provided mobile number or account number, it cannot be modified, cancelled, refunded or removed. All executed transactions are final and any/all refunds are not available via www.dohwe.com . Once sent, Top-Up cannot be refunded or removed from the receiving phone by Dohwe. Please check that the mobile number entered is correct prior to completing a Top-Up transaction or Schedule.
        {`\n \n`}
        Any reversal or refund is to be done via the specific service provider’s support channels as highlighted below
        {`\n \n`}
        Netone: {`\n`}
        Econet: {`\n`}
        Telecel: {`\n`}
        ZETDC:
      </Text>
      </View>
      <View style={{marginBottom:70}}></View>
        </ScrollView>
     </View>
  );

  
}
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
    const actionSheetStyles = StyleSheet.create({
      actionSheetContainer: {
        width: '100%',
        paddingHorizontal: 7,
        height: '90%',
        backgroundColor:colors.lightGray1
      },
      actionSheetHeaderContainer: {
        flexDirection:'row',
        justifyContent: 'space-between',
        paddingTop:5,
        paddingHorizontal:5
      },
      actionCloseText:{
        fontSize:18
      },
      termsBlackText: {
        fontWeight:'bold',
        fontFamily: 'montserrat-regular',
        color:'#000000'
      },
      counterText: {
        fontWeight:'bold',
        color:'#000000',
        fontSize:15
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
      
        fontSize: 23,
        fontWeight: '500',
        color: 'rgba(0,0,0,0.8)',
        textAlign:'center'
    
      },
      actionTitleLeft: {
      
        fontSize: 23,
        fontWeight: '500',
        marginLeft:10
    
      },
      actionlogoImage: {
        width: 80,
        height: 40,
        resizeMode: 'contain',
      },
      actionScrollView:{
        //backgroundColor:'#ffffff',
         height:'100%',
          paddingTop:10
        
      },
      actionWhiteView:{
        backgroundColor:'rgba(255,255,255,0.5)',
        marginHorizontal:10,
        padding:10, 
        marginTop:10, 
        borderRadius:15 
      },
      actionBorderView:{
        backgroundColor:'rgba(255,255,255,0)',
        marginHorizontal:10,
        padding:10, 
        marginTop:10, 
        borderRadius:15,
        borderWidth:1,
        borderColor:'rgba(0,0,0,0.3)'
      },
      actionClearView:{marginHorizontal:10, padding:10, marginTop:10, borderRadius:15 },
      actionListItemView:{flexDirection:'row', marginTop:5, marginLeft:5, marginRight:30, alignItems:'center',     resizeMode: 'contain' },
    
    });