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
import { DEFAULT_NAVBAR_HEIGHT } from '../constants/constants'
import Constants from 'expo-constants';



export default class Header extends React.Component {

  _goBack(){
    const { navigation } = this.props;
    navigation.goBack();
  }

  render() {
    const { title } = this.props
    return (
      <View style={styles.header}>
        <TouchableOpacity style={styles.backArrow} onPress={() => this._goBack()}>
          <Icon
            type='ionicon'
            name={Platform.OS === 'ios'? 'ios-arrow-back' : 'md-arrow-back'}
            size={26} />
        </TouchableOpacity>
        <View style={styles.headerTitleContainer}>
          <Text style={styles.headerTitle}>{title}</Text>
        </View>
      </View>
    )
  }
}

const styles = StyleSheet.create({
  header: {
    width: '100%',
    height: DEFAULT_NAVBAR_HEIGHT + Constants.statusBarHeight,
    backgroundColor: colors.lightGray1,
    flexDirection: 'row',
    paddingHorizontal: 10,
    paddingTop: Constants.statusBarHeight,
  },
  headerTitleContainer: {
    flexDirection: 'row',
    justifyContent: 'center',
    width: '70%'
  },
  headerTitle: {
    fontFamily: 'montserrat-regular',
    fontSize: 20,
    alignSelf: 'center'
  },
  backArrow: {
    justifyContent: 'center',
    width: 50,
    height: 50,
    //backgroundColor: colors.white,
    borderRadius: 100,
    alignSelf: "center"
  },
});
