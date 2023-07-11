import React, { Component } from 'react';
import RS from '../service/LanguageMapService';
import LoginService from '../service/LoginService';
import Winput from '../controls/Winput';
import { Link, Redirect, withRouter } from 'react-router-dom';
import { Collapse, Container, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import jQuery from 'jquery';
import WorchartStore from "../falanx/WorchartStore";
import * as LoginActions from "../falanx/LoginActions";
import * as DialogActions from "../falanx/DialogActions";

class Header extends Component {
    constructor(props) {
        super(props);

        this.form = {
            firmcode: React.createRef(),
            email: React.createRef(),
            password: React.createRef(),
        };

        this.state = WorchartStore.getLoginState();
        this.handleLogin = this.handleLogin.bind(this);
    }

    componentDidMount() {
        this.stateListener = WorchartStore.addListener(() => {
            this.setState(WorchartStore.getLoginState());
        });
        this.setState(WorchartStore.getLoginState());
    }

    toggleLogin(loginType) {
        this.form.email.current.reset();
        this.form.password.current.reset();
        this.form.firmcode.current.reset();
        LoginActions.setLoginType(loginType);
    }

    handleLogin(e) {
        e.preventDefault();

        DialogActions.toggleLoading();
        if (this.validateForm()) {
            var email = this.form.email.current.getValue();
            var password = this.form.password.current.getValue();
            var firmCode = this.state.loginType == 0 ? "" : this.form.firmcode.current.getValue();

            LoginService.handleLogin(email, password, firmCode).then(result => {
                DialogActions.toggleLoading();
                if (result && result.userToken) {
                    window.jQuery("#loginModal").modal("hide");
                    this.props.history.push("/firma");
                }
            });
        }
        else {
            DialogActions.toggleLoading();
        }
    }

    validateForm() {
        var isValid = this.form.email.current.isValid();
        isValid = this.form.password.current.isValid() && isValid;

        if (this.state.loginType == 1) {
            isValid = this.form.firmcode.current.isValid() && isValid;
        }
        return isValid;
    }

    render() {

        //if (this.state.isLoggedIn) {
        //    return <Redirect to='/firma' push="true" />
        //}

        return (
            <header id="header">
                <div className="container-fluid">
                    <div id="logo" className="pull-left">
                        <h1><a href="#intro" className="scrollto">Worchart</a></h1>
                        <a href="#intro"><img src="/res/img/logo.png" alt="" title="" /></a>
                    </div>

                    <nav id="nav-menu-container">
                        <ul className="nav-menu">
                            <li className=""><NavLink tag={Link} className="menu-active" to="/">Anasayfa</NavLink></li>
                            <li><a href="#about">About Us</a></li>
                            <li><NavLink tag={Link} to="/firma">Firma</NavLink></li>
                            <li><a href="#services">Services</a></li>
                            <li><a href="#portfolio">Portfolio</a></li>
                            <li><a href="#team">Team</a></li>
                            {
                                this.state.loggedIn ? <li><a href="javascript:void(0)">{this.state.name}</a></li>
                                    : <li><a href="#" className="btn-login" data-toggle="modal" data-target="#loginModal">{RS.get("LBL.LOGINTITLE")}</a></li>
                            }
                            <li className="menu-has-children">
                                <a href="">Drop Down</a>
                                <ul>
                                    <li><a href="#">Drop Down 1</a></li>
                                    <li><a href="#">Drop Down 3</a></li>
                                    <li><a href="#">Drop Down 4</a></li>
                                    <li><a href="#">Drop Down 5</a></li>
                                </ul>
                            </li>
                            <li><a href="#contact">Contact</a></li>
                        </ul>
                    </nav>
                </div>

                <div className="modal fade" id="loginModal" tabIndex="-1" role="dialog" aria-labelledby="loginModal" aria-hidden="true">
                    <div className="modal-dialog" role="document">
                        <div className="modal-content">
                            <div className="modal-header">
                                <h5 className="modal-title" id="exampleModalLabel">{RS.get("LBL.LOGINTITLE")}</h5>
                                <button type="button" className="close" data-dismiss="modal" aria-label="Close">
                                    <span aria-hidden="true">&times;</span>
                                </button>
                            </div>
                            <div className="modal-body">
                                <form action="" method="post" role="form" className="contactForm" onSubmit={this.handleLogin} >
                                    <div className="form-group col-md-12 tab-container text-center">
                                        <span className={this.state.loginType == 0 ? "btn-tab active" : "btn-tab"} onClick={() => this.toggleLogin(0)}>{RS.get("LBL.USERLOGIN")}</span>
                                        <span className={this.state.loginType == 0 ? "btn-tab" : "btn-tab active"} onClick={() => this.toggleLogin(1)}>{RS.get("LBL.COMPANYLOGIN")}</span>
                                    </div>

                                    {this.state.error && <p className="error-text text-center">{RS.get(this.state.error)}</p>}

                                    <Winput type="text" title="LBL.FIRMCODE" ref={this.form.firmcode} parentClass={this.state.loginType == 0 ? " dn" : ""} id="firmcode" message="MSG.INPUTREQUIRED" maxLength="20" />

                                    <Winput type="email" title="LBL.EMAIL" ref={this.form.email} id="email" message="MSG.ENTERVALIDEMAIL" maxLength="50" />

                                    <Winput type="password" title="LBL.PASSWORD" ref={this.form.password} id="password" message="MSG.INPUTREQUIRED" maxLength="20" />


                                    <div className="form-group col-md-12">
                                        { /*<label><input type="checkbox" name="rememberme" id="rememberme" /> {RS.get("LBL.REMEMBERME")}</label>*/}
                                        <Winput type="checkbox" title="LBL.REMEMBERME" ref={this.form.rememberme} id="rememberme" plaine="true" />
                                        <a href="#" className="fr">{RS.get("LBL.FORGOTPASSWORD")}</a>
                                    </div>
                                    <div className="form-group col-md-12 text-right">
                                        <button className="w100 btn btn-success">{RS.get("BTN.LOGIN")}</button>
                                    </div>
                                    <div className="form-group col-md-12 text-center">
                                        <span>{RS.get("MSG.REGISTER")}  <NavLink tag={Link} to="/uye-ol">{RS.get("LBL.REGISTER")}</NavLink></span>
                                    </div>
                                </form>
                            </div>
                            {/*
                             *
                             *  <div className="modal-footer">
                                <button type="button" className="btn btn-secondary" data-dismiss="modal">Close</button>
                                <button type="button" className="btn btn-primary">Save changes</button>
                            </div>
                             * 
                             * */}
                        </div>
                    </div>
                </div>
            </header>
        );
    }
}

export default withRouter(Header);