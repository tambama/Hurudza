import React from 'react';
import { StyleSheet, Image, Text, View, TouchableOpacity, Platform } from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import { Input , Icon, ListItem } from 'react-native-elements';
import moment from 'moment'
import colors from '../constants/Colors';
import { MontserratListItem } from '../components/StyledTextListItem'

export default class ActivityItem extends React.Component {

  render() {
    const sentIcon = <Ionicons name={'ios-paper-plane'} color={'#ccc'} size={16} />
    const requestIcon = <Ionicons name={'ios-repeat'} color={'#ccc'} size={18} />
    const { item, user } = this.props;
    var fontWeight = item.isRead ? '100' : '500';
    var backgroundColor = item.isRead ? colors.white : '#f0f3f6';

    var message = item.description;
    if(item.requestTo === user.id && item.granted){
      message = `You sent ${item.grantedLiters}L of ${item.product} to ${item.requestFromUser}`;
    } 

    if(item.requestTo === user.id && !item.granted){
      message = `You have a ${item.liters}L ${item.product} request from ${item.requestFromUser}`;
    } 

    if(item.requestFrom === user.id && item.granted){
      message = `You received ${item.grantedLiters}L of ${item.product} from ${item.requestFromUser}`;
    }

    if(item.requestFrom === user.id && !item.granted){
      message = `You requested ${item.liters}L of ${item.product} from ${item.requestFromUser}`;
    }

    return (
        <ListItem
          style={styles.requestItem}
          onPress={() => {(item.granted || item.declined) ? null : this.props.toDetail(item)}}>
            <ListItem.Content>
              <View style={styles.requestItemContent}>
                <View style={styles.requestItemContentLeft}>
                  {item.requestTo === user.id ? requestIcon : sentIcon }
                  <View style={styles.requestItemContentMiddle}>
                    <View style={styles.requestItemTitleContainer}>
                      <Text style={styles.requestItemTitlePrefix}></Text>
                      <Text style={styles.requestItemTitle}>{item.requestFromUser}</Text>
                    </View>
                    <Text style={styles.requestItemSubtitle}>{message}</Text>
                  </View>
                </View>
                <View style={styles.requestItemRightText}>
                  <Text>{moment(item.createdDate).calendar()}</Text>
                </View>
              </View>
            </ListItem.Content>
        </ListItem>
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
  activity: {
    flexDirection: 'row',
    width:'100%'
  },
  activityIconContainer: {
    marginRight: 25,
    alignItems: 'center',
    justifyContent: 'center',
  },
  activityInfo: {
    flexDirection: 'column',
  },
  activityContainer: {
    backgroundColor: '#fff',
    paddingHorizontal: 15,
    height: 80,
    justifyContent: 'center',
    marginTop: 2,
    marginRight: 5,
    marginLeft: 5,
  },
  couponHeaderText: {
    fontSize: 12,
    marginTop: 2,
    color: '#000',
  },
  priceText: {
    fontSize: 10,
    marginTop: 1,
    color: '#000',
  },
  userBlock: {
    height: 100,
    backgroundColor: '#fff',
    flexDirection: 'row',
  },
  date: {
    fontSize: 10,
    fontWeight: '100',
    color: colors.iconsGrey,
  },
  subtitleStyle: {
    fontSize: 10,
    fontWeight:'400', 
    color:colors.accentColor,
  },
  rightTitleStyle: {
    fontSize:8,
    fontWeight:'400',
    color: colors.black
  },
  request:{
    height: '38%'
  },
  requestItemContent: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    width: '100%'
  },
  requestItemContentLeft: {
    flexDirection: 'row'
  },
  requestItemContentMiddle: {
    marginLeft: 15
  },
  requestTitleContainer: {
    flexDirection:'row',
    justifyContent:'space-between',
    paddingHorizontal: 15,
    backgroundColor: colors.white,
    paddingTop: 5,
    marginTop: 5,
  },
  requestTitle:{
    fontSize:18,
    fontWeight: '500'
  },
  requestTitleRight: {
    fontWeight: '200'
  },
  requestItem: {
    marginBottom: 5
  },
  requestItemTitleContainer:{
    flexDirection: 'row',
    marginBottom: 4
  },
  requestItemTitlePrefix: {
    fontWeight: '100'
  },
  requestItemTitle:{
    fontSize: 15,
    fontWeight: '500'
  },
  requestItemSubtitle: {
    fontSize: 12,
    fontWeight: '200'
  },
  requestItemRightText:{
    fontSize: 16,
    fontWeight: '500',
    alignSelf: 'center'
  },
});
