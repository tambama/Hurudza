import React, { Component, createRef } from 'react';
import { 
  Text, 
  View, 
  Dimensions, 
  StyleSheet, 
  Image, 
  ScrollView, 
  TouchableOpacity, 
  TextInput,
  ActivityIndicator,
  SafeAreaView
} from 'react-native';
import { connect } from 'react-redux'
import ActionSheet from "react-native-actions-sheet";
import { Icon, Button, Input, Badge, ListItem } from 'react-native-elements';
import { LinearGradient } from 'expo-linear-gradient';

import Carousel from 'react-native-reanimated-carousel';// Version can be specified in package.json

import colors from '../../constants/Colors'
import Colors from '../../constants/Colors';

import { walletActions } from '../../actions/wallet';
import { stationActions } from '../../actions/station';
import { omcActions } from '../../actions/omc';
import { currencyActions } from '../../actions/currency';
import { Routes } from '../../navigation/Routes';
import currency from 'currency.js';

const SLIDER_WIDTH = Dimensions.get('window').width;
const ITEM_WIDTH = Math.round(SLIDER_WIDTH * 0.80);

class WalletsCarousel extends Component {
  constructor(props) {
    super(props);
    this._renderItem = this._renderItem.bind(this)
    this.actionSheetRef = createRef()
    this.payActionSheetRef = createRef()
    this.topupActionSheetRef = createRef()
    this.transferActionSheetRef = createRef()
    this.createActionSheetRef = createRef()
  }

  state = {
    index: 0,
    nestedScrollEnabled: false,
    productId: 0,
    oilCompanyId: 0,
    currencyId: 0
  }

  _onClose = () => {
    this.setState({nestedScrollEnabled: true})
  };

  _onSlide = (index) => {
    const { dispatch, wallets, onSlide } = this.props;
    this.setState({index: index})
    onSlide(index)
    dispatch(walletActions.select(wallets[index]))
  }

  _selectWallet = (wallet) => {
    const { dispatch, navigation } = this.props;

    dispatch(walletActions.select(wallet));
    //navigation.navigate(Routes.WalletDetail)
  }

  _onSelectOmc = (id) => {
    const { dispatch } = this.props;
    this.setState({oilCompanyId: id})
    dispatch(currencyActions.clear());
    dispatch(currencyActions.getOmcCouponCurrencies(id));
  }

  _onSelectFuelType = (productId) => {
    const { dispatch, selectedWallet } = this.props;
    const { stationCode } = this.state;
    this.setState({productId: productId})
    dispatch(stationActions.getFuelPriceByStationCode(selectedWallet.currencyId, productId, stationCode));
  }
  
  _onCreateClick = () => {
    return;
    this.createActionSheetRef.current?.setModalVisible();
  }

  _onCreatewallet = () => {
    return;
    const { dispatch, user } = this.props;
    const { oilCompanyId, productId, currencyId } = this.state;

    dispatch(walletActions.add({
      ownerId: user.id,
      creatorId: user.id,
      companyId: oilCompanyId, 
      productId: productId,
      currencyId: currencyId
    }))
  }

  componentDidUpdate(){
    const { dispatch, addingWallet, addedWallet, navigation } = this.props;

    if(addedWallet && !addingWallet){
      this.createActionSheetRef.current.setModalVisible();
      this.setState({oilCompanyId: 0, productId: 0, currencyId: 0})
      dispatch(walletActions.clear())
      navigation.navigate(Routes.Wallets)
    }
  }

  componentDidMount(){
    const { wallets, selectedWallet } = this.props;
    if(wallets.length > 0 && Object.entries(selectedWallet).length === 0){
      this._selectWallet(wallets[0])
    }
  }

  _renderItem({ item, index }) {
    if(item.id === 0){
      return (
        <TouchableOpacity style={styles.itemContainer} onPress={() => this._onCreateClick()}>
          <View>
            <View style={{
              position: "absolute",
              right: 15,
            }}>
              <Image
                source={require('../../assets/images/logo-blue.png')}
                resizeMode={'contain'}
                style={{
                  width: 100,
                  height: 70,
                }} />
            </View>
            <View style={{
                flexDirection: 'row',
                justifyContent: 'space-between',
                paddingHorizontal: 25,
                paddingTop: 25
            }}>
                <Text style={{
                    color: colors.black,
                    fontSize: 20
                }}>create a new</Text>
            </View>
            <View style={{
                paddingHorizontal: 25,
                marginTop: 5
            }}>
                <Text style={{
                    color: colors.black,
                    fontSize: 24
                }}>Wallet</Text>
            </View>
          </View>
          <View style={{
              marginBottom: 15,
              marginHorizontal: 25,
              width: '35%',
              shadowColor: colors.primaryBlue,
              shadowOffset: {
                width: 0,
                height: 5,
              },
              shadowOpacity: 0.25,
              shadowRadius: 3.84,
              elevation: 5,
          }}>
            <LinearGradient
                // Background Linear Gradient
                colors={['#52B6FE', colors.primaryBlue]}
                start={{x:0, y:0}}
                end={{x:1, y:1}}
                style={{
                    position: 'absolute',
                    left: 0,
                    right: 0,
                    top: 0,
                    height: '100%',
                    borderRadius: 16
                }}
            />
              <Button
                title='create'
                onPress={() => this._onCreateClick()}
                buttonStyle={{
                  backgroundColor: 'transparent'
                }}
              />
          </View>
        </TouchableOpacity>
      )
    } else {
      
      function currencyFormat(num) {                                          // convert balance with Regex
        return num.toFixed(2).replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1,')
      }
      const balance = currencyFormat(item.balance);
      return (
        <TouchableOpacity style={{...styles.itemContainer, backgroundColor: index === this.state.index ? colors.white: colors.accentColor}} onPress={() => this._selectWallet(item)}>
            <View>
              <View style={{
                position: "absolute",
                right: 15,
              }}>
                <Image
                  source={require('../../assets/images/logo-blue.png')}
                  resizeMode={'contain'}
                  style={{
                    width: 100,
                    height: 70,
                  }} />
              </View>
              <View style={{
                  flexDirection: 'row',
                  justifyContent: 'space-between',
                  paddingHorizontal: 25,
                  paddingTop: 25
              }}>
                  <Text style={{
                      color: index === this.state.index ? colors.black: colors.white
                  }}>{item.shortName}</Text>
              </View>
              <View style={{
                  paddingHorizontal: 25,
                  marginTop: 5
              }}>
                  <Text style={{
                      color: index === this.state.index ? colors.black: colors.white,
                      marginTop:10,
                      fontSize: 30,
                  }}>{item.currency} {balance}</Text>
              </View>
            </View>
            <View style={{
                marginBottom: 15,
                paddingHorizontal: 25
            }}>
                <Text style={{
                      color: index === this.state.index ? colors.black: colors.white,
                }}>{item.company}</Text>
                <Text style={{
                      color: index === this.state.index ? colors.black: colors.white,
                  }}>{item.accountNumber.match(new RegExp('.{1,4}', 'g')).join("-")}</Text>
            </View>
        </TouchableOpacity>
      );
    }
  }
  
  render() {
    const { nestedScrollEnabled, liters, productId, paymentMethods, oilCompanyId, currencyId, topupAmount, topupLiters, transferAmount, transferLiters, paymentMethod, mobileNumber } = this.state;
    const { products, wallets, addingWallet, loading, omcs, selectedWallet, currencies } = this.props

    
    if(wallets.length === 0 && !loading){
      wallets.unshift({id: 0})
    }

    let companies = omcs.map(omc => {
      return { key: omc.id, label: omc.name, value: omc.id }
    })

    let newCurrencies = currencies;
        if(productId !== 0){
            newCurrencies = currencies.filter(c => c.productId === productId)
        }

    let currs = newCurrencies.map(currency => {
      return { key: currency.id, label: currency.name, value: currency.id }
    })

    let isCreateWalletValid = oilCompanyId > 0 && productId > 0 && currencyId > 0

    return (
      <View style={{justifyContent:'center'}}>
        <Carousel
      
      ref={(c) => this.carousel = c}
      data={wallets}
      width= {SLIDER_WIDTH}
      height={SLIDER_WIDTH*0.8}
      //containerCustomStyle={styles.carouselContainer}
      inactiveSlideShift={0}
      onSnapToItem={(index) => this._onSlide(index)}
      renderItem={this._renderItem}
      useScrollView={true}
      mode='parallax'
      style={{ width: SLIDER_WIDTH, alignItems:'center'}}  
      />
  </View>
    );
  }
}

const styles = StyleSheet.create({
  carouselContainer: {
    marginLeft: -25
  },
  slideStyle: {
    flexDirection: 'column',
    justifyContent: 'center'
  },
  itemContainer: {
    width: '100%',
    height: '80%',
    marginLeft: 0,
    marginRight: 0,
    paddingHorizontal: 0,
    borderRadius:16,
    alignSelf: 'flex-start',
    justifyContent: 'space-between',
    backgroundColor: Colors.white,
    shadowColor: colors.shadowBlue,
    shadowOffset: {
      width: 0,
      height: 5,
    },
    shadowOpacity: 0.25,
    shadowRadius: 3.84,
    elevation: 5,
  },
  itemLabel: {
    color: 'white',
    fontSize: 24
  },
  counter: {
    marginTop: 25,
    fontSize: 30,
    fontWeight: 'bold',
    textAlign: 'center'
  },
  actionSheetContainer: {
    width: '100%',
    padding: 12,
    height: 300,
  },
  actionsContainer: {
    flexDirection: 'row',
    justifyContent: 'space-evenly',
    alignItems: 'center',
    marginBottom: 15,
  },
  action: {
    width: 60,
    height: 60,
    borderRadius: 10,
    backgroundColor: colors.lightGray1,
    flexDirection: 'column',
    justifyContent: 'center'
  },
  actionTitle: {
    alignSelf: 'center', 
    marginTop: 5, 
    fontSize: 14,
    fontWeight: '500',
    color: colors.primaryBlue
  },
  inputContainer: {
    borderColor:'transparent',
  },
  textInputContainer: {
    width: '100%',
    height: 50,
    borderRadius: 5,
    borderWidth: 1,
    borderColor: colors.lightGray1,
    marginVertical: 15,
    paddingHorizontal: 10,
    fontSize: 16,
  },
  textInput: {
    fontSize: 16
  },
  pickerContainer: {
    borderWidth: 1,
    borderColor: colors.lightGray1,
    borderRadius: 5
  },
  picker: {
    width: '100%',
    height: 50,
    paddingHorizontal: 10,
    fontSize: 16,
  },
  amountContainer: {
    flexDirection:'row',
    justifyContent: 'flex-end',
    marginBottom: 15,
  }
});

const pickerSelectStyles = StyleSheet.create({
  inputIOS: {
    fontSize: 16,
    paddingVertical: 12,
    paddingHorizontal: 10,
    borderWidth: 1,
    borderColor: colors.lightGray1,
    borderRadius: 4,
    color: 'black',
    paddingRight: 30, // to ensure the text is never behind the icon
  },
  inputAndroid: {
    fontSize: 16,
    paddingHorizontal: 10,
    paddingVertical: 8,
    borderWidth: 0.5,
    borderColor: colors.lightGray1,
    borderRadius: 8,
    color: 'black',
    paddingRight: 30, // to ensure the text is never behind the icon
  },
  iconContainer: {
    top: 15,
    right: 10,
  },
});

function mapStateToProps(state) {
  const { user } = state.authentication
  const { wallets, wallet, selectedWallet, loading, paying, paid, addingWallet, addedWallet, updatingWallet, updatedWallet, transfering, transfered } = state.wallets
  const { products, selectedFuelType } = state.products
  const { fuelPrice, loadingPrice, loadingStation, stationError, selectedStation } = state.stations
  const { omcs, omcFuelPrice } = state.omcs
  const { currencies } = state.currencies;
  return {
      user,
      wallets,
      wallet,
      selectedWallet,
      paying,
      paid,
      addingWallet,
      addedWallet,
      updatingWallet,
      updatedWallet,
      loading,
      products,
      selectedFuelType,
      fuelPrice,
      loadingPrice,
      loadingStation,
      stationError,
      selectedStation,
      omcs,
      omcFuelPrice,
      transfering,
      transfered,
      currencies
  };
}

export default connect(mapStateToProps)(WalletsCarousel);
