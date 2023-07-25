import React, { Component } from 'react'
import {
  ScrollView,
  Text,
  TouchableHighlight,
  View,
  Platform,
  TextInput,
  StyleSheet,
  Dimensions,
  Animated
} from 'react-native';
import { connect } from 'react-redux';
import { MaterialCommunityIcons } from '@expo/vector-icons'
import { Button, ButtonGroup } from 'react-native-elements';
import colors from '../constants/Colors'


const screen = Dimensions.get('window');

class AddWalletScreen extends Component {
     static navigationOptions = {
         title: 'New Wallet'
     }
    constructor() {
        super();
        this.state = {
            fueltype: "petrol",
            paymentmethod: "wallet",
            id: "",
            litres: 0,
            name: "",
            selectedPaymentMethod: null, 
            selectedFuelType: null,
        }

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
    componentDidMount(){
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


    _handleAddWallet() {
      String.prototype.capitalize = function () {
        return this.charAt(0).toUpperCase() + this.slice(1);
      }

      const wallet = {
        name: (this.state.name).capitalize(),
        petrol: this.state.fueltype === 'petrol' ? this.state.litres : 0,
        Diesel: this.state.fueltype === 'Diesel' ? this.state.litres : 0,
        couponstatus: false
      }

      fetch('http://192.168.100.4:3000/addwallet', {
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
    const {
      selectedPaymentMethod
    } = this.state
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
        }, styles.contentContainer
        ]}
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
                   marginBottom: 100
                 }
               }
            >

              <View style={{
                width:'100%',
                backgroundColor:'#fff',
                marginBottom:5,
                justifyContent:'flex-start',
                paddingHorizontal:10,
                paddingTop: 20,
                flexDirection:'column',
              }}>
                  
                  <Text style={{fontWeight:'500',}}>Enter Wallet Name</Text> 
               
                  <View style = {
                    {
                      flexDirection: "row",
                      justifyContent: "flex-start",
                      marginBottom: 5,
                    }
                  } >
                    < TextInput placeholder = "Wallet Name"
                    name = "name"
                    onChangeText = {
                      (name) => {
                        this.setState({
                          name
                        })
                      }
                    }
                    style = {
                      styles.textInput
                    }
                    />
                  </View>
              </View>


              <View style={{
                width:'100%',
                backgroundColor:'#fff',
                marginBottom:5,
                justifyContent:'flex-start',
                paddingHorizontal:10,
                paddingTop: 20,
                flexDirection:'column',
              }}>
                  
                  <Text style={{fontWeight:'500',}}>Amount</Text> 
                  
                  <View style = {
                    {
                      flexDirection: "row",
                      justifyContent: "flex-start",
                      marginBottom: 5,
                    }
                  } >
                    < TextInput keyboardType = 'numeric'
                    name = "litres"
                    onChangeText = {
                      (litres) => {
                        this.setState({
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
                  <Text style={{fontWeight:'500'}}>Select Fuel Type</Text>
                  <View>
                    <ButtonGroup 
                        onPress={this.updateFuelIndex}
                        selectedIndex={selectedFuelType}
                        buttons={buttons2}
                        containerStyle={{height: 40}}
                        selectedButtonStyle={
                          {
                            backgroundColor: colors.accentColor
                          }
                        }
                    />
                  </View>
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
                  <Text style={{fontWeight:'500', marginBottom: 5}}>Select Payment Method</Text>
                  <View>
                    <ButtonGroup 
                        onPress={this.updateIndex}
                        selectedIndex={selectedPaymentMethod}
                        buttons={buttons}
                        containerStyle={{height: 40}}
                        selectedButtonStyle={
                          {
                            backgroundColor: colors.accentColor
                          }
                        }
                    />
                  </View>
                </View>
              </View>

                    <Button
                    title = 'Add New Wallet'
                   onPress = {
                     () => {
                       this._handleAddWallet()
                     }
                   }
                    buttonStyle = {{
                      marginTop:10,
                      backgroundColor: colors.accentColor,
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

          </ScrollView>
      
          
        </Animated.View>  
         
      </View>
    )
  }
}



const styles = StyleSheet.create({
    container: {
        flex: 1,
        backgroundColor: '#f1f1f1',
    },
    contentContainer: {
        paddingTop: 10,
        marginHorizontal: 10,
        backgroundColor: '#f1f1f1'
    },
    textInput: {
        height: 40,
        paddingLeft: 6,
        width: "90%"
    },
    btnItem: {
        height: 30,
        backgroundColor: "#f1f1f1",
        alignItems: "center",
        justifyContent: "center",

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

export default connect(mapStateToProps)(AddWalletScreen);