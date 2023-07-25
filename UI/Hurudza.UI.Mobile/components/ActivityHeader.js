import React from 'react';
import { StyleSheet, Image, Text, View, TouchableOpacity, TouchableHighlight, StatusBar, Animated, Platform } from 'react-native';
import { Feather } from '@expo/vector-icons';
import colors from '../constants/Colors';



export default class ActivityHeader extends React.Component {
  constructor(props) {
    super(props)

    this.state = {
      tabButtonState: 1,
      tabState: 1,
      subHeaderBg: colors.fu7,
      subHeaderPosY: 0,
    }
  }


  componentDidMount() {
  }
  //Change local tabState
  _changeTabState(tab) {
    this.setState({
      tabState: tab,
    })
  }
  //Return Params when ready
  getParams() {
   return this.props.route.params || {}
  }
  //Get the element height then set the state, subHeaderBg
  onLayout(e) {
    this.setState({
        subHeaderPosY: e.nativeEvent.layout.y,
    })
  }

  //Sub Header animation
  subHeaderPlatform(params) {
    if(Platform.OS === 'ios'){
      return '#fff'
    }else{
      return params.subHeaderBg
    }

  }
  subHeaderPlatformInit() {
    if(Platform.OS === 'ios'){
      return '#fff'
    }else{
      return colors.fu7
    }

  }
  //Sub Header Text Animation
  subHeaderPlatformText(params) {
    if(Platform.OS === 'ios'){
      return '#000'
    }else{
      return params.subHeaderText
    }

  }
  subHeaderPlatformTextInit() {
    if(Platform.OS === 'ios'){
      return '#000'
    }else{
      return '#fff'
    }

  }

  goBack() {
    this.props.navigation.pop();
  }

  render() {
      let params = this.getParams();

    return (
       <View style={styles.container}>

    <StatusBar backgroundColor={Platform.OS === 'ios'? '#fff': colors.fu7} barStyle={Platform.OS === 'ios'? 'dark-content': 'light-content'} />

          <Animated.View style={{
            position: 'absolute',
            top: 0,
            left: 0,
            height: params !== undefined &&
                    params.changingHeight !== undefined
                    ? params.changingHeight : 120,
            backgroundColor: Platform.OS === 'ios' ? '#fff' : colors.fu7,
            width: '100%',
          }}>
          </Animated.View>


          <Animated.View style={{
            position: 'absolute',
            top: 50,
            left: 25,
          }}>
            <TouchableOpacity
              onPress={() => {this.goBack()}}
              >
                <Feather
                    name='arrow-left'
                    size={25}
                    color= {Platform.OS === 'ios' ? '#000' : '#fff'}
                />
            </TouchableOpacity>
            </Animated.View>

            <Animated.Text style={{
              color: Platform.OS === 'ios' ? '#000' : '#fff',
              fontWeight: "500",
              fontSize: params !== undefined &&
                        params.changingFontSize !== undefined
                        ? params.changingFontSize : 20,
              position: 'absolute',
              top:  params !== undefined &&
                    params.changingHeight !== undefined
                    ? params.changingTop : 80,
              left: params !== undefined &&
                    params.changingLeft !== undefined
                    ? params.changingLeft : 25,
          }}>
              {this.props.navigation.state.routeName}
            </Animated.Text>



            <Animated.View
                onLayout={() => {this.onLayout}}
                style={{
                  position: 'absolute',
                  top: params !== undefined &&
                        params.changingSubHeaderTop !== undefined
                        ? params.changingSubHeaderTop : 120,
                  left: 0,
                  width: '100%',
                  height: 40,
                  backgroundColor: params !== undefined &&
                        params.subHeaderBg !== undefined
                        ? this.subHeaderPlatform(params) : this.subHeaderPlatformInit(),
                  flexDirection: 'row',
                  alignItems: 'flex-end',
                  justifyContent: 'flex-start',
                  paddingLeft: 25,
                }}>
              <TouchableOpacity
                style={[styles.tabButtonA, (this.state.tabState == 1)? styles.tabButtonAFocused:{}]}
                onPress={() => {this._changeTabState(1); params.changeTabState(1);}}
                >
                    <Animated.Text style={{

                        fontWeight: '500',
                        fontSize: 13,
                        marginBottom: 10,
                        color: params !== undefined &&
                               params.subHeaderText !== undefined
                               ? this.subHeaderPlatformText(params) : this.subHeaderPlatformTextInit(),
                    }}>
                  All
                </Animated.Text>
              </TouchableOpacity>
              <TouchableOpacity
              style={[styles.tabButton,(this.state.tabState == 2)? styles.tabButtonFocused:{}]}
              onPress={() => {this._changeTabState(2); params.changeTabState(2);}}
              >
                    <Animated.Text style={{

                        fontWeight: '500',
                        fontSize: 13,
                        marginBottom: 10,
                        color: params !== undefined &&
                               params.subHeaderText !== undefined
                               ? this.subHeaderPlatformText(params) : this.subHeaderPlatformTextInit(),
                    }}>
                  Requests
                </Animated.Text>
              </TouchableOpacity>
               <TouchableOpacity
               style={[styles.tabButton,(this.state.tabState == 3)? styles.tabButtonFocused:{}]}
               onPress={() => { this._changeTabState(3); params.changeTabState(3);}}
               >
                    <Animated.Text style={{

                        fontWeight: '500',
                        fontSize: 13,
                        marginBottom: 10,
                        color: params !== undefined &&
                               params.subHeaderText !== undefined
                               ? this.subHeaderPlatformText(params) : this.subHeaderPlatformTextInit(),
                    }}>
                  Messages
                </Animated.Text>
              </TouchableOpacity>
            </Animated.View>



       </View>

    );
  }
}

const styles = StyleSheet.create({
  container: {
    marginBottom: 5,
    top: 0,
    left: 0,
    right: 0,
    width: '100%',
  },
  activityHeaderContainer: {
    marginBottom: 10,
    position: 'absolute',
    top: 0,
    left: 0,
    width: '100%',
  },
  headerContainer: {
    flexDirection:'column',
    alignItems: 'flex-start',
    backgroundColor: colors.fu7,
    paddingTop:25,
    height: 125,
    paddingLeft: 25,

  },
  backButton: {
    marginBottom: 20,
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



});
