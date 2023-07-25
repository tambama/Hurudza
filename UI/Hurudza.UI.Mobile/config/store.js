import { createStore, applyMiddleware } from 'redux';
import logger from 'redux-logger';
import thunkMiddleware from 'redux-thunk'
import reducers from '../reducers';

const middleware = [];
middleware.push(thunkMiddleware);
if(process.env.NODE_ENV === 'development'){
    //middleware.push(logger);
}

const store = createStore(reducers, applyMiddleware(...middleware));

export default store;