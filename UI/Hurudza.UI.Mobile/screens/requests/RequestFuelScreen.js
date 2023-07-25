import React, { Component } from 'react';
import { Animated, Dimensions, View, StyleSheet, Platform } from 'react-native';
import { connect } from 'react-redux';
import { Input , Icon, ButtonGroup, ListItem } from 'react-native-elements';

import ParallaxContainer from '../../components/ParallaxContainer'
import { CustomButton } from '../../components/Buttons'
import { MontserratText } from '../../components/StyledText'
import { MontserratListItem } from '../../components/StyledTextListItem'
import DropdownAlert from 'react-native-dropdownalert';

import { productActions } from '../../actions/product';
import { requestActions } from '../../actions/request';
import { alertActions } from '../../actions/alert';
import colors from '../../constants/Colors';
import * as Font from 'expo-font';

class RequestFuelScreen extends Component {
  constructor(props){
    super(props) 

    this.state = {
      liters: 0,
      sendTo: '',
      ecocashNumber: '',
      fontLoaded:false,
      selectedIndex: -1,
      description: ''
    };
  }

  updateIndex = (selectedIndex) => {
    const { navigation } = this.props;
    this.setState({selectedIndex})
  }

  UNSAFE_componentWillMount(){
    const { dispatch } = this.props;
    dispatch(productActions.getAll())
  }

  handlePressSelectFuelType = () => {
    const { navigation } = this.props;
    navigation.navigate('SelectFuelTypeBuy', { title: 'Base Currency', type: 'base', back: 'RequestFuel' });
  }

  handlePressSelectCard = () => {
    const { navigation } = this.props;
    navigation.navigate('SelectCardBuy', { title: 'Base Currency', type: 'base', back: 'BuyRequest' });
  }

  _sendRequestAsync = () => {
    const { dispatch, selectedCurrency, selectedOmc, selectedFuelType, navigation, user } = this.props
    const { liters, description, sendTo } = this.state
    dispatch(requestActions.add({
      description: description,
      productId: selectedFuelType.id,
      liters: liters,
      requestTo: sendTo,
      requestFrom: user.userName
    }))
  }

  componentDidUpdate(){
    const { navigation, type, message, alertWithType, dispatch, addedRequest } = this.props;
    if(type === 'alert-danger'){
      this.dropDownAlertRef.alertWithType('error', 'Error', message);
      dispatch(alertActions.clear())
    } else if(type === 'alert-success'){
      this.dropDownAlertRef.alertWithType('success', 'Success', message);
      dispatch(alertActions.clear())
    }

    if(addedRequest){
      dispatch(requestActions.clear())
      navigation.goBack(null)
    }
 
  }
 
  _renderContent = () => {
    const { selectedFuelType, addingRequest } = this.props
    const { liters, sendTo, description } = this.state;
    const isValid = liters !== 0
      && sendTo !== ''
      && selectedFuelType.id !== 0 

      let litersLabel = 'Liters'
      let buttonLabel = 'Send Request'

    return (
      <Animated.View 
        style={{
          height:  Dimensions.get('window').height - (Platform.OS === 'ios' ? 210 : 115)  
        }}
      >
        <View>

          <View style={{
              paddingHorizontal: 10,
              paddingTop: 10,
            }}>
            <Input
              inputContainerStyle={styles.spacing}
              ref={(ref) => this.sendToInputRef = ref}
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
              returnKeyType='done'
              placeholder='0773727342'
              inputStyle={{
                fontSize: 14,
                fontFamily: this.state.fontLoaded ? 'montserrat-regular' : null,
              }}
            />

            <ListItem
              title={
                <MontserratText>Fuel Type</MontserratText>
              }
              rightTitle={
                <MontserratText>{selectedFuelType.name}</MontserratText>
              }
              rightTitleStyle={{ fontSize: 15 }}
              onPress={() => this.handlePressSelectFuelType()}
              containerStyle={{...styles.spacing20, marginBottom: 20}}
              rightIcon={
                <Icon
                  name='chevron-down-box'
                  type='material-community'
                  size={24} 
                  color={selectedFuelType.id > 0 ? colors.accentColor : colors.lightGray2}
                />
              }
            />

            <Input
              inputContainerStyle={styles.spacing}
              ref={(ref) => this.litersInputRef = ref}
              withRef={true}
              onChangeText={(value) => this.setState({ liters: value })}
              onSubmitEditing={() => this.noteInputRef.focus()}
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
              returnKeyType='next'
              placeholder='25'
              inputStyle={{
                fontSize: 14,
                fontFamily: this.state.fontLoaded ? 'montserrat-regular' : null,
              }}
            />

            <Input
              inputContainerStyle={styles.spacing}
              ref={(ref) => this.noteInputRef = ref}
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
              returnKeyType='done'
              placeholder='Send request with a note'
              inputStyle={{
                fontSize: 14,
                fontFamily: this.state.fontLoaded ? 'montserrat-regular' : null,
              }}
            />

            <CustomButton
              onPress={() => this._sendRequestAsync()}
              isEnabled={isValid} 
              isLoading={addingRequest}
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
        navBarTitle='Request Fuel'
        headerTitle="Request Fuel" 
        headerSubTitle="Fuel is sent as a request."
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
  const { requests, loading, addingRequest, addedRequest, selectedRequest } = state.requests
  const { selectedFuelType } = state.products
  const { type, message } = state.alert
  return {
      user,
      requests,
      selectedRequest,
      addingRequest,
      addedRequest,
      loading,
      selectedFuelType,
      type,
      message,
      primaryColor: state.themes.primaryColor,
  };
}

export default connect(mapStateToProps)(RequestFuelScreen);