import { branchConstants } from '../constants/branch';

const initialState = {
    addingBranch: false,
    addedBranch: false,
    loading: false,
    selectedBranch: {
      code: 0,
      branchName: 'Select'
    },
    branches: [],
  }

export function branches(state = initialState, action) { 
  switch (action.type) {
    case branchConstants.GET_ALL_BRANCHES_REQUEST:
      return {
        ...state,
        loading: true
      };
    case branchConstants.GET_ALL_BRANCHES_SUCCESS:
      return {
        ...state,
        branches: action.branches
      };
    case branchConstants.GET_ALL_BRANCHES_FAILURE:
      return { 
        error: action.error
      };
      case branchConstants.GET_BANK_BRANCHES_REQUEST:
        return {
          ...state,
          loading: true
        };
      case branchConstants.GET_BANK_BRANCHES_SUCCESS:
        return {
          ...state,
          branches: action.branches
        };
      case branchConstants.GET_BANK_BRANCHES_FAILURE:
        return { 
          error: action.error
        };
    case branchConstants.GET_USER_BRANCHES_REQUEST:
      return {
        ...state,
        loading:true
      };
    case branchConstants.GET_USER_BRANCHES_SUCCESS:
      return {
        ...state,
        branches: action.branches.map((_branch) => { return {..._branch}}),
        loading: false
      }
    case branchConstants.GET_USER_BRANCHES_FAILURE:
      return {
        ...state,
        loading: false
      }
    case branchConstants.SELECT_BRANCH:
      return {
        ...state,
        selectedBranch: action._branch
      }
    case branchConstants.ADD_BRANCH_REQUEST:
      return { 
          ...state, 
          addingBranch: true 
        };
    case branchConstants.ADD_BRANCH_SUCCESS:
      return { 
          ...state, 
          addingBranch: false, 
          addedBranch: true, 
          branches: [...state.branches, {...action._branch}]
        };
    case branchConstants.ADD_BRANCH_FAILURE:
      return { 
          ...state, 
          addingBranch: false
        };
    case branchConstants.UPDATE_BRANCH_REQUEST:
      return { 
          ...state, 
          updatingBranch: true,
          updatedBranch:false 
        };
    case branchConstants.UPDATE_BRANCH_SUCCESS:
      return { 
          ...state, 
          updatingBranch: false, 
          updatedBranch: true, 
          branches: state.branches.map(_branch =>
            _branch.id === action._branch.id
              ? action._branch
              : _branch
            )
        };
    case branchConstants.UPDATE_BRANCH_FAILURE:
      return { 
          ...state, 
          updatingBranch: false
        };
    case branchConstants.DELETE_BRANCH_REQUEST:
      // add 'deleting:true' property to _branch being deleted
      return {
        ...state,
        branches: state.branches.map(_branch =>
          _branch.id === action.id
            ? { ..._branch, deleting: true }
            : _branch
        )
      };
    case branchConstants.DELETE_BRANCH_SUCCESS:
      // remove deleted _branch from state
      return {
        branches: state.branches.filter(_branch => _branch.id !== action.id)
      };
    case branchConstants.DELETE_BRANCH_FAILURE:
      // remove 'deleting:true' property and add 'deleteError:[error]' property to _branch 
      return {
        ...state,
        branches: state.branches.map(_branch => {
          if (_branch.id === action.id) {
            // make copy of _branch without 'deleting:true' property
            const { deleting, ...branchCopy } = _branch;
            // return copy of _branch with 'deleteError:[error]' property
            return { ...branchCopy, deleteError: action.error };
          }

          return _branch;
        })
      };
    case branchConstants.CLEAR:
      return {
        ...state,
        addedBranch: false,
        addedBranch: false,
        addingBranch: false,
        loading: false
      }
    default:
      return state
  }
}