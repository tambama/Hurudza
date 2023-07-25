import React from 'react';
import { ScrollView, StyleSheet, 
  View, 
  Text, 
  FlatList, 
  Platform, 
  Modal, 
  Alert, 
  TouchableHighlight, 
  TouchableOpacity, 
  TextInput,
  Animated,
  Dimensions } from 'react-native';
import { connect } from 'react-redux'
import { MaterialCommunityIcons, AntDesign } from '@expo/vector-icons'
import colors from '../constants/Colors';
import CouponItem from '../components/CouponItem';
import Header from '../components/Header';

class CouponsScreen extends React.Component {
  static navigationOptions = {
    title: 'Coupons',
  };
//Modal Logic
    state = {
        modalVisible: false,
        
      };

//Fonts
componentDidMount() {
      
        this.scrollY = new Animated.Value(0);
        this.changingHeight = this.scrollY.interpolate({
            inputRange: [0, 50],
            outputRange: [100, 60],
            extrapolate: "clamp"
        });
         this.changingTop = this.scrollY.interpolate({
            inputRange: [0, 50],
            outputRange: [100, 60],
            extrapolate: "clamp"
        });
         this.changingHeaderTop = this.scrollY.interpolate({
            inputRange: [0, 50],
            outputRange: [65, 30],
            extrapolate: "clamp"
        });
         this.changingHeaderLeft = this.scrollY.interpolate({
            inputRange: [0, 50],
            outputRange: [25, (Dimensions.get('window').width/2 - 35)],
            extrapolate: "clamp"
        });
         this.changingFontSize = this.scrollY.interpolate({
            inputRange: [0, 50],
            outputRange: [18, 14],
            extrapolate: "clamp"
        });

        this.props.navigation.setParams({
            changingHeight: this.changingHeight,
            changingTop: this.changingHeaderTop,
            changingLeft: this.changingHeaderLeft,
            changingFontSize: this.changingFontSize,
        });
    }      

  setModalVisible(visible) {
    this.setState({modalVisible: visible});
  }

//Handle Item Press
  _handleItemPress = () => {
    this.props.navigation.navigate('BuyCoupons')
    //this.setModalVisible(true);
  }

  render() {
    return (
      <View style={styles.container}>
      <Animated.View
        style={{
          top: this.changingTop !== undefined && 
               this.changingTop !== undefined
               ? this.changingTop : 100,
        }}
      >
        <FlatList
          data={this.testProps.coupons}
          keyExtractor={(item, index) => item.key}
          renderItem={({item}) => <CouponItem itemPress={this._handleItemPress} coupon={item} />}
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
        />
        </Animated.View>

        <TouchableOpacity onPress={this._handleItemPress} style={styles.fab}>
          <AntDesign
            name={'plus'}
            size={26}
            color={'#fff'}
            style={styles.fabIcon}
          />
        </TouchableOpacity>



      </View>
    );
  }
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    paddingTop: 3,
    backgroundColor: '#f1f1f1',
  },
  modalContent: {
    flexDirection: 'column',
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    margin: 0,
  },
  modalHeader: {
    backgroundColor: colors.fu7,
    height: 52,
  },
  modalView: {
    backgroundColor: '#f1f1f1',
    flex:1,
    justifyContent: 'center',
    alignItems: 'center',
  },

  modalDialog: {
    backgroundColor: 'white',
    height: '50%',
    width: '90%',
    borderRadius: 20,
    paddingTop: 25,
    paddingBottom: 25,
    paddingLeft: 25,
    paddingRight: 25,
  },
  mContainer: {
    flexDirection: 'column',

  },
  mHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
  },
  mIcon: {
    height: 40,
    width: 40,
    backgroundColor: '#FFA300',
    borderRadius: 40/2,
    justifyContent: 'center',
    alignItems: 'center',
    paddingTop: 2,
    
  },
  listHeader: {
    fontSize: 14,
    color: '#ccc',
    marginLeft: 15,
    marginTop: 10,
    marginBottom: 12,
  },
  listHeaderIcon: {

    marginLeft: 5,

  },
 fab: { 
  position: 'absolute', 
  width: 56, 
  height: 56, 
  alignItems: 'center', 
  justifyContent: 'center', 
  right: 20, 
  bottom: 20, 
  backgroundColor: colors.fu8, 
  borderRadius: 56/2, 
  elevation: 8 
  }, 
  fabIcon: { 
    marginTop: 2,
    marginLeft: 2,
  },

});

function mapStateToProps(state) {
  const { user } = state.authentication
  const { coupons, loading } = state.coupons
  return {
      user,
      coupons,
      loading
  };
}

export default connect(mapStateToProps)(CouponsScreen);
