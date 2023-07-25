import React from 'react';
import { Text } from 'react-native';
import * as Font from 'expo-font';
import { Input } from 'react-native-elements';
import colors from '../../constants/Colors';

import { MontserratText } from './StyledText'

export class MontserratInput extends React.Component {
  static propTypes = {
    isEnabled: PropTypes.bool
  }

  state = {
    isFocused: false
  }

  focus = () => this.textInputRef.focus()

  render() {
    const { isEnabled, ...otherProps } = this.props
    const { isFocused } = this.state
    const color = isEnabled ? colors.black : colors.lightGray2
    const placeholderTextColor = colors.lightGray2
    const borderColor = isFocused ? colors.black : colors.lightGray2
    return <Input {
      ...this.props} 
      label={
        <MontserratText style={this.props.labelStyle}>{this.props.label}</MontserratText>
      }
      ref={(ref) => this.textInputRef = ref}
    />;
  }
}
