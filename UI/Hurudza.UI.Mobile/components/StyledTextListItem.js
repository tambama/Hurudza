import React from 'react';
import { Text } from 'react-native';
import { Icon, ListItem } from 'react-native-elements'
import * as Font from 'expo-font';

import { MontserratText } from './StyledText'

export class MontserratListItem extends React.Component {

  render() {
    return <ListItem {
      ...this.props}
      title={
        <MontserratText style={this.props.titleStyle}>{this.props.title}</MontserratText>
      }
      subtitle={
        <MontserratText style={this.props.subtitleStyle}>{this.props.subtitle}</MontserratText>
      }
      rightTitle={
        <MontserratText style={this.props.rightTitleStyle}>{this.props.rightTitle}</MontserratText>
      }
    />;
  }
}
