import React,{useState, useEffect} from 'react';
import MapView, { Callout, Marker, Polygon } from 'react-native-maps';
import { StyleSheet, View } from 'react-native';
import { Icon, ListItem, Button, Input, Badge, Text } from 'react-native-elements';
import * as Location from 'expo-location';

export default function App() {
  const [location1, setLocation1] = useState(null)
  const [location2, setLocation2] = useState(null)
  const [location3, setLocation3] = useState(null)
  const [locations, setLocations] = useState([{"latitude": -17.7675344, "longitude": 31.1534596}, {"latitude": -17.7674974, "longitude": 31.1534551}, {"latitude": -17.7675449, "longitude": 31.1534087}, {"latitude": -17.7675354, "longitude": 31.1534391}, {"latitude": -17.767539, "longitude": 31.1534486}, {"latitude": -17.767533, "longitude": 31.1534282}, {"latitude": -17.7673953, "longitude": 31.1534679}, {"latitude": -17.7675558, "longitude": 31.1534929}])
  const [isfull, setIsFull] = useState(false);
  const [location, setLocation] = useState({
    longitude:31.143623,
    latitude:-17.768583,
    latitudeDelta: 0.0922,
    longitudeDelta: 0.0421
  });

  const [errorMsg, setErrorMsg] = useState(null);
  const locatePress = async () => {
        let { status } = await Location.requestForegroundPermissionsAsync();
        if (status !== 'granted') {
          setErrorMsg('Permission to access location was denied');
          return;
        }

        let location = await Location.getCurrentPositionAsync({});
        locations.push({latitude:location.coords.latitude,longitude:location.coords.latitude})
        setLocations(locations);
        if(locations.length > 2)
          setIsFull(true);
        console.log(locations.length)

      }

    function calcArea(locations) {

      if (!locations.length) {    
          return 0;
      }
      if (locations.length < 3) {
          return 0;
      }
      let radius = 6371000;
    
      const diameter = radius * 2;
      const circumference = diameter * Math.PI;
      const listY = [];
      const listX = [];
      const listArea = [];
      // calculate segment x and y in degrees for each point
    
      const latitudeRef = locations[0].latitude;
      const longitudeRef = locations[0].longitude;
      for (let i = 1; i < locations.length; i++) {
        let latitude = locations[i].latitude;
        let longitude = locations[i].longitude;
        listY.push(calculateYSegment(latitudeRef, latitude, circumference));
    
        listX.push(calculateXSegment(longitudeRef, longitude, latitude, circumference));
    
      }
    
      // calculate areas for each triangle segment
      for (let i = 1; i < listX.length; i++) {
        let x1 = listX[i - 1];
        let y1 = listY[i - 1];
        let x2 = listX[i];
        let y2 = listY[i];
        listArea.push(calculateAreaInSquareMeters(x1, x2, y1, y2));
    
      }
    
      // sum areas of all triangle segments
      let areasSum = 0;
      listArea.forEach(area => areasSum = areasSum + area)
    
    
      // get abolute value of area, it can't be negative
      let areaCalc = Math.abs(areasSum);// Math.sqrt(areasSum * areasSum);  
      return areaCalc;
    }
    
    function calculateAreaInSquareMeters(x1, x2, y1, y2) {
      return (y1 * x2 - x1 * y2) / 2;
    }
    
    function calculateYSegment(latitudeRef, latitude, circumference) {
      return (latitude - latitudeRef) * circumference / 360.0;
    }
    
    function calculateXSegment(longitudeRef, longitude, latitude, circumference)     {
      return (longitude - longitudeRef) * circumference * Math.cos((latitude * (Math.PI / 180))) / 360.0;
    }

    myArea =calcArea([{latitude:-17.765252, longitude:31.139591},{latitude:-17.768583, longitude:31.143623},{latitude:-17.769993, longitude:31.141520}, {latitude:-17.767473, longitude:31.138448}])
    let text = 'Waiting..';
  if (errorMsg) {
    text = errorMsg;
  } else if (location1) {
    text = JSON.stringify(location1.longitude);
  }
  const isValid = location3!== null &&location2!== null && location1!== null
  return (
    <View style={styles.container}>
      <View style={{height:'60%'}}>
      <MapView style={styles.map} 
      region={location}
      >
        <Marker coordinate={location1} title='lock1'>
          <Icon name='md-location-sharp' type='ionicon' color='red' title='First point'/>
</Marker>
<Marker coordinate={location2}>
          <Icon name='wallet' type='ant-design' title='Second point'/>
</Marker>
<Marker coordinate={location3}>
          <Icon name='wallet' type='ant-design' title='First point'/>
</Marker>
{isfull?(
    <Polygon coordinates={[{latitude:-17.765252, longitude:31.139591},{latitude:-17.768583, longitude:31.143623},{latitude:-17.769993, longitude:31.141520}, {latitude:-17.767473, longitude:31.138448}]}
    fillColor={'rgba(100, 100, 200, 0.3)'}
>
<Callout></Callout>
</Polygon>
):null
}
      </MapView>
      </View>
      <View style={{marginTop:15, marginHorizontal:15, alignItems:'center'}}>
        <Button title='Add location' onPress={()=>{locatePress()}}/>

      </View>
      <View style={{marginTop:15, marginHorizontal:15}}>
        <Text>Position 1: {locations.length===4?'mres':'emma'}</Text>
        <Text>Position 2: {location2?location2.longitude:'ngira'}</Text>
        <Text>Position 3: {location3?location3.longitude:'ngaa'}</Text>
      </View>
      <View style={{marginTop:15, marginHorizontal:15, alignItems:'center'}}>
        <Button title='Show' onPress={()=>{showPress()}}/>
        <Text>{myArea}</Text>
        <Text>{text}</Text>
      </View>

    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
  },
  map: {
    width: '100%',
    height: '100%',
  },
});
