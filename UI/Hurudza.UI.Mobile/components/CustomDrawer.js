import { StyleSheet, Text, TouchableOpacity, View } from 'react-native'
import React from 'react'
import { DrawerContentScrollView, DrawerItemList } from '@react-navigation/drawer'
import { Avatar, Image } from '@rneui/themed'
import { Icon } from '@rneui/base'
import colors from '../constants/Colors';
import { useNavigation } from '@react-navigation/native'

const CustomDrawer = (props) => {
  const nav = useNavigation();
  return (
    <View style={{flex:1, backgroundColor:colors.lightGray1, borderTopRightRadius:20, borderBottomRightRadius:20}}>
      <View style={{width:'100%', alignItems:'center', flexDirection:'row', paddingHorizontal:20, justifyContent:'space-between', marginTop:50, marginBottom:30}}>
          <Text style={{fontFamily:'montserrat-semi', fontSize:40, color:'#52734D'}}>Hurudza.</Text>
          <TouchableOpacity onPress={() => nav.goBack()}>
          <Icon name='close' type='materiaL' size={30}/>
          </TouchableOpacity>

      </View>
      <DrawerContentScrollView {...props}>

        <DrawerItemList {...props}/>
    </DrawerContentScrollView>
    <View style={{marginBottom:40,marginHorizontal:15}}>
    <TouchableOpacity style={{paddingHorizontal:5, flexDirection:'row',marginBottom:15,justifyContent:'space-between', alignItems:'center', backgroundColor:colors.lightGray2, paddingVertical:15, borderRadius:5}}>
      <View style={{flexDirection:'row'}}>
      <Avatar
    size={45}
    title="PT"
    titleStyle={{color:'#ffffff'}}
    containerStyle={{ backgroundColor: "#52734D", borderRadius:10 }}
    />
        <View style={{marginLeft:10}}>
      <Text style={{fontFamily:'montserrat-semi', fontSize:20}}>Peniel Tambama</Text>
      <Text style={{fontFamily:'montserrat-regular', fontSize:15, marginTop:-3}}>Farm manager</Text>
    </View>
      </View>
      <Icon name='chevron-right' color={'#52734D'} size={30}/>
    </TouchableOpacity>
    <TouchableOpacity style={{flexDirection:'row', justifyContent:'center'}}>
      <Icon name='log-out' type='feather' size={30}/>
      <Text style={{fontFamily:'montserrat-semi', fontSize:20, marginLeft:5}}>Logout</Text>
    </TouchableOpacity>
    </View>
    </View>

  )
}

export default CustomDrawer

const styles = StyleSheet.create({})