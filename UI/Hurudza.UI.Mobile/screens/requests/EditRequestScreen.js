import React, { Component, createRef } from 'react';
import { Animated, Dimensions, View, StyleSheet, Platform, Text } from 'react-native';
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

class EditRequestScreen extends Component {
  constructor(props){
    super(props) 

    this.litersInputRef = createRef()
    this.noteInputRef = createRef()

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
    const { dispatch, products, selectedRequest } = this.props;
    this.setState({liters: selectedRequest.liters, description: selectedRequest.description})
    let selectedFuelType = products.filter(f => f.id === selectedRequest.productId);
    dispatch(productActions.select(selectedFuelType[0]))
  }

  handlePressSelectFuelType = () => {
    const { navigation } = this.props;
    navigation.navigate('SelectFuelTypeBuy', { title: 'Base Currency', type: 'base', back: 'RequestFuel' });
  }

  _sendRequestAsync = () => {
    const { dispatch, selectedFuelType, selectedRequest } = this.props
    const { liters, description } = this.state
    dispatch(requestActions.updateRequest({
      id: selectedRequest.id,
      description: description,
      productId: selectedFuelType.id,
      liters: liters,
      requestFrom: selectedRequest.requestFrom,
      requestTo: selectedRequest.requestTo,
      isRead: false,
      granted: false,
      declined: false,
      parentCouponId:null,
      grantedLiters:0
    }))
  }

  componentDidUpdate(){
    const { navigation, type, message, dispatch, updatedRequest } = this.props;
    if(type === 'alert-danger'){
      this.dropDownAlertRef.alertWithType('error', 'Error', message);
      dispatch(alertActions.clear())
    } else if(type === 'alert-success'){
      this.dropDownAlertRef.alertWithType('success', 'Success', message);
      dispatch(alertActions.clear())
    }

    if(updatedRequest){
      dispatch(requestActions.clear())
      navigation.goBack(null)
    }
 
  }
 
  _renderContent = () => {
    const { selectedFuelType, updatingRequest, selectedRequest } = this.props
    const { liters, description } = this.state;
    const isValid = liters !== 0 && selectedFuelType.id !== 0 

    let litersLabel = 'Liters'
    let buttonLabel = 'Update Request'

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
                        <Text style={styles.activityItemTitlePrefix}>to: </Text>
                        <Text style={styles.activityItemTitle}>{selectedRequest.requestToUserName}</Text>
                      </View>
                    </View>
                  </View>
                </View>
              </ListItem.Content>
          </ListItem>
          <View style={{
              paddingHorizontal: 10,
              paddingTop: 10,
            }}>

            <View style={{...styles.shadow, marginTop: 5, marginBottom: 10 }}>
              <ListItem onPress={() => this.handlePressSelectFuelType()}>
                <ListItem.Content>
                  <ListItem.Title style={styles.title}>{selectedFuelType.name}</ListItem.Title>
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
                ref={this.litersInputRef}
                withRef={true}
                value={liters.toString()}
                onChangeText={(value) => this.setState({ liters: value })}
                onSubmitEditing={() => this.noteInputRef.current.focus()}
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
                returnKeyType='done'
                inputStyle={{
                  fontSize: 14,
                  fontFamily: 'montserrat-regular',
                }}
              />
            </View>

            <View style={{...styles.shadow, marginBottom: 20, padding:5, backgroundColor: colors.white}}>
              <Input
                ref={this.noteInputRef}
                withRef={true}
                value={description}
                placeholder='Reason for fuel request'
                onChangeText={(value) => this.setState({ description: value })}
                label={
                  <MontserratText style={[
                    styles.inputLabel,
                    {
                      fontWeight: litersLabel !== 'Liters' ? '500' : '100',
                      color: litersLabel !== 'Liters' ? colors.accentColor : colors.black
                    }
                  ]}>Reason (optional)</MontserratText>
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
                inputStyle={{
                  fontSize: 14,
                  fontFamily: 'montserrat-regular',
                }}
              />
            </View>

            <CustomButton
              onPress={() => this._sendRequestAsync()}
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
  const { requests, loading, updatingRequest, updatedRequest, selectedRequest } = state.requests
  const { selectedFuelType, products } = state.products
  const { type, message } = state.alert
  return {
      user,
      requests,
      selectedRequest,
      updatingRequest,
      updatedRequest,
      loading,
      products,
      selectedFuelType,
      type,
      message,
      primaryColor: state.themes.primaryColor,
  };
}

export default connect(mapStateToProps)(EditRequestScreen);