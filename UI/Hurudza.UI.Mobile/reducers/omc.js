import { omcConstants } from '../constants/omc';

const initialState = {
    addingOmc: false,
    addedOmc: false,
    loading: false,
    selectedOmc: {
      id: 0,
      name: 'Select'
    },
    omcs: [],
    fuelPrice: {},
    omcFuelPrice: {}
  }

export function omcs(state = initialState, action) { 
  switch (action.type) {
    case omcConstants.GET_ALL_OMCS_REQUEST:
      return {
        ...state,
        loading: true
      };
    case omcConstants.GET_ALL_OMCS_SUCCESS:
      return {
        ...state,
        omcs: action.omcs
      };
    case omcConstants.GET_ALL_OMCS_FAILURE:
      return { 
        error: action.error
      };
      case omcConstants.GET_OMCS_WITH_COUPON_PRICES_REQUEST:
        return {
          ...state,
          loading: true
        };
      case omcConstants.GET_OMCS_WITH_COUPON_PRICES_SUCCESS:
        return {
          ...state,
          omcs: action.omcs
        };
      case omcConstants.GET_OMCS_WITH_COUPON_PRICES_FAILURE:
        return { 
          error: action.error
        };
        case omcConstants.GET_OMC_FUEL_PRICE_REQUEST:
          return {
            ...state,
            requestingPrice: true,
            fuelPrice: {},
            omcFuelPrice: {}
          };
        case omcConstants.GET_OMC_FUEL_PRICE_SUCCESS:
          return {
            ...state,
            requestingPrice: false,
            requestedPrice: true,
            fuelPrice: action.price,
            omcFuelPrice: action.price
          };
        case omcConstants.GET_OMC_FUEL_PRICE_FAILURE:
          return { 
            error: action.error,
            requestingPrice: false,
            fuelPrice: {},
            omcFuelPrice: {}
          };
    case omcConstants.GET_USER_OMCS_REQUEST:
      return {
        ...state,
        loading:true
      };
    case omcConstants.GET_USER_OMCS_SUCCESS:
      return {
        ...state,
        omcs: action.omcs.map((_omc) => { return {..._omc}}),
        loading: false
      }
    case omcConstants.GET_USER_OMCS_FAILURE:
      return {
        ...state,
        loading: false
      }
    case omcConstants.SELECT_OMC:
      return {
        ...state,
        selectedOmc: action._omc
      }
    case omcConstants.ADD_OMC_REQUEST:
      return { 
          ...state, 
          addingOmc: true 
        };
    case omcConstants.ADD_OMC_SUCCESS:
      return { 
          ...state, 
          addingOmc: false, 
          addedOmc: true, 
          omcs: [...state.omcs, {...action._omc}]
        };
    case omcConstants.ADD_OMC_FAILURE:
      return { 
          ...state, 
          addingOmc: false
        };
    case omcConstants.UPDATE_OMC_REQUEST:
      return { 
          ...state, 
          updatingOmc: true,
          updatedOmc:false 
        };
    case omcConstants.UPDATE_OMC_SUCCESS:
      return { 
          ...state, 
          updatingOmc: false, 
          updatedOmc: true, 
          omcs: state.omcs.map(_omc =>
            _omc.id === action._omc.id
              ? action._omc
              : _omc
            )
        };
    case omcConstants.UPDATE_OMC_FAILURE:
      return { 
          ...state, 
          updatingOmc: false
        };
    case omcConstants.DELETE_OMC_REQUEST:
      // add 'deleting:true' property to _omc being deleted
      return {
        ...state,
        omcs: state.omcs.map(_omc =>
          _omc.id === action.id
            ? { ..._omc, deleting: true }
            : _omc
        )
      };
    case omcConstants.DELETE_OMC_SUCCESS:
      // remove deleted _omc from state
      return {
        omcs: state.omcs.filter(_omc => _omc.id !== action.id)
      };
    case omcConstants.DELETE_OMC_FAILURE:
      // remove 'deleting:true' property and add 'deleteError:[error]' property to _omc 
      return {
        ...state,
        omcs: state.omcs.map(_omc => {
          if (_omc.id === action.id) {
            // make copy of _omc without 'deleting:true' property
            const { deleting, ...omcCopy } = _omc;
            // return copy of _omc with 'deleteError:[error]' property
            return { ...omcCopy, deleteError: action.error };
          }

          return _omc;
        })
      };
    case omcConstants.CLEAR:
      return {
        ...state,
        addedOmc: false,
        addedOmc: false,
        addingOmc: false,
        loading: false,
        fuelPrice: {},
        omcFuelPrice: {}
      }
    default:
      return state
  }
}