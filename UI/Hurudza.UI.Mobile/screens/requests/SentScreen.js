import React, { Component } from 'react';
import { Animated, Dimensions, View, StyleSheet, Platform, TouchableOpacity, FlatList, ActivityIndicator, Text } from 'react-native';
import { connect } from 'react-redux';
import { Input , Icon, ListItem } from 'react-native-elements';
import ActionSheet from 'react-native-actionsheet'

import ParallaxContainer from '../../components/ParallaxContainer'
import ActivityItem from '../../components/ActivityItem'
import { Separator } from "../../components/List";
import DropdownAlert from 'react-native-dropdownalert';

import { userActions } from '../../actions/user';
import { alertActions } from '../../actions/alert'; 
import { productActions } from '../../actions/product'; 
import { requestActions } from '../../actions/request';
import colors from '../../constants/Colors';

class SentScreen extends Component {
  constructor(props){
    super(props) 
  }

  UNSAFE_componentWillMount(){
    const { dispatch, user } = this.props
    dispatch(requestActions.getUserRequests(user.id));
    dispatch(productActions.getAll())
  }

  toDetailScreen = (item) => { 
    const { dispatch, navigation, user } = this.props
    dispatch(requestActions.select({
      id: item.id,
      description: item.description,
      liters: item.liters,
      requestFrom: item.requestFrom,
      requestTo: item.requestTo,
      isRead: item.requestFrom !== user.mobileNumber,
      granted: item.granted,
      declined: item.declined,
      parentCoupon:null,
    }));
    
  }

  componentDidUpdate(){
    const { selected, navigation, type, message, alertWithType, dispatch } = this.props;

    if(type === 'alert-danger'){
      this.dropDownAlertRef.alertWithType('error', 'Error', message);
    }

    //dispatch(alertActions.clear());

    if(selected){
      dispatch(requestActions.clear())
      navigation.navigate('ActivityDetail')
    }

  }
 
  _renderContent = () => {
    const {user, navigation, requests} = this.props;
    return (
      <Animated.View 
        style={{
          height:  Dimensions.get('window').height - (Platform.OS === 'ios' ? 210 : 115) 
        }}
      >
        <View>
            {
              requests === undefined && (
                <View style={[styles.horizontal]}> 
                  <ActivityIndicator size="large" color="#0000ff" />
                </View>
              )
            }
            {
              requests !== undefined && (
                <FlatList
                  style={styles.scrollView}
                  data={requests}
                  keyExtractor={(item, index) => item.id.toString()}
                  renderItem={({item}) => (<ActivityItem  item={item} user={user} toDetail={this.toDetailScreen}/>)}
                  scrollEventThrottle={16}
                  onScroll = { Animated.event([
                                {
                                    nativeEvent: {
                                        contentOffset: {
                                            y: this.scrollY
                                        }
                                    }
                                }
                            ])}
                  ItemSeparatorComponent={Separator}
                />
              )
            }
        </View> 
        <DropdownAlert inactiveStatusBarStyle='dark-content' inactiveStatusBarBackgroundColor='#fff' ref={ref => this.dropDownAlertRef = ref} />
      </Animated.View>
    )
  }

  render(){
    const {user} = this.props
    return(
      <ParallaxContainer
        navigation={this.props.navigation} 
        children={this._renderContent()}
        navBarTitle='Your Activity'
        headerTitle='Your Activity' 
        headerSubTitle='All your notifications and activity history'
        backToHome={true}/>
    )
  }
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: 'center'
  },
  horizontal: {
    flexDirection: 'row',
    justifyContent: 'space-around',
    padding: 10
  },
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
  tabsContainer: {
    position: 'absolute',
    top: 100,
    left: 0,
    width: '100%',
    backgroundColor: colors.fu7,
    flexDirection: 'row',
    alignItems: 'flex-start',
    justifyContent: 'flex-start',
  },
  tabButtonFocused: {
    borderBottomWidth: 2,
    borderBottomColor: Platform.OS === 'ios'? colors.fu6 : colors.fu3,
  },
  tabButton: {
    width: '25%',
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
  },
  tabButtonAFocused: {
    borderBottomWidth: 2,
    borderBottomColor: Platform.OS === 'ios'? colors.fu6 : colors.fu3,
  },
  tabButtonA: {
    width: '15%',
    flexDirection: 'row',
    paddingLeft: 2,
    alignItems: 'center',
    justifyContent: 'flex-start',

  },
  tabButtonText: {
    fontWeight: '500',
    fontSize: 12,
    marginBottom: 10,
  }
})

function mapStateToProps(state) {
  const { user } = state.authentication
  const { requests, selected, selectedRequest } = state.requests
  const { type, message } = state.alert
  return {
      user,
      requests,
      selected,
      selectedRequest,
      type,
      message,
      primaryColor: state.themes.primaryColor,
  };
}

export default connect(mapStateToProps)(SentScreen);