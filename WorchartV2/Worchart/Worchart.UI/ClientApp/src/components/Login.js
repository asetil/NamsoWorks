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
                <h1>�ye Giri�i</h1>

                <div>
                    <label>E-posta</label>
                    <input type="text" name="Email" placeholder="E-posta"/>
                </div>

                <div>
                    <label>�ifre</label>
                    <input type="password" name="Password" placeholder="�ifre" />
                </div>
                
                <button className="btn btn-primary" onClick={this.login}>Giri� Yap</button>
            </div>
        );
    }
}
