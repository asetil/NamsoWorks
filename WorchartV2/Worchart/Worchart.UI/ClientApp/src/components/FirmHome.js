import React, { Component } from 'react';
import * as LoginActions from "../falanx/LoginActions";
import LoginService from "../service/LoginService";
import FirmService from "../service/FirmService";
import { Home } from './Home';
import { Link } from 'react-router-dom';
import { NavLink } from 'reactstrap';

export class FirmHome extends Component {
    static displayName = FirmHome.name;
    constructor(props) {
        super(props);

        this.state = {
            company: {},
            loginState: LoginService.getLoginState(),
            load : false
        };
    }

    componentDidMount() {
        if (!this.state.loginState.loggedIn) {
            LoginActions.showLogin("MSG.AUTHORIZATIONFAILED");
            return;
        }

        var loginState = this.state.loginState;
        FirmService.getCompany(this.state.loginState.firmId).then((company) => {
            this.setState({ company: company, loginState, load:true });
        });
    }

    componentDidUpdate() {
        if (!this.state.load) {
            var loginState = this.state.loginState;
            FirmService.getCompany(loginState.firmId).then((company) => {
                this.setState({ company: company, loginState, load: true });
            });
        }
    }

    render() {
        if (!this.state.loginState.loggedIn) {
            return <Home />
        }

        return (
            <React.Fragment>
                <section id="call-to-action" className="wow fadeIn">
                    <div className="container text-center">
                    </div>
                </section>
                <section id="about">
                    <div className="container">
                        <div className="row about-cols">
                            <div className="firm-info col-md-12">{this.state.company.name}</div>
                            <div className="col-md-4 wow fadeInUp">
                                <div className="about-col">
                                    <div className="img">
                                        <img src="/res/img/about-mission.jpg" alt="" className="img-fluid" />
                                        <div className="icon"><i className="ion-ios-speedometer-outline"></i></div>
                                    </div>
                                    <h2 className="title"><a href="#">Projelerim</a></h2>
                                    <p>
                                        Projelerinizi Yönetin, Yeni Proje Oluşturun
                            </p>
                                </div>
                            </div>

                            <div className="col-md-4 wow fadeInUp" data-wow-delay="0.1s">
                                <div className="about-col">
                                    <div className="img">
                                        <img src="/res/img/about-plan.jpg" alt="" className="img-fluid" />
                                        <div className="icon"><i className="ion-ios-list-outline"></i></div>
                                    </div>
                                    <h2 className="title"><NavLink tag={Link} to={'/firma/ekip/' + this.state.loginState.firmId}>Ekibim</NavLink></h2>
                                    <p>
                                        Ekibinizi ve rollerini yönetin.
                            </p>
                                </div>
                            </div>

                            <div className="col-md-4 wow fadeInUp" data-wow-delay="0.2s">
                                <div className="about-col">
                                    <div className="img">
                                        <img src="/res/img/about-vision.jpg" alt="" className="img-fluid" />
                                        <div className="icon"><i className="ion-ios-eye-outline"></i></div>
                                    </div>
                                    <h2 className="title"><a href="#">Firma Bilgileri</a></h2>
                                    <p>
                                        Firma bilgilerinizi görüntüleyin.</p>
                                </div>
                            </div>

                        </div>

                    </div>
                </section>
            </React.Fragment>
        );
    }
}
