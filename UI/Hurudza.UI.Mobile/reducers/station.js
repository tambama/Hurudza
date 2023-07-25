import { stationConstants } from '../constants/station';

const initialState = {
    addingStation: false,
    addedStation: false,
    loading: false,
    stations: [],
    fuelPrice: {},
    stationError: '',
    loadingStation: false,
    selectedStation: {}
  }

export function stations(state = initialState, action) { 
  switch (action.type) {
    case stationConstants.GET_ALL_STATIONS_REQUEST:
      return {
        ...state,
        loading: true
      };
    case stationConstants.GET_ALL_STATIONS_SUCCESS:
      return {
        ...state,
        stations: action.stations,
        loading: false
      };
    case stationConstants.GET_ALL_STATIONS_FAILURE:
      return { 
        error: action.error,
        loading: false
      };
      case stationConstants.GET_NEAREST_SERVICE_STATIONS_REQUEST:
        return {
          ...state,
          loading: true
        };
      case stationConstants.GET_NEAREST_SERVICE_STATIONS_SUCCESS:
        return {
          ...state,
          stations: action.stations,
          loading: false
        };
      case stationConstants.GET_NEAREST_SERVICE_STATIONS_FAILURE:
        return { 
          error: action.error,
          loading: false
        };
        case stationConstants.SEARCH_NEAREST_SERVICE_STATIONS_REQUEST:
          return {
            ...state,
            loading: true,
            loaded: false
          };
        case stationConstants.SEARCH_NEAREST_SERVICE_STATIONS_SUCCESS:
          return {
            ...state,
            stations: action.stations,
            loading: false,
            loaded: true
          };
        case stationConstants.SEARCH_NEAREST_SERVICE_STATIONS_FAILURE:
          return { 
            error: action.error,
            loading: false,
            loaded: false
          };
    case stationConstants.GET_USER_STATIONS_REQUEST:
      return {
        ...state,
        loading:true
      };
    case stationConstants.GET_USER_STATIONS_SUCCESS:
      return {
        ...state,
        stations: action.stations.map((_station) => { return {..._station}}),
        loading: false
      }
    case stationConstants.GET_USER_STATIONS_FAILURE:
      return {
        ...state,
        loading: false
      }
      case stationConstants.GET_STATION_BY_CODE_REQUEST:
        return {
          ...state,
          stationError: '',
          selectedStation: {},
          loadingStation:true
        };
      case stationConstants.GET_STATION_BY_CODE_SUCCESS:
        return {
          ...state,
          selectedStation: action.station,
          loadingStation: false
        }
      case stationConstants.GET_STATION_BY_CODE_FAILURE:
        return {
          ...state,
          stationError: action.error,
          selectedStation: {},
          loadingStation: false
        }
      case stationConstants.GET_FUEL_PRICE_BY_STATION_CODE_REQUEST:
        return {
          ...state,
          loadingPrice:true
        };
      case stationConstants.GET_FUEL_PRICE_BY_STATION_CODE_SUCCESS:
        return {
          ...state,
          fuelPrice: action.fuelPrice,
          loadingPrice: false
        }
      case stationConstants.GET_FUEL_PRICE_BY_STATION_CODE_FAILURE:
        return {
          ...state,
          loadingPrice: false
        }
    case stationConstants.SELECT_STATION:
      return {
        ...state,
        selectedStation: action._station
      }
    case stationConstants.ADD_STATION_REQUEST:
      return { 
          ...state, 
          addingStation: true 
        };
    case stationConstants.ADD_STATION_SUCCESS:
      return { 
          ...state, 
          addingStation: false, 
          addedStation: true, 
          stations: [...state.stations, {...action._station}]
        };
    case stationConstants.ADD_STATION_FAILURE:
      return { 
          ...state, 
          addingStation: false
        };
    case stationConstants.UPDATE_STATION_REQUEST:
      return { 
          ...state, 
          updatingStation: true,
          updatedStation:false 
        };
    case stationConstants.UPDATE_STATION_SUCCESS:
      return { 
          ...state, 
          updatingStation: false, 
          updatedStation: true, 
          stations: state.stations.map(_station =>
            _station.id === action._station.id
              ? action._station
              : _station
            )
        };
    case stationConstants.UPDATE_STATION_FAILURE:
      return { 
          ...state, 
          updatingStation: false
        };
    case stationConstants.DELETE_STATION_REQUEST:
      // add 'deleting:true' property to _station being deleted
      return {
        ...state,
        stations: state.stations.map(_station =>
          _station.id === action.id
            ? { ..._station, deleting: true }
            : _station
        )
      };
    case stationConstants.DELETE_STATION_SUCCESS:
      // remove deleted _station from state
      return {
        stations: state.stations.filter(_station => _station.id !== action.id)
      };
    case stationConstants.DELETE_STATION_FAILURE:
      // remove 'deleting:true' property and add 'deleteError:[error]' property to _station 
      return {
        ...state,
        stations: state.stations.map(_station => {
          if (_station.id === action.id) {
            // make copy of _station without 'deleting:true' property
            const { deleting, ...stationCopy } = _station;
            // return copy of _station with 'deleteError:[error]' property
            return { ...stationCopy, deleteError: action.error };
          }

          return _station;
        })
      };
    case stationConstants.CLEAR:
      return {
        ...state,
        addedStation: false,
        addedStation: false,
        addingStation: false,
        fuelPrice: {},
        loading: false,
        loaded: false,
        stationError: '',
        loadingStation: false,
        selectedStation: {}
      }
    default:
      return state
  }
}