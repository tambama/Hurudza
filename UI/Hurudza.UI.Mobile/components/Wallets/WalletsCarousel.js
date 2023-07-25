import React, { Component } from 'react';
import { Text, View, Dimensions, StyleSheet } from 'react-native';

import Carousel from 'react-native-snap-carousel'; // Version can be specified in package.json
import { LinearGradient } from 'expo-linear-gradient';

import { scrollInterpolator, animatedStyles } from '../../utils/animations';
import colors from '../../constants/Colors'

const SLIDER_WIDTH = Dimensions.get('window').width;
const ITEM_WIDTH = Math.round(SLIDER_WIDTH * 0.95);
const ITEM_HEIGHT = Math.round(ITEM_WIDTH * 3 / 4);

const DATA = [];
for (let i = 0; i < 1; i++) {
  DATA.push(i)
}

export default class WalletsCarousel extends Component {
  
  state = {
    index: 0
  }

  constructor(props) {
    super(props);
    this._renderItem = this._renderItem.bind(this)
  }

  _renderItem({ item }) {
    return (
      <View style={styles.itemContainer}>
            <LinearGradient
                // Background Linear Gradient
                colors={[colors.primaryBlue, '#52B6FE']}
                start={{x:0, y:0}}
                end={{x:1, y:1}}
                style={{
                    position: 'absolute',
                    left: 0,
                    right: 0,
                    top: 0,
                    height: '100%',
                    borderRadius:16
                }}
            />
          <View>
            <View style={{
                flexDirection: 'row',
                justifyContent: 'space-between',
                paddingHorizontal: 25,
                paddingTop: 25
            }}>
                <Text style={{
                    color: colors.white
                }}>Petrol</Text>
                <Text style={{
                    color: colors.white
                }}>TREK</Text>
            </View>
            <View style={{
                paddingHorizontal: 25,
                marginTop: 5
            }}>
                <Text style={{
                    color: colors.white,
                    fontSize: 24
                }}>$350</Text>
            </View>
          </View>
          <View style={{
              marginBottom: 15,
              paddingHorizontal: 25
          }}>
              <Text style={{
                    color: colors.white,
                }}>Peniel Tambama</Text>
              <Text style={{
                    color: colors.white,
                }}>3011 - 5145 - 3566 - 6781</Text>
          </View>
      </View>
    );
  }
  
  render() {
    return (
      <View>
        <Carousel
          ref={(c) => this.carousel = c}
          data={DATA}
          renderItem={this._renderItem}
          sliderWidth={SLIDER_WIDTH}
          itemWidth={ITEM_WIDTH}
          containerCustomStyle={styles.carouselContainer}
          inactiveSlideShift={0}
          onSnapToItem={(index) => this.setState({ index })}
          scrollInterpolator={scrollInterpolator}
          slideInterpolatedStyle={animatedStyles}
          useScrollView={true}
          layout={'stack'}
          layoutCardOffset={18}          
        />
      </View>
    );
  }
}

const styles = StyleSheet.create({
  carouselContainer: {
      paddingVertical: 5
  },
  itemContainer: {
    width: '100%',
    height: '100%',
    borderRadius:16,
    justifyContent: 'space-between'
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
  }
});
