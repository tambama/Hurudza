import React, { Component, createRef } from 'react';
import { 
  View, 
  StyleSheet, 
  Platform, 
  TouchableOpacity, 
  ActivityIndicator, 
  Text,
  TextInput,
  ScrollView,
  RefreshControl,
  StatusBar
} from 'react-native';
import { connect } from 'react-redux';
import { Input , Icon, ListItem, Overlay, Badge, Button } from 'react-native-elements';
import ActionSheet from "react-native-actions-sheet";
import moment from 'moment'


import { userActions } from '../../actions/user';
import { alertActions } from '../../actions/alert'; 
import { productActions } from '../../actions/product'; 
import { requestActions } from '../../actions/request';
import colors from '../../constants/Colors';
import { walletActions } from '../../actions/wallet';
import { Routes } from '../../navigation/Routes';

class ActivityScreen extends Component {
  constructor(props){
    super(props) 
    this.actionSheetRef = createRef()
    this.grantActionSheetRef = createRef()

    this.state = {
      isVisible: false,
      nestedScrollEnabled: false,
      isSelectingId: 0
    }
  }

  _onClose = () => {
    this.setState({nestedScrollEnabled: true})
  };

  _onSendClick = () => {
    const { navigation } = this.props;

    this.actionSheetRef.current?.setModalVisible();
    navigation.navigate(Routes.AcceptRequest);
  }

  _onEditClick = () => {
    const { navigation, dispatch } = this.props;

    this.actionSheetRef.current?.setModalVisible();
    dispatch(requestActions.clear());
    navigation.navigate(Routes.EditRequest);
  }

  _onDeclineClick = () => {
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

  _selectRequest = (item) => {
    const { dispatch, navigation, user } = this.props
    dispatch(requestActions.select({
      id: item.id,
      description: item.description,
      liters: item.liters,
      requestFrom: item.requestFrom,
      requestTo: item.requestTo,
      isRead: item.isRead ? item.isRead : user.id !== item.creatorId,
      granted: item.granted,
      declined: item.declined,
      parentCoupon:null,
    }));

    this.setState({isSelectingId : item.id});
  }

  _onCancelClick = () => {
    const { dispatch, selectedRequest } = this.props;

    dispatch(requestActions.delete(selectedRequest.id));
  } 

  _onRefresh = () => {
    const { dispatch, user } = this.props;

    dispatch(walletActions.clear);
    dispatch(requestActions.getUserRequests(user.id))
  }

  _renderRequestItem = (request) => {
    const { user, selecting } = this.props
    const { isSelectingId } = this.state
    let type = 'evilicon'
    let name = `arrow-${user.id === request.creatorId ? 'up' : 'down'}`
    let size = 40
    let color = request.isRead ? colors.iconsGrey : 'green'
    let sign = ''
    let requestor = user.id === request.creatorId ? request.requestToUserName : request.requestFromUserName

    return (
        <ListItem
          key={request.id}
          style={{...styles.activityItem}} 
          containerStyle={{borderRadius: 5}}
          onPress={() => this._selectRequest(request)}>
            <ListItem.Content>
              <View style={styles.activityItemContent}>
                <View style={styles.activityItemContentLeft}>
                  <View style={{width: 50}}>
                    <Icon
                      type={type}
                      name={name} 
                      size={size}
                      color={color} />
                  </View>
                  <View style={styles.activityItemContentMiddle}>
                    <View style={styles.activityItemTitleContainer}>
                      <Text style={styles.activityItemTitlePrefix}></Text>
                      <Text style={styles.activityItemTitle}>{requestor}</Text>
                    </View>
                    <Text style={styles.activityItemSubtitle}>{moment(request.createdDate).calendar()}</Text>
                  </View>
                </View>
                {
                  selecting && isSelectingId === request.id ? (
                    <ActivityIndicator color={colors.activeBlue} />
                  ) : (
                    <Text style={styles.activityItemRightText}>{sign} {request.liters} L</Text>
                  )
                }
              </View>
            </ListItem.Content>
        </ListItem>
    )
  }

  componentDidUpdate(){
    const { dispatch, selected, updatingRequest, updatedRequest, deletingRequest, deletedRequest } = this.props

    Platform.OS === 'android' ? StatusBar.setBarStyle('dark-content') : null;
    Platform.OS === 'android' ? StatusBar.setBackgroundColor(colors.white) : null;

    if(selected){
      this.actionSheetRef.current?.setModalVisible();
      dispatch(requestActions.clear());
    }

    if(!updatingRequest && updatedRequest){
      this.actionSheetRef.current?.setModalVisible();
      dispatch(requestActions.clear());
    }

    if(!deletingRequest && deletedRequest){
      this.actionSheetRef.current?.setModalVisible();
      dispatch(requestActions.clear());
    }
  }

  render(){
    const { user, requests, updatingRequest, loading, selectedRequest, deletingRequest } = this.props;
    const { nestedScrollEnabled } = this.state
    let requestor = user.id === selectedRequest?.creatorId ? selectedRequest?.requestToUserName : selectedRequest?.requestFromUserName
    let isIncoming = user.id !== selectedRequest?.creatorId
    let isGranted = selectedRequest?.granted

    return(
      <ScrollView
        onLayout={() => this._onRefresh()}
        refreshControl={
          <RefreshControl refreshing={loading} onRefresh={() => this._onRefresh()} />
      }>
        {
          requests === undefined && (
            <View style={[styles.horizontal]}> 
              <ActivityIndicator size="large" color="#0000ff" />
            </View>
          )
        }
        {
          requests.length > 0 && requests.map(request => (
            this._renderRequestItem(request)
          ))
        }
        <ActionSheet
          initialOffsetFromBottom={0.7}
          ref={this.actionSheetRef}
          bounceOnOpen={true}
          bounciness={8}
          gestureEnabled={true}
          onClose={() => this._onClose()}
          defaultOverlayOpacity={0.3}
          containerStyle={{}}>
          <ScrollView
            nestedScrollEnabled={true}
            scrollEnabled={nestedScrollEnabled}
            style={{
              width: '100%',
              minHeight: 375,
              backgroundColor: 'white'
            }}>
            <View
              style={{
                flexDirection: 'row',
                justifyContent: 'space-between',
                alignItems: 'center',
                marginBottom: 15,
                width: '100%',
                paddingHorizontal: 12
              }}>
              <View style={styles.activityItemContent}>
                <View style={styles.activityItemContentLeft}>
                  <View style={styles.activityItemContentMiddle}>
                    <View style={styles.activityItemTitleContainer}>
                      <Text style={styles.activityItemTitlePrefix}></Text>
                      <Text style={styles.activityItemTitle}>{requestor}</Text>
                    </View>
                    <Text style={styles.activityItemSubtitle}>{moment(selectedRequest?.createdDate).calendar()}</Text>
                  </View>
                </View>
                <View>
                  <Text style={styles.activityItemRightText}>{selectedRequest?.liters} L</Text>
                  <Badge badgeStyle={{backgroundColor: colors.accentColorIOS}} value={selectedRequest?.product} />
                </View>
              </View>
            </View>
            <View style={{
              paddingHorizontal: 25,
              marginBottom: 15
            }}>
              <Text style={{fontWeight: '400'}}>{selectedRequest?.description}</Text>
            </View>

            <View style={styles.actionButtonsContainer}>
              {
                isIncoming ? (
                  <View>
                    {
                      !isGranted && (
                        <View>
                          <TouchableOpacity 
                            style={styles.actionButtonContainer} onPress={() => this._onSendClick()}>
                            <Text style={styles.actionButtonTitle}>Send Fuel</Text>
                            <View style={styles.actionButtonIconContainer}>
                              <Icon
                                type='evilicon'
                                name='share-apple'
                                size={30}
                                color={colors.black} />
                            </View>
                          </TouchableOpacity>
                          <TouchableOpacity style={styles.actionButtonContainer} onPress={() => this._onDeclineClick()}>
                            <Text style={styles.actionButtonTitle}>Decline Request</Text>
                            <View style={styles.actionButtonIconContainer}>
                              {
                                updatingRequest ? (
                                  <ActivityIndicator color={colors.black} />
                                ) : (
                                  <Icon
                                    type='antdesign'
                                    name='frowno' />
                                )
                              }
                            </View>
                          </TouchableOpacity>
                        </View>
                      )
                    }
                  </View>
                ) : (
                  <View>
                    {
                      !isGranted && (
                        <View>
                          <TouchableOpacity 
                            style={styles.actionButtonContainer} onPress={() => this._onEditClick()}>
                            <Text style={styles.actionButtonTitle}>Edit Request</Text>
                            <View style={styles.actionButtonIconContainer}>
                              <Icon
                                type='antdesign'
                                name='edit'
                                size={26}
                                color={colors.black} />
                            </View>
                          </TouchableOpacity>
                          <TouchableOpacity style={styles.actionButtonContainer} onPress={() => this._onCancelClick()}>
                            <Text style={styles.actionButtonTitle}>Cancel Request</Text>
                            <View style={styles.actionButtonIconContainer}>
                              {
                                deletingRequest ? (
                                  <ActivityIndicator color={colors.black} />
                                ) : (
                                  <Icon
                                    type='evilicon'
                                    name='trash'
                                    size={30} />
                                )
                              }
                            </View>
                          </TouchableOpacity>
                        </View>
                      )
                    }
                  </View>
                )
              }
            </View>
          </ScrollView>
        </ActionSheet>
        
      </ScrollView> 
    )
  }
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: 'center'
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
    marginTop: 5,
    marginHorizontal: 5,
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
    fontSize: Platform.OS === 'ios' ? 12 : 11,
    color: Platform.OS === 'ios' ? colors.black : colors.secondaryTextGrey,
    fontWeight: Platform.OS === 'ios' ? '200' : '100'
  },
  activityItemRightText:{
    fontSize: 16,
    fontWeight: '300',
    alignSelf: 'center'
  },
  actionButtonsContainer: {
    backgroundColor: '#fcfcfc',
    height: 200,
    paddingHorizontal: 15
  },
  actionButtonContainer: {
    marginHorizontal: 10,
    marginTop: 20,
    paddingHorizontal: 15,
    borderRadius: 15,
    flexDirection: 'row',
    justifyContent: 'space-between',
    backgroundColor: colors.white,
    height: 40,
    shadowColor: colors.activeBlue,
    shadowOffset: {
      width: 0,
      height: 2,
    },
    shadowOpacity: 0.25,
    shadowRadius: 3.84,
    elevation: 2,
  },
  actionButtonTitle: {
    alignSelf: 'center'
  },
  actionButtonIconContainer: {
    alignSelf: 'center'
  }
})

function mapStateToProps(state) {
  const { user } = state.authentication
  const { requests, loading, selecting, selected, selectedRequest, updatingRequest, updatedRequest, deletingRequest, deletedRequest } = state.requests
  const { wallets, selectedWallet } = state.wallets
  const { type, message } = state.alert
  return {
      user,
      requests,
      updatingRequest,
      updatedRequest,
      deletingRequest,
      deletedRequest,
      loading,
      selecting,
      selected,
      selectedRequest,
      wallets,
      selectedWallet,
      type,
      message,
      primaryColor: state.themes.primaryColor,
  };
}

export default connect(mapStateToProps)(ActivityScreen);