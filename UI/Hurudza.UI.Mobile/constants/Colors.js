import React from 'react';
import { Platform } from 'react-native';
const tintColor = '#000';

export default {
  tintColor,
  accentColor: Platform.OS === 'ios' ? '#8a8fb3' : '#434fb5',
  accentColorIOS: '#8a8fb3',
  accentColorAndroid: '#434fb5',
  tabIconSelected: tintColor,
  tabBar: '#fefefe',
  errorBackground: 'red',
  errorText: '#fff',
  warningBackground: '#EAEB5E',
  warningText: '#666804',
  noticeBackground: tintColor,
  noticeText: '#fff',
  placeholderTextColor: '#9EA0A4',
  blue: '#008EFE',
  lightGray1: '#f1f1f1',
  lightGray2: '#ccc',
  inputBorderGray: '#0f0f0f',
  purpleBackground: '#403aba',
  //primaryBlue: '#4F6D7A',
  // blueBackground: '#4d65f5',
  blueBackground: '#3c3c9e',
  fu1: '#1b1649',
  fu2: '#4d65f5',
  fu3: '#836eff',
  fu4: '#4d65f5',
  fu5: '#403aba',
  fu6: '#1c1565',
  fu7: '#434fb5',
  fu8: '#836EFF',
  blueBackground: '#4d65f5',
  iconsGrey: '#d3d3d3',
  iconsBlue: '#434fb5',
  secondaryTextGrey: '#6c6977',
  headerBlueBlackground: '#434fb5', // '#4e67c6', //#202084'
  red: '#d82e52',
  deepRed:'#c4144e',
  purpleBlue:'#4d65f5',
  black: '#000',
  white: '#fff',
  textInputBorderGrey: '#0B84FD',
  transparent:'transparent',
  
  primaryBlue: '#3e4fea',
  activeBlue: '#4d66f5',
  shadowBlue: '#3e4fea',

  // TREK COLORS
  trekGrey: '#828282',

  // Dohwe Colors
  dohweBlue:'#1c5db9'
};
