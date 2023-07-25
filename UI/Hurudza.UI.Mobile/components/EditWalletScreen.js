import React, { Component } from 'react';
import {
  ScrollView,
  Text,
  TouchableHighlight,
  View,
  Platform,
  Dimensions,
  TextInput,
  StyleSheet,
  Animated
} from 'react-native';
import { Button, ButtonGroup } from 'react-native-elements';
import { connect } from 'react-redux';
import { SimpleLineIcons, MaterialCommunityIcons } from '@expo/vector-icons'

const screen = Dimensions.get('window');

class EditWalletScreen extends Component {
    static navigationOptions = {
        title: 'Edit Wallet'
    }
    constructor() {
        super();
        this.state = {
            fueltype: "petrol",
            selectedPaymentMethod: null, 
            selectedFuelType: null,
            paymentmethod: "wallet",
            litres: 0,
            wallet: {},
        };
        this.updateFuelIndex = this.updateFuelIndex.bind(this)
        this.updateIndex = this.updateIndex.bind(this)
    }

    updateFuelIndex(selectedFuelType) {
      this.setState({
        selectedFuelType
      })
    }

    updateIndex(selectedPaymentMethod) {
      this.setState({
        selectedPaymentMethod
      })
    }

    componentDidMount() {
      this.setState({
        wallet: this.props.navigation.getParam('wallet')
      })

      this.scrollY = new Animated.Value(0);
      this.changingHeight = this.scrollY.interpolate({
        inputRange: [0, 50],
        outputRange: [120, 60],
        extrapolate: "clamp"
      });
      this.changingTop = this.scrollY.interpolate({
        inputRange: [0, 50],
        outputRange: [120, 60],
        extrapolate: "clamp"
      });
      this.changingHeaderTop = this.scrollY.interpolate({
        inputRange: [0, 50],
        outputRange: [70, 30],
        extrapolate: "clamp"
      });
      this.changingHeaderLeft = this.scrollY.interpolate({
        inputRange: [0, 50],
        outputRange: [25, (Dimensions.get('window').width / 2 - 35)],
        extrapolate: "clamp"
      });
      this.changingFontSize = this.scrollY.interpolate({
        inputRange: [0, 50],
        outputRange: [20, 18],
        extrapolate: "clamp"
      });

      this.props.navigation.setParams({
        changingHeight: this.changingHeight,
        changingTop: this.changingHeaderTop,
        changingLeft: this.changingHeaderLeft,
        changingFontSize: this.changingFontSize,
      });
    }
    _handleWalletEdit(){
      const wallet = {
        id: this.state.wallet.id,
        petrol: this.state.fueltype === 'petrol' ? ( parseFloat(this.state.wallet.petrol) + parseFloat(this.state.litres)) : this.state.wallet.petrol,
        Diesel: this.state.fueltype === 'Diesel' ? ( parseFloat(this.state.wallet.Diesel) + parseFloat(this.state.litres)) : this.state.wallet.Diesel,
      }

      fetch('http://192.168.100.4:3000/editwallet', {
        method: 'POST',
        headers: {
          'content-type': 'application/json'
        },
        body: JSON.stringify(wallet)
      })

      this.props.navigation.goBack();
    }

    render() {
      const buttons = ['Wallet', 'Bank', 'Coupon', 'Ecocash']
      const { selectedPaymentMethod } = this.state
      const buttons2 = ['Petrol', 'Diesel']
      const {
        selectedFuelType
      } = this.state
        return (
        <View style={styles.container}>
          <Animated.View
          style={[{
            top: this.changingTop !== undefined &&
              this.changingTop !== undefined ?
              this.changingTop : 120,
          },styles.contentContainer]}
          >

              <ScrollView
                scrollEventThrottle = {
                  16
                }
                onScroll = {
                  Animated.event([{
                    nativeEvent: {
                      contentOffset: {
                        y: this.scrollY
                      }
                    }
                  }])
                }
                style = {
                  {
                    marginBottom: 70
                  }
                }
              >

                <View style={{
                  width:'100%',
                  backgroundColor:'#fff',
                  marginBottom:5,
                  justifyContent:'flex-start',
                  paddingHorizontal:20,
                  paddingTop: 20,
                  flexDirection:'column',
                }}>
                    <SimpleLineIcons
                        name='drop'
                        size={32}
                        color= {'#c3c3c3'}
                    />

                    <View style = {
                        {
                          flexDirection: "row",
                          justifyContent: "flex-start",
                          marginTop:10,
                        }
                      } >
                        <Text style={{fontWeight:'300', fontSize:16,}}>Enter Amount</Text> 
                    </View>
                    <View style = {
                      {
                        flexDirection: "row",
                        justifyContent: "flex-start",
                        marginBottom: 10,
                      }
                    } >
                      < TextInput keyboardType = 'numeric'
                      name = "litres"
                      onChangeText = {
                        (litres) => {this.setState({
                          litres
                        })
                      }
                      }
                      placeholder = "Enter Litres"
                      style = {
                        styles.textInput
                      }
                      />
                    </View>
                </View>
                
              <View style={{
                width:'100%',
                height: 100,
                backgroundColor: '#fff',
                padding:10,
                justifyContent:'center',
                marginBottom:5
              }}>
                <View>
                  <Text style={{fontWeight:'500', fontSize:16}}>Select Fuel Type</Text>
                  <View>
                    <ButtonGroup 
                        onPress={this.updateFuelIndex}
                        selectedIndex={selectedFuelType}
                        buttons={buttons2}
                        containerStyle={{height: 40}}
                        selectedButtonStyle={{backgroundColor:'#1c1565'}}
                    />
                  </View>
                </View>
              </View>
               
              <View style={{
                  width:'100%',
                  backgroundColor:'transparent',
                  marginBottom:5,
                  justifyContent:'center',
                  alignItems: 'center',
                  paddingHorizontal:20,
                  flexDirection:'column',
                }}>
                    <Button
                    title = 'Save Wallet'
                   onPress = {
                     () => {
                       this._handleWalletEdit()
                     }
                   }
                    buttonStyle = {{
                      marginTop:10,
                      backgroundColor: Platform.OS === 'ios' ? '#fff' : '#1c1565',
                      width: screen.width - 80
                    }}
                    titleStyle={{
                      color:Platform.OS === 'ios' ? '#000': '#fff'
                    }}
                    icon={
                      <MaterialCommunityIcons
                        name="chevron-right"
                        size={26}
                        color={Platform.OS === 'ios' ? '#000': '#fff'}
                      style = {
                        {
                          position: 'absolute',
                          right: 5,
                          justifyContent: 'center',
                        }
                      }
                      />
                    }
                    iconRight/>
                </View>

            </ScrollView>
          </Animated.View>
      </View>
        );
    }
}


const styles = StyleSheet.create({
    container: {
        flex: 1,
        backgroundColor: '#f1f1f1',
    },
    contentContainer: {
        paddingTop: 10,
        marginHorizontal: 20,
        backgroundColor: '#f1f1f1'
    },
    textInput: {
        height: 40,
        paddingLeft: 6,
        width: "90%"
    },
    welcomeContainer: {
        flexDirection: 'row',
        justifyContent: 'space-between',
        backgroundColor: Platform.OS === 'ios' ? '#fff' : '#1c1565',
        height: 80,
        paddingTop: 22
    },
    btnItem: {
        height: 30,
        backgroundColor: "#f1f1f1",
        alignItems: "center",
        justifyContent: "center",

    },
    headerContainer: {
      flexDirection: 'row',
      justifyContent: 'space-between',
      alignItems: 'center',
      backgroundColor: Platform.OS === 'ios' ? '#fff' : '#1c1565',
      paddingTop: 15,
      paddingBottom: 10,
    },

});

function mapStateToProps(state) {
    const {
        user
    } = state.authentication
    const {
        wallets,
        loading
    } = state.wallets
    return {
        user,
        wallets,
        loading
    };
}

export default connect(mapStateToProps, )(EditWalletScreen);