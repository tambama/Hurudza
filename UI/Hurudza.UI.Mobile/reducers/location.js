import { locationConstants } from '../constants/location';

const initialState = {
    location: {
      name: '',
      address: '',
      city: '',
      country: ''
    },
    addingLocation: false,
    addedCountry: false,
    loading: false,
  }

export function location(state = initialState, action) { 
  switch (action.type) {
    case locationConstants.GETALL_REQUEST:
      return {
        ...state,
        loading: true
      };
    case locationConstants.GETALL_SUCCESS:
      return {
        ...state,
        locations: action.locations
      };
    case locationConstants.GETALL_FAILURE:
      return { 
        error: action.error
      };
    case locationConstants.GETUSERLOCATION_REQUEST:
      return {
        ...state,
        loading:true
      };
    case locationConstants.GETUSERLOCATION_SUCCESS:
      return {
        ...state,
        locations: action.locations.map((location, index) => index === 0 ? { ...location, default : true }: location)
      }
    case locationConstants.GETUSERLOCATION_FAILURE:
      return {
        error: action.error
      }
    case locationConstants.SELECT_LOCATION:
      return {
        ...state,
        selectedLocation: action.location
      }
    case locationConstants.PICK_LOCATION:
      return {
        ...state,
        pickedLocation: action.location
      }
    case locationConstants.GET_CURRENT_LOCATION:
      return {
        ...state,
        currentLocation: action.location
      }
    case locationConstants.ADD_REQUEST:
      return { 
          ...state, 
          addingLocation: true 
        };
    case locationConstants.ADD_SUCCESS:
      return { 
          ...state, 
          addingLocation: false, 
          addedLocation: true, 
          locations: [...state.locations, action.location]
        };
    case locationConstants.ADD_FAILURE:
      return { 
          ...state, 
          addingLocation: false
        };
    case locationConstants.DELETE_REQUEST:
      // add 'deleting:true' property to location being deleted
      return {
        ...state,
        locations: state.locations.map(location =>
          location.id === action.id
            ? { ...location, deleting: true }
            : location
        )
      };
    case locationConstants.DELETE_SUCCESS:
      // remove deleted location from state
      return {
        locations: state.locations.filter(location => location.id !== action.id)
      };
    case locationConstants.DELETE_FAILURE:
      // remove 'deleting:true' property and add 'deleteError:[error]' property to location 
      return {
        ...state,
        locations: state.locations.map(location => {
          if (location.id === action.id) {
            // make copy of location without 'deleting:true' property
            const { deleting, ...locationCopy } = location;
            // return copy of location with 'deleteError:[error]' property
            return { ...locationCopy, deleteError: action.error };
          }

          return location;
        })
      };
    case locationConstants.CLEAR:
      return {
        ...state,
        addedLocation: false,
        addedCountry: false,
        addingLocation: false,
        loading: false
      }
    default:
      return state
  }
}