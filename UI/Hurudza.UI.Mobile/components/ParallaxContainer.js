import React, { Component } from 'react';
import PropTypes from 'prop-types';
import {
  Text,
  View,
  Image,
  Animated,
  ScrollView,
  StyleSheet,
  FlatList,
  Dimensions,
  TouchableHighlight,
  Platform,
  TouchableOpacity,
  StatusBar,
  RefreshControl
} from 'react-native';
import { connect } from 'react-redux';
import { Icon, ListItem, Button } from 'react-native-elements';
import { getStatusBarHeight, isIphoneX, ifIphoneX } from 'react-native-iphone-x-helper'
import { SafeAreaView } from 'react-native-safe-area-context';
import Constants from 'expo-constants';

import DropdownAlert from 'react-native-dropdownalert';
import { alertActions } from '../actions/alert';

import { MontserratText } from '../components/StyledText';
import colors from '../constants/Colors';

import { userActions } from '../actions/user'

import { SCREEN_WIDTH, SCREEN_HEIGHT, DEFAULT_WINDOW_MULTIPLIER, DEFAULT_NAVBAR_HEIGHT, PARALLAX_MULTIPLIER } from '../constants/constants';

const ScrollViewPropTypes = ScrollView.propTypes;
const AnimatedIcon = Animated.createAnimatedComponent(Icon);

class ParallaxContainer extends Component {
  constructor() {
    super();

    this.state = {
      scrollY: new Animated.Value(0)
    };
  }

  //Pop current view and navigate to home
    _goBack = () => {
        const { backToHome } = this.props;
        if(backToHome){
          this.props.navigation.navigate('Home')
        } else {
          this.props.navigation.pop()
        }

    }

  scrollTo(where) {
    if (!this._scrollView) return;
    this._scrollView.scrollTo(where);
  }

  renderBackground() {
    var { windowHeight, backgroundSource, onBackgroundLoadEnd, onBackgroundLoadError } = this.props;
    var { scrollY } = this.state;
    if (!windowHeight || !backgroundSource) {
      return null;
    }

    return (
      <Animated.View
        style={[
          styles.background,
          {
            height: windowHeight,
            transform: [
              {
                translateY: scrollY.interpolate({
                  inputRange: [-windowHeight, 0, windowHeight],
                  outputRange: [windowHeight / 2, 0, -windowHeight / 3]
                })
              },
              {
                scale: scrollY.interpolate({
                  inputRange: [-windowHeight, 0, windowHeight],
                  outputRange: [2, 1, 1]
                })
              }
            ]
          }
        ]}
        onLoadEnd={onBackgroundLoadEnd}
        onError={onBackgroundLoadError}
      >
      </Animated.View>
    );
  }

  renderHeaderView() {
    const { windowHeight, backgroundSource, headerTitle, headerSubTitle, navBarHeight, rightIconView, pressRightIcon } = this.props;
    const { scrollY } = this.state;
    if (!windowHeight || !backgroundSource) {
      return null;
    }

    const newNavBarHeight = navBarHeight || DEFAULT_NAVBAR_HEIGHT;
    const newWindowHeight = windowHeight - newNavBarHeight;


    Platform.OS === 'android' ? StatusBar.setBarStyle('dark-content') : null
    Platform.OS === 'android' ? StatusBar.setBackgroundColor(colors.lightGray1) : null

    return (
      <Animated.View
        style={{
          opacity: scrollY.interpolate({
            inputRange: [-windowHeight, 0, windowHeight * DEFAULT_WINDOW_MULTIPLIER],
            outputRange: [1, 1, 0]
          })
        }}
      >
        <View style={{height: newWindowHeight, justifyContent: 'center', paddingHorizontal: 20, paddingBottom: 15}}>
          {this.props.headerView ||
            (
              <View style={{flexDirection: 'row', justifyContent: 'space-between'}}>
                <View style={{paddingVertical: 10}}>
                  <MontserratText style={styles.headerTitle}>{headerTitle}</MontserratText>
                  <MontserratText style={styles.headerSubTitle}>{headerSubTitle}</MontserratText>
                </View>
                {
                  rightIconView &&
                  (
                    <View style={{paddingVertical:15}}>
                      <Icon
                        name='account-edit'
                        type='material-community'
                        size={26}
                        onPress={pressRightIcon}
                      />
                    </View>
                  )
                }
              </View>
            )
          }
        </View>
      </Animated.View>
    );
  }

  renderNavBarTitle() {
    const { windowHeight, backgroundSource, navBarTitleColor, navBarTitleComponent } = this.props;
    const { scrollY } = this.state;
    if (!windowHeight || !backgroundSource) {
      return null;
    }

    return (
      <Animated.View
        style={{
          opacity: scrollY.interpolate({
            inputRange: [-windowHeight, windowHeight * PARALLAX_MULTIPLIER, windowHeight * 0.5],
            outputRange: [0, 0, 1]
          })
        }}
      >
        {navBarTitleComponent ||
        <MontserratText style={{ fontSize: 20, color: navBarTitleColor || colors.black }}>
          {this.props.navBarTitle || 'Dohwe.'}
        </MontserratText>}
      </Animated.View>
    );
  }

  rendernavBar() {
    const {
      windowHeight, backgroundSource, hasLeftIcon, leftIcon, hasTopLeftAdd, topLeftAddPress,
      rightIcon, leftIconOnPress, rightIconOnPress, navBarColor, navBarHeight, leftIconUnderlayColor, rightIconUnderlayColor,
      showLogout, pressLogout
    } = this.props;
    const { scrollY } = this.state;
    if (!windowHeight || !backgroundSource) {
      return null;
    }

    let nh = navBarHeight || DEFAULT_NAVBAR_HEIGHT;

    const newNavBarHeight = nh - (Platform.OS === 'ios' ? 15 : 0)

    if(this.props.navBarView)
    {
        return (
          <Animated.View
            style={{
              height: newNavBarHeight,
              width: SCREEN_WIDTH,
              flexDirection: 'row',
              backgroundColor: navBarColor || colors.lightGray1
            }}
          >
          {this.props.navBarView}
          </Animated.View>
        );
    }
    else
    {
        return (
          <Animated.View
            style={{
              height: newNavBarHeight,
              width: SCREEN_WIDTH,
              flexDirection: 'row',
              backgroundColor: navBarColor || colors.lightGray1
            }}
          >
          { hasLeftIcon &&
            <View
              style={{
                flex: 1,
                justifyContent: 'center',
                alignItems: 'center',
              }}
            >
              <AnimatedIcon
                name={leftIcon && leftIcon.name || (Platform.OS === 'ios'? 'ios-arrow-back' : 'md-arrow-back')}
                type={leftIcon && leftIcon.type || 'ionicon'}
                color={
                  leftIcon && leftIcon.color ||
                  colors.black
                  }
                size={leftIcon && leftIcon.size || 26}
                onPress={() => this._goBack()}
                underlayColor={leftIconUnderlayColor || 'transparent'}
              />
            </View>
          }
          {
            hasTopLeftAdd && (
              <Button
                title='Add'
                type='clear'
                onPress={topLeftAddPress}
                style={{
                  marginLeft: 15,
                  marginTop: 5
                }} />
            )
          }
            <View
              style={{
                flex: 5,
                paddingLeft: 20,
                alignSelf: 'center',
              }}
            >
              {this.renderNavBarTitle()}
            </View>
          {rightIcon &&
            <View
              style={{
                flex: 1,
                justifyContent: 'center',
                alignItems: 'center'
              }}
            >
              <Icon
                name={rightIcon && rightIcon.name || 'present'}
                type={rightIcon && rightIcon.type || 'simple-line-icon'}
                color={rightIcon && rightIcon.color || 'black'}
                size={rightIcon && rightIcon.size || 23}
                onPress={rightIconOnPress}
                underlayColor={rightIconUnderlayColor || 'transparent'}
              />
            </View>
          }
          {showLogout &&
            <Animated.View>
              <TouchableOpacity
                style={{
                  flex: 1,
                  justifyContent: 'center',
                  alignItems: 'center'
                }}
                onPress={pressLogout}
              >
                <MontserratText style={{width:70}}>Logout</MontserratText>
              </TouchableOpacity>
            </Animated.View>
          }
          </Animated.View>
        );
    }
  }

  componentDidUpdate(){
    const { type, message, dispatch } = this.props;

    if(type === 'alert-danger'){
      this.dropDownAlertRef.alertWithType('error', 'Error', message);
      dispatch(alertActions.clear());
    } else if(type === 'alert-success'){
      this.dropDownAlertRef.alertWithType('success', 'Success', message);
      dispatch(alertActions.clear());
    }

  } 

  render() {
    const { hasLoading, loading, loadData, style, ...props } = this.props;

    return (
      <View style={[styles.container, style]}>
        <StatusBar barStyle={'dark-content'} backgroundColor={colors.lightGray1} />
        {this.renderBackground()}
        {this.rendernavBar()}
        <Animated.ScrollView
          ref={component => {
            this._scrollView = component;
          }}
          {...props}
          style={styles.scrollView}
          showsVerticalScrollIndicator={false}
          onScroll={ Animated.event([
            { nativeEvent: { contentOffset: { y: this.state.scrollY } } }
          ], { useNativeDriver: true })}
          scrollEventThrottle={16}
          onLayout={hasLoading ? loadData : null}
          refreshControl={ hasLoading ? <RefreshControl refreshing={loading} onRefresh={loadData} /> : null}
        >
          {this.renderHeaderView()}
          <View style={[styles.content, props.scrollableViewStyle]}>
            {this.props.children}
          </View>
        </Animated.ScrollView>
        {
          this.props.showFab && (
            <TouchableOpacity onPress={this.props.fabPress} style={styles.fab}>
              <Icon
                name={'plus'}
                type={'antdesign'}
                size={26}
                color={'#fff'}
                style={styles.fabIcon}
              />
            </TouchableOpacity>
          )
        }
        <DropdownAlert inactiveStatusBarStyle='dark-content' inactiveStatusBarBackgroundColor={colors.white} ref={ref => this.dropDownAlertRef = ref} />
      </View>
    );
  }
}

ParallaxContainer.defaultProps = {
  backgroundSource: {uri: 'http://i.imgur.com/6Iej2c3.png'},
  windowHeight: SCREEN_HEIGHT * DEFAULT_WINDOW_MULTIPLIER,
  leftIconOnPress: () => {console.log('Left icon pressed')},
  rightIconOnPress: () => console.log('Right icon pressed'),
  hasLeftIcon: false,
  leftIcon: {},
  hasLeftPress: false,
  hasTopLeftAdd: false,
  topLeftAddPress: () => {console.log('Top left Add pressed')},
  showFab:false,
  fabPress: () => console.log('fab pressed'),
  showLogout: false,
  pressLogout: () => console.log('logout'),
  loadData: () => console.log('loading'),
  hasLoading: false,
  backToHome: false,
  rightIconView: false,
  pressRightIcon: () => console.log('edit name')
};

ParallaxContainer.propTypes = {
  ...ScrollViewPropTypes,
  backgroundSource: PropTypes.object,
  windowHeight: PropTypes.number,
  navBarTitle: PropTypes.string,
  navBarTitleColor: PropTypes.string,
  navBarTitleComponent: PropTypes.node,
  navBarColor: PropTypes.string,
  headerImage: PropTypes.string,
  headerTitle: PropTypes.string,
  headerSubTitle: PropTypes.string,
  headerView: PropTypes.node,
  hasLeftIcon: PropTypes.bool,
  leftIcon: PropTypes.object,
  leftIconOnPress: PropTypes.func,
  hasLeftPress: PropTypes.bool,
  hasTopLeftAdd: PropTypes.bool,
  topLeftAddPress: PropTypes.func,
  rightIcon: PropTypes.object,
  showFab: PropTypes.bool,
  fabPress: PropTypes.func,
  showLogout: PropTypes.bool,
  pressLogout: PropTypes.func,
  loadData: PropTypes.func,
  loading: PropTypes.bool,
  hasLoading: PropTypes.bool,
  backToHome: PropTypes.bool,
  rightIconView: PropTypes.bool,
  pressRightIcon: PropTypes.func
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    borderColor: colors.lightGray1,
    paddingTop: Platform.OS === 'ios' ? Constants.statusBarHeight : 0
  },
  scrollView: {
    backgroundColor: 'transparent',
  },
  background: {
    position: 'absolute',
    backgroundColor: colors.lightGray1,
    width: SCREEN_WIDTH,
    resizeMode: 'cover'
  },
  content: {
    backgroundColor: colors.lightGray1,
    flex: 1,
    flexDirection: 'column'
  },
  headerView: {
    justifyContent: 'center',
    alignItems: 'center',
  },
  headerTitle: {
    fontSize: 24, 
    fontWeight: '400', 
    color: colors.black,
    marginBottom: 10
  },
  headerSubTitle: {
    fontWeight:'100', 
    color: colors.black,
  },

 fab: {
  position: 'absolute',
  width: 56,
  height: 56,
  alignItems: 'center',
  justifyContent: 'center',
  right: 20,
  bottom: 20,
  backgroundColor: colors.accentColor,
  borderRadius: 56/2,
  elevation: 8
  },
  fabIcon: {
    marginTop: 2,
    marginLeft: 2,
  },
});

function mapStateToProps(state) {
  const { type, message } = state.alert
  return {
      type,
      message,
  };
}

export default connect(mapStateToProps)(ParallaxContainer);