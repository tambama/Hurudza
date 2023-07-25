import React from 'react';
import { Text } from 'react-native';
import * as Font from 'expo-font';

export class MontserratText extends React.Component {
  constructor(props){
    super(props) 

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
    return <Text {
      ...this.props} 
      style={
        [
          this.props.style, 
          { 
            fontFamily: this.state.fontLoaded ? 'montserrat-regular' : null, 
          }
        ]
    } 
    />;
  }
}
