import React, { Component } from 'react';
import { View, Text, ScrollView, StyleSheet, Platform, TouchableHighlight,TouchableOpacity, Dimensions, Animated } from 'react-native';
import { connect } from 'react-redux';
import { SimpleLineIcons, Entypo } from '@expo/vector-icons'
import colors from '../constants/Colors';

const screen = Dimensions.get('window');

class WalletDetailScreen extends Component {
  static navigationOptions = {
      title: 'Wallet details'
  }

  constructor(props){
    super(props);
      this.state = {
        wallet: {},
      }
  }


  async componentDidMount(){

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

  renderCoupons(){

      if(this.state.wallet.couponStatus == true){
        return (
          <View style={{
            width:'100%',
            backgroundColor:'#fff',
            marginBottom:5,
            justifyContent:'flex-start',
            paddingHorizontal:20,
            paddingTop: 15,
            flexDirection:'column',
          }}>
              <SimpleLineIcons
                  name='drop'
                  size={36}
                  color= {Platform.OS === 'ios' ? colors.iconsGrey : colors.iconsBlue}
              />

              <View style = {
                  {
                    flexDirection: "row",
                    justifyContent: "flex-end",
                  }
                } >
                  <Text style={{
                    fontSize:18, 
                    marginBottom:5,
                    fontWeight:Platform.OS === 'ios' ? '500' : '400',
                  }}>Coupons</Text> 
              </View>
              <View style = {
                {
                  flexDirection: "row",
                  justifyContent: "flex-end",
                  marginVertical: 5,
                }
              } >
                  <Text style={{
                    color:colors.secondaryTextGrey,
                    fontWeight:Platform.OS === 'ios' ? '500' : '400',
                  }}>{this.state.wallet.coupons} Available</Text> 
              </View>
              
              < View style = {
                {
                  justifyContent: "center",
                  alignItems: "center",
                  width: '100%'
                }
              } >

                  < TouchableHighlight style = {
                    [styles.btnItem, {
                        height: 40,
                        justifyContent: "center",
                        alignItems: "center",
                        borderRadius: 20,
                        backgroundColor: colors.accentColor,
                        width: "50%",
                        marginBottom: 15,
                  }]}
                    
                  onPress = {
                    () => {
                      this.props.navigation.navigate('Coupons')
                    }
                  }>
                      <View style = {
                        {
                          flexDirection: "row",
                          justifyContent: "center",
                        }
                      } >
                
                            <Text  style={{
                              color: "#fff",
                              fontWeight:Platform.OS === 'ios' ? '500' : '400',
                            }} >View Coupons</Text>


                      </View>

                  </TouchableHighlight>
          
              </View>

          </View>

        );
      }
      return(
        <View>
          
        </View>
      )
      
  }
  
  


  render() {
   
    
    return (
      <View style = {
        styles.container 
      } 
      >
        < Animated.View style = {
          [styles.contentContainer, {
            top: this.changingTop !== undefined &&
              this.changingTop !== undefined ?
              this.changingTop : 120,
          }]
        } >
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
            >

              <View style={{
                width:'100%',
                backgroundColor:'#fff',
                marginBottom:5,
                justifyContent:'flex-start',
                paddingHorizontal:20,
                paddingTop:20,
                flexDirection:'column',
              }}>
                  <SimpleLineIcons
                      name='drop'
                      size={36}
                      color= {Platform.OS === 'ios' ? colors.iconsGrey : colors.iconsBlue}
                  />

                  <View style = {
                      {
                        flexDirection: "row",
                        justifyContent: "flex-end",
                      }
                    } >
                      <Text style={{
                        fontSize:18, 
                        marginBottom:5,
                        fontWeight:Platform.OS === 'ios' ? '500' : '400',
                      }}>Petrol</Text> 
                  </View>
                  <View style = {
                    {
                      flexDirection: "row",
                      justifyContent: "flex-end",
                      marginVertical: 10,
                      
                    }
                  } >

                      <Text style={{
                        color:colors.secondaryTextGrey,
                        fontWeight:Platform.OS === 'ios' ? '500' : '400',
                      }} >{this.state.wallet.petrol}L Available</Text> 

                  </View>
              </View>

              <View style={{
                width:'100%',
                backgroundColor:'#fff',
                marginBottom:5,
                justifyContent:'flex-start',
                paddingHorizontal:20,
                paddingTop:20,
                flexDirection:'column',
              }}>
                  <SimpleLineIcons
                      name='drop'
                      size={36}
                      color= {Platform.OS === 'ios' ? colors.iconsGrey : colors.iconsBlue}
                  />

                  <View style = {
                      {
                        flexDirection: "row",
                        justifyContent: "flex-end",
                      }
                    } >
                      <Text style={{
                        fontSize:18, 
                        marginBottom:5,
                        fontWeight:Platform.OS === 'ios' ? '500' : '400',
                      }}>Diesel</Text> 
                  </View>
                  <View style = {
                    {
                      flexDirection: "row",
                      justifyContent: "flex-end",
                      marginVertical: 10,
                    }
                  } >

                      <Text style={{
                        color:colors.secondaryTextGrey,
                        fontWeight:Platform.OS === 'ios' ? '500' : '400',
                      }}>{this.state.wallet.Diesel}L Available</Text> 


                  </View>
              </View>
              {this.renderCoupons()}

            </ScrollView>   
        </Animated.View>
        
        
         <TouchableOpacity onPress = {
            () => {
              this.props.navigation.navigate('EditWalletScreen', {wallet: this.state.wallet, screen: 'Wallet Edit'})
            }} style={styles.fab}>
          <Entypo
            name={'edit'}
            size={30}
            color={'#fff'}
            style={styles.fabIcon}
          />
        </TouchableOpacity>
        
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

  btnItem: {
    height: 40,
    backgroundColor: "#f1f1f1",
    alignItems: "center",
    justifyContent: "center",

  },

  headerContainer: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    backgroundColor: Platform.OS === 'ios' ? colors.white : colors.headerBlueBlackground,
    paddingTop: 15,
    paddingBottom: 10,
  },

  fab: {
      position: 'absolute',
      width: 50,
      height: 50,
      alignItems: 'center',
      justifyContent: 'center',
      right: 20,
      bottom: 30,
      backgroundColor: colors.accentColor,
      borderRadius: 25,
      elevation: 8
    },

    fabIcon: {
      marginTop: 2,
      marginLeft: 2,
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
export default connect(
  mapStateToProps,
)(WalletDetailScreen);
