import React, { Component } from 'react';
import LoginService from '../service/LoginService';

export class Login extends Component {
    static displayName = Login.name;

    constructor(props) {
        super(props);
        this.state = { username: "", password: "" };
        this.login = this.login.bind(this);
    }

    login() {
        var service = new LoginService();
        service.login("", "").then(result => {

        });
    }

    render() {
        return (
            <div>
                <h1>Üye Giriþi</h1>

                <div>
                    <label>E-posta</label>
                    <input type="text" name="Email" placeholder="E-posta"/>
                </div>

                <div>
                    <label>Þifre</label>
                    <input type="password" name="Password" placeholder="Þifre" />
                </div>
                
                <button className="btn btn-primary" onClick={this.login}>Giriþ Yap</button>
            </div>
        );
    }
}
