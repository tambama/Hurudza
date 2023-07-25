import {StyleSheet, Dimensions} from 'react-native';
import { Platform } from 'expo-core';
const screen = Dimensions.get("window");


const styles = StyleSheet.create({
    btndel: {
        height: 40,
        width: (screen.width - 140-20)/2,
        borderColor: "#000",
        backgroundColor: "#fff",
        alignItems: "center",
        justifyContent: "center",

    },
    text: {
        color: "#4d65f5"
    },
     textInput: {
         height: 40,
         paddingLeft: 6,
         width: screen.width - 140
     },
    container: {
        flex: 1
    },
    item: {
        backgroundColor: "#fff",
        alignItems: "center",
        flex: 1,
        margin: 7,
        height: 150,
        backgroundColor: "#fff",
    },
    addItem: {
        backgroundColor: "#4d65f5",
        alignItems: "center",
        justifyContent: "center",
        flex: 1,
        marginVertical: '10%',
        width: "75%"
    },
    itemInvisible: {
        backgroundColor: "transparent"
    },
    walletModal: {
        justifyContent: 'center',
        alignItems: 'center',
        width: screen.width - 80,
        height: screen.height - 200,
        marginHorizontal: '10%',
        marginVertical: '10%',
        backgroundColor: '#fff',
        
    },
});

export default styles;