import React, { Component, useState } from 'react';
import RS from '../service/LanguageMapService';

function WCheckbox(props) {
    var [checked, setChecked] = useState(props.value || false);
    
    return (
        <span className="w-checkbox" onClick={() => { setChecked(!checked) }}>
            <i className={checked ? "fa fa-check-square" : "far fa-square"}></i> {RS.get(props.title)}
        </span>
    );
}

export default WCheckbox;
