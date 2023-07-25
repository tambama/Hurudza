import React, { Component } from 'react';
import { Animated, Dimensions, View, StyleSheet, Platform } from 'react-native';
import { connect } from 'react-redux';
import { Input , Icon, ButtonGroup, ListItem } from 'react-native-elements';

import ParallaxContainer from '../../components/ParallaxContainer'
import { CustomButton } from '../../components/Buttons'
import { MontserratText } from '../../components/StyledText'
import { MontserratListItem } from '../../components/StyledTextListItem'
import DropdownAlert from 'react-native-dropdownalert';

import { omcActions } from '../../actions/omc';
import { productActions } from '../../actions/product';
import { couponActions } from '../../actions/coupon';
import { bankCardActions } from '../../actions/bankCard';
import { alertActions } from '../../actions/alert';
import colors from '../../constants/Colors';
import * as Font from 'expo-font';

class SendFuelScreen extends Component {
  constructor(props){
    super(props) 

    this.state = {
      liters: 0,
      description: '',
      sendTo: '',
      ecocashNumber: '',
      selectedIndex: -1
    };
  }

  updateIndex = (selectedIndex) => {
    const { navigation } = this.props;
    this.setState({selectedIndex}) 
    if(selectedIndex === 3){
      navigation.navigate('SelectCardBuy', { title: 'Base Currency', type: 'base', back: 'BuyFuel' });
    }
    if(selectedIndex === 2){
      navigation.navigate('SelectCoupon', { title: 'Base Currency', type: 'base', back: 'SendFuel' });
    }
  }

  UNSAFE_componentWillMount(){
    const { dispatch, user } = this.props;
    dispatch(couponActions.getUserCoupons(user.id));
    dispatch(omcActions.getOmcsWithCouponPrices())
    dispatch(productActions.getAll())
    dispatch(bankCardActions.getUserBankCards(user.id))
  }

  handlePressSelectCurrency = () => {
    const { navigation } = this.props;
    navigation.navigate('SelectCurrency', { title: 'Base Currency', type: 'base', back: 'SendFuel' });
  }

  handlePressSelectFuelType = () => {
    const { navigation } = this.props;
    navigation.navigate('SelectFuelTypeBuy', { title: 'Base Currency', type: 'base', back: 'SendFuel' });
  }

  handlePressSelectCoupon = () => {
    const { navigation } = this.props;
    navigation.navigate('SelectCoupon', { title: 'Base Currency', type: 'base', back: 'SendFuel' });
  }

  handlePressSelectCard = () => {
    const { navigation } = this.props;
    navigation.navigate('SelectCardBuy', { title: 'Base Currency', type: 'base', back: 'BuyCoupon' });
  }

  _shareCouponAsync = () => {
    const { dispatch, selectedCoupon, user } = this.props
    const { liters, description, sendTo } = this.state
    dispatch(couponActions.add({
      name: `${selectedCoupon.omc} ${selectedCoupon.currency} ${selectedCoupon.product}`,
      description: description,
      ownerId: sendTo,
      omcId: selectedCoupon.omcId, 
      productId: selectedCoupon.productId,
      couponPriceId: selectedCoupon.couponPriceId,
      liters: liters,
      currencyId: selectedCoupon.currencyId,
      boughtById: user.id,
      parentCouponId: selectedCoupon.id
    }))
  }

  _buyAsync = () => {
    const { dispatch, selectedCurrency, selectedOmc, selectedFuelType, couponPrices, navigation } = this.props
    const { liters, description } = this.state
    var couponPriceId = couponPrices.filter(p => p.currencyId === selectedCurrency.id)[0].id;
    dispatch(couponActions.add({
      name: `${selectedOmc.name} ${selectedFuelType.name} ${selectedCurrency.name}`,
      description: description,
      ownerId: '6be26c70-afa0-4fd2-8759-ac412d4053e0',
      omcId: selectedOmc.id, 
      productId: selectedFuelType.id,
      couponPriceId: couponPriceId,
      liters: liters,
      currencyId: selectedCurrency.id,
      boughtById: '6be26c70-afa0-4fd2-8759-ac412d4053e0'
    }))
  }

  goToBuy = () => {
    const { navigation } = this.props
    navigation.navigate('Coupons')
  }

  componentDidUpdate(){
    const { updated, navigation, type, message, alertWithType, dispatch, addedCoupon } = this.props;
    if(type === 'alert-danger'){
      this.dropDownAlertRef.alertWithType('error', 'Error', message);
      dispatch(alertActions.clear())
    } else if(type === 'alert-success'){
      this.dropDownAlertRef.alertWithType('success', 'Success', message);
      dispatch(alertActions.clear())
    }

    if(addedCoupon){
      dispatch(couponActions.clear());
      navigation.goBack(null);
    }
  }
 
  _renderContent = () => {
    const { selectedCurrency, selectedFuelType, addingCoupon, couponPriceChanged, couponPrices, selectedBankCard, selectedCoupon } = this.props
    const buttons = ['Ecocash', 'Wallet', 'Coupon', 'Card']
    const { liters, sendTo, ecocashNumber, selectedIndex } = this.state;
    const isValid = selectedFuelType.id !== 0 
      && ((selectedIndex === 0 && ecocashNumber !== '') || (selectedIndex === 2 && selectedCoupon.id !== 0) || (selectedIndex === 3 && selectedBankCard.id !== 0))
      && liters !== 0
      && sendTo !== ''

      let litersLabel = 'Liters'
      let buttonLabel = 'Send'
      if(couponPriceChanged && selectedCurrency.id !== 0){
        var prices = couponPrices.filter(p => p.currencyId === selectedCurrency.id && p.productId === selectedFuelType.id)
        if(prices.length > 0){
          var price = prices[0].price;
          var litersPrice = liters * price;
          litersLabel = `${liters} Liters = ${selectedCurrency.name} ${litersPrice.toFixed(2)}`;
        }
      }
    return (
      <Animated.View 
        style={{
          height:  Dimensions.get('window').height - (Platform.OS === 'ios' ? 210 : 115) 
        }}
      >
        <View>

          <ListItem
            title={
              <MontserratText>Fuel Type</MontserratText>
            }
            rightTitle={
              <MontserratText>{selectedFuelType.name}</MontserratText>
            }
            rightTitleStyle={{ fontSize: 15 }}
            onPress={() => this.handlePressSelectFuelType()}
            containerStyle={styles.spacing}
            rightIcon={
              <Icon
                name='chevron-down-box'
                type='material-community'
                size={24} 
                color={selectedFuelType.id > 0 ? colors.accentColor : colors.lightGray2}
              />
            }
          />

          <View style={{
              paddingHorizontal: 10
            }}>
            <Input
              inputContainerStyle={styles.spacing}
              ref={(ref) => this.sendToInputRef = ref}
              onSubmitEditing={() => this.litersInputRef.focus()}
              withRef={true}
              onChangeText={(value) => this.setState({ sendTo: value })}
              label={
                <MontserratText style={styles.inputLabel}>Send To</MontserratText>
              }
              leftIcon={
                <Icon
                  name='person'
                  type='material'
                  size={24} 
                  color={colors.lightGray2}
                />
              }
              leftIconContainerStyle={styles.inputIconLeft}
              keyboardType='phone-pad'
              returnKeyType='next'
              placeholder='0773727342'
              inputStyle={{
                fontSize: 14,
                fontFamily: 'montserrat-regular',
              }}
            />
            <Input
              inputContainerStyle={styles.spacing}
              ref={(ref) => this.litersInputRef = ref}
              withRef={true}
              onSubmitEditing={() => this.descriptionInputRef.focus()}
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
              placeholder='25'
              returnKeyType='next'
              inputStyle={{
                fontSize: 14,
                fontFamily: 'montserrat-regular',
              }}
            />

            <Input
              inputContainerStyle={styles.spacing}
              ref={(ref) => this.descriptionInputRef = ref}
              withRef={true}
              onChangeText={(value) => this.setState({ description: value })}
              label={
                <MontserratText style={[
                  styles.inputLabel,
                  {
                    fontWeight: litersLabel !== 'Liters' ? '500' : '100',
                    color: litersLabel !== 'Liters' ? colors.accentColor : colors.black
                  }
                ]}>Note</MontserratText>
              }
              leftIcon={
                <Icon
                  name='file-document-outline'
                  type='material-community'
                  size={24} 
                  color={colors.lightGray2}
                />
              }
              leftIconContainerStyle={styles.inputIconLeft}
              placeholder='School run fuel...'
              inputStyle={{
                fontSize: 14,
                fontFamily: 'montserrat-regular',
              }}
            />

            {
              selectedFuelType.id !== 0 && (
                <View style={styles.spacing}>
                  <MontserratText style={{marginLeft:10}}>Select Payment Method:</MontserratText>
                  <ButtonGroup
                    onPress={this.updateIndex}
                    selectedIndex={selectedIndex}
                    buttons={buttons}
                    selectedButtonStyle={styles.selectedButton} 
                  />
                </View>
              )
            }

            {
              selectedIndex === 3 && selectedBankCard.id !== 0 && (
                <View style={styles.spacing}>
                  <MontserratListItem
                      title={selectedBankCard.bank}
                      subtitle={`Card •••• ${selectedBankCard.accountNumber.slice(-4)}`}
                      onPress={() => this.handlePressSelectCard()}
                      rightIcon={
                        <Icon
                          name='chevron-down-box'
                          type='material-community'
                          size={24} 
                          color={colors.accentColor}
                        />
                      }
                  />
                </View>
              )
            }

            {
              selectedIndex === 2 && selectedCoupon.id !== 0 && (
                <View style={styles.spacing}>
                  <MontserratListItem
                      title={selectedCoupon.name}
                      subtitle={`${selectedCoupon.litersBalance}L ${selectedCoupon.product}`}
                      onPress={() => this.handlePressSelectCoupon()} 
                      rightIcon={
                        <Icon
                          name='chevron-down-box'
                          type='material-community'
                          size={24} 
                          color={colors.accentColor}
                        />
                      }
                  />
                </View>
              )
            }

            {
              selectedIndex === 0 && (
                <Input
                  inputContainerStyle={styles.spacing}
                  ref={(ref) => this.ecocashNumberInputRef = ref}
                  withRef={true}
                  onChangeText={(value) => this.setState({ ecocashNumber: value })}
                  label={
                    <MontserratText style={[
                      styles.inputLabel,
                      {
                        fontWeight: litersLabel !== 'Liters' ? '500' : '100',
                        color: litersLabel !== 'Liters' ? colors.accentColor : colors.black
                      }
                    ]}>Ecocash number</MontserratText>
                  }
                  leftIcon={
                    <Icon
                      name={Platform.OS === 'ios' ? 'ios-phone-portrait' : 'md-phone-portrait'}
                      type='ionicon'
                      size={24} 
                      color={colors.lightGray2}
                    />
                  }
                  leftIconContainerStyle={styles.inputIconLeft}
                  keyboardType='numeric'
                  placeholder='0773727342'
                  inputStyle={{
                    fontSize: 14,
                    fontFamily: 'montserrat-regular',
                  }}
                />
              )
            }

            <CustomButton
              onPress={() => selectedBankCard.id !== 0 ? this._buyAsync() : this._shareCouponAsync()} 
              isEnabled={isValid} 
              isLoading={addingCoupon}
              buttonStyle={[
                styles.loginButton,
                {
                  backgroundColor: !isValid ? colors.lightGray1 : colors.accentColor
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
      </Animated.View>
    )
  }

  render(){
    return(
      <ParallaxContainer
        navigation={this.props.navigation} 
        children={this._renderContent()}
        navBarTitle='Send Fuel'
        headerTitle="Send Fuel" 
        headerSubTitle="Fuel is sent as a coupon."
        showFab={false}/>
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
  }
})

function mapStateToProps(state) {
  const { user } = state.authentication
  const { coupons, loading, addingCoupon, selectedCoupon, addedCoupon } = state.coupons
  const { selectedCurrency } = state.currencies
  const { selectedFuelType } = state.products
  const { couponPrices, couponPriceChanged } = state.couponPrices
  const { selectedOmc } = state.omcs 
  const { selectedBankCard } = state.bankCards
  const { type, message } = state.alert
  return {
      user,
      coupons,
      selectedCoupon,
      couponPrices,
      couponPriceChanged,
      addingCoupon,
      addedCoupon,
      loading,
      selectedCurrency,
      selectedFuelType,
      selectedOmc,
      selectedBankCard,
      type,
      message,
      primaryColor: state.themes.primaryColor,
  };
}

export default connect(mapStateToProps)(SendFuelScreen);