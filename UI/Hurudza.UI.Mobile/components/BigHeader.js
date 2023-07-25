import React from 'react';
import { 
  StyleSheet, 
  Text, 
  View, 
  TouchableOpacity,
  Platform 
} from 'react-native';
import { Icon } from 'react-native-elements';
import colors from '../constants/Colors';
import Constants from 'expo-constants';



export default class BigHeader extends React.Component {
  render() {
    const { title, rightIcon } = this.props
    return (
      <View style={styles.welcomeContainer}>
        <Text style={styles.headerTitle}>{title}</Text>
        {
          rightIcon
        }
      </View>
    )
  }
}

const styles = StyleSheet.create({
  welcomeContainer: {
    height: Platform.OS === 'ios' ? 100 : 63,
    paddingTop: Platform.OS === 'ios' ? Constants.statusBarHeight : 0,
    flexDirection:'row',
    justifyContent: 'space-between',
    backgroundColor: Platform.OS === 'ios' ? colors.lightGray1 : colors.lightGray1,
    marginBottom: 10,
  },
  headerTitle: {
    fontSize: 24,
    fontWeight: '500',
    marginHorizontal: 15,
    paddingTop: 7,
    fontFamily:'montserrat-bold'
  },
});
