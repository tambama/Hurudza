import { requestConstants } from '../constants/request';

const initialState = {
    addingRequest: false,
    addedRequest: false,
    loading: false,
    requests: [],
    unreadCount: 0,
    selectedRequest: {
      id: 0
    },
    selecting: false,
    selected: false
  }

export function requests(state = initialState, action) { 
  switch (action.type) {
    case requestConstants.GET_ALL_REQUESTS_REQUEST:
      return {
        ...state,
        loading: true
      };
    case requestConstants.GET_ALL_REQUESTS_SUCCESS:
      return {
        ...state,
        requests: action.requests
      };
    case requestConstants.GET_ALL_REQUESTS_FAILURE:
      return { 
        error: action.error
      };
    case requestConstants.GET_USER_REQUESTS_REQUEST:
      return {
        ...state,
        loading:true
      };
    case requestConstants.GET_USER_REQUESTS_SUCCESS:
      return {
        ...state,
        requests: action.requests,
        loading: false
      }
    case requestConstants.GET_USER_REQUESTS_FAILURE:
      return {
        ...state,
        loading: false
      }
      case requestConstants.GET_UNREAD_USER_REQUESTS_REQUEST:
        return {
          ...state,
        };
      case requestConstants.GET_UNREAD_USER_REQUESTS_SUCCESS:
        return {
          ...state,
          unreadCount: action.unreadCount,
        }
      case requestConstants.GET_UNREAD_USER_REQUESTS_FAILURE:
        return {
          ...state,
        }
      case requestConstants.SELECT_REQUEST_REQUEST:
        return {
          ...state,
          selecting: true,
          selected: false
        };
      case requestConstants.SELECT_REQUEST_SUCCESS:
        return {
          ...state,
          selectedRequest: action._request,
          selecting: false,
          selected: true,
          requests: state.requests.map(_request =>
            _request.id === action._request.id
              ? action._request
              : _request
            ),
        };
      case requestConstants.SELECT_REQUEST_FAILURE:
        return { 
          error: action.error,
          selecting: false,
          selected: false
        };
    case requestConstants.ADD_REQUEST_REQUEST:
      return { 
          ...state, 
          addingRequest: true 
        };
    case requestConstants.ADD_REQUEST_SUCCESS:
      return { 
          ...state, 
          addingRequest: false, 
          addedRequest: true, 
          requests: [...state.requests, {...action._request}]
        };
    case requestConstants.ADD_REQUEST_FAILURE:
      return { 
          ...state, 
          addingRequest: false
        };
    case requestConstants.UPDATE_REQUEST_REQUEST:
      return { 
          ...state, 
          updatingRequest: true,
          updatedRequest:false 
        };
    case requestConstants.UPDATE_REQUEST_SUCCESS:
      return { 
          ...state, 
          selected: false,
          updatingRequest: false, 
          updatedRequest: true, 
          requests: state.requests.map(_request =>
            _request.id === action._request.id
              ? action._request
              : _request
            ).filter(r => r.granted === false),
          selectedRequest: action._request
        };
    case requestConstants.UPDATE_REQUEST_FAILURE:
      return { 
          ...state, 
          updatingRequest: false
        };
    case requestConstants.DELETE_REQUEST_REQUEST:
      // add 'deletingRequest:true' property to _request being deletedRequest
      return {
        ...state,
        deletingRequest: true,
        deletedRequest: false
      };
    case requestConstants.DELETE_REQUEST_SUCCESS:
      // remove deletedRequest _request from state
      return {
        requests: state.requests.filter(_request => _request.id !== action.id),
        deletingRequest: false,
        deletedRequest: true
      };
    case requestConstants.DELETE_REQUEST_FAILURE:
      // remove 'deletingRequest:true' property and add 'deleteError:[error]' property to _request 
      return {
        ...state,
        deletingRequest: false,
        deletedRequest: false
      };
    case requestConstants.CLEAR:
      return {
        ...state,
        addedRequest: false,
        addingRequest: false,
        deletingRequest: false,
        deletedRequest: false,
        selected:false,
        loading: false,
        updatingRequest: false,
        updatedRequest: false
      }
    default:
      return state
  }
}