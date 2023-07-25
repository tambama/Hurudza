import React from 'react';
import PropTypes from 'prop-types';
import { View, Text, TouchableHighlight } from 'react-native';
import { MontserratText } from '../StyledText'

import styles from './styles';
import Icon from './Icon';

const ListItem = ({text, subTitle, onPress, selected, checkmark = true, visible = true, hasSubTitle = false, customIcon = null, iconBackground}) => (
    <TouchableHighlight onPress={onPress} underlayColor={styles.$underlayColor}>
        <View style={styles.row}>
            <View>
                <MontserratText style={styles.text}>{text}</MontserratText>
                {hasSubTitle && (
                    <MontserratText style={styles.subText}>{subTitle}</MontserratText>
                )}
            </View>
            {selected ? <Icon checkmark={checkmark} visible={visible} iconBackground={iconBackground}/> : <Icon /> }
            {customIcon}
        </View>
    </TouchableHighlight>
);

ListItem.propTypes = {
    text: PropTypes.string,
    subTitle: PropTypes.string,
    onPress: PropTypes.func,
    selected: PropTypes.bool,
    checkmark: PropTypes.bool,
    visible: PropTypes.bool,
    hasSubTitle: PropTypes.bool,
    customIcon: PropTypes.element,
    iconBackground: PropTypes.string,
};

export default ListItem