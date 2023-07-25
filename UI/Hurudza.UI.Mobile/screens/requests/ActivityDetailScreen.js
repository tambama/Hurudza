import React, { Component } from 'react';
import { Animated, Dimensions, View, StyleSheet, Platform, TouchableOpacity, Text } from 'react-native';
import { connect } from 'react-redux';
import { Input , Icon, ListItem, Button } from 'react-native-elements';
import ActionSheet from 'react-native-actionsheet'

import ParallaxContainer from '../../components/ParallaxContainer'
import ActivityItem from '../../components/ActivityItem'
import { MontserratText } from '../../components/StyledText'
import { MontserratListItem } from '../../components/StyledTextListItem'
import { Separator } from '../../components/List'
import DropdownAlert from 'react-native-dropdownalert';

import { userActions } from '../../actions/user';
import { requestActions } from '../../actions/request';
import { alertActions } from '../../actions/alert';
import colors from '../../constants/Colors';

class ActivityDetailScreen extends Component {
  constructor(props){
    super(props) 
  }

  toDetailScreen = (item) => {
    const { dispatch, navigation } = this.props
    dispatch(requestActions.select(item));
    navigation.navigate('ActivityDetail')
  }

  toAcceptScreen = () => {
    const { navigation } = this.props
    navigation.navigate('AcceptRequest')
  }

  _actionSelected = (index) => {
    if(index === 0){
      const { dispatch, selectedRequest } = this.props;
      dispatch(requestActions.updateRequest({
        id: selectedRequest.id,
        description: selectedRequest.description,
        liters: selectedRequest.liters,
        requestFrom: selectedRequest.requestFrom,
        requestTo: selectedRequest.requestTo,
        isRead: true,
        granted: selectedRequest.granted,
        declined: true,
        parentCouponId:null,
        grantedLiters:0
      }))
    }
  }

  _deleteRequest = (index) => {
    if(index === 0){
      const { dispatch, selectedRequest } = this.props;
      dispatch(requestActions.updateRequest({
        id: selectedRequest.id,
        description: selectedRequest.description,
        liters: selectedRequest.liters,
        requestFrom: selectedRequest.requestFrom,
        requestTo: selectedRequest.requestTo,
        isRead: true,
        granted: selectedRequest.granted,
        declined: false,
        parentCouponId:null,
        grantedLiters:0,
        deleted:true
      }))
    }
  }

  componentDidUpdate(){
    const { selected, navigation, type, message, alertWithType, dispatch, updatedRequest, user } = this.props;

    if(type === 'alert-danger'){
      this.dropDownAlertRef.alertWithType('error', 'Error', message);
      dispatch(alertActions.clear());
    } else if(type === 'alert-success'){
      this.dropDownAlertRef.alertWithType('success', 'Success', message);
      dispatch(alertActions.clear());
    }

    if(updatedRequest){
      dispatch(requestActions.getUserRequests(user.id))
      navigation.navigate('Activity')
    }

  }
 
  _renderContent = () => {
    const {user, navigation, requests, selectedRequest} = this.props;
    return (
      <Animated.View 
        style={{
          height:  Dimensions.get('window').height - (Platform.OS === 'ios' ? 210 : 115) 
        }}
      > 
        {
          selectedRequest.requestFrom === user.id ?
            (
              <ActionSheet
                ref={o => this.ActionSheet = o}
                title={'What do you want to do?'}
                options={['Delete Request', 'cancel']}
                cancelButtonIndex={1}
                destructiveButtonIndex={1}
                onPress={(index) => { this._deleteRequest(index) }}
              />
            ) : (
              <ActionSheet
                ref={o => this.ActionSheet = o}
                title={'What do you want to do?'}
                options={['Reject Request', 'cancel']}
                cancelButtonIndex={1}
                destructiveButtonIndex={1}
                onPress={(index) => { this._actionSelected(index) }}
              />
            )
        }
        <View style={{flex:1}}>
          <MontserratListItem
            title="Request from"
            subtitle={selectedRequest.requestFromUser}
            subtitleStyle={styles.subtitleStyle} />
          <View style={{width:'100%', height:1, backgroundColor: colors.lightGray1}}></View>
          <MontserratListItem
            title="Liters"
            subtitle={`${selectedRequest.liters} ${selectedRequest.product}`}
            subtitleStyle={styles.subtitleStyle} />
          <View style={{width:'100%', height:1, backgroundColor: colors.lightGray1}}></View>
          <MontserratListItem
            title="Note"
            subtitle={`${selectedRequest.description}`}
            subtitleStyle={styles.subtitleStyle} />
          <View style={{width:'100%', height:1, backgroundColor: colors.lightGray1}}></View>
          <View style={{
            flexDirection:'row',
            paddingTop: 5,
            justifyContent:'space-evenly'
          }}>
            {
              selectedRequest.requestFrom !== user.id && (
                <Button
                  title="Send Fuel"
                  type="clear"
                  titleStyle={{color:colors.black, marginLeft:5}}
                  icon={
                    <Icon
                      name="check"
                      type="material"
                      size={26}
                      color={colors.lightGray2}
                    />
                  } 
                  onPress = {() => this.toAcceptScreen()}/>
              )
            }
            <Button
              title={selectedRequest.requestFrom === user.id ? 'Delete' : 'Reject'}
              type="clear"
              titleStyle={{color:colors.black, marginLeft:5}}
              icon={
                <Icon
                  name="delete" 
                  type="antdesign"
                  size={20}
                  color={colors.lightGray2}
                />
              } 
              onPress = {() => this.ActionSheet.show()}/>
          </View>
        </View> 
        <DropdownAlert inactiveStatusBarStyle='dark-content' inactiveStatusBarBackgroundColor='#fff' ref={ref => this.dropDownAlertRef = ref} />
      </Animated.View>
    )
  }

  render(){
    const { user, selectedRequest } = this.props
    return(
      <ParallaxContainer
        navigation={this.props.navigation} 
        children={this._renderContent()}
        navBarTitle='Fuel Request'
        headerTitle='Fuel Request' 
        headerSubTitle={`Fuel will be sent to ${selectedRequest.requestFromUser} as a coupon`}
        backToHome={true}/>
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
  subtitleStyle: {
    fontSize: 12,
    fontWeight:'400', 
    color: colors.accentColor,
  },
  title: {
    color: '#fff',
    fontWeight: "500",
    fontSize: 18,
    marginBottom: 'auto',
  },
})

function mapStateToProps(state) {
  const { user } = state.authentication
  const { requests, selectedRequest, updatedRequest } = state.requests
  const { type, message } = state.alert
  return {
      user,
      requests,
      selectedRequest,
      updatedRequest,
      type,
      message,
      primaryColor: state.themes.primaryColor,
  };
}

export default connect(mapStateToProps)(ActivityDetailScreen);