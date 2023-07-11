import React, { Component } from 'react';
import * as LoginActions from "../falanx/LoginActions";
import FirmService from '../service/FirmService';
import LoginService from "../service/LoginService";
import { Home } from './Home';
import TeamView from './TeamView';
import WForm from '../controls/WForm';
import WModal from '../controls/WModal';

export class FirmTeam extends Component {
    static displayName = FirmTeam.name;
    constructor(props) {
        super(props);
        
        this.state = {
            teams: [],
            selectedTeam: undefined,
            loginState: LoginService.getLoginState(),
            loaded: false
        };
        this.newTeam = { mode: "edit", companyID: this.state.loginState.firmId };
    }

    componentDidMount() {
        this.loadTeams(false);
    }

    componentDidUpdate() {
        this.loadTeams(false);
    }

    loadTeams(refresh) {
        if (!this.state.loaded || refresh) {
            if (!this.state.loginState.loggedIn) {
                LoginActions.showLogin("MSG.AUTHORIZATIONFAILED");
                return;
            }

            var loginState = this.state.loginState;
            FirmService.getTeams(loginState.firmId).then(teams => {
                if (teams) {
                    this.setState({ ...this.state, teams: teams, loaded: true });
                }
            });
        }
    }

    setSelectedTeam(team) {
        this.setState({ ...this.state, selectedTeam: team });
    }

    render() {
        if (!this.state.loginState.loggedIn) {
            return <Home />
        }

        var teamList = this.state.teams.map(team => <div key={team.id} className="col-md-4 wow fadeInUp">
            <div className="about-col team">
                <div className="img">
                    <img src={`/res/img/${team.logo || "team/0.jpg"}`} alt={team.name} className="img-fluid" />
                    <div className="icon"><i className="ion-ios-speedometer-outline"></i></div>
                </div>
                <h2 className="title"><a href="javascript:void(0)" onClick={() => this.setSelectedTeam(team)}>{team.name}</a></h2>
                <p>Projelerinizi Yönetin, Yeni Proje Oluşturun </p>
            </div>
        </div>);

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
                                <button className="btn btn-new" onClick={() => this.setSelectedTeam(this.newTeam)}><i className="fa fa-plus"></i> Yeni Ekip Oluştur</button>
                            </div>
                            {teamList}
                        </div>
                    </div>
                </section>

                <WModal className="team-modal" title="LBL.SAVETEAM" open={this.state.selectedTeam}>
                    <TeamView team={this.state.selectedTeam} />
                </WModal>
            </React.Fragment>
        );
    }
}
