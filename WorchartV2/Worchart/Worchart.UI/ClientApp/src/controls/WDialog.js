import React, { Component } from 'react';
import WorchartStore from "../falanx/WorchartStore";
import RS from '../service/LanguageMapService';

export class WDialog extends Component {
    constructor(props) {
        super(props);
        this.state = WorchartStore.getDialogState();
    }

    componentDidMount() {
        this.stateListener = WorchartStore.addListener(() => {
            this.setState(WorchartStore.getDialogState());
        });
    }

    getIcon(type) {
        switch (type) {
            case 1: return "fa fa-check";
            case 2: return "fa fa-bug";
            case 3: return "fa fa-exclamation-triangle";
            case 4: return "fa fa-info-circle";
        }
        return "fa fa-question-circle";
    }

    render() {
        var toasterItems = this.state.toaster.map(i =>
            <span key={i.id} className={"toaster-item toaster-" + i.type}>
                <i className={this.getIcon(i.type)}></i> {i.message}
            </span>);

        return (
            <React.Fragment>
                <div className="aware-toaster">
                    {toasterItems}
                </div>
                {this.state.loading && <div className="page-loading"><span><i className="fa fa-cog fa-spin"></i> {RS.get("MSG.LOADING")}</span></div>}
            </React.Fragment>
        );
    }
}