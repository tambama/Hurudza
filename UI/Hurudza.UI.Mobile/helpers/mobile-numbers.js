export async function getNetwork(mobileNumber){
    var number = mobileNumber.trim().replace(' ', '');
    if (await isEconet(number))
        return 1;

    if(await isNetone(number))
        return 2;

    if(await isTelecel(number))
        return 3;

    return 0;
}

export async function isEconet(val){
    return (new RegExp(/^((00|\+)?(263))?0?7(7|8)[0-9]{7}$/, 'g').test(val))
}

export async function isNetone(val){
    return (new RegExp(/^((00|\+)?(263))?0?71[0-9]{7}$/, 'g').test(val))
}

export async function isTelecel(val){
    return (new RegExp(/^((00|\+)?(263))?0?73[0-9]{7}$/, 'g').test(val))
}