import React, { Component } from 'react';
import LoginService from "../service/LoginService";
import WForm from '../controls/WForm';

export class Register extends Component {
    static displayName = Register.name;
    constructor(props) {
        super(props);

        this.form = {
            id: "register",
            model: {},
            items: [
                { type: "text", title: "LBL.NAME", field: "name", maxLength: 50, required: true },
                { type: "text", title: "LBL.EMAIL", field: "email", maxLength: 50, required: true },
                { type: "password", title: "LBL.PASSWORD", field: "password", maxLength: 20, required: true },
                { type: "password", title: "LBL.REPEATPASSWORD", field: "repeatpsw", maxLength: 20, required: true },
            ],
            onSubmit: this.onSave
        };

        this.state = {
            loginState: LoginService.getLoginState(),
        };
    }

    onSave(form) {
        console.log(form);
    }

    render() {
        return (
            <React.Fragment>
                <section id="call-to-action" className="wow fadeIn">
                    <div className="container text-center">
                    </div>
                </section>
                <section id="about">
                    <div className="container">
                        <div className="row about-cols">
                            <div className="col-md-12 wow fadeInUp">
                                <WForm title="LBL.REGISTERPAGE" form={this.form} className="register-form" onSubmit={(form) => { console.log(form); }} onCancel={() => { }} />
                            </div>
                        </div>
                    </div>
                </section>
            </React.Fragment>
        );
    }
}
