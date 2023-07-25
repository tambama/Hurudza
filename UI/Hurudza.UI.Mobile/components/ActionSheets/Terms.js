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

export default function Terms() {
    const definitionsViewRef = useRef(null);
    const [shown, setShown] = useState(false);
    const [shown2, setShown2] = useState(false);
    const [shown3, setShown3] = useState(false);
    const [shown4, setShown4] = useState(false);
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
      <Text style={{...styles.actionTitle}}>Terms & Conditions</Text>
      <View style={styles.actionWhiteView}>
      <Text style={{...styles.termsGreyText,textAlign:'center'}}>These Terms of Service(Agreement) set out the legally-binding terms and conditions for your use of the <Text style={styles.termsText}>Dohwe</Text> Services. </Text>
      </View>
      <View style={styles.actionClearView}>
        <Text style={{...styles.termsGreyText,textAlign:'center'}}>
        BY YOUR USE OF OUR SERVICE, YOU AGREE THAT YOU HAVE READ, UNDERSTOOD AND ACCEPT TO BE BOUND BY THESE TERMS OF SERVICE SET FORTH BELOW. IF YOU DO NOT AGREE TO BE BOUND BY THIS AGREEMENT, YOU SHOULD NOT USE THE SERVICE. THIS AGREEMENT, EFFECTIVE FROM 1 JULY 2021 CONSTITUTES A LEGAL CONTRACT BETWEEN YOU AND <Text style={styles.termsText}>WWW.DOHWE.COM</Text>(TOGETHER WITH ITS SUBSIDIARIES AND OTHER AFFILIATES) AND SHALL SUPERSEDE ANY EXISTING AGREEMENT YOU MAY HAVE WITH US EXCEPT WHERE OTHERWISE COMMUNICATED. <Text style={styles.termsText}>WWW.DOHWE.COM</Text>  RESERVES THE RIGHT, AT ITS SOLE DISCRETION, TO REVISE, ADD, OR DELETE PORTIONS OF THESE TERMS AND CONDITIONS FROM TIME TO TIME WITHOUT FURTHER NOTICE TO YOU. WE MAY, WITHOUT OBLIGATION, PROVIDE YOU ADVANCE NOTICE OF ANY MATERIAL REVISIONS TO THE EMAIL ADDRESS LINKED TO YOUR USER ACCOUNT.
        </Text>
      </View>
      <View style={styles.actionWhiteView}>
      <Text style={{...styles.termsGreyText,textAlign:'center'}}>To be eligible to register for a www.dohwe.com account in order to use the Services, or to continue using the Services : </Text>

      
      <View style ={styles.actionListItemView} //List Item
      >
        <Icon name='checkmark-circle-outline' type='ionicon' color={'green'}></Icon>
        <Divider orientation='vertical' style={{marginHorizontal:5}}/>
        <Text style={styles.termsBlackText}>You shall be at least 18 years of age</Text>
      </View>
      <View style ={styles.actionListItemView} //List Item
      >
        <Icon name='checkmark-circle-outline' type='ionicon' color={'green'}></Icon>
        <Divider orientation='vertical' style={{marginHorizontal:5}}/>
        <Text style={styles.termsBlackText}>You have the rights, authority, and capacity to enter into this Agreement</Text>
      </View>
      <View style ={styles.actionListItemView} //List Item
      >
        <Icon name='checkmark-circle-outline' type='ionicon' color={'green'}></Icon>
        <Divider orientation='vertical' style={{marginHorizontal:5}}/>
        <Text style={styles.termsBlackText}>You shall abide by all of the terms and conditions of this Agreement</Text>
      </View>
      <View style ={styles.actionListItemView} //List Item
      >
        <Icon name='checkmark-circle-outline' type='ionicon' color={'green'}></Icon>
        <Divider orientation='vertical' style={{marginHorizontal:5}}/>
        <Text style={styles.termsBlackText}>You shall not impersonate any person or entity, or falsely state or otherwise misrepresent identity, age or affiliation with any person or entity.</Text>
      </View>
      <View style ={styles.actionListItemView} //List Item
      >
        <Icon name='checkmark-circle-outline' type='ionicon' color={'green'}></Icon>
        <Divider orientation='vertical' style={{marginHorizontal:5}}/>
        <Text style={styles.termsBlackText}>You must review and accept this Agreement by clicking on the “I Accept” button or other mechanism provided. </Text>
      </View>
      <View style ={styles.actionListItemView} //List Item
      >
        <Icon name='checkmark-circle-outline' type='ionicon' color={'green'}></Icon>
        <Divider orientation='vertical' style={{marginHorizontal:5}}/>
          <Text style={styles.termsBlackText}>You acknowledge that www.dohwe.com at any time in its sole discretion, with or without notice, including without limitation may terminate the account, and any information uploaded to the account or prohibit you from using or accessing the Services for any reasons known to them including the ones stated above.</Text>
        </View>
      <View style ={styles.actionListItemView} //List Item
      >
        <Icon name='checkmark-circle-outline' type='ionicon' color={'green'}></Icon>
        <Divider orientation='vertical' style={{marginHorizontal:5}}/>
        <Text style={styles.termsBlackText}>This license shall extend to your affiliates, provided that such affiliates are acting via your account and provided further that you remain jointly and severally liable for all acts and omissions of your affiliates.</Text>
      </View>
      </View>
      <View style={styles.actionClearView}>
        <Text style={{...styles.termsBlackText}}>
        In this Agreement, we, us, our or www.dohwe.com will refer collectively to Thinkstack Private Limited Trading as Dohwe and its subsidiaries/affiliates existing from time to time and the terms you, your and Customer will refer to you. If you are registering for an account in order to use the Services on behalf of an organisation, then you are entering into this Agreement on behalf of that organisation and represent and warrant that you have the authority to bind that organisation to this Agreement (and, in which case, the terms you, your and Client will refer to that organization). Dohwe and the Client are each referred to in this Agreement as a Party and collectively as the Parties.
        </Text>
      </View>
      <View style={{flexDirection:'row', justifyContent:'space-between', marginTop:15}}>
      <Text style={{...actionSheetStyles.actionTitleLeft, }}>1. Definitions</Text>
      <Icon name={shown?'chevron-up':'chevron-down'} type='ionicon' style={{marginRight:25, marginTop:5}} onPress={()=>{setShown(!shown)}}></Icon>
      </View>

{
    shown &&
          <View style={actionSheetStyles.actionBorderView} ref={definitionsViewRef}>
          <Text style={{...styles.termsBlackText}}>1.1 In this Agreement, unless otherwise specified, the following words shall have the meanings next to them:</Text>
  
  
          <View style ={actionSheetStyles.actionListItemView} //List Item
        >
          <Icon name='checkmark-circle-outline' type='ionicon' color={'green'}></Icon>
          <Divider orientation='vertical' style={{marginHorizontal:5}}/>
          <Text style={actionSheetStyles.termsBlackText}>Affiliate means in relation to a Party, any other body corporate directly or indirectly, Controlling, Controlled by, or is under common Control with, such Party and Affiliates shall be construed accordingly.</Text>
        </View>
        <View style ={actionSheetStyles.actionListItemView} //List Item
        >
          <Icon name='checkmark-circle-outline' type='ionicon' color={'green'}></Icon>
          <Divider orientation='vertical' style={{marginHorizontal:5}}/>
          <Text style={actionSheetStyles.termsBlackText}>API Key means the unique and secret authentication code issued to you on the User Account which you shall use to access the API Platform.</Text>
        </View>
        <View style ={actionSheetStyles.actionListItemView} //List Item
        >
          <Icon name='checkmark-circle-outline' type='ionicon' color={'green'}></Icon>
          <Divider orientation='vertical' style={{marginHorizontal:5}}/>
          <Text style={actionSheetStyles.termsBlackText}>API Platform means Dohwe’s application programming interface platform accessible on the Site.</Text>
        </View>
        <View style ={actionSheetStyles.actionListItemView} //List Item
        >
          <Icon name='checkmark-circle-outline' type='ionicon' color={'green'}></Icon>
          <Divider orientation='vertical' style={{marginHorizontal:5}}/>
          <Text style={actionSheetStyles.termsBlackText}>Control means, in relation to a body corporate, the power or ability of a person to secure that the affairs of the body corporate are conducted directly or indirectly in accordance with the wishes of that person: (1) by means of the holding of shares, or the possession of voting power, in or in relation to that or any other body corporate; or (2) by virtue of any powers conferred by the articles of association, or any other document, regulating that or any other body corporate, and Controlling and Controlled shall be construed accordingly.</Text>
        </View>
        <View style ={actionSheetStyles.actionListItemView} //List Item
        >
          <Icon name='checkmark-circle-outline' type='ionicon' color={'green'}></Icon>
          <Divider orientation='vertical' style={{marginHorizontal:5}}/>
          <Text style={actionSheetStyles.termsBlackText}>Disclosing Party means a Party that discloses Confidential Information.</Text>
        </View>
        <View style ={actionSheetStyles.actionListItemView} //List Item
        >
          <Icon name='checkmark-circle-outline' type='ionicon' color={'green'}></Icon>
          <Divider orientation='vertical' style={{marginHorizontal:5}}/>
          <Text style={actionSheetStyles.termsBlackText}>Force Majeure Event means any happening or event which is beyond the reasonable control of a Party and which negatively affects a Party’s performance of its obligations or makes such performance impossible or so impracticable as to be considered impossible in the circumstances including acts of God, riots, war, armed conflict, civil strife, acts of terrorism, acts of government, the Regulator or other regulators, fire, power outages, material adverse weather conditions including flood, storm or earthquake, or disaster, geographical topography, or the unexpected refusal, or inability or delay by a third party to supply goods or services to a Party.</Text>
        </View>
        <View style ={actionSheetStyles.actionListItemView} //List Item
        >
          <Icon name='checkmark-circle-outline' type='ionicon' color={'green'}></Icon>
          <Divider orientation='vertical' style={{marginHorizontal:5}}/>
          <Text style={actionSheetStyles.termsBlackText}>Intellectual Property means any and all patents, trademarks, copyrights, inventions, Works, trade names, domain names, rights in get-up, rights in goodwill or to sue for passing off, rights in designs, rights in computer software, database rights, rights in Confidential Information, know-how, trade secrets, discoveries, creations, inventions or improvements upon or additions to an invention, moral rights, any research effort relating to any of the above and any other intellectual property rights, in each case whether registered or unregistered and including all applications (or rights to apply) for, and renewals or extensions of, such rights and all similar or equivalent rights or forms of protection which may now or in the future subsist in any part of the world.</Text>
        </View>
        <View style ={actionSheetStyles.actionListItemView} //List Item
        >
          <Icon name='checkmark-circle-outline' type='ionicon' color={'green'}></Icon>
          <Divider orientation='vertical' style={{marginHorizontal:5}}/>
          <Text style={actionSheetStyles.termsBlackText}>Service Provider mean a company whose services Dohwe resells in a country Dohwe operates in and Service Providers shall be construed accordingly.</Text>
        </View>
        <View style ={actionSheetStyles.actionListItemView} //List Item
        >
          <Icon name='checkmark-circle-outline' type='ionicon' color={'green'}></Icon>
          <Divider orientation='vertical' style={{marginHorizontal:5}}/>
          <Text style={actionSheetStyles.termsBlackText}>Personal Data means any information relating to an identified or identifiable natural or juristic person; an identifiable person is one who can be identified, directly or indirectly, in particular by reference to an identifier such as a name, identity number , mobile number , address data, service provider account number, schedule lists, spend data.</Text>
        </View>
        <View style ={actionSheetStyles.actionListItemView} //List Item
        >
          <Icon name='checkmark-circle-outline' type='ionicon' color={'green'}></Icon>
          <Divider orientation='vertical' style={{marginHorizontal:5}}/>
          <Text style={actionSheetStyles.termsBlackText}>Person includes a natural person, body corporate, unincorporated venture, trust, joint venture, association, statutory corporation, state, state agency, governmental authority or firm.</Text>
        </View>
        <View style ={actionSheetStyles.actionListItemView} //List Item
        >
          <Icon name='checkmark-circle-outline' type='ionicon' color={'green'}></Icon>
          <Divider orientation='vertical' style={{marginHorizontal:5}}/>
          <Text style={actionSheetStyles.termsBlackText}>Receiving Party means a Party that receives Confidential Information.</Text>
        </View>
        <View style ={actionSheetStyles.actionListItemView} //List Item
        >
          <Icon name='checkmark-circle-outline' type='ionicon' color={'green'}></Icon>
          <Divider orientation='vertical' style={{marginHorizontal:5}}/>
          <Text style={actionSheetStyles.termsBlackText}>Regulator means the relevant regulatory authority in a country we operate in that governs benefits provision and reselling.</Text>
        </View>
        <View style ={actionSheetStyles.actionListItemView} //List Item
        >
          <Icon name='checkmark-circle-outline' type='ionicon' color={'green'}></Icon>
          <Divider orientation='vertical' style={{marginHorizontal:5}}/>
          <Text style={actionSheetStyles.termsBlackText}>Representatives means a Party’s directors, officers, employees or agents.</Text>
        </View>
        <View style ={actionSheetStyles.actionListItemView} //List Item
        >
          <Icon name='checkmark-circle-outline' type='ionicon' color={'green'}></Icon>
          <Divider orientation='vertical' style={{marginHorizontal:5}}/>
          <Text style={actionSheetStyles.termsBlackText}>Schedule  means the instruction set that is given to Dohwe by your organisation, which describes the specific Service(s) you have requested and the applicable pricing of the Service(s), Frequency of delivery of the services , which shall form part of this Agreement.</Text>
        </View>
        <View style ={actionSheetStyles.actionListItemView} //List Item
        >
          <Icon name='checkmark-circle-outline' type='ionicon' color={'green'}></Icon>
          <Divider orientation='vertical' style={{marginHorizontal:5}}/>
          <Text style={actionSheetStyles.termsBlackText}>Wallet means your wallet in the User Account to which you load a value of money to utilize in respect of the Services.</Text>
        </View>
        <View style ={actionSheetStyles.actionListItemView} //List Item
        >
          <Icon name='checkmark-circle-outline' type='ionicon' color={'green'}></Icon>
          <Divider orientation='vertical' style={{marginHorizontal:5}}/>
          <Text style={actionSheetStyles.termsBlackText}>Account means a customer’s account where you can upload scheduled lists, load wallet, access usage report, access APIs for integration into your systems and manage multiple organisations lists.</Text>
        </View>
  
        </View>
}

<View style={{flexDirection:'row', justifyContent:'space-between', marginTop:15}}>
      <Text style={{...actionSheetStyles.actionTitleLeft, }}>2. Access and Security</Text>
      <Icon name={shown2?'chevron-up':'chevron-down'} type='ionicon' style={{marginRight:25, marginTop:5}} onPress={()=>{setShown2(!shown2)}}></Icon>
      </View>
      {
        shown2 &&
        <View style={{...actionSheetStyles.actionClearView}}>

        <View style ={actionSheetStyles.actionListItemView} //List Item
        >
          <Text style={actionSheetStyles.counterText}>2.1</Text>
          <Divider orientation='vertical' style={{marginHorizontal:5}}/>
          <Text style={actionSheetStyles.termsBlackText}>To access and use the Services, you must create a User Account. The User Account shall be solely accessible using a username and password set by you, and an API Key generated by you on the User Account.</Text>
        </View>
  
        <View style ={actionSheetStyles.actionListItemView} //List Item
        >
          <Text style={actionSheetStyles.counterText}>2.2</Text>
          <Divider orientation='vertical' style={{marginHorizontal:5}}/>
          <Text style={actionSheetStyles.termsBlackText}>Dohwe shall work together with you on the integration between your systems and Dohwe for the purpose of this Agreement.</Text>
        </View>
        <View style ={actionSheetStyles.actionListItemView} //List Item
        >
          <Text style={actionSheetStyles.counterText}>2.3</Text>
          <Divider orientation='vertical' style={{marginHorizontal:5}}/>
          <Text style={actionSheetStyles.termsBlackText}>You shall access the user Account over a secure HTTPS connection. You shall be fully responsible for contents of your user Account, the API Key and your internal local area Service and security setups, including configuration of firewalls and other protocols required to protect your Service from hackers and malicious intrusion.</Text>
        </View>
        <View style ={actionSheetStyles.actionListItemView} //List Item
        >
          <Text style={actionSheetStyles.counterText}>2.4</Text>
          <Divider orientation='vertical' style={{marginHorizontal:5}}/>
          <View>
            <Text style={actionSheetStyles.termsBlackText}>You may be requested to complete a Service Order Form confirming your requested Service and you shall comply with Dohwe’s KYC documentation when required, including but not necessarily limited to submitting to Dohwe:</Text>
          <Text style={actionSheetStyles.termsBlackText}><Text style={actionSheetStyles.counterText}>2.4.1</Text>  a duly filled out KYC form; </Text>
          <Text style={actionSheetStyles.termsBlackText}><Text style={actionSheetStyles.counterText}>2.4.2</Text>  a copy of its certificate of incorporation;</Text>
          <Text style={actionSheetStyles.termsBlackText}><Text style={actionSheetStyles.counterText}>2.4.3</Text>  a copy of its tax identification number from the relevant tax authority; and</Text>
          <Text style={actionSheetStyles.termsBlackText}><Text style={actionSheetStyles.counterText}>2.4.4</Text>  a form from the relevant companies registry indicating the shareholder and directors structure.</Text>
  
          <Text style={actionSheetStyles.termsBlackText}>2.4.5  AT may from time to time require you to update the identification information/ documents it holds on your behalf as may be required by a Regulator or a Service Provider and shall have the discretion to suspend the provision of the Services until such update is provided.</Text>
          </View>
        </View>
        </View>
      }



<View style={{flexDirection:'row', justifyContent:'space-between', marginTop:15}}>
      <Text style={{...actionSheetStyles.actionTitleLeft, }}>3. Fees and Payment</Text>
      <Icon name={shown3?'chevron-up':'chevron-down'} type='ionicon' style={{marginRight:25, marginTop:5}} onPress={()=>{setShown3(!shown3)}}></Icon>
      </View>
      {
        shown3 &&
        <View style={actionSheetStyles.actionClearView}>
        <View style ={actionSheetStyles.actionListItemView} //List Item
        >
          <Text style={actionSheetStyles.counterText}>3.1</Text>
          <Divider orientation='vertical' style={{marginHorizontal:5}}/>
          <Text style={actionSheetStyles.termsBlackText}>Unless otherwise agreed upon by both Parties in writing, you shall pay Dohwe the fees set out on www.dohwe.com , in advance. </Text>
        </View>
        <View style ={actionSheetStyles.actionListItemView} //List Item
        >
          <Text style={actionSheetStyles.counterText}>3.2</Text>
          <Divider orientation='vertical' style={{marginHorizontal:5}}/>
          <Text style={actionSheetStyles.termsBlackText}>Invoices in respect of any invoiceable payments shall be sent to the e-mail address specified by you, and shall be deemed received on the date sent. Such invoices will specify the payment period. Without prejudice to any other right or remedy that it may have, if you fail to pay an invoice within seven (7) days of the due date indicated in such invoice, Dohwe shall charge a finance charge of 20% per month on the sum due payable from the due date until the invoice is settled. If you fail to pay Dohwe any sum due within thirty (30) days of the due date indicated in such invoice, Dohwe may suspend all or part of the Services until payment has been made in full. Dohwe shall immediately without undue delay or any additional requests from you reactivate any Services once it has received full payment from you.</Text>
        </View>
        <View style ={actionSheetStyles.actionListItemView} //List Item
        >
          <Text style={actionSheetStyles.counterText}>3.3</Text>
          <Divider orientation='vertical' style={{marginHorizontal:5}}/>
          <Text style={actionSheetStyles.termsBlackText}>The fees shall be subject to change from time to time in line with any rates adjustments by the Service Provider, the Regulator, and/or the government of the relevant jurisdiction the Services are provided in and such change shall be notified to you as soon as reasonably possible. Any changes in charges at the discretion of Dohwe shall be notified to you at least thirty (7) days prior to the changes taking effect.</Text>
        </View>
        <View style ={actionSheetStyles.actionListItemView} //List Item
        >
          <Text style={actionSheetStyles.counterText}>3.4</Text>
          <Divider orientation='vertical' style={{marginHorizontal:5}}/>
          <View>
            <Text style={actionSheetStyles.termsBlackText}>All fees payable to Dohwe shall be: </Text>
          <Text style={actionSheetStyles.termsBlackText}><Text style={actionSheetStyles.counterText}>3.4.1</Text>  inclusive of VAT (unless otherwise stated); and</Text>
          <Text style={actionSheetStyles.termsBlackText}><Text style={actionSheetStyles.counterText}>3.4.2</Text>  paid in full without any set-off, counterclaim, deduction or withholding (other than any deduction or withholding of tax as may be required by law).</Text>
  
          </View>
        </View>
  
        </View>
      }


<View style={{flexDirection:'row', justifyContent:'space-between', marginTop:15}}>
      <Text style={{...actionSheetStyles.actionTitleLeft, }}>4. Acts of a Service Provider</Text>
      <Icon name={shown4?'chevron-up':'chevron-down'} type='ionicon' style={{marginRight:25, marginTop:5}} onPress={()=>{setShown4(!shown4)}}></Icon>
      </View>
      { 
      shown4 && (
        <View style={actionSheetStyles.actionClearView}>
        <View style ={actionSheetStyles.actionListItemView} //List Item
        >
          <Text style={actionSheetStyles.counterText}>4.1</Text>
          <Divider orientation='vertical' style={{marginHorizontal:5}}/>
          <Text style={actionSheetStyles.termsBlackText}>You are hereby made aware that a Service Provider may have the right to act unilaterally and to the prejudice of Dohwe including but not limited to the Service Provider (a) reviewing its rate charge at any time and (b) at its sole direction deciding to terminate a service. The Parties hereby agree that in the event that an act by the Service Provider against Dohwe prejudices you, you shall have no claim of whatsoever nature against Dohwe and/or its Representatives, irrespective of the prejudice suffered by you. </Text>
        </View>
        <View style ={actionSheetStyles.actionListItemView} //List Item
        >
          <Text style={actionSheetStyles.counterText}>4.2</Text>
          <Divider orientation='vertical' style={{marginHorizontal:5}}/>
          <Text style={actionSheetStyles.termsBlackText}>The Parties agree to have good faith negotiations to amend the Agreement in the event of a unilateral and prejudicial act by a Service Provider against Dohwe and in the event that Parties cannot agree to the amended terms, either Party shall have the right to terminate the Agreement in accordance with clause.</Text>
        </View>
  
        </View>
      )

      }


<View style={{flexDirection:'row', justifyContent:'space-between', marginTop:15}}>
      <Text style={{...actionSheetStyles.actionTitleLeft, }}>5. Confidentiality</Text>
      <Icon name={shown5?'chevron-up':'chevron-down'} type='ionicon' style={{marginRight:25, marginTop:5}} onPress={()=>{setShown5(!shown5)}}></Icon>
      </View>
      {
        shown5 &&
        <View style={{...actionSheetStyles.actionClearView}}>

        <View style ={actionSheetStyles.actionListItemView} //List Item
        >
          <Text style={actionSheetStyles.counterText}>5.1</Text>
          <Divider orientation='vertical' style={{marginHorizontal:5}}/>
          <Text style={actionSheetStyles.termsBlackText}>In this Agreement, Confidential Information means any non-public information or data, regardless of whether it is in tangible form, disclosed by either Party that is marked or otherwise designated as confidential or proprietary or that should otherwise be reasonably understood to be confidential given the nature of the information and the circumstances surrounding disclosure. Confidential Information does not include information which is already known to the receiving Party at the time of disclosure by the Disclosing Party; is or becomes publicly known through no wrongful act of the Receiving Party; is independently developed by the Receiving Party without benefit of the Disclosing Party’s Confidential Information; or is received by the Receiving Party from a third party without restriction and without a breach of an obligation of confidentiality.</Text>
        </View>
        <View style ={actionSheetStyles.actionListItemView} //List Item
        >
          <Text style={actionSheetStyles.counterText}>5.2</Text>
          <Divider orientation='vertical' style={{marginHorizontal:5}}/>
          <Text style={actionSheetStyles.termsBlackText}> Each Party hereby agrees that if either Party provides Confidential Information to the other Party, such Confidential Information shall be held in the strictest of confidence and the receiving Party shall afford such Confidential Information the same care and protection as it affords generally to its own confidential and proprietary information (which in any case shall not be less than reasonable care) to avoid disclosure to or unauthorized use by any third party.</Text>
        </View>
        <View style ={actionSheetStyles.actionListItemView} //List Item
        >
          <Text style={actionSheetStyles.counterText}>5.3</Text>
          <Divider orientation='vertical' style={{marginHorizontal:5}}/>
          <Text style={actionSheetStyles.termsBlackText}>The receiving Party may disclose confidential Information to the minimum extent required by (a) any applicable order of any court of competent jurisdiction or any competent judicial, governmental or regulatory body or (b) the applicable laws or regulations of any country or governmental authority with jurisdiction over the affairs of the receiving Party in line with THE PRIVACY POLICY SHARED</Text>
        </View>
        <View style ={actionSheetStyles.actionListItemView} //List Item
        >
          <Text style={actionSheetStyles.counterText}>5.4</Text>
          <Divider orientation='vertical' style={{marginHorizontal:5}}/>
          <Text style={actionSheetStyles.termsBlackText}> Dohwe shall ensure that the collection, handling, storage, processing and disposal and any other use (collectively Processing) of Personal Data is done in compliance with all applicable data, privacy and cyber security laws and that Personal Data that is accessed or collected during the performance or utilization of the Services is kept secure and Dohwe shall use appropriate technological, Organisational and security practices and systems in respect of the Personal Data to comply with legal and regulatory requirements including data protection requirements. Dohwe shall take prompt remedial action against any unauthorized use, storage, reproduction or redistribution of the Personal Data and shall immediately notify you of any Personal Data breaches and no later than seventy two (72) hours after it has become aware of the breach. Dohwe shall keep records of Personal Data breaches, indicating the relevant facts, their effects and the remedial actions taken.</Text>
        </View>
        <View style ={actionSheetStyles.actionListItemView} //List Item
        >
          <Text style={actionSheetStyles.counterText}>5.5</Text>
          <Divider orientation='vertical' style={{marginHorizontal:5}}/>
          <Text style={actionSheetStyles.termsBlackText}>If Dohwe will be Processing Personal Data from the EEA, Switzerland, or the United Kingdom on your behalf, and you wish to execute a Data Protection Agreement (DPA) with Dohwe, as required by the General Data Protection Regulation (GDPR), then you may do so by submitting a request to product@dohwe.com . Upon receipt of your request, we will send you a GDPR DPA ready for execution.</Text>
        </View>
        </View>
      }



<View style={{flexDirection:'row', justifyContent:'space-between', marginTop:15}}>
      <Text style={{...actionSheetStyles.actionTitleLeft, }}>6. Intellectual Property</Text>
      <Icon name={shown6?'chevron-up':'chevron-down'} type='ionicon' style={{marginRight:25, marginTop:5}} onPress={()=>{setShown6(!shown6)}}></Icon>
      </View>
      {
        shown6 &&
              <View style={actionSheetStyles.actionClearView}>
              <View style ={actionSheetStyles.actionListItemView} //List Item
              >
                <Text style={actionSheetStyles.counterText}>6.1</Text>
                <Divider orientation='vertical' style={{marginHorizontal:5}}/>
                <Text style={actionSheetStyles.termsBlackText}>Each Party shall retain its Intellectual Property whether registered or not, used by or related to either Party. All legal and beneficial rights the Intellectual Property which Dohwe provides to you for the purpose of using the Services will remain at all times the property of Dohwe or its owner or licensor. To the extent that it is so entitled, Dohwe grants you a non-exclusive, non-transferable license to use such Intellectual Property for the sole purpose of using the Services as contemplated under this Agreement. The Client shall not modify, adapt, translate, reverse engineer or disassemble Dohwe’s APIs, Services or any other Intellectual Property owned by Dohwe. </Text>
              </View>
              <View style ={actionSheetStyles.actionListItemView} //List Item
              >
                <Text style={actionSheetStyles.counterText}>6.2</Text>
                <Divider orientation='vertical' style={{marginHorizontal:5}}/>
                <Text style={actionSheetStyles.termsBlackText}>If you provide any feedback to Dohwe the Site or Services, you hereby assign to Dohwe all right, title, and interest in and to the feedback, and Dohwe is free to use the feedback without payment or restriction.</Text>
              </View>
        
              </View>
      }



<View style={{flexDirection:'row', justifyContent:'space-between', marginTop:15}}>
      <Text style={{...actionSheetStyles.actionTitleLeft, }}>7. Term and Termination</Text>
      <Icon name={shown7?'chevron-up':'chevron-down'} type='ionicon' style={{marginRight:25, marginTop:5}} onPress={()=>{setShown7(!shown7)}}></Icon>
      </View>
      {
        shown7 &&       
        <View style={{...actionSheetStyles.actionClearView}}>

        <View style ={actionSheetStyles.actionListItemView} //List Item
        >
          <Text style={actionSheetStyles.counterText}>7.1</Text>
          <Divider orientation='vertical' style={{marginHorizontal:5}}/>
          <View>
            <Text style={actionSheetStyles.termsBlackText}>Term : {`\n`}
            Unless otherwise specified in a Service Order Form, this Agreement, as may be updated from time to time, will commence on the date it is accepted by you and shall continue unless terminated.</Text>
          <Text style={actionSheetStyles.termsBlackText}></Text>
          </View>
        </View>
        <View style ={actionSheetStyles.actionListItemView} //List Item
        >
          <Text style={actionSheetStyles.counterText}>7.2</Text>
          <Divider orientation='vertical' style={{marginHorizontal:5}}/>
          <View>
            <Text style={actionSheetStyles.termsBlackText}>Termination for breach by either Party :</Text>
          <Text style={actionSheetStyles.termsBlackText}>Either Party shall be entitled to terminate this Agreement in the event that: </Text>
          <Text style={actionSheetStyles.termsBlackText}>(i) the other Party; commits a breach of any of its material obligations herein and fails to remedy such breach within thirty (30) Days after delivery of written notice thereof from the non-defaulting Party; </Text>
          <Text style={actionSheetStyles.termsBlackText}>(ii) if it repeatedly breaches any of the terms of this Agreement in such a manner as to reasonably justify the opinion that its conduct is inconsistent with it having the intention or ability to give effect to the terms of this Agreement; or</Text>
          <Text style={actionSheetStyles.termsBlackText}>(iii) the other Party becomes insolvent.</Text>
          </View>
        </View>
        <View style ={actionSheetStyles.actionListItemView} //List Item
        >
          <Text style={actionSheetStyles.counterText}>7.3</Text>
          <Divider orientation='vertical' style={{marginHorizontal:5}}/>
          <View>
            <Text style={actionSheetStyles.termsBlackText}>Termination by Dohwe</Text>
          <Text style={actionSheetStyles.termsBlackText}>7.3.1 Dohwe shall be entitled to terminate the Agreement with immediate effect by serving written notice to you</Text>
          <Text style={actionSheetStyles.termsBlackText}>if you breach the Agreement  or</Text>
          <Text style={actionSheetStyles.termsBlackText}>Where Dohwe has been instructed to cease providing the Services by a Regulator or by any other competent authority, or if an agreement between Dohwe and a Service Provider integral to the provision of the Services is terminated.</Text>
          </View>
        </View>
        <View style ={actionSheetStyles.actionListItemView} //List Item
        >
          <Text style={actionSheetStyles.counterText}>7.4</Text>
          <Divider orientation='vertical' style={{marginHorizontal:5}}/>
          <View>
            <Text style={actionSheetStyles.termsBlackText}>Termination for convenience</Text>
          <Text style={actionSheetStyles.termsBlackText}>A Party may terminate this Agreement without cause by serving thirty (30) days’ written notice of termination on the other Party.</Text>
          </View>
        </View>
        <View style ={actionSheetStyles.actionListItemView} //List Item
        >
          <Text style={actionSheetStyles.counterText}>7.5</Text>
          <Divider orientation='vertical' style={{marginHorizontal:5}}/>
          <View>
            <Text style={actionSheetStyles.termsBlackText}>Consequences of termination of the Agreement</Text>
          <Text style={actionSheetStyles.termsBlackText}>7.5.1Dohwe shall deactivate the Services immediately upon expiry of the notice period provided in the notice of termination issued by either Party or immediately on receipt of a notice of termination from either Party where no period has been provided (the Effective Termination Date).</Text>
          <Text style={actionSheetStyles.termsBlackText}>7.5.2 The termination of the Agreement shall not affect any rights, remedies, obligations or liabilities of the Parties that have accrued up to the Effective Termination Date, including the right to claim damages in respect of any breach of the Agreement which existed at or before the Effective Termination Date.</Text>
          <Text style={actionSheetStyles.termsBlackText}>7.5.3 Upon termination of this Agreement, any amounts in respect of the Service charges payable which have not been paid shall be paid in full by you within thirty (30) days (including weekends) of the Effective Termination Date. Failure to do so interests stipulated in 3.2 apply.</Text>
          <Text style={actionSheetStyles.termsBlackText}>7.5.4 Where applicable, Dohwe will refund the following amounts paid by you less any deductions which Dohwe is entitled to charge to you under this Agreement within thirty (30) days of the Effective Termination Date:</Text>
          <Text style={actionSheetStyles.termsBlackText}>The deposit (if any) paid by you (without interest); and</Text>
          <Text style={actionSheetStyles.termsBlackText}>The value of any credited but unutilized amount in the Wallet.</Text>
          
          </View>
        </View>
  
        </View>
      }



<View style={{flexDirection:'row', justifyContent:'space-between', marginTop:15}}>
      <Text style={{...actionSheetStyles.actionTitleLeft, }}>8. Indemnification</Text>
      <Icon name={shown8?'chevron-up':'chevron-down'} type='ionicon' style={{marginRight:25, marginTop:5}} onPress={()=>{setShown8(!shown8)}}></Icon>
      </View>
      {shown8 &&
              <View style={actionSheetStyles.actionClearView}>
              <View style ={actionSheetStyles.actionListItemView} //List Item
              >
                <Text style={actionSheetStyles.counterText}>8.1</Text>
                <Divider orientation='vertical' style={{marginHorizontal:5}}/>
                <Text style={actionSheetStyles.termsBlackText}>Subject to the limitations in clause 11, each Party (the Indemnifying Party) hereby agrees to indemnify, defend, protect and hold harmless the other Party (the Indemnified Party) and its Affiliates, from and against, and to assume liability for any loss, damage, expense or cost (including, without limitation, reasonable attorneys’ fees and expenses) arising out of or in connection with: (i) an infringement by the Indemnifying Party of a third party’s Intellectual Property (ii) any violation by the Indemnifying Party of any applicable law or governmental regulation; and (iii) any material breach by the Indemnifying Party of its obligations under this Agreement. You further agree to indemnify Dohwe and its affiliates against any claims of whatever nature by third parties arising from or due to your use of the Services.</Text>
              </View>
              <View style ={actionSheetStyles.actionListItemView} //List Item
              >
                <Text style={actionSheetStyles.counterText}>8.2</Text>
                <Divider orientation='vertical' style={{marginHorizontal:5}}/>
                <Text style={actionSheetStyles.termsBlackText}>Nothing in this Agreement excludes or is intended to exclude either party’s liability for fraud caused by the actions or omissions of such Party or its Representatives.</Text>
              </View>
        
              </View>
      }


<View style={{flexDirection:'row', justifyContent:'space-between', marginTop:15}}>
      <Text style={{...actionSheetStyles.actionTitleLeft, }}>9. Limitation of Liability</Text>
      <Icon name={shown9?'chevron-up':'chevron-down'} type='ionicon' style={{marginRight:25, marginTop:5}} onPress={()=>{setShown9(!shown9)}}></Icon>
      </View>
      { shown9 &&
              <View style={actionSheetStyles.actionClearView}>
              <View style ={actionSheetStyles.actionListItemView} //List Item
              >
                <Text style={actionSheetStyles.termsBlackText}>Except as provided in clause 10, in no event shall either Party be liable to the other Party for any consequential, special or indirect losses or damage sustained by either party or any third parties in using the Services, howsoever arising whether under contract, tort or otherwise (including, without limitation, third party claims, loss of business or profit, loss of, customers, loss of data or information, cost of substitute performance, or damage to reputation or goodwill) even if it has been advised of the possibility of such damages.</Text>
              </View>
        
              </View>
      }



<View style={{flexDirection:'row', justifyContent:'space-between', marginTop:15}}>
      <Text style={{...actionSheetStyles.actionTitleLeft, }}>10. Notices</Text>
      <Icon name={shown10?'chevron-up':'chevron-down'} type='ionicon' style={{marginRight:25, marginTop:5}} onPress={()=>{setShown10(!shown10)}}></Icon>
      </View>
      { shown10 &&
              <View style={actionSheetStyles.actionClearView}>
              <View style ={actionSheetStyles.actionListItemView} //List Item
              >
                <Text style={actionSheetStyles.termsBlackText}>You hereby authorize Dohwe to send notices to you relating to this Agreement (e.g., Service updates, notices of breach and/or suspension) via email to the email address you provide to us in the Service Order Form, and if no Service Order Form has been executed, to the email address linked to your User Account. It is your responsibility to keep your email address current, and you will be deemed to have received any email sent to the last known email address Dohwe has on record for you. Notices that Dohwe sends to you via email will be deemed effective upon Dohwe’s sending of the email. Notices provided to Dohwe under this Agreement shall be sent to the attention of product@dohwe.com.</Text>
              </View>
        
              </View>
      }



<View style={{flexDirection:'row', justifyContent:'space-between', marginTop:15}}>
      <Text style={{...actionSheetStyles.actionTitleLeft, }}>11. Variation</Text>
      <Icon name={shown11?'chevron-up':'chevron-down'} type='ionicon' style={{marginRight:25, marginTop:5}} onPress={()=>{setShown11(!shown11)}}></Icon>
      </View>
      {shown11 &&
              <View style={actionSheetStyles.actionClearView}>
              <View style ={actionSheetStyles.actionListItemView} //List Item
              >
                <Text style={actionSheetStyles.termsBlackText}>No variation, amendment or any alteration to any of the terms and conditions of this Agreement shall be of any force or effect unless they have been reduced to writing and have been duly signed by the Parties. The Parties agree that no other terms or conditions, whether oral or written, and whether express or implied, apply to this Agreement.</Text>
              </View>
        
              </View>

      }



<View style={{flexDirection:'row', justifyContent:'space-between', marginTop:15}}>
      <Text style={{...actionSheetStyles.actionTitleLeft, }}>12. Waiver</Text>
      <Icon name={shown12?'chevron-up':'chevron-down'} type='ionicon' style={{marginRight:25, marginTop:5}} onPress={()=>{setShown12(!shown12)}}></Icon>
      </View>
      {shown12 &&
              <View style={actionSheetStyles.actionClearView}>
              <View style ={actionSheetStyles.actionListItemView} //List Item
              >
                <Text style={actionSheetStyles.termsBlackText}>No waiver of any of the terms and conditions of this Agreement will be binding for any purpose unless expressed in writing and signed by the Party giving the same, and any such waiver will be effective only in a specific instance and for the purpose given. No failure or delay on the part of either Party in exercising any right, power or privilege will operate as a waiver, nor will any single or partial exercise of any right, power or privilege.</Text>
              </View>
        
              </View>

      }



<View style={{flexDirection:'row', justifyContent:'space-between', marginTop:15}}>
      <Text style={{...actionSheetStyles.actionTitleLeft, }}>13. Severability</Text>
      <Icon name={shown13?'chevron-up':'chevron-down'} type='ionicon' style={{marginRight:25, marginTop:5}} onPress={()=>{setShown13(!shown13)}}></Icon>
      </View>
      {
        shown13 &&
        <View style={actionSheetStyles.actionClearView}>
        <View style ={actionSheetStyles.actionListItemView} //List Item
        >
          <Text style={actionSheetStyles.termsBlackText}>If any term of this Agreement is to any extent illegal, otherwise invalid or incapable of being enforced, such term shall be excluded to the extent of such invalidity or unenforceability; all other terms shall remain in full force and effect; and to the extent permitted and possible, the invalid or unenforceable term shall be deemed replaced by a term that is valid and enforceable and that comes closest to expressing the intention of such invalid or unenforceable term.</Text>
        </View>
  
        </View>
      }



<View style={{flexDirection:'row', justifyContent:'space-between', marginTop:15}}>
      <Text style={{...actionSheetStyles.actionTitleLeft, }}>14. Entire Agreement</Text>
      <Icon name={shown14?'chevron-up':'chevron-down'} type='ionicon' style={{marginRight:25, marginTop:5}} onPress={()=>{setShown14(!shown14)}}></Icon>
      </View>
      {shown14 &&
              <View style={actionSheetStyles.actionWhiteView}>
              <View style ={actionSheetStyles.actionListItemView} //List Item
              >
                <Text style={actionSheetStyles.termsBlackText}>14.1 This Agreement including the additional terms, policies and agreements indicated in clause 2.3, constitutes the entire agreement of the Parties and it supersedes any prior written or oral agreements between the Parties. In case of any ambiguity or conflict between this Agreement, and a Service Order Form, this Agreement shall take precedence except where the dispute is in relation to the applicable pricing.</Text>
              </View>
        
              </View>
      }



<View style={{flexDirection:'row', justifyContent:'space-between', marginTop:15}}>
      <Text style={{...actionSheetStyles.actionTitleLeft, }}>15. Assignment</Text>
      <Icon name={shown15?'chevron-up':'chevron-down'} type='ionicon' style={{marginRight:25, marginTop:5}} onPress={()=>{setShown15(!shown15)}}></Icon>
      </View>
      { shown15 &&
              <View style={actionSheetStyles.actionClearView}>
              <View style ={actionSheetStyles.actionListItemView} //List Item
              >
                <Text style={actionSheetStyles.termsBlackText}>Parties shall have provision to assign any or all of their rights and obligations under this Agreement without the prior written consent of the other Party.</Text>
              </View>
        
              </View>
      }

<View style={{flexDirection:'row', justifyContent:'space-between', marginTop:15}}>
      <Text style={{...actionSheetStyles.actionTitleLeft, }}>16. Force Majeure Event</Text>
      <Icon name={shown16?'chevron-up':'chevron-down'} type='ionicon' style={{marginRight:25, marginTop:5}} onPress={()=>{setShown16(!shown16)}}></Icon>
      </View>
      { shown16 &&
        <View style={{...actionSheetStyles.actionClearView}}>

        <View style ={actionSheetStyles.actionListItemView} //List Item
        >
          <Text style={actionSheetStyles.counterText}>16.1</Text>
          <Divider orientation='vertical' style={{marginHorizontal:5}}/>
          <Text style={actionSheetStyles.termsBlackText}>Provided it has complied with notice requirements under this clause, if a Party is prevented, hindered or delayed in or from performing any of its obligations under this Agreement by a Force Majeure Event (the Affected Party), the Affected Party shall not be in breach of this Agreement or otherwise liable for any such failure or delay in the performance of such obligations. The time for performance of such obligations shall be extended accordingly.</Text>
        </View>
  
        <View style ={actionSheetStyles.actionListItemView} //List Item
        >
          <Text style={actionSheetStyles.counterText}>16.2</Text>
          <Divider orientation='vertical' style={{marginHorizontal:5}}/>
          <Text style={actionSheetStyles.termsBlackText}>The corresponding obligations of the other Party will be suspended, and it’s time for performance of such obligations extended, to the same extent as those of the Affected Party.</Text>
        </View>
        <View style ={actionSheetStyles.actionListItemView} //List Item
        >
          <Text style={actionSheetStyles.counterText}>16.3</Text>
          <Divider orientation='vertical' style={{marginHorizontal:5}}/>
          <View>
            <Text style={actionSheetStyles.termsBlackText}>The Affected Party shall:</Text>
          <Text style={actionSheetStyles.termsBlackText}><Text style={actionSheetStyles.counterText}>16.3.1</Text> as soon as reasonably practicable after the start of the Force Majeure Event but no later than ten (10) days from its start, notify the other Party in writing of the Force Majeure Event, the date on which it started, it’s likely or potential duration, and the effect of the Force Majeure Event on its ability to perform any of its obligations under the Agreement; and</Text>
          <Text style={actionSheetStyles.termsBlackText}><Text style={actionSheetStyles.counterText}>16.3.2</Text> Use all reasonable endeavors to mitigate the effect of the Force Majeure Event on the performance of its obligations.</Text>
          </View>
        </View>
        <View style ={actionSheetStyles.actionListItemView} //List Item
        >
          <Text style={actionSheetStyles.counterText}>16.4</Text>
          <Divider orientation='vertical' style={{marginHorizontal:5}}/>
          <Text style={actionSheetStyles.termsBlackText}>If the Force Majeure Event prevents, hinders or delays the Affected Party’s performance of its obligations for a continuous period of more than two (2) months, the Party not affected by the Force Majeure Event may terminate this Agreement by giving thirty (30) days written notice to the Affected Party.</Text>
        </View>
        </View>
      }

<View style={{flexDirection:'row', justifyContent:'space-between', marginTop:15}}>
      <Text style={{...actionSheetStyles.actionTitleLeft, }}>17. Governing Law and Dispute Resolution</Text>
      <Icon name={shown17?'chevron-up':'chevron-down'} type='ionicon' style={{marginRight:25, marginTop:5}} onPress={()=>{setShown17(!shown17)}}></Icon>
      </View>
      { shown17 &&
        <View style={{...actionSheetStyles.actionClearView}}>

        <View style ={actionSheetStyles.actionListItemView} //List Item
        >
          <Text style={actionSheetStyles.counterText}>17.1</Text>
          <Divider orientation='vertical' style={{marginHorizontal:5}}/>
          <View>
          <Text style={actionSheetStyles.termsBlackText}>Governing Law</Text>
          <Text style={actionSheetStyles.termsBlackText}>This Agreement and any non-contractual obligations arising out of or in connection with it shall be governed by, and construed in accordance with the laws of Zimbabwe. </Text>
          </View>
        </View>
  
        <View style ={actionSheetStyles.actionListItemView} //List Item
        >
          <Text style={actionSheetStyles.counterText}>17.2</Text>
          <Divider orientation='vertical' style={{marginHorizontal:5}}/>
          <View>
          <Text style={actionSheetStyles.termsBlackText}>Amicable Settlement</Text>
          <Text style={actionSheetStyles.termsBlackText}><Text style={actionSheetStyles.counterText}>17.2.1</Text> The Parties shall use their best efforts to settle amicably any dispute arising from or in connection with this Agreement through good faith negotiations between the senior officers of the Parties. The Party seeking resolution of a dispute will first give notice in writing to the other Party, setting forth the nature of the dispute and a concise statement of the issues to be resolved.</Text>
          <Text style={actionSheetStyles.termsBlackText}><Text style={actionSheetStyles.counterText}>17.2.2</Text> All information exchanged during this meeting or any subsequent dispute resolution process, shall be regarded as “without prejudice” communications for the purpose of settlement negotiations and shall be treated as confidential by the Parties and their Representatives, unless otherwise required by law. However, evidence that is independently admissible or discoverable shall not be rendered inadmissible or non-discoverable by virtue of its use during the dispute resolution process.</Text>
          </View>

        </View>
        <View style ={actionSheetStyles.actionListItemView} //List Item
        >
          <Text style={actionSheetStyles.counterText}>17.3</Text>
          <Divider orientation='vertical' style={{marginHorizontal:5}}/>
          <View>
            <Text style={actionSheetStyles.termsBlackText}>Arbitration</Text>
          <Text style={actionSheetStyles.termsBlackText}><Text style={actionSheetStyles.counterText}>17.3.1</Text> If the dispute has not been settled amicably within thirty (30) days (or such longer period as may be agreed upon between the Parties) from when the dispute resolution process was instituted, a Party may elect to refer the dispute to arbitration for final resolution in Harare, Zimbabwe.</Text>
          <Text style={actionSheetStyles.termsBlackText}><Text style={actionSheetStyles.counterText}>17.3.2</Text> Where a Party elects to commence arbitration proceedings, such arbitration shall be determined by a single arbitrator to be appointed by agreement between the Parties or, in default of such agreement, within fourteen (14) days of the notification of a dispute, the arbitrator shall be appointed upon the application of either Party, by a Judge of the Supreme Court of Harare.</Text>
          <Text style={actionSheetStyles.termsBlackText}><Text style={actionSheetStyles.counterText}>17.3.3</Text> The arbitration shall be conducted in Harare, Zimbabwe. The language of the arbitration shall be English.</Text>
          <Text style={actionSheetStyles.termsBlackText}><Text style={actionSheetStyles.counterText}>17.3.4</Text> The award of the arbitrator shall be final and binding upon the Parties and any Party may apply to a court of competent jurisdiction for enforcement of such award.</Text>
          <Text style={actionSheetStyles.termsBlackText}><Text style={actionSheetStyles.counterText}>17.3.5</Text> Notwithstanding the foregoing, a Party is entitled to seek preliminary injunctive relief or interim or conservatory measures from any court of competent jurisdiction pending the final decision or award of the arbitrator.</Text>
          </View>
        </View>
        <View style ={actionSheetStyles.actionListItemView} //List Item
        >
          <Text style={actionSheetStyles.counterText}>16.4</Text>
          <Divider orientation='vertical' style={{marginHorizontal:5}}/>
          <Text style={actionSheetStyles.termsBlackText}>If the Force Majeure Event prevents, hinders or delays the Affected Party’s performance of its obligations for a continuous period of more than two (2) months, the Party not affected by the Force Majeure Event may terminate this Agreement by giving thirty (30) days written notice to the Affected Party.</Text>
        </View>
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