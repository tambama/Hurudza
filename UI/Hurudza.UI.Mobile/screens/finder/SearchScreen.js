import React, { createRef } from 'react';
import { connect } from 'react-redux';
import { 
  Platform, 
  View, 
  StyleSheet, 
  Text, 
  ScrollView, 
  Image, 
  TextInput, 
  TouchableOpacity,
  RefreshControl,
  Picker 
} from 'react-native'
import { ListItem, Icon, SearchBar } from 'react-native-elements';
import * as Location from 'expo-location';
import * as Permissions from 'expo-permissions';
import MapView, { Marker, Callout } from 'react-native-maps';
import { stationActions } from '../../actions/station'
import { getStatusBarHeight, isIphoneX, ifIphoneX } from 'react-native-iphone-x-helper';
import getDirections from 'react-native-google-maps-directions'
import DropdownAlert from 'react-native-dropdownalert';

import colors from '../../constants/Colors';

import { alertActions } from '../../actions/alert';

class SearchScreen extends React.Component {
  detailActionSheetRef = createRef()

  state = {
    isSearchFocused: false,
    showSearchBar: false,
    search: '',
    productId: 0,
    locationResult: null,
    location: {coords: { latitude: 37.78825, longitude: -122.4324}},
  };

  componentDidMount() {
    this._getLocationAsync();
  }

  _handleMapRegionChange = mapRegion => {
    this.setState({ mapRegion });
  };

  _getLocationAsync = async () => {
   let { status } = await Permissions.askAsync(Permissions.LOCATION);
   if (status !== 'granted') {
     this.setState({
       locationResult: 'Permission to access location was denied',
       location,
     });
   }

   let location = await Location.getCurrentPositionAsync({});
   this.setState({ locationResult: JSON.stringify(location), location, });
  };

  handleGetDirections = (station) => {
    const { coords } = this.state.location;

    const data = {
       source: {
        latitude: coords.latitude,
        longitude: coords.longitude
      },
      destination: {
        latitude: station.latitude,
        longitude: station.longitude
      },
      params: [
        {
          key: "travelmode",
          value: "driving"        // may be "walking", "bicycling" or "transit" as well
        },
        // {
        //   key: "dir_action",
        //   value: "navigate"       // this instantly initializes navigation using the given travel mode
        // },
        {
          key: "basemap",
          value: "terrain"        // Defines the type of map to display. The value can be either roadmap (default), satellite, or terrain.
        }
      ],
    }
 
    getDirections(data)
  }

  UNSAFE_componentWillMount(){
    const { dispatch, currentLocation } = this.props;
    if(currentLocation !== undefined){
      this.setState({location: currentLocation})
    }

    const { coords } = currentLocation
    dispatch(stationActions.getNearestServiceStations(coords.latitude, coords.longitude, 3));
  }

  componentDidUpdate(){
    const { type, message, dispatch, loading, loaded } = this.props;
    const { showSearchBar } = this.state

    if(type === 'alert-danger'){
      this.dropDownAlertRef.alertWithType('error', 'Error', message);
      dispatch(alertActions.clear());
    }

    if(!loading && loaded){
      this.setState({showSearchBar: !showSearchBar})
      dispatch(stationActions.clear())
    }

  } 

  _updateSearch = (search) => {
    this.setState({ search });
  };

  _onSearch = () => {
    const { dispatch, currentLocation } = this.props;
    const { search } = this.state;

    const { coords } = currentLocation;
    if(search !== ''){
      dispatch(stationActions.searchNearestServiceStations(coords.latitude, coords.longitude, search, 5));
    } else {
      dispatch(stationActions.getNearestServiceStations(coords.latitude, coords.longitude, 3));
    }
  } 

  _onRefresh = () => {
    const { dispatch, currentLocation } = this.props;

    const { coords } = currentLocation;
    dispatch(stationActions.getNearestServiceStations(coords.latitude, coords.longitude, 3));
  }

  _renderSearch = () => {
    const { showSearchBar, search } = this.state
    const { loading }= this.props
    return (
      <View style={{
        width: '85%',
        position: 'absolute',
        top: Platform.OS === 'ios' ? 50 : 20,
        left: 25,
      }}>
        {
          !showSearchBar && (
            <TouchableOpacity style={{
              width: 50,
              height: 50,
              backgroundColor: colors.white,
              opacity: 0.7,
              borderRadius: 50,
              justifyContent: 'center',
              shadowColor: colors.primaryBlue,
              shadowOffset: {
                width: 0,
                height: 5,
              },
              shadowOpacity: 0.25,
              shadowRadius: 3.84,
              elevation: 5,
            }}
            onPress={() => this.setState({showSearchBar: !showSearchBar})}>
              <Icon
                type='material-community'
                name='map-search-outline' />
            </TouchableOpacity>
          )
        }
        {
          showSearchBar && (
            <View>
              <SearchBar
                platform={ Platform.OS === 'ios' ? 'ios' : 'android' }
                placeholder='Search here'
                onChangeText={this._updateSearch}
                value={search}
                showLoading={loading}
                onCancel={() => this.setState({showSearchBar: !showSearchBar})} 
                onSubmitEditing={() => this._onSearch()}
                returnKeyType='search'
                containerStyle={ Platform.OS === 'ios' ? {
                  height: 50,
                  borderRadius: 30
                } : {
                  width: '100%',
                  borderRadius: 30,
                }}
                inputContainerStyle={ 
                  Platform.OS === 'ios' ? {
                  backgroundColor: colors.white,
                } : null } />
            </View>
          )
        }
      </View>
    )
  }

  render() {
    /* Go ahead and delete ExpoConfigView and replace it with your
     * content, we just wanted to give you a quick view of your config */
    const { stations, loading, provider } = this.props;
    const { isSearchFocused,location } = this.state;
    return (
      <View style={styles.container}>
        <MapView
          showsUserLocation={true}
          followsUserLocation={false}
          loadingEnabled={true}
          provider={provider}
          style={styles.map}
          initialRegion={{ latitude: location.coords.latitude, longitude: location.coords.longitude, latitudeDelta: 0.0922, longitudeDelta: 0.0421 }} >
              {
                  stations !== undefined && stations.length > 0 && stations.map(station => (
                      <Marker
                          title={station.name}
                          description={station.address}
                          key={station.id}
                          coordinate={{latitude: station.latitude, longitude: station.longitude}}
                        >
                          <Callout>
                            {
                              this._renderStation(station, false)
                            }
                          </Callout>
                      </Marker>
                  ))
              }
        </MapView>
        {
          this._renderSearch()
        }
        <ScrollView style={{
          flex: 1, 
          width: '95%', 
          height: 200,
          backgroundColor: 'transparent',
          position: "absolute",
          bottom: 30,
        }}
        refreshControl={
          <RefreshControl refreshing={loading} onRefresh={() => this._onRefresh()} />
        }
        showsVerticalScrollIndicator={false}>
          {
            stations !== undefined && stations.length > 0 ? stations.map(station => this._renderStation(station, true)) : null
          }
        </ScrollView>
        <DropdownAlert inactiveStatusBarStyle='dark-content' inactiveStatusBarBackgroundColor='#fff' ref={ref => this.dropDownAlertRef = ref} />
      </View>
    );
  }

  _renderStation = (station, show) => {
    return (
      <TouchableOpacity 
        onPress={() => this.handleGetDirections(station)}
        key={station.id.toString()}
        style={{
          flex: 1,
          flexDirection: 'row',
          width: '100%',
          height: 70,
          backgroundColor: '#fff',
          opacity: 0.9,
          alignItems: 'center',
          marginBottom: 5,
          borderRadius: 5
        }}>
        <View style={{
          width: '100%',
          paddingHorizontal: 15,
          alignItems: 'flex-start',
        }}>
          <View style={{
            width: '100%',
            flexDirection: 'row',
            justifyContent: 'space-between'
          }}>
            <View>
              <Text style={{color:'#8a8fb3', fontWeight: '600', marginBottom: 3}}>{station.address}</Text>
              <View style={{
                flexDirection: 'row'
              }}>
              {
                station.fuelPrices.map((price, index) => (
                  <Text key={price.id} style={{color: '#1b1649', fontWeight: '400', marginLeft: 2}}>{index === 0 ? '' : ' | '}{price.currency} {price.product}</Text>
                ))
              }
              </View>
            </View>
            {
              show && (
                <View style={{
                  justifyContent: 'center'
                }}>
                  <Icon
                    type='material-community'
                    name='directions'
                    size={26}
                    color={colors.blue} />
                </View>
              )
            }
          </View>
          <View style={{
            flexDirection: 'row'
          }}>
            <View style={{
              width: 10,
              height: 10,
              backgroundColor: '#1bbd1b',
              borderRadius: 50,
              marginTop: Platform.OS === 'ios' ? 5 : 7,
              marginLeft: 2
            }}>
            </View>
            <Text style={{ 
              marginLeft: 5, 
              fontWeight: Platform.OS === 'ios' ? '200' : '100', 
              marginTop: Platform.OS === 'ios' ? 1 : 0,
            }}>{station.distance.toFixed(1)} Km</Text>
          </View>
        </View>

      </TouchableOpacity>
    )
  }
}

const styles = StyleSheet.create({
  container: {
    ...StyleSheet.absoluteFillObject,
    justifyContent: 'flex-end',
    alignItems: 'center',
  },
  headerContainer: {
    flex: 1,
    flexDirection: "row",
    justifyContent: "space-between",
    position: 'absolute',
    top: getStatusBarHeight(),
    height: 40,
    width: '100%',
    paddingTop:10
  },
  headerTouchable: {
  },
  headerNavigationIcons: {
    color: Platform.OS === "ios" ? '#000000' : '#ffffff',
    marginHorizontal:15 
  },
  map: {
    ...StyleSheet.absoluteFillObject,
  },
  // on the style sheet
  callout: {    
    flex:1,
    flexDirection: 'row',
    justifyContent: "center",
    position: 'absolute',
    top: getStatusBarHeight(),
  },
  calloutView: {
    flexDirection: "row",
    backgroundColor: "rgba(255, 255, 255, 0.9)",
    borderRadius: 10,
    width: "90%",
    marginTop: 20,
  },
  calloutSearch: {
    borderColor: "transparent",
    marginLeft: 10,
    width: "90%",
    height: 40,
    borderWidth: 0.0  
  }
});

const customMapStyle = [
  {
    elementType: 'geometry',
    stylers: [
      {
        color: '#242f3e',
      },
    ],
  },
  {
    elementType: 'labels.text.fill',
    stylers: [
      {
        color: '#746855',
      },
    ],
  },
  {
    elementType: 'labels.text.stroke',
    stylers: [
      {
        color: '#242f3e',
      },
    ],
  },
  {
    featureType: 'administrative.locality',
    elementType: 'labels.text.fill',
    stylers: [
      {
        color: '#d59563',
      },
    ],
  },
  {
    featureType: 'poi',
    elementType: 'labels.text.fill',
    stylers: [
      {
        color: '#d59563',
      },
    ],
  },
  {
    featureType: 'poi.park',
    elementType: 'geometry',
    stylers: [
      {
        color: '#263c3f',
      },
    ],
  },
  {
    featureType: 'poi.park',
    elementType: 'labels.text.fill',
    stylers: [
      {
        color: '#6b9a76',
      },
    ],
  },
  {
    featureType: 'road',
    elementType: 'geometry',
    stylers: [
      {
        color: '#38414e',
      },
    ],
  },
  {
    featureType: 'road',
    elementType: 'geometry.stroke',
    stylers: [
      {
        color: '#212a37',
      },
    ],
  },
  {
    featureType: 'road',
    elementType: 'labels.text.fill',
    stylers: [
      {
        color: '#9ca5b3',
      },
    ],
  },
  {
    featureType: 'road.highway',
    elementType: 'geometry',
    stylers: [
      {
        color: '#746855',
      },
    ],
  },
  {
    featureType: 'road.highway',
    elementType: 'geometry.stroke',
    stylers: [
      {
        color: '#1f2835',
      },
    ],
  },
  {
    featureType: 'road.highway',
    elementType: 'labels.text.fill',
    stylers: [
      {
        color: '#f3d19c',
      },
    ],
  },
  {
    featureType: 'transit',
    elementType: 'geometry',
    stylers: [
      {
        color: '#2f3948',
      },
    ],
  },
  {
    featureType: 'transit.station',
    elementType: 'labels.text.fill',
    stylers: [
      {
        color: '#d59563',
      },
    ],
  },
  {
    featureType: 'water',
    elementType: 'geometry',
    stylers: [
      {
        color: '#17263c',
      },
    ],
  },
  {
    featureType: 'water',
    elementType: 'labels.text.fill',
    stylers: [
      {
        color: '#515c6d',
      },
    ],
  },
  {
    featureType: 'water',
    elementType: 'labels.text.stroke',
    stylers: [
      {
        color: '#17263c',
      },
    ],
  },
];

function mapStateToProps(state) {
  const { user } = state.authentication
  const { stations, loading } = state.stations
  const { currentLocation } = state.location;
  return {
      user,
      stations,
      loading,
      currentLocation
  };
}

export default connect(mapStateToProps)(SearchScreen);
