import React from 'react';
import { StyleSheet, Image, Text, View, TouchableOpacity, Platform } from 'react-native';
import colors from '../constants/Colors';
import * as Font from 'expo-font';


//Station Image Logos
const logos = {
    puma: {
        uri: require('../assets/images/puma-logo.png')
    },
    total: {
        uri: require('../assets/images/total-logo.png')
    },
    zuva: {
        uri: require('../assets/images/zuva-logo.png')
    },
    shell: {
        uri: require('../assets/images/shell-logo.png')
    },
}


export default class CouponItem extends React.Component {
  constructor(props){
    super(props)
        this.logos = {
        puma: {
            uri: require('../assets/images/puma-logo.png')
        },
        total: {
            uri: require('../assets/images/total-logo.png')
        },
        zuva: {
            uri: require('../assets/images/zuva-logo.png')
        },
        shell: {
            uri: require('../assets/images/shell-logo.png')
        },
    }

    this.state = {
      fontLoaded:false
    };

  }

  async componentDidMount() {
    await Font.loadAsync({
      'montserrat-regular': require('../assets/fonts/Montserrat-Regular.ttf'),
    });

    this.setState({ fontLoaded: true });  
  };

  render() {
    const { item } = this.props.coupon;
    return (
        <View
          style={styles.couponContainer}
         >
          <View style={styles.coupon}>
            <View style={styles.stationLogoContainer}>

            <Image
                source={this.logos['total'].uri}
                style={styles.stationLogo}
            />

            </View>
            <View style={styles.fuels}>
              <Text style={
                {
                  ...styles.couponHeaderText,
                  fontWeight:Platform.OS === 'ios' ? '500' : '400',
                  fontFamily: this.state.fontLoaded ? 'montserrat-regular' : null,
                }
              }>
                {item.name}
              </Text>
              <Text style={
                {
                  ...styles.priceText,
                  fontWeight:Platform.OS === 'ios' ? '500' : '400',
                  fontFamily: this.state.fontLoaded ? 'montserrat-regular' : null,
                }
              }>
                {item.litersBalance}L {item.product} avaliable
              </Text>
            </View>

          </View>
        </View>
    );
  }
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    paddingTop: 0,
  },
  stationLogo: {
    height: 50,
    width:50,

  },
  coupon: {
    flexDirection: 'row',
    alignItems: 'center',
    
  },
  stationLogoContainer: {
    borderRightColor: '#f1f1f1',
    borderRightWidth: 1,
    paddingRight: 25,
    marginRight: 25,
  },
  fuels: {
    flexDirection: 'column',
  },
  buyView: {
    flexDirection: 'column',
    justifyContent: 'center',
    alignItems: 'center',
  },
  buyButton: {
    height: 40,
    width: 40,
    backgroundColor: colors.fu2,
    borderRadius: 40/2,
    justifyContent: 'center',
    alignItems: 'center',
    paddingTop: 2,
  },
  optionIconContainer: {
    marginRight: 9,
  },
  couponContainer: {
    backgroundColor: '#fff',
    paddingHorizontal: 15,
    paddingVertical: 15,
    marginTop: 2,
    marginRight: 5,
    marginLeft: 5,
    
  },
  couponHeaderText: {
    fontSize: 14,
    marginTop: 2,
    color: '#000',
    fontWeight: "500",
  },
  priceText: {
    fontSize: 12,
    marginTop: 1,
    color: colors.secondaryTextGrey,
    fontWeight: "500"
    
  },
  
  userBlock: {
    height: 100,
    backgroundColor: '#fff',
    flexDirection: 'row',
  },

});
