import React, { Component, createRef } from 'react';
import { Text, View, Dimensions, StyleSheet, Image, ScrollView, TouchableOpacity, TextInput } from 'react-native';
import ActionSheet from "react-native-actions-sheet";

import Carousel from 'react-native-snap-carousel'; // Version can be specified in package.json
import { LinearGradient } from 'expo-linear-gradient';

import { scrollInterpolator, animatedStyles } from '../../utils/animations';
import colors from '../../constants/Colors'
import Colors from '../../constants/Colors';

const SLIDER_WIDTH = Dimensions.get('window').width;
const ITEM_WIDTH = Math.round(SLIDER_WIDTH * 0.95);
const ITEM_HEIGHT = Math.round(ITEM_WIDTH * 3 / 4);

const DATA = [];
for (let i = 0; i < 1; i++) {
  DATA.push(i)
}

export default class WalletsCarousel extends Component {
  
  state = {
    index: 0,
    nestedScrollEnabled: false,
  }

  constructor(props) {
    super(props);
    this._renderItem = this._renderItem.bind(this)
    this.actionSheetRef = createRef()
  }

  _onClose = () => {
    this.setState({nestedScrollEnabled: true})
  };

  _renderItem({ item }) {
    return (
      <TouchableOpacity style={styles.itemContainer} onPress={() => this.actionSheetRef.current.setModalVisible()}>
          <View>
            <View style={{
              position: "absolute",
              right: 15,
            }}>
              <Image
                source={require('../../assets/images/omc/trek.png')}
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
                    color: colors.black
                }}>Petrol</Text>
            </View>
            <View style={{
                paddingHorizontal: 25,
                marginTop: 5
            }}>
                <Text style={{
                    color: colors.black,
                    fontSize: 26
                }}>$350</Text>
            </View>
          </View>
          <View style={{
              marginBottom: 15,
              paddingHorizontal: 25
          }}>
              <Text style={{
                    color: colors.black,
                }}>Peniel Tambama</Text>
              <Text style={{
                    color: colors.black,
                }}>3011 - 5145 - 3566 - 6781</Text>
          </View>
      </TouchableOpacity>
    );
  }
  
  render() {
    const { nestedScrollEnabled } = this.state;
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
        <ActionSheet
          initialOffsetFromBottom={0.5}
          ref={this.actionSheetRef}
          bounceOnOpen={true}
          bounciness={8}
          gestureEnabled={true}
          onClose={() => this._onClose()}
          defaultOverlayOpacity={0.3}>
          <ScrollView
            nestedScrollEnabled={true}
            scrollEnabled={nestedScrollEnabled}
            style={{
              width: '100%',
              padding: 12,
              maxHeight: 500,
            }}>
            <View
              style={{
                flexDirection: 'row',
                justifyContent: 'space-between',
                alignItems: 'center',
                marginBottom: 15,
              }}>
              {['#4a4e4d', '#0e9aa7', '#3da4ab', '#f6cd61', '#fe8a71'].map(
                color => (
                  <TouchableOpacity
                    onPress={() => {
                      actionSheetRef.current?.setModalVisible();
                    }}
                    key={color}
                    style={{
                      width: 60,
                      height: 60,
                      borderRadius: 100,
                      backgroundColor: color,
                    }}
                  />
                ),
              )}
            </View>

            <TextInput
              style={{
                width: '100%',
                minHeight: 50,
                borderRadius: 5,
                borderWidth: 1,
                borderColor: '#f0f0f0',
                marginBottom: 15,
                paddingHorizontal: 10,
              }}
              multiline={true}
              placeholder="Write your text here"
            />

            <View style={{}}>
              {[
                100,
                60,
                150,
                200,
                170,
                80,
                41,
                101,
                61,
                151,
                202,
                172,
                82,
                43,
                103,
                64,
                155,
                205,
                176,
                86,
                46,
                106,
                66,
                152,
                203,
                173,
                81,
                42,
              ].map(item => (
                <TouchableOpacity
                  key={item}
                  onPress={() => {
                    actionSheetRef.current?.setModalVisible();
                  }}
                  style={{
                    flexDirection: 'row',
                    justifyContent: 'space-between',
                    alignItems: 'center',
                  }}>
                  <View
                    style={{
                      width: item,
                      height: 15,
                      backgroundColor: '#f0f0f0',
                      marginVertical: 15,
                      borderRadius: 5,
                    }}
                  />

                  <View
                    style={{
                      width: 30,
                      height: 30,
                      backgroundColor: '#f0f0f0',
                      borderRadius: 100,
                    }}
                  />
                </TouchableOpacity>
              ))}
            </View>
          </ScrollView>
        </ActionSheet>
      
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
    justifyContent: 'space-between',
    backgroundColor: Colors.white,
    shadowColor: colors.primaryBlue,
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
  }
});
