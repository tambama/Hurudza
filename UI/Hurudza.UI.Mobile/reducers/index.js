import { combineReducers } from 'redux';

import { authentication } from './authentication';
import { registration } from './registration';
import { otp } from './otp';
import { user } from './user';
import { requests } from './request';
import { alert } from './alert';
import { omcs } from './omc';
import { stations } from './station';
import { wallets } from './wallet';
import { zesa } from './zesa';
import { coupons } from './coupon';
import { banks } from './bank';
import { branches } from './branch';
import { bankCards } from './bankCard';
import { location } from './location';
import { currencies } from './currency';
import { products } from './product';
import { couponPrices } from './couponPrice';
import { transactions } from './transaction'
import { payments } from './payment';
import themes from './themes';

import { signup } from './signup';

export default combineReducers({
    currencies,
    products,
    couponPrices,
    user,
    authentication,
    alert,
    registration,
    otp,
    omcs,
    stations,
    wallets,
    zesa,
    coupons,
    requests,
    banks,
    branches,
    bankCards,
    location,
    transactions,
    themes,
    payments,
    signup
});