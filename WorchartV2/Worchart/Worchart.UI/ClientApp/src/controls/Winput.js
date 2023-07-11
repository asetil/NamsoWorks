import React, { Component } from 'react';
import RS from '../service/LanguageMapService';
import WCheckbox from './WCheckbox';
import WFileUpload from './WFileUpload';

export default class Winput extends Component {
    constructor(props) {
        super(props);

        this.inputRef = React.createRef();

        this.state = {
            valid: -1
        };
    }

    getValue() {
        return this.inputRef.current.value;
    }

    reset() {
        this.inputRef.current.value = "";
        this.setState({
            valid: -1
        });
    }

    isValid() {
        var value = this.inputRef.current.value;
        var valid = value && value.length > 0 ? true : false;
        this.setState({
            valid: valid ? 1 : 0
        });
        return valid;
    }

    renderInput() {
        var type = this.props.type;
        if (type == "select") {
            return <select ref={this.inputRef}>
                <option>Seçiniz</option>
            </select>
        }
        else if (type == "file") {
            return <WFileUpload ref={this.inputRef} {...this.props} />
        }
        else if (type == "textarea") {
            return <textarea ref={this.inputRef} className={this.props.className + " form-control"} name={this.props.id} id={this.props.id} maxLength={this.props.maxLength}/>
        }
        else if (type == "checkbox") {
            return <WCheckbox ref={this.inputRef} className={this.props.className} name={this.props.id} id={this.props.id} title={this.props.title} />
        }
        return <input type={type} ref={this.inputRef} className={this.props.className + " form-control"} name={this.props.id} id={this.props.id} maxLength={this.props.maxLength} />
    }

    render() {
        return (
            this.props.plaine ? this.renderInput() :
                <div className={"form-group col-md-12 " + this.props.parentClass + (this.state.valid == 0 ? " error" : "")}>
                    {this.props.type != "checkbox" && <label>{RS.get(this.props.title)}</label>}
                    {this.renderInput()}
                    <span className={"validation " + (this.state.valid == 0 ? "" : "dn")}><i className="fa fa-info-circle"></i> {RS.get(this.props.message)}</span>
                </div>
        );
    }
}
