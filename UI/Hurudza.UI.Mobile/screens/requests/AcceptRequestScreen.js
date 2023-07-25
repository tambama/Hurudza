import React, { Component } from 'react';
import { Animated, Dimensions, View, StyleSheet, Platform, Text } from 'react-native';
import { connect } from 'react-redux';
import { Input , Icon, ButtonGroup, ListItem } from 'react-native-elements';
import moment from 'moment'

import ParallaxContainer from '../../components/ParallaxContainer'
import { CustomButton } from '../../components/Buttons'
import { MontserratText } from '../../components/StyledText'
import { MontserratListItem } from '../../components/StyledTextListItem'
import DropdownAlert from 'react-native-dropdownalert';

import { omcActions } from '../../actions/omc';
import { requestActions } from '../../actions/request';
import { productActions } from '../../actions/product';
import { couponActions } from '../../actions/coupon';
import { bankCardActions } from '../../actions/bankCard';
import { alertActions } from '../../actions/alert';
import colors from '../../constants/Colors';
import * as Font from 'expo-font';
import { walletActions } from '../../actions/wallet';

class AcceptRequestScreen extends Component {
  constructor(props){
    super(props) 

    this.state = {
      liters: 0,
      sendTo: '',
      ecocashNumber: '',
      selectedIndex: -1,
    };
  }

  handlePressSelectWallet = () => {
    const { navigation } = this.props;
    navigation.navigate('SelectWallet', { title: 'Select Wallet', type: 'base', back: 'AcceptRequest' });
  }

  _buyAsync = () => {
    const { dispatch, selectedRequest, selectedWallet } = this.props
    const { liters } = this.state
    
    dispatch(requestActions.grantWalletRequest({
      id: selectedRequest.id,
      grantedLiters: liters,
      walletId: selectedWallet.id
    }))
  }

  componentDidUpdate(){
    const { navigation, type, message, dispatch, updatedRequest, user } = this.props;

    if(type === 'alert-danger'){
      this.dropDownAlertRef.alertWithType('error', 'Error', message);
      dispatch(alertActions.clear());
    } else if(type === 'alert-success'){
      this.dropDownAlertRef.alertWithType('success', 'Success', message);
      dispatch(alertActions.clear());
    }

    if(updatedRequest){
      dispatch(requestActions.clear())
      dispatch(walletActions.getUserWallets(user.id))
      dispatch(requestActions.getUnreadUserRequests(user.id));
      navigation.navigate('Activity')
    }

  }
 
  _renderContent = () => {
    const { user, selectedWallet, selectedRequest, updatingRequest } = this.props
    const buttons = ['Wallet', 'Coupon']
    const { liters } = this.state;

    let walletName = 'Select Wallet';
    let walletBalance = '***'
    console.log(selectedWallet)
    if(selectedWallet.id > 0){
      walletName = `${selectedWallet.walletCode.match(new RegExp('.{1,4}', 'g')).join("-")}`
      walletBalance = `${selectedWallet.currency} Balance: ${selectedWallet.signedAmount}, ${selectedWallet.product}`
    }

    let requestor = user.id === selectedRequest.creatorId ? selectedRequest.requestToUserName : selectedRequest.requestFromUserName

    const isValid =  selectedWallet.id !== 0 && liters !== 0

    let litersLabel = 'Liters'
    let buttonLabel = 'Send'

    return (
      <View>
        <View>
          <ListItem
            key={selectedRequest.id}
            style={{...styles.activityItem}}>
              <ListItem.Content>
                <View style={styles.activityItemContent}>
                  <View style={styles.activityItemContentLeft}>
                    <View style={styles.activityItemContentMiddle}>
                      <View style={styles.activityItemTitleContainer}>
                        <Text style={styles.activityItemTitlePrefix}></Text>
                        <Text style={styles.activityItemTitle}>{requestor}</Text>
                      </View>
                      <Text style={styles.activityItemSubtitle}>{moment(selectedRequest.createdDate).calendar()}</Text>
                      <Text style={{fontWeight: '300'}}>{`Requested ${selectedRequest.liters} Liters of ${selectedRequest.product}.`}</Text>
                    </View>
                  </View>
                </View>
              </ListItem.Content>
          </ListItem>
          <View style={{
              paddingHorizontal: 10
            }}>
            <View style={{...styles.shadow, marginTop: 5, marginBottom: 10 }}>
              <ListItem onPress={() => this.handlePressSelectWallet()}>
                <ListItem.Content>
                  <ListItem.Title style={styles.title}>{walletName}</ListItem.Title>
                  <ListItem.Subtitle style={styles.subtitleStyle}>{walletBalance}</ListItem.Subtitle>
                </ListItem.Content>
                <Icon
                  name='chevron-down-box'
                  type='material-community'
                  size={24} 
                  color={colors.accentColor}
                />
              </ListItem>
            </View>

            <View style={{...styles.shadow, marginBottom: 20, padding:5, backgroundColor: colors.white}}>
              <Input
                ref={(ref) => this.litersInputRef = ref}
                withRef={true}
                onChangeText={(value) => this.setState({ liters: value })}
                label={
                  <MontserratText style={[
                    styles.inputLabel,
                    {
                      fontWeight: litersLabel !== 'Liters' ? '500' : '100',
                      color: litersLabel !== 'Liters' ? colors.accentColor : colors.black
                    }
                  ]}>{litersLabel}</MontserratText>
                }
                leftIcon={
                  <Icon
                    name='drop'
                    type='simple-line-icon'
                    size={24} 
                    color={colors.lightGray2}
                  />
                }
                leftIconContainerStyle={styles.inputIconLeft}
                keyboardType='decimal-pad'
                returnKeyType={'done'}
                placeholder='20'
                inputStyle={{
                  fontSize: 14,
                  fontFamily: 'montserrat-regular',
                }}
              />
            </View>

            <CustomButton
              onPress={() => this._buyAsync()}
              isEnabled={isValid} 
              isLoading={updatingRequest}
              buttonStyle={[
                styles.loginButton,
                {
                  backgroundColor: !isValid ? colors.lightGray1 : colors.primaryBlue
                }
              ]}
              textStyle={[
                styles.loginButtonText, 
                {
                  color: isValid ? colors.white : colors.black
                }
              ]}
              text={buttonLabel}
            />
          </View>
        </View>
        <DropdownAlert inactiveStatusBarStyle='dark-content' inactiveStatusBarBackgroundColor='#fff' ref={ref => this.dropDownAlertRef = ref} />
      </View>
    )
  }

  render(){
    return(
      this._renderContent()
    )
  }
}

const styles = StyleSheet.create({
  inputLabel: {
    color: colors.black 
  },
  inputIconLeft: {
    marginLeft: 0,
    marginRight: 20
  },
  loginButton: {
  },
  loginButtonText: {
    fontWeight: '300',
    color: Platform.OS === 'ios' || Platform.OS === 'android' ? colors.black : colors.white,
    fontSize: 14,
  },
  spacing: {
    marginBottom: 30
  },
  selectedButton: {
    backgroundColor: colors.accentColor
  },
  title: {
    fontFamily: 'montserrat-regular',
    fontSize: 15
  },
  subtitleStyle: {
    fontSize: 12,
    fontWeight:'400', 
    color: colors.accentColor,
  },
  activity:{
    height: '38%'
  },
  activityItemContent: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    width: '100%'
  },
  activityItemContentLeft: {
    flexDirection: 'row'
  },
  activityItemContentMiddle: {
    marginLeft: 15
  },
  activityTitleContainer: {
    flexDirection:'row',
    justifyContent:'space-between',
    paddingHorizontal: 15,
    backgroundColor: colors.white,
    paddingTop: 5,
    marginTop: 5,
  },
  activityTitle:{
    fontSize:18,
    fontWeight: '500'
  },
  activityTitleRight: {
    fontWeight: '200'
  },
  activityItem: {
    marginBottom: 5,
    marginTop: 5,
    marginHorizontal: 0,
  },
  activityItemTitleContainer:{
    flexDirection: 'row',
    marginBottom: 4
  },
  activityItemTitlePrefix: {
    fontWeight: '100'
  },
  activityItemTitle:{
    fontSize: 15,
    fontWeight: '500'
  },
  activityItemSubtitle: {
    fontSize: 12,
    fontWeight: '200'
  },
  activityItemRightText:{
    fontSize: 16,
    fontWeight: '300',
    alignSelf: 'center'
  },
  shadow: {
    shadowColor: colors.activeBlue,
    shadowOffset: {
      width: 0,
      height: 2,
    },
    shadowOpacity: 0.25,
    shadowRadius: 2.84,
    elevation: 2,
  }
})

function mapStateToProps(state) {
  const { user } = state.authentication
  const { coupons, loading, addingCoupon, selectedCoupon } = state.coupons
  const { selectedCurrency } = state.currencies
  const { selectedFuelType, products } = state.products
  const { couponPrices, couponPriceChanged } = state.couponPrices
  const { selectedOmc } = state.omcs 
  const { selectedWallet } = state.wallets
  const { requests, selectedRequest, updatingRequest, updatedRequest } = state.requests
  const { type, message } = state.alert
  return {
      user,
      coupons,
      requests,
      updatingRequest,
      updatedRequest,
      selectedRequest,
      selectedCoupon,
      couponPrices,
      couponPriceChanged,
      addingCoupon,
      loading,
      selectedCurrency,
      products,
      selectedFuelType,
      selectedOmc,
      selectedWallet,
      type,
      message,
      primaryColor: state.themes.primaryColor,
  };
}

export default connect(mapStateToProps)(AcceptRequestScreen);