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

export default function Privacy() {
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
    <ScrollView style={styles.actionScrollView}>
      <Text style={{...styles.actionTitle}}>Privacy Policy</Text>
      <View style={styles.actionWhiteView}>
      <Text style={{...styles.termsGreyText,textAlign:'center'}}>Your privacy is important to us. It is www.dohwe.com policy to respect your privacy regarding any information we may collect from you across our website, www.dohwe.com, mobile app and other sites we own and operate.</Text>
      </View>
      <View style={{flexDirection:'row', justifyContent:'space-between', marginTop:15}}>
      <Text style={{...actionSheetStyles.actionTitleLeft, }}>1. Information we collect</Text>
      </View>

{
    shown &&
          <View style={actionSheetStyles.actionBorderView} ref={definitionsViewRef}>
          <Text style={{...styles.termsBlackText}}>"Personal Data" means information about you, from which you are identifiable, directly or indirectly, including but not limited to your name, email address, credit card or other payment information, mobile numbers, account numbers , budgets. We may also collect device data, such as your IMEI number, IP Address, operating system, device settings, list of installed mobile apps and geo-location data. We may access the Contact list information on your device to provide rich and personalized features. What we collect can depend on the individual settings of your device and software. We recommend checking the policies of your device manufacturer or software provider to learn what information they make available to us.{`\n \n`}
The provision of your Personal Data is voluntary. However, if you do not provide www.dohwe.com with your Personal Data, www.dohwe.com will not be able to provide services or accept payments from you.
{`\n \n`}
www.dohwe.com may collect, use, disclose and process your Personal Data lawfully, fairly and in a transparent manner for business and activities of www.dohwe.com which shall include, without limitation the following (the “Purposes”):</Text>

          <View style ={actionSheetStyles.actionListItemView} //List Item
        >
          <Icon name='checkmark-circle-outline' type='ionicon' color={'green'}></Icon>
          <Divider orientation='vertical' style={{marginHorizontal:5}}/>
          <Text style={actionSheetStyles.termsBlackText}>to validate and/or process schedules, Payments and/ or Notifications;</Text>
        </View>
        <View style ={actionSheetStyles.actionListItemView} //List Item
        >
          <Icon name='checkmark-circle-outline' type='ionicon' color={'green'}></Icon>
          <Divider orientation='vertical' style={{marginHorizontal:5}}/>
          <Text style={actionSheetStyles.termsBlackText}>to develop, enhance and provide what is required to meet your needs;</Text>
        </View>
        <View style ={actionSheetStyles.actionListItemView} //List Item
        >
          <Icon name='checkmark-circle-outline' type='ionicon' color={'green'}></Icon>
          <Divider orientation='vertical' style={{marginHorizontal:5}}/>
          <Text style={actionSheetStyles.termsBlackText}>for internal administrative purposes, such as auditing, data analysis, database records;</Text>
        </View>
        <View style ={actionSheetStyles.actionListItemView} //List Item
        >
          <Icon name='checkmark-circle-outline' type='ionicon' color={'green'}></Icon>
          <Divider orientation='vertical' style={{marginHorizontal:5}}/>
          <Text style={actionSheetStyles.termsBlackText}>to respond to questions, comments and feedback from you; and in accordance with any applicable laws permitting the use, collection, disclosure and processing of Personal Data.</Text>
        </View>
        <View style ={actionSheetStyles.actionListItemView} //List Item
        >
          <Icon name='checkmark-circle-outline' type='ionicon' color={'green'}></Icon>
          <Divider orientation='vertical' style={{marginHorizontal:5}}/>
          <Text style={actionSheetStyles.termsBlackText}>to comply with a legal and regulatory obligations.</Text>
        </View>
        <Text style={actionSheetStyles.termsBlackText}> {`\n`}We keep your Personal Data during the subsistence of a valid contract between ourselves and yourselves and destroy the same on termination of an existing contract. While we retain this information, we will protect it within commercially acceptable means to prevent loss and theft, as well as unauthorized access, disclosure, copying, use or modification. That said, we advise that no method of electronic transmission or storage is 100% secure and we cannot guarantee absolute data security.</Text>
        </View>
        
}

<View style={{flexDirection:'row', justifyContent:'space-between', marginTop:15}}>
      <Text style={{...actionSheetStyles.actionTitleLeft, }}>2. Disclosure of personal data to third parties</Text>
      </View>
      {
        shown2 &&
        <View style={{...actionSheetStyles.actionBorderView}}>

        <Text style={actionSheetStyles.termsBlackText}>www.dohwe.com may disclose your Personal Data to:</Text>
        <View style ={actionSheetStyles.actionListItemView} //List Item
        >
          <Icon name='checkmark-circle-outline' type='ionicon' color={'green'}></Icon>
          <Divider orientation='vertical' style={{marginHorizontal:5}}/>
          <Text style={actionSheetStyles.termsBlackText}>third party service providers for the purpose of enabling them to provide their services, including (without limitation) IT service providers, data storage, web-hosting and server providers, fraud prevention, debt collectors, maintenance or problem-solving providers, marketing or advertising providers, professional advisors and payment systems operators;</Text>
        </View>
        <View style ={actionSheetStyles.actionListItemView} //List Item
        >
          <Icon name='checkmark-circle-outline' type='ionicon' color={'green'}></Icon>
          <Divider orientation='vertical' style={{marginHorizontal:5}}/>
          <Text style={actionSheetStyles.termsBlackText}>sponsors or promoters of any competition we run; and</Text>
        </View>
        <View style ={actionSheetStyles.actionListItemView} //List Item
        >
          <Icon name='checkmark-circle-outline' type='ionicon' color={'green'}></Icon>
          <Divider orientation='vertical' style={{marginHorizontal:5}}/>
          <Text style={actionSheetStyles.termsBlackText}>courts, tribunals, regulatory authorities and law enforcement officers, as required by law, in connection with any actual or prospective legal proceedings, or in order to establish, exercise or defend our legal rights.</Text>
        </View>
  
        </View>
      }



<View style={{flexDirection:'row', justifyContent:'space-between', marginTop:15}}>
      <Text style={{...actionSheetStyles.actionTitleLeft, }}>3. International transfers of personal data</Text>
      </View>
      {
        shown3 &&
        <View style={actionSheetStyles.actionBorderView}>
          <Text style={actionSheetStyles.termsBlackText}>The Personal Data we collect is stored and processed where we or our partners, affiliates and third-party providers maintain facilities. By providing us with your personal information, you consent to the disclosure to these overseas third parties. {`\n \n`}

We will ensure that any transfer of personal information from countries in the European Economic Area (EEA) to countries outside the EEA will be protected by appropriate safeguards, for example by using standard data protection clauses approved by the European Commission, or the use of binding corporate rules or other legally accepted means. {`\n \n`}

Where we transfer personal information from a non-EEA country to another country, you acknowledge that third parties in other jurisdictions may not be subject to similar data protection laws to the ones in our jurisdiction. There are risks if any such third party engages in any act or practice that would contravene the data privacy laws in our jurisdiction and this might mean that you will not be able to seek redress under our jurisdiction’s privacy laws.</Text>
  
        </View>
      }


<View style={{flexDirection:'row', justifyContent:'space-between', marginTop:15}}>
      <Text style={{...actionSheetStyles.actionTitleLeft, }}>4. Your rights and controlling your personal data</Text>
      </View>
      { 
      shown4 && (
        <View style={actionSheetStyles.actionBorderView}>
          <Text style={actionSheetStyles.termsBlackText}>Choice and consent: By providing personal information to us, you consent to us collecting, holding, using and disclosing your personal information in accordance with this privacy policy. If you are under 16 years of age, you must have, and warrant to the extent permitted by law to us, that you have your parent or legal guardian’s permission to access and use the website and they (your parents or guardian) have consented to you providing us with your personal information. You do not have to provide personal information to us, however, if you do not, it may affect your use of this website or the products and/or services offered on or through it.
          {`\n \n`}
Information from third parties: If we receive personal information about you from a third party, we will protect it as set out in this privacy policy. If you are a third party providing personal information about somebody else, you represent and warrant that you have such person’s consent to provide the personal information to us.
{`\n \n`}
Restrict: You may choose to restrict the collection or use of your personal information. If you have previously agreed to us using your personal information for direct marketing purposes, you may change your mind at any time by contacting us using privacy@www.dohwe.com. If you ask us to restrict or limit how we process your personal information, we will let you know how the restriction affects your use of our website or products and services.
{`\n \n`}
Access and data portability: You may request details of the personal information that we hold about you. You may request a copy of the personal information we hold about you. Where possible, we will provide this information in CSV format or other easily readable machine format. You may request that we erase the personal information we hold about you at any time. You may also request that we transfer this personal information to another third party.
{`\n \n`}
Correction: If you believe that any information we hold about you is inaccurate, out of date, incomplete, irrelevant or misleading, please correct that via our website or mobile app.
{`\n \n`}
Notification of data breaches: We will comply laws applicable to us in respect of any data breach.
{`\n \n`}
Complaints: If you believe that we have breached a relevant data protection law and wish to make a complaint, please contact us using privacy@www.dohwe.com and provide us with full details of the alleged breach. We will promptly investigate your complaint and respond to you, in writing, setting out the outcome of our investigation and the steps we will take to deal with your complaint. You also have the right to contact a regulatory body or data protection authority in relation to your complaint.
{`\n \n`}
Unsubscribe: To unsubscribe from our e-mail database or opt-out of communications (including marketing communications), please contact us using the opt-out using the opt-out facilities provided in the communication.</Text>
  
        </View>
      )

      }


<View style={{flexDirection:'row', justifyContent:'space-between', marginTop:15}}>
      <Text style={{...actionSheetStyles.actionTitleLeft, }}>5. Cookies</Text>
      <Icon name={shown5?'chevron-up':'chevron-down'} type='ionicon' style={{marginRight:25, marginTop:5}} onPress={()=>{setShown5(!shown5)}}></Icon>
      </View>
      {
        shown5 &&
        <View style={actionSheetStyles.actionClearView}>
          <Text style={actionSheetStyles.termsBlackText}>We use “cookies” to collect information about you and your activity across our site. A cookie is a small piece of data that our website stores on your computer, and accesses each time you visit, so we can understand how you use our site. This helps us serve you content based on preferences you have specified.</Text>
  
        </View>
      }



<View style={{flexDirection:'row', justifyContent:'space-between', marginTop:15}}>
      <Text style={{...actionSheetStyles.actionTitleLeft, }}>6. Business transfers</Text>
      <Icon name={shown6?'chevron-up':'chevron-down'} type='ionicon' style={{marginRight:25, marginTop:5}} onPress={()=>{setShown6(!shown6)}}></Icon>
      </View>
      {
        shown6 &&
        <View style={actionSheetStyles.actionClearView}>
          <Text style={actionSheetStyles.termsBlackText}>If we or our assets are acquired, or in the unlikely event that we go out of business or enter bankruptcy, we would include data among the assets transferred to any parties who acquire us. You acknowledge that such transfers may occur, and that any parties who acquire us may continue to use your personal information according to this policy.</Text>
  
        </View>
      }



<View style={{flexDirection:'row', justifyContent:'space-between', marginTop:15}}>
      <Text style={{...actionSheetStyles.actionTitleLeft, }}>7. Limits of our policy</Text>
      <Icon name={shown7?'chevron-up':'chevron-down'} type='ionicon' style={{marginRight:25, marginTop:5}} onPress={()=>{setShown7(!shown7)}}></Icon>
      </View>
      {
        shown7 &&       
        <View style={actionSheetStyles.actionClearView}>
          <Text style={actionSheetStyles.termsBlackText}>Our website may link to external sites that are not operated by us. Please be aware that we have no control over the content and policies of those sites, and cannot accept responsibility or liability for their respective privacy practices.</Text>
  
        </View>
      }



<View style={{flexDirection:'row', justifyContent:'space-between', marginTop:15}}>
      <Text style={{...actionSheetStyles.actionTitleLeft, }}>8. Changes to this policy</Text>
      <Icon name={shown8?'chevron-up':'chevron-down'} type='ionicon' style={{marginRight:25, marginTop:5}} onPress={()=>{setShown8(!shown8)}}></Icon>
      </View>
      {shown8 &&
        <View style={actionSheetStyles.actionClearView}>
        <Text style={actionSheetStyles.termsBlackText}>At our discretion, we may change our privacy policy to reflect current acceptable practices. We will take reasonable steps to let users know about changes via our website. Your continued use of this site after any changes to this policy will be regarded as acceptance of our practices around privacy and personal information.
        {`\n \n`}
If we make a significant change to this privacy policy, for example changing a lawful basis on which we process your personal information, we will ask you to re-consent to the amended privacy policy.</Text>

      </View>
      }
      <View style={{marginBottom:70}}></View>
        </ScrollView>
     </View>
  );

  
}
const styles = StyleSheet.create({
    container: {
      flex: 1,
      backgroundColor: '#fff',
    },
  
    Bottomcontainer: {
      bottom:0,
      justifyContent:'center',
      backgroundColor: '#fff',
      alignItems:'center'
    },
    termsContainer: {
      width:'90%',
      marginBottom:10,
      textAlign:'center',
      alignItems:'center'
    },
    welcomeContainer: {
      height: Platform.OS === 'ios' ? '14%' : 63,
      paddingTop: Platform.OS === 'ios' ? Constants.statusBarHeight : 5,
      flexDirection:'row',
      justifyContent: 'space-between',
      paddingBottom: 5,
      paddingHorizontal:10,
      alignItems:'flex-start'
    },
    headerTitle: {
      fontSize: 25,
      fontWeight: '800',
      marginHorizontal: 15,
      color:'#1c5db9',
      marginBottom:2
    },
    subTitle: {
      fontSize: 14,
      fontWeight: '400',
      marginHorizontal: 15,
      width:'75%',
      color:'#003366',
      marginBottom:50
    },
    websiteLink: {
      bottom:0,
      position:'absolute',
      flexDirection:'row',
      alignSelf: "center",
      alignItems:'center',
      alignContent: 'center',
      ...ifIphoneX({
        marginBottom: getBottomSpace()
      }, {
          marginBottom: 15
      })
    },
    myLink: {
  
      flexDirection:'row',
      alignSelf: "center",
      alignItems:'center',
      alignContent: 'center',
      ...ifIphoneX({
        marginBottom: getBottomSpace()
      }, {
          marginBottom: 15
      })
    },
    termsGreyText: {
      textAlign:'center',
      fontWeight:'300',
      fontFamily: 'montserrat-regular',
    },
    termsBlackText: {
      fontWeight:'bold',
      fontFamily: 'montserrat-regular',
      color:'#000000'
    },
    termsPasswordScreenText: {
      fontWeight: '300',
      textAlign:'center'
    },
    termsText: {
      color:'#1c5db9',
      fontFamily: 'montserrat-regular',
      fontWeight: '100',
      fontSize: 14
    },
    termsBold: {
      color:'#000000',
      fontWeight: '800',
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
      width: '95%',
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
    },
    inputLabel:{
      color: 'gray',
      fontWeight: '300',
      fontSize: 14,
      marginBottom:-5
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
    textDohweBlue:{
      color:'#1c5db9',
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
      width: Platform.OS === 'ios' ? 350 : 290
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
  
    // Action sheet styles
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
    actionClearView:{marginHorizontal:10, padding:10, marginTop:10, borderRadius:15 },
    actionListItemView:{flexDirection:'row', marginTop:5, marginLeft:5, marginRight:30, alignItems:'center',     resizeMode: 'contain' },
  
  
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